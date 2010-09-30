using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SourceSystemFunctionRowset<T>: SourceRowset where T: SqlScriptableToken {
		private readonly T function;

		[Rule("<SourceRowset> ::= <Openxml> <RowsetAlias>", typeof(OpenxmlFunction))]
		[Rule("<SourceRowset> ::= <TextTableFunction> <RowsetAlias>", typeof(FulltextTableFunction))]
		public SourceSystemFunctionRowset(T function, RowsetAlias rowsetAlias)
			: base(rowsetAlias) {
			Debug.Assert(function != null);
			this.function = function;
		}

		public T Function {
			get {
				return function;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(function, WhitespacePadding.None);
			base.WriteTo(writer);
		}
	}
}