using System;
using System.Linq;

using NUnit.Framework;

namespace bsn.ModuleStore.Sql.Script {
	[TestFixture]
	public class SqlIdentifierTest: AssertionHelper {
		[Test]
		public void TryDequoteBracket() {
			string result;
			Expect(SqlIdentifier.TryDequote("[Cool \" Stuff]", out result));
			Expect(result, EqualTo("Cool \" Stuff"));
		}

		[Test]
		public void TryDequoteInvalidBracket() {
			string result;
			Expect(!SqlIdentifier.TryDequote("Some[Identifier", out result));
			Expect(result, Null);
		}

		[Test]
		public void TryDequoteInvalidQuote() {
			string result;
			Expect(!SqlIdentifier.TryDequote("Some\"Identifier", out result));
			Expect(result, Null);
		}

		[Test]
		public void TryDequoteInvalidText() {
			string result;
			Expect(!SqlIdentifier.TryDequote("Some Identifier", out result));
			Expect(result, Null);
		}

		[Test]
		public void TryDequoteNoQuote() {
			string result;
			Expect(SqlIdentifier.TryDequote("SomeIdentifier", out result));
			Expect(result, EqualTo("SomeIdentifier"));
		}

		[Test]
		public void TryDequoteQuote() {
			string result;
			Expect(SqlIdentifier.TryDequote("\"Cool [ Stuff\"", out result));
			Expect(result, EqualTo("Cool [ Stuff"));
		}
	}
}