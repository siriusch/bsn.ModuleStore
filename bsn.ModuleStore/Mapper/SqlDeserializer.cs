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
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace bsn.ModuleStore.Mapper {
	/// <summary>
	/// The DbDeserializer generic class allows to create an efficient database Deserializer for classes and structs which transforms database rows into object instances.
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
		private readonly bool callConstructor;

		/// <summary>
		/// Creates a new database Deserializer which will read from the given reader.
		/// </summary>
		/// <remarks>Disposing the DbDeserializer will also dispose the reader. Also, you must not continue to use the Deserializer afterwards if you call <see cref="SqlDataReader.NextResult"/>.</remarks>
		/// <param name="reader">The SqlDataReader to read data from. The DbDeserializer will take ownership of the reader.</param>
		/// <param name="callConstructor">If true, the normal default constructor is called instead of creating empty instances. Empty instances, however, are much faster.</param>
		public SqlDeserializer(SqlDataReader reader, bool callConstructor): base(reader, typeof(T)) {
			this.callConstructor = callConstructor;
		}

		/// <summary>
		/// Creates a new database Deserializer which will read from the given reader. Object instances will be created without calling the condtructor.
		/// </summary>
		/// <remarks>Disposing the DbDeserializer will also dispose the reader. Also, you must not continue to use the Deserializer afterwards if you call <see cref="SqlDataReader.NextResult"/>.</remarks>
		/// <param name="reader">The SqlDataReader to read data from. The DbDeserializer will take ownership of the reader.</param>
		public SqlDeserializer(SqlDataReader reader): this(reader, false) {}

		/// <summary>
		/// Deserialize one row into a new instance, or all available rows for lists. If <typeparamref name="T"/> is not a list (supported types for lists: <see cref="List{T}"/>, <see cref="IList{T}"/>, <see cref="ICollection{T}"/> or <typeparamref name="T"/>[].
		/// </summary>
		/// <returns>An instance of <typeparamref name="T"/></returns>
		public T Deserialize(IInstanceProvider provider) {
			return Deserialize(provider, int.MaxValue);
		}

		/// <summary>
		/// Deserialize one row into a new instance, or all available rows for lists. If <typeparamref name="T"/> is not a list (supported types for lists: <see cref="List{T}"/>, <see cref="IList{T}"/>, <see cref="ICollection{T}"/> or <typeparamref name="T"/>[].
		/// </summary>
		/// <param name="provider">The provider.</param>
		/// <param name="maxRows">The maximal number of rows to read (for lists).</param>
		/// <returns>An instance of <typeparamref name="T"/></returns>
		public T Deserialize(IInstanceProvider provider, int maxRows) {
			return (T)DeserializeInternal(maxRows, provider, callConstructor, null);
		}

		/// <summary>
		/// Deserialize all rows as enumeration of instances.
		/// </summary>
		/// <returns>An enumeration of instances of <typeparamref name="T"/></returns>
		public IEnumerable<T> DeserializeInstances(IInstanceProvider provider) {
			return DeserializeInstances(provider, Int32.MaxValue);
		}

		/// <summary>
		/// Deserialize the given number of rows as enumeration of instances.
		/// </summary>
		/// <param name="provider">The provider.</param>
		/// <param name="maxRows">The maximal number of rows to read.</param>
		/// <returns>
		/// An enumeration of instances of <typeparamref name="T"/>
		/// </returns>
		public IEnumerable<T> DeserializeInstances(IInstanceProvider provider, int maxRows) {
			if (TypeInfo.IsCollection) {
				throw new NotSupportedException("Collections cannot be deserialized as instances");
			}
			return DeserializeInstancesInternal<T>(maxRows, provider, callConstructor, null);
		}
	}

	/// <summary>
	/// The DbDeserializer class is the base class for the generic <see cref="SqlDeserializer{T}"/> class. Please use the generic version, this class is for internal use only by <see cref="SqlCallProxy"/>.
	/// </summary>
	/// <seealso cref="SqlDeserializer{T}"/>
	public class SqlDeserializer: IDisposable {
		protected internal class DeserializerContext: IDisposable {
			private readonly IDictionary<SqlDeserializer, object[]> buffers = new Dictionary<SqlDeserializer, object[]>();
			private readonly bool callConstructor;
			private readonly Guid contextId = Guid.NewGuid();
			private readonly SqlDataReader dataReader;
			private readonly IInstanceProvider provider;
			private readonly IDictionary<string, object> state;
			private readonly IDeserializationStateProvider stateProvider;
			private XmlNameTable nameTable;
			private XmlDocument xmlDocument;

			public DeserializerContext(SqlDataReader dataReader, IInstanceProvider provider, XmlNameTable nameTable) {
				this.provider = provider;
				this.dataReader = dataReader;
				this.nameTable = nameTable;
			}

			public DeserializerContext(SqlDeserializer deserializer, IInstanceProvider provider, bool callConstructor, XmlNameTable nameTable) {
				this.provider = provider;
				this.callConstructor = callConstructor;
				this.nameTable = nameTable;
				dataReader = deserializer.reader;
				stateProvider = provider as IDeserializationStateProvider;
				if (stateProvider != null) {
					state = new SortedDictionary<string, object>(StringComparer.Ordinal);
					stateProvider.BeginDeserialize(state);
				}
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

			public object GetInstance(Type instanceType, object identity) {
				if (provider != null) {
					object result;
					if (provider.TryGetInstance(instanceType, identity, out result)) {
						return result;
					}
				}
				return callConstructor ? Activator.CreateInstance(instanceType, true) : FormatterServices.GetUninitializedObject(instanceType);
			}

			internal object[] GetBuffer(SqlDeserializer deserializer) {
				object[] result;
				if (!buffers.TryGetValue(deserializer, out result)) {
					result = new object[deserializer.TypeInfo.Mapping.Members.Length];
					buffers.Add(deserializer, result);
				}
				return result;
			}

			internal void NotifyInstancePopulated(object instance) {
				if (stateProvider != null) {
					stateProvider.InstanceDeserialized(state, instance);
				}
			}

			public void Dispose() {
				if (stateProvider != null) {
					stateProvider.EndDeserialize(state);
				}
			}
		}

		internal class NestedMemberConverter: MemberConverter {
			public NestedMemberConverter(Type type, int memberIndex): base(type, memberIndex) {}

			public override object Process(DeserializerContext context, int column) {
				throw new NotSupportedException("Nested members need to be deserialized directly via SqlDeserializer");
			}
		}

		internal class TypeMapping {
			private static readonly Dictionary<Type, TypeMapping> mappings = new Dictionary<Type, TypeMapping>();

			public static TypeMapping Get(Type type) {
				if (type == null) {
					throw new ArgumentNullException("type");
				}
				TypeMapping result;
				lock (mappings) {
					if (!mappings.TryGetValue(type, out result)) {
						result = new TypeMapping(type);
						mappings.Add(type, result);
					}
				}
				return result;
			}

			private static IEnumerable<FieldInfo> GetAllFields(Type type) {
				while (type != null) {
					foreach (FieldInfo field in type.GetFields(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.DeclaredOnly)) {
						yield return field;
					}
					type = type.BaseType;
				}
			}

			private readonly List<KeyValuePair<string, KeyValuePair<bool, MemberConverter>>> converters;
			private readonly MemberInfo[] members;

			private TypeMapping(Type type) {
				if (type == null) {
					throw new ArgumentNullException("type");
				}
				converters = new List<KeyValuePair<string, KeyValuePair<bool, MemberConverter>>>();
				List<MemberInfo> memberInfos = new List<MemberInfo>();
				if (!(type.IsPrimitive || type.IsInterface || (typeof(string) == type))) {
					bool hasIdentity = false;
					foreach (FieldInfo field in GetAllFields(type)) {
						SqlColumnAttribute columnAttribute = SqlColumnAttribute.GetColumnAttribute(field, false);
						if (columnAttribute != null) {
							converters.Add(new KeyValuePair<string, KeyValuePair<bool, MemberConverter>>(columnAttribute.Name, new KeyValuePair<bool, MemberConverter>((!hasIdentity) && (hasIdentity |= columnAttribute.Identity), MemberConverter.Get(field.FieldType, memberInfos.Count, columnAttribute.DateTimeKind))));
							memberInfos.Add(field);
						} else if (field.IsDefined(typeof(SqlDeserializeAttribute), true)) {
							converters.Add(new KeyValuePair<string, KeyValuePair<bool, MemberConverter>>(null, new KeyValuePair<bool, MemberConverter>(false, new NestedMemberConverter(field.FieldType, memberInfos.Count))));
							memberInfos.Add(field);
						}
					}
				}
				members = memberInfos.ToArray();
			}

			public ICollection<KeyValuePair<string, KeyValuePair<bool, MemberConverter>>> Converters {
				get {
					return converters;
				}
			}

			public MemberInfo[] Members {
				get {
					return members;
				}
			}
		}

		/// <summary>
		/// Returns true if the type given is a known XML type.
		/// </summary>
		/// <param name="type">Te type to be checked.</param>
		/// <returns>True if the type is recognized as XML type.</returns>
		public static bool IsXmlType(Type type) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			return typeof(XContainer).IsAssignableFrom(type) || typeof(XmlReader).IsAssignableFrom(type) || typeof(XPathNavigator).IsAssignableFrom(type) || typeof(IXPathNavigable).IsAssignableFrom(type);
		}

		private readonly SortedList<int, KeyValuePair<bool, MemberConverter>> columnConverters;
		private readonly Dictionary<NestedMemberConverter, SqlDeserializer> nestedDeserializers;
		private readonly SqlDataReader reader;
		private readonly SqlDeserializerTypeInfo typeInfo;
		private bool disposeReader = true;

		internal SqlDeserializer(SqlDataReader reader, Type type) {
			if (reader == null) {
				throw new ArgumentNullException("reader");
			}
			if (type.IsAbstract || type.IsInterface || type.IsPrimitive || type.IsPointer || typeof(ResultSet).IsAssignableFrom(type)) {
				throw new NotSupportedException("Deserialization only supports normal classes and structs.");
			}
			this.reader = reader;
			typeInfo = SqlDeserializerTypeInfo.Get(type);
			foreach (KeyValuePair<string, KeyValuePair<bool, MemberConverter>> column in typeInfo.Mapping.Converters) {
				if (column.Key == null) {
					if (nestedDeserializers == null) {
						nestedDeserializers = new Dictionary<NestedMemberConverter, SqlDeserializer>();
					}
#warning Cyclic nested members would cause a stack overflow here
					nestedDeserializers.Add((NestedMemberConverter)column.Value.Value, new SqlDeserializer(reader, column.Value.Value.Type));
				} else {
					if (columnConverters == null) {
						columnConverters = new SortedList<int, KeyValuePair<bool, MemberConverter>>(typeInfo.Mapping.Converters.Count);
					}
					columnConverters.Add(reader.GetOrdinal(column.Key), column.Value);
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether to dispose the reader when the deserializer is disposed.
		/// </summary>
		/// <value><c>true</c> if the reader shall be disposed; otherwise, <c>false</c>.</value>
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
		public SqlDeserializerTypeInfo TypeInfo {
			get {
				return typeInfo;
			}
		}

		protected object CreateInstance(DeserializerContext context) {
			Debug.Assert(context != null);
			object identity = null;
			object[] buffer = context.GetBuffer(this);
			if (columnConverters != null) {
				foreach (KeyValuePair<int, KeyValuePair<bool, MemberConverter>> converter in columnConverters) {
					object value = converter.Value.Value.Process(context, converter.Key);
					if (converter.Value.Key) {
						identity = value;
					}
					buffer[converter.Value.Value.MemberIndex] = value;
				}
			}
			if (nestedDeserializers != null) {
				foreach (KeyValuePair<NestedMemberConverter, SqlDeserializer> nested in nestedDeserializers) {
					buffer[nested.Key.MemberIndex] = nested.Value.CreateInstance(context);
				}
			}
			object result = context.GetInstance(typeInfo.InstanceType, identity);
			if (result != null) {
				FormatterServices.PopulateObjectMembers(result, typeInfo.Mapping.Members, buffer);
				context.NotifyInstancePopulated(result);
				if (typeInfo.RequiresNotification) {
					((ISqlDeserializationHook)result).AfterDeserialization();
				}
			}
			return result;
		}

		internal IEnumerable<T> DeserializeInstancesInternal<T>(int maxRows, IInstanceProvider provider, bool callConstructor, XmlNameTable nameTable) {
			Debug.Assert(typeof(T).IsAssignableFrom(TypeInfo.InstanceType));
			using (DeserializerContext context = new DeserializerContext(this, provider, callConstructor, nameTable)) {
				while (maxRows > 0) {
					if (!reader.Read()) {
						break;
					}
					yield return (T)CreateInstance(context);
					maxRows--;
				}
			}
		}

		internal object DeserializeInternal(int maxRows, IInstanceProvider provider, bool callConstructor, XmlNameTable nameTable) {
			if (maxRows < 0) {
				throw new ArgumentOutOfRangeException("maxRows");
			}
			if (typeInfo.IsCollection) {
				IList list = typeInfo.CreateList();
				foreach (object instance in DeserializeInstancesInternal<object>(maxRows, provider, callConstructor, nameTable)) {
					list.Add(instance);
				}
				return typeInfo.FinalizeList(list);
			}
			using (DeserializerContext deserializerContext = new DeserializerContext(this, provider, callConstructor, nameTable)) {
				if (!reader.Read()) {
					throw new InvalidOperationException("No more rows");
				}
				return CreateInstance(deserializerContext);
			}
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