// bsn ModuleStore database versioning
// -----------------------------------
// 
// Copyright 2010 by Arsène von Wyss - avw@gmx.ch
// 
// Development has been supported by Sirius Technologies AG, Basel
// 
// Source:
// 
// https://bsn-modulestore.googlecode.com/hg/
// 
// License:
// 
// The library is distributed under the GNU Lesser General Public License:
// http://www.gnu.org/licenses/lgpl.html
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;

using NLog;

using bsn.ModuleStore.Mapper;
using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	public class DatabaseInventory: Inventory {
		private class XsltUtils {
			// ReSharper disable UnusedMember.Local
			public string AsText(XPathNavigator nav) {
				return nav.InnerXml;
			}
			// ReSharper restore UnusedMember.Local
		}

		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		private static readonly ICollection<Type> objectsToRename = new HashSet<Type> {
				typeof(CreateFunctionStatement),
				typeof(CreateProcedureStatement),
				typeof(CreateTriggerStatement)
		};
		private static readonly XslCompiledTransform scripter = LoadTransform("UserObjectScripter.xslt");
		private static readonly XslCompiledTransform userObjectList = LoadTransform("UserObjectList.xslt");

		internal static Exception CreateException(string message, SqlScriptableToken token, DatabaseEngine targetEngine) {
			var writer = new StringWriter();
			writer.WriteLine(message);
			token.WriteTo(new SqlWriter(writer, targetEngine));
			return new InvalidOperationException(writer.ToString());
		}

		private static XslCompiledTransform LoadTransform(string embeddedResourceName) {
			var transform = new XslCompiledTransform(Debugger.IsAttached);
			using (var stream = typeof(DatabaseInventory).Assembly.GetManifestResourceStream(typeof(DatabaseInventory), embeddedResourceName)) {
				Debug.Assert(stream != null);
				using (var reader = XmlReader.Create(stream)) {
					transform.Load(reader, new XsltSettings(false, false), null);
				}
			}
			return transform;
		}

		private readonly string schemaName;
		private readonly DatabaseEngine targetEngine;

		public DatabaseInventory(ManagementConnectionProvider database, string schemaName)
				: base() {
			if (database == null) {
				throw new ArgumentNullException(nameof(database));
			}
			targetEngine = database.Engine;
			this.schemaName = schemaName;
			var arguments = CreateArguments(database);
			using (var command = database.GetConnection().CreateCommand()) {
				command.Transaction = database.GetTransaction();
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@sSchema", schemaName);
				using (var writer = new StringWriter()) {
					userObjectList.Transform(new XDocument().CreateReader(), arguments, writer);
					var commandText = writer.ToString();
					command.CommandText = Regex.Replace(commandText, @"\s+", " ");
					log.Trace("Command: {sql}", commandText);
				}
				using (var reader = command.ExecuteReader(CommandBehavior.SingleResult)) {
					var definitionColumn = reader.GetOrdinal("xDefinition");
					var nameColumn = reader.GetOrdinal("sName");
					var builder = new StringBuilder(65520);
					while (reader.Read()) {
						builder.Length = 0;
						using (var writer = new StringWriter(builder)) {
							var xml = reader.GetSqlXml(definitionColumn);
							scripter.Transform(xml.CreateReader(), arguments, writer);
							if (log.IsTraceEnabled) {
								log.Trace("Object name: {objectName}\n  XML: {xml}\n  SQL: {sql}", reader.GetString(nameColumn), xml.Value, builder.ToString());
							}
						}
						try {
							try {
								using (var scriptReader = new StringReader(builder.ToString())) {
									var objectStatement = ProcessSingleScript(scriptReader, statement => {
										if (log.IsErrorEnabled) {
											log.Error("Cannot process statement error: {statement}", statement.ToString());
										}
										throw CreateException("Cannot process statement:", statement, TargetEngine);
									}).SingleOrDefault(statement => objectsToRename.Any(t => t.IsInstanceOfType(statement)));
									if (objectStatement != null) {
										objectStatement.ObjectName = reader.GetString(nameColumn);
									}
								}
							} catch (ParseException ex) {
								ex.FileName = reader.GetString(nameColumn);
								throw;
							}
						} catch {
							Trace.WriteLine(builder.ToString());
							throw;
						}
					}
				}
			}
		}

		public string SchemaName => schemaName;

		public DatabaseEngine TargetEngine => targetEngine;

		public IEnumerable<string> GenerateUninstallSql() {
			var buffer = new StringBuilder(512);
			SetQualification(SchemaName);
			try {
				var resolver = new DependencyResolver();
				foreach (var statement in Objects.SelectMany(o => o.CreateStatementFragments(CreateFragmentMode.Alter))) {
					resolver.Add(statement);
				}
				foreach (IAlterableCreateStatement statement in resolver.GetInOrder(true).Where(s => !(s is CreateIndexStatement)).Reverse()) {
					if ((!(statement is AlterTableAddConstraintFragment addConstraint)) || !(addConstraint.Constraint is TableUniqueConstraintBase)) {
						yield return WriteStatement(statement.CreateDropStatement(), buffer, TargetEngine);
					}
				}
				if (!schemaName.Equals("dbo", StringComparison.OrdinalIgnoreCase)) {
					buffer.Length = 0;
					using (TextWriter writer = new StringWriter(buffer)) {
						var sqlWriter = new SqlWriter(writer, TargetEngine);
						sqlWriter.WriteKeyword("DROP SCHEMA ");
						new SchemaName(SchemaName).WriteTo(sqlWriter);
					}
					yield return buffer.ToString();
				}
			} finally {
				UnsetQualification();
			}
		}

		protected override void AddObject(CreateStatement createStatement) {
			if (createStatement == null) {
				throw new ArgumentNullException(nameof(createStatement));
			}
			createStatement.ObjectSchema = schemaName;
			base.AddObject(createStatement);
		}

		private XsltArgumentList CreateArguments(ManagementConnectionProvider database) {
			var arguments = new XsltArgumentList();
			arguments.AddExtensionObject("urn:utils", new XsltUtils());
			arguments.AddParam("engine", "", targetEngine.ToString());
			arguments.AddParam("azure", "", targetEngine == DatabaseEngine.SqlAzure);
			arguments.AddParam("version", "", database.EngineVersion.Major);
			return arguments;
		}
	}
}
