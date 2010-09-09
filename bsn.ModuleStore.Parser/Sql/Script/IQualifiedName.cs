using System;

namespace bsn.ModuleStore.Sql.Script {
	public interface IQualifiedName<TQ>: IQualified<TQ>, IEquatable<IQualifiedName<TQ>>, IComparable<IQualifiedName<TQ>> where TQ: SqlName {
		SqlName Name {
			get;
		}

		bool IsOverridden {
			get;
		}

		void SetOverride(IQualified<TQ> qualificationProvider);
	}
}
