using System;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class DeclareVariableItem: DeclareItem {
		private readonly Expression initialization;
		private readonly Qualified<TypeName> typeName;

		[Rule("<DeclareItem> ::= <VariableName> <OptionalAs> <TypeNameQualified>", ConstructorParameterMapping = new[] {0, 2})]
		public DeclareVariableItem(VariableName variable, Qualified<TypeName> typeName): this(variable, typeName, null) {}

		[Rule("<DeclareItem> ::= <VariableName> <OptionalAs> <TypeNameQualified> '=' <Expression>", ConstructorParameterMapping = new[] {0, 2, 4})]
		public DeclareVariableItem(VariableName variable, Qualified<TypeName> typeName, Expression initialization): base(variable) {
			Debug.Assert(typeName != null);
			this.typeName = typeName;
			this.initialization = initialization;
		}

		public Expression Initialization {
			get {
				return initialization;
			}
		}

		public Qualified<TypeName> TypeName {
			get {
				return typeName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			base.WriteTo(writer);
			writer.WriteScript(typeName);
			writer.WriteScript(initialization, "=", null);
		}
	}
}