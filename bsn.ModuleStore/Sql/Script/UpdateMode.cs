using System.IO;
using System.Text.RegularExpressions;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class UpdateMode: SqlToken, IScriptable {
		protected UpdateMode() {}

		public abstract UpdateModeKind Kind {
			get;
		}

		public abstract void WriteTo(TextWriter writer);
	}
}