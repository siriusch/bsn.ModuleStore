using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class CrossJoin: Join {
		[Rule("<Join> ::= CROSS JOIN <SourceRowset>", ConstructorParameterMapping = new[] {2})]
		public CrossJoin(SourceRowset joinRowset): base(joinRowset) {}

		public override JoinKind Kind {
			get {
				return JoinKind.Cross;
			}
		}
	}
}