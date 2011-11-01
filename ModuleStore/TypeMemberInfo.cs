// bsn ModuleStore database versioning
// -----------------------------------
// 
// Copyright 2010 by Arsène von Wyss - avw@gmx.ch
// 
// Development has been supported by Sirius Technologies AG, Basel
// 
// Source:
// 
// https://bsn-modulestore.googlecode.com/hg/
// 
// License:
// 
// The library is distributed under the GNU Lesser General Public License:
// http://www.gnu.org/licenses/lgpl.html
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Reflection;

namespace bsn.ModuleStore.Console {
	[Serializable]
	internal struct TypeMemberInfo {
		private readonly string memberName;
		private readonly MemberTypes memberType;

		public TypeMemberInfo(MemberInfo info) {
			memberType = info.MemberType;
			memberName = info.Name;
		}

		public string MemberName {
			get {
				return memberName;
			}
		}

		public MemberTypes MemberType {
			get {
				return memberType;
			}
		}

		public bool Equals(TypeMemberInfo other) {
			return Equals(other.memberType, memberType) && Equals(other.memberName, memberName);
		}

		public override bool Equals(object obj) {
			if (obj == null) {
				return false;
			}
			if (obj.GetType() != typeof(TypeMemberInfo)) {
				return false;
			}
			return Equals((TypeMemberInfo)obj);
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
