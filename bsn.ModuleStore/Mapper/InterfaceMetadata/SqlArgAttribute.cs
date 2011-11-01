using System;
using System.Data;
using System.Data.Common;
using System.Xml;
using System.Xml.XPath;

namespace bsn.ModuleStore.Mapper.InterfaceMetadata {
	/// <summary>
	/// The DbArgAttribute attribute is used to specify an explicit database argument binding.
	/// <br/><br/>
	/// Information which can be specified includes the <see cref="SqlNameAttribute.Name"/>, the <see cref="SqlArgAttribute.Type"/> information, the <see cref="SqlArgAttribute.Size"/> where applicable, and the <see cref="SqlArgAttribute.Nullable"/> definition.
	/// </summary>
	/// <remarks>XML arguments are supported if they are <see cref="XmlReader"/>, <see cref="XPathNavigator"/> or <see cref="IXPathNavigable"/></remarks>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
	public sealed class SqlArgAttribute: SqlColumnAttributeBase {
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlArgAttribute"/> class.
		/// </summary>
		public SqlArgAttribute(): this(null) {}

		/// <summary>
		/// Create a new DbArg attribute with the given binding <paramref name="name"/>.
		/// </summary>
		/// <param name="name">The name to be used as binding.</param>
		public SqlArgAttribute(string name): base(name) {
			Nullable = true;
			ListElementType = null;
			Type = null;
		}

		/// <summary>
		/// Gets or sets the type of the list element.
		/// </summary>
		/// <value>The type of the list element.</value>
		public Type ListElementType {
			get;
			set;
		}

		/// <summary>
		/// Specifies whether the database argument may be null or not. For value types, Nullable set implicitly depending whether they implement <see cref="System.Nullable{T}"/> or not.
		/// </summary>
		/// <seealso cref="DbParameter.IsNullable"/>
		public bool Nullable {
			get;
			set;
		}

		/// <summary>
		/// Specifies the size of the argument if applicable.
		/// </summary>
		/// <seealso cref="DbParameter.Size"/>
		public int Size {
			get;
			set;
		}

		/// <summary>
		/// Specifies the <see cref="SqlDbType"/> to be used.
		/// </summary>
		/// <seealso cref="SqlDbType"/>
		public SqlDbType? Type {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name of the user defined type.
		/// </summary>
		/// <value>The name of the user defined type.</value>
		public string UserDefinedTypeName {
			get;
			set;
		}
	}
}
