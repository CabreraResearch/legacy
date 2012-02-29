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
    public class CswUpdateSchemaTo01M10 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'M', 10 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {

            string MaterialsCategory = "Materials";

            #region case 24459

            CswTableUpdate ObjectClassUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "createcomponentobjectclass", "object_class" );
            DataTable ObjectClasstable = ObjectClassUpdate.getEmptyTable();
            DataRow NewObjectClassRow = ObjectClasstable.NewRow();
            ObjectClasstable.Rows.Add( NewObjectClassRow );
            NewObjectClassRow["objectclass"] = CswNbtMetaDataObjectClass.NbtObjectClass.ComponentClass.ToString();
            ObjectClassUpdate.update( ObjectClasstable );

            CswNbtMetaDataObjectClassProp ComponentNumberProp = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.ComponentClass, CswNbtObjClassComponent.PercentagePropertyName, CswNbtMetaDataFieldType.NbtFieldType.Number );

            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( _CswNbtSchemaModTrnsctn.getModuleId( CswNbtResources.CswNbtModule.CISPro ), _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ComponentClass ).ObjectClassId );


            string ComponentNodeTypeName = "Component";
            CswNbtMetaDataNodeType ComponentNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass.ToString(), ComponentNodeTypeName, MaterialsCategory );
            CswNbtView ComponentView = _CswNbtSchemaModTrnsctn.makeView();
            ComponentView.makeNew( "Components", NbtViewVisibility.Global, null, null, null );
            CswNbtViewRelationship ComponentRelationship = ComponentView.AddViewRelationship( ComponentNodeType, false );
            ComponentView.Category = MaterialsCategory;
            ComponentView.save();


            #endregion


            #region case 24981
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( _CswNbtSchemaModTrnsctn.getModuleId( CswNbtResources.CswNbtModule.CISPro ), _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass ).ObjectClassId );

            CswNbtMetaDataObjectClassProp SynonymMaterialObjClassProp = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialSynonymClass, CswNbtObjClassMaterialSynonym.MaterialPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Relationship );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( SynonymMaterialObjClassProp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fktype, NbtViewRelatedIdType.ObjectClassId.ToString() );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( SynonymMaterialObjClassProp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fkvalue, _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass ) );

            CswNbtMetaDataObjectClassProp SynonymNameObjClassProp = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialSynonymClass, CswNbtObjClassMaterialSynonym.NamePropertyName, CswNbtMetaDataFieldType.NbtFieldType.Text );


            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( _CswNbtSchemaModTrnsctn.getModuleId( CswNbtResources.CswNbtModule.CISPro ), _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialSynonymClass ).ObjectClassId );


            CswNbtMetaDataNodeType MaterialSynonymNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( SynonymMaterialObjClassProp.ObjectClassId, "Material Synonym", MaterialsCategory );

            CswNbtView SynonymView = _CswNbtSchemaModTrnsctn.makeView();
            SynonymView.makeNew( "Synonyms", NbtViewVisibility.Global, null, null, null );
            CswNbtViewRelationship SynonymTypeRelationship = SynonymView.AddViewRelationship( MaterialSynonymNodeType, false );
            SynonymView.Category = MaterialsCategory;
            SynonymView.save();
            #endregion


            #region case 24457-ChemicalNodeType

            CswNbtMetaDataObjectClass MaterialObjectClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            string ChemicalNodeTypeName = "Chemical";
            CswNbtMetaDataNodeType ChemicalNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( ChemicalNodeTypeName );
            if( null == ChemicalNodeType )
            {
                //**********************************************************************
                //Identity Tab
                ChemicalNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass.ToString(), ChemicalNodeTypeName, MaterialsCategory );
                CswNbtMetaDataNodeTypeTab ChemicalIdentityTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( ChemicalNodeType, "Identity", 0 );


                //CswNbtView ChemicalView = ChemicalNodeType.CreateDefaultView();// this causes an exception when you try to save the view, to the effect of: "You must call makeNewView() before saving"
                CswNbtView ChemicalView = _CswNbtSchemaModTrnsctn.makeView();
                ChemicalView.makeNew( "Chemicals", NbtViewVisibility.Global, null, null, null );
                CswNbtViewRelationship ChemicalRelationship = ChemicalView.AddViewRelationship( ChemicalNodeType, false );
                ChemicalView.Category = MaterialsCategory;
                ChemicalView.save();

                //Object Class based props first
                CswNbtMetaDataNodeTypeProp CasNoProp = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( ChemicalNodeType.NodeTypeId, CswNbtObjClassMaterial.CasNoPropertyName );
                //CasNoProp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, ChemicalIdentityTab.TabId );

                //Node-type only props

                //Case 25253 is assigned to deal with the fact that with this property, you cannot actually instance a Chemical node
                CswNbtMetaDataNodeTypeProp SynonymsProp = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.Grid, "Synonyms", ChemicalIdentityTab.TabId );
                CswNbtView SynonymGridPropView = _CswNbtSchemaModTrnsctn.restoreView( SynonymsProp.ViewId ); //Create view id on the SynonymGridProp
                SynonymGridPropView.ViewMode = NbtViewRenderingMode.Grid;
                CswNbtViewRelationship ChemRel = SynonymGridPropView.AddViewRelationship( ChemicalNodeType, true );

                CswNbtMetaDataNodeTypeProp SynToMaterialProp = MaterialSynonymNodeType.getNodeTypePropByObjectClassProp( SynonymMaterialObjClassProp.PropName );
                CswNbtViewRelationship SynRel = SynonymGridPropView.AddViewRelationship( ChemRel, NbtViewPropOwnerType.Second, SynToMaterialProp, false ); //"First": relationshiop from parent view relationship to the child; "Second": the child is defining the relationship to the parent
                CswNbtMetaDataNodeTypeProp SynonymnNameProp = MaterialSynonymNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterialSynonym.NamePropertyName );
                CswNbtViewProperty NameViewProp = SynonymGridPropView.AddViewProperty( SynRel, SynonymnNameProp );
                SynonymGridPropView.save();
                SynonymsProp.ViewId = SynonymGridPropView.ViewId;
                SynonymsProp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );

                /* 
                * When this is is working, we'll also need to create the composite property
                */


                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.Quantity, "Expiration Interval", ChemicalIdentityTab.TabId );

                //**********************************************************************
                //Hazards Tab 
                CswNbtMetaDataNodeTypeTab HazardsTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( ChemicalNodeType, "Hazards", 1 );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.NFPA, "NFPA", HazardsTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.MultiList, "PPE", HazardsTab.TabId ).ListOptions = "Goggles,Gloves,Clothing,Fume Hood";
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.Logical, "Hazardous", HazardsTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.ImageList, "Storage Type", HazardsTab.TabId );

                /*For these properties, see case 25291
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.MultiList, "GHS", HazardsTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.MultiList, "EU R&S", HazardsTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.MultiList, "EU Picto", HazardsTab.TabId );
                 */

                //**********************************************************************
                //Physical Tab
                CswNbtMetaDataNodeTypeTab PhysicalTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( ChemicalNodeType, "Physical Tab", 2 );

                //Object Class based props
                CswNbtMetaDataNodeTypeProp SpecificGravityProp = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( ChemicalNodeType.NodeTypeId, CswNbtObjClassMaterial.SpecificGravityPropertyName );
                SpecificGravityProp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, PhysicalTab.TabId );

                CswNbtMetaDataNodeTypeProp PhysicalStateProp = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( ChemicalNodeType.NodeTypeId, CswNbtObjClassMaterial.PhysicalStatePropertyName );
                PhysicalStateProp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, PhysicalTab.TabId );
                PhysicalStateProp.ListOptions = "Solid, Liquid, Gas";

                //Node type only props
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
            if( null == SupplyNodeType || SupplyNodeType.getObjectClass().ObjectClass != MaterialObjectClass.ObjectClass )
            {
                SupplyNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass.ToString(), SupplyNodeTypeName, MaterialsCategory );
                CswNbtMetaDataNodeTypeTab SupplyIdentityTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( SupplyNodeType, "Identity", 0 );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( SupplyNodeType, CswNbtMetaDataFieldType.NbtFieldType.Memo, "Description", SupplyIdentityTab.TabId );

                CswNbtMetaDataNodeTypeTab PictureTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( SupplyNodeType, "Picture", 1 );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( SupplyNodeType, CswNbtMetaDataFieldType.NbtFieldType.ImageList, "Picture", PictureTab.TabId );

                CswNbtView SupplyView = _CswNbtSchemaModTrnsctn.makeView();
                SupplyView.makeNew( "Supplies", NbtViewVisibility.Global, null, null, null );
                CswNbtViewRelationship ChemicalRelationship = SupplyView.AddViewRelationship( SupplyNodeType, false );
                SupplyView.Category = MaterialsCategory;
                SupplyView.save();


            }//if else the Supply nodetype already exists

            #endregion

            #region case 24457-BiologicalNodeType

            string BiologicalNodeTypeName = "Biological";
            CswNbtMetaDataNodeType BiologicalNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( BiologicalNodeTypeName );
            if( null == BiologicalNodeType || BiologicalNodeType.getObjectClass().ObjectClass != MaterialObjectClass.ObjectClass )
            {
                BiologicalNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass.ToString(), BiologicalNodeTypeName, MaterialsCategory );
                CswNbtMetaDataNodeTypeTab BiologicalIdentityTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( BiologicalNodeType, "Identity", 0 );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( BiologicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.List, "Reference Type", BiologicalIdentityTab.TabId ).ListOptions = "ATCC,NIH,CDC";
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( BiologicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.Text, "Reference Number", BiologicalIdentityTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( BiologicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.List, "Type", BiologicalIdentityTab.TabId ).ListOptions = "DNA,RNA,Protein,Organism";
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( BiologicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.Text, "Species Origin", BiologicalIdentityTab.TabId );


                CswNbtMetaDataNodeTypeTab BiosafetyTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( BiologicalNodeType, "Biosafety", 1 );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( BiologicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.List, "Biosafety Level", BiosafetyTab.TabId );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( BiologicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.MultiList, "Vectors", BiosafetyTab.TabId );

                CswNbtView BiologicallView = _CswNbtSchemaModTrnsctn.makeView();
                BiologicallView.makeNew( "Biologicals", NbtViewVisibility.Global, null, null, null );
                CswNbtViewRelationship ChemicalRelationship = BiologicallView.AddViewRelationship( BiologicalNodeType, false );
                BiologicallView.Category = MaterialsCategory;
                BiologicallView.save();


            }//if else the Supply nodetype already exists

            #endregion





        }//Update()

    }//class  CswUpdateSchemaTo01M10

}//namespace ChemSW.Nbt.Schema