using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class RowsetCombineClause: SqlScriptableToken {
		private readonly SelectQuery selectQuery;

		protected RowsetCombineClause(SelectQuery selectQuery): base() {
			Debug.Assert(selectQuery != null);
			this.selectQuery = selectQuery;
		}

		public SelectQuery SelectQuery {
			get {
				return selectQuery;
			}
		}

		protected abstract string CombineSpecifier {
			get;
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write(CombineSpecifier);
			writer.WriteScript(selectQuery, WhitespacePadding.NewlineBefore);
		}
	}
}