using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bsn.ModuleStore.Sql.Script {
	internal interface ICreateOrAlterStatement {
		void WriteToInternal(SqlWriter writer, string command);
	}
}
