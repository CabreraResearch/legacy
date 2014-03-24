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
    class CswNbtTestREAD
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
        /// Creates a test user and then fetches it using the APIs GET, catching and reporting any errors
        /// </summary>
        [Test]
        public void TestResourceGET()
        {
            const string TestUsername = "MyRestUser";
            const string RoleName = "Administrator";
            const int TestPageSize = 15;
            const string TestLanguage = "da";

            CswNbtObjClassUser testUser = _createTestUser( TestUsername, RoleName, TestPageSize, TestLanguage );

            CswNbtResourceWithProperties Resource = new CswNbtResourceWithProperties();
            CswNbtAPIGenericRequest Request = new CswNbtAPIGenericRequest( "User", testUser.NodeId.PrimaryKey.ToString() );

            string error = string.Empty;
            try
            {
                CswNbtWebServiceREAD.GetResource( TestData.CswNbtResources, Resource, Request );
            }
            catch( Exception ex )
            {
                error = ex.Message;
            }

            Assert.IsEmpty( error, "GETing a resource failed with exception: " + error );

            //When CIS-53085 is finished, we may want to consider verifying the prop values at the top of this method is equal to the resource fetched
        }

        /// <summary>
        /// Creates two test users and then does a GET on the User NT, verifying that there are at least 2 Users 
        /// </summary>
        [Test]
        public void TestCollectionGET()
        {
            const string RoleName = "Administrator";

            const string TestUsername1 = "MyRestUser1";
            const int TestPageSize1 = 15;
            const string TestLanguage1 = "da";
            CswNbtObjClassUser testUser1 = _createTestUser( TestUsername1, RoleName, TestPageSize1, TestLanguage1 );

            const string TestUsername2 = "MyRestUser2";
            const int TestPageSize2 = 25;
            const string TestLanguage2 = "es";
            CswNbtObjClassUser testUser2 = _createTestUser( TestUsername2, RoleName, TestPageSize2, TestLanguage2 );

            CswNbtResourceCollection resourceCollection = new CswNbtResourceCollection();
            CswNbtAPIGenericRequest request = new CswNbtAPIGenericRequest( "User", string.Empty );

            string error = string.Empty;
            try
            {
                CswNbtWebServiceREAD.GetCollection( TestData.CswNbtResources, resourceCollection, request );
            }
            catch( Exception ex )
            {
                error = ex.Message;
            }
            Assert.IsEmpty( error, "GETing a collection failed with exception: " + error );

            bool Found1 = false;
            bool Found2 = false;
            foreach( CswNbtResource resource in resourceCollection.getEntities() )
            {
                if( resource.NodeId == testUser1.NodeId )
                {
                    Found1 = true;
                }
                else if( resource.NodeId == testUser2.NodeId )
                {
                    Found2 = true;
                }
            }

            Assert.IsTrue( Found1, "GETing Resource did not contain the first test user when it should have." );
            Assert.IsTrue( Found2, "GETing Resource did not contain the second test user when it should have." );
        }
    }
}
