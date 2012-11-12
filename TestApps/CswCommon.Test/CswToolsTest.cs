using ChemSW.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CswCommon.Test
{
    [TestClass]
    public class CswToolsTest
    {
        [TestMethod]
        public void IsAlphaNumericTest()
        {
            string[] ValidStrings = { "abc", "123", "abc123" };
            string[] InvalidStrings = { "abc!", "1'23", "(abc123)" };

            foreach( string StringToValidate in ValidStrings )
            {
                Assert.IsTrue( CswTools.IsAlphaNumeric( StringToValidate ) );
            }
            foreach( string StringToValidate in InvalidStrings )
            {
                Assert.IsFalse( CswTools.IsAlphaNumeric( StringToValidate ) );
            }
        }
    }
}
