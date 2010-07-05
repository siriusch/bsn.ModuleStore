using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class SetIdentityInsertStatement: SqlStatement {
		private readonly Toggle enabled;
		private readonly TableName table;

		[Rule("<SetOptionStatement> ::= SET IDENTITY_INSERT <TableName> <Toggle>", ConstructorParameterMapping=new[] { 2, 3 })]
		public SetIdentityInsertStatement(TableName tableName, Toggle toggle) {
			if (tableName == null) {
				throw new ArgumentNullException("tableName");
			}
			if (toggle == null) {
				throw new ArgumentNullException("toggle");
			}
			table = tableName;
			enabled = toggle;
		}

		public override void WriteTo(System.IO.TextWriter writer) {
			writer.Write("SET IDENTITY INSERT ");
			table.WriteTo(writer);
			writer.Write(' ');
			enabled.WriteTo(writer);
		}
	}
}