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
	public sealed class XmlDirective: SqlScriptableToken {
		private readonly StringLiteral elementName;
		private readonly Identifier key;
		private readonly Identifier value;

		[Rule("<XmlDirective> ::= Id Id")]
		public XmlDirective(Identifier key, Identifier value): this(key) {
			this.value = value;
		}

		[Rule("<XmlDirective> ::= Id <OptionalElementName>")]
		public XmlDirective(Identifier key, Optional<StringLiteral> elementName): this(key) {
			this.elementName = elementName;
		}

		private XmlDirective(Identifier key) {
			Debug.Assert(key != null);
			this.key = key;
		}

		public StringLiteral ElementName {
			get {
				return elementName;
			}
		}

		public Identifier Key {
			get {
				return key;
			}
		}

		public Identifier Value {
			get {
				return value;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.WriteScript(key, WhitespacePadding.None);
			writer.WriteScript(value, WhitespacePadding.SpaceBefore);
			if (elementName != null) {
				writer.Write('(');
				writer.WriteScript(elementName, WhitespacePadding.None);
				writer.Write(')');
			}
		}
	}
}
