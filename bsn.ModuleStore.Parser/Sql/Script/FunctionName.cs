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

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("COALESCE")]
	[Terminal("CONVERT")]
	public sealed class FunctionName: SqlQuotedName {
		private static KeyValuePair<string, bool> FormatName(string name) {
			bool isBuiltIn = ScriptParser.TryGetBuiltinFunctionName(ref name);
			return new KeyValuePair<string, bool>(name, isBuiltIn);
		}

		private readonly bool builtinFunction;

		public FunctionName(string name): this(FormatName(name)) {}

		[Rule("<FunctionName> ::= SystemFuncId")]
		public FunctionName(SysFunctionIdentifier identifier): this(new KeyValuePair<string, bool>(identifier.Value, true)) {}

		[Rule("<FunctionName> ::= Id")]
		[Rule("<FunctionName> ::= QuotedId")]
		public FunctionName(Identifier identifier): this(identifier.Value) {}

		private FunctionName(KeyValuePair<string, bool> functionName): base(functionName.Key) {
			builtinFunction = functionName.Value;
		}

		public bool IsBuiltinFunction {
			get {
				return builtinFunction;
			}
		}

		protected internal override void WriteToInternal(SqlWriter writer, bool isPartOfQualifiedName) {
			if (IsBuiltinFunction) {
				if ((writer.Engine == DatabaseEngine.SqlAzure) && (Value.Equals("NEWSEQUENTIALID", StringComparison.OrdinalIgnoreCase))) {
					writer.Write("NEWID");
				} else {
					writer.Write(Value);
				}
			} else {
				base.WriteToInternal(writer, true);
			}
		}
	}
}
