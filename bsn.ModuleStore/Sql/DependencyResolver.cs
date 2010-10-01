using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	public class DependencyResolver {
		private class DependencyNode {
			private readonly HashSet<string> edges = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			private readonly string objectName;
			private readonly Statement statement;

			public DependencyNode(string objectName, Statement statement) {
				this.objectName = objectName;
				this.statement = statement;
				foreach (SqlName referencedObjectName in statement.GetReferencedObjectNames().Where(n => !n.Value.Equals(objectName, StringComparison.OrdinalIgnoreCase))) {
					if (referencedObjectName is FunctionName) {
						Debugger.Break();
					}
					edges.Add(referencedObjectName.Value);
				}
			}

			public HashSet<string> Edges {
				get {
					return edges;
				}
			}

			public string ObjectName {
				get {
					return objectName;
				}
			}

			public Statement Statement {
				get {
					return statement;
				}
			}
		}

		private readonly SortedList<string, DependencyNode> dependencies = new SortedList<string, DependencyNode>(StringComparer.OrdinalIgnoreCase);
		private readonly HashSet<string> existingObjectNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		public int State {
			get {
				return dependencies.Count+existingObjectNames.Count*16384;
			}
		}

		public void Add(CreateStatement statement) {
			if (statement == null) {
				throw new ArgumentNullException("statement");
			}
			Add(statement.ObjectName, statement);
		}

		public void Add(string objectName, Statement statement) {
			if (statement == null) {
				throw new ArgumentNullException("statement");
			}
			if (string.IsNullOrEmpty(objectName)) {
				throw new ArgumentNullException("objectName");
			}
			dependencies.Add(objectName, new DependencyNode(objectName, statement));
		}

		public void AddExistingObject(string objectName) {
			existingObjectNames.Add(objectName);
		}

		public IEnumerable<Statement> GetInOrder(bool throwOnCycle) {
			Queue<DependencyNode> nodes = new Queue<DependencyNode>(dependencies.Values);
			int skipCount = 0;
			while (nodes.Count > 0) {
				DependencyNode node = nodes.Dequeue();
				if (existingObjectNames.IsSupersetOf(node.Edges)) {
					existingObjectNames.Add(node.ObjectName);
					dependencies.Remove(node.ObjectName);
					skipCount = 0;
					StatementBlock block = node.Statement as StatementBlock;
					if (block != null) {
						Stack<IEnumerator<Statement>> blockStack = new Stack<IEnumerator<Statement>>();
						try {
							blockStack.Push(block.Statements.GetEnumerator());
							do {
								IEnumerator<Statement> enumerator = blockStack.Pop();
								if (enumerator.MoveNext()) {
									blockStack.Push(enumerator);
									block = enumerator.Current as StatementBlock;
									if (block != null) {
										blockStack.Push(block.Statements.GetEnumerator());
									} else {
										yield return enumerator.Current;
									}
								} else {
									enumerator.Dispose();
								}
							} while (blockStack.Count > 0);
						} finally {
							while (blockStack.Count > 0) {
								blockStack.Pop().Dispose();
							}
						}
					} else {
						yield return node.Statement;
					}
				} else {
					nodes.Enqueue(node);
					if (skipCount++ > nodes.Count) {
						if (throwOnCycle) {
							StringBuilder unresolvedMsg = new StringBuilder();
							foreach (DependencyNode unresolvedNode in nodes) {
								unresolvedMsg.Length = 0;
								unresolvedMsg.Append(unresolvedNode.ObjectName);
								unresolvedMsg.Append(" seems to depend on ");
								string separator = string.Empty;
								foreach (string edge in unresolvedNode.Edges) {
									unresolvedMsg.Append(separator);
									unresolvedMsg.Append(edge);
									separator = ", ";
								}
								Trace.WriteLine(unresolvedMsg.ToString());
							}
							throw new InvalidOperationException("Cycle or missing dependency detected");
						}
						yield break;
					}
				}
			}
		}
	}
}