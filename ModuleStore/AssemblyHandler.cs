using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

using bsn.ModuleStore.Sql;

namespace bsn.ModuleStore.Console {
	public class AssemblyHandler: IDisposable, IAssemblyHandle {
		private class ReflectionAssemblyHandle: MarshalByRefObject {
			private readonly Assembly assembly;

			public ReflectionAssemblyHandle(string assemblyFile): base() {
				assembly = Assembly.ReflectionOnlyLoadFrom(assemblyFile);
			}

			public AssemblyName AssemblyName {
				get {
					return assembly.GetName();
				}
			}

			public KeyValuePair<CustomAttributeData, string>[] GetAssemblyCustomAttributeData() {
				return AssemblyHandle.FindCustomAttributes<CustomAttributeData>(assembly, CustomAttributeData.GetCustomAttributes, CustomAttributeData.GetCustomAttributes);
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

		private readonly AppDomain domain;
		private readonly ReflectionAssemblyHandle handle;
		private int disposed;

		public AssemblyHandler(FileInfo assemblyFileName) {
			if (assemblyFileName == null) {
				throw new ArgumentNullException("assemblyFileName");
			}
			AppDomain newDomain = AppDomain.CreateDomain("ModuleStore Assembly Discovery AppDomain");
			string typeName = typeof(ReflectionAssemblyHandle).FullName;
			Debug.Assert(!string.IsNullOrEmpty(typeName));
			try {
				string assemblyReflectionLoaderTypeName = typeof(AssemblyReflectionLoader).FullName;
				Debug.Assert(!string.IsNullOrEmpty(assemblyReflectionLoaderTypeName));
				newDomain.CreateInstance(typeof(AssemblyReflectionLoader).Assembly.FullName, assemblyReflectionLoaderTypeName);
				handle = (ReflectionAssemblyHandle)newDomain.CreateInstanceAndUnwrap(typeof(ReflectionAssemblyHandle).Assembly.FullName, typeName, false, BindingFlags.Default, null, new object[] {assemblyFileName.FullName}, null, null, null);
			} catch {
				handle = null;
				AppDomain.Unload(newDomain);
				throw;
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

		~AssemblyHandler() {
			DisposeIfNotDisposed(false);
		}

		public AssemblyName AssemblyName {
			get {
				return handle.AssemblyName;
			}
		}

		public KeyValuePair<T, string>[] GetCustomAttributes<T>() where T: Attribute {
			KeyValuePair<CustomAttributeData, string>[] assemblyCustomAttributeData = handle.GetAssemblyCustomAttributeData();
			if (assemblyCustomAttributeData.Length == 0) {
				return new KeyValuePair<T, string>[0];
			}
			Dictionary<string, Assembly> execAssemblies = new Dictionary<string, Assembly>();
			foreach (Assembly execAssembly in AppDomain.CurrentDomain.GetAssemblies()) {
				execAssemblies.Add(execAssembly.FullName, execAssembly);
			}
			List<KeyValuePair<T, string>> result = new List<KeyValuePair<T, string>>(assemblyCustomAttributeData.Length);
			foreach (KeyValuePair<CustomAttributeData, string> customAttributeData in assemblyCustomAttributeData) {
				Assembly execAssembly;
				if (execAssemblies.TryGetValue(customAttributeData.Key.Constructor.DeclaringType.Assembly.FullName, out execAssembly)) {
					Type execAttributeType = execAssembly.GetType(customAttributeData.Key.Constructor.DeclaringType.FullName);
					if (typeof(T).IsAssignableFrom(execAttributeType)) {
						ConstructorInfo execConstructor = execAttributeType.GetConstructor(customAttributeData.Key.Constructor.GetParameters().OrderBy(arg => arg.Position).Select(arg => arg.ParameterType).ToArray());
						T attribute = (T)execConstructor.Invoke(customAttributeData.Key.ConstructorArguments.Select(argument => argument.Value).ToArray());
						if (customAttributeData.Key.NamedArguments != null) {
							foreach (CustomAttributeNamedArgument namedArgument in customAttributeData.Key.NamedArguments) {
								MemberInfo execMember = execAttributeType.GetMember(namedArgument.MemberInfo.Name, namedArgument.MemberInfo.MemberType, BindingFlags.Instance|BindingFlags.Public).FirstOrDefault();
								if (execMember != null) {
									switch (execMember.MemberType) {
									case MemberTypes.Property:
										((PropertyInfo)execMember).SetValue(attribute, namedArgument.TypedValue.Value, null);
										break;
									case MemberTypes.Field:
										((FieldInfo)execMember).SetValue(attribute, namedArgument.TypedValue.Value);
										break;
									}
								}
							}
						}
						result.Add(new KeyValuePair<T, string>(attribute, customAttributeData.Value));
					}
				} else {
					Debug.WriteLine(customAttributeData.Key.Constructor.DeclaringType, "Skipping custom attribute");
				}
			}
			return result.ToArray();
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
	}
}