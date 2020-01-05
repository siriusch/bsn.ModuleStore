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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Sql {
	public class DependencyResolver {
		private class DependencyNode {
			private static bool IsLocalName(SqlName name) {
				if (name is TableName tableName) {
					return tableName.IsTempTable;
				}
				return name is VariableName;
			}

			private readonly HashSet<string> edges = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			private readonly string objectName;
			private readonly IInstallStatement statement;

			public DependencyNode(string objectName, IInstallStatement statement) {
				this.objectName = objectName;
				this.statement = statement;
				foreach (var referencedObjectName in statement.GetReferencedObjectNames<SqlName>().Where(n => !(IsLocalName(n) || n.Value.Equals(objectName, StringComparison.OrdinalIgnoreCase)))) {
					edges.Add(referencedObjectName.Value);
				}
			}

			public HashSet<string> Edges => edges;

			public string ObjectName => objectName;

			public IInstallStatement Statement => statement;
		}

		private readonly SortedList<string, List<DependencyNode>> dependencies = new SortedList<string, List<DependencyNode>>(StringComparer.OrdinalIgnoreCase);
		private readonly HashSet<string> existingObjectNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		public int State => dependencies.Count+existingObjectNames.Count*16384;

		public void Add(IInstallStatement statement) {
			if (statement == null) {
				throw new ArgumentNullException(nameof(statement));
			}
			if (!statement.ObjectName.StartsWith("@")) {
				if (!dependencies.TryGetValue(statement.ObjectName, out var dependencyNodes)) {
					dependencyNodes = new List<DependencyNode>();
					dependencies.Add(statement.ObjectName, dependencyNodes);
				}
				dependencyNodes.Add(new DependencyNode(statement.ObjectName, statement));
			}
		}

		public void AddExistingObject(string objectName) {
			existingObjectNames.Add(objectName);
		}

		public IEnumerable<IInstallStatement> GetInOrder(bool throwOnCycle) {
			var nodes = new Queue<DependencyNode>(dependencies.Values.SelectMany(n => n).OrderBy(n => n.ObjectName, StringComparer.OrdinalIgnoreCase));
			// we start with obvious "direct dependencies"
			var directDependencies = GetDirectDependencies(nodes, n => existingObjectNames.Contains(n.Value));
			var skipCount = 0;
			while (nodes.Count > 0) {
				var node = nodes.Dequeue();
				if (((directDependencies.Count == 0) || (directDependencies.Contains(node))) && CheckDependenciesExist(node)) {
					RemoveDependency(node);
					skipCount = 0;
					if (node.Statement.IsPartOfSchemaDefinition) {
						directDependencies.UnionWith(GetDirectDependencies(nodes, n => n.Value.Equals(node.ObjectName, StringComparison.OrdinalIgnoreCase)));
					}
					yield return node.Statement;
					directDependencies.IntersectWith(nodes);
				} else {
					nodes.Enqueue(node);
					if (skipCount++ > nodes.Count) {
						if (throwOnCycle) {
							var unresolvedMsg = new StringBuilder();
							foreach (var unresolvedNode in nodes) {
								unresolvedMsg.Length = 0;
								unresolvedMsg.Append(unresolvedNode.ObjectName);
								unresolvedMsg.Append(" seems to depend on ");
								var separator = string.Empty;
								foreach (var edge in unresolvedNode.Edges) {
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

		internal void TransferPendingObjects(DependencyResolver other) {
			foreach (var statement in dependencies.SelectMany(p => p.Value).Select(n => n.Statement).Where(s => !existingObjectNames.Contains(s.ObjectName)).Distinct()) {
				other.Add(statement);
			}
		}

		private bool CheckDependenciesExist(DependencyNode node) {
			var effectiveExistingObjectNames = new HashSet<string>(existingObjectNames, existingObjectNames.Comparer);
			effectiveExistingObjectNames.ExceptWith(dependencies.Keys);
			return effectiveExistingObjectNames.IsSupersetOf(node.Edges);
		}

		private IEnumerable<DependencyNode> GetAllDependencies(DependencyNode node) {
			var result = new HashSet<DependencyNode>();
			var queue = new Queue<DependencyNode>();
			queue.Enqueue(node);
			do {
				var dependencyNode = queue.Dequeue();
				if (result.Add(dependencyNode)) {
					foreach (var edge in dependencyNode.Edges) {
						if (dependencies.TryGetValue(edge, out var dependencyNodes)) {
							foreach (var innerDependency in dependencyNodes) {
								queue.Enqueue(innerDependency);
							}
						}
					}
				}
			} while (queue.Count > 0);
			return result;
		}

		private HashSet<DependencyNode> GetDirectDependencies(IEnumerable<DependencyNode> nodes, Func<SqlName, bool> isNameMatch) {
			// in order to avoid trouble with indexes in queries (such as with the (NOEXPAND) hint) we process indexes and table modifications in a prioritized manner
			var result = new HashSet<DependencyNode>();
			foreach (var dependencyNode in nodes) {
				switch (dependencyNode.Statement) {
				case CreateIndexStatement createIndex when isNameMatch(createIndex.TableName.Name):
				case AlterTableStatement alterTable when isNameMatch(alterTable.TableName.Name):
					result.UnionWith(GetAllDependencies(dependencyNode));
					break;
				}
			}
			return result;
		}

		private void RemoveDependency(DependencyNode node) {
			var dependencyNodes = dependencies[node.ObjectName];
			dependencyNodes.Remove(node);
			if (dependencyNodes.Count == 0) {
				dependencies.Remove(node.ObjectName);
				existingObjectNames.Add(node.ObjectName);
			}
		}
	}
}
