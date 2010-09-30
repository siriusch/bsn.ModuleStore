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
		[Rule("<OptionalLanguage> ::=", typeof(Literal))]
		[Rule("<OptionalDefault> ::=", typeof(Literal))]
		[Rule("<OptionalOpenxmlSchema> ::=", typeof(OpenxmlSchema))]
		[Rule("<OptionalFunctionParameterList> ::=", typeof(Sequence<FunctionParameter>))]
		[Rule("<OptionalVarying> ::=", typeof(VaryingToken))]
		[Rule("<OptionalOutput> ::=", typeof(UnreservedKeyword))]
		[Rule("<OptionalReadonly> ::=", typeof(Identifier))]
		[Rule("<ExecuteParameterGroup> ::=", typeof(Sequence<ExecuteParameter>))]
		[Rule("<ProcedureParameterGroup> ::=", typeof(Sequence<ProcedureParameter>))]
		[Rule("<ViewOptionalCheckOption> ::=", typeof(WithCheckOptionToken))]
		[Rule("<ColumnNameGroup> ::=", typeof(Sequence<ColumnName>))]
		[Rule("<IndexOptionalUnique> ::=", typeof(UniqueToken))]
		[Rule("<IndexOptionGroup> ::=", typeof(Sequence<IndexOption>))]
		[Rule("<OptionalForeignRefColumn> ::=", typeof(ColumnName))]
		[Rule("<IntoClause> ::=", typeof(DestinationRowset))]
		[Rule("<WhereClause> ::=", typeof(Predicate))]
		[Rule("<GroupClause> ::=", typeof(Sequence<Expression>))]
		[Rule("<HavingClause> ::=", typeof(Predicate))]
		[Rule("<OptionalOrderClause> ::=", typeof(Sequence<OrderExpression>))]
		[Rule("<OptionalPercent> ::=", typeof(PercentToken))]
		[Rule("<OptionalElementName> ::=", typeof(StringLiteral))]
		[Rule("<OptionalFromClause> ::=", typeof(FromClause))]
		public Optional(): this(null) {}

		[Rule("<FulltextColumnType> ::= ~TYPE ~COLUMN <TypeNameQualified>", typeof(Qualified<SchemaName, TypeName>))]
		[Rule("<FulltextColumnGroup> ::= ~'(' <FulltextColumnList> ~')'", typeof(Sequence<FulltextColumn>))]
		[Rule("<OptionalLanguage> ::= ~LANGUAGE <IntegerLiteral>", typeof(Literal))]
		[Rule("<OptionalLanguage> ::= ~LANGUAGE <StringLiteral>", typeof(Literal))]
		[Rule("<OptionalDefault> ::= ~'=' <Literal>", typeof(Literal))]
		[Rule("<OptionalOpenxmlSchema> ::= <OpenxmlImplicitSchema>", typeof(OpenxmlSchema))]
		[Rule("<OptionalOpenxmlSchema> ::= <OpenxmlExplicitSchema>", typeof(OpenxmlSchema))]
		[Rule("<OptionalFunctionParameterList> ::= <FunctionParameterList>", typeof(Sequence<FunctionParameter>))]
		[Rule("<OptionalVarying> ::= VARYING", typeof(VaryingToken))]
		[Rule("<OptionalOutput> ::= OUTPUT", typeof(UnreservedKeyword))]
		[Rule("<OptionalReadonly> ::= Id", typeof(Identifier))]
		[Rule("<ExecuteParameterGroup> ::= <ExecuteParameterList>", typeof(Sequence<ExecuteParameter>))]
		[Rule("<ProcedureParameterGroup> ::= <ProcedureParameterList>", typeof(Sequence<ProcedureParameter>))]
		[Rule("<ViewOptionalCheckOption> ::= WITH_CHECK_OPTION", typeof(WithCheckOptionToken))]
		[Rule("<ColumnNameGroup> ::= ~'(' <ColumnNameList> ~')'", typeof(Sequence<ColumnName>))]
		[Rule("<IndexOptionalUnique> ::= UNIQUE", typeof(UniqueToken))]
		[Rule("<IndexOptionGroup> ::= ~WITH ~'(' <IndexOptionList> ~')'", typeof(Sequence<IndexOption>))]
		[Rule("<OptionalForeignRefColumn> ::= ~'(' <ColumnName> ~')'", typeof(ColumnName))]
		[Rule("<IntoClause> ::= ~INTO <DestinationRowset>", typeof(DestinationRowset))]
		[Rule("<WhereClause> ::= ~WHERE <Predicate>", typeof(Predicate))]
		[Rule("<GroupClause> ::= ~GROUP ~BY <ExpressionList>", typeof(Sequence<Expression>))]
		[Rule("<HavingClause> ::= ~HAVING <Predicate>", typeof(Predicate))]
		[Rule("<OptionalOrderClause> ::= <OrderClause>", typeof(Sequence<OrderExpression>))]
		[Rule("<OptionalPercent> ::= PERCENT", typeof(PercentToken))]
		[Rule("<OptionalElementName> ::= ~'(' StringLiteral ~')'", typeof(StringLiteral))]
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