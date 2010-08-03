using System;
using System.IO;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SqlParens<T>: SqlToken where T: SqlToken {
		private readonly Sequence<T> inner;

		public SqlParens(Sequence<T> inner) {
			this.inner = inner;
		}

		public Sequence<T> Inner {
			get {
				return inner;
			}
		}

		protected abstract string Separator {
			get;
		}

		public override void WriteTo(TextWriter writer) {
			if (inner != null) {
				writer.Write('(');
				string separator = string.Empty;
				foreach (T item in inner) {
					writer.Write(separator);
					item.WriteTo(writer);
					separator = Separator;
				}
				writer.Write(')');
			}
		}
	}
}