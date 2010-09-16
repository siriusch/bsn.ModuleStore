using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DeclareVariableItem: DeclareItem {
		private readonly Expression initialization;
		private readonly Qualified<SchemaName, TypeName> typeName;

		[Rule("<DeclareItem> ::= <VariableName> ~<OptionalAs> <TypeNameQualified>")]
		public DeclareVariableItem(VariableName variable, Qualified<SchemaName, TypeName> typeName): this(variable, typeName, null) {}

		[Rule("<DeclareItem> ::= <VariableName> ~<OptionalAs> <TypeNameQualified> ~'=' <Expression>")]
		public DeclareVariableItem(VariableName variable, Qualified<SchemaName, TypeName> typeName, Expression initialization): base(variable) {
			Debug.Assert(typeName != null);
			this.typeName = typeName;
			this.initialization = initialization;
		}

		public Expression Initialization {
			get {
				return initialization;
			}
		}

		public Qualified<SchemaName, TypeName> TypeName {
			get {
				return typeName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.WriteScript(typeName, WhitespacePadding.None);
			writer.WriteScript(initialization, WhitespacePadding.None, "=", null);
		}
	}
}