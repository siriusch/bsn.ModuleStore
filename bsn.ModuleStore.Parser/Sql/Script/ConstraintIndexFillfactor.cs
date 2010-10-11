﻿// bsn ModuleStore database versioning
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

namespace bsn.ModuleStore.Sql.Script {
#warning Maybe create a base class for the (now only implicitly implemented) <IndexOptionGroup> and inherit from it
	public sealed class ConstraintIndexFillfactor: ConstraintIndex {
		private readonly IntegerLiteral fillfactor;

		[Rule("<ConstraintIndex> ::= ~WITH ~FILLFACTOR ~'=' <IntegerLiteral>")]
		public ConstraintIndexFillfactor(IntegerLiteral fillfactor) {
			this.fillfactor = fillfactor;
		}

		public IntegerLiteral Fillfactor {
			get {
				return fillfactor;
			}
		}

		public override bool HasValue {
			get {
				return true;
			}
		}

		public override void WriteTo(SqlWriter writer) {
			writer.Write("WITH FILLFACTOR=");
			writer.WriteScript(fillfactor, WhitespacePadding.None);
		}
	}
}
