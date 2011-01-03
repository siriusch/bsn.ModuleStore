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
using System.Data.SqlClient;
using System.Linq;

using bsn.ModuleStore.Mapper.Deserialization;

namespace bsn.ModuleStore.Mapper {
	public abstract class ResultSet {
		protected ResultSet() {}

		public abstract Type DataType {
			get;
		}

		public virtual Type InnerResultSetType {
			get {
				return null;
			}
		}

		public abstract int Count {
			get;
		}

		protected internal abstract void Load(SqlDataReader reader, IInstanceProvider provider);
	}

	public class ResultSet<T>: ResultSetBase<T> {
		protected internal override void Load(SqlDataReader reader, IInstanceProvider provider) {
			if (reader.HasRows) {
				List<T> result = new List<T>(512);
				using (SqlDeserializer<T> deserializer = new SqlDeserializer<T>(reader)) {
					deserializer.DisposeReader = false;
					result.AddRange(deserializer.DeserializeInstances(provider));
				}
				if (result.Capacity >= (result.Count*2)) {
					result.Capacity = result.Count;
				}
				Items = result;
			}
		}
	}

	public class ResultSet<T, TInner>: ResultSet<T> where TInner: ResultSet, new() {
		private readonly TInner inner = new TInner();

		public TInner Inner {
			get {
				return inner;
			}
		}

		public override sealed Type InnerResultSetType {
			get {
				return typeof(TInner);
			}
		}

		protected internal override void Load(SqlDataReader reader, IInstanceProvider provider) {
			base.Load(reader, provider);
			if (!reader.NextResult()) {
				throw new InvalidOperationException("An additional result set is expected, but the reader contains no more result sets");
			}
			inner.Load(reader, provider);
		}
	}
}