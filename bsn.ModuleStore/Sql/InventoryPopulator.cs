// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

using bsn.GoldParser.Xml;

namespace bsn.ModuleStore.Sql {
	public class InventoryPopulator {
		public void ProcessScript(XElement doc) {
			foreach (XElement element in doc.Elements("Statement")) {
				XAttribute statementKind = element.Attribute("Kind");
				Debug.Assert(statementKind != null);
				switch (statementKind.Value) {
				case "CreateTable":
				case "AlterTable":
				case "CreateIndex":
				case "CreateFulltext":
				case "CreateFunction":
				case "CreateProcedure":
				case "CreateView":
				}
			}
		}
	}
}