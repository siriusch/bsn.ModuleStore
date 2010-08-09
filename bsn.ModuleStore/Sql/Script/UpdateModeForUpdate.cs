using System;
using System.Collections.Generic;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class UpdateModeForUpdate: UpdateMode {
		private readonly List<ColumnName> columns = new List<ColumnName>();

		[Rule("<CursorUpdate> ::= FOR_UPDATE", AllowTruncationForConstructor = true)]
		public UpdateModeForUpdate() {
		}

		[Rule("<CursorUpdate> ::= FOR_UPDATE OF <ColumnNameList>", ConstructorParameterMapping=new[] { 2 })]
		public UpdateModeForUpdate(Sequence<ColumnName> columns)
				: base() {
			if (columns == null) {
				throw new ArgumentNullException("columns");
			}
			this.columns.AddRange(columns);
		}

		public List<ColumnName> Columns {
			get {
				return columns;
			}
		}

		public override UpdateModeKind Kind {
			get {
				return UpdateModeKind.ForUpdate;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("FOR UPDATE");
			if (columns.Count>0) {
				writer.Write(" OF ");
				writer.WriteSequence(columns, null, ", ", null);
			}
		}
	}
}