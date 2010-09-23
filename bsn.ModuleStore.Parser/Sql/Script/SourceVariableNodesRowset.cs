using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SourceVariableNodesRowset: SourceNodesRowset {
		private readonly VariableName variableName;

		[Rule("<SourceRowset> ::= <VariableName> ~'.' <NamedFunction> <RowsetAlias>")]
		public SourceVariableNodesRowset(VariableName variableName, NamedFunction functionCall, RowsetAlias rowsetAlias): base(functionCall, rowsetAlias) {
			this.variableName = variableName;
		}

		public VariableName VariableName {
			get {
				return variableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(variableName, WhitespacePadding.None);
			writer.Write('.');
			base.WriteTo(writer);
		}
	}
}