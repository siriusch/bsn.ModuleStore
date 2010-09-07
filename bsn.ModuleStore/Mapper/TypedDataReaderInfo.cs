using System;
using System.Collections.Generic;
using System.Reflection;

namespace bsn.ModuleStore.Mapper {
	internal class TypedDataReaderInfo {
		private static readonly Dictionary<Type, TypedDataReaderInfo> infos = new Dictionary<Type, TypedDataReaderInfo>();

		public static TypedDataReaderInfo Get(Type type) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			TypedDataReaderInfo result;
			lock (infos) {
				if (!infos.TryGetValue(type, out result)) {
					result = new TypedDataReaderInfo(type);
					infos.Add(type, result);
				}
			}
			return result;
		}

		private readonly Dictionary<string, KeyValuePair<string, Type>> properties;

		private TypedDataReaderInfo(Type type) {
			if (!typeof(ITypedDataReader).IsAssignableFrom(type)) {
				throw new ArgumentException("The interface must inherit from ITypedDataReader");
			}
			PropertyInfo[] propertyInfos = type.GetProperties();
			properties = new Dictionary<string, KeyValuePair<string, Type>>(propertyInfos.Length);
			foreach (PropertyInfo property in propertyInfos) {
				if ((!property.CanRead) || (property.CanWrite)) {
					throw new NotSupportedException("Properties for the typed data reader must be read-only.");
				}
				properties.Add(property.GetGetMethod().Name, new KeyValuePair<string, Type>(SqlColumnAttribute.GetColumnAttribute(property, true).Name, property.PropertyType));
			}
		}

		public int ColumnCount {
			get {
				return properties.Count;
			}
		}

		public bool TryGetColumnInfo(string propertyGetterName, out KeyValuePair<string, Type> columnAttribute) {
			return properties.TryGetValue(propertyGetterName, out columnAttribute);
		}
	}
}