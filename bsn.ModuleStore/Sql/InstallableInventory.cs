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
using System.IO;
using System.Linq;
using System.Text;

using NLog;

using bsn.ModuleStore.Sql.Script;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql {
	public abstract class InstallableInventory: Inventory {
		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private readonly List<IScriptableStatement> additionalSetupStatements = new List<IScriptableStatement>();

		public IEnumerable<IScriptableStatement> AdditionalSetupStatements => additionalSetupStatements;

		public IEnumerable<string> GenerateInstallSql(DatabaseEngine targetEngine, string schemaName, string ownerName) {
			if (string.IsNullOrEmpty(schemaName)) {
				throw new ArgumentNullException(nameof(schemaName));
			}
			var newSchema = !schemaName.Equals("dbo", StringComparison.OrdinalIgnoreCase);
			var builder = new StringBuilder(4096);
			var resolver = new DependencyResolver();
			var createStatements = Objects.SelectMany(o => o.CreateStatementFragments(newSchema ? CreateFragmentMode.CreateOnNewSchema : CreateFragmentMode.CreateOnExistingSchema)).Where(s => s.DoesApplyToEngine(targetEngine));
			if (newSchema) {
				SetQualification(null);
				try {
					using (var writer = new StringWriter(builder)) {
						var sqlWriter = new SqlWriter(writer, targetEngine);
						sqlWriter.WriteKeyword("CREATE SCHEMA");
						using (sqlWriter.Indent()) {
							sqlWriter.WriteScript(new SchemaName(schemaName), WhitespacePadding.SpaceBefore);
							if (!string.IsNullOrEmpty(ownerName)) {
								sqlWriter.Write(" AUTHORIZATION");
								sqlWriter.WriteScript(new ObjectName(ownerName), WhitespacePadding.SpaceBefore);
							}
							var schemaResolver = new DependencyResolver();
							foreach (var statement in createStatements) {
								if (statement.IsPartOfSchemaDefinition) {
									if (statement is CreateTableFragment) {
										sqlWriter.WriteLine();
										statement.WriteTo(sqlWriter);
										resolver.AddExistingObject(statement.ObjectName);
										schemaResolver.AddExistingObject(statement.ObjectName);
									} else {
										schemaResolver.Add(statement);
									}
								} else {
									resolver.Add(statement);
								}
							}
							try {
								foreach (var statement in schemaResolver.GetInOrder(true)) {
									sqlWriter.WriteLine();
									statement.WriteTo(sqlWriter);
									resolver.AddExistingObject(statement.ObjectName);
								}
							} catch (InvalidOperationException ex) {
								schemaResolver.TransferPendingObjects(resolver);
								log.Debug(ex, "SCHEMA CREATE trimmed because of {message} - processing continues", ex.Message);
							}
						}
						yield return writer.ToString();
					}
				} finally {
					UnsetQualification();
				}
			} else {
				foreach (var statement in createStatements) {
					resolver.Add(statement);
				}
			}
			SetQualification(schemaName);
			try {
				foreach (var statement in resolver.GetInOrder(true)) {
					yield return WriteStatement(statement, builder, targetEngine);
				}
				if (AdditionalSetupStatements.Any()) {
					foreach (var table in Objects.OfType<CreateTableStatement>()) {
						yield return WriteStatement(new AlterTableNocheckConstraintStatement(table.TableName, new TableCheckToken()), builder, targetEngine);
					}
					foreach (var additionalSetupStatement in AdditionalSetupStatements) {
						yield return WriteStatement(additionalSetupStatement, builder, targetEngine);
					}
					foreach (var table in Objects.OfType<CreateTableStatement>()) {
						yield return WriteStatement(new AlterTableCheckConstraintStatement(table.TableName, new TableWithCheckToken()), builder, targetEngine);
					}
				}
			} finally {
				UnsetQualification();
			}
		}

		protected void AddAdditionalSetupStatement(IScriptableStatement statement) {
			if (statement == null) {
				throw new ArgumentNullException(nameof(statement));
			}
			additionalSetupStatements.Add(statement);
		}

		protected void AdditionalSetupStatementSetSchemaOverride() {
			StatementSetSchemaOverride(additionalSetupStatements);
		}

		protected void StatementSetSchemaOverride(IEnumerable<IScriptableStatement> statements) {
			foreach (var statement in statements.OfType<Statement>()) {
				foreach (var name in statement.GetInnerSchemaQualifiedNames(n => ObjectSchemas.Contains(n))) {
					name.SetOverride(this);
				}
			}
		}
	}
}
