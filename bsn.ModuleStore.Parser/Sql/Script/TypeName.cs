using System;
using System.Collections.Generic;
using System.Linq;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class TypeName: SqlQuotedName {
		private static KeyValuePair<string, bool> FormatName(string name) {
			bool isBuiltIn = ScriptParser.TryGetBuiltinTypeName(ref name);
			return new KeyValuePair<string, bool>(name, isBuiltIn);
		}

		private readonly bool builtinType;

		[Rule("<TypeName> ::= Id")]
		[Rule("<TypeName> ::= QuotedId")]
		public TypeName(SqlIdentifier identifier): this(FormatName(identifier.Value)) {}

		private TypeName(KeyValuePair<string, bool> typeName): base(typeName.Key) {
			builtinType = typeName.Value;
		}

		public bool IsBuiltinType {
			get {
				return builtinType;
			}
		}

		protected internal override void WriteToInternal(SqlWriter writer, bool isPartOfQualifiedName) {
			if (isPartOfQualifiedName || (!IsBuiltinType)) {
				base.WriteToInternal(writer, isPartOfQualifiedName);
			} else {
				writer.Write(Value);
			}
		}
	}
}