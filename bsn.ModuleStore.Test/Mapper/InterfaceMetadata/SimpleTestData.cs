#if DEBUG

using System;

using bsn.ModuleStore.Mapper.Serialization;

namespace bsn.ModuleStore.Mapper.InterfaceMetadata {
	internal class SimpleTestData: IEquatable<SimpleTestData> {
		[SqlColumn("iData")]
		[SqlTableValueParameter(Index = 2)]
		private int data;

		[SqlColumn("uidKey")]
		[SqlTableValueParameter(Index = 0)]
		private Guid id;

		[SqlColumn("sKey")]
		[SqlTableValueParameter(Index = 1)]
		private string key;

		public SimpleTestData(Guid id, string key, int data) {
			this.id = id;
			this.key = key;
			this.data = data;
		}

		public SimpleTestData() {}

		public int Data {
			get {
				return data;
			}
			set {
				data = value;
			}
		}

		public Guid Id {
			get {
				return id;
			}
			set {
				id = value;
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
			if (obj.GetType() != typeof(SimpleTestData)) {
				return false;
			}
			return Equals((SimpleTestData)obj);
		}

		public override int GetHashCode() {
			unchecked {
				int result = data;
				result = (result*397)^id.GetHashCode();
				result = (result*397)^(key != null ? key.GetHashCode() : 0);
				return result;
			}
		}

		public override string ToString() {
			return string.Format("Id: {0}, Key: {1}, Data: {2}", id, key, data);
		}

		public bool Equals(SimpleTestData other) {
			if (ReferenceEquals(null, other)) {
				return false;
			}
			if (ReferenceEquals(this, other)) {
				return true;
			}
			return (other.data == data) && other.id.Equals(id) && String.Equals(other.key, key);
		}
	}
}

#endif
