using System;
using System.Globalization;
using System.IO;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public abstract class SqlToken: SemanticToken {
		protected static StringWriter CreateWriter() {
			return new StringWriter(CultureInfo.InvariantCulture);
		}

		public override sealed string ToString() {
			using (StringWriter writer = CreateWriter()) {
				WriteTo(writer);
				return writer.ToString();
			}
		}

		public virtual void WriteTo(TextWriter writer) {
			throw new NotImplementedException();
		}
	}
}