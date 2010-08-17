using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using bsn.ModuleStore.Sql.Script;

using NUnit.Framework;

namespace bsn.ModuleStore.Sql {
	[TestFixture]
	public class ScriptParserTest: AssertionHelper {
		[TestFixtureSetUp]
		public void Initialize() {
			ScriptParser.GetSemanticActions();
		}

		public List<Statement> ParseWithRoundtrip(string sql, int expectedStatementCount) {
			Stopwatch sw = new Stopwatch();
			sw.Start();
			ICollection<IQualifiedName<SchemaName>> names;
			IEnumerable<Statement> parsedStatements = ScriptParser.Parse(sql, out names);
			sw.Stop();
			long parseTime = sw.ElapsedMilliseconds;
			List<Statement> statements = parsedStatements.ToList();
			Expect(statements.Count, EqualTo(expectedStatementCount));
			sw.Reset();
			sw.Start();
			string sqlGen = GenerateSql(statements);
			sw.Stop();
			Trace.Write(Environment.NewLine+sqlGen, string.Format("Generated SQL (parse: {0}ms | gen: {1}ms)", parseTime, sw.ElapsedMilliseconds));
			sw.Reset();
			sw.Start();
			ICollection<IQualifiedName<SchemaName>> namesRoundtrip;
			string sqlGenRoundtrip = GenerateSql(ScriptParser.Parse(sqlGen, out namesRoundtrip));
			sw.Stop();
			Trace.Write(string.Format("{0}ms", sw.ElapsedMilliseconds), "Roundtrip");
			Expect(sqlGen, EqualTo(sqlGenRoundtrip));
			Expect(names.Count, EqualTo(namesRoundtrip.Count));
			return statements;
		}

		private string GenerateSql(IEnumerable<Statement> statements) {
			using (StringWriter stringWriter = new StringWriter()) {
				SqlWriter sqlGen = new SqlWriter(stringWriter);
				foreach (Statement statement in statements) {
					statement.WriteTo(sqlGen);
					sqlGen.WriteLine(";");
				}
				return stringWriter.ToString();
			}
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
					1);
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
select x from MyCTE", 1);
		}
	}
}