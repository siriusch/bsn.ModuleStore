using System;
using System.Collections.Generic;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	public class DependencyResolver {
		private class DependencyNode {
			private readonly HashSet<string> edges = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			private readonly CreateStatement statement;

			public DependencyNode(CreateStatement statement) {
				this.statement = statement;
				foreach (SqlName referencedObjectName in statement.GetReferencedObjectNames()) {
					edges.Add(referencedObjectName.Value);
				}
			}

			public HashSet<string> Edges {
				get {
					return edges;
				}
			}

			public CreateStatement Statement {
				get {
					return statement;
				}
			}
		}

		private readonly SortedList<string, DependencyNode> dependencies = new SortedList<string, DependencyNode>(StringComparer.OrdinalIgnoreCase);

		public void Add(CreateStatement statement) {
			dependencies.Add(statement.ObjectName, new DependencyNode(statement));
		}

		public IEnumerable<CreateStatement> GetInOrder() {
			HashSet<string> resolvedObjects = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			Queue<DependencyNode> nodes = new Queue<DependencyNode>(dependencies.Values);
			int skipCount = 0;
			while (nodes.Count > 0) {
				DependencyNode node = nodes.Dequeue();
				if (resolvedObjects.IsSupersetOf(node.Edges)) {
					resolvedObjects.Add(node.Statement.ObjectName);
					skipCount = 0;
					yield return node.Statement;
				} else {
					nodes.Enqueue(node);
					if (skipCount++ > nodes.Count) {
						throw new InvalidOperationException("Cycle or missing dependency detected");
					}
				}
			}
		}
	}
}
