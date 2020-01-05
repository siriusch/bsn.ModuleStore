using System;

using bsn.ModuleStore.Mapper.Serialization;

namespace bsn.ModuleStore.Mapper.InterfaceMetadata {
	public class InterfaceMetadataProvider: IMetadataProvider {
		public ISqlCallInfo GetCallInfo(Type interfaceToProxy) {
			return SqlCallInfo.Get(interfaceToProxy, SerializationTypeInfoProvider);
		}

		public ISerializationTypeInfoProvider SerializationTypeInfoProvider => new SerializationTypeInfoProvider(new SerializationTypeMappingProvider());
	}
}
