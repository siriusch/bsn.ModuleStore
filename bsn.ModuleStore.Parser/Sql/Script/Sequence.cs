using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class Sequence<T>: SqlToken, IEnumerable<T> where T: SqlToken {
		private readonly T item;
		private readonly Sequence<T> next;

		[Rule("<CursorOptionList> ::=", typeof(Identifier))]
		[Rule("<ForeignKeyActionList> ::=", typeof(ForeignKeyAction))]
		[Rule("<ColumnConstraintList> ::=", typeof(ColumnConstraint))]
		[Rule("<JoinChain> ::=", typeof(Join))]
		public Sequence(): this(null, null) {}

		[Rule("<SetValueList> ::= <SetValue>", typeof(SqlScriptableToken))]
		[Rule("<ColumnNameList> ::= <ColumnName>", typeof(ColumnName))]
		[Rule("<StatementList> ::= <StatementGroup>", typeof(Statement))]
		[Rule("<StatementList> ::= <StatementGroup> ~<Terminator>", typeof(Statement))]
		[Rule("<OpenxmlColumnList> ::= <OpenxmlColumn>", typeof(OpenxmlColumn))]
		[Rule("<DeclareItemList> ::= <DeclareItem>", typeof(DeclareItem))]
		[Rule("<FulltextColumnList> ::= <FulltextColumn>", typeof(FulltextColumn))]
		[Rule("<FunctionParameterList> ::= <FunctionParameter>", typeof(FunctionParameter))]
		[Rule("<ProcedureParameterList> ::= <ProcedureParameter>", typeof(ProcedureParameter))]
		[Rule("<ExecuteParameterList> ::= <ExecuteParameter>", typeof(ExecuteParameter))]
		[Rule("<TableDefinitionList> ::= <TableDefinition>", typeof(TableDefinition))]
		[Rule("<IndexColumnList> ::= <IndexColumn>", typeof(IndexColumn))]
		[Rule("<IndexOptionList> ::= <IndexOption>", typeof(IndexOption))]
		[Rule("<TriggerOperationList> ::= <TriggerOperation>", typeof(DmlOperationToken))]
		[Rule("<TriggerNameQualifiedList> ::= <TriggerNameQualified>", typeof(Qualified<SchemaName, TriggerName>))]
		[Rule("<CTEList> ::= <CTE>", typeof(CommonTableExpression))]
		[Rule("<ColumnItemList> ::= <ColumnItem>", typeof(ColumnItem))]
		[Rule("<OrderList> ::= <Order>", typeof(OrderExpression))]
		[Rule("<ExpressionList> ::= <Expression>", typeof(Expression))]
		[Rule("<XmlDirectiveList> ::= <XmlDirective>", typeof(XmlDirective))]
		[Rule("<UpdateItemList> ::= <UpdateItem>", typeof(UpdateItem))]
		[Rule("<CaseWhenExpressionList> ::= <CaseWhenExpression>", typeof(CaseWhen<Expression>))]
		[Rule("<CaseWhenPredicateList> ::= <CaseWhenPredicate>", typeof(CaseWhen<Predicate>))]
		public Sequence(T item): this(item, null) {}

		[Rule("<CursorOptionList> ::= Id <CursorOptionList>", typeof(Identifier))]
		[Rule("<ForeignKeyActionList> ::= <ForeignKeyAction> <ForeignKeyActionList>", typeof(ForeignKeyAction))]
		[Rule("<ColumnConstraintList> ::= <ColumnConstraint> <ColumnConstraintList>", typeof(ColumnConstraint))]
		[Rule("<JoinChain> ::= <Join> <JoinChain>", typeof(Join))]
		[Rule("<SetValueList> ::= <SetValue> <SetValueList>", typeof(SqlScriptableToken))]
		[Rule("<ColumnNameList> ::= <ColumnName> ~',' <ColumnNameList>", typeof(ColumnName))]
		[Rule("<StatementList> ::= <StatementGroup> ~<Terminator> <StatementList>", typeof(Statement))]
		[Rule("<OpenxmlColumnList> ::= <OpenxmlColumn> ~',' <OpenxmlColumnList>", typeof(OpenxmlColumn))]
		[Rule("<DeclareItemList> ::= <DeclareItem> ~',' <DeclareItemList>", typeof(DeclareItem))]
		[Rule("<FulltextColumnList> ::= <FulltextColumn> ~',' <FulltextColumnList>", typeof(FulltextColumn))]
		[Rule("<FunctionParameterList> ::= <FunctionParameter> ~',' <FunctionParameterList>", typeof(FunctionParameter))]
		[Rule("<ProcedureParameterList> ::= <ProcedureParameter> ~',' <ProcedureParameterList>", typeof(ProcedureParameter))]
		[Rule("<ExecuteParameterList> ::= <ExecuteParameter> ~',' <ExecuteParameterList>", typeof(ExecuteParameter))]
		[Rule("<TableDefinitionList> ::= <TableDefinition> ~',' <TableDefinitionList>", typeof(TableDefinition))]
		[Rule("<IndexColumnList> ::= <IndexColumn> ~',' <IndexColumnList>", typeof(IndexColumn))]
		[Rule("<IndexOptionList> ::= <IndexOption> ~',' <IndexOptionList>", typeof(IndexOption))]
		[Rule("<TriggerOperationList> ::= <TriggerOperation> ~',' <TriggerOperationList>", typeof(DmlOperationToken))]
		[Rule("<TriggerNameQualifiedList> ::= <TriggerNameQualified> ~',' <TriggerNameQualifiedList>", typeof(Qualified<SchemaName, TriggerName>))]
		[Rule("<CTEList> ::= <CTE> ~',' <CTEList>", typeof(CommonTableExpression))]
		[Rule("<ColumnItemList> ::= <ColumnItem> ~',' <ColumnItemList>", typeof(ColumnItem))]
		[Rule("<OrderList> ::= <Order> ~',' <OrderList>", typeof(OrderExpression))]
		[Rule("<ExpressionList> ::= <Expression> ~',' <ExpressionList>", typeof(Expression))]
		[Rule("<XmlDirectiveList> ::= <XmlDirective> ~',' <XmlDirectiveList>", typeof(XmlDirective))]
		[Rule("<UpdateItemList> ::= <UpdateItem> ~',' <UpdateItemList>", typeof(UpdateItem))]
		[Rule("<CaseWhenExpressionList> ::= <CaseWhenExpression> <CaseWhenExpressionList>", typeof(CaseWhen<Expression>))]
		[Rule("<CaseWhenPredicateList> ::= <CaseWhenPredicate> <CaseWhenPredicateList>", typeof(CaseWhen<Predicate>))]
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