using System;
using System.Collections;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.TreeEvents;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;
using ChemSW.TblDn;

namespace ChemSW.Nbt.Sched
{

    public class CswNbtSchdItemGenerateNode: CswNbtSchdItem
    {
        private CswNbtResources _CswNbtResources = null;
        private CswNbtNode _CswNbtNodeGenerator;

        public CswNbtSchdItemGenerateNode( CswNbtResources CswNbtResources, CswNbtNode CswNbtNodeGenerator )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtNodeGenerator = CswNbtNodeGenerator;
            SchedItemName = "GenerateNode";
        }//ctor

        public override void reset()
        {
            _Succeeded = true;
            _StatusMessage = string.Empty;
        }//

        /// <summary>
        /// Determines whether the Generator is due for running
        /// </summary>
        /// <remarks>If you change this, consider changing CswNbtSchdItemGenerateEmailReport.doesItemRunNow()</remarks>
        override public bool doesItemRunNow()
        {
            bool ReturnVal = false;

            CswNbtObjClassGenerator GeneratorNode = CswNbtNodeCaster.AsGenerator( _CswNbtNodeGenerator);
            if( GeneratorNode.Enabled.Checked == Tristate.True )
            {
				DateTime ThisDueDateValue = GeneratorNode.NextDueDate.DateTimeValue.Date;
                DateTime InitialDueDateValue = GeneratorNode.DueDateInterval.getStartDate().Date;
				DateTime FinalDueDateValue = GeneratorNode.FinalDueDate.DateTimeValue.Date;

                // BZ 7866
                if( ThisDueDateValue != DateTime.MinValue )
                {
                    // BZ 7124 - set runtime
					if( GeneratorNode.RunTime.DateTimeValue != DateTime.MinValue )
						ThisDueDateValue = ThisDueDateValue.AddTicks( GeneratorNode.RunTime.DateTimeValue.TimeOfDay.Ticks );

                    Int32 WarnDays = (Int32) GeneratorNode.WarningDays.Value;
                    if( WarnDays > 0 )
                    {
                        TimeSpan WarningDaysSpan = new TimeSpan( WarnDays, 0, 0, 0, 0 );
                        ThisDueDateValue = ThisDueDateValue.Subtract( WarningDaysSpan );
                        InitialDueDateValue = InitialDueDateValue.Subtract( WarningDaysSpan );
                    }

                    // if we're within the initial and final due dates, but past the current due date (- warning days) and runtime
                    if( ( DateTime.Now.Date >= InitialDueDateValue ) &&
                        ( DateTime.Now.Date <= FinalDueDateValue || DateTime.MinValue.Date == FinalDueDateValue ) &&
                        ( DateTime.Now >= ThisDueDateValue ) )
                    {
                        ReturnVal = true;
                    }
                } // if( ThisDueDateValue != DateTime.MinValue )
            } // if( GeneratorNode.Enabled.Checked == Tristate.True )
            return ( ReturnVal );

        }//doesItemRunNow() 


        override public void run()
        {
            try
            {
                CswNbtActGenerateNodes CswNbtActGenerateNodes = new CswNbtActGenerateNodes( _CswNbtResources );
                CswNbtActGenerateNodes.makeNode( _CswNbtNodeGenerator );

                if( null != OnScheduleItemWasRun )
                {
                    OnScheduleItemWasRun( this, _CswNbtNodeGenerator );
                }

            }//try

            catch( Exception Exception )
            {
                _Succeeded = false;
                _StatusMessage = "Error running Generator " + Name + ": " + Exception.Message;
            }

        }//run()

        override public string Name 
        {
            get
            {
                string ReturnVal = "";
                if( _CswNbtNodeGenerator != null )
                    ReturnVal = "Generate Node (for Node: " + _CswNbtNodeGenerator.NodeName + ")";
                else
                    ReturnVal = "Generate Node (for Unknown)";

                return (  ReturnVal );

            }//get

        }//Name

        private bool _Succeeded = true;
        override public bool Succeeded
        {
            get
            {
                return( _Succeeded );
            }//

        }//Succeeded

        private string _StatusMessage = "";
        override public string StatusMessage
        {
            get
            {
                return( _StatusMessage );
            }//

        }//StatusMessage

    }//CswNbtSchdItemGenerateNode

}//namespace ChemSW.Nbt.Sched
