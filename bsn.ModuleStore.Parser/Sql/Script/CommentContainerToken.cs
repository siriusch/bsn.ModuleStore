using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class CommentContainerToken: SqlScriptableToken {
		private IList<string> comments = new string[0];

		protected void WriteCommentsTo(SqlWriter writer) {
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

		public ICollection<string> Comments {
			get {
				return comments;
			}
		}
	}
}