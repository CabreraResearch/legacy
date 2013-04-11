using System;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.MtSched.Sched;
using ChemSW.Nbt.ChemCatCentral;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicNbtExtChemDataSync : ICswScheduleLogic
    {
        #region Properties

        private CswEnumScheduleLogicRunStatus _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        public CswEnumScheduleLogicRunStatus LogicRunStatus
        {
            set { _LogicRunStatus = value; }
            get { return ( _LogicRunStatus ); }
        }
        private CswSchedItemTimingFactory _CswSchedItemTimingFactory = new CswSchedItemTimingFactory();
        private CswScheduleLogicDetail _CswScheduleLogicDetail;
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }
        public string RuleName
        {
            get { return ( CswEnumNbtScheduleRuleNames.ExtChemDataSync ); }
        }

        #endregion Properties

        #region Scheduler Methods

        public void initScheduleLogicDetail( CswScheduleLogicDetail CswScheduleLogicDetailIn )
        {
            _CswScheduleLogicDetail = CswScheduleLogicDetailIn;
        }


        public bool hasLoad( ICswResources CswResources )
        {
            //******************* DUMMY IMPLMENETATION FOR NOW **********************//
            return ( true );
            //******************* DUMMY IMPLMENETATION FOR NOW **********************//
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
                    // Get all sync modules
                    Collection<CswEnumNbtModuleName> SyncModules = new Collection<CswEnumNbtModuleName>();
                    SyncModules.Add( CswEnumNbtModuleName.FireDbSync );

                    // Check to see if at least one is enabled
                    if( SyncModules.Any( SyncModule => CswNbtResources.Modules.IsModuleEnabled( SyncModule ) ) )
                    {
                        // Get all nodes that need to be synced.
                        Collection<CswPrimaryKey> MaterialPks = getMaterialPks( CswNbtResources );
                        if( MaterialPks.Count > 0 )
                        {
                            // Check C3 Status
                            CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( CswNbtResources );
                            bool C3ServiceStatus = CswNbtC3ClientManager.checkC3ServiceReferenceStatus( CswNbtResources );
                            if( C3ServiceStatus )
                            {
                                foreach( CswPrimaryKey MaterialPk in MaterialPks )
                                {
                                    CswNbtObjClassMaterial MaterialNode = CswNbtResources.Nodes.GetNode( MaterialPk );

                                    // FireDb Sync Module
                                    MaterialNode.syncFireDbData();
                                    MaterialNode.postChanges( false );

                                    //Todo: Add subsequent sync modules here
                                }
                            }

                            _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Succeeded;
                        }
                    }
                }
                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtExtChemDataSync exception: " + Exception.Message;
                    CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Failed;
                }
            }
        }

        #endregion Scheduler Methods

        #region Schedule-Specific Logic

        public Collection<CswPrimaryKey> getMaterialPks( CswNbtResources CswNbtResources )
        {
            Collection<CswPrimaryKey> MaterialPks = new Collection<CswPrimaryKey>();

            // Create the view
            CswNbtView MaterialsToBeSyncedView = new CswNbtView( CswNbtResources );
            CswNbtMetaDataObjectClass MaterialOC = CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialClass );
            CswNbtViewRelationship ParentRelationship = MaterialsToBeSyncedView.AddViewRelationship( MaterialOC, true );

            CswNbtMetaDataObjectClassProp CasNoOCP = MaterialOC.getObjectClassProp( CswNbtObjClassMaterial.PropertyName.CasNo );
            MaterialsToBeSyncedView.AddViewPropertyAndFilter( ParentRelationship,
                MetaDataProp: CasNoOCP,
                Value: "",
                SubFieldName: CswEnumNbtSubFieldName.Text,
                FilterMode: CswEnumNbtFilterMode.NotNull );

            CswNbtMetaDataObjectClassProp C3SyncDateOCP = MaterialOC.getObjectClassProp( CswNbtObjClassMaterial.PropertyName.C3SyncDate );
            MaterialsToBeSyncedView.AddViewPropertyAndFilter( ParentRelationship,
                MetaDataProp: C3SyncDateOCP,
                Value: "",
                SubFieldName: CswEnumNbtSubFieldName.Value,
                FilterMode: CswEnumNbtFilterMode.Null );

            // Get and iterate the Tree
            ICswNbtTree MaterialPksTree = CswNbtResources.Trees.getTreeFromView( MaterialsToBeSyncedView, false, false, false );
            Int32 MaterialsProcessedPerIteration = CswConvert.ToInt32( CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.NodesProcessedPerCycle ) );
            Int32 MaterialsToSync = MaterialPksTree.getChildNodeCount();
            if( MaterialsToSync > 0 )
            {
                for( int i = 0; i < MaterialsProcessedPerIteration && i < MaterialsToSync; i++ )
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
