using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace bsn.ModuleStore.Mapper {
	/// <summary>
	/// This is the base class for attributes specifying a property or argument name mapping to columns
	/// </summary>
	public abstract class SqlColumnAttributeBase: Attribute {
		/// <summary>
		/// Get a single <see cref="SqlColumnAttribute"/> instance.
		/// </summary>
		/// <param name="member">The <see cref="MemberInfo"/> to query for the <see cref="SqlColumnAttribute"/> attribute.</param>
		/// <param name="autoCreate">If true, a <see cref="SqlColumnAttribute"/> is inferred from the MemberInfo when no attribute is found. Otherwise, null is returned in this situation.</param>
		/// <returns>The <see cref="SqlColumnAttribute"/> for the member.</returns>
		public static T Get<T>(MemberInfo member, bool autoCreate) where T: SqlColumnAttributeBase, new() {
			if (member == null) {
				throw new ArgumentNullException("member");
			}
			T result = member.GetCustomAttributes(typeof(T), true).Cast<T>().FirstOrDefault();
			if (result == null) {
				if (!autoCreate) {
					return null;
				}
				result = new T();
			}
			if (string.IsNullOrEmpty(result.Name)) {
				result = (T)result.CloneWithName(member.Name);
			}
			return result;
		}

		private DateTimeKind dateTimeKind = DateTimeKind.Unspecified;
		private bool getCachedByIdentity;
		private bool identity;
		private string name;

		/// <summary>
		/// Create a new <see cref="SqlColumnAttribute"/>.
		/// </summary>
		/// <param name="name">The DB column name to bind to.</param>
		protected SqlColumnAttributeBase([Localizable(false)] string name) {
			this.name = name;
		}

		/// <summary>
		/// Gets or sets the kind of the DateTime, if the instance is one.
		/// </summary>
		/// <value>The kind of the date time.</value>
		public DateTimeKind DateTimeKind {
			get {
				return dateTimeKind;
			}
			set {
				dateTimeKind = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the column <see cref="SqlColumnAttribute"/> is a foreign key to an instance cached by identity in the provider.
		/// </summary>
		public bool GetCachedByIdentity {
			get {
				return getCachedByIdentity;
			}
			set {
				getCachedByIdentity = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the column <see cref="SqlColumnAttribute"/> is an identity column for this data type.
		/// </summary>
		public bool Identity {
			get {
				return identity;
			}
			set {
				identity = value;
			}
		}

		/// <summary>
		/// The name for the database binding.
		/// </summary>
		public string Name {
			get {
				return name;
			}
		}

		public SqlColumnAttributeBase CloneWithName(string newName) {
			SqlColumnAttributeBase result = (SqlColumnAttributeBase)MemberwiseClone();
			result.name = newName;
			return result;
		}
	}
}
