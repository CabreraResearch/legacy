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
        /// when the generator's duedate is explicitly set to today, assert that the NextDueDate is set to next Monday
        /// </summary>
        [Test]
        public void updateNextDueDateTest_ExplicitSet()
        {
            CswNbtObjClassGenerator GeneratorNode = TestData.Nodes.createGeneratorNode( CswEnumRateIntervalType.WeeklyByDay );
            GeneratorNode.NextDueDate.DateTimeValue = DateTime.Today;
            GeneratorNode.postChanges( true );
            Assert.AreEqual( getNextDay( DayOfWeek.Monday ), GeneratorNode.NextDueDate.DateTimeValue );
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
                ExistingGen.NextDueDate.DateTimeValue = DateTime.MinValue;
                ExistingGen.updateNextDueDate( ForceUpdate: true, DeleteFutureNodes: false );
                ExistingGen.postChanges( true );
            }
            Assert.AreEqual( getNextDate( 15 ), ExistingGen.NextDueDate.DateTimeValue );
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
            while( NextDay.Day != DayOfMonth )
            {
                NextDay = NextDay.AddDays( 1 );
            }
            return NextDay;
        }

        #endregion Private Helper Functions

    }
}
