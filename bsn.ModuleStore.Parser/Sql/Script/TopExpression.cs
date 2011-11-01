// bsn ModuleStore database versioning
// -----------------------------------
// 
// Copyright 2010 by Arsène von Wyss - avw@gmx.ch
// 
// Development has been supported by Sirius Technologies AG, Basel
// 
// Source:
// 
// https://bsn-modulestore.googlecode.com/hg/
// 
// License:
// 
// The library is distributed under the GNU Lesser General Public License:
// http://www.gnu.org/licenses/lgpl.html
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TopExpression: SqlScriptableToken, IOptional {
		private readonly Expression expression;
		private readonly bool percent;
		private readonly bool withTies;

		[Rule("<OptionalTop> ::=")]
		public TopExpression(): this(null, null, null) {}

		[Rule("<TopLegacy> ::= ~TOP IntegerLiteral <OptionalPercent>")]
		public TopExpression(IntegerLiteral integerLiteral, Optional<PercentToken> percent): this(integerLiteral, percent, null) {}

		[Rule("<Top> ::= ~TOP ~'(' <Expression> ~')' <OptionalPercent> <OptionalWithTies>")]
		public TopExpression(Expression expression, Optional<PercentToken> percent, OptionToken option) {
			this.expression = expression;
			this.percent = percent.HasValue();
			withTies = option is OptionTiesToken;
		}

		public Expression Expression {
			get {
				return expression;
			}
		}

		public bool Percent {
			get {
				return percent;
			}
		}

		public bool WithTies {
			get {
				return withTies;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			if (HasValue) {
				writer.Write("TOP (");
				writer.WriteScript(expression, WhitespacePadding.None);
				writer.Write(')');
				if (percent) {
					writer.Write(" PERCENT");
				}
				if (withTies) {
					writer.Write(" WITH TIES");
				}
			}
		}

		public bool HasValue {
			get {
				return expression != null;
			}
		}
	}
}
