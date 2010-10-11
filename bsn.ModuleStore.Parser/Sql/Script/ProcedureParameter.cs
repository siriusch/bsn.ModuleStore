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
using bsn.ModuleStore.Sql.Script.Tokens;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ProcedureParameter: SqlScriptableToken {
		private readonly Literal defaultValue;
		private readonly bool output;
		private readonly ParameterName parameterName;
		private readonly Qualified<SchemaName, TypeName> parameterTypeName;
		private readonly bool readOnly;
		private readonly bool varying;

		[Rule("<ProcedureParameter> ::= <ParameterName> <TypeNameQualified> <OptionalVarying> <OptionalDefault> <OptionalOutput> <OptionalReadonly>")]
		public ProcedureParameter(ParameterName parameterName, Qualified<SchemaName, TypeName> parameterTypeName, Optional<VaryingToken> varying, Optional<Literal> defaultValue, Optional<UnreservedKeyword> output, Optional<Identifier> readonlyIdentifier) {
			Debug.Assert(parameterName != null);
			Debug.Assert(parameterTypeName != null);
			this.parameterName = parameterName;
			this.parameterTypeName = parameterTypeName;
			this.varying = varying.HasValue();
			this.defaultValue = defaultValue;
			this.output = output.HasValue();
			if (readonlyIdentifier.HasValue()) {
				if (string.Equals(readonlyIdentifier.Value.Value, "READONLY", StringComparison.OrdinalIgnoreCase)) {
					readOnly = true;
				} else {
					Debug.Fail("READONLY expected");
				}
			}
		}

		public Literal DefaultValue {
			get {
				return defaultValue;
			}
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

		public Qualified<SchemaName, TypeName> ParameterTypeName {
			get {
				return parameterTypeName;
			}
		}

		public bool ReadOnly {
			get {
				return readOnly;
			}
		}

		public bool Varying {
			get {
				return varying;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(parameterName, WhitespacePadding.None);
			writer.WriteScript(parameterTypeName, WhitespacePadding.SpaceBefore);
			if (varying) {
				writer.Write(" VARYING");
			}
			writer.WriteScript(defaultValue, WhitespacePadding.None, "=", null);
			if (output) {
				writer.Write(" OUTPUT");
			}
			if (readOnly) {
				writer.Write(" READONLY");
			}
		}
	}
}
