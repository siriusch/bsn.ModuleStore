using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace bsn.ModuleStore.Sql {
	[TestFixture]
	public class ScriptParserTest: AssertionHelper {
		[Test]
		public void LoadGrammar() {
			Expect(ScriptParser.LoadGrammar(), Not.Null);
		}
	}
}
