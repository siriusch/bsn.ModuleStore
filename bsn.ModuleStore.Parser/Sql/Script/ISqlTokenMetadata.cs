using System;
using System.Collections.Generic;

namespace bsn.ModuleStore.Sql.Script {
	internal interface ISqlTokenMetadata<TToken> where TToken: SqlToken {
		IEnumerable<SqlToken> EnumerateTokens(TToken instance);
	}
}