using System;

namespace bsn.ModuleStore.Mapper {
	/// <summary>
	/// The DbDeserializeAttribute can be specified on fields representing nested data that should be deserialized.
	/// </summary>
	/// <example>
	/// The class B contains the nested class A, which will be deserialized as inner object from a resultset containing the columns [a,b].
	/// <code>
	/// public class A {
	///		[DbColumn('a')]
	///		private int a;
	/// }
	/// 
	/// public class B {
	///		[DbDeserialize]
	///		private A a;
	///		[DbColumn('b')]
	///		private int b;
	/// }
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class SqlDeserializeAttribute: Attribute {}
}