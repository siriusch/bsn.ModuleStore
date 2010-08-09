using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TypedVariableDeclaration: DeclareStatement {
		private readonly Expression initialization;
		private readonly Qualified<TypeName> typeName;

		[Rule("<DeclareItem> ::= <VariableName> <OptionalAs> <TypeNameQualified>", ConstructorParameterMapping = new[] {0, 2})]
		public TypedVariableDeclaration(VariableName variable, Qualified<TypeName> typeName): this(variable, typeName, null) {}

		[Rule("<DeclareItem> ::= <VariableName> <OptionalAs> <TypeNameQualified> '=' <Expression>", ConstructorParameterMapping = new[] {0, 2, 4})]
		public TypedVariableDeclaration(VariableName variable, Qualified<TypeName> typeName, Expression initialization): base(variable) {
			if (typeName == null) {
				throw new ArgumentNullException("typeName");
			}
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

		public override void WriteTo(TextWriter writer) {
			writer.Write("DECLARE ");
			writer.WriteScript(Variable);
			writer.Write(" ");
			writer.WriteScript(typeName);
			writer.WriteScript(initialization, "=", null);
		}
	}
}