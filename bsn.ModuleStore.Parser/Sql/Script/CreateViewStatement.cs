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
using System.Diagnostics;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateViewStatement: AlterableCreateStatement, ICreateOrAlterStatement {
		private readonly List<ColumnName> columnNames;
		private readonly SelectStatement selectStatement;
		private readonly Qualified<SchemaName, ViewName> viewName;
		private readonly OptionToken viewOption;
		private readonly bool withCheckOption;

		[Rule("<CreateViewStatement> ::= ~CREATE ~VIEW <ViewNameQualified> <ColumnNameGroup> <ViewOptionalAttribute> ~AS <SelectStatement> <ViewOptionalCheckOption>")]
		public CreateViewStatement(Qualified<SchemaName, ViewName> viewName, Optional<Sequence<ColumnName>> columnNames, OptionToken viewOption, SelectStatement selectStatement, Optional<WithCheckOptionToken> withCheckOption) {
			Debug.Assert(viewName != null);
			Debug.Assert(selectStatement != null);
			this.viewName = viewName;
			this.viewOption = viewOption;
			this.columnNames = columnNames.ToList();
			this.selectStatement = selectStatement;
			this.withCheckOption = withCheckOption.HasValue();
		}

		public IEnumerable<ColumnName> ColumnNames {
			get {
				return columnNames;
			}
		}

		public override ObjectCategory ObjectCategory {
			get {
				return ObjectCategory.View;
			}
		}

		public override string ObjectName {
			get {
				return viewName.Name.Value;
			}
			set {
				viewName.Name = new ViewName(value);
			}
		}

		public SelectStatement SelectStatement {
			get {
				return selectStatement;
			}
		}

		public Qualified<SchemaName, ViewName> ViewName {
			get {
				return viewName;
			}
		}

		public OptionToken ViewOption {
			get {
				return viewOption;
			}
		}

		public bool WithCheckOption {
			get {
				return withCheckOption;
			}
		}

		protected override SchemaName SchemaName {
			get {
				return viewName.Qualification;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteToInternal(writer, "CREATE");
		}

		protected override IInstallStatement CreateAlterStatement() {
			return new AlterOfCreateStatement<CreateViewStatement>(this);
		}

		protected override IInstallStatement CreateDropStatement() {
			return new DropViewStatement(viewName);
		}

		private void WriteToInternal(SqlWriter writer, string command) {
			WriteCommentsTo(writer);
			writer.WriteKeyword(command);
			writer.WriteKeyword(" VIEW ");
			writer.WriteScript(viewName, WhitespacePadding.None);
			if (columnNames.Count > 0) {
				writer.Write(" (");
				writer.WriteScriptSequence(columnNames, WhitespacePadding.None, w => w.Write(", "));
				writer.Write(')');
			}
			writer.WriteScript(viewOption, WhitespacePadding.SpaceBefore);
			using (writer.Indent()) {
				writer.WriteKeyword(" AS");
				writer.WriteLine();
				writer.WriteScript(selectStatement, WhitespacePadding.None);
				if (withCheckOption) {
					writer.WriteKeyword(" WITH CHECK OPTION");
				}
			}
		}

		void ICreateOrAlterStatement.WriteToInternal(SqlWriter writer, string command) {
			if (string.IsNullOrEmpty(command)) {
				throw new ArgumentNullException("command");
			}
			WriteToInternal(writer, command);
		}
	}
}
