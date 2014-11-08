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
	public abstract class ExecuteParameter: SqlScriptableToken {}

	public class ExecuteParameter<T>: ExecuteParameter where T: SqlScriptableToken {
		private readonly bool output;
		private readonly ParameterName parameterName;
		private readonly T value;

		[Rule("<ExecuteParameter> ::= <VariableName> <OptionalOutput>", typeof(VariableName))]
		[Rule("<ExecuteParameter> ::= <SystemVariableName> <OptionalOutput>", typeof(VariableName))]
		[Rule("<ExecuteParameter> ::= <Literal> <OptionalOutput>", typeof(Literal))]
		public ExecuteParameter(T value, Optional<UnreservedKeyword> output): this(null, value, output.HasValue()) {}

		[Rule("<ExecuteParameter> ::= <ParameterName> ~'=' <VariableName> <OptionalOutput>", typeof(VariableName))]
		[Rule("<ExecuteParameter> ::= <ParameterName> ~'=' <SystemVariableName> <OptionalOutput>", typeof(VariableName))]
		[Rule("<ExecuteParameter> ::= <ParameterName> ~'=' <Literal> <OptionalOutput>", typeof(Literal))]
		public ExecuteParameter(ParameterName parameterName, T value, Optional<UnreservedKeyword> output): this(parameterName, value, output.HasValue()) {}

		protected ExecuteParameter(ParameterName parameterName, T value, bool output) {
			Debug.Assert(value != null);
			this.parameterName = parameterName;
			this.value = value;
			this.output = output;
		}

		public bool Output {
			get {
				return output;
			}
		}

		public ParameterName ParameterName {
			get {
				return parameterName;
			}
		}

		public T Value {
			get {
				return value;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(ParameterName, WhitespacePadding.None, null, w => w.Write('='));
			WriteValueTo(writer);
			if (Output) {
				writer.WriteKeyword(" OUTPUT");
			}
		}

		protected virtual void WriteValueTo(SqlWriter writer) {
			writer.WriteScript(value, WhitespacePadding.None);
		}
	}
}
