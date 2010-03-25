// (C) 2010 Arsène von Wyss / bsn
using System;

namespace bsn.ModuleStore.Sql.Definitions {
	public enum SqlObjectKind {
		None,
		Constraint,
		FulltextIndex,
		Function,
		Index,
		Procedure,
		Table,
		Trigger,
		View,
		XmlSchemaCollection
	}
}