using System;
using System.IO;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public class FulltextChangeTracking: SqlToken {
		public enum TrackingMode {
			Off,
			OffNoPopulation,
			Manual,
			Auto
		}

		private readonly TrackingMode mode;

		[Rule("<FulltextChangeTracking> ::= WITH_CHANGE_TRACKING Id", ConstructorParameterMapping = new[] {1})]
		public FulltextChangeTracking(Identifier identifier) {
			if (string.Equals(identifier.Value, "MANUAL", StringComparison.OrdinalIgnoreCase)) {
				mode = TrackingMode.Manual;
			} else if (string.Equals(identifier.Value, "AUTO", StringComparison.OrdinalIgnoreCase)) {
				mode = TrackingMode.Auto;
			}
		}

		[Rule("<FulltextChangeTracking> ::= WITH_CHANGE_TRACKING OFF", ConstructorParameterMapping = new[] {1})]
		[Rule("<FulltextChangeTracking> ::= WITH_CHANGE_TRACKING OFF ',' NO_POPULATION", ConstructorParameterMapping = new[] {3})]
		public FulltextChangeTracking(SqlToken mode) {
			if (mode is ToggleToken) {
				this.mode = TrackingMode.Off;
			} else {
				this.mode = TrackingMode.OffNoPopulation;
			}
		}

		public override void WriteTo(TextWriter writer) {
			writer.Write("WITH CHANGE TRACKING ");
			if (mode == TrackingMode.OffNoPopulation) {
				writer.Write("OFF, NO POPULATION");
			} else {
				writer.Write(mode.ToString().ToUpperInvariant());
			}
		}
	}
}