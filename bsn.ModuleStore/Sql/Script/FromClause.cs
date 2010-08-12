using System;
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class FromClause: SqlScriptableToken {
		private readonly List<Join> join;
		private readonly SourceRowset sourceRowset;

		[Rule("<FromClause> ::= FROM <SourceRowset> <JoinChain>", ConstructorParameterMapping = new[] {1, 2})]
		public FromClause(SourceRowset sourceRowset, Sequence<Join> join) {
			Debug.Assert(sourceRowset != null);
			this.sourceRowset = sourceRowset;
			this.join = join.ToList();
		}

		public List<Join> Join {
			get {
				return join;
			}
		}

		public SourceRowset SourceRowset {
			get {
				return sourceRowset;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("FROM ");
			writer.WriteScript(sourceRowset, WhitespacePadding.None);
		}
	}
}