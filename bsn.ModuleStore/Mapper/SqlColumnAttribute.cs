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

using bsn.ModuleStore.Mapper.Serialization;

namespace bsn.ModuleStore.Mapper {
	/// <summary>
	/// The <see cref="SqlColumnAttribute"/> is used to change the binding name on <see cref="ITypedDataReader"/> interfaces, or to specify the fields to be deserialized when the <see cref="SqlDeserializer"/> is used.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field|AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class SqlColumnAttribute: SqlColumnAttributeBase {
		private bool notNull;

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlColumnAttribute"/> class, specifying a column name for the mapping.
		/// </summary>
		/// <param name="name">The SQL column name to be used for mapping this member</param>
		public SqlColumnAttribute(string name): base(name) {}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlColumnAttribute"/> class.
		/// </summary>
		public SqlColumnAttribute(): this(null) {}

		/// <summary>
		/// Determine whether a column is allowed to be null
		/// </summary>
		public bool NotNull {
			get {
				return notNull;
			}
			set {
				notNull = value;
			}
		}
	}
}
