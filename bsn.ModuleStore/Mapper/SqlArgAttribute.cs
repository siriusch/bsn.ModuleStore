using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.XPath;

namespace bsn.ModuleStore.Mapper {
	/// <summary>
	/// The DbArgAttribute attribute is used to specify an explicit database argument binding.
	/// <br/><br/>
	/// Information which can be specified includes the <see cref="SqlNameAttribute.Name"/>, the <see cref="SqlArgAttribute.Type"/> information, the <see cref="SqlArgAttribute.Size"/> where applicable, and the <see cref="SqlArgAttribute.Nullable"/> definition.
	/// </summary>
	/// <remarks>XML arguments are supported if they are <see cref="XmlReader"/>, <see cref="XPathNavigator"/> or <see cref="IXPathNavigable"/></remarks>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
	public sealed class SqlArgAttribute: SqlColumnAttribute {
		private static readonly Dictionary<Type, SqlDbType> dbTypeMapping = new Dictionary<Type, SqlDbType> {
			                                                               		{typeof(long), SqlDbType.BigInt},
			                                                               		{typeof(byte[]), SqlDbType.VarBinary},
			                                                               		{typeof(bool), SqlDbType.Bit},
			                                                               		{typeof(char), SqlDbType.NChar},
			                                                               		{typeof(char[]), SqlDbType.NVarChar},
			                                                               		{typeof(string), SqlDbType.NVarChar},
			                                                               		{typeof(DateTime), SqlDbType.DateTime},
			                                                               		{typeof(DateTimeOffset), SqlDbType.DateTimeOffset}, // not supported in SQL 2005!
			                                                               		{typeof(decimal), SqlDbType.Decimal},
			                                                               		{typeof(float), SqlDbType.Real},
			                                                               		{typeof(double), SqlDbType.Float},
			                                                               		{typeof(int), SqlDbType.Int},
			                                                               		{typeof(short), SqlDbType.SmallInt},
			                                                               		{typeof(sbyte), SqlDbType.TinyInt},
			                                                               		{typeof(Guid), SqlDbType.UniqueIdentifier}
			                                                               };

		/// <summary>
		/// Get the matching <see cref="DbType"/> for the given type. For value types, <paramref name="nullable"/> information is also returned.
		/// </summary>
		/// <param name="type">The .NET type to match.</param>
		/// <param name="nullable">Only set when <paramref name="type"/> is a value type; true is the type is <see cref="System.Nullable{T}"/>.</param>
		/// <returns>The <see cref="DbType"/> best matching the given .NET type, or <see cref="DbType.Object"/> otherwise.</returns>
		public static SqlDbType GetTypeMapping(Type type, out bool? nullable) {
			nullable = null;
			if(type != null) {
				if (type.IsByRef && type.HasElementType) {
					type = type.GetElementType();
				}
				if(SqlDeserializer.IsNullableType(type)) {
					type = type.GetGenericArguments()[0];
					nullable = true;
				} else if (type.IsValueType) {
					nullable = false;
				}
				SqlDbType result;
				if (dbTypeMapping.TryGetValue(type, out result)) {
					return result;
				}
				if (SqlDeserializer.IsXmlType(type)) {
					return SqlDbType.Xml;
				}
			}
			return SqlDbType.Udt;
		}

		private bool nullable = true;
		private int size;
		private SqlDbType type = SqlDbType.Udt;

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlArgAttribute"/> class.
		/// </summary>
		public SqlArgAttribute(): this(null) {}

		/// <summary>
		/// Create a new DbArg attribute with the given binding <paramref name="name"/>.
		/// </summary>
		/// <param name="name">The name to be used as binding.</param>
		public SqlArgAttribute(string name): base(name) {}

		/// <summary>
		/// True whenever a specific <see cref="DbType"/> has been set.
		/// </summary>
		public bool HasType {
			get {
				return type != SqlDbType.Udt;
			}
		}

		/// <summary>
		/// Specifies whether the database argument may be null or not. For value types, Nullable set implicitly depending whether they implement <see cref="System.Nullable{T}"/> or not.
		/// </summary>
		/// <seealso cref="SqlParameter.IsNullable"/>
		public bool Nullable {
			get {
				return nullable;
			}
			set {
				nullable = value;
			}
		}

		/// <summary>
		/// Specifies the size of the argument if applicable.
		/// </summary>
		/// <seealso cref="SqlParameter.Size"/>
		public int Size {
			get {
				return size;
			}
			set {
				size = value;
			}
		}

		/// <summary>
		/// Specifies the <see cref="DbType"/> to be used.
		/// </summary>
		/// <seealso cref="SqlParameter.SqlDbType"/>
		public SqlDbType Type {
			get {
				return type;
			}
			set {
				type = value;
			}
		}
	}
}