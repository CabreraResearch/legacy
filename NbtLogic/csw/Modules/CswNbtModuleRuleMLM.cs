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

            //Case 27864 on enable show Material props...
            //   Manufacturing Sites
            //   UN Code
            CswNbtMetaDataObjectClass materialOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
            foreach( CswNbtMetaDataNodeType materialNT in materialOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp manufacturingSitesNTP = materialNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.ManufacturingSites );
                manufacturingSitesNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, false, materialNT.getFirstNodeTypeTab().TabId );

                CswNbtMetaDataNodeTypeProp uNCodeNTP = materialNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.UNCode );
                uNCodeNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, false, materialNT.getFirstNodeTypeTab().TabId );
            }

        }

        public override void OnDisable()
        {
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

            //Case 27864 on enable hide Material props...
            //   Manufacturing Sites
            //   UN Code
            CswNbtMetaDataObjectClass materialOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
            foreach( CswNbtMetaDataNodeType materialNT in materialOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp manufacturingSitesNTP = materialNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.ManufacturingSites );
                manufacturingSitesNTP.removeFromAllLayouts();

                CswNbtMetaDataNodeTypeProp uNCodeNTP = materialNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.UNCode );
                uNCodeNTP.removeFromAllLayouts();
            }

        } // OnDisable()

    } // class CswNbtModuleCISPro
}// namespace ChemSW.Nbt
