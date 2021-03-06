﻿// bsn ModuleStore database versioning
// -----------------------------------
// 
// Copyright 2010 by Arsène von Wyss - avw@gmx.ch
// 
// Development has been supported by Sirius Technologies AG, Basel
// 
// Source:
// 
// https://bsn-modulestore.googlecode.com/hg/
// 
// License:
// 
// The library is distributed under the GNU Lesser General Public License:
// http://www.gnu.org/licenses/lgpl.html
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateTriggerStatement: AlterableCreateStatement, ICreateOrAlterStatement, ITableBound {
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

		public override ObjectCategory ObjectCategory => ObjectCategory.Trigger;

		public override string ObjectName {
			get => triggerName.Name.Value;
			set => triggerName.Name = new TriggerName(value);
		}

		public ReplicationToken Replication => replication;

		public Statement Statement => statement;

		public Qualified<SchemaName, TriggerName> TriggerName => triggerName;

		public IEnumerable<DmlOperation> TriggerOperations => triggerOperations;

		public TriggerTypeToken Type => type;

		protected override SchemaName SchemaName => triggerName.Qualification;

		public override void WriteTo(SqlWriter writer) {
			WriteToInternal(writer, "CREATE");
		}

		protected override IInstallStatement CreateDropStatement() {
			return new DropTriggerStatement(triggerName, tableName);
		}

		protected override void Initialize(Symbol symbol, LineInfo position) {
			base.Initialize(symbol, position);
			LockInnerUnqualifiedTableNames(IsInsertedOrDeletedTableName);
		}

		private void WriteToInternal(SqlWriter writer, string command) {
			WriteCommentsTo(writer);
			writer.WriteKeyword(command);
			writer.WriteKeyword(" TRIGGER ");
			writer.WriteScript(triggerName, WhitespacePadding.None);
			writer.WriteKeyword(" ON ");
			writer.WriteScript(tableName, WhitespacePadding.SpaceAfter);
			writer.WriteScript(type, WhitespacePadding.SpaceAfter);
			string prefix = null;
			foreach (var operation in triggerOperations) {
				writer.Write(prefix);
				writer.WriteEnum(operation, WhitespacePadding.None);
				prefix = ", ";
			}
			writer.WriteScript(replication, WhitespacePadding.SpaceBefore);
			writer.WriteKeyword(" AS");
			using (writer.Indent()) {
				writer.WriteScript(statement, WhitespacePadding.NewlineBefore);
			}
		}

		void ICreateOrAlterStatement.WriteToInternal(SqlWriter writer, string command) {
			if (string.IsNullOrEmpty(command)) {
				throw new ArgumentNullException(nameof(command));
			}
			WriteToInternal(writer, command);
		}

		public Qualified<SchemaName, TableName> TableName => tableName;
	}
}
