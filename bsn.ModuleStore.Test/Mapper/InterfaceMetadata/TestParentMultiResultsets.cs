using System;

namespace bsn.ModuleStore.Mapper.InterfaceMetadata {
	internal class TestParentMultiResultsets: IIdentifiable<Guid>, IEquatable<TestParentMultiResultsets> {
		[SqlColumn("uidParent", Identity = true)]
		private Guid id;

		[SqlColumn("sKeyParent")]
		private string key;

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
			if (obj.GetType() != typeof(TestParentMultiResultsets)) {
				return false;
			}
			return Equals((TestParentMultiResultsets)obj);
		}

		public override int GetHashCode() {
			return id.GetHashCode();
		}

		public bool Equals(TestParentMultiResultsets other) {
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
