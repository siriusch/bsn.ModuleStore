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

using Xunit;

#pragma warning disable 649

namespace bsn.ModuleStore.Mapper.Serialization {
	public class SqlSerializerTest {
		public class Inner {
			[SqlColumn("id")]
			private Guid id;

			public Guid Id {
				get {
					return id;
				}
			}
		}

		public class Outer {
			[SqlDeserialize]
			private Inner inner;

			[SqlColumn("val")]
			private string val;

			public Inner Inner {
				get {
					return inner;
				}
			}

			public string Val {
				get {
					return val;
				}
			}
		}

		[Fact]
		public void Mocking() {
			var data = new {
				uidAssemblyGuid = Guid.NewGuid(),
				uidModule = Guid.NewGuid(),
				sSchema = "ModuleStore",
				fSchemaExists = true,
				dtSetup = DateTime.UtcNow,
				binSetupHash = new byte[20],
				dtUpdate = DateTime.UtcNow.AddHours(-1),
				iUpdateVersion = 1
			};
			Module module = SqlDeserializer<Module>.Mock(data);
			Assert.Equal(data.uidAssemblyGuid, module.AssemblyGuid);
			Assert.Equal(data.uidModule, module.Id);
			Assert.Equal(data.sSchema, module.Schema);
			Assert.Equal(data.fSchemaExists, module.SchemaExists);
			Assert.Equal(data.dtSetup, module.SetupDate);
			Assert.Equal(data.dtUpdate, module.UpdateDate);
			Assert.Equal(data.iUpdateVersion, module.UpdateVersion);
		}

		[Fact]
		public void MockingNestedDeserialize() {
			var data = new {
				id = Guid.NewGuid(),
				val = "Value"
			};
			Outer outer = SqlDeserializer<Outer>.Mock(data);
			Assert.Equal(data.id, outer.Inner.Id);
			Assert.Equal(data.val, outer.Val);
		}
	}
}

#pragma warning restore 649
