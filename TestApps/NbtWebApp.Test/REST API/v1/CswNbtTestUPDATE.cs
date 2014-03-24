using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Test;
using NbtWebApp.Services;
using NbtWebApp.WebSvc.Logic.API;
using NbtWebApp.WebSvc.Logic.API.DataContracts;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace NbtWebApp.Test.REST_API
{
    [TestFixture]
    class CswNbtTestUPDATE
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
        /// Creates a new user for testing with. It is the consumers responsibility to provide valid values for properties. Supplying a non-existant Role name will throw an error
        /// </summary>
        private CswNbtObjClassUser _createTestUser( string Username, string RoleName, int PageSize, string Language )
        {
            CswNbtMetaDataNodeType UserNT = TestData.CswNbtResources.MetaData.getNodeType( "User" );
            if( null == UserNT )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Unable to create a test User for REST API unit tests - no User NT was found", "Could not find a user NT by name 'User'" );
            }
            CswNbtObjClassRole RoleNode = TestData.CswNbtResources.Nodes.makeRoleNodeFromRoleName( RoleName );
            if( null == RoleNode )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Unable to create a test User for REST API unit tests - no Role with name '" + RoleName + "'", "Could not find a role by name '" + RoleName + "'" );
            }

            CswNbtObjClassUser TestUser = TestData.CswNbtResources.Nodes.makeNodeFromNodeTypeId( UserNT.NodeTypeId, OnAfterMakeNode : delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassUser AsUser = NewNode;
                    AsUser.UsernameProperty.Text = Username;
                    AsUser.Role.RelatedNodeId = RoleNode.NodeId;
                    AsUser.PageSizeProperty.Value = PageSize;
                    AsUser.LanguageProperty.Value = Language;
                } );

            return TestUser;
        }

        /// <summary>
        /// Creates a test user using the APIs CREATE and then fetches it then verifies the node can be fetched using CswNbtResources
        /// </summary>
        [Test]
        public void TestUpdate()
        {
            CswNbtObjClassUser TestUser = _createTestUser( "Username1", "Administrator", 15, "en" );
            CswNbtAPIGenericRequest Request = new CswNbtAPIGenericRequest( "User", TestUser.NodeId.PrimaryKey.ToString() );
            CswNbtResourceWithProperties FetchedResource = new CswNbtResourceWithProperties();
            CswNbtWebServiceREAD.GetResource( TestData.CswNbtResources, FetchedResource, Request );

            CswNbtMetaDataNodeType UserNT = TestData.CswNbtResources.MetaData.getNodeType( "User" );
            CswNbtMetaDataNodeTypeProp UserNameProp = UserNT.getNodeTypeProp("Username" );

            JObject PropData = FetchedResource.PropertyData;
            const string NewUsername = "Username2";
            PropData["prop_nodes_" + TestUser.NodeId.PrimaryKey + "_" + UserNameProp.PropId]["values"]["text"] = NewUsername; //When CIS-53085 is completed we can change this terrible line of code

            CswNbtAPIGenericRequest EditRequest = new CswNbtAPIGenericRequest( "User", TestUser.NodeId.PrimaryKey.ToString() )
                {
                    PropData = PropData
                };
            CswNbtResourceWithProperties EditedResource = new CswNbtResourceWithProperties();
            CswNbtWebServiceUPDATE.Edit( TestData.CswNbtResources, EditedResource, EditRequest );

            CswNbtObjClassUser EditedUser = TestData.CswNbtResources.Nodes.GetNode( TestUser.NodeId );
            Assert.AreEqual(EditedUser.UsernameProperty.Text, NewUsername, "Test username did not have the same username as the one supplied for the API EDIT request");
        }
    }
}
