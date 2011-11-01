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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class Sequence<T>: SqlToken, IEnumerable<T> where T: SqlToken {
		private readonly T item;
		private readonly Sequence<T> next;

		[Rule("<CursorOptionList> ::=", typeof(Identifier))]
		[Rule("<ForeignKeyActionList> ::=", typeof(ForeignKeyAction))]
		[Rule("<ColumnConstraintList> ::=", typeof(ColumnConstraint))]
		[Rule("<ComputedColumnConstraintList> ::=", typeof(ColumnConstraint))]
		[Rule("<JoinChain> ::=", typeof(Join))]
		[Rule("<XmlDirectiveList> ::=", typeof(XmlDirective))]
		public Sequence(): this(null, null) {}

		[Rule("<SetValueList> ::= <SetValue>", typeof(SqlScriptableToken))]
		[Rule("<ColumnNameList> ::= <ColumnName>", typeof(ColumnName))]
		[Rule("<StatementList> ::= <StatementGroup>", typeof(Statement))]
		[Rule("<StatementList> ::= <StatementGroup> ~<Terminator>", typeof(Statement))]
		[Rule("<StatementList> ::= <Label>", typeof(Statement))]
		[Rule("<StatementList> ::= <Label> ~<Terminator>", typeof(Statement))]
		[Rule("<OpenxmlColumnList> ::= <OpenxmlColumn>", typeof(OpenxmlColumn))]
		[Rule("<DeclareItemList> ::= <DeclareItem>", typeof(DeclareItem))]
		[Rule("<FulltextColumnList> ::= <FulltextColumn>", typeof(FulltextColumn))]
		[Rule("<FunctionParameterList> ::= <FunctionParameter>", typeof(Parameter))]
		[Rule("<NamedFunctionList> ::= <NamedFunction>", typeof(NamedFunction))]
		[Rule("<ProcedureParameterList> ::= <ProcedureParameter>", typeof(ProcedureParameter))]
		[Rule("<ExecuteParameterList> ::= <ExecuteParameter>", typeof(ExecuteParameter))]
		[Rule("<TableDefinitionList> ::= <TableDefinition>", typeof(TableDefinition))]
		[Rule("<IndexColumnList> ::= <IndexColumn>", typeof(IndexColumn))]
		[Rule("<IndexOptionList> ::= <IndexOption>", typeof(IndexOption))]
		[Rule("<TriggerOperationList> ::= <TriggerOperation>", typeof(DmlOperationToken))]
		[Rule("<TriggerNameQualifiedList> ::= <TriggerNameQualified>", typeof(Qualified<SchemaName, TriggerName>))]
		[Rule("<ColumnNameQualifiedList> ::= <ColumnNameQualified>", typeof(Qualified<SqlName, ColumnName>))]
		[Rule("<CTEList> ::= <CTE>", typeof(CommonTableExpression))]
		[Rule("<ColumnItemList> ::= <ColumnItem>", typeof(ColumnItem))]
		[Rule("<OrderList> ::= <Order>", typeof(OrderExpression))]
		[Rule("<ExpressionList> ::= <Expression>", typeof(Expression))]
		[Rule("<UpdateItemList> ::= <UpdateItem>", typeof(UpdateItem))]
		[Rule("<CaseWhenExpressionList> ::= <CaseWhenExpression>", typeof(CaseWhen<Expression>))]
		[Rule("<CaseWhenPredicateList> ::= <CaseWhenPredicate>", typeof(CaseWhen<Predicate>))]
		[Rule("<XmlNamespaceList> ::= <XmlNamespace>", typeof(XmlNamespace))]
		[Rule("<TableHintList> ::= <TableHint>", typeof(TableHint))]
		[Rule("<IndexValueList> ::= IntegerLiteral", typeof(IntegerLiteral))]
		[Rule("<MergeWhenMatchedList> ::= <MergeWhenMatched>", typeof(MergeWhenMatched))]
		[Rule("<ValuesList> ::= ~'(' <ExpressionList> ~')'", typeof(Sequence<Expression>))]
		[Rule("<QueryHintOptionList> ::= <QueryHintOption>", typeof(QueryHintOption))]
		[Rule("<VariableNameList> ::= <VariableName>", typeof(VariableName))]
		[Rule("<RaiserrorOptionList> ::= <RaiserrorOption>", typeof(UnreservedKeyword))]
		public Sequence(T item): this(item, null) {}

		[Rule("<CursorOptionList> ::= Id <CursorOptionList>", typeof(Identifier))]
		[Rule("<ForeignKeyActionList> ::= <ForeignKeyAction> <ForeignKeyActionList>", typeof(ForeignKeyAction))]
		[Rule("<ColumnConstraintList> ::= <ColumnConstraint> <ColumnConstraintList>", typeof(ColumnConstraint))]
		[Rule("<ComputedColumnConstraintList> ::= <ComputedColumnConstraint> <ComputedColumnConstraintList>", typeof(ColumnConstraint))]
		[Rule("<JoinChain> ::= <Join> <JoinChain>", typeof(Join))]
		[Rule("<SetValueList> ::= <SetValue> <SetValueList>", typeof(SqlScriptableToken))]
		[Rule("<ColumnNameList> ::= <ColumnName> ~',' <ColumnNameList>", typeof(ColumnName))]
		[Rule("<StatementList> ::= <StatementGroup> ~<Terminator> <StatementList>", typeof(Statement))]
		[Rule("<StatementList> ::= <Label> <StatementList>", typeof(Statement))]
		[Rule("<StatementList> ::= <Label> ~<Terminator> <StatementList>", typeof(Statement))]
		[Rule("<OpenxmlColumnList> ::= <OpenxmlColumn> ~',' <OpenxmlColumnList>", typeof(OpenxmlColumn))]
		[Rule("<DeclareItemList> ::= <DeclareItem> ~',' <DeclareItemList>", typeof(DeclareItem))]
		[Rule("<FulltextColumnList> ::= <FulltextColumn> ~',' <FulltextColumnList>", typeof(FulltextColumn))]
		[Rule("<FunctionParameterList> ::= <FunctionParameter> ~',' <FunctionParameterList>", typeof(Parameter))]
		[Rule("<NamedFunctionList> ::= <NamedFunction> ~'.' <NamedFunctionList>", typeof(NamedFunction))]
		[Rule("<ProcedureParameterList> ::= <ProcedureParameter> ~',' <ProcedureParameterList>", typeof(ProcedureParameter))]
		[Rule("<ExecuteParameterList> ::= <ExecuteParameter> ~',' <ExecuteParameterList>", typeof(ExecuteParameter))]
		[Rule("<TableDefinitionList> ::= <TableDefinition> ~',' <TableDefinitionList>", typeof(TableDefinition))]
		[Rule("<IndexColumnList> ::= <IndexColumn> ~',' <IndexColumnList>", typeof(IndexColumn))]
		[Rule("<IndexOptionList> ::= <IndexOption> ~',' <IndexOptionList>", typeof(IndexOption))]
		[Rule("<TriggerOperationList> ::= <TriggerOperation> ~',' <TriggerOperationList>", typeof(DmlOperationToken))]
		[Rule("<TriggerNameQualifiedList> ::= <TriggerNameQualified> ~',' <TriggerNameQualifiedList>", typeof(Qualified<SchemaName, TriggerName>))]
		[Rule("<ColumnNameQualifiedList> ::= <ColumnNameQualified> ~',' <ColumnNameQualifiedList>", typeof(Qualified<SqlName, ColumnName>))]
		[Rule("<CTEList> ::= <CTE> ~',' <CTEList>", typeof(CommonTableExpression))]
		[Rule("<ColumnItemList> ::= <ColumnItem> ~',' <ColumnItemList>", typeof(ColumnItem))]
		[Rule("<OrderList> ::= <Order> ~',' <OrderList>", typeof(OrderExpression))]
		[Rule("<ExpressionList> ::= <Expression> ~',' <ExpressionList>", typeof(Expression))]
		[Rule("<XmlDirectiveList> ::= ~',' <XmlDirective> <XmlDirectiveList>", typeof(XmlDirective))]
		[Rule("<UpdateItemList> ::= <UpdateItem> ~',' <UpdateItemList>", typeof(UpdateItem))]
		[Rule("<CaseWhenExpressionList> ::= <CaseWhenExpression> <CaseWhenExpressionList>", typeof(CaseWhen<Expression>))]
		[Rule("<CaseWhenPredicateList> ::= <CaseWhenPredicate> <CaseWhenPredicateList>", typeof(CaseWhen<Predicate>))]
		[Rule("<XmlNamespaceList> ::= <XmlNamespace> ~',' <XmlNamespaceList>", typeof(XmlNamespace))]
		[Rule("<TableHintList> ::= <TableHint> ~',' <TableHintList>", typeof(TableHint))]
		[Rule("<IndexValueList> ::= IntegerLiteral ~',' <IndexValueList>", typeof(IntegerLiteral))]
		[Rule("<MergeWhenMatchedList> ::= <MergeWhenMatched> <MergeWhenMatchedList>", typeof(MergeWhenMatched))]
		[Rule("<ValuesList> ::= ~'(' <ExpressionList> ~')' ~',' <ValuesList>", typeof(Sequence<Expression>))]
		[Rule("<QueryHintOptionList> ::= <QueryHintOption> ~',' <QueryHintOptionList>", typeof(QueryHintOption))]
		[Rule("<VariableNameList> ::= <VariableName> ~',' <VariableNameList>", typeof(VariableName))]
		[Rule("<RaiserrorOptionList> ::= <RaiserrorOption> ~',' <RaiserrorOptionList>", typeof(UnreservedKeyword))]
		public Sequence(T item, Sequence<T> next) {
			if (next != null) {
				if (next.Item != null) {
					this.next = next;
				} else {
					Debug.Assert(next.Next == null);
				}
			}
			this.item = item;
		}

		public T Item {
			get {
				return item;
			}
		}

		public Sequence<T> Next {
			get {
				return next;
			}
		}

		public IEnumerator<T> GetEnumerator() {
			for (Sequence<T> sequence = this; sequence != null; sequence = sequence.Next) {
				if (sequence.Item != null) {
					yield return sequence.Item;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
