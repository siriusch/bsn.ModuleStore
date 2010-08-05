using System;

using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ForeignKeyAction: SqlToken {
		private readonly IToken action;
		private readonly IToken operation;

		[Rule("<ForeignKeyAction> ::= ON DELETE NO_ACTION", ConstructorParameterMapping = new[] {1})]
		[Rule("<ForeignKeyAction> ::= ON UPDATE NO_ACTION", ConstructorParameterMapping = new[] {1})]
		public ForeignKeyAction(IToken operation): this(operation, null) {}

		[Rule("<ForeignKeyAction> ::= ON DELETE CASCADE", ConstructorParameterMapping = new[] {1, 2})]
		[Rule("<ForeignKeyAction> ::= ON UPDATE CASCADE", ConstructorParameterMapping = new[] {1, 2})]
		[Rule("<ForeignKeyAction> ::= ON DELETE SET NULL", ConstructorParameterMapping = new[] {1, 3})]
		[Rule("<ForeignKeyAction> ::= ON UPDATE SET NULL", ConstructorParameterMapping = new[] {1, 3})]
		[Rule("<ForeignKeyAction> ::= ON DELETE SET DEFAULT", ConstructorParameterMapping = new[] {1, 3})]
		[Rule("<ForeignKeyAction> ::= ON UPDATE SET DEFAULT", ConstructorParameterMapping = new[] {1, 3})]
		public ForeignKeyAction(IToken operation, IToken action) {
			if (operation == null) {
				throw new ArgumentNullException("operation");
			}
			this.operation = operation;
			this.action = action;
		}
	}
}