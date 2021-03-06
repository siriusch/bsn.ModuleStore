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
using System.Collections.Generic;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class OutputClause: SqlScriptableToken, IOptional {
		private readonly List<ColumnItem> columnItems;
		private readonly List<ColumnName> destinationColumnNames;
		private readonly DestinationRowset destinationName;

		[Rule("<OutputClause> ::=")]
		public OutputClause(): this(null, null, null) {}

		[Rule("<OutputClause> ::= ~OUTPUT <ColumnItemList>")]
		public OutputClause(Sequence<ColumnItem> columnItems): this(columnItems, null, null) {}

		[Rule("<OutputClause> ::= ~OUTPUT <ColumnItemList> ~INTO <DestinationRowset> <ColumnNameGroup>")]
		public OutputClause(Sequence<ColumnItem> columnItems, DestinationRowset destinationName, Optional<Sequence<ColumnName>> destinationColumnNames) {
			this.destinationName = destinationName;
			this.destinationColumnNames = destinationColumnNames.ToList();
			this.columnItems = columnItems.ToList();
		}

		public IEnumerable<ColumnItem> ColumnItems => columnItems;

		public IEnumerable<ColumnName> DestinationColumnNames => destinationColumnNames;

		public DestinationRowset DestinationName => destinationName;

		public override void WriteTo(SqlWriter writer) {
			if (HasValue) {
				writer.WriteKeyword("OUTPUT ");
				using (writer.Indent()) {
					writer.WriteScriptSequence(columnItems, WhitespacePadding.None, w => w.Write(", "));
					if (destinationName != null) {
						writer.WriteLine();
						writer.WriteKeyword("INTO ");
						writer.WriteScript(destinationName, WhitespacePadding.None);
						if (destinationColumnNames.Count > 0) {
							writer.Write(" (");
							writer.WriteScriptSequence(destinationColumnNames, WhitespacePadding.None, w => w.Write(", "));
							writer.Write(')');
						}
					}
				}
			}
		}

		protected override void Initialize(Symbol symbol, LineInfo position) {
			base.Initialize(symbol, position);
			LockInnerUnqualifiedTableNames(IsInsertedOrDeletedTableName);
		}

		public bool HasValue => columnItems.Count > 0;
	}
}
