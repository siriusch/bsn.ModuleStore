using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Diagnostics;

using bsn.ModuleStore.Sql.Script;

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace bsn.ModuleStore.Sql {
	public class DatabaseInventory: Inventory {
		private static readonly ICollection<Type> supportedTypes = new[] {typeof(Index), typeof(FullTextIndex), typeof(UserDefinedFunction), typeof(StoredProcedure), typeof(Table), typeof(Trigger), typeof(View), typeof(XmlSchemaCollection)};

		private readonly string connectionString;
		private readonly string schemaName;
		private bool schemaExists;

		public DatabaseInventory(string connectionString, string schemaName) {
			if (connectionString == null) {
				throw new ArgumentNullException("connectionString");
			}
			this.connectionString = connectionString;
			this.schemaName = schemaName;
		}

		public override bool SchemaExists {
			get {
				return schemaExists;
			}
		}

		public string SchemaName {
			get {
				return schemaName;
			}
		}

		public override void Populate() {
			base.Populate();
			schemaExists = false;
			using (SqlConnection connection = new SqlConnection(connectionString)) {
				Server server = new Server(new ServerConnection(connection));
				Database database = server.Databases[connection.Database];
				if (database.IsSystemObject) {
					database = null;
				}
				if (database == null) {
					throw new KeyNotFoundException(string.Format("The connection string [{0}] does not point to a valid user database", connectionString));
				}
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
					options.SchemaQualify = false;
					options.SchemaQualifyForeignKeysReferences = false;
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
						NamedSmoObject smoObject = server.GetSmoObject(urn) as NamedSmoObject;
						if (IsSupportedType(smoObject)) {
							Debug.Assert(smoObject != null);
							Console.WriteLine(string.Format("{0}: ?", smoObject.Name));
							StringCollection script = ((Microsoft.SqlServer.Management.Smo.IScriptable)smoObject).Script(options);
							foreach (string statementScript in script) {
								foreach (Statement statement in ScriptParser.Parse(statementScript)) {
									Console.WriteLine("----------- {0} ----------------------------------", statement.GetType().Name);
									Console.WriteLine(statement.ToString());
								}
							}
						}
					}
				}
			}
		}

		internal bool IsSupportedType(NamedSmoObject smoInstance) {
			if (smoInstance is Microsoft.SqlServer.Management.Smo.IScriptable) {
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