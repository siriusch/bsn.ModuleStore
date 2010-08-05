using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class DropIndexStatement: SqlDropStatement {
		[Rule("<DropIndexStatement> ::= DROP INDEX <IndexName> ON <TableName> <IndexOptionGroup>", ConstructorParameterMapping = new[] { 2, 4, 5 })]
		public DropIndexStatement(IndexName indexName, TableName tableName, Optional<Sequence<IndexOption>> indexOptions) {}
	}
}
