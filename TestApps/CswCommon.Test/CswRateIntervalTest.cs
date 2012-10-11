using System;
using System.Collections;
using ChemSW;
using ChemSW.Core;
using ChemSW.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CswCommon.Test
{
    [TestClass]
    public class CswRateIntervalTest
    {
        private CswResources _CswResources;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            _CswResources = CswResourceFactory.make( AppType.Nbt, SetupMode.TestProject, false, false );
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
        }


        [TestMethod]
        public void HourlyTest()
        {
            CswRateInterval r = new CswRateInterval( _CswResources );
            DateTime StartDate = new DateTime( 2012, 10, 5, 1, 30, 15 );
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

        [TestMethod]
        public void WeeklyByDayTest()
        {
            CswRateInterval r = new CswRateInterval( _CswResources );
            DateTime StartDate = new DateTime( 2012, 10, 5, 1, 30, 15 );
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
                DateTime TargetDate = CurrentDate.AddDays(1);
                while( TargetDate.DayOfWeek != DayOfWeek.Monday &&
                       TargetDate.DayOfWeek != DayOfWeek.Wednesday &&
                       TargetDate.DayOfWeek != DayOfWeek.Friday )
                {
                    TargetDate = TargetDate.AddDays( 1 );
                }
                Assert.AreEqual( r.getNext( CurrentDate ), TargetDate );
            }
        }

        [TestMethod]
        public void MonthlyByDateTest()
        {
            CswRateInterval r = new CswRateInterval( _CswResources );
            DateTime StartDate = new DateTime( 2012, 10, 15, 1, 30, 15 );
            
            r.setMonthlyByDate( 1, 15, 10, 2012 );
            Assert.AreEqual( r.getFirst(), StartDate );
            DateTime CurrentDate = StartDate;
            for( Int32 i = 1; i <= 24; i++ )
            {
                CurrentDate = CurrentDate.AddMonths( 1 );
                Assert.AreEqual( r.getNext( CurrentDate ), CurrentDate.AddMonths( 1 ) );
            }
        }

        [TestMethod]
        public void MonthlyByWeekAndDayTest()
        {
            CswRateInterval r = new CswRateInterval( _CswResources );
            DateTime StartDate = new DateTime( 2012, 10, 8, 1, 30, 15 );

            r.setMonthlyByWeekAndDay( 1, 2, DayOfWeek.Monday, 10, 2012 );
            Assert.AreEqual( r.getFirst(), StartDate );
            DateTime CurrentDate = StartDate;
            for( Int32 i = 1; i <= 24; i++ )
            {
                CurrentDate = new DateTime( CurrentDate.Year, CurrentDate.Month + 1, 1 );
                DateTime TargetDate = CurrentDate;
                while( TargetDate.Day < 8 && TargetDate.DayOfWeek != DayOfWeek.Monday ) { TargetDate.AddDays( 1 ); }
                Assert.AreEqual( r.getNext( CurrentDate ), TargetDate );
            }
        }

        [TestMethod]
        public void YearlyTest()
        {
            CswRateInterval r = new CswRateInterval( _CswResources );
            DateTime StartDate = new DateTime( 2012, 10, 15, 1, 30, 15 );

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
