using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class Literal: SqlToken {}

	public abstract class Literal<T>: Literal where T: IConvertible {
		private readonly T value;

		protected Literal(T value) {
			this.value = value;
		}

		public T Value {
			get {
				return value;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write(value.ToString(NumberFormatInfo.InvariantInfo));
		}
	}
}