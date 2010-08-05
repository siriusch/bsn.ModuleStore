using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class DropTriggerStatement: SqlDropStatement {
		private readonly TriggerName triggerName;

		[Rule("<DropTriggerStatement> ::= DROP TRIGGER <TriggerName>", ConstructorParameterMapping = new[] {2})]
		public DropTriggerStatement(TriggerName triggerName) {
			if (triggerName == null) {
				throw new ArgumentNullException("triggerName");
			}
			this.triggerName = triggerName;
		}
	}
}