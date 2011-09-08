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
//  
using System;
using System.Linq;
using System.Reflection;

using NUnit.Framework;

namespace bsn.ModuleStore.Mapper.Serialization {
	[TestFixture]
	public class MembersKeyTest: AssertionHelper {
		private class MembersA {
#pragma warning disable 169
			private int a;
			private Guid b;
			private bool? z;
#pragma warning restore 169
		}

		private class MembersB {
#pragma warning disable 169
			private int a;
			private Guid b;
#pragma warning restore 169
		}

		private class MembersD: MembersA {}

		private class MembersC: MembersA {
#pragma warning disable 169
			private object c;
#pragma warning restore 169
		}

		private static readonly MemberInfo[] membersA = typeof(MembersA).GetAllFieldsAndProperties().ToArray();
		private static readonly MemberInfo[] membersB = typeof(MembersB).GetAllFieldsAndProperties().ToArray();
		private static readonly MemberInfo[] membersC = typeof(MembersC).GetAllFieldsAndProperties().ToArray();
		private static readonly MemberInfo[] membersD = typeof(MembersD).GetAllFieldsAndProperties().ToArray();

		[Test]
		public void CommonType() {
			Expect(MembersKey.GetCommonType(membersC), EqualTo(typeof(MembersC)));
		}

		[Test]
		public void Different() {
			Expect(new MembersKey(membersA), Not.EqualTo(new MembersKey(membersB)));
		}

		[Test]
		public void DifferentHashCode() {
			Expect(new MembersKey(membersA).GetHashCode(), Not.EqualTo(new MembersKey(membersB).GetHashCode()));
		}

		[Test]
		public void DifferentHashCodeWithEmpty() {
			Expect(new MembersKey(membersA).GetHashCode(), Not.EqualTo(new MembersKey().GetHashCode()));
		}

		[Test]
		public void DifferentWithEmpty() {
			Expect(new MembersKey(membersA), Not.EqualTo(new MembersKey()));
		}

		[Test]
		public void Equal() {
			Expect(new MembersKey(membersA), EqualTo(new MembersKey(membersD)));
		}

		[Test]
		public void EqualHashCode() {
			Expect(new MembersKey(membersA).GetHashCode(), EqualTo(new MembersKey(membersD).GetHashCode()));
		}

		[Test]
		public void EqualHashCodeWithEmpty() {
			Expect(new MembersKey().GetHashCode(), EqualTo(new MembersKey().GetHashCode()));
		}
	}
}
