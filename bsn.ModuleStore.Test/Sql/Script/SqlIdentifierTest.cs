using System;
using System.Linq;

using NUnit.Framework;

namespace bsn.ModuleStore.Sql.Script {
	[TestFixture]
	public class SqlIdentifierTest: AssertionHelper {
		[Test]
		public void TryDequoteBracket() {
			string result;
			Expect(QuotedIdentifier.TryDequote("[Cool \" Stuff]", out result));
			Expect(result, EqualTo("Cool \" Stuff"));
		}

		[Test]
		public void TryDequoteQuote() {
			string result;
			Expect(QuotedIdentifier.TryDequote("\"Cool [ Stuff\"", out result));
			Expect(result, EqualTo("Cool [ Stuff"));
		}

		[Test]
		public void TryDequoteQuoteWithInnerQuote() {
			string result;
			Expect(QuotedIdentifier.TryDequote("\"Cool \"\" Stuff\"", out result));
			Expect(result, EqualTo("Cool \" Stuff"));
		}
	}
}