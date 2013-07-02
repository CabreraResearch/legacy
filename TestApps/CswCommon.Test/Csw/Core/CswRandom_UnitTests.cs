using ChemSW.Core;
using NUnit.Framework;

namespace CswCommon.Test.Csw.Core
{
    [TestFixture]
    public class CswRandom_UnitTests
    {
        [Test]
        public void CswRandomStringComplexAll_UnitTest()
        {
            bool AllPass = true;
            for( int i = 0; i < 1000; i += 1 )
            {
                string RdmStr = CswRandom.RandomString();
                //All strings are 12 characters and at least 1 out of 1000 has a letter, a number and a special char
                AllPass = RdmStr.Length == 12 && 
                    AllPass && 
                    CswTools.HasAlpha( RdmStr ) && 
                    CswTools.HasNumber( RdmStr ) && 
                    CswTools.HasSpecialCharacter( RdmStr );
            }
            Assert.IsTrue(AllPass, "Not all strings met the complexity requirements.");
        }

        [Test]
        public void CswRandomStringComplexAlphaNumeric_UnitTest()
        {
            bool AllPass = true;
            for ( int i = 0; i < 1000; i += 1 )
            {
                string RdmStr = CswRandom.RandomString( new CswRandom.Config {IncludeLetters = true, IncludeNumbers = true, IncludeSymbols = false, Length = 16 } );
                //All strings are 16 characters, none has a special character and at least 1 out of 1000 has both a letter and a number 
                AllPass = RdmStr.Length == 16 && 
                    false == CswTools.HasSpecialCharacter( RdmStr )  && 
                    AllPass && 
                    CswTools.HasAlpha( RdmStr ) && 
                    CswTools.HasNumber( RdmStr );
            }
            Assert.IsTrue( AllPass, "Not all strings met the complexity requirements." );
        }


        [Test]
        public void CswRandomStringComplexAlpha_UnitTest()
        {
            bool AllPass = true;
            for ( int i = 0; i < 1000; i += 1 )
            {
                string RdmStr = CswRandom.RandomString( new CswRandom.Config { IncludeLetters = true, IncludeNumbers = false, IncludeSymbols = false, Length = 9 } );
                //All strings are 9 characters, none has a special character or a number and all have letters
                AllPass = AllPass &&
                    RdmStr.Length == 9 &&
                    false == CswTools.HasSpecialCharacter( RdmStr ) &&
                    false == CswTools.HasNumber( RdmStr ) &&
                    CswTools.HasAlpha( RdmStr );
            }
            Assert.IsTrue( AllPass, "Not all strings met the complexity requirements." );
        }


    }
}
