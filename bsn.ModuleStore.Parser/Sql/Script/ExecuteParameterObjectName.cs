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
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class ExecuteParameterObjectName: ExecuteParameter<Qualified<SchemaName, ObjectName>> {
		private static readonly Regex rxColumnName = new Regex(@"^\s*(?<object>([a-z][a-z0-9_]*|\[[^\]]+\]|""[^""]+"")s*\.\s*([a-z][a-z0-9_]*|\[[^\]]+\]|""[^""]+""))s*\.\s*(?<column>[a-z][a-z0-9_]*|\[[^\]]+\]|""[^""]+"")\s*$",
		                                                       RegexOptions.Compiled|RegexOptions.CultureInvariant|RegexOptions.ExplicitCapture|RegexOptions.IgnoreCase);

		private readonly string columnName;
		private readonly bool unicodeObjectName;

		public ExecuteParameterObjectName(ParameterName parameterName, StringLiteral objectName, bool output): this(parameterName, objectName, rxColumnName.Match(objectName.Value), output) {}

		private ExecuteParameterObjectName(ParameterName parameterName, StringLiteral objectName, Match columnMatch, bool output): base(parameterName, ScriptParser.ParseObjectName(columnMatch.Success ? columnMatch.Groups["object"].Value : objectName.Value), output) {
			unicodeObjectName = objectName.IsUnicode;
			columnName = columnMatch.Groups["column"].Value;
		}

		protected override void WriteValueTo(SqlWriter writer) {
			using (StringWriter nameWriter = new StringWriter(CultureInfo.InvariantCulture)) {
				Value.WriteTo(new SqlWriter(nameWriter, writer.Engine, SqlWriterMode.NoComments));
				if (!string.IsNullOrEmpty(columnName)) {
					nameWriter.Write('.');
					nameWriter.Write(columnName);
				}
				new StringLiteral(nameWriter.ToString(), unicodeObjectName, null).WriteTo(writer);
			}
		}
	}
}
