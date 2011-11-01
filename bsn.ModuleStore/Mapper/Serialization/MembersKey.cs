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
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace bsn.ModuleStore.Mapper.Serialization {
	internal struct MembersKey: IEquatable<MembersKey> {
		public static Type GetCommonType(MemberInfo[] members) {
			if (members == null) {
				throw new ArgumentNullException("members");
			}
			if (members.Length > 0) {
				Type result = members[0].DeclaringType;
				for (int i = 1; i < members.Length; i++) {
					Type declaringType = members[i].DeclaringType;
					if (!declaringType.IsAssignableFrom(result)) {
						Debug.Assert(result.IsAssignableFrom(declaringType));
						result = declaringType;
					}
				}
				return result;
			}
			return typeof(object);
		}

		private readonly MemberInfo[] members;

		public MembersKey(MemberInfo[] members) {
			if (members == null) {
				throw new ArgumentNullException("members");
			}
			this.members = (MemberInfo[])members.Clone();
		}

		public override bool Equals(object obj) {
			if (obj is MembersKey) {
				return Equals((MembersKey)obj);
			}
			return false;
		}

		public override int GetHashCode() {
			int result = typeof(MembersKey).GetHashCode();
			if (members != null) {
				unchecked {
					result = members.Aggregate(result, (current, t) => current^current*13+t.GetHashCode());
				}
			}
			return result;
		}

		public bool Equals(MembersKey other) {
			if (other.members == members) {
				return true;
			}
			if ((other.members == null) || (members == null) || (other.members.Length != members.Length)) {
				return false;
			}
			for (int i = 0; i < members.Length; i++) {
				if (other.members[i] != members[i]) {
					return false;
				}
			}
			return true;
		}
	}
}
