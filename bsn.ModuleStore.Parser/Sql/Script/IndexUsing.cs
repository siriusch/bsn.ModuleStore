using System;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class IndexUsing: SqlScriptableToken, IOptional {
		private readonly IndexFor indexFor;
		private readonly IndexName indexName;

		[Rule("<IndexUsing> ::=")]
		public IndexUsing(): this(null, null) {}

		[Rule("<IndexUsing> ::= ~USING_XML_INDEX <IndexName> FOR_VALUE")]
		[Rule("<IndexUsing> ::= ~USING_XML_INDEX <IndexName> FOR_PATH")]
		[Rule("<IndexUsing> ::= ~USING_XML_INDEX <IndexName> FOR_PROPERTY")]
		public IndexUsing(IndexName indexName, IndexForToken indexFor) {
			this.indexName = indexName;
			this.indexFor = indexFor.IndexFor;
		}

		public IndexFor IndexFor {
			get {
				return indexFor;
			}
		}

		public IndexName IndexName {
			get {
				return indexName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			if (HasValue) {
				writer.Write("USING XML INDEX ");
				writer.WriteScript(indexName, WhitespacePadding.None);
				writer.WriteEnum(indexFor, WhitespacePadding.SpaceBefore);
			}
		}

		public bool HasValue {
			get {
				return indexName != null;
			}
		}
	}
}