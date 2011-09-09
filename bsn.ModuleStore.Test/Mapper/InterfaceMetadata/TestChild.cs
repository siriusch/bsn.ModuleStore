#if DEBUG

using System;

namespace bsn.ModuleStore.Mapper.InterfaceMetadata {
	internal class TestChild: IEquatable<TestChild>, IIdentifiable<Guid>, ISqlDeserializationHook {
		[SqlColumn("uidChild", Identity = true)]
		private Guid id;

		[SqlColumn("sKeyChild")]
		private string key;

		[SqlDeserialize]
		private TestParent parent;

		public string Key {
			get {
				return key;
			}
			set {
				key = value;
			}
		}

		public TestParent Parent {
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
			if (obj.GetType() != typeof(TestChild)) {
				return false;
			}
			return Equals((TestChild)obj);
		}

		public override int GetHashCode() {
			return id.GetHashCode();
		}

		public bool Equals(TestChild other) {
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

		public void AfterDeserialization() {
			// register with parent to build bidirection link
			parent.Children.Add(this);
		}
	}
}

#endif
