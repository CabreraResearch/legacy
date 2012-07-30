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
            CswNbtMetaDataNodeTypeLayoutMgr.LayoutType AddLayout = CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add;

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
                            MaterialNT.IconFileName = "atom.gif";
                            MaterialNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassMaterial.TradenamePropertyName ) );

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
                                case CswNbtObjClassMaterial.TradenamePropertyName: 
                                    MatNTP.updateLayout( EditLayout, false, IdentityTab.TabId, 1, 1 );
                                    MatNTP.updateLayout( AddLayout, false, Int32.MinValue, 1, 1 ); 
                                    break;
                                case CswNbtObjClassMaterial.SupplierPropertyName: 
                                    MatNTP.updateLayout( EditLayout, false, IdentityTab.TabId, 2, 1 );
                                    MatNTP.updateLayout( AddLayout, false, Int32.MinValue, 2, 1 ); 
                                    break;
                                case CswNbtObjClassMaterial.PartNumberPropertyName: 
                                    MatNTP.updateLayout( EditLayout, false, IdentityTab.TabId, 3, 1 ); 
                                    MatNTP.updateLayout( AddLayout, false, Int32.MinValue, 3, 1 ); 
                                    break;
                                case CswNbtObjClassMaterial.RequestPropertyName: MatNTP.updateLayout( EditLayout, false, IdentityTab.TabId, 4, 1 ); break;
                                case CswNbtObjClassMaterial.ReceivePropertyName: MatNTP.updateLayout( EditLayout, false, IdentityTab.TabId, 5, 1 ); break;

                                // Chemical
                                case "Synonyms": MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 1 ); break;
                                case CswNbtObjClassMaterial.CasNoPropertyName: MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 2, 1 ); break;
                                case "Components": MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 3, 1 ); break;
                                case CswNbtObjClassMaterial.RegulatoryListsPropertyName: MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 4, 1 ); break; ;
                                case CswNbtObjClassMaterial.ExpirationIntervalPropertyName: MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 5, 1 ); break;
                                case CswNbtObjClassMaterial.ApprovalStatusPropertyName: MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 6, 1 ); break;

                                // Hazards
                                case "Hazardous": MatNTP.updateLayout( EditLayout, false, HazardsTab.TabId, 1, 1 ); break;
                                case "NFPA": MatNTP.updateLayout( EditLayout, false, HazardsTab.TabId, 2, 1 ); break;
                                case CswNbtObjClassMaterial.StorageCompatibilityPropertyName: MatNTP.updateLayout( EditLayout, false, HazardsTab.TabId, 3, 1 ); break;
                                case "PPE": MatNTP.updateLayout( EditLayout, false, HazardsTab.TabId, 4, 1 ); break;

                                // Physical
                                case "Physical Description":
                                    MatNTP.updateLayout( EditLayout, false, PhysicalTab.TabId, 1, 1 );
                                    MatNTP.TextAreaColumns = 20;
                                    break;
                                case CswNbtObjClassMaterial.PhysicalStatePropertyName: 
                                    MatNTP.updateLayout( EditLayout, false, PhysicalTab.TabId, 2, 1 ); 
                                    MatNTP.updateLayout( AddLayout, false, Int32.MinValue, 4, 1 );
                                    break;
                                case "Molecular Weight": MatNTP.updateLayout( EditLayout, false, PhysicalTab.TabId, 3, 1 ); break;
                                case CswNbtObjClassMaterial.SpecificGravityPropertyName: MatNTP.updateLayout( EditLayout, false, PhysicalTab.TabId, 4, 1 ); break;
                                case "pH": MatNTP.updateLayout( EditLayout, false, PhysicalTab.TabId, 5, 1 ); break;
                                case "Boiling Point": MatNTP.updateLayout( EditLayout, false, PhysicalTab.TabId, 3, 2 ); break;
                                case "Melting Point": MatNTP.updateLayout( EditLayout, false, PhysicalTab.TabId, 4, 2 ); break;
                                case "Aqueous Solubility": MatNTP.updateLayout( EditLayout, false, PhysicalTab.TabId, 5, 2 ); break;
                                case "Flash Point": MatNTP.updateLayout( EditLayout, false, PhysicalTab.TabId, 3, 3 ); break;
                                case "Vapor Pressure": MatNTP.updateLayout( EditLayout, false, PhysicalTab.TabId, 4, 3 ); break;
                                case "Vapor Density": MatNTP.updateLayout( EditLayout, false, PhysicalTab.TabId, 5, 3 ); break;
                                
                                // Structure
                                case "Formula": MatNTP.updateLayout( EditLayout, false, StructureTab.TabId, 1, 1 ); break;
                                case "Structure": MatNTP.updateLayout( EditLayout, false, StructureTab.TabId, 2, 1 ); break;

                                // Documents
                                case "Documents": MatNTP.updateLayout( EditLayout, false, DocumentsTab.TabId, 1, 1 ); break;

                                // Containers
                                case "Inventory Levels": MatNTP.updateLayout( EditLayout, false, ContainersTab.TabId, 1, 2 ); break;

                                // (delete)
                                case "Storage Type": _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( MatNTP ); break;
                            }

                            break;
                        case "Biological":
                            MaterialNT.IconFileName = "dna.gif";
                            MaterialNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( "Biological Name" ) );

                            IdentityTab = _getTab( MaterialNT, "Identity", 0 );
                            namedTab = _getTab( MaterialNT, MaterialNT.NodeTypeName, 1 );
                            BiosafetyTab = _getTab( MaterialNT, "Biosafety", 2 );
                            PictureTab = _getTab( MaterialNT, "Picture", 3 );
                            DocumentsTab = _getTab( MaterialNT, "Documents", 4 );
                            ContainersTab = _getTab( MaterialNT, "Containers", 5 );

                            _removeTab( MaterialNT, "Physical" );

                            switch( MatNTP.PropName )
                            {
                                // Identity
                                case CswNbtObjClassMaterial.TradenamePropertyName:
                                    MatNTP.PropName = "Biological Name";
                                    MatNTP.updateLayout( EditLayout, false, IdentityTab.TabId, 1, 1 );
                                    MatNTP.updateLayout( AddLayout, false, Int32.MinValue, 1, 1 );
                                    break;
                                case CswNbtObjClassMaterial.SupplierPropertyName:
                                    MatNTP.updateLayout( EditLayout, false, IdentityTab.TabId, 2, 1 );
                                    MatNTP.updateLayout( AddLayout, false, Int32.MinValue, 2, 1 );
                                    break;
                                case CswNbtObjClassMaterial.PartNumberPropertyName:
                                    MatNTP.updateLayout( EditLayout, false, IdentityTab.TabId, 3, 1 );
                                    MatNTP.updateLayout( AddLayout, false, Int32.MinValue, 3, 1 );
                                    break;
                                case CswNbtObjClassMaterial.RequestPropertyName: MatNTP.updateLayout( EditLayout, false, IdentityTab.TabId, 4, 1 ); break;

                                // Biological
                                case "Synonyms": MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 1 ); break;
                                case "Reference Number": MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 2, 1 ); break;
                                case "Type": MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 3, 1 ); break;
                                case "Species Origin": MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 4, 1 ); break;
                                case "Vectors": MatNTP.updateLayout( EditLayout, false, BiosafetyTab.TabId, 5, 1 ); break;
                                case "Reference Type": MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 6, 1 ); break;
                                //case CswNbtObjClassMaterial.ExpirationIntervalPropertyName: MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 6, 1 ); break;
                                case CswNbtObjClassMaterial.ApprovalStatusPropertyName: MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 7, 1 ); break;

                                // Biosafety
                                case "Biosafety Level": MatNTP.updateLayout( EditLayout, false, BiosafetyTab.TabId, 1, 1 ); break;
                                case CswNbtObjClassMaterial.PhysicalStatePropertyName: 
                                    MatNTP.updateLayout( EditLayout, false, BiosafetyTab.TabId, 3, 1 );
                                    MatNTP.updateLayout( AddLayout, false, Int32.MinValue, 4, 1 );
                                    MatNTP.ListOptions = CswNbtObjClassMaterial.PhysicalStates.Options.ToString();
                                    break;
                                case CswNbtObjClassMaterial.SpecificGravityPropertyName: MatNTP.updateLayout( EditLayout, false, BiosafetyTab.TabId, 4, 1 ); break;
                                //case CswNbtObjClassMaterial.StorageCompatibilityPropertyName: MatNTP.updateLayout( EditLayout, false, BiosafetyTab.TabId, 5, 1 ); break;

                                // Documents
                                case "Documents": MatNTP.updateLayout( EditLayout, false, DocumentsTab.TabId, 1, 1 ); break;

                                // Containers
                                case "Inventory Levels": MatNTP.updateLayout( EditLayout, false, ContainersTab.TabId, 1, 2 ); break;

                                //case CswNbtObjClassMaterial.CasNoPropertyName: MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 2 ); break;
                                //case CswNbtObjClassMaterial.RegulatoryListsPropertyName: MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 4 ); break;

                                // (delete)
                                case "Storage Type": _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( MatNTP ); break;
                            }
                            break;
                        case "Supply":
                            MaterialNT.IconFileName = "tube.gif";
                            MaterialNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassMaterial.TradenamePropertyName ) );

                            IdentityTab = _getTab( MaterialNT, "Identity", 0 );
                            namedTab = _getTab( MaterialNT, MaterialNT.NodeTypeName, 1 );
                            PictureTab = _getTab( MaterialNT, "Picture", 2 );
                            DocumentsTab = _getTab( MaterialNT, "Documents", 3 );
                            ContainersTab = _getTab( MaterialNT, "Containers", 4 );

                            _removeTab( MaterialNT, "Physical" );

                            switch( MatNTP.PropName )
                            {
                                // Identity
                                case CswNbtObjClassMaterial.TradenamePropertyName:
                                    MatNTP.updateLayout( EditLayout, false, IdentityTab.TabId, 1, 1 );
                                    MatNTP.updateLayout( AddLayout, false, Int32.MinValue, 1, 1 );
                                    break;
                                case CswNbtObjClassMaterial.SupplierPropertyName:
                                    MatNTP.updateLayout( EditLayout, false, IdentityTab.TabId, 2, 1 );
                                    MatNTP.updateLayout( AddLayout, false, Int32.MinValue, 2, 1 );
                                    break;
                                case CswNbtObjClassMaterial.PartNumberPropertyName:
                                    MatNTP.updateLayout( EditLayout, false, IdentityTab.TabId, 3, 1 );
                                    MatNTP.updateLayout( AddLayout, false, Int32.MinValue, 3, 1 );
                                    break;
                                case CswNbtObjClassMaterial.RequestPropertyName: MatNTP.updateLayout( EditLayout, false, IdentityTab.TabId, 4, 1 ); break;

                                // Supply
                                case "Description": MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 1 ); break;
                                case "Synonyms": MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 2, 1 ); break;
                                //case CswNbtObjClassMaterial.ExpirationIntervalPropertyName: MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 4, 1 ); break;
                                case CswNbtObjClassMaterial.ApprovalStatusPropertyName: MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 5, 1 ); break;

                                // Picture
                                case "Picture": MatNTP.updateLayout( EditLayout, false, PictureTab.TabId, 1, 1 ); break;

                                // Documents
                                case "Documents": MatNTP.updateLayout( EditLayout, false, DocumentsTab.TabId, 1, 1 ); break;

                                //case CswNbtObjClassMaterial.CasNoPropertyName: MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 2 ); break;
                                //case CswNbtObjClassMaterial.RegulatoryListsPropertyName: MatNTP.updateLayout( EditLayout, false, namedTab.TabId, 1, 4 ); break; ;
                                //case CswNbtObjClassMaterial.StorageCompatibilityPropertyName: MatNTP.updateLayout( EditLayout, false, HazardsTab.TabId, 1, 3 ); break;
                                //case CswNbtObjClassMaterial.PhysicalStatePropertyName: MatNTP.updateLayout( EditLayout, false, PhysicalTab.TabId, 1, 2 ); break;
                                //case CswNbtObjClassMaterial.SpecificGravityPropertyName: MatNTP.updateLayout( EditLayout, false, PhysicalTab.TabId, 1, 4 ); break;

                                // Containers
                                case "Inventory Levels": MatNTP.updateLayout( EditLayout, false, ContainersTab.TabId, 1, 2 ); break;

                                // (delete)
                                case "Storage Type": _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( MatNTP ); break;
                                case "Components": _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( MatNTP ); break;
                            }

                            break;
                    } // switch( MaterialNT.NodeTypeName )
                } // foreach( CswNbtMetaDataNodeTypeProp MatNTP in MaterialNT.getNodeTypeProps() )
            } // foreach( CswNbtMetaDataNodeType MaterialNT in MaterialOC.getNodeTypes() )
        } // update() 


        private CswNbtMetaDataNodeTypeTab _getTab( CswNbtMetaDataNodeType MaterialNT, string TabName, Int32 Order )
        {
            CswNbtMetaDataNodeTypeTab tab = MaterialNT.getNodeTypeTab( TabName );
            if( null == tab )
            {
                tab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( MaterialNT, TabName, Order );
            }
            else
            {
                tab.TabOrder = Order;
            }
            return tab;
        } // _getTab()

        private void _removeTab( CswNbtMetaDataNodeType MaterialNT, string TabName )
        {
            CswNbtMetaDataNodeTypeTab tab = MaterialNT.getNodeTypeTab( TabName );
            if( null != tab )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeTab( tab );
            }
        } // _removeTab()

    }//class CswUpdateSchemaCase25898

}//namespace ChemSW.Nbt.Schema
