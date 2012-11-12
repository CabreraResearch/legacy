using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using System.Data;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the CISPro Module
    /// </summary>
    public class CswNbtModuleRuleMLM : CswNbtModuleRule
    {
        public CswNbtModuleRuleMLM( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswNbtModuleName ModuleName { get { return CswNbtModuleName.CISPro; } }
        public override void OnEnable()
        {
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.CISPro ) )
            {
                _CswNbtResources.Modules.EnableModule( CswNbtModuleName.CISPro );
            }

            //Turn on all views in the MLM (demo) category
            _CswNbtResources.Modules.ToggleViewsInCategory( false, "MLM (demo)", NbtViewVisibility.Global);

            //Case 27866 on enable show Container props...
            //   Lot Controlled
            //   Requisitionable
            //   Reserved For
            CswNbtMetaDataObjectClass containerOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            foreach( CswNbtMetaDataNodeType containerNT in containerOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeTab cmgTab = containerNT.getNodeTypeTab( "Central Material Group" );
                if( null == cmgTab )
                {
                    cmgTab = _CswNbtResources.MetaData.makeNewTab( containerNT, "Central Material Group", containerNT.GetMaximumTabOrder() + 1 );
                }
                CswNbtMetaDataNodeTypeProp lotControlledNTP = containerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.LotControlled );
                lotControlledNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, false, TabId: cmgTab.TabId );

                CswNbtMetaDataNodeTypeProp requisitionableNTP = containerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.Requisitionable );
                requisitionableNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, false, TabId: cmgTab.TabId );

                CswNbtMetaDataNodeTypeProp reservedForNTP = containerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.ReservedFor );
                reservedForNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, false, TabId: cmgTab.TabId );
            }
        }

        public override void OnDisable()
        {
            //Turn on off views in the MLM (demo) category
            _CswNbtResources.Modules.ToggleViewsInCategory( true, "MLM (demo)", NbtViewVisibility.Global );

            //Case 27866 on disable hide Container props...
            //   Lot Controlled
            //   Requisitionable
            //   Reserved For
            CswNbtMetaDataObjectClass containerOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            foreach( CswNbtMetaDataNodeType containerNT in containerOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp lotControlledNTP = containerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.LotControlled );
                lotControlledNTP.removeFromAllLayouts();

                CswNbtMetaDataNodeTypeProp requisitionableNTP = containerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.Requisitionable );
                requisitionableNTP.removeFromAllLayouts();

                CswNbtMetaDataNodeTypeProp reservedForNTP = containerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.ReservedFor );
                reservedForNTP.removeFromAllLayouts();

                CswNbtMetaDataNodeTypeTab cmgTab = containerNT.getNodeTypeTab( "Central Material Group" );
                _CswNbtResources.MetaData.DeleteNodeTypeTab( cmgTab );
            }
        } // OnDisable()

    } // class CswNbtModuleCISPro
}// namespace ChemSW.Nbt
