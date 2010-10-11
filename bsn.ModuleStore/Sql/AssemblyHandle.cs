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
using System.IO;
using System.Linq;
using System.Reflection;

namespace bsn.ModuleStore.Sql {
	public class AssemblyHandle: IAssemblyHandle {
		internal static KeyValuePair<T, string>[] FindCustomAttributes<T>(Assembly assembly, Func<Assembly, IEnumerable<T>> forAssembly, Func<MemberInfo, IEnumerable<T>> forMember) {
			List<KeyValuePair<T, string>> result = new List<KeyValuePair<T, string>>();
			string assemblyName = assembly.GetName().Name;
			foreach (T attribute in forAssembly(assembly)) {
				result.Add(new KeyValuePair<T, string>(attribute, assemblyName));
			}
			foreach (Type type in assembly.GetTypes()) {
				string typePrefix = type.Namespace;
				foreach (T attribute in forMember(type)) {
					result.Add(new KeyValuePair<T, string>(attribute, typePrefix));
				}
				foreach (MemberInfo member in type.GetMembers(BindingFlags.Instance|BindingFlags.DeclaredOnly|BindingFlags.Public)) {
					foreach (T attribute in forMember(member)) {
						result.Add(new KeyValuePair<T, string>(attribute, typePrefix));
					}
				}
			}
			return result.ToArray();
		}

		private readonly Assembly assembly;

		public AssemblyHandle(Assembly assembly) {
			if (assembly == null) {
				throw new ArgumentNullException("assembly");
			}
			this.assembly = assembly;
		}

		public IEnumerable<T> GetCustomAttributes<T>(AttributeTargets targets, bool inherit) where T: Attribute {
			if ((targets&AttributeTargets.Assembly) != 0) {}
			if ((targets&
			     (AttributeTargets.Delegate|AttributeTargets.Enum|AttributeTargets.Class|AttributeTargets.Struct|AttributeTargets.Interface|AttributeTargets.Property|AttributeTargets.Field|AttributeTargets.Event|AttributeTargets.Constructor|AttributeTargets.Method|AttributeTargets.Parameter|
			      AttributeTargets.ReturnValue)) != 0) {
				foreach (Type type in assembly.GetTypes()) {
					if (((type.IsEnum) && ((targets&AttributeTargets.Enum) != 0)) || ((type.IsValueType) && ((targets&AttributeTargets.Enum) != 0)) || ((type.IsInterface) && ((targets&AttributeTargets.Enum) != 0)) || ((type.IsSubclassOf(typeof(Delegate))) && ((targets&AttributeTargets.Enum) != 0)) ||
					    ((targets&AttributeTargets.Class) != 0)) {
						foreach (T attribute in type.GetCustomAttributes(typeof(T), inherit)) {
							yield return attribute;
						}
					}
					if (type.IsGenericTypeDefinition && ((targets&AttributeTargets.GenericParameter) != 0)) {
						foreach (Type genericArgument in type.GetGenericArguments()) {
							foreach (T attribute in genericArgument.GetCustomAttributes(typeof(T), inherit)) {
								yield return attribute;
							}
						}
					}
					foreach (MemberInfo member in type.GetMembers()) {
						switch (member.MemberType) {
						case MemberTypes.Method:
							if ((targets&AttributeTargets.ReturnValue) != 0) {
								MethodBase methodBase = (MethodBase)member;
							}
							goto case MemberTypes.Constructor;
						case MemberTypes.Constructor:
							if ((targets&AttributeTargets.Parameter) != 0) {
								MethodBase methodBase = (MethodBase)member;
							}
							goto case MemberTypes.Field;
						case MemberTypes.Field:
						case MemberTypes.Property:
						case MemberTypes.Event:
							foreach (T attribute in member.GetCustomAttributes(typeof(T), inherit)) {
								yield return attribute;
							}
							break;
						}
					}
				}
			}
		}

		public AssemblyName AssemblyName {
			get {
				return assembly.GetName();
			}
		}

		public KeyValuePair<T, string>[] GetCustomAttributes<T>() where T: Attribute {
			return FindCustomAttributes(assembly, a => a.GetCustomAttributes(typeof(T), false).Cast<T>(), m => m.GetCustomAttributes(typeof(T), false).Cast<T>());
		}

		public string[] GetManifestResourceNames() {
			return assembly.GetManifestResourceNames();
		}

		public Stream GetManifestResourceStream(Type type, string streamName) {
			if (type != null) {
				return assembly.GetManifestResourceStream(type, streamName);
			}
			return assembly.GetManifestResourceStream(streamName);
		}
	}
}
