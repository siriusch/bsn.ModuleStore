using System;

namespace bsn.ModuleStore.Console {
	[Serializable]
	internal struct QualifiedTypeNameInfo {
		private readonly string assemblyName;
		private readonly string typeName;

		public QualifiedTypeNameInfo(Type type): this(type.Assembly.FullName, type.FullName) {}

		public QualifiedTypeNameInfo(string assemblyName, string typeName) {
			this.assemblyName = assemblyName;
			this.typeName = typeName;
		}

		public string AssemblyName {
			get {
				return assemblyName;
			}
		}

		public string TypeName {
			get {
				return typeName;
			}
		}

		public bool Equals(QualifiedTypeNameInfo other) {
			return Equals(other.assemblyName, assemblyName) && Equals(other.typeName, typeName);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (obj.GetType() != typeof(QualifiedTypeNameInfo)) {
				return false;
			}
			return Equals((QualifiedTypeNameInfo)obj);
		}

		public Type FindType(bool throwOnError) {
			return Type.GetType(GetAssemblyQualifiedName(), throwOnError, false);
		}

		public string GetAssemblyQualifiedName() {
			return String.Format("{0}, {1}", typeName, assemblyName);
		}

		public override int GetHashCode() {
			unchecked {
				return ((assemblyName != null ? assemblyName.GetHashCode() : 0)*397)^(typeName != null ? typeName.GetHashCode() : 0);
			}
		}

		public override string ToString() {
			return GetAssemblyQualifiedName();
		}
	}
}