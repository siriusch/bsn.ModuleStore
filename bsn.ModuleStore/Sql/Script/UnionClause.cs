using System;
using System.IO;

using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class UnionClause: SqlToken, IScriptable, IOptional {
		private readonly SelectQuery selectQuery;

		[Rule("<UnionClause> ::=")]
		public UnionClause(): this(null) {
		}

		[Rule("<UnionClause> ::= UNION <SelectQuery>", ConstructorParameterMapping=new[] { 1 })]
		public UnionClause(SelectQuery selectQuery) : base() {
			this.selectQuery = selectQuery;
		}

		public virtual bool All {
			get {
				return false;
			}
		}

		public void WriteTo(TextWriter writer) {
			if (HasValue) {
				writer.Write("UNION ");
				if (All) {
					writer.Write("ALL ");
				}
				writer.WriteLine();
				writer.WriteScript(selectQuery);
			}
		}

		public bool HasValue {
			get {
				return selectQuery != null;
			}
		}
	}
}