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
using System.Diagnostics;
using System.IO;
using System.Linq;

using NUnit.Framework;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	[TestFixture]
	public class ScriptParserTest: AssertionHelper {
		private class SchemaQualified: IQualified<SchemaName> {
			private readonly SchemaName schema;

			public SchemaQualified(string schema) {
				this.schema = new SchemaName(schema);
			}

			public SchemaName Qualification {
				get {
					return schema;
				}
			}
		}

		[TestFixtureSetUp]
		public void Initialize() {
			ScriptParser.GetSemanticActions();
		}

		public List<Statement> ParseWithRoundtrip(string sql, int expectedStatementCount, string schema) {
			GenerateSql(ScriptParser.Parse(sql)); // warm-up
			Stopwatch sw = new Stopwatch();
			sw.Start();
			IEnumerable<Statement> parsedStatements = ScriptParser.Parse(sql);
			sw.Stop();
			long parseTime = sw.ElapsedMilliseconds;
			List<Statement> statements = parsedStatements.ToList();
			Expect(statements.Count, EqualTo(expectedStatementCount));
			if (schema != null) {
				IQualified<SchemaName> qualified = new SchemaQualified(schema);
				foreach (IQualifiedName<SchemaName> qualifiedName in statements.SelectMany(s => s.GetObjectSchemaQualifiedNames(schema))) {
					qualifiedName.SetOverride(qualified);
				}
			}
			sw.Reset();
			sw.Start();
			string sqlGen = GenerateSql(statements);
			sw.Stop();
			Trace.Write(Environment.NewLine+sqlGen, string.Format("Generated SQL (parse: {0}ms | gen: {1}ms)", parseTime, sw.ElapsedMilliseconds));
			sw.Reset();
			sw.Start();
			IEnumerable<Statement> parsedStatementsRoundtrip = ScriptParser.Parse(sqlGen);
			string sqlGenRoundtrip = GenerateSql(parsedStatementsRoundtrip);
			sw.Stop();
			Trace.Write(string.Format("{0}ms", sw.ElapsedMilliseconds), "Roundtrip");
			Expect(sqlGen, EqualTo(sqlGenRoundtrip));
			return statements;
		}

		private string GenerateSql(IEnumerable<Statement> statements) {
			using (StringWriter stringWriter = new StringWriter()) {
				SqlWriter sqlGen = new SqlWriter(stringWriter, DatabaseEngine.Unknown);
				foreach (Statement statement in statements) {
					statement.WriteTo(sqlGen);
					sqlGen.WriteLine(";");
				}
				return stringWriter.ToString();
			}
		}

		[Test]
		public void TableWithFillFactor() {
			ParseWithRoundtrip(@"CREATE TABLE [Acl].[tblACE] ( 
 [idAce] bigint IDENTITY(1, 1) NOT NULL, 
 [uidObject] uniqueidentifier NOT NULL, 
 [uidSubject] uniqueidentifier NOT NULL, 
 [iKind] tinyint NOT NULL, 
 [iLevel] tinyint NOT NULL, 
 [idRight] smallint NOT NULL, 
 [timestamp] timestamp NOT NULL, 
 CONSTRAINT [PK_tblACE] PRIMARY KEY CLUSTERED ( 
    [idAce] ASC 
) WITH FILLFACTOR = 90, 
 CONSTRAINT [UK_tblACE_uidSubject_uidObject_iKind_idRight] UNIQUE NONCLUSTERED ( 
    [uidSubject] ASC, 
    [uidObject] ASC, 
    [iKind] ASC, 
    [idRight] ASC 
) WITH FILLFACTOR = 90, 
 CONSTRAINT [FK_tblACE_tblRight] FOREIGN KEY ([idRight]) REFERENCES [Acl].[tblRight] ([idRight]) ON DELETE CASCADE 
);", 1, null);
		}

		[Test]
		public void BeginTransaction() {
			ParseWithRoundtrip(@"BEGIN TRAN", 1, null);
		}

		[Test]
		public void BeginTransactionIdentifierName() {
			ParseWithRoundtrip(@"BEGIN TRANSACTION MyTrans", 1, null);
		}

		[Test]
		public void BeginTransactionVariableName() {
			ParseWithRoundtrip(@"BEGIN TRANSACTION @trans", 1, null);
		}

		[Test]
		public void BeginTransactionWithMark() {
			ParseWithRoundtrip(@"BEGIN TRANSACTION MyTrans WITH MARK", 1, null);
		}

		[Test]
		public void BeginTransactionWithMarkNamed() {
			ParseWithRoundtrip(@"BEGIN TRANSACTION MyTrans WITH MARK 'My Trans'", 1, null);
		}

		[Test]
		public void BitwiseAnd() {
			ParseWithRoundtrip(@"PRINT 1&0", 1, null);
		}

		[Test]
		public void BitwiseNot() {
			ParseWithRoundtrip(@"PRINT ~1", 1, null);
		}

		[Test]
		public void BitwiseOr() {
			ParseWithRoundtrip(@"PRINT 1|0", 1, null);
		}

		[Test]
		public void BitwiseXor() {
			ParseWithRoundtrip(@"PRINT 1^0", 1, null);
		}

		[Test]
		public void CommitTransaction() {
			ParseWithRoundtrip(@"COMMIT TRAN", 1, null);
		}

		[Test]
		public void CommitTransactionIdentifierName() {
			ParseWithRoundtrip(@"COMMIT TRANSACTION MyTrans", 1, null);
		}

		[Test]
		public void CommitTransactionLegacy() {
			ParseWithRoundtrip(@"COMMIT", 1, null);
		}

		[Test]
		public void CommitTransactionVariableName() {
			ParseWithRoundtrip(@"COMMIT TRANSACTION @trans", 1, null);
		}

		[Test]
		public void CreateFunctionWithReadonlyParameter() {
			ParseWithRoundtrip(
					@"CREATE FUNCTION [dbo].[fnIndicatorStructureResolve]
  (
   @tblPeriodIndicator [dbo].[udtUidList] READONLY,
   @tblStructure [dbo].[udtStructureList] READONLY
  )
RETURNS TABLE
  AS
RETURN
  (
WITH  [IndicatorStructures]
        AS (
            SELECT DISTINCT [pi].[uidPeriodIndicator], [s].[uidStructure], [i].[uidPersistLevel],
                CASE WHEN [i].[uidPersistLevel] = [s].[uidLevel] THEN CONVERT(bit, 1)
                     ELSE CONVERT(bit, 0)
                END AS [bPersist]
              FROM @tblPeriodIndicator AS [pis]
              JOIN 
                [dbo].[tblPeriodIndicator] AS [pi] ON [pis].[uid] = [pi].[uidPeriodIndicator]
              JOIN 
                [dbo].[tblIndicator] AS [i] ON [i].[uidIndicator] = [pi].[uidIndicator] 
              JOIN 
                @tblStructure AS [s] ON [i].[uidOwnerStructure] = [s].[uidStructure]
              WHERE ([i].[uidPersistLevel] IS NOT NULL)
            UNION ALL
            SELECT [is].[uidPeriodIndicator], [s].[uidStructure], [is].[uidPersistLevel],
                CASE WHEN [is].[uidPersistLevel] = [s].[uidLevel] THEN CONVERT(bit, 1)
                     ELSE CONVERT(bit, 0)
                END
              FROM [IndicatorStructures] AS [is] 
              JOIN 
                @tblStructure AS [s] ON [s].[uidParent] = [is].[uidStructure]
              WHERE [is].[bPersist] = 0
           )
  SELECT [is].[uidPeriodIndicator], [is].[uidStructure]
    FROM [IndicatorStructures] [is]
    WHERE [is].[bPersist] = 1
        )",
					1, null);
		}

		[Test]
		public void CreateIndex() {
			ParseWithRoundtrip(@"CREATE UNIQUE INDEX AK_UnitMeasure_Name ON Production.UnitMeasure(Name);", 1, null);
		}

		[Test]
		public void CreateIndexFiltered() {
			ParseWithRoundtrip(@"CREATE NONCLUSTERED INDEX FIBillOfMaterialsWithEndDate ON Production.BillOfMaterials (ComponentID, StartDate) WHERE EndDate IS NOT NULL ;", 1, null);
		}

		[Test]
		public void CreateTypeAsTable() {
			ParseWithRoundtrip(@"CREATE TYPE dbo.LocationTableType AS TABLE 
    ( LocationName VARCHAR(50)
    , CostRate INT );", 1, null);
		}

		[Test]
		public void CreateTypeFrom() {
			ParseWithRoundtrip(@"CREATE TYPE dbo.SSN
FROM varchar(11) NOT NULL ;", 1, null);
		}

		[Test]
		public void DateTime2SpecificVariable() {
			ParseWithRoundtrip(@"DECLARE @dt datetime2(4)", 1, null);
		}

		[Test]
		public void DateTime2Variable() {
			ParseWithRoundtrip(@"DECLARE @dt datetime2", 1, null);
		}

		[Test]
		public void DeclareVariant() {
			ParseWithRoundtrip(@"DECLARE @v sql_variant", 1, null);
		}

		[Test]
		public void DropType() {
			ParseWithRoundtrip(@"DROP TYPE dbo.SSN;", 1, null);
		}

		[Test]
		public void Fetch() {
			ParseWithRoundtrip(@"FETCH @curs", 1, null);
		}

		[Test]
		public void FetchAbsoluteVariable() {
			ParseWithRoundtrip(@"FETCH ABSOLUTE @i FROM curs INTO @a, @b", 1, null);
		}

		[Test]
		public void FetchGlobal() {
			ParseWithRoundtrip(@"FETCH NEXT FROM GLOBAL curs", 1, null);
		}

		[Test]
		public void FetchInto() {
			ParseWithRoundtrip(@"FETCH FIRST FROM curs INTO @a, @b", 1, null);
		}

		[Test]
		public void FetchRelativeInt() {
			ParseWithRoundtrip(@"FETCH RELATIVE 12 FROM curs INTO @a, @b", 1, null);
		}

		[Test]
		public void GetGrammar() {
			Expect(ScriptParser.GetGrammar(), Not.Null);
		}

		[Test]
		public void GetSemanticActions() {
			Expect(ScriptParser.GetSemanticActions(), Not.Null);
		}

		[Test]
		public void InsertValuesMultiple() {
			ParseWithRoundtrip(@"INSERT INTO [dbo].[tblStatusTransition]([uidStatusFrom], [uidStatusTo], [bForecastedValuesOnly], [sFunction])
VALUES (N'4d460bf8-ef37-e211-ace9-8c598b00dad1', N'17bd16ff-ef37-e211-ace9-8c598b00dad1', 0, N'Status:Commit'),
(N'17bd16ff-ef37-e211-ace9-8c598b00dad1', N'4d460bf8-ef37-e211-ace9-8c598b00dad1', 0, N'Status:Verify'),
(N'17bd16ff-ef37-e211-ace9-8c598b00dad1', N'18bd16ff-ef37-e211-ace9-8c598b00dad1', 0, N'Status:Verify'),
(N'18bd16ff-ef37-e211-ace9-8c598b00dad1', N'4d460bf8-ef37-e211-ace9-8c598b00dad1', 0, N'Status:Restate'),
(N'18bd16ff-ef37-e211-ace9-8c598b00dad1', N'4d460bf8-ef37-e211-ace9-8c598b00dad1', 1, N'Status:RestateForecast');", 1, null);
		}

		[Test]
		public void InsertValuesSingle() {
			ParseWithRoundtrip(@"INSERT INTO [dbo].[tblStatusTransition]([uidStatusFrom], [uidStatusTo], [bForecastedValuesOnly], [sFunction])
VALUES (N'4d460bf8-ef37-e211-ace9-8c598b00dad1', N'17bd16ff-ef37-e211-ace9-8c598b00dad1', 0, N'Status:Commit');", 1, null);
		}

		[Test]
		public void JoinHashHint() {
			ParseWithRoundtrip(@"SELECT * FROM a INNER HASH JOIN b ON a.x=b.x", 1, null);
		}

		[Test]
		public void JoinLoopHint() {
			ParseWithRoundtrip(@"SELECT * FROM a INNER LOOP JOIN b ON a.x=b.x", 1, null);
		}

		[Test]
		public void JoinMergeHint() {
			ParseWithRoundtrip(@"SELECT * FROM a INNER MERGE JOIN b ON a.x=b.x", 1, null);
		}

		[Test]
		public void LikePredicate() {
			ParseWithRoundtrip(@"SELECT * FROM x WHERE x.y LIKE 'a'", 1, null);
		}

		[Test]
		public void LikeWithCollationPredicate() {
			ParseWithRoundtrip(@"SELECT * FROM x WHERE x.y LIKE 'a' COLLATE Latin1_General_BIN2", 1, null);
		}

		[Test]
		public void LikeWithExpressionPredicate() {
			ParseWithRoundtrip(@"SELECT * FROM x WHERE x.y LIKE 'a%'+x.z", 1, null);
		}

		[Test]
		public void MergeUpdateDelete() {
			ParseWithRoundtrip(
					@"MERGE Production.ProductInventory AS target
USING (SELECT ProductID, SUM(OrderQty) FROM Sales.SalesOrderDetail AS sod
    JOIN Sales.SalesOrderHeader AS soh
    ON sod.SalesOrderID = soh.SalesOrderID
    AND soh.OrderDate = @OrderDate
    GROUP BY ProductID) AS source (ProductID, OrderQty)
ON (target.ProductID = source.ProductID)
WHEN MATCHED AND target.Quantity - source.OrderQty <= 0
    THEN DELETE
WHEN MATCHED 
    THEN UPDATE SET target.Quantity = target.Quantity - source.OrderQty, 
                    target.ModifiedDate = GETDATE()
OUTPUT $action, Inserted.ProductID, Inserted.Quantity, Inserted.ModifiedDate, Deleted.ProductID,
    Deleted.Quantity, Deleted.ModifiedDate;",
					1, null);
		}

		[Test]
		public void MergeUpdateInsert() {
			ParseWithRoundtrip(
					@"MERGE Production.UnitMeasure AS target
    USING (SELECT @UnitMeasureCode, @Name) AS source (UnitMeasureCode, Name)
    ON (target.UnitMeasureCode = source.UnitMeasureCode)
    WHEN MATCHED THEN 
        UPDATE SET Name = source.Name
	WHEN NOT MATCHED THEN	
	    INSERT (UnitMeasureCode, Name)
	    VALUES (source.UnitMeasureCode, source.Name)
	    OUTPUT deleted.*, $action, inserted.* INTO #MyTempTable;",
					1, null);
		}

		[Test]
		public void MergeUpdateInsert2() {
			ParseWithRoundtrip(
					@"MERGE INTO Sales.SalesReason AS Target
USING (VALUES ('Recommendation','Other'), ('Review', 'Marketing'), ('Internet', 'Promotion'))
       AS Source (NewName, NewReasonType)
ON Target.Name = Source.NewName
WHEN MATCHED THEN
	UPDATE SET ReasonType = Source.NewReasonType
WHEN NOT MATCHED BY TARGET THEN
	INSERT (Name, ReasonType) VALUES (NewName, NewReasonType)
OUTPUT $action INTO @SummaryOfChanges;",
					1, null);
		}

		[Test]
		public void ParseCreateComplexTableFunction() {
			ParseWithRoundtrip(
					@"CREATE FUNCTION SPLIT
(
  @s nvarchar(max),
  @trimPieces bit,
  @returnEmptyStrings bit
)
returns @t table (val nvarchar(max))
as
begin

declare @i int, @j int;
select @i = 0, @j = (len(@s) - len(replace(@s,',','')))

;with cte 
as
(
  select
    i = @i + 1,
    s = @s, 
    n = substring(@s, 0, charindex(',', @s)),
    m = substring(@s, charindex(',', @s)+1, len(@s) - charindex(',', @s))

  union all

  select 
    i = cte.i + 1,
    s = cte.m, 
    n = substring(cte.m, 0, charindex(',', cte.m)),
    m = substring(
      cte.m,
      charindex(',', cte.m) + 1,
      len(cte.m)-charindex(',', cte.m)
    )
  from cte
  where i <= @j
)
insert into @t (val)
select pieces
from 
(
  select 
  case 
    when @trimPieces = 1
    then ltrim(rtrim(case when i <= @j then n else m end))
    else case when i <= @j then n else m end
  end as pieces
  from cte
) t
where
  (@returnEmptyStrings = 0 and len(pieces) > 0)
  or (@returnEmptyStrings = 1)
option (maxrecursion 0);


return;

end",
					1, null);
		}

		[Test]
		public void ParseCreateProcedure() {
			ParseWithRoundtrip(
					@"CREATE PROCEDURE [dbo].[prcIndicatorStatusSet]
    @uidIndicatorStatus [uniqueidentifier],
    @bEditable [bit],
    @xMetadata [xml]
AS
    BEGIN
        SET NOCOUNT ON;
        SET XACT_ABORT ON;
        IF @uidIndicatorStatus IS NULL BEGIN
            SET @uidIndicatorStatus=NEWID();
            SET ROWCOUNT 0;
        END ELSE BEGIN
            UPDATE [dbo].[tblIndicatorStatus] WITH (UPDLOCK)
                SET [bEditable]=COALESCE(@bEditable, [bEditable]), [xMetadata]=COALESCE(@xMetadata, [xMetadata]) WHERE [tblIndicatorStatus].[uidIndicatorStatus]=@uidIndicatorStatus;
        END;
        IF @@ROWCOUNT=0 BEGIN
            INSERT 
                INTO [dbo].[tblIndicatorStatus] ([uidIndicatorStatus], [bEditable], [xMetadata])
                VALUES (@uidIndicatorStatus, @bEditable, @xMetadata);
        END;
        SELECT *
            FROM [dbo].[vwIndicatorStatus]
            WHERE [vwIndicatorStatus].[uidIndicatorStatus]=@uidIndicatorStatus;
    END;",
					1, null);
		}

		[Test]
		public void ParseExecWithMultipleArguments() {
			ParseWithRoundtrip(@"EXEC spMyProc 'a', 24, @b = 10, @@rownumber, @c, @d = @e", 1, null);
		}

		[Test]
		public void ParseMultilineStringLiteral() {
			ParseWithRoundtrip(@"SELECT N'This
is
on
several lines!'", 1, null);
		}

		[Test]
		public void ParseName() {
			ParseWithRoundtrip(NameOnlyStatement.Key+" [dbo].[SomeName]", 1, null);
		}

		[Test]
		public void ParseObjectId1() {
			ParseWithRoundtrip(@"SELECT OBJECT_ID('[dbo].[SomeName]')", 1, null);
		}

		[Test]
		public void ParseObjectId2() {
			ParseWithRoundtrip(@"SELECT OBJECT_ID(N'[dbo].[SomeName]', N'U')", 1, null);
		}

		[Test]
		public void ParsePredicateSubqueryAll() {
			ParseWithRoundtrip(@"IF 1 = ALL (SELECT colA FROM tblB) PRINT 'All';", 1, null);
		}

		[Test]
		public void ParsePredicateSubqueryAny() {
			ParseWithRoundtrip(@"IF 1 = ANY (SELECT colA FROM tblB) PRINT 'Any';", 1, null);
		}

		[Test]
		public void ParsePredicateSubqueryExact() {
			ParseWithRoundtrip(@"IF 1 = (SELECT colA FROM tblB) PRINT 'Exact';", 1, null);
		}

		[Test]
		public void ParsePredicateSubquerySome() {
			ParseWithRoundtrip(@"IF 1 = SOME (SELECT colA FROM tblB) PRINT 'Any';", 1, null);
		}

		[Test]
		public void ParseSimpleCTESelect() {
			ParseWithRoundtrip(@"with MyCTE(x)
as
(
select x = convert(varchar(8000),'hello')
union all
select x + 'a' from MyCTE where len(x) < 100
)
select x from MyCTE", 1, null);
		}

		[Test]
		public void ParseWithComments() {
			ParseWithRoundtrip(@"-- Line comment
BEGIN
/* block comment
on 2 lines */
SELECT * -- Comment inside select
FROM [tbl];
-- Comment may move
END;", 1, null);
		}

		[Test]
		public void ParseWithMissingTerminator() {
			ParseWithRoundtrip(@"SELECT * FROM [tblA]
SELECT * FROM [tblB]
PRINT 'Cool'", 3, null);
		}

		[Test]
		public void RaiserrorNoOption() {
			ParseWithRoundtrip(@"RAISERROR (N'<<%*.*s>>', -- Message text.
           10, -- Severity,
           1, -- State,
           7, -- First argument used for width.
           3, -- Second argument used for precision.
           N'abcde');", 1, null);
		}

		[Test]
		public void RaiserrorWithOption() {
			ParseWithRoundtrip(@"RAISERROR (N'<<%7.3s>>', -- Message text.
           10, -- Severity,
           1, -- State,
           N'abcde') WITH NOWAIT, LOG;", 1, null);
		}

		[Test]
		public void RollbackTransaction() {
			ParseWithRoundtrip(@"ROLLBACK TRAN", 1, null);
		}

		[Test]
		public void RollbackTransactionIdentifierName() {
			ParseWithRoundtrip(@"ROLLBACK TRANSACTION MyTrans", 1, null);
		}

		[Test]
		public void RollbackTransactionLegacy() {
			ParseWithRoundtrip(@"ROLLBACK", 1, null);
		}

		[Test]
		public void RollbackTransactionVariableName() {
			ParseWithRoundtrip(@"ROLLBACK TRANSACTION @trans", 1, null);
		}

		[Test]
		public void SaveTransactionIdentifierName() {
			ParseWithRoundtrip(@"SAVE TRANSACTION MyTrans", 1, null);
		}

		[Test]
		public void SaveTransactionVariableName() {
			ParseWithRoundtrip(@"SAVE TRANSACTION @trans", 1, null);
		}

		[Test]
		public void SelectExcept() {
			ParseWithRoundtrip(@"SELECT * FROM TableA EXCEPT SELECT * FROM TableB", 1, null);
		}

		[Test]
		public void SelectExceptIntersect() {
			ParseWithRoundtrip(@"SELECT * FROM TableA EXCEPT SELECT * FROM TableB INTERSECT SELECT * FROM TableC", 1, null);
		}

		[Test]
		public void SelectGroupByHaving() {
			ParseWithRoundtrip(@"SELECT a FROM tbl GROUP BY a HAVING SUM(b)>0", 1, null);
		}

		[Test]
		public void SelectIntersect() {
			ParseWithRoundtrip(@"SELECT * FROM dbo.TableA INTERSECT SELECT * FROM dbo.TableB", 1, "dbo");
		}

		[Test]
		public void SelectValuesRowset() {
			ParseWithRoundtrip(@"SELECT * FROM (VALUES (1,2,3),(4,5,6)) AS Ints (x, y, z);", 1, "dbo");
		}

		[Test]
		public void SelectVariableWhere() {
			ParseWithRoundtrip(@"SELECT @x=1 WHERE EXISTS (SELECT * FROM dbo.tbl)", 1, "dbo");
		}

		[Test]
		public void SelectWithCrossApplyAndXmlFunction() {
			ParseWithRoundtrip(
					@"SELECT [p].[uidPeriodStructureIndicator], [d].[uidPeriodIndicator].value('.', 'uniqueidentifier') AS [uidPeriodIndicatorDependency]
            FROM @tblPeriodStructureIndicator AS [p]
            CROSS APPLY [p].[xDependencies].nodes('/*/id') AS [d]([uidPeriodIndicator])", 1, "dbo");
		}

		[Test]
		public void SelectWithCrossApplyAndXmlFunctions() {
			ParseWithRoundtrip(
					@"SELECT ProductModelID, Locations.value('./@LocationID','int') as LocID,
steps.query('.') as Step       
FROM Production.ProductModel       
CROSS APPLY Instructions.nodes('/MI:root/MI:Location') as T1(Locations)       
CROSS APPLY T1.Locations.nodes('/MI:step ') as T2(steps)       
WHERE ProductModelID=7;",
					1, null);
		}

		[Test]
		public void SelectWithHints() {
			ParseWithRoundtrip(@"SELECT @a=1 OPTION (MAXRECURSION 0, RECOMPILE);", 1, null);
		}

		[Test]
		public void SelectWithNestedQueryWithXmlFunctions() {
			ParseWithRoundtrip(@"SELECT Fname, count(Fname) FROM    
  (SELECT nref.value('(author/first-name)[1]', 'nvarchar(max)') Fname
   FROM   docs CROSS APPLY xCol.nodes('/book') T(nref)
   WHERE  nref.exist ('author/first-name') = 1) Result
GROUP BY FName
ORDER BY Fname;", 1, null);
		}

		[Test]
		public void SelectWithXmlnamespaces() {
			ParseWithRoundtrip(
					@"WITH XMLNAMESPACES (
   'http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ProductModelManuInstructions' AS MI)
SELECT ProductModelID, 
       Locations.value('./@LocationID','int') as LocID,
       steps.query('.') as Steps
FROM   Production.ProductModel
CROSS APPLY Instructions.nodes('/MI:root/MI:Location') as T1(Locations)
CROSS APPLY T1.Locations.nodes('./MI:step') as T2(steps)
WHERE  ProductModelID=7
AND    steps.exist('./MI:tool') = 1;",
					1, null);
		}

		[Test]
		public void SelectWithXmlnamespacesAndCte() {
			ParseWithRoundtrip(
					@"WITH XMLNAMESPACES (
   'http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ProductModelManuInstructions' AS MI),
cteModel AS (SELECt * FROM Production.ProductModel)
SELECT ProductModelID, 
       Locations.value('./@LocationID','int') as LocID,
       steps.query('.') as Steps
FROM   cteModel
CROSS APPLY Instructions.nodes('/MI:root/MI:Location') as T1(Locations)
CROSS APPLY T1.Locations.nodes('./MI:step') as T2(steps)
WHERE  ProductModelID=7
AND    steps.exist('./MI:tool') = 1;",
					1, null);
		}

		[Test]
		public void SetTransactionIsolationLevel() {
			ParseWithRoundtrip(@"SET TRANSACTION ISOLATION LEVEL SERIALIZABLE", 1, null);
		}

		[Test]
		public void SetXmlModfiy() {
			ParseWithRoundtrip(@"SET @x.modify('insert sql:variable(""@newFeatures"") into (/Root/ProductDescription/Features)[1]')", 1, null);
		}

		[Test]
		public void StatementsWithXmlFunctions() {
			ParseWithRoundtrip(@"DECLARE @tbl TABLE (id xml);
INSERT @tbl (id) SELECT t.x FROM (SELECT NEWID() x FOR XML RAW) t(x);
SELECT id.[query]('data(*/@x)').query('*') FROM @tbl;", 3, null);
		}

		[Test]
		public void SyntaxError() {
			Expect(() => ParseWithRoundtrip(@"SELECT * FROM TableA 'Error'", 1, null), Throws.InstanceOf<ParseException>().With.Message.ContainsSubstring("SyntaxError"));
		}

		[Test]
		public void UpdateSetXmlColumnModfiy() {
			ParseWithRoundtrip(@"UPDATE tbl SET x.modify('insert sql:variable(""@newFeatures"") into (/Root/ProductDescription/Features)[1]'), tbl.y=1", 1, null);
		}

		[Test]
		public void UpdateSetXmlQualifiedColumnModfiy() {
			ParseWithRoundtrip(@"UPDATE tbl SET tbl.x.modify('insert sql:variable(""@newFeatures"") into (/Root/ProductDescription/Features)[1]'), tbl.y=1", 1, null);
		}

		[Test]
		public void UpdateSetXmlVariableColumnModfiy() {
			ParseWithRoundtrip(@"UPDATE @tbl SET [@tbl].x.modify('insert sql:variable(""@newFeatures"") into (/Root/ProductDescription/Features)[1]'), tbl.y=1", 1, null);
		}

		[Test]
		public void UpdateSetXmlVariableModfiy() {
			ParseWithRoundtrip(@"UPDATE tbl SET @x.modify('insert sql:variable(""@newFeatures"") into (/Root/ProductDescription/Features)[1]'), tbl.y=1", 1, null);
		}

		[Test]
		public void UpdateWithTableHint() {
			ParseWithRoundtrip(@"UPDATE Production.Product
WITH (TABLOCK)
SET ListPrice = ListPrice * 1.10
WHERE ProductNumber LIKE 'BK-%';", 1, null);
		}
	}
}
