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

namespace bsn.ModuleStore.Sql.Script {
	public class PredicateBetween: PredicateNegable {
		private readonly Expression lowerBound;
		private readonly Expression upperBound;
		private readonly Expression valueExpression;

		[Rule("<PredicateOr> ::= <Expression> ~BETWEEN <Expression> ~AND <Expression>")]
		public PredicateBetween(Expression valueExpression, Expression min, Expression max): this(valueExpression, false, min, max) {}

		protected PredicateBetween(Expression valueExpression, bool not, Expression lowerBound, Expression upperBound): base(not) {
			this.valueExpression = valueExpression;
			this.lowerBound = lowerBound;
			this.upperBound = upperBound;
		}

		public Expression LowerBound {
			get {
				return lowerBound;
			}
		}

		public Expression UpperBound {
			get {
				return upperBound;
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
			writer.Write(" BETWEEN ");
			writer.WriteScript(lowerBound, WhitespacePadding.None);
			writer.Write(" AND ");
			writer.WriteScript(upperBound, WhitespacePadding.None);
		}
	}
}
