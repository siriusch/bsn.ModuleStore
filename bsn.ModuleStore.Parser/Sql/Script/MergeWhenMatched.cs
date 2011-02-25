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
	public class MergeWhenMatched: CommentContainerToken {
		private readonly MergeOperation operation;
		private readonly Predicate predicate;

		[Rule("<MergeWhenMatched> ::= ~WHEN ~MATCHED ~THEN <MergeMatched>")]
		public MergeWhenMatched(MergeOperation operation): this(null, operation) {}

		[Rule("<MergeWhenMatched> ::= ~WHEN ~MATCHED ~AND <Predicate> ~THEN <MergeMatched>")]
		public MergeWhenMatched(Predicate predicate, MergeOperation operation) {
			Debug.Assert(operation != null);
			this.predicate = predicate;
			this.operation = operation;
		}

		public MergeOperation Operation {
			get {
				return operation;
			}
		}

		public Predicate Predicate {
			get {
				return predicate;
			}
		}

		public override sealed void WriteTo(SqlWriter writer) {
			WriteCommentsTo(writer);
			writer.IncreaseIndent();
			writer.Write("WHEN ");
			WriteMatchedTo(writer);
			writer.WriteLine(" THEN");
			writer.WriteScript(operation, WhitespacePadding.None);
			writer.DecreaseIndent();
		}

		protected virtual void WriteMatchedTo(SqlWriter writer) {
			writer.Write("MATCHED");
			writer.WriteScript(predicate, WhitespacePadding.None, " AND ", "");
		}
	}
}
