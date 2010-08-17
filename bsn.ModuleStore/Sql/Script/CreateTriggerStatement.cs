using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateTriggerStatement: CreateStatement {
		private readonly bool notForReplication;
		private readonly Statement statement;
		private readonly Qualified<SchemaName, TableName> tableName;
		private readonly Qualified<SchemaName, TriggerName> triggerName;
		private readonly List<DmlOperation> triggerOperations;
		private readonly TriggerType type;

		[Rule("<CreateTriggerStatement> ::= CREATE TRIGGER <TriggerNameQualified> ON <TableNameQualified> <TriggerType> <TriggerOperationList> <OptionalNotForReplication> AS <StatementGroup>", ConstructorParameterMapping = new[] {2, 4, 5, 6, 7, 9})]
		public CreateTriggerStatement(Qualified<SchemaName, TriggerName> triggerName, Qualified<SchemaName, TableName> tableName, TriggerTypeToken triggerType, Sequence<DmlOperationToken> triggerOperations, Optional<ForReplicationToken> notForReplication, Statement statement) {
			Debug.Assert(triggerName != null);
			Debug.Assert(triggerOperations != null);
			Debug.Assert(triggerType != null);
			Debug.Assert(tableName != null);
			Debug.Assert(statement != null);
			this.triggerName = triggerName;
			this.tableName = tableName;
			this.statement = statement;
			this.triggerOperations = triggerOperations.Select(token => token.Operation).ToList();
			this.notForReplication = notForReplication.HasValue();
			type = triggerType.TriggerType;
		}

		public bool NotForReplication {
			get {
				return notForReplication;
			}
		}

		public override string ObjectName {
			get {
				return triggerName.Name.Value;
			}
		}

		public Statement Statement {
			get {
				return statement;
			}
		}

		public Qualified<SchemaName, TableName> TableName {
			get {
				return tableName;
			}
		}

		public Qualified<SchemaName, TriggerName> TriggerName {
			get {
				return triggerName;
			}
		}

		public List<DmlOperation> TriggerOperations {
			get {
				return triggerOperations;
			}
		}

		public TriggerType Type {
			get {
				return type;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("CREATE TRIGGER ");
			writer.WriteScript(triggerName, WhitespacePadding.None);
			writer.Write(" ON ");
			writer.WriteScript(tableName, WhitespacePadding.SpaceAfter);
			writer.WriteEnum(type, WhitespacePadding.SpaceAfter);
			string prefix = null;
			foreach (DmlOperation operation in triggerOperations) {
				writer.Write(prefix);
				writer.WriteEnum(operation, WhitespacePadding.None);
				prefix = ", ";
			}
			writer.WriteNotForReplication(notForReplication, WhitespacePadding.SpaceBefore);
			writer.Write(" AS");
			writer.IncreaseIndent();
			writer.WriteScript(statement, WhitespacePadding.NewlineBefore);
			writer.DecreaseIndent();
		}
	}
}