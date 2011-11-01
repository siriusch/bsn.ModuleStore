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
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class CreateFunctionScalarStatement<T>: CreateFunctionStatement<T> where T: SqlScriptableToken {
		private readonly Qualified<SchemaName, TypeName> returnTypeName;

		[Rule("<CreateFunctionStatement> ::= ~CREATE ~FUNCTION <FunctionNameQualified> ~'(' <OptionalFunctionParameterList> ~')' ~RETURNS <TypeNameQualified> <OptionalFunctionOption> ~<OptionalAs> <StatementBlock>", typeof(StatementBlock))]
		[Rule("<CreateFunctionStatement> ::= ~CREATE ~FUNCTION <FunctionNameQualified> ~'(' <OptionalFunctionParameterList> ~')' ~RETURNS <TypeNameQualified> <OptionalFunctionOption> ~<OptionalAs> <ExternalName>", typeof(ExternalName))]
		public CreateFunctionScalarStatement(Qualified<SchemaName, FunctionName> functionName, Optional<Sequence<Parameter>> parameters, Qualified<SchemaName, TypeName> returnTypeName, OptionToken option, T body): base(functionName, parameters, option, body) {
			Debug.Assert(returnTypeName != null);
			this.returnTypeName = returnTypeName;
		}

		public Qualified<SchemaName, TypeName> ReturnTypeName {
			get {
				return returnTypeName;
			}
		}

		protected override void WriteToInternal(SqlWriter writer, string command) {
			base.WriteToInternal(writer, command);
			writer.WriteScript(returnTypeName, WhitespacePadding.None);
			writer.WriteScript(Option, WhitespacePadding.SpaceBefore);
			writer.WriteLine();
			writer.Write("AS");
			writer.IncreaseIndent();
			writer.WriteScript(Body, WhitespacePadding.NewlineBefore);
			writer.DecreaseIndent();
		}
	}
}
