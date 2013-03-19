using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Encryption;
using ChemSW.Nbt.Security;
using ChemSW.Security;
using NUnit.Framework;

namespace ChemSW.Nbt.Test.Security
{
    [TestFixture]
    public class CswNbtSchemaAuthenticatorTest
    {
        #region Setup and Teardown

        private TestData TestData;
        private Int32 _LoginDataHWM;
        private CswNbtSchemaAuthenticator _SchemaAuthenticator;
        private CswEncryption _CswEncryption;

        [SetUp]
        public void MyTestInitialize()
        {
            TestData = new TestData();
            _SchemaAuthenticator = new CswNbtSchemaAuthenticator( TestData.CswNbtResources );
            _CswEncryption = new CswEncryption( TestData.CswNbtResources.MD5Seed );
            DataTable MaxNodeTable = TestData.CswNbtResources.execArbitraryPlatformNeutralSqlSelect( "getHWM", "select max(loginid) as hwm from login_data" );
            _LoginDataHWM = CswConvert.ToInt32( MaxNodeTable.Rows[0]["hwm"] );
        }

        [TearDown]
        public void MyTestCleanup()
        {
            TestData.Destroy();
            TestData.CswNbtResources.execArbitraryPlatformNeutralSqlInItsOwnTransaction( "delete from login_data where loginid > " + _LoginDataHWM );
        }

        #endregion

        /// <summary>
        /// Given a valid username and password, authentication passes
        /// </summary>
        [Test]
        public void AuthenticateWithSchemaTest_Authenticated()
        {
            String Username = "gooduser";
            String Password = "goodpw1!";
            TestData.Nodes.createUserNode( Username, Password );
            AuthenticationStatus Expected = AuthenticationStatus.Authenticated;
            ICswUser User;
            AuthenticationStatus Actual = _SchemaAuthenticator.AuthenticateWithSchema( _CswEncryption, Username, Password, "127.0.0.1", out User );
            Assert.AreEqual( Expected.ToString(), Actual.ToString(), "User was not authenticated." );
        }

        /// <summary>
        /// Given an invalid username, authentication fails with a status of Failed
        /// </summary>
        [Test]
        public void AuthenticateWithSchemaTest_FailedBadUsername()
        {
            String Username = "gooduser";
            String Password = "goodpw1!";
            TestData.Nodes.createUserNode( Username, Password );
            AuthenticationStatus Expected = AuthenticationStatus.Failed;
            ICswUser User;
            AuthenticationStatus Actual = _SchemaAuthenticator.AuthenticateWithSchema( _CswEncryption, "baduser", Password, "127.0.0.1", out User );
            Assert.AreEqual( Expected.ToString(), Actual.ToString(), "User did not fail authentication as expected." );
        }

        /// <summary>
        /// Given a valid username and invalid password, authentication fails with a status of Failed
        /// </summary>
        [Test]
        public void AuthenticateWithSchemaTest_FailedBadPassword()
        {
            String Username = "gooduser";
            String Password = "goodpw1!";
            TestData.Nodes.createUserNode( Username, Password );
            AuthenticationStatus Expected = AuthenticationStatus.Failed;
            ICswUser User;
            AuthenticationStatus Actual = _SchemaAuthenticator.AuthenticateWithSchema( _CswEncryption, Username, "badpw", "127.0.0.1", out User );
            Assert.AreEqual( Expected.ToString(), Actual.ToString(), "User did not fail authentication as expected." );
        }

        /// <summary>
        /// Given a valid username and password for a locked User, authentication fails with a status of Locked
        /// </summary>
        [Test]
        public void AuthenticateWithSchemaTest_FailedLocked()
        {
            String Username = "gooduser";
            String Password = "goodpw1!";
            TestData.Nodes.createUserNode( Username, Password, isLocked: Tristate.True );
            AuthenticationStatus Expected = AuthenticationStatus.Locked;
            ICswUser User;
            AuthenticationStatus Actual = _SchemaAuthenticator.AuthenticateWithSchema( _CswEncryption, Username, Password, "127.0.0.1", out User );
            Assert.AreEqual( Expected.ToString(), Actual.ToString(), "User was not locked." );
        }

        /// <summary>
        /// Given a valid username and password for an archived User, authentication fails with a status of Arcived
        /// </summary>
        [Test]
        public void AuthenticateWithSchemaTest_FailedArchived()
        {
            String Username = "gooduser";
            String Password = "goodpw1!";
            TestData.Nodes.createUserNode( Username, Password, isArchived: Tristate.True );
            AuthenticationStatus Expected = AuthenticationStatus.Archived;
            ICswUser User;
            AuthenticationStatus Actual = _SchemaAuthenticator.AuthenticateWithSchema( _CswEncryption, Username, Password, "127.0.0.1", out User );
            Assert.AreEqual( Expected.ToString(), Actual.ToString(), "User was not archived." );
        }
    }
}
