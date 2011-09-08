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
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using NUnit.Framework;

namespace bsn.ModuleStore.Mapper.Serialization {
	[TestFixture]
	public class MembersMethodsTest: AssertionHelper {
		private class Members {
			private readonly int a;
			private readonly Guid? b;
			private readonly object c;

			public Members() {}

			public Members(int a, Guid? b, object c): this() {
				this.a = a;
				this.b = b;
				this.c = c;
			}

			public int A {
				get {
					return a;
				}
			}

			public Guid? B {
				get {
					return b;
				}
			}

			public object C {
				get {
					return c;
				}
			}
		}

		private struct Struct {
#pragma warning disable 169
			private readonly int a;
#pragma warning restore 169

			public Struct(int a) {
				this.a = a;
			}

			public int A {
				get {
					return a;
				}
			}
		}

	public struct GuidStruct {
		private Guid a;

		private Guid b;

		public GuidStruct(Guid a, Guid b) {
			this.a = a;
			this.b = b;
		}

		public Guid A {
			get {
				return a;
			}
		}

		public Guid B {
			get {
				return b;
			}
		}
	}

		private static readonly MemberInfo[] members = GetMemberFields<Members>();

		private static FieldInfo[] GetMemberFields<T>() {
			FieldInfo[] result = typeof(T).GetAllFieldsAndProperties().OfType<FieldInfo>().ToArray();
			Array.Sort(result, (x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.Name, y.Name)); // sort alphabetically
			return result;
		}

		[Test]
		public void CreateObjectMethods() {
			MembersMethods.Get(members);
		}

		[Test]
		public void CreateStructMethods() {
			MembersMethods.Get(GetMemberFields<Struct>());
		}

		[Test]
		public void ExtractMembers() {
			Members x = new Members(1, Guid.NewGuid(), new object());
			object[] data = new object[3];
			MembersMethods.Get(members).ExtractMembers(x, data);
			Expect(data[0], EqualTo(x.A));
			Expect(data[1], EqualTo(x.B));
			Expect(data[2], EqualTo(x.C));
		}

		[Test]
		public void ExtractStructMembers() {
			Struct x = new Struct(1);
			object[] data = new object[1];
			MembersMethods.Get(GetMemberFields<Struct>()).ExtractMembers(x, data);
			Expect(data[0], EqualTo(x.A));
		}

		[Test]
		public void GetMemberNullableValueType() {
			Guid? guid = Guid.NewGuid();
			Members x = new Members(0, guid, null);
			Expect(MembersMethods.Get(members).GetMember(x, 1), EqualTo(guid));
		}

		[Test]
		public void GetMemberNullableValueTypeNull() {
			Members x = new Members(0, null, null);
			Expect(MembersMethods.Get(members).GetMember(x, 1), Null);
		}

		[Test]
		public void GetMemberReferenceType() {
			object obj = new object();
			Members x = new Members(0, null, obj);
			Expect(MembersMethods.Get(members).GetMember(x, 2), EqualTo(obj));
		}

		[Test]
		public void GetMemberReferenceTypeNull() {
			Members x = new Members(0, null, null);
			Expect(MembersMethods.Get(members).GetMember(x, 2), Null);
		}

		[Test]
		public void GetMemberValueType() {
			Members x = new Members(1, null, null);
			Expect(MembersMethods.Get(members).GetMember(x, 0), EqualTo(1));
		}

		[Test]
		public void MethodsCache() {
			// ReSharper disable EqualExpressionComparison
			Expect(ReferenceEquals(MembersMethods.Get(members).GetMember, MembersMethods.Get(members).GetMember));
			Expect(ReferenceEquals(MembersMethods.Get(members).PopulateMembers, MembersMethods.Get(members).PopulateMembers));
			// ReSharper restore EqualExpressionComparison
		}

		[Test]
		public void PopulateMembersInitial() {
			Members x = new Members();
			Guid guid = Guid.NewGuid();
			object obj = new object();
			MembersMethods.Get(members).PopulateMembers(x, new[] {1, guid, obj});
			Expect(x.A, EqualTo(1));
			Expect(x.B, EqualTo(guid));
			Expect(x.C, EqualTo(obj));
		}

		[Test]
		public void PopulateMembersStruct() {
			object o = new GuidStruct();
			Guid guid1 = Guid.NewGuid();
			Guid guid2 = Guid.NewGuid();
			MembersMethods.Get(GetMemberFields<GuidStruct>()).PopulateMembers(o, new object[] { guid1, guid2 });
			GuidStruct x = (GuidStruct)o;
			Expect(x.A, EqualTo(guid1));
			Expect(x.B, EqualTo(guid2));
		}

		[Test]
		public void PopulateMembersOverride() {
			Members x = new Members();
			MembersMethods membersMethods = MembersMethods.Get(members);
			membersMethods.PopulateMembers(x, new[] {1, Guid.NewGuid(), new object()});
			membersMethods.PopulateMembers(x, new object[] {2, null, null});
			Expect(x.A, EqualTo(2));
			Expect(x.B, Null);
			Expect(x.C, Null);
		}

		[Test]
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
