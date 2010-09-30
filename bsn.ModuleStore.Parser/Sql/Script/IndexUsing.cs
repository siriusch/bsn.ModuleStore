using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class IndexUsing: SqlScriptableToken, IOptional {
		private readonly IndexName indexName;

		[Rule("<IndexUsing> ::=")]
		public IndexUsing(): this(null) {}

		protected IndexUsing(IndexName indexName) {
			this.indexName = indexName;
		}

		public virtual IndexFor IndexFor {
			get {
				return IndexFor.None;
			}
		}

		public IndexName IndexName {
			get {
				return indexName;
			}
		}

		protected virtual string IndexForSpecifier {
			get {
				return string.Empty;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			if (HasValue) {
				writer.Write("USING XML INDEX ");
				writer.WriteScript(indexName, WhitespacePadding.None);
				writer.Write(" FOR ");
				writer.Write(IndexForSpecifier);
			}
		}

		public bool HasValue {
			get {
				return IndexFor != IndexFor.None;
			}
		}
	}
}