using System;
using System.Data;
using System.Threading;
using System.Collections.Generic;
using ChemSW.MtSched.Sched;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Config;
using ChemSW.DB;
using ChemSW.MtSched.Core;

namespace ChemSW.Cis.Sched
{
    public class CswScheduleLogicInfoNbt : CswScheduleLogicDetail
    {
        private CswSchedItemTimingFactory _CswSchedItemTimingFactory = new CswSchedItemTimingFactory();

        private string _RuleName = string.Empty; 
        public CswScheduleLogicInfoNbt( string RuleName )
        {
            _RuleName = RuleName;
        }//ctor

        private ICswResources _CswResources = null;
        public ICswResources CswResources
        {
            set
            {
                _CswResources = value;
            }
        }

        private List<ICswScheduleLogicPersistedDetailParam> _RunParams = null;
        public List<ICswScheduleLogicPersistedDetailParam> RunParams
        {
            get
            {

                //if( null == _RunParams )
                //{
                //    _RunParams = new List<ICswScheduleLogicPersistedDetailParam>();

                //    _CswTableUpdateBgTasksInfo = _CswResources.makeCswTableUpdate( "CswScheduleLogicInfoParams_Update", "background_tasks_info" );

                //    CswCommaDelimitedString CswCommaDelimitedString = new CswCommaDelimitedString();
                //    CswCommaDelimitedString.Add( _ColName_ParameterName );
                //    CswCommaDelimitedString.Add( _ColName_ParameterValue );


                //    string WhereClause = " backgroundtaskid = ( select backgroundtaskid from background_tasks where lower(taskname)='" + _RuleName.ToLower() + "'";
                //    DataTable _BgTasksInfoDataTable = _CswTableUpdateBgTasksInfo.getTable( CswCommaDelimitedString, "", Int32.MinValue, WhereClause, false );

                //    foreach( DataRow CurrentRow in _BgTasksInfoDataTable.Rows )
                //    {
                //        _RunParams.Add( new CswScheduleLogicParamNbt( CurrentRow, _ColName_ParameterName, _ColName_ParameterValue ) );
                //    }//iterate rolws
                //}

                return ( _RunParams );
            }//get

        }//RunParams



        public int MaxRunTimeMs
        {
            set
            {

            }

            get
            {
                Int32 ReturnVal = Int32.MinValue;

                return ( ReturnVal );
            }
        }//MaxRunTimeMs


        public Int32 ThreadId
        {
            set
            {

            }

            get
            {
                Int32 ReturnVal = Int32.MinValue;

                return ( ReturnVal );
            }

        }//ThreadId 

        public bool Reprobate
        {
            set
            {
            }//set

            get
            {
                bool ReturnVal = false;

                return ( ReturnVal );
            }//get
        }


        public bool Rogue
        {
            set
            {

            }//set

            get
            {
                bool ReturnVal = false;

                return ( ReturnVal );
            }//get
        }


		public Int32 TotalRogueCount
        {
            set
            {
            }//set

            get
            {
                Int32 ReturnVal = 0;

                return ( ReturnVal );
            }//get
        }

        public string StatusMessage
        {
            set
            {

            }//set

            get
            {
                string ReturnVal = string.Empty;

                return ( ReturnVal );
            }//get
        }


        public Recurrance Recurrance
        {
            set
            {
            }//set

            get
            {
                Recurrance ReturnVal = Recurrance.Never;

                return ( ReturnVal );

            }//get
        }

        public Int32 Interval
        {
            set
            {
            }

            get
            {
                Int32 ReturnVal = Int32.MinValue;

                return ( ReturnVal );
            }
        }

        public DateTime RunStartTime
        {
            set
            {

            }//set

            get
            {
                DateTime ReturnVal = DateTime.MinValue;

                return ( ReturnVal );

            }//get
        }

        public DateTime RunEndTime
        {
            set
            {

            }//set

            get
            {
                DateTime ReturnVal = DateTime.MinValue;

                return ( ReturnVal );
            }//get
        }

        public Int32 FailedCount
        {
            set
            {
            }//set

            get
            {
                Int32 ReturnVal = Int32.MinValue;

                return ( ReturnVal );
            }//get
        }


        ICswSchedItemTiming _CswSchedItemTiming = null;
        public bool doesItemRunNow()
        {
            return ( false );

        }//doesItemRunNow() 

        public void update()
        {
            bool UpdateOccured = false;
            try
            {
                //if( null != _BgTasksDataTable )
                //{
                //    _CswTableUpdateBgTasks.update( _BgTasksDataTable );
                //    UpdateOccured = true;
                //}

                //if( null != _BgTasksInfoDataTable )
                //{
                //    _CswTableUpdateBgTasksInfo.update( _BgTasksInfoDataTable );
                //    UpdateOccured = true;
                //}

            }

            finally
            {
                //if( true == UpdateOccured )
                //{
                //    _CswResources.commitTransaction();
                //}

                //_BgTasksDataRow = null;
                //_BgTasksInfoDataTable = null;
                //_BgTasksDataTable = null;

                //_CswResources.clearUpdates(); 

//                _CswResources.releaseDbResources(); 
            }

        }//update()

        public void releaseResources()
        {
        }

    }//CswScheduleLogicInfoCis

}//namespace ChemSW.MtSched


