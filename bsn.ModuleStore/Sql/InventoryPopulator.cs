// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

using bsn.GoldParser.Xml;
using bsn.ModuleStore.Sql.Definitions;

namespace bsn.ModuleStore.Sql {
	public class InventoryPopulator {
		public void IEnumerable<SqlObject> ProcessScript(XElement doc) {
			foreach (XElement element in doc.Elements("Statement")) {
				XAttribute statementKind = element.Attribute("Kind");
				Debug.Assert(statementKind != null);
				switch (statementKind.Value) {
				case "CreateTable":
					yield return CreateTable(element);
				case "CreateIndex":
					yield return CreateIndex(element);
				case "CreateFulltext":
					yield return CreateFulltextIndex(element);
				case "CreateFunction":
					yield return CreateFunction(element);
				case "CreateProcedure":
					yield return CreateProcedure(element);
				case "CreateView":
					yield return CreateView(element);
				default:
					throw new NotSupportedException("SQL create scripts expected");
				}
			}
		}
	}
}