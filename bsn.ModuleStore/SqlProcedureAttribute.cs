using System;
using System.Data.Common;

namespace bsn.ModuleStore.Mapper {
	/// The DbProcAttribute attribute is used to specify an explicit database procedure binding on an interface.
	/// <br/><br/>
	/// Information which can be specified includes the <see cref="SqlColumnAttribute.Name"/>, <see cref="Timeout"/>, <see cref="UseReturnValue"/>, <see cref="DeserializeRowLimit"/>, <see cref="DeserializeReturnNullOnEmptyReader"/> and <see cref="DeserializeCallConstructor"/>.
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class SqlProcedureAttribute: SqlSetupScriptAttributeBase {
		private bool deserializeCallConstructor;
		private bool deserializeReturnNullOnEmptyReader;
		private int deserializeRowLimit = int.MaxValue;
		private int timeout;
		private SqlReturnValue useReturnValue = SqlReturnValue.Auto;

		public SqlProcedureAttribute(string embeddedResourceName): base(null, embeddedResourceName) {}

		/// <summary>
		/// If true, the default constructor will be called instead of creating empty instances. For best performance, leave this setting on false. This corresponds to the parameter passed to <see cref="SqlDeserializer{T}"/> constructor.
		/// </summary>
		public bool DeserializeCallConstructor {
			get {
				return deserializeCallConstructor;
			}
			set {
				deserializeCallConstructor = value;
			}
		}

		/// <summary>
		/// By default, if a single object is to be automatically deserialized (not a list) but no row is found, an exception will be thrown. You can use this property to change this behaviour for reference types.
		/// </summary>
		public bool DeserializeReturnNullOnEmptyReader {
			get {
				return deserializeReturnNullOnEmptyReader;
			}
			set {
				deserializeReturnNullOnEmptyReader = value;
			}
		}

		/// <summary>
		/// Controls the number of rows to deserialize when using the automatic Deserializer. This corresponds to the parameter passed to <see cref="SqlDeserializer{T}"/>.<see cref="SqlDeserializer{T}.Deserialize(int)"/>.
		/// </summary>
		public int DeserializeRowLimit {
			get {
				return deserializeRowLimit;
			}
			set {
				deserializeRowLimit = value;
			}
		}

		/// <summary>
		/// Controls the <see cref="DbCommand.CommandTimeout"/> to set on the <see cref="DbCommand"/>.
		/// </summary>
		public int Timeout {
			get {
				return timeout;
			}
			set {
				timeout = value;
			}
		}

		/// <summary>
		/// Controls the handling of the return value. If set to <see cref="SqlReturnValue.Auto"/>, it uses the SP return value for <see cref="int"/> and a scalar execution for all other data types.
		/// </summary>
		/// <seealso cref="SqlReturnValue"/>
		public SqlReturnValue UseReturnValue {
			get {
				return useReturnValue;
			}
			set {
				useReturnValue = value;
			}
		}
	}
}