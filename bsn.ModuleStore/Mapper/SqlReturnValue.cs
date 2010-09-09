using System;

namespace bsn.ModuleStore.Mapper {
	/// <summary>
	/// Controls the handling of the return value.
	/// </summary>
	public enum SqlReturnValue {
		/// <summary>
		/// Uses the return value for <see cref="int"/> and a <see cref="Scalar"/> execution otherwise.
		/// </summary>
		Auto,
		/// <summary>
		/// Always use a scalar execution (value of first column of first row of first rowset).
		/// </summary>
		Scalar,
		/// <summary>
		/// Always use the return parameter.
		/// </summary>
		ReturnValue
	}
}