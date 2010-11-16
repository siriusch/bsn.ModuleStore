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
//  
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

using bsn.ModuleStore.Mapper;
using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	public class DatabaseInventory: Inventory {
		public class XsltUtils {
			public string AsText(XPathNavigator nav) {
				return nav.InnerXml;
			}
		}

		private static readonly XslCompiledTransform scripter;
		private static readonly string userObjectList;

		static DatabaseInventory() {
			using (Stream stream = typeof(DatabaseInventory).Assembly.GetManifestResourceStream(typeof(DatabaseInventory), "UserObjectList.sql")) {
				Debug.Assert(stream != null);
				using (StreamReader reader = new StreamReader(stream)) {
					userObjectList = reader.ReadToEnd();
				}
			}
			scripter = new XslCompiledTransform(Debugger.IsAttached);
			using (Stream stream = typeof(DatabaseInventory).Assembly.GetManifestResourceStream(typeof(DatabaseInventory), "UserObjectScripter.xslt")) {
				Debug.Assert(stream != null);
				using (XmlReader reader = XmlReader.Create(stream)) {
					scripter.Load(reader, new XsltSettings(false, false), null);
				}
			}
		}

		public static Exception CreateException(string message, SqlScriptableToken token) {
			StringWriter writer = new StringWriter();
			writer.WriteLine(message);
			token.WriteTo(new SqlWriter(writer));
			return new InvalidOperationException(writer.ToString());
		}

		private readonly string schemaName;

		public DatabaseInventory(ManagementConnectionProvider database, string schemaName) {
			if (database == null) {
				throw new ArgumentNullException("database");
			}
			this.schemaName = schemaName;
			using (SqlCommand command = database.GetConnection().CreateCommand()) {
				command.Transaction = database.GetTransaction();
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@sSchema", schemaName);
				command.CommandText = userObjectList;
				using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult)) {
					int definitionColumn = reader.GetOrdinal("xDefinition");
					XsltArgumentList arguments = new XsltArgumentList();
					arguments.AddExtensionObject("urn:utils", new XsltUtils());
					StringBuilder builder = new StringBuilder(65520);
					while (reader.Read()) {
						builder.Length = 0;
						using (StringWriter writer = new StringWriter(builder)) {
							SqlXml xml = reader.GetSqlXml(definitionColumn);
							scripter.Transform(xml.CreateReader(), arguments, writer);
							TraceXmlToSql(xml, writer);
						}
						try {
							try {
								using (StringReader scriptReader = new StringReader(builder.ToString())) {
									ProcessSingleScript(scriptReader, statement => {
									                                  	throw CreateException("Cannot process statement:", statement);
									                                  });
								}
							} catch (ParseException ex) {
								ex.FileName = reader.GetString(reader.GetOrdinal("sName"));
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

		public IEnumerable<string> GenerateUninstallSql() {
			StringBuilder buffer = new StringBuilder(512);
			SetQualification(SchemaName);
			try {
				DependencyResolver resolver = new DependencyResolver();
				foreach (CreateStatement statement in Objects) {
					resolver.Add(statement);
				}
				foreach (CreateStatement statement in resolver.GetInOrder(true).Reverse()) {
					yield return WriteStatement(statement.CreateDropStatement(), false, buffer);
				}
				buffer.Length = 0;
				using (TextWriter writer = new StringWriter(buffer)) {
					SqlWriter sqlWriter = new SqlWriter(writer);
					sqlWriter.Write("DROP SCHEMA ");
					new SchemaName(SchemaName).WriteTo(sqlWriter);
				}
				yield return buffer.ToString();
			} finally {
				UnsetQualification();
			}
		}

		[Conditional("DEBUG")]
		private void TraceXmlToSql(SqlXml xml, StringWriter writer) {
			Trace.WriteLine(xml.Value, "Input XML");
			Trace.WriteLine(writer.ToString(), "Output SQL");
		}
	}
}