﻿// bsn ModuleStore database versioning
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
using System.Data;

namespace bsn.ModuleStore.Mapper {
	/// <summary>
	/// Base interface used for typed data readers. To create a typed data reader, create an interface which implements this interface as well, and use it with the <see cref="DataReaderProxy"/>.
	/// </summary>
	public interface ITypedDataReader: IDisposable {
		/// <summary>
		/// Return the current (non-typed) data record.
		/// </summary>
		/// <seealso cref="IDataRecord"/>
		IDataRecord Current {
			get;
		}

		/// <summary>
		/// Move the reader to the next resultset and add it to a typed data reader. Corresponds to <see cref="IDataReader.NextResult"/>.
		/// </summary>
		/// <remarks>Adter calling this method, the typed data reader is implicitly disposed because it transfers the ownership of the reader. Therefore, any calls afterwards (except to <see cref="IDisposable.Dispose"/>) will throw a <see cref="ObjectDisposedException"/>.</remarks>
		/// <typeparam name="T">The type of the typed data reader to create.</typeparam>
		/// <returns>If another resultset is available, the new typed data reader (which now owns the reader) is returned, otherwise null.</returns>
		T NextResult<T>() where T: ITypedDataReader;

		/// <summary>
		/// Move the reader to the next resultset. Corresponds to <see cref="IDataReader.NextResult"/>.
		/// </summary>
		/// <remarks>Adter calling this method, the typed data reader is implicitly disposed because it transfers the ownership of the reader. Therefore, any calls afterwards (except to <see cref="IDisposable.Dispose"/>) will throw a <see cref="ObjectDisposedException"/>.</remarks>
		/// <returns>If another resultset is available, the original <see cref="IDataReader"/> is returned and ownership is released, otherwise null.</returns>
		IDataReader NextResult();

		/// <summary>
		/// Move the typed data reader to the next record. Corresponds to <see cref="IDataReader.Read"/>.
		/// </summary>
		/// <returns>True if the next row could be read, false otherwise (no more rows).</returns>
		bool Read();
	}
}
