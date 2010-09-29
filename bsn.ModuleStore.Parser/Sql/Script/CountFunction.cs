﻿using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CountFunction: FunctionCall {
		private readonly Qualified<SqlName, ColumnName> columnName;
		private readonly bool? restriction;

		[Rule("<FunctionCall> ::= ~COUNT_ <Restriction> <ColumnWildNameQualified> ~')'")]
		public CountFunction(DuplicateRestrictionToken restriction, Qualified<SqlName, ColumnName> columnName): this(restriction.Distinct, columnName) {}

		[Rule("<FunctionCall> ::= ~COUNT_ <ColumnWildNameQualified> ~')'")]
		public CountFunction(Qualified<SqlName, ColumnName> columnName): this(default(bool?), columnName) {}

		private CountFunction(bool? restriction, Qualified<SqlName, ColumnName> columnName) {
			Debug.Assert(columnName != null);
			this.restriction = restriction;
			this.columnName = columnName;
		}

		public Qualified<SqlName, ColumnName> ColumnName {
			get {
				return columnName;
			}
		}

		public bool? Restriction {
			get {
				return restriction;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("COUNT(");
			writer.WriteDuplicateRestriction(restriction, WhitespacePadding.SpaceAfter);
			writer.WriteScript(columnName, WhitespacePadding.None);
			writer.Write(')');
		}
	}
}