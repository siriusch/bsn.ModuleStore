using System;

using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class Optional<T>: SqlToken where T: SqlToken {
		public static implicit operator T(Optional<T> instance) {
			if (instance != null) {
				return instance.Value;
			}
			return null;
		}

		private readonly T value;

		[Rule("<FulltextColumnType> ::=", typeof(Qualified<TypeName>))]
		[Rule("<OptionalCollate> ::=", typeof(CollationName))]
		[Rule("<OptionalLanguage> ::=", typeof(LanguageLcid))]
		[Rule("<OptionalDefault> ::=", typeof(Literal))]
		[Rule("<OptionalOpenxmlSchema> ::=", typeof(OpenxmlSchema))]
		[Rule("<FulltextColumnGroup> ::=", typeof(Sequence<FulltextColumn>))]
		[Rule("<OptionalFulltextChangeTracking> ::=", typeof(FulltextChangeTracking))]
		public Optional()
			: this(null) {
		}

		[Rule("<FulltextColumnType> ::= TYPE_COLUMN <TypeNameQualified>", typeof(Qualified<TypeName>), ConstructorParameterMapping = new[] {1})]
		[Rule("<OptionalCollate> ::= COLLATE <CollationName>", typeof(CollationName), ConstructorParameterMapping = new[] {1})]
		[Rule("<OptionalLanguage> ::= LANGUAGE_LCID", typeof(LanguageLcid))]
		[Rule("<OptionalDefault> ::= '=' <Literal>", typeof(Literal), ConstructorParameterMapping = new[] {1})]
		[Rule("<OptionalOpenxmlSchema> ::= <OpenxmlImplicitSchema>", typeof(OpenxmlSchema))]
		[Rule("<OptionalOpenxmlSchema> ::= <OpenxmlExplicitSchema>", typeof(OpenxmlSchema))]
		[Rule("<FulltextColumnGroup> ::= '(' <FulltextColumnList> ')'", typeof(Sequence<FulltextColumn>), ConstructorParameterMapping = new[] {1})]
		[Rule("<OptionalFulltextChangeTracking> ::= <FulltextChangeTracking>", typeof(FulltextChangeTracking))]
		public Optional(T value) {
			this.value = value;
		}

		public bool HasValue {
			get {
				return value != null;
			}
		}

		public T Value {
			get {
				return value;
			}
		}

		public override void WriteTo(System.IO.TextWriter writer) {
			throw new NotSupportedException();
		}
	}
}
