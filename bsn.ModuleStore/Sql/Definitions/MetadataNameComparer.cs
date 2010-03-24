// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace bsn.ModuleStore.Sql.Definitions {
	/// <summary>
	/// Compares the metadata type and name, but not its contents.
	/// </summary>
	/// <typeparam name="T">Any type inheriting from <see cref="Metadata{TSelf}"/></typeparam>
	[Serializable]
	public sealed class MetadataNameComparer<T>: ISerializable, IComparer<T>, IEqualityComparer<T> where T: Metadata<T> {
		public MetadataNameComparer() {}

		internal MetadataNameComparer(SerializationInfo info, StreamingContext context) {
		}

		/// <summary>
		/// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		/// <returns>
		/// Value
		/// Condition
		/// Less than zero
		/// <paramref name="x"/> is less than <paramref name="y"/>.
		/// Zero
		/// <paramref name="x"/> equals <paramref name="y"/>.
		/// Greater than zero
		/// <paramref name="x"/> is greater than <paramref name="y"/>.
		/// </returns>
		/// <remarks><c>null</c> items are accepted and come first</remarks>
		public int Compare(T x, T y) {
			if (ReferenceEquals(x, y)) {
				return 0;
			}
			if (x == null) {
				return -1;
			}
			if (y == null) {
				return 1;
			}
			return Metadata<T>.NameComparer.Compare(x.Name, y.Name);
		}

		/// <summary>
		/// Determines whether the specified objects are equal.
		/// </summary>
		/// <param name="x">The first object of type <paramref name="T"/> to compare.</param>
		/// <param name="y">The second object of type <paramref name="T"/> to compare.</param>
		/// <returns>
		/// true if the specified objects are equal; otherwise, false.
		/// </returns>
		/// <remarks>The result when any argument is <c>null</c> is <c>false</c></remarks>
		public bool Equals(T x, T y) {
			if (x == null) {
				return false;
			}
			if (ReferenceEquals(x, y)) {
				return true;
			}
			return x.NameEquals(y);
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
		/// <remarks>A <c>null</c> argument returns 0</remarks>
		public int GetHashCode(T obj) {
			if (obj == null) {
				return 0;
			}
			return obj.GetNameHashCode();
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {}
	}
}