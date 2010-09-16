using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class PredicateParens: Predicate {
		private readonly Predicate predicate;

		[Rule("<PredicateParens> ::= ~'(' <Predicate> ~')'")]
		public PredicateParens(Predicate predicate) {
			Debug.Assert(predicate != null);
			PredicateParens parens = predicate as PredicateParens;
			this.predicate = parens != null ? parens.predicate : predicate;
		}

		public Predicate Predicate {
			get {
				return predicate;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write('(');
			writer.WriteScript(predicate, WhitespacePadding.None);
			writer.Write(')');
		}
	}
}