using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateTriggerStatement: SqlCreateStatement {
		private readonly bool notForReplication;
		private readonly SqlStatement statement;
		private readonly TableName tableName;
		private readonly TriggerName triggerName;
		private readonly List<DmlOperation> triggerOperations;
		private readonly TriggerType type;

		[Rule("<CreateTriggerStatement> ::= CREATE TRIGGER <TriggerName> ON <TableName> <TriggerType> <TriggerOperationList> <OptionalNotForReplication> AS <StatementGroup>", ConstructorParameterMapping = new[] {2, 4, 5, 6, 7, 9})]
		public CreateTriggerStatement(TriggerName triggerName, TableName tableName, TriggerTypeToken triggerType, Sequence<DmlOperationToken> triggerOperations, Optional<ForReplicationToken> notForReplication, SqlStatement statement) {
			if (triggerName == null) {
				throw new ArgumentNullException("triggerName");
			}
			if (tableName == null) {
				throw new ArgumentNullException("tableName");
			}
			if (triggerType == null) {
				throw new ArgumentNullException("triggerType");
			}
			if (triggerOperations == null) {
				throw new ArgumentNullException("triggerOperations");
			}
			if (statement == null) {
				throw new ArgumentNullException("statement");
			}
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

		public SqlStatement Statement {
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

		public override void WriteTo(TextWriter writer) {
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