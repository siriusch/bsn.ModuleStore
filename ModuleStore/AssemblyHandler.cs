using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

using bsn.ModuleStore.Sql;

namespace bsn.ModuleStore.Console {
	public class AssemblyHandler: IDisposable {
		private readonly AppDomain domain;
		private readonly AssemblyHandle handle;
		private int disposed;

		public AssemblyHandler(FileInfo assemblyFileName) {
			if (assemblyFileName == null) {
				throw new ArgumentNullException("assemblyFileName");
			}
			string typeName = typeof(AssemblyHandle).FullName;
			Debug.Assert(typeName != null);
			AppDomain newDomain = AppDomain.CreateDomain("ModuleStore Assembly Discovery AppDomain");
			try {
				handle = (AssemblyHandle)newDomain.CreateInstanceAndUnwrap(typeof(AssemblyHandle).Assembly.FullName, typeName, false, BindingFlags.Default, null, new object[] {assemblyFileName.FullName}, null, null, null);
			} catch {
				handle = null;
				AppDomain.Unload(newDomain);
				throw;
			}
			domain = newDomain;
			Debug.WriteLine(typeName, "Loaded into new AppDomain");
		}

		public AssemblyHandle Handle {
			get {
				return handle;
			}
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
	}
}
