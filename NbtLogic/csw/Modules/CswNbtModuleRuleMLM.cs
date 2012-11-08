using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

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

            CswNbtMetaDataObjectClass RequestMatDispOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RequestMaterialDispenseClass );
            foreach( CswNbtMetaDataNodeType NodeType in RequestMatDispOc.getLatestVersionNodeTypes() )
            {
                CswNbtMetaDataNodeTypeTab CmgTab = NodeType.getNodeTypeTab( "Central Material Group" ) ?? _CswNbtResources.MetaData.makeNewTab( NodeType, "Central Material Group", NodeType.GetMaximumTabOrder() + 1 );
                Int32 C = 0;
                foreach( string CmgTabProp in CswNbtObjClassRequestMaterialDispense.PropertyName.MLMCmgTabProps )
                {
                    C += 1;
                    CswNbtMetaDataNodeTypeProp CmgNtp = NodeType.getNodeTypePropByObjectClassProp( CmgTabProp );
                    CmgNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, CmgTab.TabId, C, 1 );
                }

                CswNbtMetaDataNodeTypeTab ReceiveTab = NodeType.getNodeTypeTab( "Receive" ) ?? _CswNbtResources.MetaData.makeNewTab( NodeType, "Receive", NodeType.GetMaximumTabOrder() + 1 );
                Int32 R = 0;
                foreach( string ReceiveTabProp in CswNbtObjClassRequestMaterialDispense.PropertyName.MLMReceiveTabProps )
                {
                    R += 1;
                    CswNbtMetaDataNodeTypeProp ReceiveNtp = NodeType.getNodeTypePropByObjectClassProp( ReceiveTabProp );
                    ReceiveNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, ReceiveTab.TabId, R, 1 );
                }

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

            CswNbtMetaDataObjectClass RequestMatDispOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RequestMaterialDispenseClass );
            foreach( CswNbtMetaDataNodeType NodeType in RequestMatDispOc.getLatestVersionNodeTypes() )
            {
                foreach( string CmgTabProp in CswNbtObjClassRequestMaterialDispense.PropertyName.MLMCmgTabProps )
                {
                    CswNbtMetaDataNodeTypeProp CmgNtp = NodeType.getNodeTypePropByObjectClassProp( CmgTabProp );
                    CmgNtp.removeFromAllLayouts();
                }
                CswNbtMetaDataNodeTypeTab CmgTab = NodeType.getNodeTypeTab( "Central Material Group" );
                if( null != CmgTab )
                {
                    _CswNbtResources.MetaData.DeleteNodeTypeTab( CmgTab );
                }

                foreach( string ReceiveTabProp in CswNbtObjClassRequestMaterialDispense.PropertyName.MLMReceiveTabProps )
                {
                    CswNbtMetaDataNodeTypeProp ReceiveNtp = NodeType.getNodeTypePropByObjectClassProp( ReceiveTabProp );
                    ReceiveNtp.removeFromAllLayouts();
                }
                CswNbtMetaDataNodeTypeTab ReceiveTab = NodeType.getNodeTypeTab( "Receive" );
                if( null != ReceiveTab )
                {
                    _CswNbtResources.MetaData.DeleteNodeTypeTab( ReceiveTab );
                }
            }

        } // OnDisable()

    } // class CswNbtModuleCISPro
}// namespace ChemSW.Nbt
