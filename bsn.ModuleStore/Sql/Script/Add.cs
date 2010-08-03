using System;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("ADD")]
	[Terminal("ADD_PERSISTED")]
	public class Add: DdlOperation {}
}