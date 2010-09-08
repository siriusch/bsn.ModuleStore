using System;

namespace bsn.ModuleStore.Sql.Script {
	public enum FunctionOption {
		None,
		CalledOnNullInput,
		ReturnsNullOnNullInput
	}
}