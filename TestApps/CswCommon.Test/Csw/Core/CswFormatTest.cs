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
            string[] UnformattedNames = { "abc!", "1'23", "(abc123)", "abc123", "", "name (demo)", "this_is_valid", "_this_is_not" };
            string[] ExpectedFormattedNames = { "abc", "n123", "abc123", "abc123", "n", "name demo", "this_is_valid", "n_this_is_not" };

            Assert.AreEqual( UnformattedNames.Length, ExpectedFormattedNames.Length );
            for( int i = 0; i < UnformattedNames.Length; i++ )
            {
                Assert.AreEqual( ExpectedFormattedNames[i], CswFormat.MakeIntoValidName( UnformattedNames[i] ) );
            }
        }
    }
}
