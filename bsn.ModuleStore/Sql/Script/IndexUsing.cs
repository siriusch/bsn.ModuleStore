using System;
using System.IO;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class IndexUsing: SqlToken, IScriptable, IOptional {
		private readonly IndexName indexName;
		private readonly IndexFor indexFor;

		[Rule("<IndexUsing> ::=")]
		public IndexUsing(): this(null, null) {}

		[Rule("<IndexUsing> ::= USING_XML_INDEX <IndexName> FOR_VALUE", ConstructorParameterMapping = new[] {1, 2})]
		[Rule("<IndexUsing> ::= USING_XML_INDEX <IndexName> FOR_PATH", ConstructorParameterMapping=new[] { 1, 2 })]
		[Rule("<IndexUsing> ::= USING_XML_INDEX <IndexName> FOR_PROPERTY", ConstructorParameterMapping=new[] { 1, 2 })]
		public IndexUsing(IndexName indexName, IndexForToken indexFor) {
			this.indexName = indexName;
			this.indexFor = indexFor.IndexFor;
		}

		public IndexName IndexName {
			get {
				return indexName;
			}
		}

		public IndexFor IndexFor {
			get {
				return indexFor;
			}
		}

		public void WriteTo(TextWriter writer) {
			if (HasValue) {
				writer.Write("USING XML INDEX ");
				writer.WriteScript(indexName);
				writer.WriteValue(indexFor, " ", null);
			}
		}

		public bool HasValue {
			get {
				return indexName != null;
			}
		}
	}
}