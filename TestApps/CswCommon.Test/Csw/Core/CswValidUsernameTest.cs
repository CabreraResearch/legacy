using ChemSW.Core;
using NUnit.Framework;

namespace CswCommon.Test.Csw.Core
{
    [TestFixture]
    class CswValidUsernameTest
    {
        [Test]
        public void TestValidUsernames()
        {
            const string Username1 = "Batman";
            const string Username2 = "Batman1";
            const string Username3 = "Bat1man";
            const string Username4 = "1Batman";
            const string Username5 = "Bat.man";
            const string Username6 = "Bat-Man";
            const string Username7 = "Bat_Man";
            const string Username8 = "Bat Man";
            const string Username9 = "The.Honey_badger";
            const string Username10 = "The.Honey_badger123";
            const string Username11 = "This is a . weird  user name";
            const string Username12 = "Now .--_ this   _-. is just ridiculous";

            bool Test1 = CswTools.IsValidUsername( Username1 );
            bool Test2 = CswTools.IsValidUsername( Username2 );
            bool Test3 = CswTools.IsValidUsername( Username3 );
            bool Test4 = CswTools.IsValidUsername( Username4 );
            bool Test5 = CswTools.IsValidUsername( Username5 );
            bool Test6 = CswTools.IsValidUsername( Username6 );
            bool Test7 = CswTools.IsValidUsername( Username7 );
            bool Test8 = CswTools.IsValidUsername( Username8 );
            bool Test9 = CswTools.IsValidUsername( Username9 );
            bool Test10 = CswTools.IsValidUsername( Username10 );
            bool Test11 = CswTools.IsValidUsername( Username11 );
            bool Test12 = CswTools.IsValidUsername( Username12 );

            Assert.IsTrue( Test1, "CswTools.IsValidUsername returned false on string \"" + Username1 + "\" when is should have returned true" );
            Assert.IsTrue( Test2, "CswTools.IsValidUsername returned false on string \"" + Username2 + "\" when is should have returned true" );
            Assert.IsTrue( Test3, "CswTools.IsValidUsername returned false on string \"" + Username3 + "\" when is should have returned true" );
            Assert.IsTrue( Test4, "CswTools.IsValidUsername returned false on string \"" + Username4 + "\" when is should have returned true" );
            Assert.IsTrue( Test5, "CswTools.IsValidUsername returned false on string \"" + Username5 + "\" when is should have returned true" );
            Assert.IsTrue( Test6, "CswTools.IsValidUsername returned false on string \"" + Username6 + "\" when is should have returned true" );
            Assert.IsTrue( Test7, "CswTools.IsValidUsername returned false on string \"" + Username7 + "\" when is should have returned true" );
            Assert.IsTrue( Test8, "CswTools.IsValidUsername returned false on string \"" + Username8 + "\" when is should have returned true" );
            Assert.IsTrue( Test9, "CswTools.IsValidUsername returned false on string \"" + Username9 + "\" when is should have returned true" );
            Assert.IsTrue( Test10, "CswTools.IsValidUsername returned false on string \"" + Username10 + "\" when is should have returned true" );
            Assert.IsTrue( Test11, "CswTools.IsValidUsername returned false on string \"" + Username11 + "\" when is should have returned true" );
            Assert.IsTrue( Test12, "CswTools.IsValidUsername returned false on string \"" + Username12 + "\" when is should have returned true" );
        }

        [Test]
        public void TestInvalidUsername()
        {
            const string Username1 = "Batman ";
            const string Username2 = " Batman";
            const string Username3 = "_bat m&n";
            const string Username4 = "manbat!";
            const string Username5 = "123abc cba32+";

            bool Test1 = CswTools.IsValidUsername( Username1 );
            bool Test2 = CswTools.IsValidUsername( Username2 );
            bool Test3 = CswTools.IsValidUsername( Username3 );
            bool Test4 = CswTools.IsValidUsername( Username4 );
            bool Test5 = CswTools.IsValidUsername( Username5 );

            Assert.IsFalse( Test1, "CswTools.IsValidUsername returned true on string \"" + Username1 + "\" when is should have returned false because it has a space at the end" );
            Assert.IsFalse( Test2, "CswTools.IsValidUsername returned true on string \"" + Username2 + "\" when is should have returned false because it has a space at the front" );
            Assert.IsFalse( Test3, "CswTools.IsValidUsername returned true on string \"" + Username3 + "\" when is should have returned false because it has an invalid character" );
            Assert.IsFalse( Test4, "CswTools.IsValidUsername returned true on string \"" + Username4 + "\" when is should have returned false because it has an invalid character" );
            Assert.IsFalse( Test5, "CswTools.IsValidUsername returned true on string \"" + Username5 + "\" when is should have returned false because it has an invalid character" );
        }
    }
}
