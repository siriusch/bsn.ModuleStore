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
//  
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace bsn.ModuleStore.Mapper {
	public abstract class ResultSet {
		protected ResultSet() {}

		protected internal abstract void Load(SqlDataReader reader);

		abstract public Type DataType {
			get;
		}

		virtual public Type InnerResultSetType {
			get {
				return null;
			}
		}
	}

	public class ResultSet<T>: ResultSet, IList<T> {
		private IList<T> items = new T[0];

		public ResultSet() {
		}

		protected internal override void Load(SqlDataReader reader) {
			if (reader.HasRows) {
				using (SqlDeserializer<List<T>> deserializer = new SqlDeserializer<List<T>>(reader)) {
					deserializer.DisposeReader = false;
					items = deserializer.Deserialize();
				}
			}
		}

		T IList<T>.this[int index] {
			get {
				return this[index];
			}
			set {
				throw new NotSupportedException();
			}
		}

		public IEnumerator<T> GetEnumerator() {
			return items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return items.GetEnumerator();
		}

		void ICollection<T>.Add(T item) {
			throw new NotSupportedException();
		}

		void ICollection<T>.Clear() {
			throw new NotSupportedException();
		}

		public bool Contains(T item) {
			return items.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex) {
			items.CopyTo(array, arrayIndex);
		}

		bool ICollection<T>.Remove(T item) {
			throw new NotSupportedException();
		}

		public int Count {
			get {
				return items.Count;
			}
		}

		bool ICollection<T>.IsReadOnly {
			get {
				return true;
			}
		}

		public int IndexOf(T item) {
			return items.IndexOf(item);
		}

		void IList<T>.Insert(int index, T item) {
			throw new NotSupportedException();
		}

		void IList<T>.RemoveAt(int index) {
			throw new NotSupportedException();
		}

		public T this[int index] {
			get {
				return items[index];
			}
		}

		public sealed override Type DataType {
			get {
				return typeof(T);
			}
		}
	}

	public class ResultSet<T, TInner>: ResultSet<T> where TInner: ResultSet, new() {
		private readonly TInner inner = new TInner();

		public ResultSet() {}

		protected internal override void Load(SqlDataReader reader) {
			base.Load(reader);
			if (!reader.NextResult()) {
				throw new InvalidOperationException("An additional result set is expected, but the reader contains no more result sets");
			}
			inner.Load(reader);
		}

		public sealed override Type InnerResultSetType {
			get {
				return typeof(TInner);
			}
		}

		public TInner Inner {
			get {
				return inner;
			}
		}
	}
}
