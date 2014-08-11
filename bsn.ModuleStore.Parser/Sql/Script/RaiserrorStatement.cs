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
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class RaiserrorStatement: Statement {
		private readonly List<Expression> arguments;
		private readonly List<UnreservedKeyword> options;

		[Rule("<RaiserrorStatement> ::= ~RAISERROR ~'(' <ExpressionList> ~')'")]
		public RaiserrorStatement(Sequence<Expression> arguments): this(arguments, null) {}

		[Rule("<RaiserrorStatement> ::= ~RAISERROR ~'(' <ExpressionList> ~')' ~WITH <RaiserrorOptionList>")]
		public RaiserrorStatement(Sequence<Expression> arguments, Sequence<UnreservedKeyword> options) {
			Debug.Assert(arguments != null);
			this.arguments = arguments.ToList();
			this.options = options.ToList();
		}

		public IEnumerable<Expression> Arguments {
			get {
				return arguments;
			}
		}

		public IEnumerable<UnreservedKeyword> Options {
			get {
				return options;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.WriteKeyword("RAISERROR");
			writer.Write('(');
			using (writer.Indent()) {
				writer.WriteScriptSequence(arguments, WhitespacePadding.None, w => w.Write(", "));
			}
			writer.Write(')');
			if (options.Count > 0) {
				writer.WriteKeyword(" WITH ");
				writer.WriteScriptSequence(options, WhitespacePadding.None, w => w.Write(", "));
			}
		}
	}
}
