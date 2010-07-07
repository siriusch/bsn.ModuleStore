using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class FulltextChangeTracking: SqlToken {
		private readonly TrackingMode mode;

		public enum TrackingMode {
			Off,
			OffNoPopulation,
			Manual,
			Auto
		}

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
			if (mode is Toggle) {
				this.mode = TrackingMode.Off;
			} else {
				this.mode = TrackingMode.OffNoPopulation;
			}
		}

		public override void WriteTo(System.IO.TextWriter writer) {
			writer.Write("WITH CHANGE TRACKING ");
			if (mode == TrackingMode.OffNoPopulation) {
				writer.Write("OFF, NO POPULATION");
			} else {
				writer.Write(mode.ToString().ToUpperInvariant());
			}
		}
	}
}