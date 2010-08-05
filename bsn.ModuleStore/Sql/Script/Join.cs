using System;
using System.Diagnostics;

using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;

namespace bsn.ModuleStore.Sql.Script {
	public class Join: SqlToken {
		private readonly SourceRowset joinRowset;
		private readonly JoinKind kind;
		private readonly Predicate predicate;

		[Rule("<Join> ::= CROSS JOIN <SourceRowset>", ConstructorParameterMapping = new[] {2})]
		public Join(SourceRowset joinRowset): this(joinRowset, null) {
			kind = JoinKind.Cross;
		}

		[Rule("<Join> ::= JOIN <SourceRowset> ON <Predicate>", ConstructorParameterMapping = new[] {1, 3})]
		[Rule("<Join> ::= INNER JOIN <SourceRowset> ON <Predicate>", ConstructorParameterMapping = new[] {2, 4})]
		public Join(SourceRowset joinRowset, Predicate predicate) {
			if (joinRowset == null) {
				throw new ArgumentNullException("joinRowset");
			}
			this.predicate = predicate;
			this.joinRowset = joinRowset;
			kind = JoinKind.Inner;
		}

		[Rule("<Join> ::= LEFT JOIN <SourceRowset> ON <Predicate>", ConstructorParameterMapping = new[] {0, 2, 4})]
		[Rule("<Join> ::= LEFT OUTER JOIN <SourceRowset> ON <Predicate>", ConstructorParameterMapping = new[] {0, 3, 5})]
		[Rule("<Join> ::= RIGHT JOIN <SourceRowset> ON <Predicate>", ConstructorParameterMapping = new[] {0, 2, 4})]
		[Rule("<Join> ::= RIGHT OUTER JOIN <SourceRowset> ON <Predicate>", ConstructorParameterMapping = new[] {0, 3, 5})]
		[Rule("<Join> ::= FULL JOIN <SourceRowset> ON <Predicate>", ConstructorParameterMapping = new[] {0, 2, 4})]
		[Rule("<Join> ::= FULL OUTER JOIN <SourceRowset> ON <Predicate>", ConstructorParameterMapping = new[] {0, 3, 5})]
		public Join(IToken outerKind, SourceRowset joinRowset, Predicate predicate): this(joinRowset, predicate) {
			if (outerKind == null) {
				throw new ArgumentNullException("outerKind");
			}
			switch (outerKind.Symbol.Name) {
			case "LEFT":
				kind = JoinKind.Left;
				break;
			case "RIGHT":
				kind = JoinKind.Right;
				break;
			case "FULL":
				kind = JoinKind.Full;
				break;
			default:
				Debug.Fail("Invalid join type");
				break;
			}
		}

		public SourceRowset JoinRowset {
			get {
				return joinRowset;
			}
		}

		public JoinKind Kind {
			get {
				return kind;
			}
		}

		public Predicate Predicate {
			get {
				return predicate;
			}
		}
	}
}