using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace bsn.ModuleStore.Sql {
	public class SqlWriter {
		private readonly TextWriter writer;

		public SqlWriter(TextWriter writer) {
			if (writer == null) {
				throw new ArgumentNullException("writer");
			}
			this.writer = writer;
		}
	}
}
