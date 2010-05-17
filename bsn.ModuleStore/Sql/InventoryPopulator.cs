// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Xml.Linq;

using bsn.GoldParser.Xml;
using bsn.ModuleStore.Sql.Definitions;

namespace bsn.ModuleStore.Sql {
	public class InventoryPopulator {
		public IEnumerable<SqlObject> ProcessScript(XElement doc) {
			foreach (XElement element in doc.Elements("Statement")) {
				XAttribute statementKind = element.Attribute("Kind");
				Debug.Assert(statementKind != null);
				switch (statementKind.Value) {
				case "CreateTable":
					yield return CreateTable(element);
					break;
				case "CreateIndex":
					yield return CreateIndex(element);
					break;
				case "CreateFulltext":
					yield return CreateFulltextIndex(element);
					break;
				case "CreateFunction":
					yield return CreateFunction(element);
					break;
				case "CreateProcedure":
					yield return CreateProcedure(element);
					break;
				case "CreateView":
					yield return CreateView(element);
					break;
				default:
					throw new NotSupportedException("Only SQL create statements expected");
				}
			}
		}

		private View CreateView(XElement element) {
			View view = new View(null);
			return view;
		}

		private Procedure CreateProcedure(XElement element) {
			Procedure procedure = new Procedure(null);
			return procedure;
		}

		private Function CreateFunction(XElement element) {
			Function function = new Function(null);
			return function;
		}

		private FulltextIndex CreateFulltextIndex(XElement element) {
			FulltextIndex fulltextIndex = new FulltextIndex(null);
			return fulltextIndex;
		}

		private Index CreateIndex(XElement element) {
			Index index = new Index(null);
			return index;
		}

		private Table CreateTable(XElement element) {
			Table table = new Table(null);

			return table;
		}
	}
}