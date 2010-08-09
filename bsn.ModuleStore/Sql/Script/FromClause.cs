using System;
using System.Collections.Generic;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class FromClause: SqlToken, IScriptable {
		private readonly SourceRowset sourceRowset;
		private readonly List<Join> join;

		[Rule("<FromClause> ::= FROM <SourceRowset> <JoinChain>", ConstructorParameterMapping = new[] {1, 2})]
		public FromClause(SourceRowset sourceRowset, Sequence<Join> join) {
			if (sourceRowset == null) {
				throw new ArgumentNullException("sourceRowset");
			}
			this.sourceRowset = sourceRowset;
			this.join = join.ToList();
		}

		public SourceRowset SourceRowset {
			get {
				return sourceRowset;
			}
		}

		public List<Join> Join {
			get {
				return join;
			}
		}

		public void WriteTo(TextWriter writer) {
			writer.Write("FROM ");
			writer.WriteScript(sourceRowset);
		}
	}
}