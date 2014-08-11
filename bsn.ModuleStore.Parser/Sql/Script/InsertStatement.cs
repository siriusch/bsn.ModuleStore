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

namespace bsn.ModuleStore.Sql.Script {
	[CommonTableExpressionScope]
	public abstract class InsertStatement: Statement {
		private readonly DestinationRowset destinationRowset;
		private readonly QueryHint queryHint;
		private readonly QueryOptions queryOptions;
		private readonly TopExpression topExpression;

		protected InsertStatement(QueryOptions queryOptions, TopExpression topExpression, DestinationRowset destinationRowset, QueryHint queryHint) {
			this.queryOptions = queryOptions;
			this.topExpression = topExpression;
			this.destinationRowset = destinationRowset;
			this.queryHint = queryHint;
		}

		public DestinationRowset DestinationRowset {
			get {
				return destinationRowset;
			}
		}

		public QueryHint QueryHint {
			get {
				return queryHint;
			}
		}

		public QueryOptions QueryOptions {
			get {
				return queryOptions;
			}
		}

		public TopExpression TopExpression {
			get {
				return topExpression;
			}
		}

		public override sealed void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.WriteScript(queryOptions, WhitespacePadding.NewlineAfter);
			writer.WriteKeyword("INSERT ");
			using (writer.Indent()) {
				writer.WriteScript(topExpression, WhitespacePadding.SpaceAfter);
				writer.WriteLine();
				writer.WriteKeyword("INTO ");
				writer.WriteScript(destinationRowset, WhitespacePadding.None);
				WriteToInternal(writer);
				writer.WriteScript(QueryHint, WhitespacePadding.NewlineBefore);
			}
		}

		protected abstract void WriteToInternal(SqlWriter writer);
	}
}
