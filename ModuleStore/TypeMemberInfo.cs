using System;
using System.Reflection;

namespace bsn.ModuleStore.Console {
	[Serializable]
	internal struct TypeMemberInfo {
		private readonly MemberTypes memberType;
		private readonly string memberName;

		public TypeMemberInfo(MemberInfo info) {
			memberType = info.MemberType;
			memberName = info.Name;
		}

		public bool Equals(TypeMemberInfo other) {
			return Equals(other.memberType, memberType) && Equals(other.memberName, memberName);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (obj.GetType() != typeof(TypeMemberInfo)) {
				return false;
			}
			return Equals((TypeMemberInfo)obj);
		}

		public MemberTypes MemberType {
			get {
				return memberType;
			}
		}

		public string MemberName {
			get {
				return memberName;
			}
		}

		public override int GetHashCode() {
			unchecked {
				return (memberType.GetHashCode()*397)^(memberName != null ? memberName.GetHashCode() : 0);
			}
		}

		public override string ToString() {
			return memberName;
		}
	}
}