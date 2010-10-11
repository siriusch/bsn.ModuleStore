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
	public class PredicateIn: PredicateNegable {
		private readonly Tuple tuple;
		private readonly Expression valueExpression;

		[Rule("<PredicateIn> ::= <Expression> ~IN <Tuple>")]
		public PredicateIn(Expression valueExpression, Tuple tuple): this(valueExpression, false, tuple) {}

		protected PredicateIn(Expression valueExpression, bool not, Tuple tuple): base(not) {
			Debug.Assert(valueExpression != null);
			Debug.Assert(tuple != null);
			this.valueExpression = valueExpression;
			this.tuple = tuple;
		}

		public Tuple Tuple {
			get {
				return tuple;
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
			writer.Write(" IN ");
			writer.WriteScript(tuple, WhitespacePadding.None);
		}
	}
}
