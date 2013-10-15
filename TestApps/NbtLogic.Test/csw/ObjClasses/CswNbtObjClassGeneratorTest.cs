using System;
using System.Collections;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using NUnit.Framework;

namespace ChemSW.Nbt.Test.ObjClasses
{
    [TestFixture]
    public class CswNbtObjClassGeneratorTest
    {
        #region Setup and Teardown

        private TestData TestData;

        [SetUp]
        public void MyTestInitialize()
        {
            TestData = new TestData { FinalizeNodes = true };
        }

        [TearDown]
        public void MyTestCleanup()
        {
            TestData.Destroy();
        }

        #endregion

        #region updateNextDueDate

        /// <summary>
        /// Given a weekly generator (every Monday) with no warning days,
        /// when the generator is saved, assert that the NextDueDate is set to next Monday
        /// </summary>
        [Test]
        public void updateNextDueDateTest_NewGenerator()
        {
            CswNbtObjClassGenerator GeneratorNode = TestData.Nodes.createGeneratorNode( CswEnumRateIntervalType.WeeklyByDay );
            Assert.AreEqual( getNextDay( DayOfWeek.Monday ), GeneratorNode.NextDueDate.DateTimeValue );
        }

        /// <summary>
        /// Given a weekly generator (every Monday) with no warning days,
        /// when the generator's duedate is updated, assert that the NextDueDate is set to next Monday
        /// </summary>
        [Test]
        public void updateNextDueDateTest_ExplicitUpdate()
        {
            CswNbtObjClassGenerator GeneratorNode = TestData.Nodes.createGeneratorNode( CswEnumRateIntervalType.WeeklyByDay );
            GeneratorNode.updateNextDueDate( ForceUpdate: false, DeleteFutureNodes: false );
            Assert.AreEqual( getNextDay( DayOfWeek.Monday ), GeneratorNode.NextDueDate.DateTimeValue );
        }

        /// <summary>
        /// Given a weekly generator (every Monday) with no warning days,
        /// when the generator's duedate is explicitly set to today, assert that the NextDueDate is still set to Today (see Case 30812)
        /// </summary>
        [Test]
        public void updateNextDueDateTest_ExplicitSet()
        {
            CswNbtObjClassGenerator GeneratorNode = TestData.Nodes.createGeneratorNode( CswEnumRateIntervalType.WeeklyByDay );
            GeneratorNode.NextDueDate.DateTimeValue = DateTime.Today;
            GeneratorNode.postChanges( true );
            Assert.AreEqual( DateTime.Today, GeneratorNode.NextDueDate.DateTimeValue );
        }

        /// <summary>
        /// Given a weekly generator with a warning days of 1,
        /// when the generator's finalDueDate is updated to a date after NextDueDate, assert that the NextDueDate remains unchanged
        /// Prior to resolving Case 30022, this test failed.
        /// </summary>
        [Test]
        public void updateNextDueDateTest_FinalDueDateAfterNextDueDate()
        {
            DayOfWeek FiveDaysFromNow = DateTime.Today.AddDays( 5 ).DayOfWeek;
            CswNbtObjClassGenerator GeneratorNode = TestData.Nodes.createGeneratorNode( CswEnumRateIntervalType.WeeklyByDay, Days: new SortedList { { FiveDaysFromNow, FiveDaysFromNow } }, WarningDays: 1 );
            CswNbtObjClassGenerator ExistingGen = TestData.CswNbtResources.Nodes[GeneratorNode.NodeId];
            ExistingGen.FinalDueDate.DateTimeValue = DateTime.Today.AddDays( 7 );
            ExistingGen.postChanges( false );
            Assert.AreNotEqual( DateTime.MinValue, ExistingGen.NextDueDate.DateTimeValue );
        }

        /// <summary>
        /// Given a weekly generator with a warning days of 1,
        /// when the generator's finalDueDate is updated to a date before NextDueDate,
        /// assert that the NextDueDate is set to DateTime.MinValue
        /// </summary>
        [Test]
        public void updateNextDueDateTest_FinalDueDateBeforeNextDueDate()
        {
            DayOfWeek FiveDaysFromNow = DateTime.Today.AddDays( 5 ).DayOfWeek;
            CswNbtObjClassGenerator GeneratorNode = TestData.Nodes.createGeneratorNode( CswEnumRateIntervalType.WeeklyByDay, Days: new SortedList { { FiveDaysFromNow, FiveDaysFromNow } }, WarningDays: 1 );
            CswNbtObjClassGenerator ExistingGen = TestData.CswNbtResources.Nodes[GeneratorNode.NodeId];
            ExistingGen.FinalDueDate.DateTimeValue = DateTime.Today;
            ExistingGen.postChanges( false );
            Assert.AreEqual( DateTime.MinValue, ExistingGen.NextDueDate.DateTimeValue );
        }

        /// <summary>
        /// Given a monthly by date generator (every 15th) with no warning days,
        /// when the generator is saved and updated a second time, assert that the NextDueDate is still set to the next 15th
        /// Prior to resolving Case 30022, this test failed.
        /// </summary>
        [Test]
        public void updateNextDueDateTest_DeleteFutureMonthly()
        {
            CswNbtObjClassGenerator GeneratorNode = TestData.Nodes.createGeneratorNode( CswEnumRateIntervalType.MonthlyByDate );
            CswNbtObjClassGenerator ExistingGen = TestData.CswNbtResources.Nodes[GeneratorNode.NodeId];
            ExistingGen.updateNextDueDate( ForceUpdate: false, DeleteFutureNodes: true );
            Assert.AreEqual( getNextDate( 15 ), ExistingGen.NextDueDate.DateTimeValue );
        }

        /// <summary>
        /// Given a monthly by date generator (every 15th) that skipped a duedate,
        /// when the generator's duedate is cleared and reset, assert that the proper next duedate has been restored
        /// This is a test to figure out what's needed to write the update script for Case 30022 to fix existing NextDueDate errors.
        /// </summary>
        [Test]
        public void updateNextDueDateTest_ForceUpdateRevert()
        {
            CswNbtObjClassGenerator GeneratorNode = TestData.Nodes.createGeneratorNode( CswEnumRateIntervalType.MonthlyByDate );
            CswNbtObjClassGenerator ExistingGen = TestData.CswNbtResources.Nodes[GeneratorNode.NodeId];
            ExistingGen.updateNextDueDate( ForceUpdate: true, DeleteFutureNodes: false );
            ExistingGen.postChanges( true );
            Assert.AreEqual( getNextDate( 15 ).AddMonths( 1 ), ExistingGen.NextDueDate.DateTimeValue );
            DateTime LastDueDate = ExistingGen.DueDateInterval.getLastOccuranceBefore( ExistingGen.NextDueDate.DateTimeValue );
            if( LastDueDate > DateTime.Today )
            {
                ExistingGen.NextDueDate.DateTimeValue = DateTime.Now;
                ExistingGen.updateNextDueDate( ForceUpdate: true, DeleteFutureNodes: false );
                ExistingGen.postChanges( true );
            }
            Assert.AreEqual( getNextDate( 15 ), ExistingGen.NextDueDate.DateTimeValue );
        }

        /// <summary>
        /// Given a weekly generator with a future start date, when the generator is created, 
        /// assert that the NextDueDate has been set to the first scheduled day past the Start Date
        /// Prior to resolving Case 30114, this test failed.
        /// </summary>
        [Test]
        public void updateNextDueDateTest_FutureStartDateEnforced()
        {
            DateTime StartDate = DateTime.Today.AddDays( 10 );
            CswNbtObjClassGenerator GeneratorNode = TestData.Nodes.createGeneratorNode( 
                CswEnumRateIntervalType.WeeklyByDay, 
                Days: new SortedList
                          {
                              { DayOfWeek.Monday, DayOfWeek.Monday }, 
                              { DayOfWeek.Wednesday, DayOfWeek.Wednesday }, 
                              { DayOfWeek.Friday, DayOfWeek.Friday }
                          }, 
                WarningDays: 0,
                StartDate: StartDate );
            CswNbtObjClassGenerator ExistingGen = TestData.CswNbtResources.Nodes[GeneratorNode.NodeId];
            Assert.IsTrue( ExistingGen.NextDueDate.DateTimeValue >= StartDate,
                "NextDueDate (" + ExistingGen.NextDueDate.DateTimeValue.ToShortDateString() + ") is not greater than StartDate (" + StartDate.ToShortDateString() + ")." );
        }

        /// <summary>
        /// Given a weekly generator with a warning days of 5 whose StartDate is in the future,
        /// given that today is within the WarningDays limit of the initial NextDueDate,
        /// when the generator's NextDueDate is updated,
        /// assert that the NextDueDate is set to one week after the previous NextDueDate
        /// </summary>
        [Test]
        public void updateNextDueDateTest_NextDueDateEqualsStartDate()
        {
            DayOfWeek ThreeDaysFromNow = DateTime.Today.AddDays( 3 ).DayOfWeek;
            CswNbtObjClassGenerator GeneratorNode = TestData.Nodes.createGeneratorNode( CswEnumRateIntervalType.WeeklyByDay, Days: new SortedList { { ThreeDaysFromNow, ThreeDaysFromNow } }, StartDate: DateTime.Today.AddDays( 3 ), WarningDays: 5 );
            Assert.AreEqual( GeneratorNode.DueDateInterval.getStartDate(), GeneratorNode.NextDueDate.DateTimeValue );
            CswNbtObjClassGenerator ExistingGen = TestData.CswNbtResources.Nodes[GeneratorNode.NodeId];
            ExistingGen.updateNextDueDate( ForceUpdate: true, DeleteFutureNodes: false );
            Assert.AreEqual( DateTime.Today.AddDays( 10 ), ExistingGen.NextDueDate.DateTimeValue );
        }

        /// <summary>
        /// Given a weekly generator whose StartDate is today,
        /// assert that the NextDueDate is set to one week from today
        /// </summary>
        [Test]
        public void updateNextDueDateTest_AWeekFromToday()
        {
            DayOfWeek Today = DateTime.Today.DayOfWeek;
            CswNbtObjClassGenerator GeneratorNode = TestData.Nodes.createGeneratorNode( CswEnumRateIntervalType.WeeklyByDay, Days: new SortedList { { Today, Today } }, StartDate: DateTime.Today );
            Assert.AreEqual( DateTime.Today.AddDays( 7 ), GeneratorNode.NextDueDate.DateTimeValue );
        }

        /// <summary>
        /// Given a weekly generator with a warning days of 5 whose StartDate is three weeks ago,
        /// given that the NextDueDate is one week later than the StartDate (two weeks ago),
        /// when the generator's NextDueDate is updated,
        /// assert that the NextDueDate is set to the next week after the previous NextDueDate (one week ago)
        /// Prior to resolving Case 30308, this test failed.
        /// </summary>
        [Test]
        public void updateNextDueDateTest_NextDueDateStillInPast()
        {
            DayOfWeek ThreeDaysFromNow = DateTime.Today.AddDays( 3 ).DayOfWeek;
            CswNbtObjClassGenerator GeneratorNode = TestData.Nodes.createGeneratorNode( CswEnumRateIntervalType.WeeklyByDay, Days: new SortedList { { ThreeDaysFromNow, ThreeDaysFromNow } }, StartDate: DateTime.Today.AddDays( 3 - 21 ), WarningDays: 5 );
            GeneratorNode.postChanges( true );
            CswNbtObjClassGenerator ExistingGen = TestData.CswNbtResources.Nodes[GeneratorNode.NodeId];
            ExistingGen.NextDueDate.DateTimeValue = DateTime.Today.AddDays( 3 - 14 );
            Assert.AreEqual( DateTime.Today.AddDays( 3 - 14 ), ExistingGen.NextDueDate.DateTimeValue );
            ExistingGen.updateNextDueDate( ForceUpdate: true, DeleteFutureNodes: false );
            Assert.AreEqual( DateTime.Today.AddDays( 3 - 7 ), ExistingGen.NextDueDate.DateTimeValue );
        }

        #endregion updateNextDueDate

        #region Private Helper Functions

        //If the requested day is today, it grabs next week's date
        private DateTime getNextDay( DayOfWeek Day )
        {
            DateTime NextDay = DateTime.Today.AddDays( 1 );
            while( NextDay.DayOfWeek != Day )
            {
                NextDay = NextDay.AddDays( 1 );
            }
            return NextDay;
        }

        //If the requested date is today, it grabs next month's date
        private DateTime getNextDate( Int32 DayOfMonth )
        {
            DateTime NextDay = DateTime.Today;
            if( NextDay.Day == DayOfMonth )
            {
                NextDay = NextDay.AddMonths( 1 );
            }
            while( NextDay.Day != DayOfMonth )
            {
                NextDay = NextDay.AddDays( 1 );
            }
            return NextDay;
        }

        #endregion Private Helper Functions

    }
}
