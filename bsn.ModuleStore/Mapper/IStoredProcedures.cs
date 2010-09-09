using System;
using System.Linq;
using System.Reflection;

namespace bsn.ModuleStore.Mapper {
	public interface IStoredProcedures {
		Assembly Assembly {
			get;
		}

		string InstanceName {
			get;
		}
	}
}