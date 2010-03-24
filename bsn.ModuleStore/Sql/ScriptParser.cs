// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;

using bsn.GoldParser.Xml;

namespace bsn.ModuleStore.Sql {
	public static class ScriptParser {
		private static readonly GrammarXmlProcessor processor = GrammarXmlProcessor.Create(typeof(InventoryPopulator), "ModuleStoreSQL.cgt", "SqlConverter.xslt");

		public static XElement ParseSql(TextReader reader) {
			if (reader == null) {
				throw new ArgumentNullException("reader");
			}
			XDocument doc = new XDocument();
			ProcessResult processResult;
			using (XmlWriter writer = doc.CreateWriter()) {
				processResult = processor.TryProcess(reader, writer);
			}
			if (!processResult.Success) {
				throw new InvalidOperationException(processResult.ToString());
			}
			Debug.Assert(doc.Root != null);
			return doc.Root;
		}
	}
}