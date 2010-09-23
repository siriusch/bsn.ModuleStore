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
		private readonly UnionClause unionClause;

		[Rule("<SelectQuery> ::= ~SELECT <Restriction> <TopLegacy> <ColumnItemList> <IntoClause> <UnionClause>")]
		public SelectQuery(DuplicateRestrictionToken restriction, TopExpression top, Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, UnionClause unionClause): this(restriction.Distinct, top, columnItems, intoClause, null, unionClause) {}

		[Rule("<SelectQuery> ::= ~SELECT <Restriction> <ColumnItemList> <IntoClause> <UnionClause>")]
		public SelectQuery(DuplicateRestrictionToken restriction, Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, UnionClause unionClause): this(restriction.Distinct, null, columnItems, intoClause, null, unionClause) {}

		[Rule("<SelectQuery> ::= ~SELECT <TopLegacy> <ColumnItemList> <IntoClause> <UnionClause>")]
		public SelectQuery(TopExpression top, Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, UnionClause unionClause): this(default(bool?), top, columnItems, intoClause, null, unionClause) {}

		[Rule("<SelectQuery> ::= ~SELECT <ColumnItemList> <IntoClause> <UnionClause>")]
		public SelectQuery(Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, UnionClause unionClause): this(default(bool?), null, columnItems, intoClause, null, unionClause) {}

		[Rule("<SelectQuery> ::= ~SELECT <Restriction> <TopLegacy> <ColumnItemList> <IntoClause> <ForClause>")]
		public SelectQuery(DuplicateRestrictionToken restriction, TopExpression top, Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, ForClause forClause): this(restriction.Distinct, top, columnItems, intoClause, forClause, null) {}

		[Rule("<SelectQuery> ::= ~SELECT <Restriction> <ColumnItemList> <IntoClause> <ForClause>")]
		public SelectQuery(DuplicateRestrictionToken restriction, Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, ForClause forClause): this(restriction.Distinct, null, columnItems, intoClause, forClause, null) {}

		[Rule("<SelectQuery> ::= ~SELECT <TopLegacy> <ColumnItemList> <IntoClause> <ForClause>")]
		public SelectQuery(TopExpression top, Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, ForClause forClause): this(default(bool?), top, columnItems, intoClause, forClause, null) {}

		[Rule("<SelectQuery> ::= ~SELECT <ColumnItemList> <IntoClause> <ForClause>")]
		public SelectQuery(Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, ForClause forClause): this(default(bool?), null, columnItems, intoClause, forClause, null) {}

		protected SelectQuery(bool? restriction, TopExpression top, Sequence<ColumnItem> columnItems, Optional<DestinationRowset> intoClause, ForClause forClause, UnionClause unionClause) {
			this.top = top;
			this.intoClause = intoClause;
			this.forClause = forClause;
			this.unionClause = unionClause;
			this.columnItems = columnItems.ToList();
			this.restriction = restriction;
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

		public UnionClause UnionClause {
			get {
				return unionClause;
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
			foreach (SourceTableColumnNodesRowset virtualTable in GetInnerTokens<SourceTableColumnNodesRowset>(null, typeof(SelectQuery))) {
				virtualTableNames.Add(virtualTable.AliasName.Value);
			}
			if (virtualTableNames.Count > 0) {
				LockInnerUnqualifiedTableNames(tn => virtualTableNames.Contains(tn));
			}
		}

		protected virtual void WriteToInternal(SqlWriter writer) {}
	}
}