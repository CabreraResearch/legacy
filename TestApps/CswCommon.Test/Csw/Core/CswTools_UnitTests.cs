﻿using ChemSW.Core;
using NUnit.Framework;

namespace CswCommon.Test.Csw.Core
{
    [TestFixture]
    public class CswTools_UnitTests
    {
        [Test]
        public void CswDateFormatToPhpDateFormat_UnitTest()
        {
            foreach( CswEnumDateFormat Format in CswEnumDateFormat.All )
            {
                switch( Format )
                {
                    case CswEnumDateFormat.Mdyyyy:
                        const string PhpMdyyyy = "n/j/Y";
                        string ConversionNjy = CswTools.ConvertNetToPHP( Format );
                        Assert.AreEqual( PhpMdyyyy, ConversionNjy, "Valid PHP format [" + PhpMdyyyy + "] is not equal to Converted format [" + ConversionNjy + "]!");
                        break;
                    case CswEnumDateFormat.dMyyyy:
                        const string PhpDmyyyy = "j-n-Y";
                        string ConversionDmyyyy = CswTools.ConvertNetToPHP( Format );
                        Assert.AreEqual( PhpDmyyyy, ConversionDmyyyy, "Valid PHP format [" + PhpDmyyyy + "] is not equal to Converted format [" + ConversionDmyyyy + "]!" );
                        break;
                    case CswEnumDateFormat.ddMMMyyyy:
                        const string PhpDdMMMyyyy = "d M Y";
                        string ConversionDdmmmyyyy = CswTools.ConvertNetToPHP( Format );
                        Assert.AreEqual( PhpDdMMMyyyy, ConversionDdmmmyyyy, "Valid PHP format [" + PhpDdMMMyyyy + "] is not equal to Converted format [" + ConversionDdmmmyyyy + "]!" );
                        break;
                    case CswEnumDateFormat.yyyyMMdd_Dashes:
                        const string PhpYyyyMMdd = "Y-m-d";
                        string ConversionYyyyMMdd = CswTools.ConvertNetToPHP( Format );
                        Assert.AreEqual( PhpYyyyMMdd, ConversionYyyyMMdd, "Valid PHP format [" + PhpYyyyMMdd + "] is not equal to Converted format [" + ConversionYyyyMMdd + "]!" );
                        break;
                    case CswEnumDateFormat.yyyyMd:
                        const string PhpYyyyMd = "Y/n/j";
                        string ConversionYyyyMMd = CswTools.ConvertNetToPHP( Format );
                        Assert.AreEqual( PhpYyyyMd, ConversionYyyyMMd, "Valid PHP format [" + PhpYyyyMd + "] is not equal to Converted format [" + ConversionYyyyMMd + "]!" );
                        break;
                }
            }
        }

        [Test]
        public void CswTimeFormatToPhpDateFormat_UnitTest()
        {
            foreach( CswEnumTimeFormat Format in CswEnumTimeFormat.All )
            {
                switch( Format )
                {
                    case CswEnumTimeFormat.Hmmss:
                        const string PhpHmmss = "G:i:s";
                        string ConversionHmmss = CswTools.ConvertNetToPHP( Format );
                        Assert.AreEqual( PhpHmmss, ConversionHmmss, "Valid PHP format [" + PhpHmmss + "] is not equal to Converted format [" + ConversionHmmss + "]!" );
                        break;
                    case CswEnumTimeFormat.hmmsstt:
                        const string PhpHmmsstt = "g:i:s a";
                        string ConversionHmmsstt = CswTools.ConvertNetToPHP( Format );
                        Assert.AreEqual( PhpHmmsstt, ConversionHmmsstt, "Valid PHP format [" + PhpHmmsstt + "] is not equal to Converted format [" + ConversionHmmsstt + "]!" );
                        break;
                }
            }
        }

        [Test]
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
