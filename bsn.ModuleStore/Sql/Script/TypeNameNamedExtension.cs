using System;
using System.IO;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public sealed class TypeNameNamedExtension: TypeNameExtended {
		private readonly SqlIdentifier extension;

		[Rule("<TypeName> ::= Id '(' Id ')'", ConstructorParameterMapping = new[] {0, 2})]
		public TypeNameNamedExtension(SqlIdentifier identifier, SqlIdentifier extension)
				: base(identifier) {
			if (extension == null) {
				throw new ArgumentNullException("extension");
			}
			this.extension = extension;
		}

		protected override void WriteArguments(TextWriter writer) {
			writer.Write(extension.Value);
		}
	}
}