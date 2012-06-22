using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 25898
    /// </summary>
    public class CswUpdateSchemaCase25898 : CswUpdateSchemaTo
    {


        public override void update()
        {
            // case 25898 - Fix material layout
            // Note: While it is possible for nodetype and property names to have been mutated by customers, 
            // it's impossible at this stage of CISPro development, so we can rely on existing names.

            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            CswNbtMetaDataNodeTypeLayoutMgr.LayoutType EditLayout = CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit;


                            CswNbtMetaDataNodeTypeTab IdentityTab;
                            CswNbtMetaDataNodeTypeTab namedTab; 
                            CswNbtMetaDataNodeTypeTab HazardsTab;
                            CswNbtMetaDataNodeTypeTab PhysicalTab; 
                            CswNbtMetaDataNodeTypeTab StructureTab;
                            CswNbtMetaDataNodeTypeTab DocumentsTab;
                            CswNbtMetaDataNodeTypeTab ContainersTab;
                            CswNbtMetaDataNodeTypeTab BiosafetyTab;
                            CswNbtMetaDataNodeTypeTab PictureTab;

            foreach( CswNbtMetaDataNodeType MaterialNT in MaterialOC.getNodeTypes() )
            {
                foreach( CswNbtMetaDataNodeTypeProp MatNTP in MaterialNT.getNodeTypeProps() )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.removePropFromAllLayouts( MatNTP );

                    switch( MaterialNT.NodeTypeName )
                    {
                        case "Chemical":
                            IdentityTab = _getTab( MaterialNT, "Identity", 0 );
                            namedTab = _getTab( MaterialNT, MaterialNT.NodeTypeName, 1 );
                            HazardsTab = _getTab( MaterialNT, "Hazards", 2 );
                            PhysicalTab = _getTab( MaterialNT, "Physical", 3 );
                            StructureTab = _getTab( MaterialNT, "Structure", 4 );
                            DocumentsTab = _getTab( MaterialNT, "Documents", 5 );
                            ContainersTab = _getTab( MaterialNT, "Containers", 6 );

                            switch( MatNTP.PropName )
                            {
                                // Identity
                                case CswNbtObjClassMaterial.TradenamePropertyName: MatNTP.updateLayout( EditLayout, false, IdentityTab.TabId, 1, 1 ); break;
                                case CswNbtObjClassMaterial.SupplierPropertyName: MatNTP.updateLayout( EditLayout, false, IdentityTab.TabId, 1, 2 ); break;
                                case CswNbtObjClassMaterial.PartNumberPropertyName: MatNTP.updateLayout( EditLayout, false, IdentityTab.TabId, 1, 3 ); break;
                                case CswNbtObjClassMaterial.RequestPropertyName: MatNTP.updateLayout( EditLayout, false, IdentityTab.TabId, 1, 4 ); break;

                                // Chemical
                                case "Synonyms": MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 1 ); break;
                                case CswNbtObjClassMaterial.CasNoPropertyName: MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 2 ); break;
                                case "Components": MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 3 ); break;
                                case CswNbtObjClassMaterial.RegulatoryListsPropertyName: MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 4 ); break; ;
                                case CswNbtObjClassMaterial.ExpirationIntervalPropertyName: MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 5 ); break;
                                case CswNbtObjClassMaterial.ApprovalStatusPropertyName: MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 6 ); break;

                                // Hazards
                                case "Hazardous": MatNTP.updateLayout( EditLayout, false, HazardsTab.TabId, 1, 1 ); break;
                                case "NFPA": MatNTP.updateLayout( EditLayout, false, HazardsTab.TabId, 1, 2 ); break;
                                case CswNbtObjClassMaterial.StorageCompatibilityPropertyName: MatNTP.updateLayout( EditLayout, false, HazardsTab.TabId, 1, 3 ); break;
                                case "PPE": MatNTP.updateLayout( EditLayout, false, HazardsTab.TabId, 1, 4 ); break;

                                // Physical
                                case "Physical Description": MatNTP.updateLayout( EditLayout, false, PhysicalTab.TabId, 1, 1 ); break;
                                case CswNbtObjClassMaterial.PhysicalStatePropertyName: MatNTP.updateLayout( EditLayout, false, PhysicalTab.TabId, 1, 2 ); break;
                                case "Molecular Weight": MatNTP.updateLayout( EditLayout, false, PhysicalTab.TabId, 1, 3 ); break;
                                case CswNbtObjClassMaterial.SpecificGravityPropertyName: MatNTP.updateLayout( EditLayout, false, PhysicalTab.TabId, 1, 4 ); break;
                                case "pH": MatNTP.updateLayout( EditLayout, false, PhysicalTab.TabId, 1, 5 ); break;
                                case "Boiling Point": MatNTP.updateLayout( EditLayout, false, PhysicalTab.TabId, 2, 3 ); break;
                                case "Melting Point": MatNTP.updateLayout( EditLayout, false, PhysicalTab.TabId, 2, 4 ); break;
                                case "Aqueous Solubility": MatNTP.updateLayout( EditLayout, false, PhysicalTab.TabId, 2, 5 ); break;
                                case "Flash Point": MatNTP.updateLayout( EditLayout, false, PhysicalTab.TabId, 3, 3 ); break;
                                case "Vapor Pressure": MatNTP.updateLayout( EditLayout, false, PhysicalTab.TabId, 3, 4 ); break;
                                case "Vapor Density": MatNTP.updateLayout( EditLayout, false, PhysicalTab.TabId, 3, 5 ); break;
                                case "Inventory Levels": MatNTP.updateLayout( EditLayout, false, PhysicalTab.TabId, 1, 6 ); break;

                                // Structure
                                case "Formula": MatNTP.updateLayout( EditLayout, false, StructureTab.TabId, 1, 1 ); break;
                                case "Structure": MatNTP.updateLayout( EditLayout, false, StructureTab.TabId, 1, 2 ); break;

                                // Documents
                                case "Documents": MatNTP.updateLayout( EditLayout, false, DocumentsTab.TabId, 1, 1 ); break;
                            }

                            break;
                        case "Biological":
                            IdentityTab = _getTab( MaterialNT, "Identity", 0 );
                            namedTab = _getTab( MaterialNT, MaterialNT.NodeTypeName, 1 );
                            BiosafetyTab = _getTab( MaterialNT, "Biosafety", 2 );
                            PictureTab = _getTab( MaterialNT, "Picture", 3 );
                            DocumentsTab = _getTab( MaterialNT, "Documents", 4 );
                            ContainersTab = _getTab( MaterialNT, "Containers", 5 );

                            switch( MatNTP.PropName )
                            {
                                case CswNbtObjClassMaterial.TradenamePropertyName: MatNTP.updateLayout( EditLayout, false, IdentityTab.TabId, 1, 1 ); break;
                                case CswNbtObjClassMaterial.SupplierPropertyName: MatNTP.updateLayout( EditLayout, false, IdentityTab.TabId, 1, 2 ); break;
                                case CswNbtObjClassMaterial.PartNumberPropertyName: MatNTP.updateLayout( EditLayout, false, IdentityTab.TabId, 1, 3 ); break;
                                case CswNbtObjClassMaterial.RequestPropertyName: MatNTP.updateLayout( EditLayout, false, IdentityTab.TabId, 1, 4 ); break;

                                case "Synonyms": MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 1 ); break;
                                case CswNbtObjClassMaterial.CasNoPropertyName: MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 2 ); break;
                                case CswNbtObjClassMaterial.RegulatoryListsPropertyName: MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 4 ); break; ;
                                case CswNbtObjClassMaterial.ExpirationIntervalPropertyName: MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 5 ); break;
                                case CswNbtObjClassMaterial.ApprovalStatusPropertyName: MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 6 ); break;
                                case "Reference Type": MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 7 ); break;
                                case "Reference Number": MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 8 ); break;
                                case "Type": MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 9 ); break;
                                case "Species Origin": MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 10 ); break;

                                case "Biosafety Level": MatNTP.updateLayout( EditLayout, false, BiosafetyTab.TabId, 1, 1 ); break;
                                case "Vectors": MatNTP.updateLayout( EditLayout, false, BiosafetyTab.TabId, 1, 2 ); break;
                                case "Inventory Levels": MatNTP.updateLayout( EditLayout, false, BiosafetyTab.TabId, 1, 3 ); break;
                                case CswNbtObjClassMaterial.StorageCompatibilityPropertyName: MatNTP.updateLayout( EditLayout, false, BiosafetyTab.TabId, 1, 4 ); break;

                                case CswNbtObjClassMaterial.PhysicalStatePropertyName: MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 2 ); break;
                                case CswNbtObjClassMaterial.SpecificGravityPropertyName: MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 4 ); break;

                                case "Documents": MatNTP.updateLayout( EditLayout, false, DocumentsTab.TabId, 1, 1 ); break;
                            }

                            break;
                        case "Supply":

                            //Supply
                            //    Supply
                            //        materialsubclass
                            //        physical_state
                            //        Approval Status
                            //        Specific Gravity
                            //        Physical State
                            //        CAS No
                            //        Regulatory Lists
                            //        Part Number
                            //        Tradename
                            //        Storage Compatibility
                            //        Supplier
                            //        Expiration Interval
                            //    Identity
                            //        Description
                            //        Synonyms
                            //    Picture
                            //        Picture
                            //    Documents
                            //        Documents
                            //    Requests
                            //        Request
                            //        Submitted Requests
                            //    Physical
                            //        Inventory Levels

                            IdentityTab = _getTab( MaterialNT, "Identity", 0 );
                            namedTab = _getTab( MaterialNT, MaterialNT.NodeTypeName, 1 );
                            PictureTab = _getTab( MaterialNT, "Picture", 2 );
                            DocumentsTab = _getTab( MaterialNT, "Documents", 3 );
                            ContainersTab = _getTab( MaterialNT, "Containers", 4 );

                            switch( MatNTP.PropName )
                            {
                                case CswNbtObjClassMaterial.TradenamePropertyName: MatNTP.updateLayout( EditLayout, false, IdentityTab.TabId, 1, 1 ); break;
                                case CswNbtObjClassMaterial.SupplierPropertyName: MatNTP.updateLayout( EditLayout, false, IdentityTab.TabId, 1, 2 ); break;
                                case CswNbtObjClassMaterial.PartNumberPropertyName: MatNTP.updateLayout( EditLayout, false, IdentityTab.TabId, 1, 3 ); break;
                                case CswNbtObjClassMaterial.RequestPropertyName: MatNTP.updateLayout( EditLayout, false, IdentityTab.TabId, 1, 4 ); break;

                                case "Synonyms": MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 1 ); break;
                                case CswNbtObjClassMaterial.CasNoPropertyName: MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 2 ); break;
                                case "Components": MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 3 ); break;
                                case CswNbtObjClassMaterial.RegulatoryListsPropertyName: MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 4 ); break; ;
                                case CswNbtObjClassMaterial.ExpirationIntervalPropertyName: MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 5 ); break;
                                case CswNbtObjClassMaterial.ApprovalStatusPropertyName: MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 6 ); break;

                                //case CswNbtObjClassMaterial.StorageCompatibilityPropertyName: MatNTP.updateLayout( EditLayout, false, HazardsTab.TabId, 1, 3 ); break;

                                //case CswNbtObjClassMaterial.PhysicalStatePropertyName: MatNTP.updateLayout( EditLayout, false, PhysicalTab.TabId, 1, 2 ); break;
                                //case CswNbtObjClassMaterial.SpecificGravityPropertyName: MatNTP.updateLayout( EditLayout, false, PhysicalTab.TabId, 1, 4 ); break;

                                case "Documents": MatNTP.updateLayout( EditLayout, false, DocumentsTab.TabId, 1, 1 ); break;
                                case "": break;
                            }

                            break;
                    } // switch( MaterialNT.NodeTypeName )
                } // foreach( CswNbtMetaDataNodeTypeProp MatNTP in MaterialNT.getNodeTypeProps() )
            } // foreach( CswNbtMetaDataNodeType MaterialNT in MaterialOC.getNodeTypes() )


            //1) move all Identity tab properties to [nodetypename] tab (eg. "Chemical" tab for Chemicals)
            //2) remove Identity tab
            //3) Add form, first 3 fields: Tradename*, Supplier*, Part#, physical state
            //4) physical state must always be set, but only visible for Chemicals and Biologicals. Supplies are always solid. Biologicals can only be liquid or solid (never gas). Goes on Physical tab.
            //5) physical state should be on Physical tab for Chemicals and Biosafety tab for Biologicals
            //6) property Storage Type should be named "Storage Compatibility" and is only visible and only tested for chemicals. Goes on Hazards tab. Add graphics to imagelist.
            //7) new Containers tab: has Sizes linkgrid at top, then thickgrid of Containers (barcode, quantity, owner, expiration,location)
            //8) Move Inventory Levels prop to above containers tab rid (1 row: sizes on left, Inventory Levels on right)
            //9) add icons to all nodetypes
            //10) set nodenametemplate to {tradename} for all nodetypes
            //11) components linkgrid only applies to chemicals
            //12) specific gravity is required, always defaults to 1.0, should be numeric not scientific, and is only visible under same conditions as physical state.


            //Chemicals:
            //1) Structure tab has Structure (ft=MOL) property, and move Formula property above it

            //Supplies:
            //1) Picture is an image, not imagelist
            //2) hide (not used here): expiration interval,casno,reglists
            //3) hide reglists


            //Biologicals:
            //1) rename property "Tradename to "Bioliogical Name"
            //2) hide: casno, storage compatability
            //3) new property Storage Conditions (list: 37C, 25C, 5C, -20C, -80C) 
            //4) hide reglists

        } // update() 


        private CswNbtMetaDataNodeTypeTab _getTab( CswNbtMetaDataNodeType MaterialNT, string TabName, Int32 Order )
        {
            CswNbtMetaDataNodeTypeTab tab = MaterialNT.getNodeTypeTab( TabName );
            if( null == tab )
            {
                tab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( MaterialNT, MaterialNT.NodeTypeName, Order );
            }
            else
            {
                tab.TabOrder = Order;
            }
            return tab;
        } // _getTab()
        //private void _removeTab( CswNbtMetaDataNodeType MaterialNT, string TabName )
        //{
        //    CswNbtMetaDataNodeTypeTab tab = MaterialNT.getNodeTypeTab( TabName );
        //    if( null != tab )
        //    {
        //        _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeTab( tab );
        //    }
        //} // _removeTab()

    }//class CswUpdateSchemaCase25898

}//namespace ChemSW.Nbt.Schema
