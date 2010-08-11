using System;
using System.Diagnostics;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SourceTableVariableRowset: SourceRowset {
		private readonly VariableName tableName;

		[Rule("<SourceRowset> ::= <VariableName> <OptionalAlias>")]
		public SourceTableVariableRowset(VariableName tableName, Optional<AliasName> aliasName): base(aliasName) {
			Debug.Assert(tableName != null);
			this.tableName = tableName;
		}

		public VariableName TableName {
			get {
				return tableName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(tableName);
			base.WriteTo(writer);
		}
	}
}