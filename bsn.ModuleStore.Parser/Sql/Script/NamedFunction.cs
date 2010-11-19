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
using System.Collections.Generic;
using System.Diagnostics;

using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class NamedFunction: FunctionCall {
		private readonly List<Expression> arguments;
		private readonly Qualified<SchemaName, FunctionName> functionName;

		[Rule("<NamedFunction> ::= <FunctionName> ~'(' ~')'")]
		public NamedFunction(FunctionName functionName): this(functionName, null) {}

		[Rule("<NamedFunction> ::= <FunctionName> ~'(' <ExpressionList> ~')'")]
		[Rule("<FunctionCall> ::= COALESCE ~'(' <ExpressionList> ~')'")]
		public NamedFunction(FunctionName functionName, Sequence<Expression> arguments): this(new Qualified<SchemaName, FunctionName>(functionName), arguments.ToList()) {}

		private NamedFunction(Qualified<SchemaName, FunctionName> functionName, List<Expression> arguments) {
			Debug.Assert(functionName != null);
			this.functionName = functionName;
			if ((!functionName.IsQualified) && (functionName.Name.IsBuiltinFunction)) {
				functionName.LockOverride();
			}
			this.arguments = arguments;
		}

		public IEnumerable<Expression> Arguments {
			get {
				return arguments;
			}
		}

		public Qualified<SchemaName, FunctionName> FunctionName {
			get {
				return functionName;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(functionName, WhitespacePadding.None);
			writer.Write('(');
			writer.WriteScriptSequence(arguments, WhitespacePadding.None, ", ");
			writer.Write(')');
		}

		internal NamedFunction QualifiedWith(SqlName qualification) {
			Qualified<SchemaName, FunctionName> qualifiedFunctionName = new Qualified<SchemaName, FunctionName>(new SchemaName(qualification.Value), functionName.Name);
			qualifiedFunctionName.SetPosition(((IToken)qualification).Position);
			return new NamedFunction(qualifiedFunctionName, arguments);
		}
	}
}
