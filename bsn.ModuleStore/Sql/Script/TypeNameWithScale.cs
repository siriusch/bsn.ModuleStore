using System;
using System.Globalization;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TypeNameWithScale: TypeNameWithPrecision {
		private readonly long scale;

		[Rule("<TypeName> ::= Id '(' <IntegerLiteral> ',' <IntegerLiteral> ')'", ConstructorParameterMapping = new[] {0, 2, 4})]
		public TypeNameWithScale(SqlIdentifier identifier, IntegerLiteral precision, IntegerLiteral scale)
				: base(identifier, precision) {
			if (scale == null) {
				throw new ArgumentNullException("scale");
			}
			this.scale = scale.Value;
		}

		public long Scale {
			get {
				return scale;
			}
		}

		protected override void WriteArguments(TextWriter writer) {
			base.WriteArguments(writer);
			writer.Write(", ");
			writer.Write(scale.ToString(NumberFormatInfo.InvariantInfo));
		}
	}
}