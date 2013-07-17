using System;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the Multi Inventory Group Module
    /// </summary>
    public class CswNbtModuleRuleMultiInventoryGroup : CswNbtModuleRule
    {
        public CswNbtModuleRuleMultiInventoryGroup( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.MultiInventoryGroup; } }
        protected override void OnEnable()
        {
            int invGrpOC_Id = _CswNbtResources.MetaData.getObjectClassId( CswEnumNbtObjectClass.InventoryGroupClass );
            CswNbtActQuotas QuotasAct = new CswNbtActQuotas( _CswNbtResources );
            QuotasAct.SetQuotaForObjectClass( invGrpOC_Id, Int32.MinValue, false );
        }

        protected override void OnDisable()
        {
            CswNbtMetaDataObjectClass invGrpOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryGroupClass );
            CswNbtActQuotas QuotasAct = new CswNbtActQuotas( _CswNbtResources );
            CswNbtView invGrpView = new CswNbtView( _CswNbtResources );
            invGrpView.AddViewRelationship( invGrpOC, false );
            ICswNbtTree invGroupsTree = _CswNbtResources.Trees.getTreeFromView( invGrpView, false, true, true );
            int InvGroupsCount = invGroupsTree.getChildNodeCount();
            if( InvGroupsCount > 1 && false == _CswNbtResources.CurrentNbtUser is CswNbtSystemUser )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "Cannot disable the MultiInventoryGroup Module when multiple Inventory Groups exist", InvGroupsCount + " Inventory Group nodes exist, cannot disable the MultiInventoryGroup module" );
            }
            else
            {
                QuotasAct.SetQuotaForObjectClass( invGrpOC.ObjectClassId, 1, true );
            }
        } // OnDisable()

    } // class CswNbtModuleRuleMultiSite
}// namespace ChemSW.Nbt
