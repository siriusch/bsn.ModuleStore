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

using bsn.GoldParser.Semantic;
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ProcedureParameter: Parameter {
		private readonly bool output;
		private readonly bool varying;

		[Rule("<ProcedureParameter> ::= <ParameterName> <TypeNameQualified> <OptionalVarying> <OptionalDefault> <OptionalOutput> <OptionalReadonly>")]
		public ProcedureParameter(ParameterName parameterName, Qualified<SchemaName, TypeName> parameterTypeName, Optional<VaryingToken> varying, Optional<Literal> defaultValue, Optional<UnreservedKeyword> output, Optional<UnreservedKeyword> readOnly)
				: base(parameterName, parameterTypeName, defaultValue, readOnly) {
			this.varying = varying.HasValue();
			this.output = output.HasValue();
		}

		public bool Output {
			get {
				return output;
			}
		}

		public bool Varying {
			get {
				return varying;
			}
		}

		protected override void WriteParameterQualifiers(SqlWriter writer) {
			if (varying) {
				writer.Write(" VARYING");
			}
			base.WriteParameterQualifiers(writer);
			if (output) {
				writer.Write(" OUTPUT");
			}
		}
	}
}
