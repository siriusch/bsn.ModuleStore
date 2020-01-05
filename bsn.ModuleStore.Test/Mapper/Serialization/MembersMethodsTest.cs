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

using Xunit;

namespace bsn.ModuleStore.Mapper.Serialization {
	public class MembersMethodsTest {
		public struct GuidStruct {
			public GuidStruct(Guid a, Guid b) {
				this.a = a;
				this.b = b;
			}

			public Guid A => a;

			public Guid B => b;

			// ReSharper disable FieldCanBeMadeReadOnly.Local
			private Guid a;

			private Guid b;
			// ReSharper restore FieldCanBeMadeReadOnly.Local
		}

		private class Members {
			public Members() {}

			public Members(int a, Guid? b, object c): this() {
				this.a = a;
				this.b = b;
				this.c = c;
			}

			public int A => a;

			public Guid? B => b;

			public object C => c;
#pragma warning disable 169
			// ReSharper disable FieldCanBeMadeReadOnly.Local
			private int a;
			private Guid? b;
			private object c;
			// ReSharper restore FieldCanBeMadeReadOnly.Local
#pragma warning restore 169
		}

		private struct Struct {
#pragma warning disable 169
			// ReSharper disable FieldCanBeMadeReadOnly.Local
			private int a;
			// ReSharper restore FieldCanBeMadeReadOnly.Local
#pragma warning restore 169

			public Struct(int a) {
				this.a = a;
			}

			public int A => a;
		}

		private static readonly MemberInfo[] members = GetMemberFields<Members>();

		private static FieldInfo[] GetMemberFields<T>() {
			var result = typeof(T).GetAllFieldsAndProperties().OfType<FieldInfo>().ToArray();
			Array.Sort(result, (x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.Name, y.Name)); // sort alphabetically
			return result;
		}

		[Fact]
		public void CreateObjectMethods() {
			MembersMethods.Get(members);
		}

		[Fact]
		public void CreateStructMethods() {
			MembersMethods.Get(GetMemberFields<Struct>());
		}

		[Fact]
		public void ExtractMembers() {
			var x = new Members(1, Guid.NewGuid(), new object());
			var data = new object[3];
			MembersMethods.Get(members).ExtractMembers(x, data);
			Assert.Equal(x.A, data[0]);
			Assert.Equal(x.B, data[1]);
			Assert.Equal(x.C, data[2]);
		}

		[Fact]
		public void ExtractStructMembers() {
			var x = new Struct(1);
			var data = new object[1];
			MembersMethods.Get(GetMemberFields<Struct>()).ExtractMembers(x, data);
			Assert.Equal(x.A, data[0]);
		}

		[Fact]
		public void GetMemberNullableValueType() {
			Guid? guid = Guid.NewGuid();
			var x = new Members(0, guid, null);
			Assert.Equal(guid, MembersMethods.Get(members).GetMember(x, 1));
		}

		[Fact]
		public void GetMemberNullableValueTypeNull() {
			var x = new Members(0, null, null);
			Assert.Null(MembersMethods.Get(members).GetMember(x, 1));
		}

		[Fact]
		public void GetMemberReferenceType() {
			var obj = new object();
			var x = new Members(0, null, obj);
			Assert.Equal(obj, MembersMethods.Get(members).GetMember(x, 2));
		}

		[Fact]
		public void GetMemberReferenceTypeNull() {
			var x = new Members(0, null, null);
			Assert.Null(MembersMethods.Get(members).GetMember(x, 2));
		}

		[Fact]
		public void GetMemberValueType() {
			var x = new Members(1, null, null);
			Assert.Equal(1, MembersMethods.Get(members).GetMember(x, 0));
		}

		[Fact]
		public void MethodsCache() {
			// ReSharper disable EqualExpressionComparison
			Assert.Same(MembersMethods.Get(members).GetMember, MembersMethods.Get(members).GetMember);
			Assert.Same(MembersMethods.Get(members).PopulateMembers, MembersMethods.Get(members).PopulateMembers);
			// ReSharper restore EqualExpressionComparison
		}

		[Fact]
		public void PopulateMembersInitial() {
			var x = new Members();
			var guid = Guid.NewGuid();
			var obj = new object();
			MembersMethods.Get(members).PopulateMembers(x, new[] {1, guid, obj});
			Assert.Equal(1, x.A);
			Assert.Equal(guid, x.B);
			Assert.Equal(obj, x.C);
		}

		[Fact]
		public void PopulateMembersOverride() {
			var x = new Members();
			var membersMethods = MembersMethods.Get(members);
			membersMethods.PopulateMembers(x, new[] {1, Guid.NewGuid(), new object()});
			membersMethods.PopulateMembers(x, new object[] {2, null, null});
			Assert.Equal(2, x.A);
			Assert.Null(x.B);
			Assert.Null(x.C);
		}

		[Fact]
		public void PopulateMembersStruct() {
			object o = new GuidStruct();
			var guid1 = Guid.NewGuid();
			var guid2 = Guid.NewGuid();
			MembersMethods.Get(GetMemberFields<GuidStruct>()).PopulateMembers(o, new object[] {guid1, guid2});
			var x = (GuidStruct)o;
			Assert.Equal(guid1, x.A);
			Assert.Equal(guid2, x.B);
		}

		[Fact]
		public void PopulateNullInNonNullField() {
			Assert.Throws<NullReferenceException>(delegate {
				try {
					MembersMethods.Get(members).PopulateMembers(new Members(), new object[] {null, null, null});
				} catch (Exception ex) {
					Trace.WriteLine(ex);
					throw;
				}
			});
		}
	}
}
