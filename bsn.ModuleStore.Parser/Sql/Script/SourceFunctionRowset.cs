using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SourceFunctionRowset: SourceRowset {
		private readonly NamedFunction function;

		[Rule("<SourceRowset> ::= <SchemaName> ~'.' <NamedFunction> <RowsetAlias>")]
		public SourceFunctionRowset(SchemaName schemaName, NamedFunction function, RowsetAlias rowsetAlias): this(function.QualifiedWith(schemaName), rowsetAlias) {}

		[Rule("<SourceRowset> ::= <NamedFunction> <RowsetAlias>")]
		public SourceFunctionRowset(NamedFunction function, RowsetAlias rowsetAlias): base(rowsetAlias) {
			Debug.Assert(function != null);
			this.function = function;
		}

		public NamedFunction Function {
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