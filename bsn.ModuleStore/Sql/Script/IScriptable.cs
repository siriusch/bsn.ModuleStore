using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace bsn.ModuleStore.Sql.Script {
	public interface IScriptable {
		void WriteTo(TextWriter writer);
	}
}
