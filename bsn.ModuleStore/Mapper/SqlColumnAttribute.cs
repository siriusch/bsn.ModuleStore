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
using System.Reflection;

namespace bsn.ModuleStore.Mapper {
	/// <summary>
	/// The DbColumnAttribute is used to change the binding name on <see cref="ITypedDataReader"/> interfaces, or to specify the fields to be deserialized when the <see cref="DbDeserializer"/> is used.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property|AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class SqlColumnAttribute: Attribute {
		/// <summary>
		/// Get a single DbColumnAttribute instance.
		/// </summary>
		/// <param name="info">The <see cref="MemberInfo"/> to query for the DbColumnAttribute attribute.</param>
		/// <param name="autoCreate">If true, a DbColumnAttribute is inferred from the MemberInfo when no attribute is found. Otherwise, null is returned in this situation.</param>
		/// <returns>The DbColumnAttribute for the member.</returns>
		public static SqlColumnAttribute GetColumnAttribute(MemberInfo info, bool autoCreate) {
			if (info == null) {
				throw new ArgumentNullException("info");
			}
			SqlColumnAttribute[] columnAttributes = (SqlColumnAttribute[])info.GetCustomAttributes(typeof(SqlColumnAttribute), true);
			if (columnAttributes.Length > 0) {
				SqlColumnAttribute result = columnAttributes[0];
				if (string.IsNullOrEmpty(result.Name)) {
					result = result.CloneWithName(info.Name);
				}
				return result;
			}
			if (autoCreate) {
				return new SqlColumnAttribute(info.Name);
			}
			return null;
		}

		private DateTimeKind dateTimeKind = DateTimeKind.Unspecified;
		private bool getCachedByIdentity;
		private bool identity;
		private string name;

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlColumnAttribute"/> class.
		/// </summary>
		public SqlColumnAttribute(): this(null) {}

		/// <summary>
		/// Create a new DbColumnAttribute.
		/// </summary>
		/// <param name="name">The DB column name to bind to.</param>
		public SqlColumnAttribute([Localizable(false)] string name) {
			this.name = name;
		}

		/// <summary>
		/// Gets or sets the kind of the DateTime, if the instance is one.
		/// </summary>
		/// <value>The kind of the date time.</value>
		public DateTimeKind DateTimeKind {
			get {
				return dateTimeKind;
			}
			set {
				dateTimeKind = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the column <see cref="SqlColumnAttribute"/> is a foreign key to an instance cached by identity in the provider.
		/// </summary>
		public bool GetCachedByIdentity {
			get {
				return getCachedByIdentity;
			}
			set {
				getCachedByIdentity = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the column <see cref="SqlColumnAttribute"/> is an identity column for this data type.
		/// </summary>
		public bool Identity {
			get {
				return identity;
			}
			set {
				identity = value;
			}
		}

		/// <summary>
		/// The name for the database binding.
		/// </summary>
		public string Name {
			get {
				return name;
			}
		}

		internal SqlColumnAttribute CloneWithName(string newName) {
			SqlColumnAttribute result = (SqlColumnAttribute)MemberwiseClone();
			result.name = newName;
			return result;
		}
	}
}
