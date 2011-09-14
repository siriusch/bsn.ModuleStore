using System;

namespace bsn.ModuleStore.Mapper.InterfaceMetadata {
	internal class TestChildMultiResultsets: IIdentifiable<Guid>, IEquatable<TestChildMultiResultsets> {
		[SqlColumn("uidChild", Identity = true)]
		private Guid id;

		[SqlColumn("sKeyChild")]
		private string key;

		[SqlColumn("uidParent", GetCachedByIdentity = true)]
		private TestParentMultiResultsets parent;

		public string Key {
			get {
				return key;
			}
			set {
				key = value;
			}
		}

		public TestParentMultiResultsets Parent {
			get {
				return parent;
			}
			set {
				parent = value;
			}
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			if (obj.GetType() != typeof(TestChildMultiResultsets)) {
				return false;
			}
			return Equals((TestChildMultiResultsets)obj);
		}

		public override int GetHashCode() {
			return id.GetHashCode();
		}

		public bool Equals(TestChildMultiResultsets other) {
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
