using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.ChemCatCentral;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicNbtExtChemDataSync : ICswScheduleLogic
    {
        #region Properties

        public string RuleName
        {
            get { return ( CswEnumNbtScheduleRuleNames.ExtChemDataSync ); }
        }

        private CswEnumScheduleLogicRunStatus _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        public CswEnumScheduleLogicRunStatus LogicRunStatus
        {
            set { _LogicRunStatus = value; }
            get { return ( _LogicRunStatus ); }
        }

        private CswScheduleLogicDetail _CswScheduleLogicDetail;
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }

        private Collection<CswEnumNbtModuleName> _SyncModules = new Collection<CswEnumNbtModuleName>
            {
                CswEnumNbtModuleName.FireDbSync,
                CswEnumNbtModuleName.PCIDSync,
                CswEnumNbtModuleName.LOLISync,
                CswEnumNbtModuleName.ArielSync
            };

        #endregion Properties

        #region State

        private Collection<CswPrimaryKey> _MaterialPks = new Collection<CswPrimaryKey>();

        private void _setLoad( ICswResources CswResources )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            if( CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.CISPro ) )
            {
                if( _SyncModules.Any( SyncModule => CswNbtResources.Modules.IsModuleEnabled( SyncModule ) ) )
                {
                    // If the date is out of sync, then we get all valid Materials to be synced
                    if( outOfDate( CswNbtResources ) )
                    {
                        _MaterialPks = _getMaterialPks( CswNbtResources );
                        // Set the configuration variable value
                        CswResources.ConfigVbls.setConfigVariableValue( CswConvert.ToString( CswEnumConfigurationVariableNames.C3SyncDate ), CswConvert.ToString( DateTime.Now ) );
                        CswResources.ConfigVbls.saveConfigVariables();
                    }
                }
            }
        }

        #endregion State

        #region Scheduler Methods

        public void initScheduleLogicDetail( CswScheduleLogicDetail LogicDetail )
        {
            _CswScheduleLogicDetail = LogicDetail;
        }

        //Determines the number of material nodes that need to be synced with external data and return that value
        public Int32 getLoadCount( ICswResources CswResources )
        {
            if( _MaterialPks.Count == 0 )
            {
                _setLoad( CswResources );
            }
            return _MaterialPks.Count;
        }

        public void stop()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
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
                    if( _SyncModules.Any( SyncModule => CswNbtResources.Modules.IsModuleEnabled( SyncModule ) ) )
                    {
                        // Check C3 Status
                        CswC3Params CswC3Params = new CswC3Params();
                        CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( CswNbtResources, CswC3Params );
                        SearchClient SearchClient = CswNbtC3ClientManager.initializeC3Client();
                        if( null != SearchClient )
                        {
                            int MaterialsProcessedPerIteration = CswConvert.ToInt32( CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.NodesProcessedPerCycle ) );
                            int TotalProcessedThisIteration = 0;
                            while( TotalProcessedThisIteration < MaterialsProcessedPerIteration && _MaterialPks.Count > 0 && ( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus ) )
                            {
                                CswNbtObjClassChemical MaterialNode = CswNbtResources.Nodes[_MaterialPks[0]];
                                if( null != MaterialNode )
                                {
                                    _setPendingUpdate( CswNbtResources, CswConvert.ToString( MaterialNode.NodeId.PrimaryKey ) );
                                    _MaterialPks.RemoveAt( 0 );
                                    TotalProcessedThisIteration++;
                                } //if (null != MaterialNode)
                            }
                        }
                        else
                        {
                            // TODO: What should we do with the error in the case of the schedule service?
                        }
                    }

                    _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Succeeded; //last line

                }//try
                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtExtChemDataSync exception: " + Exception.Message + "; " + Exception.StackTrace;
                    CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Failed;
                }
            }
        }

        #endregion Scheduler Methods

        #region Schedule-Specific Logic

        private void _setPendingUpdate( CswNbtResources CswNbtResources, string NodeId )
        {
            CswTableUpdate NodesTableUpdate = CswNbtResources.makeCswTableUpdate( "ExtChemDataSync_pendingupdate", "nodes" );
            DataTable NodesTable = NodesTableUpdate.getTable( "where istemp = '0' and nodeid = '" + NodeId + "'" );
            foreach( DataRow NodesRow in NodesTable.Rows )
            {
                NodesRow["pendingupdate"] = "1";
            }
            NodesTableUpdate.update( NodesTable );
        }

        /// <summary>
        /// This method determines whether the C3SyncDate is older than either 
        /// the LastExtChemDataImportDate or the LastRegulationDataImportDate. If it is
        /// out of date, we return true so that a sync is then performed.
        /// </summary>
        /// <param name="CswNbtResources"></param>
        /// <returns></returns>
        private bool outOfDate( CswNbtResources CswNbtResources )
        {
            bool OutOfDate = false;

            CswC3SearchParams CswC3SearchParams = new CswC3SearchParams();
            CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( CswNbtResources, CswC3SearchParams );
            SearchClient SearchClient = CswNbtC3ClientManager.initializeC3Client();
            if( null != SearchClient )
            {
                string LastExtChemDataImportDate = CswNbtC3ClientManager.getLastExtChemDataImportDate( SearchClient );
                string LastRegDataImportDate = CswNbtC3ClientManager.getLastRegulationDataImportDate( SearchClient );

                // Compare the dates and return true if a sync should be performed
                DateTime NbtC3SyncDate = CswConvert.ToDateTime( CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.C3SyncDate ) );

                if( NbtC3SyncDate == DateTime.MinValue ||
                    ( NbtC3SyncDate < CswConvert.ToDateTime( LastExtChemDataImportDate ) || ( false == string.IsNullOrEmpty( LastRegDataImportDate ) && NbtC3SyncDate < CswConvert.ToDateTime( LastRegDataImportDate ) ) ) )
                {
                    OutOfDate = true;
                }
            }
            else
            {
                // TODO: What should we do with the error in the case of the schedule service?
            }

            return OutOfDate;
        }

        private Collection<CswPrimaryKey> _getMaterialPks( CswNbtResources CswNbtResources )
        {
            Collection<CswPrimaryKey> MaterialPks = new Collection<CswPrimaryKey>();

            // Create the view
            CswNbtView MaterialsToBeSyncedView = new CswNbtView( CswNbtResources );
            CswNbtMetaDataObjectClass MaterialOC = CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtViewRelationship ParentRelationship = MaterialsToBeSyncedView.AddViewRelationship( MaterialOC, true );
            CswNbtMetaDataObjectClassProp CasNoOCP = MaterialOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.CasNo );
            MaterialsToBeSyncedView.AddViewPropertyAndFilter( ParentRelationship,
                MetaDataProp: CasNoOCP,
                Value: "",
                SubFieldName: CswEnumNbtSubFieldName.Text,
                FilterMode: CswEnumNbtFilterMode.NotNull );

            // Get and iterate the Tree
            ICswNbtTree MaterialPksTree = CswNbtResources.Trees.getTreeFromView( MaterialsToBeSyncedView, false, false, false );
            Int32 MaterialsToSync = MaterialPksTree.getChildNodeCount();
            if( MaterialsToSync > 0 )
            {
                for( int i = 0; i < MaterialsToSync; i++ )
                {
                    MaterialPksTree.goToNthChild( i );
                    MaterialPks.Add( MaterialPksTree.getNodeIdForCurrentPosition() );
                    MaterialPksTree.goToParentNode();
                }
            }

            return MaterialPks;
        }

        #endregion Schedule-Specific Logic

    }//CswScheduleLogicNbtUpdtMTBF
}//namespace ChemSW.Nbt.Sched
