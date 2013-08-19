using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the CISPro Module
    /// </summary>
    public class CswNbtModuleRuleMLM: CswNbtModuleRule
    {
        public CswNbtModuleRuleMLM( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.MLM; } }
        protected override void OnEnable()
        {
            //Turn on all views in the MLM (demo) category
            _CswNbtResources.Modules.ToggleViewsInCategory( false, "MLM (demo)", CswEnumNbtViewVisibility.Global );

            //Case 27866/28156 on enable show Container props...
            //   Lot Controlled
            //   Requisitionable
            //   Reserved For
            //   Receipt Lot
            CswNbtMetaDataObjectClass containerOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            foreach( CswNbtMetaDataNodeType containerNT in containerOC.getNodeTypes() )
            {
                //CswNbtMetaDataNodeTypeTab cmgTab = containerNT.getNodeTypeTab( "Central Material Group" );
                //if( null == cmgTab )
                //{
                //    cmgTab = _CswNbtResources.MetaData.makeNewTab( containerNT, "Central Material Group", containerNT.getMaximumTabOrder() + 1 );
                //}

                _CswNbtResources.Modules.ShowProp( containerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.LotControlled );
                _CswNbtResources.Modules.ShowProp( containerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.Requisitionable );
                _CswNbtResources.Modules.ShowProp( containerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.ReservedFor );
                _CswNbtResources.Modules.ShowProp( containerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.ReceiptLot );
            }

            CswNbtMetaDataObjectClass RequestMatDispOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestMaterialDispenseClass );
            foreach( CswNbtMetaDataNodeType NodeType in RequestMatDispOc.getLatestVersionNodeTypes() )
            {
                CswNbtMetaDataNodeTypeTab ReceiveTab = NodeType.getNodeTypeTab( "Receive" ) ?? _CswNbtResources.MetaData.makeNewTab( NodeType, "Receive", NodeType.getNextTabOrder() );
                foreach( string ReceiveTabProp in CswNbtObjClassRequestMaterialDispense.PropertyName.MLMReceiveTabProps )
                {
                    CswNbtMetaDataNodeTypeProp ReceiveNtp = NodeType.getNodeTypePropByObjectClassProp( ReceiveTabProp );
                    ReceiveNtp.Hidden = false;
                }

                foreach( string CmgTabProp in CswNbtObjClassRequestMaterialDispense.PropertyName.MLMCmgTabProps )
                {
                    CswNbtMetaDataNodeTypeProp CmgNtp = NodeType.getNodeTypePropByObjectClassProp( CmgTabProp );
                    CmgNtp.Hidden = false;
                }

                //These are intentional layout moves
                CswNbtMetaDataNodeTypeTab CmgTab = NodeType.getNodeTypeTab( "Central Material Group" ) ?? _CswNbtResources.MetaData.makeNewTab( NodeType, "Central Material Group", NodeType.getNextTabOrder() );

                CswNbtMetaDataNodeTypeProp RofNtp = NodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.RecurringFrequency );
                RofNtp.updateLayout( CswEnumNbtLayoutType.Edit, true, CmgTab.TabId );
                CswNbtMetaDataNodeTypeProp NrdNtp = NodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.NextReorderDate );
                NrdNtp.updateLayout( CswEnumNbtLayoutType.Edit, RofNtp, true );
                //End intentional layout moves

            }

            //Case 28339
            //   Show Vendor.VendorType and CorporateEntity
            //   Add a filter to Material.Supplier where VendorType = 'Coporate'
            CswNbtMetaDataObjectClass vendorOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.VendorClass );
            foreach( CswNbtMetaDataNodeType vendorNT in vendorOC.getNodeTypes() )
            {
                _CswNbtResources.Modules.ShowProp( vendorNT.NodeTypeId, CswNbtObjClassVendor.PropertyName.VendorTypeName );
                _CswNbtResources.Modules.ShowProp( vendorNT.NodeTypeId, CswNbtObjClassVendor.PropertyName.CorporateEntityName );
            }

            _toggleMaterialSupplierView( false );
            _toggleReceiptLotManufacturerView( false );

            _toggleMaterialRequestApprovalLevel( CswEnumNbtObjectClass.RequestMaterialCreateClass, false );
            _toggleMaterialRequestApprovalLevel( CswEnumNbtObjectClass.RequestMaterialDispenseClass, false );

        }

        protected override void OnDisable()
        {
            //Turn on off views in the MLM (demo) category
            _CswNbtResources.Modules.ToggleViewsInCategory( true, "MLM (demo)", CswEnumNbtViewVisibility.Global );

            //Case 27866 on disable hide Container props...
            //   Lot Controlled
            //   Requisitionable
            //   Reserved For
            CswNbtMetaDataObjectClass containerOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            foreach( CswNbtMetaDataNodeType containerNT in containerOC.getNodeTypes() )
            {
                _CswNbtResources.Modules.HideProp( containerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.LotControlled );
                _CswNbtResources.Modules.HideProp( containerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.Requisitionable );
                _CswNbtResources.Modules.HideProp( containerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.ReservedFor );
                _CswNbtResources.Modules.HideProp( containerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.ReceiptLot );

                //CswNbtMetaDataNodeTypeTab cmgTab = containerNT.getNodeTypeTab( "Central Material Group" );
                //_CswNbtResources.MetaData.DeleteNodeTypeTab( cmgTab );
            }

            CswNbtMetaDataObjectClass RequestMatDispOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestMaterialDispenseClass );
            foreach( CswNbtMetaDataNodeType NodeType in RequestMatDispOc.getLatestVersionNodeTypes() )
            {
                foreach( string CmgTabProp in CswNbtObjClassRequestMaterialDispense.PropertyName.MLMCmgTabProps )
                {
                    _CswNbtResources.Modules.HideProp( NodeType.NodeTypeId, CmgTabProp );
                }

                //CswNbtMetaDataNodeTypeTab CmgTab = NodeType.getNodeTypeTab( "Central Material Group" );
                //if( null != CmgTab )
                //{
                //    _CswNbtResources.MetaData.DeleteNodeTypeTab( CmgTab );
                //}

                foreach( string ReceiveTabProp in CswNbtObjClassRequestMaterialDispense.PropertyName.MLMReceiveTabProps )
                {
                    _CswNbtResources.Modules.HideProp( NodeType.NodeTypeId, ReceiveTabProp );
                }
                //CswNbtMetaDataNodeTypeTab ReceiveTab = NodeType.getNodeTypeTab( "Receive" );
                //if( null != ReceiveTab )
                //{
                //    _CswNbtResources.MetaData.DeleteNodeTypeTab( ReceiveTab );
                //}

                //These layout moves are intentional
                CswNbtMetaDataNodeTypeProp RofNtp = NodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.RecurringFrequency );
                RofNtp.updateLayout( CswEnumNbtLayoutType.Edit, true, NodeType.getFirstNodeTypeTab().TabId );

                CswNbtMetaDataNodeTypeProp NrdNtp = NodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.NextReorderDate );
                NrdNtp.updateLayout( CswEnumNbtLayoutType.Edit, RofNtp, true );

            }

            //Case 28339
            //   Hide Vendor.VendorType and CorporateEntity
            //   Remove filter on Material.Supplier
            CswNbtMetaDataObjectClass vendorOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.VendorClass );
            foreach( CswNbtMetaDataNodeType vendorNT in vendorOC.getNodeTypes() )
            {
                _CswNbtResources.Modules.HideProp( vendorNT.NodeTypeId, CswNbtObjClassVendor.PropertyName.VendorTypeName );
                _CswNbtResources.Modules.HideProp( vendorNT.NodeTypeId, CswNbtObjClassVendor.PropertyName.CorporateEntityName );
            }

            CswNbtView newSupplierPropView = new CswNbtView( _CswNbtResources );
            newSupplierPropView.saveNew( "Supplier", CswEnumNbtViewVisibility.Property );
            newSupplierPropView.AddViewRelationship( vendorOC, true );
            newSupplierPropView.save();

            _toggleMaterialSupplierView( true );
            _toggleReceiptLotManufacturerView( true );
            _toggleMaterialRequestApprovalLevel( CswEnumNbtObjectClass.RequestMaterialCreateClass, true );
            _toggleMaterialRequestApprovalLevel( CswEnumNbtObjectClass.RequestMaterialDispenseClass, true );

        } // OnDisable()

        private void _toggleMaterialSupplierView( bool MLMDisabled )
        {
            CswNbtMetaDataObjectClass vendorOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.VendorClass );
            CswNbtMetaDataPropertySet MaterialSet = _CswNbtResources.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
            foreach( CswNbtMetaDataObjectClass materialOC in MaterialSet.getObjectClasses() )
            {
                foreach( CswNbtMetaDataNodeType materialNT in _CswNbtResources.MetaData.getNodeTypes( materialOC.ObjectClassId ) )
                {
                    CswNbtMetaDataNodeTypeProp supplierNTP = materialNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.Supplier );
                    CswNbtView supplierView = _CswNbtResources.ViewSelect.restoreView( supplierNTP.ViewId );
                    supplierView.Clear();
                    CswNbtViewRelationship parent = supplierView.AddViewRelationship( vendorOC, true );
                    if( false == MLMDisabled )
                    {
                        CswNbtMetaDataObjectClassProp vendorTypeOCP = vendorOC.getObjectClassProp( CswNbtObjClassVendor.PropertyName.VendorTypeName );
                        supplierView.AddViewPropertyAndFilter( parent,
                                                               MetaDataProp : vendorTypeOCP,
                                                               Value : CswNbtObjClassVendor.VendorTypes.Corporate,
                                                               FilterMode : CswEnumNbtFilterMode.Equals );
                    }
                    supplierView.Visibility = CswEnumNbtViewVisibility.Property;
                    supplierView.ViewName = "Supplier";
                    supplierView.save();
                }
            }
        }

        private void _toggleReceiptLotManufacturerView( bool MLMDisabled )
        {
            CswNbtMetaDataObjectClass VendorOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.VendorClass );
            CswNbtMetaDataObjectClass ReceiptLotOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ReceiptLotClass );
            foreach( CswNbtMetaDataNodeType ReceiptLotNT in ReceiptLotOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp ManufacturerNTP = ReceiptLotNT.getNodeTypePropByObjectClassProp( CswNbtObjClassReceiptLot.PropertyName.Manufacturer );
                CswNbtView ManufacturerView = _CswNbtResources.ViewSelect.restoreView( ManufacturerNTP.ViewId );
                ManufacturerView.Clear();
                CswNbtViewRelationship Parent = ManufacturerView.AddViewRelationship( VendorOC, true );
                if( false == MLMDisabled )
                {
                    CswNbtMetaDataObjectClassProp VendorTypeOCP = VendorOC.getObjectClassProp( CswNbtObjClassVendor.PropertyName.VendorTypeName );
                    ManufacturerView.AddViewPropertyAndFilter( Parent,
                        MetaDataProp : VendorTypeOCP,
                        Value : CswNbtObjClassVendor.VendorTypes.Manufacturing,
                        FilterMode : CswEnumNbtFilterMode.Equals );
                }
                ManufacturerView.Visibility = CswEnumNbtViewVisibility.Property;
                ManufacturerView.ViewName = "Manufacturer";
                ManufacturerView.save();
            }
        }

        private void _toggleMaterialRequestApprovalLevel( CswEnumNbtObjectClass ObjClass, bool MLMDisabled )
        {
            CswNbtMetaDataObjectClass createMaterialRequestOC = _CswNbtResources.MetaData.getObjectClass( ObjClass );
            foreach( CswNbtMetaDataNodeType createMaterialRequestNT in createMaterialRequestOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp approvalLevelNTP = createMaterialRequestNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.ApprovalLevel );
                if( MLMDisabled )
                {
                    approvalLevelNTP.Hidden = true;
                }
                else
                {
                    approvalLevelNTP.Hidden = false;
                }
            }
        }

    } // class CswNbtModuleCISPro
}// namespace ChemSW.Nbt
