using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ColumnName: SqlQuotedName {
		[Rule("<ColumnWild> ::= ~'*'")]
		public ColumnName(): this("*") {}

		[Rule("<ColumnName> ::= Id")]
		public ColumnName(Identifier identifier): this(identifier.Value) {}

		internal ColumnName(string name): base(name) {}

		public bool IsWildcard {
			get {
				return StringComparer.Ordinal.Equals("*", Value);
			}
		}

		protected internal override void WriteToInternal(SqlWriter writer, bool isPartOfQualifiedName) {
			if (IsWildcard) {
				writer.Write(Value);
			} else {
				base.WriteToInternal(writer, isPartOfQualifiedName);
			}
		}
	}
}