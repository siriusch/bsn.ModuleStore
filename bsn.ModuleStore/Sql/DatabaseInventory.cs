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

using Common.Logging;

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

		private static readonly ILog log = LogManager.GetLogger<DatabaseInventory>();
		private static readonly ICollection<Type> objectsToRename = new HashSet<Type> {
				typeof(CreateFunctionStatement),
				typeof(CreateProcedureStatement),
				typeof(CreateTriggerStatement)
		};
		private static readonly XslCompiledTransform scripter = LoadTransform("UserObjectScripter.xslt");
		private static readonly XslCompiledTransform userObjectList = LoadTransform("UserObjectList.xslt");

		internal static Exception CreateException(string message, SqlScriptableToken token, DatabaseEngine targetEngine) {
			StringWriter writer = new StringWriter();
			writer.WriteLine(message);
			token.WriteTo(new SqlWriter(writer, targetEngine));
			return new InvalidOperationException(writer.ToString());
		}

		private static XslCompiledTransform LoadTransform(string embeddedResourceName) {
			XslCompiledTransform transform = new XslCompiledTransform(Debugger.IsAttached);
			using (Stream stream = typeof(DatabaseInventory).Assembly.GetManifestResourceStream(typeof(DatabaseInventory), embeddedResourceName)) {
				Debug.Assert(stream != null);
				using (XmlReader reader = XmlReader.Create(stream)) {
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
				throw new ArgumentNullException("database");
			}
			targetEngine = database.Engine;
			this.schemaName = schemaName;
			XsltArgumentList arguments = CreateArguments(database);
			using (SqlCommand command = database.GetConnection().CreateCommand()) {
				command.Transaction = database.GetTransaction();
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@sSchema", schemaName);
				using (StringWriter writer = new StringWriter()) {
					userObjectList.Transform(new XDocument().CreateReader(), arguments, writer);
					command.CommandText = Regex.Replace(writer.ToString(), @"\s+", " ");
					// ReSharper disable AccessToDisposedClosure
					log.Trace(l => l(writer.ToString()));
					// ReSharper restore AccessToDisposedClosure
				}
				using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult)) {
					int definitionColumn = reader.GetOrdinal("xDefinition");
					int nameColumn = reader.GetOrdinal("sName");
					StringBuilder builder = new StringBuilder(65520);
					while (reader.Read()) {
						builder.Length = 0;
						using (StringWriter writer = new StringWriter(builder)) {
							SqlXml xml = reader.GetSqlXml(definitionColumn);
							scripter.Transform(xml.CreateReader(), arguments, writer);
							// ReSharper disable AccessToDisposedClosure
							log.Trace(l => l("Object name: {0}\n  XML: {1}\n  SQL: {2}", reader.GetString(nameColumn), xml.Value, writer));
							// ReSharper restore AccessToDisposedClosure
						}
						try {
							try {
								using (StringReader scriptReader = new StringReader(builder.ToString())) {
									CreateStatement objectStatement = ProcessSingleScript(scriptReader, statement => {
										log.Error(l => l("Cannot process statement error: {0}", statement));
										throw CreateException("Cannot process statement:", statement, TargetEngine);
									}).SingleOrDefault(statement => objectsToRename.Any(t => t.IsAssignableFrom(statement.GetType())));
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

		public string SchemaName {
			get {
				return schemaName;
			}
		}

		public DatabaseEngine TargetEngine {
			get {
				return targetEngine;
			}
		}

		public IEnumerable<string> GenerateUninstallSql() {
			StringBuilder buffer = new StringBuilder(512);
			SetQualification(SchemaName);
			try {
				DependencyResolver resolver = new DependencyResolver();
				foreach (IAlterableCreateStatement statement in Objects.SelectMany(o => o.CreateStatementFragments(CreateFragmentMode.Alter))) {
					resolver.Add(statement);
				}
				foreach (IAlterableCreateStatement statement in resolver.GetInOrder(true).Where(s => !(s is CreateIndexStatement)).Reverse()) {
					AlterTableAddConstraintFragment addConstraint = statement as AlterTableAddConstraintFragment;
					if ((addConstraint == null) || !(addConstraint.Constraint is TableUniqueConstraintBase)) {
						yield return WriteStatement(statement.CreateDropStatement(), buffer, TargetEngine);
					}
				}
				if (!schemaName.Equals("dbo", StringComparison.OrdinalIgnoreCase)) {
					buffer.Length = 0;
					using (TextWriter writer = new StringWriter(buffer)) {
						SqlWriter sqlWriter = new SqlWriter(writer, TargetEngine);
						sqlWriter.Write("DROP SCHEMA ");
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
				throw new ArgumentNullException("createStatement");
			}
			createStatement.ObjectSchema = schemaName;
			base.AddObject(createStatement);
		}

		private XsltArgumentList CreateArguments(ManagementConnectionProvider database) {
			XsltArgumentList arguments = new XsltArgumentList();
			arguments.AddExtensionObject("urn:utils", new XsltUtils());
			arguments.AddParam("engine", "", targetEngine.ToString());
			arguments.AddParam("azure", "", targetEngine == DatabaseEngine.SqlAzure);
			arguments.AddParam("version", "", database.EngineVersion.Major);
			return arguments;
		}
	}
}
