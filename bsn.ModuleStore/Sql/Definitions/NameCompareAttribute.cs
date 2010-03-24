// (C) 2010 Arsène von Wyss / bsn
using System;

namespace bsn.ModuleStore.Sql.Definitions {
	/// <summary>
	/// Attribute to control the name comparison (expecially case-sensitive or case-insensitive).
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class NameCompareAttribute: Attribute {
		private readonly StringComparison comparison;

		/// <summary>
		/// Initializes a new instance of the <see cref="NameCompareAttribute"/> class.
		/// </summary>
		/// <param name="comparison">The comparison.</param>
		public NameCompareAttribute(StringComparison comparison) {
			this.comparison = comparison;
		}

		/// <summary>
		/// Gets the name comparison kind.
		/// </summary>
		/// <value>The comparison.</value>
		public StringComparison Comparison {
			get {
				return comparison;
			}
		}

		/// <summary>
		/// Gets the comparer matching the comparison.
		/// </summary>
		public StringComparer GetComparer() {
			switch (comparison) {
			case StringComparison.CurrentCulture:
				return StringComparer.CurrentCulture;
			case StringComparison.CurrentCultureIgnoreCase:
				return StringComparer.CurrentCultureIgnoreCase;
			case StringComparison.InvariantCulture:
				return StringComparer.InvariantCulture;
			case StringComparison.InvariantCultureIgnoreCase:
				return StringComparer.InvariantCultureIgnoreCase;
			case StringComparison.Ordinal:
				return StringComparer.Ordinal;
			case StringComparison.OrdinalIgnoreCase:
				return StringComparer.OrdinalIgnoreCase;
			}
			throw new InvalidOperationException();
		}
	}
}