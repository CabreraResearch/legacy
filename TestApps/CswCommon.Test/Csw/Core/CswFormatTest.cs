using ChemSW.Core;
using NUnit.Framework;

namespace CswCommon.Test.Csw.Core
{
    [TestFixture]
    public class CswFormatTest
    {
        [Test]
        public void MakeIntoValidNameTest()
        {
            string[] UnformattedNames = { "abc!", "1'23", "(abc123)", "abc123", "" };
            string[] ExpectedFormattedNames = { "abc", "n123", "abc123", "abc123", "n" };

            Assert.AreEqual( UnformattedNames.Length, ExpectedFormattedNames.Length );
            for( int i = 0; i < UnformattedNames.Length; i++ )
            {
                Assert.AreEqual( ExpectedFormattedNames[i], CswFormat.MakeIntoValidName( UnformattedNames[i] ) );
            }
        }
    }
}
