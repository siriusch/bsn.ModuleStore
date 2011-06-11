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
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace bsn.ModuleStore.Mapper.Serialization {
	internal struct MembersMethods {
		private static readonly Dictionary<MembersKey, MembersMethods> methods = new Dictionary<MembersKey, MembersMethods>();

		public static MembersMethods Get(MemberInfo[] members) {
			MembersKey key = new MembersKey(members);
			MembersMethods result;
			lock (methods) {
				if (!methods.TryGetValue(key, out result)) {
					result = new MembersMethods(members);
					methods.Add(key, result);
				}
			}
			return result;
		}

		private static object GetNone(object instance, int index) {
			return null;
		}

		private static T NotNull<T>(T value) where T: class {
			if (value == null) {
				throw new ArgumentNullException("value");
			}
			return value;
		}

		private static void ArrayNone(object instance, object[] data) {}

		private readonly Type commonType;
		private readonly Action<object, object[]> extractMembers;
		private readonly Func<object, int, object> getMember;
		private readonly Action<object, object[]> populateMembers;

		private MembersMethods(MemberInfo[] members) {
			if (members == null) {
				throw new ArgumentNullException("members");
			}
			if (members.Length > 0) {
				commonType = MembersKey.GetCommonType(members);
				Type arg0Type = commonType.IsValueType ? typeof(object) : commonType;
				DynamicMethod populateMethod = new DynamicMethod(String.Format("{0}.PopulateMembers", commonType.FullName), null, new[] {arg0Type, typeof(object), typeof(object[])}, commonType, true);
				DynamicMethod extractMethod = new DynamicMethod(String.Format("{0}.ExtractMembers", commonType.FullName), null, new[] { arg0Type, typeof(object), typeof(object[]) }, commonType, true);
				DynamicMethod getMemberMethod = new DynamicMethod(String.Format("{0}.GetMember", commonType.FullName), typeof(object), new[] { arg0Type, typeof(object), typeof(int) }, commonType, true);
				ILGenerator populateIl = populateMethod.GetILGenerator();
				ILGenerator extractIl = extractMethod.GetILGenerator();
				ILGenerator getMemberIl = getMemberMethod.GetILGenerator();
				Label[] memberLabels = new Label[members.Length];
				populateIl.Emit(OpCodes.Ldarg_1);
				populateIl.Emit(OpCodes.Castclass, commonType);
				extractIl.Emit(OpCodes.Ldarg_1);
				extractIl.Emit(OpCodes.Castclass, commonType);
				for (int i = 0; i < memberLabels.Length; i++) {
					memberLabels[i] = getMemberIl.DefineLabel();
				}
				getMemberIl.Emit(OpCodes.Ldarg_1);
				getMemberIl.Emit(OpCodes.Castclass, commonType);
				OpCode loadInstance;
				if (commonType != arg0Type) {
					populateIl.DeclareLocal(commonType);
					extractIl.DeclareLocal(commonType);
					getMemberIl.DeclareLocal(commonType);
					populateIl.Emit(OpCodes.Stloc_0);
					extractIl.Emit(OpCodes.Stloc_0);
					getMemberIl.Emit(OpCodes.Stloc_0);
					loadInstance = OpCodes.Ldloc_0;
				} else {
					populateIl.Emit(OpCodes.Starg_S, 0);
					extractIl.Emit(OpCodes.Starg_S, 0);
					getMemberIl.Emit(OpCodes.Starg_S, 0);
					loadInstance = OpCodes.Ldarg_0;
				}
				getMemberIl.Emit(OpCodes.Ldarg_2);
				getMemberIl.Emit(OpCodes.Switch, memberLabels);
				getMemberIl.Emit(OpCodes.Ldstr, "index");
				getMemberIl.Emit(OpCodes.Newobj, typeof(ArgumentOutOfRangeException).GetConstructor(new[] {typeof(string)}));
				getMemberIl.Emit(OpCodes.Throw);
				for (int i = 0; i < members.Length; i++) {
					MemberInfo member = members[i];
					populateIl.Emit(loadInstance);
					populateIl.Emit(OpCodes.Ldarg_2);
					populateIl.Emit(OpCodes.Ldc_I4, i);
					populateIl.Emit(OpCodes.Ldelem, typeof(object));
					extractIl.Emit(OpCodes.Ldarg_2);
					extractIl.Emit(OpCodes.Ldc_I4, i);
					extractIl.Emit(loadInstance);
					getMemberIl.MarkLabel(memberLabels[i]);
					getMemberIl.Emit(loadInstance);
					FieldInfo field = member as FieldInfo;
					if (field != null) {
						populateIl.Emit(OpCodes.Unbox_Any, field.FieldType);
						populateIl.Emit(OpCodes.Stfld, field);
						extractIl.Emit(OpCodes.Ldfld, field);
						getMemberIl.Emit(OpCodes.Ldfld, field);
						if (field.FieldType.IsValueType) {
							extractIl.Emit(OpCodes.Box, field.FieldType);
							getMemberIl.Emit(OpCodes.Box, field.FieldType);
						}
					} else {
						PropertyInfo property = member as PropertyInfo;
						if (property != null) {
							populateIl.Emit(OpCodes.Unbox_Any, property.PropertyType);
							populateIl.Emit(OpCodes.Callvirt, NotNull(property.GetSetMethod(true)));
							extractIl.Emit(OpCodes.Callvirt, NotNull(property.GetGetMethod(true)));
							getMemberIl.Emit(OpCodes.Callvirt, NotNull(property.GetGetMethod(true)));
							if (property.PropertyType.IsValueType) {
								extractIl.Emit(OpCodes.Box, property.PropertyType);
								getMemberIl.Emit(OpCodes.Box, property.PropertyType);
							}
						} else {
							throw new InvalidOperationException("Field or property expected");
						}
					}
					extractIl.Emit(OpCodes.Stelem, typeof(object));
					getMemberIl.Emit(OpCodes.Ret);
				}
				populateIl.Emit(OpCodes.Ret);
				extractIl.Emit(OpCodes.Ret);
				populateMembers = (Action<object, object[]>)populateMethod.CreateDelegate(typeof(Action<object, object[]>), null);
				extractMembers = (Action<object, object[]>)extractMethod.CreateDelegate(typeof(Action<object, object[]>), null);
				getMember = (Func<object, int, object>)getMemberMethod.CreateDelegate(typeof(Func<object, int, object>), null);
			} else {
				commonType = typeof(object);
				populateMembers = ArrayNone;
				extractMembers = ArrayNone;
				getMember = GetNone;
			}
		}

		public Type CommonType {
			get {
				return commonType;
			}
		}

		public Action<object, object[]> ExtractMembers {
			get {
				return extractMembers;
			}
		}

		public Func<object, int, object> GetMember {
			get {
				return getMember;
			}
		}

		public Action<object, object[]> PopulateMembers {
			get {
				return populateMembers;
			}
		}
	}
}
