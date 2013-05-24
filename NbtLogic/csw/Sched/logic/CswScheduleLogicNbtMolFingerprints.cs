using System;
using System.Data;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicNbtMolFingerprints : ICswScheduleLogic
    {
        #region Properties

        public string RuleName
        {
            get { return ( CswEnumNbtScheduleRuleNames.MolFingerprints ); }
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

        private CswCommaDelimitedString _NonFingerprintedMols = new CswCommaDelimitedString();

        private void _setLoad( ICswResources CswResources )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            _NonFingerprintedMols = _getNonFingerPrintedMols( NbtResources );
        }

        #endregion State

        #region Scheduler Methods

        //Determine the number of non-fingerprinted Mols that need to be fingerprinted and return that value
        public Int32 getLoadCount( ICswResources CswResources )
        {
            if( _NonFingerprintedMols.Count == 0 )
            {
                _setLoad( CswResources );
            }
            _CswScheduleLogicDetail.LoadCount = _NonFingerprintedMols.Count;
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
                    int nodesPerIteration = CswConvert.ToInt32( CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.NodesProcessedPerCycle ) );
                    int molsProcessed = 0;
                    while( molsProcessed < nodesPerIteration && _NonFingerprintedMols.Count > 0 && ( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus ) )
                    {
                        int NodeId = CswConvert.ToInt32( _NonFingerprintedMols[0] );
                        CswPrimaryKey NodePK = new CswPrimaryKey( "nodes", NodeId );
                        CswNbtNode node = CswNbtResources.Nodes.GetNode( NodePK );

                        bool hasntBeenInserted = true;
                        foreach( CswNbtNodePropWrapper prop in node.Properties[(CswEnumNbtFieldType) CswEnumNbtFieldType.MOL] )
                        {
                            if( hasntBeenInserted )
                            {
                                CswNbtResources.StructureSearchManager.InsertFingerprintRecord( NodeId, prop.AsMol.Mol );
                                hasntBeenInserted = false;
                            }
                        }
                        _NonFingerprintedMols.RemoveAt( 0 );
                        molsProcessed++;
                    }

                    _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Succeeded; //last line

                }//try
                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtMolFingerprints::GetUpdatedItems() exception: " + Exception.Message + "; " + Exception.StackTrace;
                    CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Failed;
                }//catch

            }//if we're not shutting down

        }//threadCallBack()

        private CswCommaDelimitedString _getNonFingerPrintedMols( CswNbtResources CswNbtResources )
        {
            CswCommaDelimitedString nonFingerprintedMols = new CswCommaDelimitedString();

            string sql = @"select jnp.nodeid from nodetype_props ntp
                                    join jct_nodes_props jnp on ntp.nodetypepropid = jnp.nodetypepropid 
                                        where ntp.fieldtypeid = (select fieldtypeid from field_types ft where ft.fieldtype = 'MOL')
                                            and jnp.clobdata is not null 
                                            and not exists (select nodeid from mol_keys where nodeid = jnp.nodeid)";
            CswArbitrarySelect arbSelect = CswNbtResources.makeCswArbitrarySelect( "getNonFingerprintedMols", sql );

            int lowerBound = 0;
            int upperBound = 500;
            DataTable jctnodesprops = arbSelect.getTable( lowerBound, upperBound, false, false ); //only get up to 500 records to do in a day

            foreach( DataRow row in jctnodesprops.Rows )
            {
                nonFingerprintedMols.Add( row["nodeid"].ToString() );
            }

            return nonFingerprintedMols;
        }

        public void stop()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        }

        #endregion Scheduler Methods

    }//CswScheduleLogicNbtMolFingerpritns

}//namespace ChemSW.Nbt.Sched
