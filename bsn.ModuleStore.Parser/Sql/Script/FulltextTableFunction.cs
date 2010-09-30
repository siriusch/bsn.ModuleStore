using System;
using System.Diagnostics;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class FulltextTableFunction: SqlScriptableToken {
		private readonly ReservedKeyword keyword;
		private readonly Literal language;
		private readonly Expression query;
		private readonly Qualified<SchemaName, TableName> tableName;
		private readonly IntegerLiteral top;

		protected FulltextTableFunction(ReservedKeyword keyword, Qualified<SchemaName, TableName> tableName, Expression query, Literal language, IntegerLiteral top) {
			Debug.Assert(keyword != null);
			Debug.Assert(tableName != null);
			Debug.Assert(query != null);
			this.keyword = keyword;
			this.tableName = tableName;
			this.query = query;
			this.language = language;
			this.top = top;
		}

		public ReservedKeyword Keyword {
			get {
				return keyword;
			}
		}

		public Literal Language {
			get {
				return language;
			}
		}

		public Expression Query {
			get {
				return query;
			}
		}

		public Qualified<SchemaName, TableName> TableName {
			get {
				return tableName;
			}
		}

		public IntegerLiteral Top {
			get {
				return top;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(keyword, WhitespacePadding.None);
			writer.Write('(');
			writer.WriteScript(tableName, WhitespacePadding.None);
			writer.Write(", ");
			WriteColumnInternal(writer);
			writer.Write(", ");
			writer.WriteScript(query, WhitespacePadding.None);
			writer.WriteScript(language, WhitespacePadding.None, ", LANGUAGE ", null);
			writer.WriteScript(top, WhitespacePadding.None, ", ", null);
			writer.Write(')');
		}

		protected abstract void WriteColumnInternal(SqlWriter writer);
	}
}