using System;

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

		public override void WriteTo(System.IO.TextWriter writer) {
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

		protected abstract string Separator {
			get;
		}
	}
}