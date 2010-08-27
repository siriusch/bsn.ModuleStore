using NUnit.Framework;

namespace bsn.ModuleStore.Sql.Script {
	[TestFixture]
	public class SqlTokenMetadataFactoryTest: AssertionHelper {
		[Test]
		public void CheckFieldToPropertyRelations() {
			SqlTokenMetadataFactory.CheckFieldsAndProperties();
		}
	}
}
