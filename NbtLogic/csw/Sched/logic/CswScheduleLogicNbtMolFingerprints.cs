using System;
using System.Data;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.MtSched.Sched;
using ChemSW.Nbt.Batch;

namespace ChemSW.Nbt.Sched
{

    public class CswScheduleLogicNbtMolFingerprints : ICswScheduleLogic
    {

        public string RuleName
        {
            get { return ( NbtScheduleRuleNames.MolFingerprints.ToString() ); }
        }

        public bool hasLoad( ICswResources CswResources )
        {
            //******************* DUMMY IMPLMENETATION FOR NOW **********************//
            return ( true );
            //******************* DUMMY IMPLMENETATION FOR NOW **********************//
        }



        private LogicRunStatus _LogicRunStatus = LogicRunStatus.Idle;
        public LogicRunStatus LogicRunStatus
        {
            get { return ( _LogicRunStatus ); }
        }

        private CswSchedItemTimingFactory _CswSchedItemTimingFactory = new CswSchedItemTimingFactory();
        private CswScheduleLogicDetail _CswScheduleLogicDetail = null;
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }


        public void initScheduleLogicDetail( CswScheduleLogicDetail CswScheduleLogicDetail )
        {
            _CswScheduleLogicDetail = CswScheduleLogicDetail;

        }

        public void threadCallBack( ICswResources CswResources )
        {
            _LogicRunStatus = LogicRunStatus.Running;

            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;

            if( LogicRunStatus.Stopping != _LogicRunStatus )
            {

                try
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

                    CswNbtBatchOpMolFingerprints batchOp = new CswNbtBatchOpMolFingerprints( CswNbtResources );
                    int nodesPerIteration = CswConvert.ToInt32( CswNbtResources.ConfigVbls.getConfigVariableValue( CswConfigurationVariables.ConfigurationVariableNames.NodesProcessedPerCycle ) );
                    batchOp.makeBatchOp( nonFingerprintedMols, nodesPerIteration );

                    _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                    _LogicRunStatus = MtSched.Core.LogicRunStatus.Succeeded; //last line

                }//try

                catch( Exception Exception )
                {

                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtMolFingerprints::GetUpdatedItems() exception: " + Exception.Message;
                    CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = MtSched.Core.LogicRunStatus.Failed;

                }//catch



            }//if we're not shutting down

        }//threadCallBack()


        public void stop()
        {
            _LogicRunStatus = LogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = MtSched.Core.LogicRunStatus.Idle;
        }
    }//CswScheduleLogicNbtMolFingerpritns


}//namespace ChemSW.Nbt.Sched
