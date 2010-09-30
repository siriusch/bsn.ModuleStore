using System;
using System.Diagnostics;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class FulltextFunction: Predicate {
		private readonly ReservedKeyword keyword;
		private readonly Literal language;
		private readonly Expression query;

		protected FulltextFunction(ReservedKeyword keyword, Expression query, Literal language): base() {
			Debug.Assert(keyword != null);
			Debug.Assert(query != null);
			this.keyword = keyword;
			this.query = query;
			this.language = language;
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

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(keyword, WhitespacePadding.None);
			writer.Write('(');
			WriteColumnInternal(writer);
			writer.Write(", ");
			writer.WriteScript(query, WhitespacePadding.None);
			writer.WriteScript(language, WhitespacePadding.None, ", LANGUAGE ", null);
			writer.Write(')');
		}

		protected abstract void WriteColumnInternal(SqlWriter writer);
	}
}