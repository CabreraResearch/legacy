using System;
using System.Collections;
using ChemSW;
using ChemSW.Core;
using ChemSW.Config;
using NUnit.Framework;

namespace CswCommon.Test.Csw.Core
{
    [TestFixture]
    public class CswRateIntervalTest
    {
        private CswResources _CswResources;

        [SetUp]
        public void MyTestInitialize()
        {
            _CswResources = CswResourceFactory.make( AppType.Nbt, SetupMode.TestProject, false, false );
        }

        [TearDown]
        public void MyTestCleanup()
        {
        }


        [Test]
        public void HourlyTest()
        {
            CswRateInterval r = new CswRateInterval( _CswResources );
            DateTime StartDate = new DateTime( 2012, 10, 5 );
            for( Int32 h = 1; h < 24; h++ )
            {
                r.setHourly( h, StartDate );
                Assert.AreEqual( r.getFirst(), StartDate );
                DateTime CurrentDate = StartDate;
                for( Int32 d = 1; d <= 10; d++ )
                {
                    CurrentDate = CurrentDate.AddHours( h );
                    Assert.AreEqual( r.getNext( CurrentDate ), CurrentDate.AddHours( h ) );
                }
            }
        }

        [Test]
        public void WeeklyByDayTest()
        {
            CswRateInterval r = new CswRateInterval( _CswResources );
            DateTime StartDate = new DateTime( 2012, 10, 5 );
            SortedList Days = new SortedList();
            Days.Add( DayOfWeek.Monday, DayOfWeek.Monday );
            Days.Add( DayOfWeek.Wednesday, DayOfWeek.Wednesday );
            Days.Add( DayOfWeek.Friday, DayOfWeek.Friday );
            r.setWeeklyByDay( Days, StartDate );

            Assert.AreEqual( r.getFirst(), StartDate );
            DateTime CurrentDate = StartDate;
            for( Int32 i = 1; i <= 365; i++ )
            {
                CurrentDate = CurrentDate.AddDays( 1 );
                DateTime TargetDate = CurrentDate.AddDays( 1 );
                while( TargetDate.DayOfWeek != DayOfWeek.Monday &&
                       TargetDate.DayOfWeek != DayOfWeek.Wednesday &&
                       TargetDate.DayOfWeek != DayOfWeek.Friday )
                {
                    TargetDate = TargetDate.AddDays( 1 );
                }
                Assert.AreEqual( r.getNext( CurrentDate ), TargetDate );
            }
        }

        [Test]
        public void MonthlyByDateTest()
        {
            CswRateInterval r = new CswRateInterval( _CswResources );
            DateTime StartDate = new DateTime( 2012, 10, 15 );

            r.setMonthlyByDate( 1, 15, 10, 2012 );
            Assert.AreEqual( r.getFirst(), StartDate );
            DateTime CurrentDate = StartDate;
            for( Int32 i = 1; i <= 24; i++ )
            {
                CurrentDate = CurrentDate.AddMonths( 1 );
                Assert.AreEqual( r.getNext( CurrentDate ), CurrentDate.AddMonths( 1 ) );
            }
        }

        [Test]
        public void MonthlyByWeekAndDayTest()
        {
            CswRateInterval r = new CswRateInterval( _CswResources );
            DateTime StartDate = new DateTime( 2012, 10, 8 );

            r.setMonthlyByWeekAndDay( 1, 2, DayOfWeek.Monday, 10, 2012 );
            Assert.AreEqual( r.getFirst(), StartDate );
            DateTime CurrentDate = StartDate;
            for( Int32 i = 1; i <= 24; i++ )
            {
                CurrentDate = CurrentDate.AddMonths( 1 );
                DateTime TargetDate = CurrentDate;
                while( TargetDate.Day < 15 && TargetDate.DayOfWeek != DayOfWeek.Monday )
                {
                    TargetDate = TargetDate.AddDays( 1 );
                }
                Assert.AreEqual( r.getNext( CurrentDate.AddDays( -1 ) ), TargetDate );
            }
        }

        [Test]
        public void YearlyTest()
        {
            CswRateInterval r = new CswRateInterval( _CswResources );
            DateTime StartDate = new DateTime( 2012, 10, 15 );

            r.setYearlyByDate( StartDate );
            Assert.AreEqual( r.getFirst(), StartDate );
            DateTime CurrentDate = StartDate;
            for( Int32 i = 1; i <= 24; i++ )
            {
                CurrentDate = CurrentDate.AddYears( 1 );
                Assert.AreEqual( r.getNext( CurrentDate ), CurrentDate.AddYears( 1 ) );
            }
        }

    }
}
