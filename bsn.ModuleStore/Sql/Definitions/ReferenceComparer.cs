using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace bsn.ModuleStore.Sql.Definitions {
	/// <summary>
	/// Equiality comparer which only uses the object reference.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Serializable]
	public sealed class ReferenceEqualityComparer<T>: IEqualityComparer<T> where T: class {
		private static readonly ReferenceEqualityComparer<T> @default = new ReferenceEqualityComparer<T>();

		/// <summary>
		/// Gets the default ReferenceEqualityComparer for the type <typeparamref name="T"/>.
		/// </summary>
		/// <value>The default.</value>
		public static ReferenceEqualityComparer<T> Default {
			get {
				return @default;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReferenceEqualityComparer&lt;T&gt;"/> class.
		/// </summary>
		public ReferenceEqualityComparer() {}

		/// <summary>
		/// Determines whether the specified objects are equal.
		/// </summary>
		/// <param name="x">The first object of type <paramref name="T"/> to compare.</param>
		/// <param name="y">The second object of type <paramref name="T"/> to compare.</param>
		/// <returns>
		/// true if the specified objects are equal; otherwise, false.
		/// </returns>
		public bool Equals(T x, T y) {
			return ReferenceEquals(x, y);
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.
		/// </exception>
		public int GetHashCode(T obj) {
			if (obj == null) {
				throw new ArgumentNullException("obj");
			}
			return RuntimeHelpers.GetHashCode(obj);
		}
	}
}
