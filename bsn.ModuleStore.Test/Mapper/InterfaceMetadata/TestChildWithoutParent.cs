#if DEBUG

using System;

namespace bsn.ModuleStore.Mapper.InterfaceMetadata {
	internal class TestChildWithoutParent: IEquatable<TestChildWithoutParent>, IIdentifiable<Guid> {
		[SqlColumn("uidChild", Identity = true)]
		private Guid id;

		[SqlColumn("sKeyChild")]
		private string key;

		[SqlColumn("uidParent", Identity = true)]
		private Guid parentId;

		public string Key {
			get {
				return key;
			}
			set {
				key = value;
			}
		}

		public Guid ParentId {
			get {
				return parentId;
			}
			set {
				parentId = value;
			}
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			if (obj.GetType() != typeof(TestChildWithoutParent)) {
				return false;
			}
			return Equals((TestChildWithoutParent)obj);
		}

		public override int GetHashCode() {
			return id.GetHashCode();
		}

		public bool Equals(TestChildWithoutParent other) {
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
