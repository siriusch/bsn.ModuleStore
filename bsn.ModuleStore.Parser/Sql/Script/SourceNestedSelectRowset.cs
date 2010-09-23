using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SourceNestedSelectRowset: SourceRowset {
		private readonly SelectQuery @select;

		[Rule("<SourceRowset> ::= ~'(' <SelectQuery> ~')' <RowsetAlias>")]
		public SourceNestedSelectRowset(SelectQuery select, RowsetAlias rowsetAlias): base(rowsetAlias) {
			Debug.Assert(select != null);
			this.@select = select;
		}

		public SelectQuery Select {
			get {
				return select;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write('(');
			writer.IncreaseIndent();
			writer.WriteScript(select, WhitespacePadding.NewlineBefore);
			writer.DecreaseIndent();
			writer.WriteLine();
			writer.Write(')');
			base.WriteTo(writer);
		}
	}
}