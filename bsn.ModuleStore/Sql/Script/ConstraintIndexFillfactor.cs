using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ConstraintIndexFillfactor: ConstraintIndex {
		private readonly IntegerLiteral fillfactor;

		[Rule("<ConstraintIndex> ::= WITH_FILLFACTOR '=' <IntegerLiteral>", ConstructorParameterMapping = new[] {2})]
		public ConstraintIndexFillfactor(IntegerLiteral fillfactor) {
			this.fillfactor = fillfactor;
		}
	}
}