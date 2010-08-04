using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class Optional<T>: SqlToken where T: SqlToken {
		public static implicit operator T(Optional<T> instance) {
			if (instance != null) {
				return instance.Value;
			}
			return null;
		}

		private readonly T value;

		[Rule("<FulltextColumnType> ::=", typeof(Qualified<TypeName>))]
		[Rule("<FulltextColumnGroup> ::=", typeof(Sequence<FulltextColumn>))]
		[Rule("<OptionalCollate> ::=", typeof(CollationName))]
		[Rule("<OptionalLanguage> ::=", typeof(LanguageLcid))]
		[Rule("<OptionalDefault> ::=", typeof(Literal))]
		[Rule("<OptionalOpenxmlSchema> ::=", typeof(OpenxmlSchema))]
		[Rule("<OptionalFulltextChangeTracking> ::=", typeof(FulltextChangeTracking))]
		[Rule("<OptionalFunctionParameterList> ::=", typeof(Sequence<FunctionParameter>))]
		[Rule("<OptionalFunctionOption> ::=", typeof(FunctionOption))]
		[Rule("<OptionalVarying> ::=", typeof(Varying))]
		[Rule("<OptionalOutput> ::=", typeof(Output))]
		[Rule("<OptionalReadonly> ::=", typeof(Identifier))]
		[Rule("<ExecuteParameterGroup> ::=", typeof(Sequence<ExecuteParameter>))]
		[Rule("<ProcedureParameterGroup> ::=", typeof(Sequence<ProcedureParameter>))]
		[Rule("<ProcedureFor> ::=", typeof(ForReplication))]
		[Rule("<ProcedureOptionGroup> ::=", typeof(WithRecompile))]
		[Rule("<ViewOptionalAttribute> ::=", typeof(WithViewMetadata))]
		[Rule("<ViewOptionalCheckOption> ::=", typeof(WithCheckOption))]
		[Rule("<ColumnNameGroup> ::=", typeof(Sequence<ColumnName>))]
		public Optional(): this(null) {}

		[Rule("<FulltextColumnType> ::= TYPE_COLUMN <TypeNameQualified>", typeof(Qualified<TypeName>), ConstructorParameterMapping = new[] {1})]
		[Rule("<FulltextColumnGroup> ::= '(' <FulltextColumnList> ')'", typeof(Sequence<FulltextColumn>), ConstructorParameterMapping = new[] {1})]
		[Rule("<OptionalCollate> ::= COLLATE <CollationName>", typeof(CollationName), ConstructorParameterMapping = new[] {1})]
		[Rule("<OptionalLanguage> ::= LANGUAGE_LCID", typeof(LanguageLcid))]
		[Rule("<OptionalDefault> ::= '=' <Literal>", typeof(Literal), ConstructorParameterMapping = new[] {1})]
		[Rule("<OptionalOpenxmlSchema> ::= <OpenxmlImplicitSchema>", typeof(OpenxmlSchema))]
		[Rule("<OptionalOpenxmlSchema> ::= <OpenxmlExplicitSchema>", typeof(OpenxmlSchema))]
		[Rule("<OptionalFulltextChangeTracking> ::= <FulltextChangeTracking>", typeof(FulltextChangeTracking))]
		[Rule("<OptionalFunctionParameterList> ::= <FunctionParameterList>", typeof(Sequence<FunctionParameter>))]
		[Rule("<OptionalFunctionOption> ::= WITH <FunctionOption>", typeof(FunctionOption), ConstructorParameterMapping = new[] {1})]
		[Rule("<OptionalVarying> ::= VARYING", typeof(Varying))]
		[Rule("<OptionalOutput> ::= OUTPUT", typeof(Output))]
		[Rule("<OptionalReadonly> ::= Id", typeof(Identifier))]
		[Rule("<ExecuteParameterGroup> ::= <ExecuteParameterList>", typeof(Sequence<ExecuteParameter>))]
		[Rule("<ProcedureParameterGroup> ::= <ProcedureParameterList>", typeof(Sequence<ProcedureParameter>))]
		[Rule("<ProcedureFor> ::= FOR_REPLICATION", typeof(ForReplication))]
		[Rule("<ProcedureOptionGroup> ::= WITH_RECOMPILE", typeof(WithRecompile))]
		[Rule("<ViewOptionalAttribute> ::= WITH_VIEW_METADATA", typeof(WithViewMetadata))]
		[Rule("<ViewOptionalCheckOption> ::= WITH_CHECK_OPTION", typeof(WithCheckOption))]
		[Rule("<ColumnNameGroup> ::= '(' <ColumnNameList> ')'", typeof(Sequence<ColumnName>), ConstructorParameterMapping=new[] { 1 })]
		public Optional(T value) {
			this.value = value;
		}

		public T Value {
			get {
				return value;
			}
		}

		public override void WriteTo(TextWriter writer) {
			throw new NotSupportedException();
		}
	}
}