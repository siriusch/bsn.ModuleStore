using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
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
		/// <remarks>Disposing the DbDeserializer will also dispose the reader. Also, you must not continue to use the Deserializer afterwards if you call <see cref="IDataReader.NextResult"/>.</remarks>
		/// <param name="reader">The IDataReader to read data from. The DbDeserializer will take ownership of the reader.</param>
		/// <param name="callConstructor">If true, the normal default constructor is called instead of creating empty instances. Empty instances, however, are much faster.</param>
		public SqlDeserializer(IDataReader reader, bool callConstructor): base(reader, typeof(T)) {
			this.callConstructor = callConstructor;
		}

		/// <summary>
		/// Creates a new database Deserializer which will read from the given reader. Object instances will be created without calling the condtructor.
		/// </summary>
		/// <remarks>Disposing the DbDeserializer will also dispose the reader. Also, you must not continue to use the Deserializer afterwards if you call <see cref="IDataReader.NextResult"/>.</remarks>
		/// <param name="reader">The IDataReader to read data from. The DbDeserializer will take ownership of the reader.</param>
		public SqlDeserializer(IDataReader reader): this(reader, false) {}

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
			return (T)DeserializeInternal(maxRows, false, callConstructor, null);
		}
	}

	/// <summary>
	/// The DbDeserializer class is the base class for the generic <see cref="SqlDeserializer{T}"/> class. Please use the generic version, this class is for internal use only by <see cref="SqlCallProxy"/>.
	/// </summary>
	/// <seealso cref="SqlDeserializer{T}"/>
	public class SqlDeserializer: IDisposable {
		internal class DeserializerContext {
			public readonly object[] Buffer;
			public readonly bool CallConstructor;
			public readonly IDataReader DataReader;
			public readonly SqlDeserializer Deserializer;
			private XmlNameTable nameTable;
			private XmlDocument xmlDocument;

			public DeserializerContext(IDataReader dataReader, XmlNameTable nameTable) {
				DataReader = dataReader;
				this.nameTable = nameTable;
			}

			public DeserializerContext(SqlDeserializer deserializer, bool callConstructor, XmlNameTable nameTable) {
				CallConstructor = callConstructor;
				Deserializer = deserializer;
				this.nameTable = nameTable;
				DataReader = deserializer.reader;
				Buffer = new object[deserializer.typeInfo.Mapping.Members.Length];
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
		}

		internal class NestedMemberConverter: MemberConverter {
			public NestedMemberConverter(Type type, int memberIndex): base(type, memberIndex) {}

			public override object Process(DeserializerContext context, int column) {
				return context.Deserializer.GetNestedDeserializer(this).DeserializeInternal(1, true, context.CallConstructor, context.NameTable);
			}
		}

		internal class TypeInfo {
			private static readonly Dictionary<Type, TypeInfo> infos = new Dictionary<Type, TypeInfo>();

			public static TypeInfo Get(Type type) {
				TypeInfo result;
				lock (infos) {
					if (!infos.TryGetValue(type, out result)) {
						result = new TypeInfo(type);
						infos.Add(type, result);
					}
				}
				return result;
			}

			private readonly Type instanceType;
			private readonly bool isXmlType;
			private readonly MethodInfo listToArray;
			private readonly Type listType;
			private readonly TypeMapping mapping;
			private readonly bool requiresNotification;
			private readonly MemberConverter simpleConverter;
			private readonly Type type;

			private TypeInfo(Type type) {
				this.type = type;
				if (type.IsArray) {
					if (type.GetArrayRank() != 1) {
						throw new NotSupportedException("Only arrays with one dimension are supported by the DbDeserializer");
					}
					instanceType = type.GetElementType();
					Debug.Assert(instanceType != null);
				} else if (type.IsGenericType && ((type.GetGenericTypeDefinition() == typeof(ICollection<>)) || (type.GetGenericTypeDefinition() == typeof(IList<>)) || (type.GetGenericTypeDefinition() == typeof(List<>)))) {
					instanceType = type.GetGenericArguments()[0];
				} else {
					instanceType = type;
				}
				if (instanceType.IsArray) {
					throw new NotSupportedException("Nested arrays cannot be deserialized by the DbDeserializer");
				}
				requiresNotification = typeof(ISqlDeserializationHook).IsAssignableFrom(instanceType);
				mapping = TypeMapping.Get(instanceType);
				if (IsCollection) {
					listType = typeof(List<>).MakeGenericType(instanceType);
					if (type.IsArray) {
						listToArray = listType.GetMethod("ToArray");
					}
				}
				isXmlType = IsXmlType(instanceType);
				if (isXmlType || IsNullableType(instanceType) || instanceType.IsPrimitive || SqlCallProcedureInfo.IsNativeType(instanceType)) {
					simpleConverter = MemberConverter.Get(instanceType, 0);
				}
			}

			public Type InstanceType {
				get {
					return instanceType;
				}
			}

			public bool IsCollection {
				get {
					return type != instanceType;
				}
			}

			public bool IsXmlInstanceType {
				get {
					return isXmlType;
				}
			}

			public Type ListType {
				get {
					return listType;
				}
			}

			public bool RequiresNotification {
				get {
					return requiresNotification;
				}
			}

			public MemberConverter SimpleConverter {
				get {
					return simpleConverter;
				}
			}

			public Type Type {
				get {
					return type;
				}
			}

			internal TypeMapping Mapping {
				get {
					return mapping;
				}
			}

			public IList CreateList() {
				return (IList)Activator.CreateInstance(listType);
			}

			public object FinalizeList(IList list) {
				if ((list != null) && (listToArray != null)) {
					return listToArray.Invoke(list, null);
				}
				return list;
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

			private readonly List<KeyValuePair<string, MemberConverter>> converters;
			private readonly MemberInfo[] members;

			private TypeMapping(Type type) {
				if (type == null) {
					throw new ArgumentNullException("type");
				}
				converters = new List<KeyValuePair<string, MemberConverter>>();
				List<MemberInfo> memberInfos = new List<MemberInfo>();
				if (!(type.IsPrimitive || type.IsInterface || (typeof(string) == type))) {
					foreach (FieldInfo field in GetAllFields(type)) {
						SqlColumnAttribute columnAttribute = SqlColumnAttribute.GetColumnAttribute(field, false);
						if (columnAttribute != null) {
							converters.Add(new KeyValuePair<string, MemberConverter>(columnAttribute.Name, MemberConverter.Get(field.FieldType, memberInfos.Count)));
							memberInfos.Add(field);
						} else if (field.GetCustomAttributes(typeof(SqlDeserializeAttribute), true).Length > 0) {
							converters.Add(new KeyValuePair<string, MemberConverter>(null, new NestedMemberConverter(field.FieldType, memberInfos.Count)));
							memberInfos.Add(field);
						}
					}
				}
				members = memberInfos.ToArray();
			}

			public ICollection<KeyValuePair<string, MemberConverter>> Converters {
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
		/// Returns true if the type given is a nullable type (that is, <see cref="Nullable{T}"/>).
		/// </summary>
		/// <param name="type">The type to be checked.</param>
		/// <returns>True if the type is recognized as nullable type.</returns>
		public static bool IsNullableType(Type type) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>));
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
			return typeof(XmlReader).IsAssignableFrom(type) || typeof(XPathNavigator).IsAssignableFrom(type) || typeof(IXPathNavigable).IsAssignableFrom(type);
		}

		private readonly SortedList<int, MemberConverter> columnConverters;
		private readonly List<NestedMemberConverter> nestedConverters;
		private readonly IDataReader reader;
		private readonly TypeInfo typeInfo;
		private Dictionary<MemberConverter, SqlDeserializer> nestedDeserializers;

		internal SqlDeserializer(IDataReader reader, Type type) {
			if (reader == null) {
				throw new ArgumentNullException("reader");
			}
			if (type.IsAbstract || type.IsInterface || type.IsPrimitive || type.IsPointer) {
				throw new NotSupportedException("Deserialization only supports normal classes and structs.");
			}
			this.reader = reader;
			typeInfo = TypeInfo.Get(type);
			foreach (KeyValuePair<string, MemberConverter> column in typeInfo.Mapping.Converters) {
				if (column.Key == null) {
					if (nestedConverters == null) {
						nestedConverters = new List<NestedMemberConverter>();
					}
					nestedConverters.Add((NestedMemberConverter)column.Value);
				} else {
					if (columnConverters == null) {
						columnConverters = new SortedList<int, MemberConverter>(typeInfo.Mapping.Converters.Count);
					}
					columnConverters.Add(reader.GetOrdinal(column.Key), column.Value);
				}
			}
		}

		/// <summary>
		/// The <see cref="IDataReader"/> which was passed into the constructor.
		/// </summary>
		/// <remarks>As long as the DbDeserializer is in use, do not alter the state of the Reader. Especially, do not call <see cref="IDataReader.NextResult"/>.</remarks>
		public IDataReader Reader {
			get {
				return reader;
			}
		}

		internal TypeInfo Info {
			get {
				return typeInfo;
			}
		}

		internal object DeserializeInternal(int maxRows, bool skipFirstRead, bool callConstructor, XmlNameTable nameTable) {
			if (maxRows < 0) {
				throw new ArgumentOutOfRangeException("maxRows");
			}
			DeserializerContext context = new DeserializerContext(this, callConstructor, nameTable);
			if (typeInfo.IsCollection) {
				IList list = typeInfo.CreateList();
				while (maxRows > 0) {
					if (skipFirstRead) {
						skipFirstRead = false;
					} else {
						if (!reader.Read()) {
							break;
						}
					}
					list.Add(CreateInstance(context));
					maxRows--;
				}
				return typeInfo.FinalizeList(list);
			}
			if (!skipFirstRead) {
				if (!reader.Read()) {
					throw new InvalidOperationException("No more rows");
				}
			}
			return CreateInstance(context);
		}

		internal SqlDeserializer GetNestedDeserializer(MemberConverter memberConverter) {
			if (nestedDeserializers == null) {
				nestedDeserializers = new Dictionary<MemberConverter, SqlDeserializer>();
			}
			SqlDeserializer result;
			if (!nestedDeserializers.TryGetValue(memberConverter, out result)) {
				result = new SqlDeserializer(reader, memberConverter.Type);
				nestedDeserializers.Add(memberConverter, result);
			}
			return result;
		}

		private object CreateInstance(DeserializerContext context) {
			if (columnConverters != null) {
				foreach (KeyValuePair<int, MemberConverter> converter in columnConverters) {
					context.Buffer[converter.Value.MemberIndex] = converter.Value.Process(context, converter.Key);
				}
			}
			if (nestedConverters != null) {
				foreach (NestedMemberConverter converter in nestedConverters) {
					context.Buffer[converter.MemberIndex] = converter.Process(context, -1);
				}
			}
			object result = (context.CallConstructor) ? Activator.CreateInstance(typeInfo.InstanceType, true) : FormatterServices.GetUninitializedObject(typeInfo.InstanceType);
			FormatterServices.PopulateObjectMembers(result, typeInfo.Mapping.Members, context.Buffer);
			if (typeInfo.RequiresNotification) {
				((ISqlDeserializationHook)result).AfterDeserialization();
			}
			return result;
		}

		/// <summary>
		/// Dispose the <see cref="Reader"/> associated to this DbDeserializer.
		/// </summary>
		public void Dispose() {
			reader.Dispose();
		}
	}
}