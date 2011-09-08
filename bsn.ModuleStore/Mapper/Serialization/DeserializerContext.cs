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
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Xml;

namespace bsn.ModuleStore.Mapper.Serialization {
	public sealed class DeserializerContext {
		private readonly IDictionary<SqlDeserializer, object[]> buffers = new Dictionary<SqlDeserializer, object[]>();
		private readonly bool callConstructor;
		private readonly SqlDeserializationContext context;
		private readonly SqlDataReader dataReader;
		private XmlNameTable nameTable;
		private XmlDocument xmlDocument;

		internal DeserializerContext(SqlDeserializationContext context, SqlDataReader dataReader, bool callConstructor, XmlNameTable nameTable) {
			this.dataReader = dataReader;
			this.context = context;
			this.callConstructor = callConstructor;
			this.nameTable = nameTable;
		}

		public SqlDataReader DataReader {
			get {
				return dataReader;
			}
		}

		public XmlNameTable NameTable {
			get {
				if (nameTable == null) {
					nameTable = new NameTable();
				}
				return nameTable;
			}
		}

		public XmlDocument XmlDocument {
			get {
				if (xmlDocument == null) {
					xmlDocument = new XmlDocument(NameTable);
				}
				return xmlDocument;
			}
		}

		public object GetInstance(Type instanceType, object identity, out InstanceOrigin instanceOrigin) {
			object result;
			if (!context.TryGetInstance(instanceType, identity, out result, out instanceOrigin)) {
				instanceOrigin = InstanceOrigin.New;
				result = (callConstructor) ? Activator.CreateInstance(instanceType, true) : FormatterServices.GetUninitializedObject(instanceType);
			}
			return result;
		}

		public void RequireDeserialization(object obj) {
			context.AssertDeserialization(obj);
		}

		internal object[] GetBuffer(SqlDeserializer deserializer) {
			object[] result;
			if (!buffers.TryGetValue(deserializer, out result)) {
				result = new object[deserializer.TypeInfo.Mapping.MemberCount];
				buffers.Add(deserializer, result);
			}
			return result;
		}
	}
}
