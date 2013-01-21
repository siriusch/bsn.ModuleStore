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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;

namespace bsn.ModuleStore.Mapper.Serialization {
	/// <summary>
	/// The SqlDeserializer generic class allows to create an efficient database Deserializer for classes and structs which transforms database rows into object instances.
	/// <br/><br/>
	/// All fields which are to be deserialized from the database need to habe either a <see cref="SqlColumnAttribute"/> or <see cref="SqlDeserializeAttribute"/> attached to them.
	/// <br/><br/>
	/// If the class implements <see cref="ISqlDeserializationHook"/>, it will be notified after deserialization of the single instance (database row) is complete.
	/// </summary>
	/// <remarks>Auto-Properties cannot be deserialized, use a backing field instead.<br/><br/>XML deserialization is supported for the following types: XPathDocument, XmlDocument, XmlElement</remarks>
	/// <seealso cref="SqlColumnAttribute"/>
	/// <seealso cref="SqlDeserializeAttribute"/>
	/// <seealso cref="ISqlDeserializationHook"/>
	/// <typeparam name="T">The type of the object to be deserialized to.</typeparam>
	public class SqlDeserializer<T>: SqlDeserializer {
		public static T Mock(object columnData, IMetadataProvider metadataProvider = null, bool callConstructor = false) {
			Dictionary<string, object> dataDictionary = new Dictionary<string, object>(StringComparer.Ordinal);
			foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(columnData)) {
				dataDictionary.Add(property.Name, property.GetValue(columnData));
			}
			return Mock(dataDictionary, metadataProvider, callConstructor);
		}

		public static T Mock(IDictionary<string, object> columnData, IMetadataProvider metadataProvider = null, bool callConstructor = false) {
			return (T)Mock(typeof(T), columnData, metadataProvider, callConstructor);
		}

		private readonly bool callConstructor;

		/// <summary>
		/// Creates a new database Deserializer which will read from the given reader.
		/// </summary>
		/// <remarks>Disposing the DbDeserializer will also dispose the reader. Also, you must not continue to use the Deserializer afterwards if you call <see cref="SqlDataReader.NextResult"/>.</remarks>
		/// <param name="reader">The SqlDataReader to read data from. The DbDeserializer will take ownership of the reader.</param>
		/// <param name="callConstructor">If true, the normal default constructor is called instead of creating empty instances. Empty instances, however, are much faster.</param>
		public SqlDeserializer(SqlDeserializationContext context, SqlDataReader reader, bool callConstructor): base(context, reader, typeof(T), true, false) {
			this.callConstructor = callConstructor;
		}

		/// <summary>
		/// Creates a new database Deserializer which will read from the given reader. Object instances will be created without calling the condtructor.
		/// </summary>
		/// <remarks>Disposing the DbDeserializer will also dispose the reader. Also, you must not continue to use the Deserializer afterwards if you call <see cref="SqlDataReader.NextResult"/>.</remarks>
		/// <param name="reader">The SqlDataReader to read data from. The DbDeserializer will take ownership of the reader.</param>
		public SqlDeserializer(SqlDeserializationContext context, SqlDataReader reader): this(context, reader, false) {}

		/// <summary>
		/// Deserialize one row into a new instance, or all available rows for lists. If <typeparamref name="T"/> is not a list (supported types for lists: <see cref="List{T}"/>, <see cref="IList{T}"/>, <see cref="ICollection{T}"/> or <typeparamref name="T"/>[].
		/// </summary>
		/// <returns>An instance of <typeparamref name="T"/></returns>
		public T Deserialize() {
			return Deserialize(int.MaxValue);
		}

		/// <summary>
		/// Deserialize one row into a new instance, or all available rows for lists. If <typeparamref name="T"/> is not a list (supported types for lists: <see cref="List{T}"/>, <see cref="IList{T}"/>, <see cref="ICollection{T}"/> or <typeparamref name="T"/>[].
		/// </summary>
		/// <param name="maxRows">The maximal number of rows to read (for lists).</param>
		/// <returns>An instance of <typeparamref name="T"/></returns>
		public T Deserialize(int maxRows) {
			return (T)DeserializeInternal(maxRows, callConstructor, null);
		}

		/// <summary>
		/// Deserialize all rows as enumeration of instances.
		/// </summary>
		/// <returns>An enumeration of instances of <typeparamref name="T"/></returns>
		public IEnumerable<T> DeserializeInstances() {
			return DeserializeInstances(Int32.MaxValue);
		}

		/// <summary>
		/// Deserialize the given number of rows as enumeration of instances.
		/// </summary>
		/// <param name="maxRows">The maximal number of rows to read.</param>
		/// <returns>
		/// An enumeration of instances of <typeparamref name="T"/>
		/// </returns>
		public IEnumerable<T> DeserializeInstances(int maxRows) {
			if (TypeInfo.IsCollection) {
				throw new NotSupportedException("Collections cannot be deserialized as instances");
			}
			return DeserializeInstancesInternal<T>(maxRows, callConstructor, null);
		}
	}

	/// <summary>
	/// The DbDeserializer class is the base class for the generic <see cref="SqlDeserializer{T}"/> class. Please use the generic version, this class is for internal use only by <see cref="SqlCallProxy"/>.
	/// </summary>
	/// <seealso cref="SqlDeserializer{T}"/>
	public class SqlDeserializer: IDisposable {
		protected internal class DeserializerContext: IDeserializerContext {
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

			internal object[] GetBuffer(SqlDeserializer deserializer) {
				object[] result;
				if (!buffers.TryGetValue(deserializer, out result)) {
					result = new object[deserializer.TypeInfo.Mapping.MemberCount];
					buffers.Add(deserializer, result);
				}
				return result;
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
					result = CreateInstance(instanceType, callConstructor);
				}
				return result;
			}

			public void RequireDeserialization(object obj) {
				context.AssertDeserialization(obj);
			}

			public bool IsDeserialized(object obj) {
				return context.IsDeserialized(obj);
			}

			public SqlXml GetSqlXml(int column) {
				return dataReader.GetSqlXml(column);
			}

			public object GetValue(int column) {
				return dataReader.GetValue(column);
			}
		}

		private class MockContext: IDeserializerContext {
			private readonly IDictionary<int, object> columnData;
			private readonly IDictionary<string, int> columnIndex;
			private XmlNameTable nameTable;
			private XmlDocument xmlDocument;

			public MockContext(IDictionary<string, object> dictionary, ISerializationTypeMapping mapping) {
				List<IMemberConverter> memberConverters = mapping.Converters.Where(m => !string.IsNullOrEmpty(m.ColumnName)).ToList();
				columnIndex = memberConverters.ToDictionary(m => m.ColumnName, m => m.MemberIndex, StringComparer.Ordinal);
				columnData = memberConverters.ToDictionary(m => m.MemberIndex, m => dictionary[m.ColumnName]);
			}

			public int GetOrdinal(string columnName) {
				return columnIndex[columnName];
			}

			public SqlXml GetSqlXml(int column) {
				object value = GetValue(column);
				return (SqlXml)value;
			}

			public object GetValue(int column) {
				return columnData[column] ?? DBNull.Value;
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
				throw new NotSupportedException("The mock doesn't support this operation");
			}

			public bool IsDeserialized(object obj) {
				return false;
			}

			public void RequireDeserialization(object obj) {}
		}

		private static bool AreAllBufferElementsNull(object[] buffer) {
			bool allBufferElementsNull = true;
			for (int i = 0; (i < buffer.Length) && (allBufferElementsNull); i++) {
				allBufferElementsNull &= (buffer[i] == null);
			}
			return allBufferElementsNull;
		}

		protected static object CreateInstance(Type instanceType, bool callConstructor) {
			return (callConstructor) ? Activator.CreateInstance(instanceType, true) : FormatterServices.GetUninitializedObject(instanceType);
		}

		protected static object Mock(Type type, IDictionary<string, object> columnData, IMetadataProvider metadataProvider = null, bool callConstructor = false) {
			ISerializationTypeMapping mapping = ((metadataProvider != null) ? metadataProvider.SerializationTypeInfoProvider : ModuleDatabase.SerializationTypeInfoProvider).TypeMappingProvider.GetMapping(type);
			MockContext context = new MockContext(columnData, mapping);
			object[] buffer = new object[mapping.MemberCount];
			foreach (IMemberConverter converter in mapping.Converters) {
				NestedMemberConverter nestedConverter = converter as NestedMemberConverter;
				buffer[converter.MemberIndex] = nestedConverter == null ? converter.ProcessFromDb(context, context.GetOrdinal(converter.ColumnName)) : Mock(nestedConverter.Type, columnData, metadataProvider, callConstructor);
			}
			object result = CreateInstance(type, callConstructor);
			mapping.PopulateInstanceMembers(result, buffer);
			ISqlDeserializationHook callback = result as ISqlDeserializationHook;
			if (callback != null) {
				callback.AfterDeserialization();
			}
			return result;
		}

		private readonly SortedList<int, MemberConverter> columnConverters;
		private readonly SqlDeserializationContext context;
		private readonly Dictionary<NestedMemberConverter, SqlDeserializer> nestedDeserializers;
		private readonly SqlDataReader reader;
		private readonly bool returnNullOnEmptyBuffer;
		private readonly ISerializationTypeInfo typeInfo;
		private bool disposeReader;

		internal SqlDeserializer(SqlDeserializationContext context, SqlDataReader reader, Type type, bool disposeReader, bool returnNullOnEmptyBuffer) {
			if (context == null) {
				throw new ArgumentNullException("context");
			}
			if (reader == null) {
				throw new ArgumentNullException("reader");
			}
			if (type.IsAbstract || type.IsInterface || type.IsPrimitive || type.IsPointer || typeof(ResultSet).IsAssignableFrom(type)) {
				throw new NotSupportedException("Deserialization only supports normal classes and structs.");
			}
			this.returnNullOnEmptyBuffer = returnNullOnEmptyBuffer;
			this.context = context;
			this.reader = reader;
			this.disposeReader = disposeReader;
			typeInfo = context.GetSerializationTypeInfo(type);
			foreach (MemberConverter converter in typeInfo.Mapping.Converters) {
				NestedMemberConverter nestedConverter = converter as NestedMemberConverter;
				if (nestedConverter != null) {
					if (nestedDeserializers == null) {
						nestedDeserializers = new Dictionary<NestedMemberConverter, SqlDeserializer>();
					}
#warning Cyclic nested members would cause a stack overflow here
					SqlDeserializer nestedDeserializer = new SqlDeserializer(context, reader, nestedConverter.Type, false, true);
					NestedListMemberConverter nestedListConverter = nestedConverter as NestedListMemberConverter;
					nestedDeserializers.Add(nestedListConverter ?? nestedConverter, nestedDeserializer);
				} else {
					if (columnConverters == null) {
						columnConverters = new SortedList<int, MemberConverter>(typeInfo.Mapping.Converters.Count);
					}
					columnConverters.Add(reader.GetOrdinal(converter.ColumnName), converter);
				}
			}
		}

		public bool DisposeReader {
			get {
				return disposeReader;
			}
			set {
				disposeReader = value;
			}
		}

		/// <summary>
		/// The <see cref="SqlDataReader"/> which was passed into the constructor.
		/// </summary>
		/// <remarks>As long as the DbDeserializer is in use, do not alter the state of the Reader. Especially, do not call <see cref="SqlDataReader.NextResult"/>.</remarks>
		public SqlDataReader Reader {
			get {
				return reader;
			}
		}

		/// <summary>
		/// Gets the type information used for deserialization.
		/// </summary>
		/// <value>The type info.</value>
		public ISerializationTypeInfo TypeInfo {
			get {
				return typeInfo;
			}
		}

		protected object CreateInstance(DeserializerContext context, out InstanceOrigin instanceOrigin) {
			Debug.Assert(context != null);
			object identity = null;
			object[] buffer = context.GetBuffer(this);
			if (columnConverters != null) {
				foreach (KeyValuePair<int, MemberConverter> converter in columnConverters) {
					object value = converter.Value.ProcessFromDb(context, converter.Key);
					if (converter.Value.IsIdentity) {
						identity = value;
					}
					buffer[converter.Value.MemberIndex] = value;
				}
			}
			if (nestedDeserializers != null) {
				foreach (KeyValuePair<NestedMemberConverter, SqlDeserializer> nested in nestedDeserializers) {
					InstanceOrigin nestedOrigin;
					buffer[nested.Key.MemberIndex] = nested.Value.CreateInstance(context, out nestedOrigin);
					if (nested.Key is NestedListMemberConverter) {
						IList list = nested.Value.TypeInfo.CreateList();
						if (buffer[nested.Key.MemberIndex] != null) {
							list.Add(buffer[nested.Key.MemberIndex]);
						}
						buffer[nested.Key.MemberIndex] = list;
					}
				}
			}
			if (returnNullOnEmptyBuffer) {
				if (AreAllBufferElementsNull(buffer)) {
					instanceOrigin = InstanceOrigin.None;
					return null;
				}
			}
			object result = context.GetInstance(typeInfo.InstanceType, identity, out instanceOrigin);
			if (result != null) {
				if (!context.IsDeserialized(result)) {
					if (instanceOrigin == InstanceOrigin.ResultSet) {
						// this was a forward-created reference (see CachedMemberConverter.ProcessFromDb), thus mark it as being new
						instanceOrigin = InstanceOrigin.New;
					}
					TypeInfo.Mapping.PopulateInstanceMembers(result, buffer);
					this.context.NotifyInstancePopulated(result);
					if (typeInfo.RequiresNotification) {
						((ISqlDeserializationHook)result).AfterDeserialization();
					}
				} else if (instanceOrigin == InstanceOrigin.ResultSet) {
					// add the instances to the nested lists
					if (nestedDeserializers != null) {
						foreach (KeyValuePair<NestedMemberConverter, SqlDeserializer> nested in nestedDeserializers) {
							if (nested.Key is NestedListMemberConverter) {
								IList list = buffer[nested.Key.MemberIndex] as IList;
								IList resultList = typeInfo.Mapping.GetMember(result, nested.Key.MemberIndex) as IList;
								if ((list != null) && (resultList != null)) {
									foreach (object entry in list) {
										resultList.Add(entry);
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		internal IEnumerable<T> DeserializeInstancesInternal<T>(int maxRows, bool callConstructor, XmlNameTable nameTable) {
			Debug.Assert(typeof(T).IsAssignableFrom(TypeInfo.InstanceType));
			DeserializerContext deserializerContext = new DeserializerContext(context, reader, callConstructor, nameTable);
			while (maxRows > 0) {
				if (!reader.Read()) {
					break;
				}
				if (typeInfo.SimpleConverter != null) {
					yield return (T)typeInfo.SimpleConverter.ProcessFromDb(deserializerContext, 0);
				} else {
					InstanceOrigin instanceOrigin;
					T instance = (T)CreateInstance(deserializerContext, out instanceOrigin);
					if (instanceOrigin != InstanceOrigin.ResultSet) {
						yield return instance;
					}
				}
				maxRows--;
			}
		}

		internal object DeserializeInternal(int maxRows, bool callConstructor, XmlNameTable nameTable) {
			if (maxRows < 0) {
				throw new ArgumentOutOfRangeException("maxRows");
			}
			if (typeInfo.IsCollection) {
				IList list = typeInfo.CreateList();
				foreach (object instance in DeserializeInstancesInternal<object>(maxRows, callConstructor, nameTable)) {
					list.Add(instance);
				}
				return typeInfo.FinalizeList(list);
			}
			if (!reader.Read()) {
				throw new InvalidOperationException("No more rows");
			}
			DeserializerContext deserializerContext = new DeserializerContext(context, reader, callConstructor, nameTable);
			if (typeInfo.SimpleConverter != null) {
				return typeInfo.SimpleConverter.ProcessFromDb(deserializerContext, 0);
			}
			InstanceOrigin instanceOrigin;
			return CreateInstance(deserializerContext, out instanceOrigin);
		}

		/// <summary>
		/// Dispose the <see cref="Reader"/> associated to this DbDeserializer.
		/// </summary>
		public void Dispose() {
			if (disposeReader) {
				reader.Dispose();
			}
		}
	}
}
