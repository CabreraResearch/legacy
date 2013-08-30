using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Encryption;
using ChemSW.Nbt.Security;
using ChemSW.Security;
using ChemSW.WebSvc;
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
            TestData = new TestData { FinalizeNodes = true };
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
            CswWebSvcSessionAuthenticateData.Authentication.Request AuthenticationRequest  = new CswWebSvcSessionAuthenticateData.Authentication.Request();
            AuthenticationRequest.UserName = "gooduser";
            AuthenticationRequest.Password = "goodpw1!";
            AuthenticationRequest.IpAddress = "127.0.0.1";

            TestData.Nodes.createUserNode( "gooduser", "goodpw1!" );
            CswEnumAuthenticationStatus Expected = CswEnumAuthenticationStatus.Authenticated;
            ICswUser User;
            AuthenticationRequest.AuthenticationStatus = CswEnumAuthenticationStatus.Failed;
            AuthenticationRequest.AuthenticationStatus = _SchemaAuthenticator.AuthenticateWithSchema( _CswEncryption, AuthenticationRequest, out User );
            Assert.AreEqual( Expected.ToString(), AuthenticationRequest.AuthenticationStatus.ToString(), "User was not authenticated." );
        }

        /// <summary>
        /// Given an invalid username, authentication fails with a status of Failed
        /// </summary>
        [Test]
        public void AuthenticateWithSchemaTest_FailedBadUsername()
        {
            CswWebSvcSessionAuthenticateData.Authentication.Request AuthenticationRequest  = new CswWebSvcSessionAuthenticateData.Authentication.Request { UserName = "baduser", Password = "goodpw1!", IpAddress = "127.0.0.1" };

            TestData.Nodes.createUserNode( "gooduser", "goodpw1!" );

            CswEnumAuthenticationStatus Expected = CswEnumAuthenticationStatus.Failed;
            ICswUser User;
            AuthenticationRequest.AuthenticationStatus = CswEnumAuthenticationStatus.Failed;
            AuthenticationRequest.AuthenticationStatus = _SchemaAuthenticator.AuthenticateWithSchema( _CswEncryption, AuthenticationRequest, out User );
            Assert.AreEqual( Expected.ToString(), AuthenticationRequest.AuthenticationStatus.ToString(), "User did not fail authentication as expected." );
        }

        /// <summary>
        /// Given a valid username and invalid password, authentication fails with a status of Failed
        /// </summary>
        [Test]
        public void AuthenticateWithSchemaTest_FailedBadPassword()
        {
            CswWebSvcSessionAuthenticateData.Authentication.Request AuthenticationRequest  = new CswWebSvcSessionAuthenticateData.Authentication.Request { UserName = "gooduser", Password = "badpw", IpAddress = "127.0.0.1" };

            TestData.Nodes.createUserNode( "gooduser", "goodpw1!" );

            CswEnumAuthenticationStatus Expected = CswEnumAuthenticationStatus.Failed;
            ICswUser User;
            AuthenticationRequest.AuthenticationStatus = CswEnumAuthenticationStatus.Failed;
            AuthenticationRequest.AuthenticationStatus = _SchemaAuthenticator.AuthenticateWithSchema( _CswEncryption, AuthenticationRequest, out User );
            Assert.AreEqual( Expected.ToString(), AuthenticationRequest.AuthenticationStatus.ToString(), "User did not fail authentication as expected." );
        }

        /// <summary>
        /// Given a valid username and password for a locked User, authentication fails with a status of Locked
        /// </summary>
        [Test]
        public void AuthenticateWithSchemaTest_FailedLocked()
        {
            CswWebSvcSessionAuthenticateData.Authentication.Request AuthenticationRequest  = new CswWebSvcSessionAuthenticateData.Authentication.Request { UserName = "gooduser", Password = "goodpw1!", IpAddress = "127.0.0.1" };

            TestData.Nodes.createUserNode( "gooduser", "goodpw1!", isLocked: CswEnumTristate.True );

            CswEnumAuthenticationStatus Expected = CswEnumAuthenticationStatus.Locked;
            ICswUser User;
            AuthenticationRequest.AuthenticationStatus = CswEnumAuthenticationStatus.Failed;
            AuthenticationRequest.AuthenticationStatus = _SchemaAuthenticator.AuthenticateWithSchema( _CswEncryption, AuthenticationRequest, out User );
            Assert.AreEqual( Expected.ToString(), AuthenticationRequest.AuthenticationStatus.ToString(), "User was not locked." );
        }

        /// <summary>
        /// Given a valid username and password for an archived User, authentication fails with a status of Arcived
        /// </summary>
        [Test]
        public void AuthenticateWithSchemaTest_FailedArchived()
        {
            CswWebSvcSessionAuthenticateData.Authentication.Request AuthenticationRequest  = new CswWebSvcSessionAuthenticateData.Authentication.Request { UserName = "gooduser", Password = "goodpw1!", IpAddress = "127.0.0.1" };

            TestData.Nodes.createUserNode( "gooduser", "goodpw1!", isArchived: CswEnumTristate.True );

            CswEnumAuthenticationStatus Expected = CswEnumAuthenticationStatus.Archived;
            ICswUser User;
            AuthenticationRequest.AuthenticationStatus = CswEnumAuthenticationStatus.Failed;
            AuthenticationRequest.AuthenticationStatus = _SchemaAuthenticator.AuthenticateWithSchema( _CswEncryption, AuthenticationRequest, out User );
            Assert.AreEqual( Expected.ToString(), AuthenticationRequest.AuthenticationStatus.ToString(), "User was not archived." );
        }
    }
}
