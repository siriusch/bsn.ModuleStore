using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using bsn.ModuleStore.Sql.Script;

using Microsoft.SqlServer.Management.Smo;

namespace bsn.ModuleStore.Sql {
	public class DatabaseInventory: Inventory {
		private static readonly ICollection<Type> supportedTypes = new[] {typeof(Index), typeof(FullTextIndex), typeof(UserDefinedFunction), typeof(StoredProcedure), typeof(Table), typeof(Trigger), typeof(View), typeof(XmlSchemaCollection)};

		public static Exception CreateException(string message, SqlScriptableToken token) {
			StringWriter writer = new StringWriter();
			writer.WriteLine(message);
			token.WriteTo(new SqlWriter(writer));
			return new InvalidOperationException(writer.ToString());
		}

		private readonly string schemaName;

		public DatabaseInventory(Microsoft.SqlServer.Management.Smo.Database database, string schemaName) {
			if (database == null) {
				throw new ArgumentNullException("database");
			}
			if (database.IsSystemObject) {
				throw new ArgumentException("The connection does not point to a valid user database", "database");
			}
			database.Refresh();
			this.schemaName = schemaName ?? database.DefaultSchema;
			Schema schema = database.Schemas[schemaName];
			if (schema != null) {
				ScriptingOptions options = new ScriptingOptions();
				options.AgentJobId = false;
				options.AllowSystemObjects = false;
				options.AnsiPadding = false;
				options.ClusteredIndexes = true;
				options.ConvertUserDefinedDataTypesToBaseType = false;
				options.ContinueScriptingOnError = false;
				options.DriAll = true;
				options.EnforceScriptingOptions = true;
				options.ExtendedProperties = false;
				options.FullTextIndexes = true;
				options.IncludeDatabaseContext = false;
				options.IncludeDatabaseRoleMemberships = false;
				options.IncludeHeaders = false;
				options.IncludeIfNotExists = false;
				options.Indexes = true;
				options.LoginSid = false;
				options.NoCommandTerminator = false;
				options.NoFileGroup = true;
				options.NoIdentities = false;
				options.NoAssemblies = false;
				options.NoIndexPartitioningSchemes = true;
				options.NonClusteredIndexes = true;
				options.NoTablePartitioningSchemes = true;
				options.OptimizerData = false;
				options.Permissions = false;
				options.PrimaryObject = true;
				options.SchemaQualify = true;
				options.SchemaQualifyForeignKeysReferences = true;
				options.Bindings = false;
				options.DriIncludeSystemNames = false;
				options.ScriptDrops = false;
				options.Statistics = false;
				options.TargetServerVersion = SqlServerVersion.Version90;
				options.TimestampToBinary = false;
				options.Triggers = false;
				options.WithDependencies = false;
				options.XmlIndexes = true;
				foreach (Urn urn in schema.EnumOwnedObjects()) {
					NamedSmoObject smoObject = database.Parent.GetSmoObject(urn) as NamedSmoObject;
					if (IsSupportedType(smoObject)) {
						Debug.Assert(smoObject != null);
						smoObject.Refresh();
						StringCollection script;
						try {
							script = ((IScriptable)smoObject).Script(options);
						} catch (FailedOperationException ex) {
							Debug.WriteLine(ex.Message, "SMO scripting exception, retrying...");
							smoObject.Refresh();
							GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
							GC.WaitForPendingFinalizers();
							script = ((IScriptable)smoObject).Script(options);
						}
						using (StringCollectionReader scriptReader = new StringCollectionReader(script, ";")) {
							try {
								try {
									ProcessSingleScript(scriptReader, statement => {
										throw CreateException("Cannot process statement:", statement);
									});
								} catch (ParseException ex) {
									ex.FileName = smoObject.Name;
									throw;
								}
							} catch {
								Trace.WriteLine(string.Join(";\r\n", script.Cast<string>().ToArray()));
								throw;
							}
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
					yield return WriteStatement(statement.CreateDropStatement(), buffer);
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

		internal bool IsSupportedType(NamedSmoObject smoInstance) {
			if (smoInstance is IScriptable) {
				Type type = smoInstance.GetType();
				foreach (Type supportedType in supportedTypes) {
					if (supportedType.IsAssignableFrom(type)) {
						return !true.Equals(smoInstance.Properties["IsSystemObject"].Value);
					}
				}
			}
			return false;
		}
	}
}