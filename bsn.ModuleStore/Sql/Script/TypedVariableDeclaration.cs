using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class TypedVariableDeclaration: DeclareStatement {
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

		public override void WriteTo(TextWriter writer) {
			writer.Write("DECLARE ");
			Variable.WriteTo(writer);
			writer.Write(" ");
			typeName.WriteTo(writer);
			if (initialization != null) {
				writer.Write(" = ");
				initialization.WriteTo(writer);
			}
		}
	}
}