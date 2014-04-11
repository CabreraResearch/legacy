using System;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
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

            // CIS-51775
            CswNbtMetaDataObjectClass LocationOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            foreach( Int32 LocationNodeTypeId in LocationOC.getNodeTypeIds().Keys )
            {
                _CswNbtResources.Modules.ShowProp( LocationNodeTypeId, CswNbtObjClassLocation.PropertyName.InventoryGroup );
            }
            CswNbtMetaDataObjectClass RequestItemOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestItemClass );
            foreach( Int32 RequestItemNodeTypeId in RequestItemOC.getNodeTypeIds().Keys )
            {
                _CswNbtResources.Modules.ShowProp( RequestItemNodeTypeId, CswNbtObjClassRequestItem.PropertyName.InventoryGroup );
            }
            _CswNbtResources.Modules.ToggleView( false, "Inventory Groups", CswEnumNbtViewVisibility.Global );
        } // OnEnable()

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

            // CIS-51775
            CswNbtMetaDataObjectClass LocationOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            foreach( Int32 LocationNodeTypeId in LocationOC.getNodeTypeIds().Keys )
            {
                _CswNbtResources.Modules.HideProp( LocationNodeTypeId, CswNbtObjClassLocation.PropertyName.InventoryGroup );
            }
            CswNbtMetaDataObjectClass RequestItemOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestItemClass );
            foreach( Int32 RequestItemNodeTypeId in RequestItemOC.getNodeTypeIds().Keys )
            {
                _CswNbtResources.Modules.HideProp( RequestItemNodeTypeId, CswNbtObjClassRequestItem.PropertyName.InventoryGroup );
            }
            _CswNbtResources.Modules.ToggleView( true, "Inventory Groups", CswEnumNbtViewVisibility.Global );
        } // OnDisable()

    } // class CswNbtModuleRuleMultiInventoryGroup
}// namespace ChemSW.Nbt
