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
//  
using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class PredicateLike: PredicateNegable {
		private readonly StringLiteral escape;
		private readonly StringLiteral text;
		private readonly Expression valueExpression;

		[Rule("<PredicateLike> ::= <Expression> ~LIKE <CollableStringLiteral>")]
		public PredicateLike(Expression valueExpression, StringLiteral text): this(valueExpression, false, text, null) {}

		[Rule("<PredicateLike> ::= <Expression> ~LIKE <CollableStringLiteral> ~ESCAPE StringLiteral")]
		public PredicateLike(Expression valueExpression, StringLiteral text, StringLiteral escape): this(valueExpression, false, text, escape) {}

		protected PredicateLike(Expression valueExpression, bool not, StringLiteral text, StringLiteral escape): base(not) {
			Debug.Assert(valueExpression != null);
			Debug.Assert(text != null);
			this.valueExpression = valueExpression;
			this.text = text;
			this.escape = escape;
		}

		public StringLiteral Escape {
			get {
				return escape;
			}
		}

		public StringLiteral Text {
			get {
				return text;
			}
		}

		public Expression ValueExpression {
			get {
				return valueExpression;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.WriteScript(valueExpression, WhitespacePadding.None);
			base.WriteTo(writer);
			writer.Write(" LIKE ");
			writer.WriteScript(text, WhitespacePadding.None);
			writer.WriteScript(escape, WhitespacePadding.None, "ESCAPE ", null);
		}
	}
}
