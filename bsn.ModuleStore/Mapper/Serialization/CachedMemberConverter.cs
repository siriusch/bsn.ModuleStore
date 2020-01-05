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

namespace bsn.ModuleStore.Mapper.Serialization {
	internal class CachedMemberConverter: MemberConverter {
		private abstract class IdentifiableGetter {
			internal sealed class Generic<T>: IdentifiableGetter where T: struct, IEquatable<T> {
				// ReSharper disable StaticFieldInGenericType
				// ReSharper disable UnusedMember.Local
				public static readonly Generic<T> Default = new Generic<T>();
				// ReSharper restore UnusedMember.Local
				// ReSharper restore StaticFieldInGenericType

				public override Type Type => typeof(T);

				public override object GetIdentity(object instance) {
					if (instance is IIdentifiable<T> identifiable) {
						return identifiable.Id;
					}
					return instance;
				}
			}

			public abstract Type Type {
				get;
			}

			public abstract object GetIdentity(object instance);
		}

		private readonly IdentifiableGetter getter;
		private readonly IMemberConverter identityMember;

		public CachedMemberConverter(Type type, bool isIdentity, string columnName, int memberIndex, DateTimeKind dateTimeKind): base(type, isIdentity, columnName, memberIndex) {
			foreach (var member in type.GetAllFieldsAndProperties()) {
				var columnAttribute = SqlColumnAttributeBase.Get<SqlColumnAttribute>(member, false);
				if ((columnAttribute != null) && columnAttribute.Identity) {
					identityMember = Get(member.GetMemberType(), false, columnName, memberIndex, dateTimeKind);
					break;
				}
			}
			if (identityMember == null) {
				throw new InvalidOperationException($"The type {type.FullName} cannot be retrieved from the cache because it lacks an identity column");
			}
			foreach (var @interface in type.GetInterfaces()) {
				if (@interface.IsGenericType && typeof(IIdentifiable<>).Equals(@interface.GetGenericTypeDefinition())) {
					// ReSharper disable AssignNullToNotNullAttribute
					getter = (IdentifiableGetter)typeof(IdentifiableGetter.Generic<>).MakeGenericType(@interface.GetGenericArguments()).GetField("Default").GetValue(null);
					// ReSharper restore AssignNullToNotNullAttribute
					break;
				}
			}
		}

		public override Type DbClrType {
			get {
				if (getter != null) {
					return getter.Type;
				}
				return base.DbClrType;
			}
		}

		public override object ProcessFromDb(IDeserializerContext context, int column) {
			var identity = identityMember.ProcessFromDb(context, column);
			if (identity != null) {
				var result = context.GetInstance(Type, identity, out var instanceOrigin);
				if (instanceOrigin == InstanceOrigin.New) {
					context.RequireDeserialization(result);
				}
				return result;
			}
			return null;
		}

		public override object ProcessToDb(object value) {
			if (getter != null) {
				return getter.GetIdentity(value) ?? DBNull.Value;
			}
			return base.ProcessToDb(value);
		}
	}
}
