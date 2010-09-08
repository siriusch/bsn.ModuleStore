using System;
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class FromClause: SqlScriptableToken {
		private readonly List<Join> joins;
		private readonly Source source;

		[Rule("<FromClause> ::= FROM <Source> <JoinChain>", ConstructorParameterMapping = new[] {1, 2})]
		public FromClause(Source source, Sequence<Join> join) {
			Debug.Assert(source != null);
			this.source = source;
			this.joins = join.ToList();
		}

		public IEnumerable<Join> Joins {
			get {
				return joins;
			}
		}

		public Source Source {
			get {
				return source;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("FROM ");
			writer.WriteScript(source, WhitespacePadding.None);
			writer.WriteScriptSequence(joins, WhitespacePadding.NewlineBefore, null);
		}
	}
}