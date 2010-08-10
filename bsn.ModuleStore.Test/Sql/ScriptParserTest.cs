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
			ParseWithRoundtrip("BREAK; CONTINUE", 2);
		}
	}
}
