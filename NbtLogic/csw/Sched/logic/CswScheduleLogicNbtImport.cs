using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.ImportExport;
using ChemSW.RscAdo;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicNbtImport : ICswScheduleLogic
    {
        #region Properties

        public string RuleName
        {
            get { return ( CswEnumNbtScheduleRuleNames.Import ); }
        }

        private CswEnumScheduleLogicRunStatus _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        public CswEnumScheduleLogicRunStatus LogicRunStatus
        {
            get { return ( _LogicRunStatus ); }
        }

        private CswScheduleLogicDetail _CswScheduleLogicDetail = null;
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }

        public void initScheduleLogicDetail( CswScheduleLogicDetail LogicDetail )
        {
            _CswScheduleLogicDetail = LogicDetail;
        }

        #endregion Properties

        #region State

        private StringCollection _DataTableNames = new StringCollection();

        #endregion State

        #region Scheduler Methods

        public Int32 getLoadCount( ICswResources CswResources )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;

            // Only recalculate load count if it's zero
            if( _DataTableNames.Count == 0 )
            {
                CswNbtImporter Importer = new CswNbtImporter( CswNbtResources );
                _DataTableNames = Importer.getImportDataTableNames();
            }
            _CswScheduleLogicDetail.LoadCount = _DataTableNames.Count;
            return _CswScheduleLogicDetail.LoadCount;
        }

        public void threadCallBack( ICswResources CswResources )
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Running;

            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;

            if( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus )
            {
                try
                {
                    Int32 ImportLimit = CswConvert.ToInt32( CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.NodesProcessedPerCycle ) );
                    if( Int32.MinValue == ImportLimit )
                    {
                        ImportLimit = 10;  // Default
                    }

                    if( _DataTableNames.Count > 0 )
                    {
                        CswNbtImporter Importer = new CswNbtImporter( CswNbtResources );
                        Int32 RowsProcessed;
                        bool MoreToDo = Importer.ImportRows( ImportLimit, _DataTableNames[0], out RowsProcessed );
                        if( false == MoreToDo )
                        {
                            _DataTableNames.RemoveAt( 0 );
                        }
                    }
                    else
                    {
                        _CswScheduleLogicDetail.LoadCount = 0;
                    }
                    _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Succeeded; //last line
                }//try
                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtImport::threadCallBack() exception: " + Exception.Message + "; " + Exception.StackTrace;
                    CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Failed;
                }//catch

            }//if we're not shutting down

        }//threadCallBack()

        public void stop()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        }

        #endregion Scheduler Methods

    }//CswScheduleLogicNbtNodeCounts

}//namespace ChemSW.Nbt.Sched
