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
using System.Linq;
using System.Reflection;

using Xunit;

namespace bsn.ModuleStore.Mapper.Serialization {
	public class MembersKeyTest {
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

		private class MembersC: MembersA {
#pragma warning disable 169
			private object c;
#pragma warning restore 169
		}

		private class MembersD: MembersA {}

		private static readonly MemberInfo[] membersA = typeof(MembersA).GetAllFieldsAndProperties().ToArray();
		private static readonly MemberInfo[] membersB = typeof(MembersB).GetAllFieldsAndProperties().ToArray();
		private static readonly MemberInfo[] membersC = typeof(MembersC).GetAllFieldsAndProperties().ToArray();
		private static readonly MemberInfo[] membersD = typeof(MembersD).GetAllFieldsAndProperties().ToArray();

		[Fact]
		public void CommonType() {
			Assert.Equal(typeof(MembersC), MembersKey.GetCommonType(membersC));
		}

		[Fact]
		public void Different() {
			Assert.NotEqual(new MembersKey(membersA), new MembersKey(membersB));
		}

		[Fact]
		public void DifferentHashCode() {
			Assert.NotEqual(new MembersKey(membersA).GetHashCode(), new MembersKey(membersB).GetHashCode());
		}

		[Fact]
		public void DifferentHashCodeWithEmpty() {
			Assert.NotEqual(new MembersKey(membersA).GetHashCode(), new MembersKey().GetHashCode());
		}

		[Fact]
		public void DifferentWithEmpty() {
			Assert.NotEqual(new MembersKey(membersA), new MembersKey());
		}

		[Fact]
		public void Equal() {
			Assert.Equal(new MembersKey(membersA), new MembersKey(membersD));
		}

		[Fact]
		public void EqualHashCode() {
			Assert.Equal(new MembersKey(membersA).GetHashCode(), new MembersKey(membersD).GetHashCode());
		}

		[Fact]
		public void EqualHashCodeWithEmpty() {
			Assert.Equal(new MembersKey().GetHashCode(), new MembersKey().GetHashCode());
		}
	}
}
