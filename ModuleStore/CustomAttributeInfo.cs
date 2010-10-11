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
using System.Linq;
using System.Reflection;

namespace bsn.ModuleStore.Console {
	[Serializable]
	internal struct CustomAttributeInfo {
		private static object GetSimpleValue(object value) {
			if (value != null) {
				Type valueType = value.GetType();
				if (valueType.IsEnum) {
					return Convert.ChangeType(value, Enum.GetUnderlyingType(valueType));
				}
				if (typeof(Type).IsAssignableFrom(valueType)) {
					return new QualifiedTypeNameInfo((Type)value);
				}
			}
			return value;
		}

		private readonly QualifiedTypeNameInfo attributeType;
		private readonly KeyValuePair<QualifiedTypeNameInfo, object>[] constructorArguments;
		private readonly KeyValuePair<TypeMemberInfo, object>[] namedArguments;

		public CustomAttributeInfo(CustomAttributeData data) {
			attributeType = new QualifiedTypeNameInfo(data.Constructor.DeclaringType);
			constructorArguments = data.Constructor.GetParameters().OrderBy(p => p.Position).Select(p => new KeyValuePair<QualifiedTypeNameInfo, object>(new QualifiedTypeNameInfo(p.ParameterType), GetSimpleValue(data.ConstructorArguments[p.Position].Value))).ToArray();
			if (data.NamedArguments == null) {
				namedArguments = new KeyValuePair<TypeMemberInfo, object>[0];
			} else {
				namedArguments = data.NamedArguments.Select(argument => new KeyValuePair<TypeMemberInfo, object>(new TypeMemberInfo(argument.MemberInfo), GetSimpleValue(argument.TypedValue.Value))).ToArray();
			}
		}

		public QualifiedTypeNameInfo AttributeType {
			get {
				return attributeType;
			}
		}

		public KeyValuePair<QualifiedTypeNameInfo, object>[] ConstructorArguments {
			get {
				return constructorArguments;
			}
		}

		public KeyValuePair<TypeMemberInfo, object>[] NamedArguments {
			get {
				return namedArguments;
			}
		}
	}
}
