using System;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class NumericLiteral<T>: Literal<T> where T: IConvertible {
		protected NumericLiteral(T value): base(value) {}

		public abstract double AsDouble {
			get;
		}
	}
}