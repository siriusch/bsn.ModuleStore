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
using System.Linq;

namespace bsn.ModuleStore.Mapper {
	public abstract class InstanceManager<TId, TManager> where TId: struct, IEquatable<TId> where TManager: InstanceManager<TId, TManager> {
		private readonly ManagedInstanceProvider<TId, TManager> provider;

		protected InstanceManager(ManagedInstanceProvider<TId, TManager> provider) {
			this.provider = new ManagedInstanceProvider<TId, TManager>((TManager)this);
		}

		protected ManagedInstanceProvider<TId, TManager> Provider {
			get {
				return provider;
			}
		}
	}
}