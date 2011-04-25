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
using System.ComponentModel;
using System.Data.Common;

using bsn.ModuleStore.Mapper.Serialization;

namespace bsn.ModuleStore.Mapper {
	/// The <c>SqlProcedureAttribute</c> attribute is used to specify an explicit database procedure binding on an interface.
	/// <br/><br/>
	/// Information which can be specified includes the <see cref="SqlColumnAttribute.Name"/>, <see cref="Timeout"/>, <see cref="UseReturnValue"/>, <see cref="DeserializeRowLimit"/>, <see cref="DeserializeReturnNullOnEmptyReader"/> and <see cref="DeserializeCallConstructor"/>.
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class SqlProcedureAttribute: SqlSetupScriptAttributeBase {
		private bool deserializeCallConstructor;
		private bool deserializeReturnNullOnEmptyReader;
		private int deserializeRowLimit = int.MaxValue;
		private bool executeFirstCommentBeforeInvocation;
		private bool requireTransaction;
		private int timeout;
		private SqlReturnValue useReturnValue = SqlReturnValue.Auto;

		public SqlProcedureAttribute([Localizable(false)] string embeddedResourceName): base(null, embeddedResourceName) {}

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
		/// By default, if a single object is to be automatically deserialized (not a list) but no row is found, an exception will be thrown. You can use this property to change this behavior for reference types.
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
		/// Sometimes specific statements (such as creating temporary tables) need to be executed before the SP is called. These statements can be put in a leading comment.
		/// </summary>
		public bool ExecuteFirstCommentBeforeInvocation {
			get {
				return executeFirstCommentBeforeInvocation;
			}
			set {
				executeFirstCommentBeforeInvocation = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether a transaction is required to execute this stored procedure. The transaction will not be created, but checked only.
		/// </summary>
		public bool RequireTransaction {
			get {
				return requireTransaction;
			}
			set {
				requireTransaction = value;
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
