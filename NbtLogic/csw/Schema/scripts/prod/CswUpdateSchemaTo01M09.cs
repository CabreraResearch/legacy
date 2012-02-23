using System;
using System.Data;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01M-09
    /// </summary>
    public class CswUpdateSchemaTo01M09 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'M', 09 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {

            #region case 24457-ChemicalNodeType

            CswNbtMetaDataObjectClass MaterialObjectClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            string ChemicalNodeTypeName = "Chemical";
            string MaterialsCategory = "Materials";
            CswNbtMetaDataNodeType ChemicalNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( ChemicalNodeTypeName );
            if( null == ChemicalNodeType || ChemicalNodeType.getObjectClass().ObjectClass != MaterialObjectClass.ObjectClass )
            {
                //**********************************************************************
                //Identity Tab
                ChemicalNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass.ToString(), ChemicalNodeTypeName, MaterialsCategory );
                CswNbtMetaDataNodeTypeTab ChemicalIdentityTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( ChemicalNodeType, "Identity", 0 );


                //Object Class based props first
                CswNbtMetaDataNodeTypeProp CasNoProp = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( ChemicalNodeType.NodeTypeId, CswNbtObjClassMaterial.CasNoPropertyName );
                CasNoProp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, ChemicalIdentityTab.TabId );

                //nu props
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.Grid, "Synonymns", ChemicalIdentityTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.Grid, "Components", ChemicalIdentityTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.Quantity, "Expiration Interval", ChemicalIdentityTab.TabId );

                //**********************************************************************
                //Hazards Tab
                CswNbtMetaDataNodeTypeTab HazardsTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( ChemicalNodeType, "Hazards", 1 );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.NFPA, "NFPA", HazardsTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.MultiList, "GHS", HazardsTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.MultiList, "EU R&S", HazardsTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.MultiList, "EU Picto", HazardsTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.MultiList, "PPE", HazardsTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.Logical, "Hazardous", HazardsTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.ImageList, "Storage Type", HazardsTab.TabId );

                //**********************************************************************
                //Physical Tab
                CswNbtMetaDataNodeTypeTab PhysicalTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( ChemicalNodeType, "Physical Tab", 2 );

                //Object Class based props
                CswNbtMetaDataNodeTypeProp SpecificGravityProp = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( ChemicalNodeType.NodeTypeId, CswNbtObjClassMaterial.SpecificGravityPropertyName );
                SpecificGravityProp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, PhysicalTab.TabId );

                CswNbtMetaDataNodeTypeProp PhysicalStateProp = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( ChemicalNodeType.NodeTypeId, CswNbtObjClassMaterial.PhysicalStatePropertyName );
                PhysicalStateProp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, PhysicalTab.TabId );
                PhysicalStateProp.ListOptions = "Solid, Liquid, Gas";

                //nu props
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.Text, "Formula", PhysicalTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.MOL, "Structure", PhysicalTab.TabId );



                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.Memo, "Physical Description", PhysicalTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.Text, "Molecular Weight", PhysicalTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.Text, "pH", PhysicalTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.Text, "Boiling Point", PhysicalTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.Text, "Melting Point", PhysicalTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.Text, "Aqueous Solubility", PhysicalTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.Text, "Flash Point", PhysicalTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.Text, "Vapor Pressure", PhysicalTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.Text, "Vapor Density", PhysicalTab.TabId );

            }//if else the Chemical node type does not already exist

            #endregion

            #region case 24457-SupplyNodeType


            string SupplyNodeTypeName = "Supply";
            CswNbtMetaDataNodeType SupplyNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( SupplyNodeTypeName );
            if( null == SupplyNodeTypeName || SupplyNodeType.getObjectClass().ObjectClass != MaterialObjectClass.ObjectClass )
            {
                SupplyNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass.ToString(), ChemicalNodeTypeName, MaterialsCategory );
                CswNbtMetaDataNodeTypeTab SupplyIdentityTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( SupplyNodeType, "Identity", 0 );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( SupplyNodeType, CswNbtMetaDataFieldType.NbtFieldType.Memo, "Description", SupplyIdentityTab.TabId );

                CswNbtMetaDataNodeTypeTab PictureTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( SupplyNodeType, "Identity", 0 );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( SupplyNodeType, CswNbtMetaDataFieldType.NbtFieldType.ImageList, "Picture", PictureTab.TabId );

            }//if else the Supply nodetype already exists

            #endregion

            #region case 24457-BiologicalNodeType

            string BiologicalNodeTypeName = "Biological";
            CswNbtMetaDataNodeType BiologicalNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( BiologicalNodeTypeName );
            if( null == BiologicalNodeTypeName || BiologicalNodeType.getObjectClass().ObjectClass != MaterialObjectClass.ObjectClass )
            {
                BiologicalNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass.ToString(), ChemicalNodeTypeName, MaterialsCategory );
                CswNbtMetaDataNodeTypeTab BiologicalIdentityTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( BiologicalNodeType, "Identity", 0 );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( BiologicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.List, "Reference Type", BiologicalIdentityTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( BiologicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.Text, "Reference Number", BiologicalIdentityTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( BiologicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.List, "Type", BiologicalIdentityTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( BiologicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.Text, "Species Origin", BiologicalIdentityTab.TabId );


                CswNbtMetaDataNodeTypeTab BiosafetyTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( BiologicalNodeType, "Identity", 0 );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( BiologicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.List, "Biosafety Level", BiosafetyTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( BiologicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.MultiList, "Vectors", BiosafetyTab.TabId );

            }//if else the Supply nodetype already exists

            #endregion

        }//Update()

    }//class  CswUpdateSchemaTo01M09

}//namespace ChemSW.Nbt.Schema