using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SourceParens: Source {
		private readonly Source inner;
		private readonly List<Join> joins;

		[Rule("<Source> ::= '(' <Source> <JoinChain> ')'", ConstructorParameterMapping = new[] {1, 2})]
		public SourceParens(Source inner, Sequence<Join> joins) {
			Debug.Assert(inner != null);
			this.inner = inner;
			this.joins = joins.ToList();
		}

		public Source Inner {
			get {
				return inner;
			}
		}

		public List<Join> Joins {
			get {
				return joins;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("(");
			writer.IncreaseIndent();
			writer.WriteScript(inner, WhitespacePadding.NewlineBefore);
			writer.WriteScriptSequence(joins, WhitespacePadding.NewlineBefore, null);
			writer.DecreaseIndent();
			writer.WriteLine();
			writer.Write(")");
		}
	}
}