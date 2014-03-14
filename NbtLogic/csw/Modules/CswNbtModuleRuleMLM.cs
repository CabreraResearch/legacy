using System;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

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
                _CswNbtResources.Modules.ShowProp( containerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.LotControlled );
                _CswNbtResources.Modules.ShowProp( containerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.Requisitionable );
                _CswNbtResources.Modules.ShowProp( containerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.ReservedFor );
                //Case 30723 - Receipt Lot is only "semi-disabled" when MLM is off, so just add it back to the layout when it's enabled
                CswNbtMetaDataNodeTypeProp ReceiptLotNTP = _CswNbtResources.MetaData.getNodeTypeProp( containerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.ReceiptLot );
                if( null != ReceiptLotNTP )
                {
                    CswNbtMetaDataNodeTypeProp DateCreatedNTP = containerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.DateCreated );
                    ReceiptLotNTP.updateLayout( CswEnumNbtLayoutType.Edit, DateCreatedNTP, true );
                }
            }

            //Case 31546 - If requesting is disabled, we don't need to show anything on the Request Item OC because it is not available
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Requesting ) )
            {
                CswNbtObjClassRequestItem.ToggleMLMProps( _CswNbtResources, false );
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
            
            setReceiptLotPermissions( _CswNbtResources, true );

            //Case CIS-52280 on enable show Material props...
            //   Manufacturing Sites Grid
            //   Requires Cleaning Event
            CswNbtMetaDataPropertySet materialPS = _CswNbtResources.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
            foreach( CswNbtMetaDataObjectClass materialOC in materialPS.getObjectClasses() )
            {
                foreach( CswNbtMetaDataNodeType materialNT in materialOC.getNodeTypes() )
                {
                    _CswNbtResources.Modules.ShowProp( materialNT.NodeTypeId, CswNbtPropertySetMaterial.PropertyName.ManufacturingSites );
                    _CswNbtResources.Modules.ShowProp( materialNT.NodeTypeId, CswNbtPropertySetMaterial.PropertyName.RequiresCleaningEvent );
                }
            }
            
            } // OnEnable()



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
                //if( null != cmgTab )
                //{
                //    cmgTab.DesignNode.Node.delete( false, true );
                //}
            }

            //Case 31546 - If requesting is disabled, we don't need to hide anything on the Request Item OC because it is not available
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Requesting ) )
            {
                CswNbtObjClassRequestItem.ToggleMLMProps( _CswNbtResources, true );
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

            //Case CIS-52280 on disable hide Material props...
            //   Manufacturing Sites Grid
            //   Requires Cleaning Event
            CswNbtMetaDataPropertySet materialPS = _CswNbtResources.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
            foreach( CswNbtMetaDataObjectClass materialOC in materialPS.getObjectClasses() )
            {
                foreach( CswNbtMetaDataNodeType materialNT in materialOC.getNodeTypes() )
                {
                    _CswNbtResources.Modules.HideProp( materialNT.NodeTypeId, CswNbtPropertySetMaterial.PropertyName.ManufacturingSites );
                    _CswNbtResources.Modules.HideProp( materialNT.NodeTypeId, CswNbtPropertySetMaterial.PropertyName.RequiresCleaningEvent );
                }
            }

            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.ManufacturerLotInfo ) )
            {
                setReceiptLotPermissions( _CswNbtResources, false );
            }
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
                                                               MetaDataProp: vendorTypeOCP,
                                                               Value: CswNbtObjClassVendor.VendorTypes.Corporate,
                                                               FilterMode: CswEnumNbtFilterMode.Equals );
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
                        MetaDataProp: VendorTypeOCP,
                        Value: CswNbtObjClassVendor.VendorTypes.Manufacturing,
                        FilterMode: CswEnumNbtFilterMode.Equals );
                }
                ManufacturerView.Visibility = CswEnumNbtViewVisibility.Property;
                ManufacturerView.ViewName = "Manufacturer";
                ManufacturerView.save();
            }
        }

        public static void setReceiptLotPermissions( CswNbtResources CswNbtResources, bool PermValue )
        {
            // CIS-52258 - grant Receipt Lot permissions to cispro_ roles

            CswNbtMetaDataObjectClass ReceiptLotOC = CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ReceiptLotClass );

            CswNbtMetaDataObjectClass RoleOC = CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );
            CswNbtMetaDataObjectClassProp RoleNameOCP = RoleOC.getObjectClassProp( CswNbtObjClassRole.PropertyName.Name );

            CswNbtView View = new CswNbtView( CswNbtResources );
            View.ViewName = "MLM Enable - Find CISPro roles";
            CswNbtViewRelationship roleRelationship = View.AddViewRelationship( RoleOC, false );
            CswNbtViewProperty roleNameProp = View.AddViewProperty( roleRelationship, RoleNameOCP );
            View.AddViewPropertyFilter( roleNameProp,
                                        SubFieldName: CswNbtFieldTypeRuleText.SubFieldName.Text,
                                        FilterMode: CswEnumNbtFilterMode.Begins,
                                        Value: "cispro_",
                                        CaseSensitive: false );
            ICswNbtTree RoleTree = CswNbtResources.Trees.getTreeFromView( View, false, true, IncludeHiddenNodes: true );
            for( Int32 r = 0; r < RoleTree.getChildNodeCount(); r++ )
            {
                RoleTree.goToNthChild( r );

                CswNbtObjClassRole RoleNode = RoleTree.getCurrentNode();
                foreach( CswNbtMetaDataNodeType ReceiptLotNT in ReceiptLotOC.getNodeTypes() )
                {
                    if( false == PermValue )
                    {
                        CswNbtResources.Permit.set( new CswEnumNbtNodeTypePermission[]
                            {
                                CswEnumNbtNodeTypePermission.View,
                                CswEnumNbtNodeTypePermission.Create,
                                CswEnumNbtNodeTypePermission.Edit,
                                CswEnumNbtNodeTypePermission.Delete
                            },
                            ReceiptLotNT, RoleNode, PermValue );
                    }
                    else
                    {
                        CswNbtResources.Permit.set( CswEnumNbtNodeTypePermission.View, ReceiptLotNT, RoleNode, PermValue );
                        if( "cispro_receiver" == RoleNode.Name.Text.ToLower() ||
                            "cispro_admin" == RoleNode.Name.Text.ToLower() )
                        {
                            CswNbtResources.Permit.set( CswEnumNbtNodeTypePermission.Edit, ReceiptLotNT, RoleNode, PermValue );
                        }
                        if( "cispro_admin" == RoleNode.Name.Text.ToLower() )
                        {
                            CswNbtResources.Permit.set( CswEnumNbtNodeTypePermission.Delete, ReceiptLotNT, RoleNode, PermValue );
                        }
                    }
                }
                //RoleNode.postChanges( false );  CswNbtPermit does this.

                RoleTree.goToParentNode();
            }
        } // setReceiptLotPermissions()

    } // class CswNbtModuleCISPro
}// namespace ChemSW.Nbt
