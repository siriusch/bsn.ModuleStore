using System;
using System.Collections.Generic;
using System.IO;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class SelectQuery: SqlToken, IScriptable {
		private readonly List<ColumnItem> columnItems;
		private readonly DestinationRowset intoClause;
		private readonly bool? restriction;
		private readonly TopExpression top;
		private readonly UnionClause unionClause;

		[Rule("<SelectQuery> ::= SELECT <Restriction> <TopLegacy> <ColumnItemList> <IntoClause> <UnionClause>", ConstructorParameterMapping = new[] {1, 2, 3, 4, 5})]
		public SelectQuery(DuplicateRestrictionToken restriction, TopExpression top, Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, UnionClause unionClause): this(restriction.Distinct, top, columnItems, intoClause, unionClause) {}

		[Rule("<SelectQuery> ::= SELECT <Restriction> <ColumnItemList> <IntoClause> <UnionClause>", ConstructorParameterMapping=new[] { 1, 2, 3, 4 })]
		public SelectQuery(DuplicateRestrictionToken restriction, Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, UnionClause unionClause) : this(restriction.Distinct, null, columnItems, intoClause, unionClause) {
		}

		[Rule("<SelectQuery> ::= SELECT <TopLegacy> <ColumnItemList> <IntoClause> <UnionClause>", ConstructorParameterMapping=new[] { 1, 2, 3, 4 })]
		public SelectQuery(TopExpression top, Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, UnionClause unionClause) : this(default(bool?), top, columnItems, intoClause, unionClause) {
		}

		[Rule("<SelectQuery> ::= SELECT <ColumnItemList> <IntoClause> <UnionClause>", ConstructorParameterMapping=new[] { 1, 2, 3 })]
		public SelectQuery(Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, UnionClause unionClause)
			: this(default(bool?), null, columnItems, intoClause, unionClause) {
		}

		protected SelectQuery(bool? restriction, TopExpression top, Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, UnionClause unionClause) {
			this.top = top;
			this.intoClause = intoClause;
			this.unionClause = unionClause;
			this.columnItems = columnItems.ToList();
			this.restriction = restriction;
		}

		public List<ColumnItem> ColumnItems {
			get {
				return columnItems;
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

		public UnionClause UnionClause {
			get {
				return unionClause;
			}
		}

		protected virtual void WriteToInternal(SqlWriter writer) {}

		public void WriteTo(SqlWriter writer) {
			writer.Write("SELECT ");
			writer.WriteDuplicateRestriction(restriction, WhitespacePadding.SpaceAfter);
			writer.WriteScript(top, WhitespacePadding.SpaceAfter);
			writer.WriteSequence(columnItems, WhitespacePadding.None, ", ");
			writer.WriteScript(intoClause, WhitespacePadding.NewlineBefore, "INTO ", null);
			WriteToInternal(writer);
			writer.WriteScript(unionClause, WhitespacePadding.NewlineBefore);
		}
	}
}