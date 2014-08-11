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
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ConvertFunction: FunctionCall {
		private readonly IntegerLiteral style;
		private readonly TypeName typeName;
		private readonly Expression valueExpression;

		[Rule("<FunctionCall> ::= ~CONVERT ~'(' <TypeName> ~',' <Expression> ~')'")]
		public ConvertFunction(TypeName typeName, Expression valueExpression): this(typeName, valueExpression, null) {}

		[Rule("<FunctionCall> ::= ~CONVERT ~'(' <TypeName> ~',' <Expression> ~',' IntegerLiteral ~')'")]
		public ConvertFunction(TypeName typeName, Expression valueExpression, IntegerLiteral style) {
			Debug.Assert(typeName != null);
			Debug.Assert(valueExpression != null);
			this.typeName = typeName;
			this.valueExpression = valueExpression;
			this.style = style;
		}

		public IntegerLiteral Style {
			get {
				return style;
			}
		}

		public TypeName TypeName {
			get {
				return typeName;
			}
		}

		public Expression ValueExpression {
			get {
				return valueExpression;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteIdentifier("CONVERT");
			writer.Write('(');
			writer.WriteScript(typeName, WhitespacePadding.None);
			writer.Write(", ");
			writer.WriteScript(valueExpression, WhitespacePadding.None);
			writer.WriteScript(style, WhitespacePadding.None, w => w.Write(", "), null);
			writer.Write(')');
		}
	}
}
