using System;
using System.IO;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class EnableTriggerStatement: EnableDisableTriggerStatement {
		[Rule("<EnableTriggerStatement> ::= ENABLE_TRIGGER ALL ON <TableName>", ConstructorParameterMapping = new[] {3})]
		public EnableTriggerStatement(TableName tableName): this(null, tableName) {}

		[Rule("<EnableTriggerStatement> ::= ENABLE_TRIGGER <TriggerNameList> ON <TableName>", ConstructorParameterMapping = new[] {1, 3})]
		public EnableTriggerStatement(Sequence<TriggerName> triggerNames, TableName tableName): base(triggerNames, tableName) {}

		public override void WriteTo(TextWriter writer) {
			writer.Write("ENABLE");
			base.WriteTo(writer);
		}
	}
}