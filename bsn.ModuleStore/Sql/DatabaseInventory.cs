using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;

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

		private readonly Database database;
		private readonly string schemaName;
		private bool schemaExists;

		public DatabaseInventory(Database database, string schemaName) {
			if (database == null) {
				throw new ArgumentNullException("database");
			}
			if (database.IsSystemObject) {
				throw new ArgumentException("The connection does not point to a valid user database", "database");
			}
			this.database = database;
			this.schemaName = schemaName ?? database.DefaultSchema;
		}

		public override bool SchemaExists {
			get {
				return schemaExists;
			}
		}

		public override string SchemaName {
			get {
				return schemaName;
			}
		}

		public override void Populate() {
			base.Populate();
			schemaExists = false;
			Schema schema = database.Schemas[schemaName];
			if (schema != null) {
				schemaExists = true;
				ScriptingOptions options = new ScriptingOptions();
				options.AgentJobId = false;
				options.AllowSystemObjects = false;
				options.AnsiPadding = false;
				options.ClusteredIndexes = true;
				options.ConvertUserDefinedDataTypesToBaseType = false;
				options.ContinueScriptingOnError = false;
				options.DriAll = true;
				options.EnforceScriptingOptions = false;
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
				options.SchemaQualify = true;
				options.SchemaQualifyForeignKeysReferences = true;
				options.Bindings = false;
				options.DriIncludeSystemNames = false;
				options.ScriptDrops = false;
				options.Statistics = false;
				options.TargetServerVersion = SqlServerVersion.Version90;
				options.TimestampToBinary = false;
				options.Triggers = true;
				options.WithDependencies = false;
				options.XmlIndexes = true;
				foreach (Urn urn in schema.EnumOwnedObjects()) {
					NamedSmoObject smoObject = database.Parent.GetSmoObject(urn) as NamedSmoObject;
					if (IsSupportedType(smoObject)) {
						Debug.Assert(smoObject != null);
						StringCollection script = ((IScriptable)smoObject).Script(options);
						CreateTableStatement createTable = null;
						foreach (string statementScript in script) {
							using (TextReader scriptReader = new StringReader(statementScript)) {
								ProcessSingleScript(scriptReader, ref createTable, statement => {
								                                                   	throw CreateException("Cannot process statement:", statement);
								                                                   });
							}
						}
					}
				}
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