using System;
using System.Diagnostics;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TypeNameNamedExtension: TypeNameExtended {
		private readonly SqlIdentifier extension;

		[Rule("<TypeName> ::= Id '(' Id ')'", ConstructorParameterMapping = new[] {0, 2})]
		public TypeNameNamedExtension(SqlIdentifier identifier, SqlIdentifier extension): base(identifier) {
			Debug.Assert(extension != null);
			this.extension = extension;
		}

		public SqlIdentifier Extension {
			get {
				return extension;
			}
		}

		protected override void WriteArguments(SqlWriter writer) {
			writer.Write(extension.Value);
		}
	}
}