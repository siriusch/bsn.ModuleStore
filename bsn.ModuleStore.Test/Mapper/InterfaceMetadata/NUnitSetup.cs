#if DEBUG
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace bsn.ModuleStore.Mapper.InterfaceMetadata {
	internal class NUnitSetup {
		private readonly string connectionString;

		public NUnitSetup(string connectionString) {
			this.connectionString = connectionString;
		}

		public string ConnectionString {
			get {
				return connectionString;
			}
		}

		public void CleanUpTestDatabase() {
			DropTables();
			DropProcedures();
			DropTypes();
		}

		public void SetupTestEnvironment() {
			CleanUpTestDatabase();
			CreateTypes();
			CreateTables();
			CreateProcedures();
			ExecuteSetupProcedures();
		}

		private void CreateProcedures() {
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("CREATE PROCEDURE [dbo].[spListSimpleTypes] ( @sKey NVARCHAR(50) )");
			sb.AppendLine("AS BEGIN");
			sb.AppendLine("SET NOCOUNT ON ;");
			sb.AppendLine("SELECT  * FROM    [dbo].[tblSimpleTypes] AS st WHERE   [sKey] = @sKey OR @sKey IS NULL");
			sb.AppendLine("END;");
			ExecuteSqlStatement(sb.ToString());
			sb = new StringBuilder();
			sb.AppendLine("CREATE PROCEDURE [dbo].[spSaveSimpleTypes] ( @tblInput dbo.udtSimpleType READONLY )");
			sb.AppendLine("AS BEGIN");
			sb.AppendLine("SET NOCOUNT ON ;");
			sb.AppendLine("INSERT  INTO [dbo].[tblSimpleTypes]([uidKey],[sKey],[iData]) SELECT  [uidKey],[sKey],[iData] FROM    @tblInput");
			sb.AppendLine("END;");
			ExecuteSqlStatement(sb.ToString());
			sb = new StringBuilder();
			sb.AppendLine("CREATE PROCEDURE [dbo].[spGetKey] ( @uidKey UNIQUEIDENTIFIER )");
			sb.AppendLine("AS BEGIN");
			sb.AppendLine("SET NOCOUNT ON ;");
			sb.AppendLine("SELECT  [sKey] FROM    [dbo].[tblSimpleTypes] AS st WHERE   [uidKey] = @uidKey");
			sb.AppendLine("END;");
			ExecuteSqlStatement(sb.ToString());
			sb = new StringBuilder();
			sb.AppendLine("CREATE PROCEDURE [dbo].[spClearSimpleTypes]");
			sb.AppendLine("AS BEGIN");
			sb.AppendLine("SET NOCOUNT ON ;");
			sb.AppendLine("DELETE  FROM [dbo].[tblSimpleTypes]");
			sb.AppendLine("END;");
			ExecuteSqlStatement(sb.ToString());
			sb = new StringBuilder();
			sb.AppendLine("CREATE PROCEDURE [dbo].[spAddSimpleTypes] (@uidKey UNIQUEIDENTIFIER,@sKey NVARCHAR(50),@iData INT )");
			sb.AppendLine("AS BEGIN");
			sb.AppendLine("SET NOCOUNT ON ;");
			sb.AppendLine("INSERT  INTO [dbo].[tblSimpleTypes]( [uidKey], [sKey], [iData] ) VALUES  ( @uidKey, @sKey, @iData )");
			sb.AppendLine("END");
			ExecuteSqlStatement(sb.ToString());
			sb = new StringBuilder();
			sb.AppendLine("CREATE PROCEDURE [dbo].[spSetupParentChildTestData]");
			sb.AppendLine("AS BEGIN");
			sb.AppendLine("SET NOCOUNT ON ;");
			sb.AppendLine("DELETE FROM [dbo].[tblChild];");
			sb.AppendLine("DELETE FROM [dbo].[tblParent];");
			sb.AppendLine("INSERT  INTO [dbo].[tblParent] ( [uidParent],[sKey] ) VALUES  ( '63129E3A-A32A-4CBC-B869-1E185C56BBEB',N'Parent 1')");
			sb.AppendLine("INSERT  INTO [dbo].[tblParent] ( [uidParent],[sKey] ) VALUES  ( '2900945C-AF46-45CB-A881-5763EB474A88',N'Parent 2')");
			sb.AppendLine("INSERT  INTO [dbo].[tblParent] ( [uidParent],[sKey] ) VALUES  ( 'F475E4B4-09F6-4334-8C1F-EB53E26B0280',N'Parent 3')");
			sb.AppendLine("INSERT INTO [dbo].[tblChild] ( [uidChild], [sKey], [uidParent] ) VALUES  ( 'EF8F2CC5-3F5C-4799-96B0-8D1055BD05C3', N'Child 1.1', '63129E3A-A32A-4CBC-B869-1E185C56BBEB' )");
			sb.AppendLine("INSERT INTO [dbo].[tblChild] ( [uidChild], [sKey], [uidParent] ) VALUES  ( 'DBE1A912-E3B0-449E-A5D8-F5000B6C453B', N'Child 1.2', '63129E3A-A32A-4CBC-B869-1E185C56BBEB' )");
			sb.AppendLine("INSERT INTO [dbo].[tblChild] ( [uidChild], [sKey], [uidParent] ) VALUES  ( '516FE5D1-99DD-45ED-934E-DA0DD38192FF', N'Child 1.3', '63129E3A-A32A-4CBC-B869-1E185C56BBEB' )");
			sb.AppendLine("INSERT INTO [dbo].[tblChild] ( [uidChild], [sKey], [uidParent] ) VALUES  ( '13A5D98F-E86A-4DE5-8DFD-D1843D41DAAC', N'Child 2.1', '2900945C-AF46-45CB-A881-5763EB474A88' )");
			sb.AppendLine("INSERT INTO [dbo].[tblChild] ( [uidChild], [sKey], [uidParent] ) VALUES  ( '73B9B423-A8D1-4444-96CA-CDDDD8CB3DF4', N'Child 2.2', '2900945C-AF46-45CB-A881-5763EB474A88' )");
			sb.AppendLine("INSERT INTO [dbo].[tblChild] ( [uidChild], [sKey], [uidParent] ) VALUES  ( '81FF3777-AADD-41AF-AE06-76CA516D1BDD', N'Child 2.3', '2900945C-AF46-45CB-A881-5763EB474A88' )");
			sb.AppendLine("END;");
			ExecuteSqlStatement(sb.ToString());
			sb = new StringBuilder();
			sb.AppendLine("CREATE PROCEDURE [dbo].[spListParentChild]");
			sb.AppendLine("AS BEGIN");
			sb.AppendLine("SET NOCOUNT ON ;");
			sb.AppendLine("SELECT  [p].[uidParent] uidParent,[p].[sKey] sKeyParent,[c].[uidParent] uidChildParent,[uidChild] uidChild,[c].[sKey] sKeyChild");
			sb.AppendLine("FROM    [dbo].[tblChild] AS c JOIN [dbo].[tblParent] AS p ON [c].[uidParent] = [p].[uidParent]");
			sb.AppendLine("ORDER BY [p].[uidParent]");
			sb.AppendLine("END");
			ExecuteSqlStatement(sb.ToString());
			sb = new StringBuilder();
			sb.AppendLine("CREATE PROCEDURE [dbo].[spListParentChildWithNull]");
			sb.AppendLine("AS BEGIN");
			sb.AppendLine("SET NOCOUNT ON ;");
			sb.AppendLine("SELECT  [p].[uidParent] uidParent,[p].[sKey] sKeyParent,[c].[uidParent] uidChildParent,[uidChild] uidChild,[c].[sKey] sKeyChild");
			sb.AppendLine("FROM    [dbo].[tblParent] AS p LEFT JOIN [dbo].[tblChild] AS c ON [c].[uidParent] = [p].[uidParent]");
			sb.AppendLine("ORDER BY [p].[uidParent]");
			sb.AppendLine("END");
			ExecuteSqlStatement(sb.ToString());
			sb = new StringBuilder();
			sb.AppendLine("CREATE PROCEDURE [dbo].[spListParentChildMultiResultsWithoutRelation]");
			sb.AppendLine("AS BEGIN");
			sb.AppendLine("SET NOCOUNT ON ;");
			sb.AppendLine("SELECT [uidChild], [sKey] sKeyChild,[uidParent] uidChildParent FROM [dbo].[tblChild] AS c ORDER BY [sKey]");
			sb.AppendLine("SELECT [p].[uidParent] ,[p].[sKey] sKeyParent FROM [dbo].[tblParent] AS p ORDER BY [sKey]");
			sb.AppendLine("END");
			ExecuteSqlStatement(sb.ToString());
			sb = new StringBuilder();
			sb.AppendLine("CREATE PROCEDURE [dbo].[spListParentChildMultiResultsChildParent]");
			sb.AppendLine("AS BEGIN");
			sb.AppendLine("SET NOCOUNT ON ;");
			sb.AppendLine("SELECT [uidChild], [sKey] sKeyChild, [uidParent] FROM [dbo].[tblChild] AS c ORDER BY [sKey]");
			sb.AppendLine("SELECT  [p].[uidParent] ,[p].[sKey] sKeyParent ,[c].[uidChild]");
			sb.AppendLine("FROM    [dbo].[tblParent] AS p JOIN [dbo].[tblChild] AS c ON [p].[uidParent] = [c].[uidParent]");
			sb.AppendLine("ORDER BY p.[sKey]");
			sb.AppendLine("END");
			ExecuteSqlStatement(sb.ToString());
		}

		private void CreateTables() {
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("CREATE TABLE [dbo].[tblSimpleTypes](");
			sb.AppendLine("[uidKey] [uniqueidentifier] ROWGUIDCOL  NOT NULL, ");
			sb.AppendLine("[sKey] [nvarchar](50) NOT NULL, ");
			sb.AppendLine("[iData] [int] NULL, ");
			sb.AppendLine("CONSTRAINT [PK_tblSimpleTypes] PRIMARY KEY CLUSTERED  ");
			sb.AppendLine("( ");
			sb.AppendLine("[uidKey] ASC ");
			sb.AppendLine(")WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY] ");
			sb.AppendLine(") ON [PRIMARY] ");
			sb.AppendLine("ALTER TABLE [dbo].[tblSimpleTypes] ADD  CONSTRAINT [DF_tblSimpleTypes_uidKey]  DEFAULT (newid()) FOR [uidKey] ");
			ExecuteSqlStatement(sb.ToString());
			sb = new StringBuilder();
			sb.AppendLine("CREATE TABLE [dbo].[tblParent](");
			sb.AppendLine("[uidParent] [uniqueidentifier] ROWGUIDCOL  NOT NULL, [sKey] [nvarchar](50) NOT NULL,");
			sb.AppendLine("CONSTRAINT [PK_tblParent] PRIMARY KEY CLUSTERED (	[uidParent] ASC )");
			sb.AppendLine("WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]");
			sb.AppendLine(") ON [PRIMARY]");
			sb.AppendLine("ALTER TABLE [dbo].[tblParent] ADD  CONSTRAINT [DF_tblParent_uidParent]  DEFAULT (newid()) FOR [uidParent]");
			ExecuteSqlStatement(sb.ToString());
			sb = new StringBuilder();
			sb.AppendLine("CREATE TABLE [dbo].[tblChild](");
			sb.AppendLine("[uidChild] [uniqueidentifier] ROWGUIDCOL  NOT NULL,	[sKey] [nvarchar](50) NOT NULL,	[uidParent] [uniqueidentifier] NOT NULL,");
			sb.AppendLine("CONSTRAINT [PK_tblChild] PRIMARY KEY CLUSTERED ( [uidChild] ASC )");
			sb.AppendLine("WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]");
			sb.AppendLine(") ON [PRIMARY]");
			sb.AppendLine("ALTER TABLE [dbo].[tblChild]  WITH CHECK ADD  CONSTRAINT [FK_tblChild_tblParent] FOREIGN KEY([uidParent])");
			sb.AppendLine("REFERENCES [dbo].[tblParent] ([uidParent])");
			sb.AppendLine("ALTER TABLE [dbo].[tblChild] CHECK CONSTRAINT [FK_tblChild_tblParent]");
			sb.AppendLine("ALTER TABLE [dbo].[tblChild] ADD  CONSTRAINT [DF_tblChild_uidChild]  DEFAULT (newid()) FOR [uidChild]");
			ExecuteSqlStatement(sb.ToString());
		}

		private void CreateTypes() {
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("CREATE TYPE [dbo].[udtSimpleType] AS TABLE(");
			sb.AppendLine("[uidKey] [uniqueidentifier] NOT NULL,");
			sb.AppendLine("[sKey] [nvarchar](50) NULL,");
			sb.AppendLine("[iData] [int] NULL,");
			sb.AppendLine("PRIMARY KEY CLUSTERED ");
			sb.AppendLine("([uidKey] ASC) WITH (IGNORE_DUP_KEY = OFF)");
			sb.AppendLine(")");
			ExecuteSqlStatement(sb.ToString());
		}

		private void DropProcedures() {
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spListSimpleTypes]') AND type in (N'P', N'PC')) BEGIN");
			sb.AppendLine("DROP PROCEDURE [dbo].[spListSimpleTypes]");
			sb.AppendLine("END");
			sb.AppendLine("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spSaveSimpleTypes]') AND type in (N'P', N'PC')) BEGIN");
			sb.AppendLine("DROP PROCEDURE [dbo].[spSaveSimpleTypes]");
			sb.AppendLine("END");
			sb.AppendLine("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spGetKey]') AND type in (N'P', N'PC')) BEGIN");
			sb.AppendLine("DROP PROCEDURE [dbo].[spGetKey]");
			sb.AppendLine("END");
			sb.AppendLine("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spClearSimpleTypes]') AND type in (N'P', N'PC')) BEGIN");
			sb.AppendLine("DROP PROCEDURE [dbo].[spClearSimpleTypes]");
			sb.AppendLine("END");
			sb.AppendLine("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spAddSimpleTypes]') AND type in (N'P', N'PC')) BEGIN");
			sb.AppendLine("DROP PROCEDURE [dbo].[spAddSimpleTypes]");
			sb.AppendLine("END");
			sb.AppendLine("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spSetupParentChildTestData]') AND type in (N'P', N'PC')) BEGIN");
			sb.AppendLine("DROP PROCEDURE [dbo].[spSetupParentChildTestData]");
			sb.AppendLine("END");
			sb.AppendLine("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spListParentChild]') AND type in (N'P', N'PC')) BEGIN");
			sb.AppendLine("DROP PROCEDURE [dbo].[spListParentChild]");
			sb.AppendLine("END");
			sb.AppendLine("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spListParentChildWithNull]') AND type in (N'P', N'PC')) BEGIN");
			sb.AppendLine("DROP PROCEDURE [dbo].[spListParentChildWithNull]");
			sb.AppendLine("END");
			sb.AppendLine("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spListParentChildMultiResultsWithoutRelation]') AND type in (N'P', N'PC')) BEGIN");
			sb.AppendLine("DROP PROCEDURE [dbo].[spListParentChildMultiResultsWithoutRelation]");
			sb.AppendLine("END");
			sb.AppendLine("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spListParentChildMultiResultsChildParent]') AND type in (N'P', N'PC')) BEGIN");
			sb.AppendLine("DROP PROCEDURE [dbo].[spListParentChildMultiResultsChildParent]");
			sb.AppendLine("END");
			ExecuteSqlStatement(sb.ToString());
		}

		private void DropTables() {
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblSimpleTypes]') AND type in (N'U')) begin ");
			sb.AppendLine("DROP TABLE [dbo].[tblSimpleTypes]");
			sb.AppendLine("END");
			ExecuteSqlStatement(sb.ToString());
			sb = new StringBuilder();
			sb.AppendLine("IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_tblChild_tblParent]') AND parent_object_id = OBJECT_ID(N'[dbo].[tblChild]')) BEGIN");
			sb.AppendLine("ALTER TABLE [dbo].[tblChild] DROP CONSTRAINT [FK_tblChild_tblParent]");
			sb.AppendLine("END");
			sb.AppendLine("IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_tblChild_uidChild]') AND type = 'D') BEGIN");
			sb.AppendLine("ALTER TABLE [dbo].[tblChild] DROP CONSTRAINT [DF_tblChild_uidChild]");
			sb.AppendLine("END");
			sb.AppendLine("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblChild]') AND type in (N'U')) BEGIN");
			sb.AppendLine("DROP TABLE [dbo].[tblChild]");
			sb.AppendLine("END");
			ExecuteSqlStatement(sb.ToString());
			sb = new StringBuilder();
			sb.AppendLine("IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_tblParent_uidParent]') AND type = 'D') BEGIN");
			sb.AppendLine("ALTER TABLE [dbo].[tblParent] DROP CONSTRAINT [DF_tblParent_uidParent]");
			sb.AppendLine("END");
			sb.AppendLine("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblParent]') AND type in (N'U')) BEGIN");
			sb.AppendLine("DROP TABLE [dbo].[tblParent]");
			sb.AppendLine("END");
			ExecuteSqlStatement(sb.ToString());
		}

		private void DropTypes() {
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("IF  EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N'udtSimpleType' AND ss.name = N'dbo') begin ");
			sb.AppendLine("DROP TYPE [dbo].[udtSimpleType]");
			sb.AppendLine("end");
			ExecuteSqlStatement(sb.ToString());
		}

		private void ExecuteSetupProcedures() {
			ExecuteSqlStatement("exec spSetupParentChildTestData");
		}

		private void ExecuteSqlStatement(string sql) {
			using (SqlConnection connection = new SqlConnection(connectionString)) {
				connection.Open();
				using (SqlCommand command = connection.CreateCommand()) {
					command.CommandText = sql;
					command.CommandType = CommandType.Text;
					command.ExecuteNonQuery();
				}
			}
		}
	}
}

#endif
