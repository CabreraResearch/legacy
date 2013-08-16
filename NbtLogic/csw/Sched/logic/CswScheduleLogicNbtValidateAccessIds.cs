using System;
using System.Collections.ObjectModel;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicNbtValidateAccessIds: ICswScheduleLogic
    {
        #region Properties

        private CswEnumScheduleLogicRunStatus _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        public CswEnumScheduleLogicRunStatus LogicRunStatus
        {
            get { return ( _LogicRunStatus ); }
        }

        private CswScheduleLogicDetail _CswScheduleLogicDetail;
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }
        public string RuleName
        {
            get { return ( CswEnumNbtScheduleRuleNames.ValidateAccessIds ); }
        }

        #endregion Properties

        #region Scheduler Methods

        public void initScheduleLogicDetail( CswScheduleLogicDetail LogicDetail )
        {
            _CswScheduleLogicDetail = LogicDetail;
        }

        //In the case where the rule always has 'work' to do, the rule should only have load when the rule is scheduled to run.
        //This is necessary to stop the rule from running once it has completed its job.
        public Int32 getLoadCount( ICswResources CswResources )
        {
            _CswScheduleLogicDetail.LoadCount = _CswScheduleLogicDetail.doesItemRunNow() ? 1 : 0;
            return _CswScheduleLogicDetail.LoadCount;
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

            if( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus )
            {
                if( CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.NBTManager ) )
                {
                    Collection<CswNbtObjClassCustomer> CustomersCollection = _getCustomers( CswNbtResources );

                    foreach ( CswNbtObjClassCustomer Customer in CustomersCollection )
                    {   
                        //does a configuration for this customer's access id exist?
                        if( false == CswNbtResources.CswDbCfgInfo.AccessIds.Contains( Customer.CompanyID.Text ) )
                        {
                          //this access id doesn't exist, we must send an email
                            string Subject = "Invalid Customer AccessID '" + Customer.CompanyID.Text + "' for schema '" + Customer.SchemaName + "'";
                            string Message = "On " + DateTime.Now.ToString() + ", the ValidateAccessIds schedule rule detected that the Access ID '" + Customer.CompanyID.Text + "' on schema '" + Customer.SchemaName + "' does not have an associated value in CswConfigUX.";
                            CswNbtResources.sendSystemAlertEmail( Subject, Message );
                        }//if false == AccessIDs.Contains( Customer.CompanyID.Text )
                    }//foreach ( Customer in CustomersCollection )

                    _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Succeeded;

                }//if ( IsModuleEnabled ( NBTManager ) )
            }//if( Stopping != _LogicRunStatus )
        }//threadCallBack()

        #endregion Scheduler Methods

        #region Schedule-specific Methods

        private Collection<CswNbtObjClassCustomer> _getCustomers( CswNbtResources NbtResources )
        {
            //fetch a tree of all customer nodes
            CswNbtMetaDataObjectClass CustomerOC = NbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.CustomerClass );
            CswNbtView CustomersView = CustomerOC.CreateDefaultView( false );

            ICswNbtTree CustomersTree = NbtResources.Trees.getTreeFromView( CustomersView, false, false, false );
            
            Collection<CswNbtObjClassCustomer> CustomersCollection = new Collection<CswNbtObjClassCustomer>();

            Int32 CustomerCount = CustomersTree.getChildNodeCount();
            if( CustomerCount > 0 )
            {
                //traverse the tree, adding each node to the collection that will be returned
                for( int i = 0; i < CustomerCount; i++ )
                {
                    CustomersTree.goToNthChild( i );
                    CustomersCollection.Add( CustomersTree.getCurrentNode() );
                    CustomersTree.goToParentNode();
                }
            }

            return CustomersCollection;
        }

        #endregion
    }//CswScheduleLogicNbtTierII
}//namespace ChemSW.Nbt.Sched
