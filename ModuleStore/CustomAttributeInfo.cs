using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace bsn.ModuleStore.Console {
	[Serializable]
	internal struct CustomAttributeInfo {
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