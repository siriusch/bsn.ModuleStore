using System;
using System.Linq;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Semantic;

using NUnit.Framework;

namespace bsn.ModuleStore.Sql.Script {
	[TestFixture]
	public class SemanticActionsTest: AssertionHelper {
		private CompiledGrammar grammar;

		[TestFixtureSetUp]
		public void SetUp() {
			grammar = ScriptParser.LoadGrammar();
		}

		[Test]
		public void ConsistencyCheck() {
			new SemanticTypeActions<SqlToken>(grammar).Initialize(true);
		}
	}
}
