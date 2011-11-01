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

using System;

namespace bsn.ModuleStore.Mapper {
	/// <summary>
	/// The SqlDeserializeAttribute can be specified on fields representing nested data that should be deserialized.
	/// </summary>
	/// <example>
	/// The class B contains the nested class A, which will be deserialized as inner object from a resultset containing the columns [a,b].
	/// <code>
	/// public class A {
	///		[DbColumn('a')]
	///		private int a;
	/// }
	/// 
	/// public class B {
	///		[DbDeserialize]
	///		private A a;
	///		[DbColumn('b')]
	///		private int b;
	/// }
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class SqlDeserializeAttribute: Attribute {
		private bool notNull;

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
