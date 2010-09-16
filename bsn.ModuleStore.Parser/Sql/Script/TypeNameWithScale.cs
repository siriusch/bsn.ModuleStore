﻿using System;
using System.Diagnostics;
using System.Globalization;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TypeNameWithScale: TypeNameWithPrecision {
		private readonly long scale;

		[Rule("<TypeName> ::= Id ~'(' <IntegerLiteral> ~',' <IntegerLiteral> ~')'")]
		public TypeNameWithScale(SqlIdentifier identifier, IntegerLiteral precision, IntegerLiteral scale): base(identifier, precision) {
			Debug.Assert(scale != null);
			this.scale = scale.Value;
		}

		public long Scale {
			get {
				return scale;
			}
		}

		protected override void WriteArguments(SqlWriter writer) {
			base.WriteArguments(writer);
			writer.Write(", ");
			writer.Write(scale.ToString(NumberFormatInfo.InvariantInfo));
		}
	}
}