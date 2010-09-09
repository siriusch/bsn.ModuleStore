using System;

namespace bsn.ModuleStore.Mapper {
	/// <summary>
	/// Classes used in database deserialization with <see cref="SqlDeserializer{T}"/> or automatically by <see cref="SqlCallProxy"/> calls can implement this interface to receive a notification after deserialization.
	/// </summary>
	public interface ISqlDeserializationHook {
		/// <summary>
		/// Called after the instance has been deserialized.
		/// </summary>
		void AfterDeserialization();
	}
}