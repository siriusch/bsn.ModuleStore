// bsn ModuleStore database versioning
// -----------------------------------
// 
// Copyright 2010 by Arsène von Wyss - avw@gmx.ch
// 
// Development has been supported by Sirius Technologies AG, Basel
// 
// Source:
// 
// https://bsn-modulestore.googlecode.com/hg/
// 
// License:
// 
// The library is distributed under the GNU Lesser General Public License:
// http://www.gnu.org/licenses/lgpl.html
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//  
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
				foreach (SqlName referencedObjectName in statement.GetReferencedObjectNames<SqlName>().Where(n => !(n.Value.StartsWith("@") || n.Value.Equals(objectName, StringComparison.OrdinalIgnoreCase)))) {
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

		private readonly SortedList<string, List<DependencyNode>> dependencies = new SortedList<string, List<DependencyNode>>(StringComparer.OrdinalIgnoreCase);
		private readonly HashSet<string> existingObjectNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		public int State {
			get {
				return dependencies.Count+existingObjectNames.Count*16384;
			}
		}

		public void Add<T>(T statement) where T: Statement, IObjectBoundStatement {
			if (statement == null) {
				throw new ArgumentNullException("statement");
			}
			Add(statement.ObjectName, statement);
		}

		public void Add(string objectName, Statement statement) {
			AddDependency(objectName, statement);
		}

		public void AddExistingObject(string objectName) {
			existingObjectNames.Add(objectName);
		}

		public IEnumerable<Statement> GetInOrder(bool throwOnCycle) {
			Queue<DependencyNode> nodes = new Queue<DependencyNode>(dependencies.Values.SelectMany(n => n));
			// we start with obvious "direct dependencies"
			HashSet<DependencyNode> directDependencies = GetDirectDependencies(nodes, n => existingObjectNames.Contains(n.Value));
			int skipCount = 0;
			while (nodes.Count > 0) {
				DependencyNode node = nodes.Dequeue();
				if (((directDependencies.Count == 0) || (directDependencies.Contains(node))) && CheckDependenciesExist(node)) {
					RemoveDependency(node);
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
						CreateStatement createStatement = node.Statement as CreateStatement;
						if ((createStatement is CreateViewStatement) || (createStatement is CreateTableStatement)) {
							directDependencies.UnionWith(GetDirectDependencies(nodes, n => n.Value.Equals(node.ObjectName, StringComparison.OrdinalIgnoreCase)));
						}
						yield return node.Statement;
					}
					directDependencies.IntersectWith(nodes);
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

		private void AddDependency(string objectName, Statement statement) {
			if (statement == null) {
				throw new ArgumentNullException("statement");
			}
			if (string.IsNullOrEmpty(objectName)) {
				throw new ArgumentNullException("objectName");
			}
			if (!objectName.StartsWith("@")) {
				List<DependencyNode> dependencyNodes;
				if (!dependencies.TryGetValue(objectName, out dependencyNodes)) {
					dependencyNodes = new List<DependencyNode>();
					dependencies.Add(objectName, dependencyNodes);
				}
				dependencyNodes.Add(new DependencyNode(objectName, statement));
			}
		}

		private bool CheckDependenciesExist(DependencyNode node) {
			HashSet<string> effectiveExistingObjectNames = new HashSet<string>(existingObjectNames, existingObjectNames.Comparer);
			effectiveExistingObjectNames.ExceptWith(dependencies.Keys);
			return effectiveExistingObjectNames.IsSupersetOf(node.Edges);
		}

		private IEnumerable<DependencyNode> GetAllDependencies(DependencyNode node) {
			HashSet<DependencyNode> result = new HashSet<DependencyNode>();
			Queue<DependencyNode> queue = new Queue<DependencyNode>();
			queue.Enqueue(node);
			do {
				DependencyNode dependencyNode = queue.Dequeue();
				if (result.Add(dependencyNode)) {
					foreach (string edge in dependencyNode.Edges) {
						List<DependencyNode> dependencyNodes;
						if (dependencies.TryGetValue(edge, out dependencyNodes)) {
							foreach (DependencyNode innerDependency in dependencyNodes) {
								queue.Enqueue(innerDependency);
							}
						}
					}
				}
			} while (queue.Count > 0);
			return result;
		}

		private HashSet<DependencyNode> GetDirectDependencies(IEnumerable<DependencyNode> nodes, Func<SqlName, bool> isNameMatch) {
			// in order to avoid trouble with indexes in queries (such as with the (NOEXPAND) hint) we process indexes and table modifiations in a prioritized manner
			HashSet<DependencyNode> result = new HashSet<DependencyNode>();
			foreach (DependencyNode dependencyNode in nodes) {
				CreateIndexStatement createIndex = dependencyNode.Statement as CreateIndexStatement;
				if ((createIndex != null) && isNameMatch(createIndex.TableName.Name)) {
					result.UnionWith(GetAllDependencies(dependencyNode));
				} else {
					AlterTableStatement alterTable = dependencyNode.Statement as AlterTableStatement;
					if ((alterTable != null) && isNameMatch(alterTable.TableName.Name)) {
						result.UnionWith(GetAllDependencies(dependencyNode));
					}
				}
			}
			return result;
		}

		private void RemoveDependency(DependencyNode node) {
			List<DependencyNode> dependencyNodes = dependencies[node.ObjectName];
			dependencyNodes.Remove(node);
			if (dependencyNodes.Count == 0) {
				dependencies.Remove(node.ObjectName);
				existingObjectNames.Add(node.ObjectName);
			}
		}
	}
}