using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the CISPro Module
    /// </summary>
    public class CswNbtModuleRuleCISPro : CswNbtModuleRule
    {
        public CswNbtModuleRuleCISPro( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswNbtModuleName ModuleName { get { return CswNbtModuleName.CISPro; } }
        public override void OnEnable()
        {
            // When CISPro is enabled, display the following properties:
            //   Location.Inventory Group
            //   Location.Storage Compatibility
            //   User.WorkUnit
            CswNbtMetaDataObjectClass LocationOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
            foreach( CswNbtMetaDataNodeType LocationNT in LocationOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp LocationInvGrpNTP = LocationNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.InventoryGroupPropertyName );
                LocationInvGrpNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, false );
                LocationInvGrpNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, LocationNT.getFirstNodeTypeTab().TabId );
                CswNbtMetaDataNodeTypeProp LocationStorCompatNTP = LocationNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.StorageCompatabilityPropertyName );
                //LocationStorCompatNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, false );
                LocationStorCompatNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, LocationNT.getFirstNodeTypeTab().TabId );
            }
            CswNbtMetaDataObjectClass UserOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            foreach( CswNbtMetaDataNodeType UserNT in UserOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp UserWorkUnitNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.WorkUnitPropertyName );
                UserWorkUnitNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, false );
                UserWorkUnitNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, UserNT.getFirstNodeTypeTab().TabId );
            }
        }

        public override void OnDisable()
        {
            // case 26717 - When CISPro is disabled, hide the following properties:
            //   Location.Inventory Group
            //   Location.Storage Compatibility
            //   User.WorkUnit
            CswNbtMetaDataObjectClass LocationOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
            foreach( CswNbtMetaDataNodeType LocationNT in LocationOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp LocationInvGrpNTP = LocationNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.InventoryGroupPropertyName );
                LocationInvGrpNTP.removeFromAllLayouts();
                CswNbtMetaDataNodeTypeProp LocationStorCompatNTP = LocationNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.StorageCompatabilityPropertyName );
                LocationStorCompatNTP.removeFromAllLayouts();
            }
            CswNbtMetaDataObjectClass UserOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            foreach( CswNbtMetaDataNodeType UserNT in UserOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp UserWorkUnitNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.WorkUnitPropertyName );
                UserWorkUnitNTP.removeFromAllLayouts();
            }
        } // OnDisable()

    } // class CswNbtModuleCISPro
}// namespace ChemSW.Nbt
