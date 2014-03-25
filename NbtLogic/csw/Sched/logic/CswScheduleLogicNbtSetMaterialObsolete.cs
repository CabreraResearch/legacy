using System;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Sched
{
    /// <summary>
    /// Scheduled Rule that checks to see if any materials have a null Obsolete property and sets it to false.
    /// </summary>
    public class CswScheduleLogicNbtSetMaterialObsolete : ICswScheduleLogic
    {
        #region Properties

        public string RuleName
        {
            get { return ( CswEnumNbtScheduleRuleNames.SetMaterialObsolete ); }
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

        #endregion Properties

        #region State

        private Collection<CswPrimaryKey> _MaterialIdsWithNullObsoleteProp = new Collection<CswPrimaryKey>();

        private void _setLoad( ICswResources CswResources )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            if( NbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.CISPro ) )
            {
                #region SQL

                String SQLSelect = @"WITH hasprop 
                                        AS (SELECT np.nodeid 
                                            FROM   jct_nodes_props np 
                                                join nodetype_props ntp 
                                                    ON ntp.nodetypepropid = np.nodetypepropid 
                                                join nodetypes nt 
                                                    ON ( nt.nodetypeid = ntp.nodetypeid ) 
                                                join object_class oc 
                                                    ON ( oc.objectclassid = nt.objectclassid ) 
                                                join jct_propertyset_objectclass psoc 
                                                    ON ( psoc.objectclassid = oc.objectclassid ) 
                                                join property_set ps 
                                                    ON ( ps.propertysetid = psoc.propertysetid ) 
                                            WHERE  ps.name = 'MaterialSet' 
                                                AND ntp.propname = 'Obsolete'), 
                                        materials 
                                        AS (SELECT n.nodeid 
                                            FROM   nodes n 
                                                inner join nodetypes nt 
                                                        ON n.nodetypeid = nt.nodetypeid 
                                                inner join object_class oc 
                                                        ON nt.objectclassid = oc.objectclassid 
                                                inner join jct_propertyset_objectclass psoc 
                                                        ON ( psoc.objectclassid = oc.objectclassid ) 
                                                inner join property_set ps 
                                                        ON ( ps.propertysetid = psoc.propertysetid ) 
                                            WHERE  ps.name = 'MaterialSet') 
                                SELECT nodeid 
                                FROM   materials 
                                WHERE  nodeid NOT IN (SELECT * 
                                                        FROM   hasprop)";

                #endregion SQL
                CswArbitrarySelect ObsoletelessMaterials = new CswArbitrarySelect( NbtResources.CswResources, "ObsoletelessMaterials", SQLSelect );
                DataTable MaterialsTbl = ObsoletelessMaterials.getTable();
                foreach( DataRow MaterialRow in MaterialsTbl.Rows )
                {
                    _MaterialIdsWithNullObsoleteProp.Add( new CswPrimaryKey( "nodes", CswConvert.ToInt32( MaterialRow["nodeid"].ToString() ) ) );
                }
            }
        }

        #endregion State

        #region Scheduler Methods

        public void initScheduleLogicDetail( CswScheduleLogicDetail LogicDetail )
        {
            _CswScheduleLogicDetail = LogicDetail;
        }

        //Determine the number of containers without Receive Dispense Transactions and return that value
        public Int32 getLoadCount( ICswResources CswResources )
        {
            if( _MaterialIdsWithNullObsoleteProp.Count == 0 )
            {
                _setLoad( CswResources );
            }
            return _MaterialIdsWithNullObsoleteProp.Count;
        }

        public void threadCallBack( ICswResources CswResources )
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Running;

            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;
            _CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;

            if( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus )
            {
                try
                {
                    if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.CISPro ) )
                    {
                        int ContainersProcessedPerIteration = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.NodesProcessedPerCycle ) );
                        int TotalProcessedThisIteration = 0;
                        while( TotalProcessedThisIteration < ContainersProcessedPerIteration && _MaterialIdsWithNullObsoleteProp.Count > 0 && ( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus ) )
                        {
                            _setObsoletePropToFalse( _CswNbtResources, _MaterialIdsWithNullObsoleteProp[0] );
                            _MaterialIdsWithNullObsoleteProp.RemoveAt( 0 );
                            TotalProcessedThisIteration++;
                        }
                    }
                    _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Succeeded;
                }
                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicSetMaterialObsolete::_setObsoletePropToFalse() exception: " + Exception.Message + "; " + Exception.StackTrace;
                    _CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Failed;
                }
            }
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

        #region Custom Logic

        private void _setObsoletePropToFalse( CswNbtResources _CswNbtResources, CswPrimaryKey MaterialId )
        {
            CswNbtNode CurrentNode = _CswNbtResources.Nodes.GetNode( MaterialId );
            CswNbtNodePropWrapper ObsoletePropWrapper = CurrentNode.Properties[CswNbtPropertySetMaterial.PropertyName.Obsolete];
            if( null != ObsoletePropWrapper )
            {
                ObsoletePropWrapper.AsLogical.Checked = CswEnumTristate.False;
                CurrentNode.postChanges( false );
            }
        }

        #endregion Custom Logic

    }//CswScheduleLogicNbtSetMaterialObsolete

}//namespace ChemSW.Nbt.Sched
