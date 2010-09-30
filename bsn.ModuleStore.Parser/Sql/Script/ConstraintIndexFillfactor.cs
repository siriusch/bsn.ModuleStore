using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
#warning Maybe create a base class for the (now only implicitly implemented) <IndexOptionGroup> and inherit from it
	public sealed class ConstraintIndexFillfactor: ConstraintIndex {
		private readonly IntegerLiteral fillfactor;

		[Rule("<ConstraintIndex> ::= ~WITH ~FILLFACTOR ~'=' <IntegerLiteral>")]
		public ConstraintIndexFillfactor(IntegerLiteral fillfactor) {
			this.fillfactor = fillfactor;
		}

		public IntegerLiteral Fillfactor {
			get {
				return fillfactor;
			}
		}

		public override bool HasValue {
			get {
				return true;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("WITH FILLFACTOR=");
			writer.WriteScript(fillfactor, WhitespacePadding.None);
		}
	}
}