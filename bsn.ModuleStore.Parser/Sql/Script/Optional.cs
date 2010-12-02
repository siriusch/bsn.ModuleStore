// bsn ModuleStore database versioning
// -----------------------------------
// 
// Copyright 2010 by Arsène von Wyss - avw@gmx.ch
// 
// Development has been supported by Sirius Technologies AG, Basel
// 
// Source:
// 
// https://bsn-modulestore.googlecode.com/hg/
// 
// License:
// 
// The library is distributed under the GNU Lesser General Public License:
// http://www.gnu.org/licenses/lgpl.html
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//  
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
		[Rule("<ProcedureParameterGroup> ::= ~'(' ~')'", typeof(Sequence<ProcedureParameter>))]
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
		[Rule("<OptionalContainsTop> ::=", typeof(IntegerLiteral))]
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
		[Rule("<ProcedureParameterGroup> ::= ~'(' <ProcedureParameterList> ~')'", typeof(Sequence<ProcedureParameter>))]
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
		[Rule("<OptionalContainsTop> ::= ~',' <IntegerLiteral>", typeof(IntegerLiteral))]
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
