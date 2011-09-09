#if DEBUG

using System;
using System.Collections.Generic;

namespace bsn.ModuleStore.Mapper.InterfaceMetadata {
	internal class TestParent: IEquatable<TestParent>, IIdentifiable<Guid> {
		private List<TestChild> children;

		[SqlColumn("uidParent", Identity = true)]
		private Guid id;

		[SqlColumn("sKeyParent")]
		private string key;

		public TestParent() {
			children = new List<TestChild>();
		}

		public List<TestChild> Children {
			get {
				if (children == null) {
					children = new List<TestChild>();
				}
				return children;
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
			return Equals((TestParent)obj);
		}

		public override int GetHashCode() {
			return id.GetHashCode();
		}

		public bool Equals(TestParent other) {
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
