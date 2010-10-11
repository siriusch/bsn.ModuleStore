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
using System.IO;
using System.Text;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	public abstract class InstallableInventory: Inventory {
		private readonly List<Statement> additionalSetupStatements = new List<Statement>();

		public IEnumerable<Statement> AdditionalSetupStatements {
			get {
				return additionalSetupStatements;
			}
		}

		public IEnumerable<string> GenerateInstallSql(string schemaName) {
			if (string.IsNullOrEmpty(schemaName)) {
				throw new ArgumentNullException("schemaName");
			}
			DependencyResolver resolver = new DependencyResolver();
			SetQualification(null);
			try {
				StringBuilder builder = new StringBuilder(4096);
				using (StringWriter writer = new StringWriter(builder)) {
					SqlWriter sqlWriter = new SqlWriter(writer);
					sqlWriter.Write("CREATE SCHEMA");
					sqlWriter.IncreaseIndent();
					sqlWriter.WriteScript(new SchemaName(schemaName), WhitespacePadding.SpaceBefore);
					foreach (CreateStatement statement in Objects) {
						if ((statement is CreateTableStatement) || (statement is CreateViewStatement)) {
							resolver.AddExistingObject(statement.ObjectName);
							sqlWriter.WriteLine();
							statement.WriteTo(sqlWriter);
						} else {
							resolver.Add(statement);
						}
					}
					sqlWriter.DecreaseIndent();
					yield return writer.ToString();
				}
				UnsetQualification();
				SetQualification(schemaName);
				foreach (Statement statement in resolver.GetInOrder(true)) {
					yield return WriteStatement(statement, true, builder);
				}
				foreach (Statement additionalSetupStatement in AdditionalSetupStatements) {
					yield return WriteStatement(additionalSetupStatement, false, builder);
				}
			} finally {
				UnsetQualification();
			}
		}

		protected void AddAdditionalSetupStatement(Statement statement) {
			if (statement == null) {
				throw new ArgumentNullException("statement");
			}
			additionalSetupStatements.Add(statement);
		}

		protected void AdditionalSetupStatementSetSchemaOverride() {
			StatementSetSchemaOverride(additionalSetupStatements);
		}

		protected void StatementSetSchemaOverride(IEnumerable<Statement> statements) {
			foreach (Statement statement in statements) {
				foreach (IQualifiedName<SchemaName> name in statement.GetInnerSchemaQualifiedNames(n => ObjectSchemas.Contains(n))) {
					name.SetOverride(this);
				}
			}
		}
	}
}
