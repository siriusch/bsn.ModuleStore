using System;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class Optional<T>: SqlToken where T: SqlToken {
		public static implicit operator T(Optional<T> instance) {
			if (instance != null) {
				return instance.Value;
			}
			return null;
		}

		private readonly T value;

		[Rule("<FulltextColumnType> ::=", typeof(Qualified<SchemaName, TypeName>))]
		[Rule("<FulltextColumnGroup> ::=", typeof(Sequence<FulltextColumn>))]
		[Rule("<OptionalLanguage> ::=", typeof(LanguageLcid))]
		[Rule("<OptionalDefault> ::=", typeof(Literal))]
		[Rule("<OptionalOpenxmlSchema> ::=", typeof(OpenxmlSchema))]
		[Rule("<OptionalFunctionParameterList> ::=", typeof(Sequence<FunctionParameter>))]
		[Rule("<OptionalVarying> ::=", typeof(VaryingToken))]
		[Rule("<OptionalOutput> ::=", typeof(OutputToken))]
		[Rule("<OptionalReadonly> ::=", typeof(Identifier))]
		[Rule("<ExecuteParameterGroup> ::=", typeof(Sequence<ExecuteParameter>))]
		[Rule("<ProcedureParameterGroup> ::=", typeof(Sequence<ProcedureParameter>))]
		[Rule("<ProcedureFor> ::=", typeof(ForReplicationToken))]
		[Rule("<ProcedureOptionGroup> ::=", typeof(WithRecompileToken))]
		[Rule("<ViewOptionalAttribute> ::=", typeof(WithViewMetadataToken))]
		[Rule("<ViewOptionalCheckOption> ::=", typeof(WithCheckOptionToken))]
		[Rule("<ColumnNameGroup> ::=", typeof(Sequence<ColumnName>))]
		[Rule("<IndexOptionalUnique> ::=", typeof(UniqueToken))]
		[Rule("<IndexOptionGroup> ::=", typeof(Sequence<IndexOption>))]
		[Rule("<IndexPrimary> ::=", typeof(PrimaryToken))]
		[Rule("<OptionalNotForReplication> ::=", typeof(ForReplicationToken))]
		[Rule("<OptionalForeignRefColumn> ::=", typeof(ColumnName))]
		[Rule("<CTEGroup> ::=", typeof(Sequence<CommonTableExpression>))]
		[Rule("<IntoClause> ::=", typeof(DestinationRowset))]
		[Rule("<WhereClause> ::=", typeof(Predicate))]
		[Rule("<GroupClause> ::=", typeof(Sequence<Expression>))]
		[Rule("<HavingClause> ::=", typeof(Predicate))]
		[Rule("<OptionalOrderClause> ::=", typeof(Sequence<OrderExpression>))]
		[Rule("<OptionalAlias> ::=", typeof(AliasName))]
		[Rule("<OptionalPercent> ::=", typeof(PercentToken))]
		[Rule("<OptionalWithTies> ::=", typeof(WithTiesToken))]
		[Rule("<OptionalElementName> ::=", typeof(StringLiteral))]
		[Rule("<OptionalFromClause> ::=", typeof(FromClause))]
		public Optional(): this(null) {}

		[Rule("<FulltextColumnType> ::= TYPE_COLUMN <TypeNameQualified>", typeof(Qualified<SchemaName, TypeName>), ConstructorParameterMapping=new[] { 1 })]
		[Rule("<FulltextColumnGroup> ::= '(' <FulltextColumnList> ')'", typeof(Sequence<FulltextColumn>), ConstructorParameterMapping = new[] {1})]
		[Rule("<OptionalLanguage> ::= LANGUAGE_LCID", typeof(LanguageLcid))]
		[Rule("<OptionalDefault> ::= '=' <Literal>", typeof(Literal), ConstructorParameterMapping = new[] {1})]
		[Rule("<OptionalOpenxmlSchema> ::= <OpenxmlImplicitSchema>", typeof(OpenxmlSchema))]
		[Rule("<OptionalOpenxmlSchema> ::= <OpenxmlExplicitSchema>", typeof(OpenxmlSchema))]
		[Rule("<OptionalFunctionParameterList> ::= <FunctionParameterList>", typeof(Sequence<FunctionParameter>))]
		[Rule("<OptionalVarying> ::= VARYING", typeof(VaryingToken))]
		[Rule("<OptionalOutput> ::= OUTPUT", typeof(OutputToken))]
		[Rule("<OptionalReadonly> ::= Id", typeof(Identifier))]
		[Rule("<ExecuteParameterGroup> ::= <ExecuteParameterList>", typeof(Sequence<ExecuteParameter>))]
		[Rule("<ProcedureParameterGroup> ::= <ProcedureParameterList>", typeof(Sequence<ProcedureParameter>))]
		[Rule("<ProcedureFor> ::= FOR_REPLICATION", typeof(ForReplicationToken))]
		[Rule("<ProcedureOptionGroup> ::= WITH_RECOMPILE", typeof(WithRecompileToken))]
		[Rule("<ViewOptionalAttribute> ::= WITH_VIEW_METADATA", typeof(WithViewMetadataToken))]
		[Rule("<ViewOptionalCheckOption> ::= WITH_CHECK_OPTION", typeof(WithCheckOptionToken))]
		[Rule("<ColumnNameGroup> ::= '(' <ColumnNameList> ')'", typeof(Sequence<ColumnName>), ConstructorParameterMapping = new[] {1})]
		[Rule("<IndexOptionalUnique> ::= UNIQUE", typeof(UniqueToken))]
		[Rule("<IndexOptionGroup> ::= WITH '(' <IndexOptionList> ')'", typeof(Sequence<IndexOption>), ConstructorParameterMapping = new[] {2})]
		[Rule("<IndexPrimary> ::= PRIMARY", typeof(PrimaryToken))]
		[Rule("<OptionalNotForReplication> ::= NOT FOR_REPLICATION", typeof(ForReplicationToken), ConstructorParameterMapping = new[] {1})]
		[Rule("<OptionalForeignRefColumn> ::= '(' <ColumnName> ')'", typeof(ColumnName), ConstructorParameterMapping = new[] {1})]
		[Rule("<CTEGroup> ::= WITH <CTEList>", typeof(Sequence<CommonTableExpression>), ConstructorParameterMapping = new[] {1})]
		[Rule("<IntoClause> ::= INTO <DestinationRowset>", typeof(DestinationRowset), ConstructorParameterMapping = new[] {1})]
		[Rule("<WhereClause> ::= WHERE <Predicate>", typeof(Predicate), ConstructorParameterMapping = new[] {1})]
		[Rule("<GroupClause> ::= GROUP BY <ExpressionList>", typeof(Sequence<Expression>), ConstructorParameterMapping = new[] {2})]
		[Rule("<HavingClause> ::= HAVING <Predicate>", typeof(Predicate), ConstructorParameterMapping = new[] {1})]
		[Rule("<OptionalOrderClause> ::= <OrderClause>", typeof(Sequence<OrderExpression>))]
		[Rule("<OptionalAlias> ::= <OptionalAs> <AliasName>", typeof(AliasName), ConstructorParameterMapping = new[] {1})]
		[Rule("<OptionalPercent> ::= PERCENT", typeof(PercentToken))]
		[Rule("<OptionalWithTies> ::= WITH_TIES", typeof(WithTiesToken))]
		[Rule("<OptionalElementName> ::= '(' StringLiteral ')'", typeof(StringLiteral), ConstructorParameterMapping = new[] {1})]
		[Rule("<OptionalFromClause> ::= <FromClause>", typeof(FromClause))]
		public Optional(T value) {
			this.value = value;
		}

		public T Value {
			get {
				return value;
			}
		}
	}
}