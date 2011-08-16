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

using bsn.ModuleStore.Mapper.Serialization;

namespace bsn.ModuleStore.Mapper {
	public interface ICallDeserializationInfo {
		/// <summary>
		/// If true, the default constructor will be called instead of creating empty instances. For best performance, leave this setting on false. This corresponds to the parameter passed to <see cref="SqlDeserializer{T}"/> constructor.
		/// </summary>
		bool DeserializeCallConstructor {
			get;
		}

		/// <summary>
		/// By default, if a single object is to be automatically deserialized (not a list) but no row is found, an exception will be thrown. You can use this property to change this behavior for reference types.
		/// </summary>
		bool DeserializeReturnNullOnEmptyReader {
			get;
		}

		/// <summary>
		/// Controls the number of rows to deserialize when using the automatic Deserializer. This corresponds to the parameter passed to <see cref="SqlDeserializer{T}"/>.<see cref="SqlDeserializer{T}.Deserialize(int)"/>.
		/// </summary>
		int DeserializeRowLimit {
			get;
		}

		/// <summary>
		/// Gets or sets a value indicating whether a transaction is required to execute this stored procedure. The transaction will not be created, but checked only.
		/// </summary>
		bool RequireTransaction {
			get;
		}
	}
}
