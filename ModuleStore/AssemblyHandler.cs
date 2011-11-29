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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

using bsn.ModuleStore.Sql;
using bsn.ModuleStore.Sql.Script;

namespace bsn.ModuleStore.Console {
	public class AssemblyHandler: IDisposable, IAssemblyHandle {
		private class ReflectionAssemblyHandle: MarshalByRefObject {
			private readonly Assembly assembly;

			public ReflectionAssemblyHandle(string assemblyFile) {
				assembly = Assembly.ReflectionOnlyLoadFrom(assemblyFile);
			}

			public AssemblyName AssemblyName {
				get {
					return assembly.GetName();
				}
			}

			public KeyValuePair<CustomAttributeInfo, string>[] GetAssemblyCustomAttributeData() {
				// ReSharper disable RedundantTypeArgumentsOfMethod
				return AssemblyHandle.FindCustomAttributes<CustomAttributeInfo>(assembly, CustomAssemblyAttributes, CustomMemberAttributes);
				// ReSharper restore RedundantTypeArgumentsOfMethod
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

			private IEnumerable<CustomAttributeInfo> CustomAssemblyAttributes(Assembly assembly) {
				return CustomAttributeData.GetCustomAttributes(assembly).Select(x => new CustomAttributeInfo(x));
			}

			private IEnumerable<CustomAttributeInfo> CustomMemberAttributes(MemberInfo member) {
				return CustomAttributeData.GetCustomAttributes(member).Select(x => new CustomAttributeInfo(x));
			}
		}

		private static readonly XNamespace asmV1 = "urn:schemas-microsoft-com:asm.v1";
		private static readonly Regex rxAssemblyMatcher = new Regex('^'+Regex.Replace(Regex.Escape(typeof(SqlAssemblyAttribute).Assembly.FullName), @"(?<=\sVersion=)[0-9]+\\\.[0-9]+\\\.[0-9]+\\\.[0-9]+(?=\W)", @"[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+")+'$');

		private static XElement DefineAssemblyCodeBase(AssemblyName name) {
			return new XElement(asmV1+"dependentAssembly", new XElement(asmV1+"assemblyIdentity", new XAttribute("name", name.Name), new XAttribute("publicKeyToken", BitConverter.ToString(name.GetPublicKeyToken()).Replace("-", "").ToLowerInvariant()), new XAttribute("culture", "neutral")),
			                    new XElement(asmV1+"codeBase", new XAttribute("version", name.Version), new XAttribute("href", name.CodeBase)));
		}

		private readonly AppDomain domain;
		private readonly ReflectionAssemblyHandle handle;
		private int disposed;

		public AssemblyHandler(FileInfo assemblyFileName) {
			if (assemblyFileName == null) {
				throw new ArgumentNullException("assemblyFileName");
			}
			AppDomainSetup setupInfo = new AppDomainSetup();
			setupInfo.ApplicationBase = assemblyFileName.DirectoryName;
			XDocument doc =
					new XDocument(new XElement("configuration",
					                           new XElement("runtime", new XElement(asmV1+"assemblyBinding", DefineAssemblyCodeBase(typeof(SqlToken).Assembly.GetName()), DefineAssemblyCodeBase(typeof(SqlAssemblyAttribute).Assembly.GetName()), DefineAssemblyCodeBase(typeof(AssemblyHandler).Assembly.GetName())))));
			using (MemoryStream stream = new MemoryStream()) {
				using (XmlWriter writer = XmlWriter.Create(stream)) {
					doc.WriteTo(writer);
				}
				setupInfo.SetConfigurationBytes(stream.GetBuffer());
				Debug.WriteLine(Encoding.UTF8.GetString(stream.GetBuffer()), "Codebase information");
			}
			AppDomain newDomain = AppDomain.CreateDomain("ModuleStore Assembly Discovery AppDomain", null, setupInfo);
			string typeName = typeof(ReflectionAssemblyHandle).FullName;
			Debug.Assert(!string.IsNullOrEmpty(typeName));
			try {
				string assemblyReflectionLoaderTypeName = typeof(AssemblyReflectionLoader).FullName;
				Debug.Assert(!string.IsNullOrEmpty(assemblyReflectionLoaderTypeName));
				newDomain.CreateInstance(typeof(AssemblyReflectionLoader).Assembly.FullName, assemblyReflectionLoaderTypeName);
				handle = (ReflectionAssemblyHandle)newDomain.CreateInstanceAndUnwrap(typeof(ReflectionAssemblyHandle).Assembly.FullName, typeName, false, BindingFlags.Default, null, new object[] {assemblyFileName.FullName}, null, null, null);
			} catch (Exception ex) {
				Debug.WriteLine(ex);
				handle = null;
				AppDomain.Unload(newDomain);
				throw new InvalidOperationException("Failed to process assembly: "+ex.Message, ex);
			}
			domain = newDomain;
			Debug.WriteLine(typeName, "Loaded into new AppDomain");
		}

		public override string ToString() {
			return handle.AssemblyName.FullName;
		}

		protected virtual void Dispose(bool disposing) {
			if (domain != null) {
				try {
					AppDomain.Unload(domain);
					Debug.WriteLine("Unloaded AppDomain");
				} catch (CannotUnloadAppDomainException) {
					Debug.WriteLine("Unloading AppDomain failed");
					if (disposing) {
						throw; // only propagate the error is we're not in the finalizer
					}
				}
			}
		}

		private void DisposeIfNotDisposed(bool disposing) {
			if (Interlocked.CompareExchange(ref disposed, 1, 0) == 0) {
				Dispose(disposing);
			}
		}

		private object ResolveArgumentValue(KeyValuePair<QualifiedTypeNameInfo, object> a) {
			if (a.Value is QualifiedTypeNameInfo) {
				return ((QualifiedTypeNameInfo)a.Value).FindType(true);
			}
			return a.Value;
		}

		private object[] ResolveArguments(ICollection<KeyValuePair<QualifiedTypeNameInfo, object>> constructorArguments) {
			List<object> result = new List<object>();
			using (IEnumerator<KeyValuePair<QualifiedTypeNameInfo, object>> enumerator = constructorArguments.GetEnumerator()) {
				while (enumerator.MoveNext()) {
					if (enumerator.Current.Key.FindType(true) == typeof(Type)) {
						QualifiedTypeNameInfo typeName = (QualifiedTypeNameInfo)enumerator.Current.Value;
						if (enumerator.MoveNext()) {
							if (enumerator.Current.Key.FindType(true) == typeof(string)) {
								result.Add(null);
								result.Add(Regex.Replace(typeName.TypeName, @"((?<=\.)[^\.]+)?$", (string)enumerator.Current.Value, RegexOptions.CultureInvariant|RegexOptions.ExplicitCapture|RegexOptions.RightToLeft));
							} else {
								result.Add(typeName.FindType(true));
								result.Add(enumerator.Current.Value);
							}
						} else {
							result.Add(typeName.FindType(true));
						}
					} else {
						result.Add(enumerator.Current.Value);
					}
				}
			}
			Debug.Assert(result.Count == constructorArguments.Count);
			return result.ToArray();
		}

		public KeyValuePair<T, string>[] GetCustomAttributes<T>() where T: Attribute {
			KeyValuePair<CustomAttributeInfo, string>[] assemblyCustomAttributeData = handle.GetAssemblyCustomAttributeData();
			if (assemblyCustomAttributeData.Length == 0) {
				return new KeyValuePair<T, string>[0];
			}
			Dictionary<string, Assembly> execAssemblies = new Dictionary<string, Assembly>();
			foreach (Assembly execAssembly in AppDomain.CurrentDomain.GetAssemblies()) {
				execAssemblies.Add(execAssembly.FullName, execAssembly);
			}
			string moduleStoreAssemblyName = typeof(SqlAssemblyAttribute).Assembly.FullName;
			List<KeyValuePair<T, string>> result = new List<KeyValuePair<T, string>>(assemblyCustomAttributeData.Length);
			foreach (KeyValuePair<CustomAttributeInfo, string> customAttributeData in assemblyCustomAttributeData) {
				Assembly execAssembly;
				if (execAssemblies.TryGetValue(rxAssemblyMatcher.Replace(customAttributeData.Key.AttributeType.AssemblyName, moduleStoreAssemblyName), out execAssembly)) {
					Type execAttributeType = execAssembly.GetType(customAttributeData.Key.AttributeType.TypeName);
					if (typeof(T).IsAssignableFrom(execAttributeType)) {
						ConstructorInfo execConstructor = execAttributeType.GetConstructor(customAttributeData.Key.ConstructorArguments.Select(a => a.Key.FindType(true)).ToArray());
						Debug.Assert(execConstructor != null);
						T attribute = (T)execConstructor.Invoke(ResolveArguments(customAttributeData.Key.ConstructorArguments));
						if (customAttributeData.Key.NamedArguments != null) {
							foreach (KeyValuePair<TypeMemberInfo, object> namedArgument in customAttributeData.Key.NamedArguments) {
								MemberInfo execMember = execAttributeType.GetMember(namedArgument.Key.MemberName, namedArgument.Key.MemberType, BindingFlags.Instance|BindingFlags.Public).FirstOrDefault();
								if (execMember != null) {
									switch (execMember.MemberType) {
									case MemberTypes.Property:
										((PropertyInfo)execMember).SetValue(attribute, namedArgument.Value, null);
										break;
									case MemberTypes.Field:
										((FieldInfo)execMember).SetValue(attribute, namedArgument.Value);
										break;
									}
								}
							}
						}
						result.Add(new KeyValuePair<T, string>(attribute, customAttributeData.Value));
					}
				} else {
					Debug.WriteLine(customAttributeData.Key.AttributeType, "Skipping custom attribute");
				}
			}
			return result.ToArray();
		}

		public AssemblyName AssemblyName {
			get {
				return handle.AssemblyName;
			}
		}

		public string[] GetManifestResourceNames() {
			return handle.GetManifestResourceNames();
		}

		public Stream GetManifestResourceStream(Type type, string streamName) {
			return handle.GetManifestResourceStream(type, streamName);
		}

		public void Dispose() {
			GC.SuppressFinalize(this);
			DisposeIfNotDisposed(true);
		}

		~AssemblyHandler() {
			DisposeIfNotDisposed(false);
		}
	}
}
