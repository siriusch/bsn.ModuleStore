using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateTriggerStatement: CreateStatement, ICreateOrAlterStatement {
		private readonly ReplicationToken replication;
		private readonly Statement statement;
		private readonly Qualified<SchemaName, TableName> tableName;
		private readonly Qualified<SchemaName, TriggerName> triggerName;
		private readonly List<DmlOperation> triggerOperations;
		private readonly TriggerTypeToken type;

		[Rule("<CreateTriggerStatement> ::= ~CREATE ~TRIGGER <TriggerNameQualified> ~ON <TableNameQualified> <TriggerType> <TriggerOperationList> <OptionalNotForReplication> ~AS <StatementGroup>")]
		public CreateTriggerStatement(Qualified<SchemaName, TriggerName> triggerName, Qualified<SchemaName, TableName> tableName, TriggerTypeToken triggerType, Sequence<DmlOperationToken> triggerOperations, ReplicationToken replication, Statement statement) {
			Debug.Assert(triggerName != null);
			Debug.Assert(triggerOperations != null);
			Debug.Assert(triggerType != null);
			Debug.Assert(tableName != null);
			Debug.Assert(statement != null);
			this.triggerName = triggerName;
			this.tableName = tableName;
			this.statement = statement;
			this.triggerOperations = triggerOperations.Select(token => token.Operation).ToList();
			this.replication = replication;
			type = triggerType;
		}

		public override ObjectCategory ObjectCategory {
			get {
				return ObjectCategory.Trigger;
			}
		}

		public override string ObjectName {
			get {
				return triggerName.Name.Value;
			}
		}

		public ReplicationToken Replication {
			get {
				return replication;
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

		public IEnumerable<DmlOperation> TriggerOperations {
			get {
				return triggerOperations;
			}
		}

		public TriggerTypeToken Type {
			get {
				return type;
			}
		}

		public override DropStatement CreateDropStatement() {
			return new DropTriggerStatement(triggerName);
		}

		public override void WriteTo(SqlWriter writer) {
			WriteToInternal(writer, "CREATE");
		}

		protected override string GetObjectSchema() {
			return triggerName.IsQualified ? triggerName.Qualification.Value : string.Empty;
		}

		protected override void Initialize(Symbol symbol, LineInfo position) {
			base.Initialize(symbol, position);
			LockInnerUnqualifiedTableNames(IsInsertedOrDeletedTableName);
		}

		private void WriteToInternal(SqlWriter writer, string command) {
			WriteCommentsTo(writer);
			writer.Write(command);
			writer.Write(" TRIGGER ");
			writer.WriteScript(triggerName, WhitespacePadding.None);
			writer.Write(" ON ");
			writer.WriteScript(tableName, WhitespacePadding.SpaceAfter);
			writer.WriteScript(type, WhitespacePadding.SpaceAfter);
			string prefix = null;
			foreach (DmlOperation operation in triggerOperations) {
				writer.Write(prefix);
				writer.WriteEnum(operation, WhitespacePadding.None);
				prefix = ", ";
			}
			writer.WriteScript(replication, WhitespacePadding.SpaceBefore);
			writer.Write(" AS");
			writer.IncreaseIndent();
			writer.WriteScript(statement, WhitespacePadding.NewlineBefore);
			writer.DecreaseIndent();
		}

		void ICreateOrAlterStatement.WriteToInternal(SqlWriter writer, string command) {
			if (string.IsNullOrEmpty(command)) {
				throw new ArgumentNullException("command");
			}
			WriteToInternal(writer, command);
		}
	}
}