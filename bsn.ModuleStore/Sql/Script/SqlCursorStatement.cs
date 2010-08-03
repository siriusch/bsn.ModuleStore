using System;
using System.IO;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SqlCursorStatement: SqlStatement {
		private readonly CursorName cursorName;

		protected SqlCursorStatement(CursorName cursorName) {
			if (cursorName == null) {
				throw new ArgumentNullException("cursorName");
			}
			this.cursorName = cursorName;
		}

		public CursorName CursorName {
			get {
				return cursorName;
			}
		}

		public override void WriteTo(TextWriter writer) {
			cursorName.WriteTo(writer);
		}
	}
}