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

using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class ExpressionFunctionCalls: ExpressionFunction {
		private readonly FunctionCall function;
		private readonly List<NamedFunction> functions;

		// the "schema-qualified function" trick - avoids reduce-reduce issue
		[Rule("<Value> ::= <TableName> ~'.' <NamedFunctionList>")]
		public ExpressionFunctionCalls(TableName schemaName, Sequence<NamedFunction> functions): this(functions.Item.QualifiedWith(schemaName), functions.Next) {}

		[Rule("<Value> ::= <FunctionCall>")]
		public ExpressionFunctionCalls(FunctionCall function): this(function, null) {}

		[Rule("<Value> ::= <FunctionCall> ~'.' <NamedFunctionList>")]
		public ExpressionFunctionCalls(FunctionCall function, Sequence<NamedFunction> functions) {
			Debug.Assert(function != null);
			this.function = function;
			this.functions = functions.ToList();
		}

		public FunctionCall Function {
			get {
				return function;
			}
		}

		public IEnumerable<NamedFunction> Functions {
			get {
				return functions;
			}
		}

		public override sealed void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			WriteToInternal(writer);
		}

		protected virtual void WriteToInternal(SqlWriter writer) {
			writer.WriteScript(function, WhitespacePadding.None);
			if (functions.Count > 0) {
				writer.Write('.');
				writer.WriteScriptSequence(functions, WhitespacePadding.None, ".");
			}
		}
	}
}
