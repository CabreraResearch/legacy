using System;
using NUnit.Framework;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Test.PropTypes
{
    [TestFixture]
    public class CswNbtNodePropCasNoTest
    {
        #region Setup and Teardown

        //private TestData TestData;

        [SetUp]
        public void MyTestInitialize()
        {
            //TestData = new TestData();
        }

        [TearDown]
        public void MyTestCleanup()
        {
            //TestData.DeleteTestNodes();
            //TestData.RevertNodeTypePropAttributes();
        }

        #endregion

        /// <summary>
        /// Assert correct results from CASNo validation
        /// </summary>
        [Test]
        public void testValidation()
        {
            // Good
            string ErrorMessage;
            Assert.IsTrue( CswNbtNodePropCASNo.Validate( "1333-74-0", out ErrorMessage ) ); // Hydrogen
            Assert.IsNullOrEmpty( ErrorMessage );
            Assert.IsTrue( CswNbtNodePropCASNo.Validate( "67-64-1", out ErrorMessage ) ); // Acetone
            Assert.IsNullOrEmpty( ErrorMessage );
            Assert.IsTrue( CswNbtNodePropCASNo.Validate( "1435052-28-0", out ErrorMessage ) ); // Something new
            Assert.IsNullOrEmpty( ErrorMessage );

            // Bad
            Assert.IsFalse( CswNbtNodePropCASNo.Validate( "dhkfjh9834y7hj.=sd\\@^Tf32i5jkds", out ErrorMessage ) ); // Completely bonkers
            Assert.IsNotNullOrEmpty( ErrorMessage );
            Assert.IsFalse( CswNbtNodePropCASNo.Validate( "3289540", out ErrorMessage ) ); //  Numeric but invalid
            Assert.IsNotNullOrEmpty( ErrorMessage );
            Assert.IsFalse( CswNbtNodePropCASNo.Validate( "", out ErrorMessage ) ); // Empty
            Assert.IsNotNullOrEmpty( ErrorMessage );
            Assert.IsFalse( CswNbtNodePropCASNo.Validate( "--", out ErrorMessage ) ); //  just dashes
            Assert.IsNotNullOrEmpty( ErrorMessage );
            Assert.IsFalse( CswNbtNodePropCASNo.Validate( "67-64-", out ErrorMessage ) ); // missing checksum
            Assert.IsNotNullOrEmpty( ErrorMessage );
            Assert.IsFalse( CswNbtNodePropCASNo.Validate( "64-67-1", out ErrorMessage ) ); // bad checksum
            Assert.IsNotNullOrEmpty( ErrorMessage );
        }
    }
}
