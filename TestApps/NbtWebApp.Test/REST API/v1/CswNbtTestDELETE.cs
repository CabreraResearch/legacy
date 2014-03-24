using System;
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
    class CswNbtTestDELETE
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
        /// Creates a test user and then DELETEs it using the REST API and verifies that attempting to fetch the node by nodeid returns null
        /// </summary>
        [Test]
        public void TestDelete()
        {
            CswNbtObjClassUser TestUser = _createTestUser( "TestDeleteUser1", "Administrator", 15, "en" );
            CswNbtAPIGenericRequest Request = new CswNbtAPIGenericRequest( "User", TestUser.NodeId.PrimaryKey.ToString() );

            CswNbtResourceWithProperties EditedResource = new CswNbtResourceWithProperties();
            string error = string.Empty;
            try
            {
                CswNbtWebServiceDELETE.Delete( TestData.CswNbtResources, EditedResource, Request );
            }
            catch( Exception ex )
            {
                error = ex.Message;
            }
            Assert.IsEmpty( error, "Delete failed with exception: " + error );

            CswNbtObjClassUser User = TestData.CswNbtResources.Nodes.GetNode( TestUser.NodeId );
            Assert.IsNull( User, "User was not null when it should have been" );
        }
    }
}
