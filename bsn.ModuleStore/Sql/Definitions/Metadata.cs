// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace bsn.ModuleStore.Sql.Definitions {
	/// <summary>
	/// The base class for metadata, such as SQL Objects.
	/// </summary>
	/// <typeparam name="TSelf">The type itself (e.g. <c>class X: Metadata&lt;X&gt;</c>.</typeparam>
	/// <remarks>Whether names are case sensitive or not can be controlled by a <see cref="NameCompareAttribute"/> on the <typeparamref name="TSelf"/> type. If not specified, they are case insensitive.</remarks>
	public abstract class Metadata<TSelf> : ISerializable, IEquatable<TSelf> where TSelf : Metadata<TSelf> {
		private static readonly StringComparer nameComparer = ResolveNameComparer();

		/// <summary>
		/// Gets the name comparer for this metadata type.
		/// </summary>
		/// <value>The name comparer.</value>
		/// <remarks>The comparer is determined by a <see cref="NameCompareAttribute"/> on the <typeparamref name="TSelf"/> type. If not specified, they are case sensitive.</remarks>
		public static StringComparer NameComparer {
			get {
				return nameComparer;
			}
		}

		private static StringComparer ResolveNameComparer() {
			foreach (NameCompareAttribute nameCompareAttribute in typeof(TSelf).GetCustomAttributes(typeof(NameCompareAttribute), true)) {
				return nameCompareAttribute.GetComparer();
			}
			return StringComparer.OrdinalIgnoreCase;
		}

		private readonly string name;

		/// <summary>
		/// Initializes a new instance of the <see cref="Metadata&lt;TSelf&gt;"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		protected Metadata(string name) {
			if (string.IsNullOrEmpty(name)) {
				throw new ArgumentNullException("name");
			}
			this.name = name;
		}

		/// <summary>
		/// Deserializes a new instance of the <see cref="Metadata&lt;TSelf&gt;"/> class.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="context">The context.</param>
		protected Metadata(SerializationInfo info, StreamingContext context): this(info, context, null) {
		}

		internal Metadata(SerializationInfo info, StreamingContext context, Func<string, string> nameProcessor) {
			if (info == null) {
				throw new ArgumentNullException("info");
			}
			name = info.GetString("name");
			if (nameProcessor != null) {
				name = nameProcessor(name);
			}
			Debug.Assert(!string.IsNullOrEmpty(name));
		}

		/// <summary>
		/// Gets the name of this metadata item.
		/// </summary>
		/// <value>The name.</value>
		public string Name {
			get {
				return name;
			}
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		public bool Equals(TSelf other) {
			if (NameEquals(other)) {
				return EqualsInternal(other);
			}
			return false;
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {
			GetObjectData(info, context);
		}

		/// <summary>
		/// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
		/// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
		/// <exception cref="T:System.Security.SecurityException">
		/// The caller does not have the required permission.
		/// </exception>
		protected virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
			if (info == null) {
				throw new ArgumentNullException("info");
			}
			info.AddValue("name", name);
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode() {
			return GetNameHashCode();
		}

		internal int GetNameHashCode() {
			return typeof(TSelf).GetHashCode()^NameComparer.GetHashCode(name);
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
		/// <returns>
		/// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="T:System.NullReferenceException">
		/// The <paramref name="obj"/> parameter is null.
		/// </exception>
		public override sealed bool Equals(object obj) {
			return Equals(obj as TSelf);
		}

		internal bool NameEquals(TSelf other) {
			return (other != null) && NameComparer.Equals(name, other.name);
		}

		/// <summary>
		/// Indicates whether the current object content is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object, ignoring the <see cref="Name"/> property.</param>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		protected virtual bool EqualsInternal(TSelf other) {
			return true;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString() {
			return name;
		}
	}
}