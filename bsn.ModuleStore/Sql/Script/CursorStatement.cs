using System;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class CursorStatement: Statement {
		private readonly CursorName cursorName;

		protected CursorStatement(CursorName cursorName) {
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
	}
}