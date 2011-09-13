using System;
using System.Data.Common;

using bsn.ModuleStore.Mapper.Serialization;

namespace bsn.ModuleStore.Mapper.InterfaceMetadata {
	/// The SqlProcAttribute attribute is used to specify an explicit database procedure binding on an interface.
	/// <br/><br/>
	/// Information which can be specified includes the <see cref="Timeout"/>, <see cref="UseReturnValue"/>, <see cref="DeserializeRowLimit"/>, <see cref="DeserializeReturnNullOnEmptyReader"/> and <see cref="DeserializeCallConstructor"/>.
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class SqlProcAttribute : Attribute {
		internal SqlProcAttribute CloneWithName(string newName)
		{
			SqlProcAttribute result = (SqlProcAttribute)MemberwiseClone();
			result.Name = newName;
			return result;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlProcAttribute"/> class.
		/// </summary>
		public SqlProcAttribute(): this(null, string.Empty) {}

		/// <summary>
		/// Create a new DbProc attribute with the given binding <paramref name="name"/>.
		/// </summary>
		/// <param name="name">The name to be used as binding.</param>
		public SqlProcAttribute(string name): this(name, string.Empty) {}

		/// <summary>
		/// Create a new DbProc attribute with the given binding <paramref name="name"/>.
		/// </summary>
		/// <param name="name">The name to be used as binding.</param>
		/// <param name="schemaName"></param>
		public SqlProcAttribute(string name, string schemaName) {
			if (string.IsNullOrEmpty(name)) {
				throw new ArgumentNullException("name");
			}
			Name = name;
			SchemaName = string.IsNullOrEmpty(schemaName) ? "dbo" : schemaName.Replace("[","").Replace("]","");
			DeserializeRowLimit = int.MaxValue;
			UseReturnValue = SqlReturnValue.Auto;
		}

		/// <summary>
		/// If true, the default constructor will be called instead of creating empty instances. For best performance, leave this setting on false. This corresponds to the parameter passed to <see cref="SqlDeserializer"/> constructor.
		/// </summary>
		public bool DeserializeCallConstructor {
			get;
			set;
		}

		/// <summary>
		/// By default, if a single object is to be automatically deserialized (not a list) but no row is found, an exception will be thrown. You can use this property to change this behaviour for reference types.
		/// </summary>
		public bool DeserializeReturnNullOnEmptyReader {
			get;
			set;
		}

		/// <summary>
		/// Controls the number of rows to deserialize when using the automatic Deserializer. This corresponds to the parameter passed to <see cref="SqlDeserializer{T}"/>.<see cref="SqlDeserializer{T}.Deserialize(int)"/>.
		/// </summary>
		public int DeserializeRowLimit {
			get;
			set;
		}

		/// <summary>
		/// The name for the database binding.
		/// </summary>
		public string Name
		{
			get;
			private set;
		}

		/// <summary>
		/// The name for the database binding.
		/// </summary>
		public string SchemaName
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the name of the procedure.
		/// </summary>
		/// <value>The name of the procedure.</value>
		public string ProcedureName {
			get {
				return string.IsNullOrEmpty(SchemaName) ? '['+Name+']' : '['+SchemaName+"].["+Name+']';
			}
		}

		/// <summary>
		/// Controls the <see cref="DbCommand.CommandTimeout"/> to set on the <see cref="DbCommand"/>.
		/// </summary>
		public int Timeout {
			get;
			set;
		}

		/// <summary>
		/// Controls the handling of the return value. If set to <see cref="SqlReturnValue.Auto"/>, it uses the SP return value for <see cref="int"/> and a scalar execution for all other data types.
		/// </summary>
		/// <seealso cref="SqlReturnValue"/>
		public SqlReturnValue UseReturnValue {
			get;
			set;
		}
	}
}
