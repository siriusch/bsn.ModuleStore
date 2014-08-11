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

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class IndexUsing: SqlScriptableToken, IOptional {
		private readonly IndexName indexName;

		[Rule("<IndexUsing> ::=")]
		public IndexUsing(): this(null) {}

		protected IndexUsing(IndexName indexName) {
			this.indexName = indexName;
		}

		public virtual IndexFor IndexFor {
			get {
				return IndexFor.None;
			}
		}

		public IndexName IndexName {
			get {
				return indexName;
			}
		}

		protected virtual string IndexForSpecifier {
			get {
				return string.Empty;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			if (HasValue) {
				writer.WriteKeyword("USING XML INDEX ");
				writer.WriteScript(indexName, WhitespacePadding.None);
				writer.WriteLine();
				writer.WriteKeyword("FOR ");
				writer.Write(IndexForSpecifier);
			}
		}

		public bool HasValue {
			get {
				return IndexFor != IndexFor.None;
			}
		}
	}
}
