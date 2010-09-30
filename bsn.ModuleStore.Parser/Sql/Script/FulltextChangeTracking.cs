using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class FulltextChangeTracking: SqlScriptableToken, IOptional {
		[Rule("<FulltextChangeTracking> ::=")]
		public FulltextChangeTracking() {}

		public virtual FulltextChangeTrackingKind ChangeTracking {
			get {
				return FulltextChangeTrackingKind.Unspecified;
			}
		}

		protected virtual string ChangeTrackingSpecifier {
			get {
				return string.Empty;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			Debug.Assert(HasValue);
			writer.Write("WITH CHANGE_TRACKING ");
			writer.Write(ChangeTrackingSpecifier);
		}

		public bool HasValue {
			get {
				return ChangeTracking != FulltextChangeTrackingKind.Unspecified;
			}
		}
	}
}