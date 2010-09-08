using System;
using System.Diagnostics;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class CursorStatement: Statement {
		private readonly CursorName cursorName;

		protected CursorStatement(CursorName cursorName) {
			Debug.Assert(cursorName != null);
			this.cursorName = cursorName;
		}

		public CursorName CursorName {
			get {
				return cursorName;
			}
		}
	}
}