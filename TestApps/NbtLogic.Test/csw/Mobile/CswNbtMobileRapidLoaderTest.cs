using ChemSW.Core;
using ChemSW.Nbt.csw.Mobile;
using NUnit.Framework;

namespace ChemSW.Nbt.Test.Mobile
{
    [TestFixture]
    public class CswNbtMobileRapidLoaderTest
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
            //CswTempFile TempTools = new CswTempFile( TestData.CswNbtResources );
            //TempTools.purgeTempFiles( "csv" ); I get "this file is currently being used by another process"
        }

        #endregion

        /// <summary>
        /// See W1005
        /// </summary>
        [Test]
        public void saveRapidLoaderDataTest()
        {
            RapidLoaderData.RapidLoaderDataRequest Request = new RapidLoaderData.RapidLoaderDataRequest
            {
                EmailAddress = "bvavra@chemsw.com",
                CSVData = "testing,1,2,3"
            };
            CswNbtMobileRapidLoader _CswNbtMobileRapidLoader = new CswNbtMobileRapidLoader( TestData.CswNbtResources );
            _CswNbtMobileRapidLoader.saveRapidLoaderData( Request );
            Assert.Inconclusive();
        }

    }
}
