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
using System.Collections.Generic;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class SelectQuery: SqlScriptableToken {
		private readonly List<ColumnItem> columnItems;
		private readonly ForClause forClause;
		private readonly DestinationRowset intoClause;
		private readonly bool? restriction;
		private readonly TopExpression top;
		private readonly RowsetCombineClause unionClause;
		private readonly Predicate whereClause;

		[Rule("<SelectQuery> ::= ~SELECT <Restriction> <TopLegacy> <ColumnItemList> <IntoClause> <WhereClause> <RowsetCombineClause>")]
		public SelectQuery(DuplicateRestrictionToken restriction, TopExpression top, Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, Optional<Predicate> whereClause, RowsetCombineClause unionClause)
				: this(restriction.Distinct, top, columnItems, intoClause, whereClause, null, unionClause) {}

		[Rule("<SelectQuery> ::= ~SELECT <Restriction> <ColumnItemList> <IntoClause> <WhereClause> <RowsetCombineClause>")]
		public SelectQuery(DuplicateRestrictionToken restriction, Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, Optional<Predicate> whereClause, RowsetCombineClause unionClause): this(restriction.Distinct, null, columnItems, intoClause, whereClause, null, unionClause) {}

		[Rule("<SelectQuery> ::= ~SELECT <TopLegacy> <ColumnItemList> <IntoClause> <WhereClause> <RowsetCombineClause>")]
		public SelectQuery(TopExpression top, Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, Optional<Predicate> whereClause, RowsetCombineClause unionClause): this(default(bool?), top, columnItems, intoClause, whereClause, null, unionClause) {}

		[Rule("<SelectQuery> ::= ~SELECT <ColumnItemList> <IntoClause> <WhereClause> <RowsetCombineClause>")]
		public SelectQuery(Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, Optional<Predicate> whereClause, RowsetCombineClause unionClause): this(default(bool?), null, columnItems, intoClause, whereClause, null, unionClause) {}

		[Rule("<SelectQuery> ::= ~SELECT <Restriction> <TopLegacy> <ColumnItemList> <IntoClause> <WhereClause> <ForClause>")]
		public SelectQuery(DuplicateRestrictionToken restriction, TopExpression top, Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, Optional<Predicate> whereClause, ForClause forClause): this(restriction.Distinct, top, columnItems, intoClause, whereClause, forClause, null) {}

		[Rule("<SelectQuery> ::= ~SELECT <Restriction> <ColumnItemList> <IntoClause> <WhereClause> <ForClause>")]
		public SelectQuery(DuplicateRestrictionToken restriction, Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, Optional<Predicate> whereClause, ForClause forClause): this(restriction.Distinct, null, columnItems, intoClause, whereClause, forClause, null) {}

		[Rule("<SelectQuery> ::= ~SELECT <TopLegacy> <ColumnItemList> <IntoClause> <WhereClause> <ForClause>")]
		public SelectQuery(TopExpression top, Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, Optional<Predicate> whereClause, ForClause forClause): this(default(bool?), top, columnItems, intoClause, whereClause, forClause, null) {}

		[Rule("<SelectQuery> ::= ~SELECT <ColumnItemList> <IntoClause> <WhereClause> <ForClause>")]
		public SelectQuery(Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, Optional<Predicate> whereClause, ForClause forClause): this(default(bool?), null, columnItems, intoClause, whereClause, forClause, null) {}

		protected SelectQuery(bool? restriction, TopExpression top, Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, Optional<Predicate> whereClause, ForClause forClause, RowsetCombineClause unionClause) {
			this.top = top;
			this.intoClause = intoClause;
			this.forClause = forClause;
			this.unionClause = unionClause;
			this.columnItems = columnItems.ToList();
			this.restriction = restriction;
			this.whereClause = whereClause;
		}

		public IEnumerable<ColumnItem> ColumnItems {
			get {
				return columnItems;
			}
		}

		public ForClause ForClause {
			get {
				return forClause;
			}
		}

		public DestinationRowset IntoClause {
			get {
				return intoClause;
			}
		}

		public bool? Restriction {
			get {
				return restriction;
			}
		}

		public TopExpression Top {
			get {
				return top;
			}
		}

		public RowsetCombineClause UnionClause {
			get {
				return unionClause;
			}
		}

		public Predicate WhereClause {
			get {
				return whereClause;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("SELECT ");
			writer.WriteDuplicateRestriction(restriction, WhitespacePadding.SpaceAfter);
			writer.WriteScript(top, WhitespacePadding.SpaceAfter);
			writer.IncreaseIndent();
			writer.WriteScriptSequence(columnItems, WhitespacePadding.None, ", ");
			writer.DecreaseIndent();
			writer.WriteScript(intoClause, WhitespacePadding.NewlineBefore, "INTO ", null);
			WriteToInternal(writer);
			writer.WriteScript(forClause, WhitespacePadding.SpaceBefore);
			writer.WriteScript(unionClause, WhitespacePadding.NewlineBefore);
		}

		protected override void Initialize(Symbol symbol, LineInfo position) {
			base.Initialize(symbol, position);
			HashSet<string> virtualTableNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (RowsetTableAlias tableAlias in GetInnerTokens<RowsetTableAlias>(null, typeof(SelectQuery))) {
				virtualTableNames.Add(tableAlias.AliasName.Value);
			}
			if (virtualTableNames.Count > 0) {
				LockInnerUnqualifiedTableNames(tn => virtualTableNames.Contains(tn));
			}
		}

		protected virtual void WriteToInternal(SqlWriter writer) {
			writer.WriteScript(whereClause, WhitespacePadding.NewlineBefore, "WHERE ", null);
		}
	}
}
