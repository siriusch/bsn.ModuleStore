using System;
using System.Linq;

namespace bsn.ModuleStore.Mapper {
	public interface IIdentifiable<T> where T: struct, IEquatable<T>, IConvertible {
		T Id {
			get;
		}
	}
}