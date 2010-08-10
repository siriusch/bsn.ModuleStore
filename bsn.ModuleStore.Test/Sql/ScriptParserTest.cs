using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using bsn.ModuleStore.Sql.Script;

using NUnit.Framework;

namespace bsn.ModuleStore.Sql {
	[TestFixture]
	public class ScriptParserTest: AssertionHelper {
		public List<Statement> ParseWithRoundtrip(string sql, int expectedStatementCount) {
			List<Statement> statements = ScriptParser.Parse(sql).ToList();
			Expect(statements.Count, EqualTo(expectedStatementCount));
			string sqlGen = GenerateSql(statements);
			Trace.Write(Environment.NewLine+sqlGen, "Generated SQL");
			Expect(sqlGen, EqualTo(GenerateSql(ScriptParser.Parse(sqlGen))));
			return statements;
		}

		private string GenerateSql(IEnumerable<Statement> statements) {
			StringWriter sqlGen = new StringWriter();
			foreach (Statement statement in statements) {
				statement.WriteTo(sqlGen);
				sqlGen.WriteLine(";");
			}
			return sqlGen.ToString();
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
		public void Parse() {
			ParseWithRoundtrip(@"CREATE FUNCTION SPLIT
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
  or (@returnEmptyStrings = 1);


return;

end", 1);
		}
	}
}
