using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateTriggerStatement: CreateStatement {
		private readonly bool notForReplication;
		private readonly Statement statement;
		private readonly TableName tableName;
		private readonly TriggerName triggerName;
		private readonly List<DmlOperation> triggerOperations;
		private readonly TriggerType type;

		[Rule("<CreateTriggerStatement> ::= CREATE TRIGGER <TriggerName> ON <TableName> <TriggerType> <TriggerOperationList> <OptionalNotForReplication> AS <StatementGroup>", ConstructorParameterMapping = new[] {2, 4, 5, 6, 7, 9})]
		public CreateTriggerStatement(TriggerName triggerName, TableName tableName, TriggerTypeToken triggerType, Sequence<DmlOperationToken> triggerOperations, Optional<ForReplicationToken> notForReplication, Statement statement) {
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

		public Statement Statement {
			get {
				return statement;
			}
		}

		public TableName TableName {
			get {
				return tableName;
			}
		}

		public TriggerName TriggerName {
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
			writer.WriteScript(triggerName);
			writer.Write(" ON ");
			writer.WriteScript(tableName);
			writer.WriteValue(type, " ", " ");
			string prefix = null;
			foreach (DmlOperation operation in triggerOperations) {
				writer.WriteValue(operation, prefix, null);
				prefix = ", ";
			}
			writer.WriteNotForReplication(notForReplication, " ", null);
			writer.WriteLine(" AS");
			writer.WriteScript(statement);
		}
	}
}