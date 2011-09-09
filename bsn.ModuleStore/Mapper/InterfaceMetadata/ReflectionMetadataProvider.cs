using System;

using bsn.ModuleStore.Mapper.Serialization;

namespace bsn.ModuleStore.Mapper.InterfaceMetadata {
	public class ReflectionMetadataProvider: IMetadataProvider {
		public ISqlCallInfo GetCallInfo(Type interfaceToProxy) {
			return SqlCallInfo.Get(interfaceToProxy, SerializationTypeInfoProvider);
		}

		public ISerializationTypeInfoProvider SerializationTypeInfoProvider {
			get {
				return new SerializationTypeInfoProvider(new SerializationTypeMappingProvider());
			}
		}
	}
}
