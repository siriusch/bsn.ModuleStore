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

		public virtual void WriteTo(TextWriter writer) {
			throw new NotImplementedException();
		}

		public sealed override string ToString() {
			using (StringWriter writer = CreateWriter()) {
				WriteTo(writer);
				return writer.ToString();
			}
		}
	}
}