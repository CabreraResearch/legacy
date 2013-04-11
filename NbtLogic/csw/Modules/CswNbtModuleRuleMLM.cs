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
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.Containers ) )
            {
                _CswNbtResources.Modules.EnableModule( CswNbtModuleName.Containers );
            }

            //Turn on all views in the MLM (demo) category
            _CswNbtResources.Modules.ToggleViewsInCategory( false, "MLM (demo)", NbtViewVisibility.Global );

            //Case 27866/28156 on enable show Container props...
            //   Lot Controlled
            //   Requisitionable
            //   Reserved For
            //   Receipt Lot
            CswNbtMetaDataObjectClass containerOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            foreach( CswNbtMetaDataNodeType containerNT in containerOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeTab cmgTab = containerNT.getNodeTypeTab( "Central Material Group" );
                if( null == cmgTab )
                {
                    cmgTab = _CswNbtResources.MetaData.makeNewTab( containerNT, "Central Material Group", containerNT.getMaximumTabOrder() + 1 );
                }
                CswNbtMetaDataNodeTypeProp lotControlledNTP = containerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.LotControlled );
                lotControlledNTP.updateLayout( CswEnumNbtLayoutType.Edit, false, TabId: cmgTab.TabId );

                CswNbtMetaDataNodeTypeProp requisitionableNTP = containerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.Requisitionable );
                requisitionableNTP.updateLayout( CswEnumNbtLayoutType.Edit, false, TabId: cmgTab.TabId );

                CswNbtMetaDataNodeTypeProp reservedForNTP = containerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.ReservedFor );
                reservedForNTP.updateLayout( CswEnumNbtLayoutType.Edit, false, TabId: cmgTab.TabId );

                CswNbtMetaDataNodeTypeProp receiptLotNTP = containerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.ReceiptLot );
                CswNbtMetaDataNodeTypeTab firstTab = containerNT.getFirstNodeTypeTab();
                receiptLotNTP.updateLayout( CswEnumNbtLayoutType.Edit, false, TabId: firstTab.TabId );
            }

            //Case 27864 on enable show Material props...
            //   Manufacturing Sites
            //   UN Code
            //CswNbtMetaDataObjectClass materialOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
            //foreach( CswNbtMetaDataNodeType materialNT in materialOC.getNodeTypes() )
            //{
            //    CswNbtMetaDataNodeTypeProp uNCodeNTP = materialNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.UNCode );
            //    CswNbtMetaDataNodeTypeTab HazardsTab = materialNT.getNodeTypeTab( "Hazards" );
            //    int TabId = HazardsTab != null ? HazardsTab.TabId : materialNT.getFirstNodeTypeTab().TabId;
            //    uNCodeNTP.updateLayout( CswEnumNbtLayoutType.Edit, false, TabId );
            //}

            CswNbtMetaDataObjectClass RequestMatDispOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RequestMaterialDispenseClass );
            foreach( CswNbtMetaDataNodeType NodeType in RequestMatDispOc.getLatestVersionNodeTypes() )
            {
                CswNbtMetaDataNodeTypeTab CmgTab = NodeType.getNodeTypeTab( "Central Material Group" ) ?? _CswNbtResources.MetaData.makeNewTab( NodeType, "Central Material Group", NodeType.getNextTabOrder() );

                CswNbtMetaDataNodeTypeProp RofNtp = NodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.RecurringFrequency );
                RofNtp.updateLayout( CswEnumNbtLayoutType.Edit, true, CmgTab.TabId );
                CswNbtMetaDataNodeTypeProp NrdNtp = NodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.NextReorderDate );
                NrdNtp.updateLayout( CswEnumNbtLayoutType.Edit, RofNtp, true );

                CswNbtMetaDataNodeTypeProp LastNtp = NrdNtp;
                foreach( string CmgTabProp in CswNbtObjClassRequestMaterialDispense.PropertyName.MLMCmgTabProps )
                {
                    CswNbtMetaDataNodeTypeProp CmgNtp = NodeType.getNodeTypePropByObjectClassProp( CmgTabProp );
                    CmgNtp.updateLayout( CswEnumNbtLayoutType.Edit, LastNtp, true );
                    LastNtp = CmgNtp;
                }

                CswNbtMetaDataNodeTypeTab ReceiveTab = NodeType.getNodeTypeTab( "Receive" ) ?? _CswNbtResources.MetaData.makeNewTab( NodeType, "Receive", NodeType.getNextTabOrder() );
                foreach( string ReceiveTabProp in CswNbtObjClassRequestMaterialDispense.PropertyName.MLMReceiveTabProps )
                {
                    CswNbtMetaDataNodeTypeProp ReceiveNtp = NodeType.getNodeTypePropByObjectClassProp( ReceiveTabProp );
                    ReceiveNtp.updateLayout( CswEnumNbtLayoutType.Edit, true, ReceiveTab.TabId );
                }
            }

            //Case 28339
            //   Show Vendor.VendorType and CorporateEntity
            //   Add a filter to Material.Supplier where VendorType = 'Coporate'
            CswNbtMetaDataObjectClass vendorOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.VendorClass );
            foreach( CswNbtMetaDataNodeType vendorNT in vendorOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeTab firstTab = vendorNT.getFirstNodeTypeTab();
                CswNbtMetaDataNodeTypeProp vendorTypeNTP = vendorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassVendor.PropertyName.VendorTypeName );
                vendorTypeNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId: firstTab.TabId );
                vendorTypeNTP.updateLayout( CswEnumNbtLayoutType.Add, true, TabId: firstTab.TabId );

                CswNbtMetaDataNodeTypeProp corporateEntityNTP = vendorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassVendor.PropertyName.CorporateEntityName );
                corporateEntityNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId: firstTab.TabId );
            }

            _toggleMaterialSupplierView( false );

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

                CswNbtMetaDataNodeTypeProp receiptLotNTP = containerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.ReceiptLot );
                receiptLotNTP.removeFromAllLayouts();
            }

            //Case 27864 on enable hide Material props...
            //   Manufacturing Sites
            //   UN Code
            //CswNbtMetaDataObjectClass materialOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
            //foreach( CswNbtMetaDataNodeType materialNT in materialOC.getNodeTypes() )
            //{
            //    CswNbtMetaDataNodeTypeProp uNCodeNTP = materialNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.UNCode );
            //    uNCodeNTP.removeFromAllLayouts();
            //}

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

                CswNbtMetaDataNodeTypeProp RofNtp = NodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.RecurringFrequency );
                RofNtp.updateLayout( CswEnumNbtLayoutType.Edit, true, NodeType.getFirstNodeTypeTab().TabId );
                CswNbtMetaDataNodeTypeProp NrdNtp = NodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.NextReorderDate );
                NrdNtp.updateLayout( CswEnumNbtLayoutType.Edit, RofNtp, true );

            }

            //Case 28339
            //   Hide Vendor.VendorType and CorporateEntity
            //   Remove filter on Material.Supplier
            CswNbtMetaDataObjectClass vendorOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.VendorClass );
            foreach( CswNbtMetaDataNodeType vendorNT in vendorOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp vendorTypeNTP = vendorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassVendor.PropertyName.VendorTypeName );
                vendorTypeNTP.removeFromAllLayouts();

                CswNbtMetaDataNodeTypeProp corporateEntityNTP = vendorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassVendor.PropertyName.CorporateEntityName );
                corporateEntityNTP.removeFromAllLayouts();
            }

            CswNbtView newSupplierPropView = new CswNbtView( _CswNbtResources );
            newSupplierPropView.saveNew( "Supplier", NbtViewVisibility.Property );
            newSupplierPropView.AddViewRelationship( vendorOC, true );
            newSupplierPropView.save();

            _toggleMaterialSupplierView( true );

        } // OnDisable()

        private void _toggleMaterialSupplierView( bool MLMDisabled )
        {
            CswNbtMetaDataObjectClass vendorOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.VendorClass );
            CswNbtMetaDataObjectClass materialOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.MaterialClass );

            foreach( CswNbtMetaDataNodeType materialNT in materialOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp supplierNTP = materialNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.Supplier );
                CswNbtView supplierView = _CswNbtResources.ViewSelect.restoreView( supplierNTP.ViewId );
                supplierView.Clear();
                CswNbtViewRelationship parent = supplierView.AddViewRelationship( vendorOC, true );
                if( false == MLMDisabled )
                {
                    CswNbtMetaDataObjectClassProp vendorTypeOCP = vendorOC.getObjectClassProp( CswNbtObjClassVendor.PropertyName.VendorTypeName );
                    supplierView.AddViewPropertyAndFilter( parent,
                        MetaDataProp: vendorTypeOCP,
                        Value: CswNbtObjClassVendor.VendorTypes.Corporate,
                        FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );
                }
                supplierView.Visibility = NbtViewVisibility.Property;
                supplierView.ViewName = "Supplier";
                supplierView.save();
            }
        }

    } // class CswNbtModuleCISPro
}// namespace ChemSW.Nbt
