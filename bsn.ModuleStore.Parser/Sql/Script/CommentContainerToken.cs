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
using System.Collections.Generic;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class CommentContainerToken: SqlScriptableToken {
		private IList<string> comments = new string[0];

		public ICollection<string> Comments {
			get {
				return comments;
			}
		}

		protected internal void WriteCommentsTo(SqlWriter writer) {
			for (int i = 0; i < comments.Count; i++) {
				writer.WriteComment(comments[i]);
			}
		}

		internal void AddComments(IList<string> comments) {
			if (comments == null) {
				throw new ArgumentNullException("comments");
			}
			if (comments.Count > 0) {
				if (this.comments.Count > 0) {
					foreach (string comment in comments) {
						this.comments.Add(comment);
					}
				} else {
					this.comments = comments;
				}
			}
		}
	}
}
