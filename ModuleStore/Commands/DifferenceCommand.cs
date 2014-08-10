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
using System.Drawing;

using bsn.CommandLine;
using bsn.CommandLine.Context;
using bsn.ModuleStore.Sql;
using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Console.Commands {
	[NamedItem("difference", "Shows the difference between the database and the scripts")]
	internal class DifferenceCommand: CommandBase<ExecutionContext> {
		public DifferenceCommand(CommandBase<ExecutionContext> parentCommand): base(parentCommand) {}

		public override void Execute(ExecutionContext executionContext, IDictionary<string, object> tags) {
			DatabaseEngine engine = DatabaseEngine.Unknown;
			Inventory sourceInventory = executionContext.GetInventory((Entities.Source)tags["source"], (bool)tags["directories"]);
			GetDatabaseEngine(sourceInventory, ref engine);
			Inventory targetInventory = executionContext.GetInventory((Entities.Source)tags["target"], (bool)tags["directories"]);
			GetDatabaseEngine(sourceInventory, ref engine);
			using (executionContext.Output.Indent()) {
				foreach (KeyValuePair<IAlterableCreateStatement, InventoryObjectDifference> difference in Inventory.Compare(sourceInventory, targetInventory, engine)) {
					switch (difference.Value) {
					case InventoryObjectDifference.None: executionContext.Output.SetForeground(Color.Gray);
						break;
					case InventoryObjectDifference.SourceOnly:
						executionContext.Output.SetForeground(Color.Lime);
						break;
					case InventoryObjectDifference.TargetOnly:
						executionContext.Output.SetForeground(Color.Red);
						break;
					case InventoryObjectDifference.Different:
						executionContext.Output.SetForeground(Color.Yellow);
						break;
					}
					executionContext.Output.WriteLine("{0} {1}: {2}", difference.Key.ObjectCategory, difference.Key.ObjectName, difference.Value);
				}
			}
		}

		public override IEnumerable<ITagItem<ExecutionContext>> GetCommandTags() {
			yield return new Tag<ExecutionContext, Entities.Source>("source", "The source for the comparison.").SetDefault(context => context.Assembly != null ? Entities.Source.Assembly : Entities.Source.Files);
			yield return new Tag<ExecutionContext, Entities.Source>("target", "The target for the comparison.").SetDefault(context => context.Connected ? Entities.Source.Database : Entities.Source.Files);
			yield return new Tag<ExecutionContext, bool>("directories", "If true, traverse directories when looking for SQL files.").SetDefault(context => false);
		}

		private void GetDatabaseEngine(Inventory sourceInventory, ref DatabaseEngine engine) {
			DatabaseInventory databaseInventory = sourceInventory as DatabaseInventory;
			if (databaseInventory != null) {
				engine = databaseInventory.TargetEngine;
			}
		}
	}
}
