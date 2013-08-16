using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30228: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30228; }
        }

        public override void update()
        {

            #region Update all NTPs "Hidden" column to "false"

            CswTableUpdate nodeTypePropsTU = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "ntp.setHidden", "nodetype_props" );
            DataTable nodetypePropsDT = nodeTypePropsTU.getTable();
            foreach( DataRow row in nodetypePropsDT.Rows )
            {
                row["hidden"] = CswConvert.ToDbVal( false );
            }
            nodeTypePropsTU.update( nodetypePropsDT );

            #endregion

            #region CISPro

            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            foreach( CswNbtMetaDataNodeType UserNT in UserOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp UserJurisdictionNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.PropertyName.Jurisdiction );
                if( null != UserJurisdictionNTP )
                {
                    UserJurisdictionNTP.updateLayout( CswEnumNbtLayoutType.Add, false );
                    UserJurisdictionNTP.updateLayout( CswEnumNbtLayoutType.Edit, DoMove : false, TabId : UserNT.getFirstNodeTypeTab().TabId );
                }
            }

            #endregion

            #region Containers

            CswNbtMetaDataObjectClass locationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            foreach( int NodeTypeId in locationOC.getNodeTypeIds() )
            {
                _CswNbtSchemaModTrnsctn.Modules.AddPropToTab( NodeTypeId, CswNbtObjClassLocation.PropertyName.Containers, "Containers" );
                _CswNbtSchemaModTrnsctn.Modules.AddPropToTab( NodeTypeId, "Inventory Levels", "Inventory Levels", 2 );
                _CswNbtSchemaModTrnsctn.Modules.AddPropToFirstTab( NodeTypeId, CswNbtObjClassLocation.PropertyName.AllowInventory );
                _CswNbtSchemaModTrnsctn.Modules.AddPropToFirstTab( NodeTypeId, CswNbtObjClassLocation.PropertyName.InventoryGroup );
                _CswNbtSchemaModTrnsctn.Modules.AddPropToFirstTab( NodeTypeId, CswNbtObjClassLocation.PropertyName.StorageCompatibility );

                CswNbtMetaDataNodeTypeProp AllowInvNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( NodeTypeId, CswNbtObjClassLocation.PropertyName.AllowInventory );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, NodeTypeId, AllowInvNTP, false );

                CswNbtMetaDataNodeTypeProp InvGrpNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( NodeTypeId, CswNbtObjClassLocation.PropertyName.InventoryGroup );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, NodeTypeId, InvGrpNTP, false );

                CswNbtMetaDataNodeTypeProp StorageCompatNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( NodeTypeId, CswNbtObjClassLocation.PropertyName.StorageCompatibility );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, NodeTypeId, StorageCompatNTP, false );
            }

            CswNbtMetaDataPropertySet MaterialSet = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
            foreach( CswNbtMetaDataObjectClass materialOC in MaterialSet.getObjectClasses() )
            {
                foreach( CswNbtMetaDataNodeType materialNT in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes( materialOC.ObjectClassId ) )
                {
                    string sizesNTPName = materialNT.NodeTypeName + " Sizes";
                    _CswNbtSchemaModTrnsctn.Modules.AddPropToTab( materialNT.NodeTypeId, sizesNTPName, "Containers", 99 );

                    _CswNbtSchemaModTrnsctn.Modules.AddPropToTab( materialNT.NodeTypeId, "Inventory Levels", "Containers", 99 );

                    string containersNTPName = materialNT.NodeTypeName + " Containers";
                    _CswNbtSchemaModTrnsctn.Modules.AddPropToTab( materialNT.NodeTypeId, containersNTPName, "Containers", 99 );

                    CswNbtMetaDataNodeTypeTab materialNTT = materialNT.getFirstNodeTypeTab();
                    _CswNbtSchemaModTrnsctn.Modules.AddPropToTab( materialNT.NodeTypeId, CswNbtObjClassChemical.PropertyName.ApprovedForReceiving, materialNTT );

                    CswNbtMetaDataNodeTypeTab materialIdentityNTT = materialNT.getIdentityTab();
                    _CswNbtSchemaModTrnsctn.Modules.AddPropToTab( materialNT.NodeTypeId, CswNbtObjClassChemical.PropertyName.Receive, materialIdentityNTT, 2, 2 );
                    _CswNbtSchemaModTrnsctn.Modules.AddPropToTab( materialNT.NodeTypeId, CswNbtObjClassChemical.PropertyName.Request, materialIdentityNTT, 1, 2 );

                    _CswNbtSchemaModTrnsctn.Modules.AddPropToTab( materialNT.NodeTypeId, CswNbtObjClassChemical.PropertyName.StorageCompatibility, "Hazards" );
                }
            }

            #endregion

            #region Fire Code

            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            foreach( int LocationNTId in LocationOC.getNodeTypeIds() )
            {
                _CswNbtSchemaModTrnsctn.Modules.AddPropToFirstTab( LocationNTId, CswNbtObjClassLocation.PropertyName.ControlZone );

                CswNbtMetaDataNodeTypeProp CtrlZoneNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( LocationNTId, CswNbtObjClassLocation.PropertyName.ControlZone );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, LocationNTId, CtrlZoneNTP, false );
            }

            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            foreach( CswNbtMetaDataNodeType ChemicalNT in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes( ChemicalOC.ObjectClassId ) )
            {
                foreach( CswNbtMetaDataNodeTypeTab Tab in ChemicalNT.getNodeTypeTabs() )
                {
                    if( Tab.TabOrder >= 4 )
                        Tab.TabOrder += 1;
                }
                CswNbtMetaDataNodeTypeTab HazardsTab = ChemicalNT.getNodeTypeTab( "Hazards" );
                if( null == HazardsTab )
                {
                    HazardsTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( ChemicalNT, "Hazards", 4 );
                }
                _CswNbtSchemaModTrnsctn.Modules.AddPropToTab( ChemicalNT.NodeTypeId, "Material Type", HazardsTab, 4, 2, "Fire Reporting" );
                _CswNbtSchemaModTrnsctn.Modules.AddPropToTab( ChemicalNT.NodeTypeId, "Special Flags", HazardsTab, 5, 2, "Fire Reporting" );
                _CswNbtSchemaModTrnsctn.Modules.AddPropToTab( ChemicalNT.NodeTypeId, "Hazard Categories", HazardsTab, 6, 2, "Fire Reporting" );
                _CswNbtSchemaModTrnsctn.Modules.AddPropToTab( ChemicalNT.NodeTypeId, "Hazard Classes", HazardsTab, 7, 2, "Fire Reporting" );
            }

            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            foreach( int ContainerNTId in ContainerOC.getNodeTypeIds() )
            {
                _CswNbtSchemaModTrnsctn.Modules.AddPropToTab( ContainerNTId, "Storage Pressure", "Fire Code" );
                _CswNbtSchemaModTrnsctn.Modules.AddPropToTab( ContainerNTId, "Storage Temperature", "Fire Code" );
                _CswNbtSchemaModTrnsctn.Modules.AddPropToTab( ContainerNTId, "Use Type", "Fire Code" );
            }

            #endregion

            #region Manufacturer Lot Info

            CswNbtMetaDataObjectClass ReceiptLotOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReceiptLotClass );
            foreach( CswNbtMetaDataNodeType ReceiptLotNT in ReceiptLotOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp ManufacturerNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( ReceiptLotNT.NodeTypeId, CswNbtObjClassReceiptLot.PropertyName.Manufacturer );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, ReceiptLotNT.NodeTypeId, ManufacturerNTP, true, ReceiptLotNT.getFirstNodeTypeTab().TabId, 7, 1 );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, ReceiptLotNT.NodeTypeId, ManufacturerNTP, true );
                CswNbtMetaDataNodeTypeProp ManufacturerLotNoNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( ReceiptLotNT.NodeTypeId, CswNbtObjClassReceiptLot.PropertyName.ManufacturerLotNo );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, ReceiptLotNT.NodeTypeId, ManufacturerLotNoNTP, true, ReceiptLotNT.getFirstNodeTypeTab().TabId, 8, 1 );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, ReceiptLotNT.NodeTypeId, ManufacturerLotNoNTP, true );
                CswNbtMetaDataNodeTypeProp ManufacturedDateNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( ReceiptLotNT.NodeTypeId, CswNbtObjClassReceiptLot.PropertyName.ManufacturedDate );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, ReceiptLotNT.NodeTypeId, ManufacturedDateNTP, true, ReceiptLotNT.getFirstNodeTypeTab().TabId, 9, 1 );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, ReceiptLotNT.NodeTypeId, ManufacturedDateNTP, true );
                _CswNbtSchemaModTrnsctn.Modules.AddPropToTab( ReceiptLotNT.NodeTypeId, CswNbtObjClassReceiptLot.PropertyName.AssignedCofA, ReceiptLotNT.getFirstNodeTypeTab(), 10, 1 );
                _CswNbtSchemaModTrnsctn.Modules.AddPropToTab( ReceiptLotNT.NodeTypeId, CswNbtObjClassReceiptLot.PropertyName.ViewCofA, ReceiptLotNT.getIdentityTab(), 1, 1 );
            }

            foreach( CswNbtMetaDataNodeType ContainerNT in ContainerOC.getNodeTypes() )
            {
                _CswNbtSchemaModTrnsctn.Modules.AddPropToTab( ContainerNT.NodeTypeId, "View C of A", ContainerNT.getIdentityTab(), 1, 3 );
            }

            #endregion

            #region MLM

            CswNbtMetaDataObjectClass containerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            foreach( CswNbtMetaDataNodeType containerNT in containerOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeTab cmgTab = containerNT.getNodeTypeTab( "Central Material Group" );
                if( null == cmgTab )
                {
                    cmgTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( containerNT, "Central Material Group", containerNT.getMaximumTabOrder() + 1 );
                }
                CswNbtMetaDataNodeTypeProp lotControlledNTP = containerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.LotControlled );
                lotControlledNTP.updateLayout( CswEnumNbtLayoutType.Edit, false, TabId : cmgTab.TabId );

                CswNbtMetaDataNodeTypeProp requisitionableNTP = containerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.Requisitionable );
                requisitionableNTP.updateLayout( CswEnumNbtLayoutType.Edit, false, TabId : cmgTab.TabId );

                CswNbtMetaDataNodeTypeProp reservedForNTP = containerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.ReservedFor );
                reservedForNTP.updateLayout( CswEnumNbtLayoutType.Edit, false, TabId : cmgTab.TabId );

                CswNbtMetaDataNodeTypeProp receiptLotNTP = containerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.ReceiptLot );
                CswNbtMetaDataNodeTypeTab firstTab = containerNT.getFirstNodeTypeTab();
                receiptLotNTP.updateLayout( CswEnumNbtLayoutType.Edit, false, TabId : firstTab.TabId );
            }

            CswNbtMetaDataObjectClass RequestMatDispOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestMaterialDispenseClass );
            foreach( CswNbtMetaDataNodeType NodeType in RequestMatDispOc.getLatestVersionNodeTypes() )
            {
                CswNbtMetaDataNodeTypeTab CmgTab = NodeType.getNodeTypeTab( "Central Material Group" )
                    ?? _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( NodeType, "Central Material Group", NodeType.getNextTabOrder() );

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

                CswNbtMetaDataNodeTypeTab ReceiveTab = NodeType.getNodeTypeTab( "Receive" ) 
                    ?? _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( NodeType, "Receive", NodeType.getNextTabOrder() );
                foreach( string ReceiveTabProp in CswNbtObjClassRequestMaterialDispense.PropertyName.MLMReceiveTabProps )
                {
                    CswNbtMetaDataNodeTypeProp ReceiveNtp = NodeType.getNodeTypePropByObjectClassProp( ReceiveTabProp );
                    ReceiveNtp.updateLayout( CswEnumNbtLayoutType.Edit, true, ReceiveTab.TabId );
                }
            }

            //Case 28339
            //   Show Vendor.VendorType and CorporateEntity
            //   Add a filter to Material.Supplier where VendorType = 'Coporate'
            CswNbtMetaDataObjectClass vendorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.VendorClass );
            foreach( CswNbtMetaDataNodeType vendorNT in vendorOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeTab firstTab = vendorNT.getFirstNodeTypeTab();
                CswNbtMetaDataNodeTypeProp vendorTypeNTP = vendorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassVendor.PropertyName.VendorTypeName );
                vendorTypeNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId : firstTab.TabId );
                vendorTypeNTP.updateLayout( CswEnumNbtLayoutType.Add, true, TabId : firstTab.TabId );

                CswNbtMetaDataNodeTypeProp corporateEntityNTP = vendorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassVendor.PropertyName.CorporateEntityName );
                corporateEntityNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId : firstTab.TabId );
            }

            _toggleMaterialRequestApprovalLevel( CswEnumNbtObjectClass.RequestMaterialCreateClass, true );
            _toggleMaterialRequestApprovalLevel( CswEnumNbtObjectClass.RequestMaterialDispenseClass, true );

            #endregion

        } // update()


        //From MLM module
        private void _toggleMaterialRequestApprovalLevel( CswEnumNbtObjectClass ObjClass, bool MLMDisabled )
        {
            CswNbtMetaDataObjectClass createMaterialRequestOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( ObjClass );
            foreach( CswNbtMetaDataNodeType createMaterialRequestNT in createMaterialRequestOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp approvalLevelNT = createMaterialRequestNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.ApprovalLevel );
                if( MLMDisabled )
                {
                    approvalLevelNT.removeFromAllLayouts();
                }
                else
                {
                    CswNbtMetaDataNodeTypeTab firstTab = createMaterialRequestNT.getFirstNodeTypeTab();
                    approvalLevelNT.updateLayout( CswEnumNbtLayoutType.Edit, true, TabId : firstTab.TabId );
                    approvalLevelNT.updateLayout( CswEnumNbtLayoutType.Add, true );
                }
            }
        }

    }

}//namespace ChemSW.Nbt.Schema