using System;
using System.Collections;
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

			public ReflectionAssemblyHandle(string assemblyFile) : base() {
				assembly = Assembly.ReflectionOnlyLoadFrom(assemblyFile);
			}

			public AssemblyName AssemblyName {
				get {
					return assembly.GetName();
				}
			}

		public string[] GetManifestResourceNames() {
			return assembly.GetManifestResourceNames();
		}

		public Stream GetManifestResourceStream(string streamName) {
			return assembly.GetManifestResourceStream(streamName);
		}


			public IList<CustomAttributeData> GetAssemblyCustomAttributeData() {
				return CustomAttributeData.GetCustomAttributes(assembly);
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
				handle = (ReflectionAssemblyHandle)newDomain.CreateInstanceAndUnwrap(typeof(ReflectionAssemblyHandle).Assembly.FullName, typeName, false, BindingFlags.Default, null, new object[] { assemblyFileName.FullName }, null, null, null);
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

		public void Dispose() {
			GC.SuppressFinalize(this);
			DisposeIfNotDisposed(true);
		}

		private IEnumerable GetCustomAttributesInternal(Type attributeType, bool inherit) {
			if (attributeType == null) {
				throw new ArgumentNullException("attributeType");
			}
			IList<CustomAttributeData> customAttributeDatas = handle.GetAssemblyCustomAttributeData();
			if (customAttributeDatas.Count > 0) {
				Dictionary<string, Assembly> execAssemblies = new Dictionary<string, Assembly>();
				foreach (Assembly execAssembly in AppDomain.CurrentDomain.GetAssemblies()) {
					execAssemblies.Add(execAssembly.FullName, execAssembly);
				}
				foreach (CustomAttributeData customAttributeData in customAttributeDatas) {
					Assembly execAssembly;
					if (execAssemblies.TryGetValue(customAttributeData.Constructor.DeclaringType.Assembly.FullName, out execAssembly)) {
						Type execAttributeType = execAssembly.GetType(customAttributeData.Constructor.DeclaringType.FullName);
						if (attributeType.IsAssignableFrom(execAttributeType)) {
							ConstructorInfo execConstructor = execAttributeType.GetConstructor(customAttributeData.Constructor.GetParameters().OrderBy(arg => arg.Position).Select(arg => arg.ParameterType).ToArray());
							Attribute attribute = (Attribute)execConstructor.Invoke(customAttributeData.ConstructorArguments.Select(argument => argument.Value).ToArray());
							if (customAttributeData.NamedArguments != null) {
								foreach (CustomAttributeNamedArgument namedArgument in customAttributeData.NamedArguments) {
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
							yield return attribute;
						}
					} else {
						Debug.WriteLine(attributeType, "Skipping custom attribute");
					}
				}
			}
		}

		public object[] GetCustomAttributes(Type attributeType, bool inherit) {
			return GetCustomAttributesInternal(attributeType, inherit).OfType<Attribute>().ToArray();
		}

		public object[] GetCustomAttributes(bool inherit) {
			return GetCustomAttributes(typeof(Attribute), inherit);
		}

		public bool IsDefined(Type attributeType, bool inherit) {
			return handle.GetAssemblyCustomAttributeData().Select(data => data.Constructor.DeclaringType).Contains(attributeType);
		}

		public AssemblyName AssemblyName {
			get {
				return handle.AssemblyName;
			}
		}

		public string[] GetManifestResourceNames() {
			return handle.GetManifestResourceNames();
		}

		public Stream GetManifestResourceStream(string streamName) {
			return handle.GetManifestResourceStream(streamName);
		}
	}
}