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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace bsn.ModuleStore.Mapper {
	public sealed class ReferenceEqualityComparer<T>: IEqualityComparer<T> where T: class {
		private static readonly ReferenceEqualityComparer<T> @default = new ReferenceEqualityComparer<T>();

		public static ReferenceEqualityComparer<T> Default {
			get {
				return @default;
			}
		}

		private ReferenceEqualityComparer() {}

		public bool Equals(T x, T y) {
			return x == y; // this must always perform a reference comparison, since T can be any class no operator overloading is applied
		}

		public int GetHashCode(T obj) {
			return RuntimeHelpers.GetHashCode(obj);
		}
	}
}