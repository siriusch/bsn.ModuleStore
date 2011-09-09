#if DEBUG

using System;
using System.Collections.Generic;

namespace bsn.ModuleStore.Mapper.InterfaceMetadata {
	internal class TestParentWithChildren: IEquatable<TestParentWithChildren>, IIdentifiable<Guid> {
		[SqlDeserialize]
		private List<TestChildWithoutParent> children;

		[SqlColumn("uidParent", Identity = true)]
		private Guid id;

		[SqlColumn("sKeyParent")]
		private string key;

		public List<TestChildWithoutParent> Children {
			get {
				return children;
			}
			set {
				children = value;
			}
		}

		public string Key {
			get {
				return key;
			}
			set {
				key = value;
			}
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			if (obj.GetType() != typeof(TestParent)) {
				return false;
			}
			return Equals((TestParentWithChildren)obj);
		}

		public override int GetHashCode() {
			return id.GetHashCode();
		}

		public bool Equals(TestParentWithChildren other) {
			if (ReferenceEquals(null, other)) {
				return false;
			}
			if (ReferenceEquals(this, other)) {
				return true;
			}
			return other.id.Equals(id);
		}

		public Guid Id {
			get {
				return id;
			}
			set {
				id = value;
			}
		}
	}
}

#endif
