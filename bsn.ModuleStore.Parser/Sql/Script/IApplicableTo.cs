using System;
using System.Linq;

namespace bsn.ModuleStore.Sql.Script {
	public interface IApplicableTo<T> {
		IQualifiedName<SchemaName> QualifiedName {
			get;
		}

		void ApplyTo(T instance);
	}
}