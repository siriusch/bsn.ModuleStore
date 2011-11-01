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
	public sealed class CreateFunctionInlineStatement: CreateFunctionStatement<SelectStatement> {
		[Rule("<CreateFunctionStatement> ::= ~CREATE ~FUNCTION <FunctionNameQualified> ~'(' <OptionalFunctionParameterList> ~')' ~RETURNS ~TABLE <OptionalFunctionOption> ~<OptionalAs> ~RETURN <FunctionInlineSelect>")]
		public CreateFunctionInlineStatement(Qualified<SchemaName, FunctionName> functionName, Optional<Sequence<Parameter>> parameters, OptionToken option, SelectStatement body): base(functionName, parameters, option, body) {}

		protected override void WriteToInternal(SqlWriter writer, string command) {
			base.WriteToInternal(writer, command);
			writer.Write("TABLE");
			writer.WriteScript(Option, WhitespacePadding.SpaceBefore);
			writer.WriteLine();
			writer.Write("AS RETURN (");
			writer.IncreaseIndent();
			writer.WriteScript(Body, WhitespacePadding.NewlineBefore);
			writer.DecreaseIndent();
			writer.WriteLine();
			writer.Write(')');
		}
	}
}
