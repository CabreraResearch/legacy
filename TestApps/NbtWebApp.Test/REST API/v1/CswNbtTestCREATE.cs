using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Test;
using NbtWebApp.Services;
using NbtWebApp.WebSvc.Logic.API;
using NbtWebApp.WebSvc.Logic.API.DataContracts;
using NUnit.Framework;

namespace NbtWebApp.Test.REST_API
{
    [TestFixture]
    class CswNbtTestCREATE
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
        
        /// <summary>
        /// Creates a test user using the APIs CREATE and then fetches it then verifies the node can be fetched using CswNbtResources
        /// </summary>
        [Test]
        public void TestCreate()
        {
            CswNbtMetaDataNodeType UserNT = TestData.CswNbtResources.MetaData.getNodeType( "User" );
            if( null == UserNT )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Unable to create a test User for REST API unit tests - no User NT was found", "Could not find a user NT by name 'User'" );
            }

            CswNbtAPIGenericRequest Request = new CswNbtAPIGenericRequest( "User", string.Empty );
            CswNbtResource CreatedResource = new CswNbtResource();

            CswNbtWebServiceCREATE.Create( TestData.CswNbtResources, CreatedResource, Request );

            CswNbtObjClassUser createdUser = TestData.CswNbtResources.Nodes.GetNode( CreatedResource.NodeId );
            Assert.IsNotNull( createdUser, "Created User was null when fetching from CswNbtResources" );

            //When CIS-53088 is finished we should consider extending this to also add POST data to test creating the user with some property values
        }
    }
}
