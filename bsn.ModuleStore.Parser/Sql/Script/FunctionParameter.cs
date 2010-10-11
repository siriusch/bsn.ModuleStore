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

namespace bsn.ModuleStore.Sql.Script {
	public sealed class FunctionParameter: SqlScriptableToken {
		private readonly Literal defaultValue;
		private readonly ParameterName parameterName;
		private readonly Qualified<SchemaName, TypeName> parameterTypeName;

		[Rule("<FunctionParameter> ::= <ParameterName> ~<OptionalAs> <TypeNameQualified> <OptionalDefault>")]
		public FunctionParameter(ParameterName parameterName, Qualified<SchemaName, TypeName> parameterTypeName, Optional<Literal> defaultValue) {
			Debug.Assert(parameterName != null);
			Debug.Assert(parameterTypeName != null);
			this.parameterName = parameterName;
			this.parameterTypeName = parameterTypeName;
			this.defaultValue = defaultValue;
		}

		public Literal DefaultValue {
			get {
				return defaultValue;
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

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(parameterName, WhitespacePadding.SpaceAfter);
			writer.WriteScript(parameterTypeName, WhitespacePadding.None);
			writer.WriteScript(defaultValue, WhitespacePadding.None, "=", null);
		}
	}
}
