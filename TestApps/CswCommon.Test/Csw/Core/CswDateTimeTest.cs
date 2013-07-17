﻿using ChemSW;
using ChemSW.Config;
using ChemSW.Core;
using NUnit.Framework;
using System;

namespace CswCommon.Test.Csw.Core
{
    [TestFixture]
    public class CswDateTimeTest
    {
        private CswResources _CswResources;

        [SetUp]
        public void MyTestInitialize()
        {
            _CswResources = CswResourceFactory.make( CswEnumAppType.Nbt, CswEnumSetupMode.TestProject, IsDeleteModeLogical: false );
        }

        [TearDown]
        public void MyTestCleanup()                                                                             
        {
        }


        [Test]
        public void XmlDateFormatTest()
        {
            string d1 = "2012-11-06T09:30:15.1234";
            CswDateTime cd1 = new CswDateTime( _CswResources );
            cd1.FromXmlDateTimeFormat( d1 );
            Assert.IsTrue( _checkDateTimeParts( cd1.ToDateTime(), 2012, 11, 6, 9, 30, 15, 123 ) );

            string d2 = "1970-01-01T00:00:00";
            CswDateTime cd2 = new CswDateTime( _CswResources );
            cd2.FromXmlDateTimeFormat( d2 );
            Assert.IsTrue( _checkDateTimeParts( cd2.ToDateTime(), 1970, 1, 1, 0, 0, 0, 0 ) );

            string d3 = "1970-01-01T00:00:00.0000";
            CswDateTime cd3 = new CswDateTime( _CswResources );
            cd3.FromXmlDateTimeFormat( d3 );
            Assert.IsTrue( _checkDateTimeParts( cd3.ToDateTime(), 1970, 1, 1, 0, 0, 0, 0 ) );

            string d4 = "2099-02-28T18:24:48";
            CswDateTime cd4 = new CswDateTime( _CswResources );
            cd4.FromXmlDateTimeFormat( d4 );
            Assert.IsTrue( _checkDateTimeParts( cd4.ToDateTime(), 2099, 2, 28, 18, 24, 48, 0 ) );
        } // XmlDateFormatTest()

        private bool _checkDateTimeParts( DateTime DT, int Year, int Month, int Day, int Hour, int Minute, int Second, int Millisecond )
        {
            return ( DT.Year == Year &&
                     DT.Month == Month &&
                     DT.Day == Day &&
                     DT.Hour == Hour &&
                     DT.Minute == Minute &&
                     DT.Second == Second &&
                     DT.Millisecond == Millisecond );
        } // _checkDateTimeParts

    } // CswDateTimeTest
}
