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
