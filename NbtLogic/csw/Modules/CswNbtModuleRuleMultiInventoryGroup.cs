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
            int invGrpOC_Id = _CswNbtResources.MetaData.getObjectClassId( CswEnumNbtObjectClass.InventoryGroupClass );
            CswNbtActQuotas QuotasAct = new CswNbtActQuotas( _CswNbtResources );
            int InvGroupsCount = QuotasAct.GetNodeCountForObjectClass( invGrpOC_Id );
            if( InvGroupsCount > 1 && false == _CswNbtResources.CurrentNbtUser is CswNbtSystemUser )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "Cannot disable the MultiInventoryGroup Module when multiple Inventory Groups exist", InvGroupsCount + " Inventory Group nodes exist, cannot disable the MultiInventoryGroup module" );
            }
            else
            {
                QuotasAct.SetQuotaForObjectClass( invGrpOC_Id, 1, true );
            }
        } // OnDisable()

    } // class CswNbtModuleRuleMultiSite
}// namespace ChemSW.Nbt
