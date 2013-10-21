using ChemSW.Nbt.Test;
using NUnit.Framework;

namespace NbtWebApp.Test.WebServices
{
    [TestFixture]
    public class CswNbtWebServiceNodeTest
    {
        #region Setup and Teardown

        private TestData TestData;

        [SetUp]
        public void MyTestInitialize()
        {
            TestData = new TestData();
        }

        [TearDown]
        public void MyTestCleanup()
        {
            TestData.Destroy();
        }

        #endregion

        [Test]
        public void TestMethod1()
        {
            Assert.IsNotNull( TestData.CswNbtResources );
        }
    }
}
