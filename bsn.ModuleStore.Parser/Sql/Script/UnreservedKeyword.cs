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

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	[Terminal("ABSOLUTE")]
	[Terminal("ACTION")]
	[Terminal("AFTER")]
	[Terminal("APPLY")]
	[Terminal("AUTO")]
	[Terminal("CALLED")]
	[Terminal("CALLER")]
	[Terminal("CAST")]
	[Terminal("CATCH")]
	[Terminal("CHANGE_TRACKING")]
	[Terminal("COLLECTION")]
	[Terminal("COMMITTED")]
	[Terminal("COUNT")]
	[Terminal("DISABLE")]
	[Terminal("ENABLE")]
	[Terminal("EXPLICIT")]
	[Terminal("EXTERNAL")]
	[Terminal("FIRST")]
	[Terminal("FULLTEXT")]
	[Terminal("GLOBAL")]
	[Terminal("HASH")]
	[Terminal("INCLUDE")]
	[Terminal("INPUT")]
	[Terminal("INSTEAD")]
	[Terminal("LANGUAGE")]
	[Terminal("LAST")]
	[Terminal("LOG")]
	[Terminal("LOOP")]
	[Terminal("MANUAL")]
	[Terminal("MARK")]
	[Terminal("MATCHED")]
	[Terminal("MAXRECURSION")]
	[Terminal("NAME")]
	[Terminal("NEXT")]
	[Terminal("NO")]
	[Terminal("NOWAIT")]
	[Terminal("ONLY")]
	[Terminal("OUTPUT")]
	[Terminal("PARTITION")]
	[Terminal("PATH")]
	[Terminal("PERSISTED")]
	[Terminal("POPULATION")]
	[Terminal("PRECISION")]
	[Terminal("PRIOR")]
	[Terminal("PROPERTY")]
	[Terminal("RAW")]
	[Terminal("READONLY")]
	[Terminal("RECOMPILE")]
	[Terminal("RELATIVE")]
	[Terminal("REPEATABLE")]
	[Terminal("RETURNS")]
	[Terminal("SCHEMABINDING")]
	[Terminal("SERVER")]
	[Terminal("SETERROR")]
	[Terminal("SOURCE")]
	[Terminal("TARGET")]
	[Terminal("TIES")]
	[Terminal("TRY")]
	[Terminal("TYPE")]
	[Terminal("UNCOMMITTED")]
	[Terminal("USING")]
	[Terminal("VALUE")]
	[Terminal("VIEW_METADATA")]
	[Terminal("WORK")]
	[Terminal("XML")]
	[Terminal("XMLNAMESPACES")]
	public class UnreservedKeyword: KeywordToken {
		public UnreservedKeyword(string text): base(text) {}

		public Identifier AsIdentifier(Symbol identifierSymbol) {
			return new Identifier(GetOriginalValue(), identifierSymbol, ((IToken)this).Position);
		}
	}
}
