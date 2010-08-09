using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class SourceTableVariableRowset: SourceRowset {
		private readonly VariableName tableName;

		[Rule("<SourceRowset> ::= <VariableName> <OptionalAlias>")]
		public SourceTableVariableRowset(VariableName tableName, Optional<AliasName> aliasName): base(aliasName) {
			if (tableName == null) {
				throw new ArgumentNullException("tableName");
			}
			this.tableName = tableName;
		}

		public VariableName TableName {
			get {
				return tableName;
			}
		}

		public override void WriteTo(System.IO.TextWriter writer) {
			writer.WriteScript(tableName);
			base.WriteTo(writer);
		}
	}
}