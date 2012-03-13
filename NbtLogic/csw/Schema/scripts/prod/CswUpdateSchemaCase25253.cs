using System;
using System.Data;
using ChemSW.DB;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 25253
    /// </summary>
    public class CswUpdateSchemaCase25253 : CswUpdateSchemaTo
    {
        public override void update()
        {

            //case 24459
            //rename ComponentClass to MaterialComponentClass if exists
            CswTableUpdate ObjectClassUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "updatecomponentobjectclass", "object_class" );
            DataTable ObjectClasstable = ObjectClassUpdate.getTable( " objectclass='ComponentClass' " );
            foreach( DataRow r in ObjectClasstable.Rows )
            {
                r["objectclass"] = CswNbtMetaDataObjectClass.NbtObjectClass.MaterialComponentClass.ToString();
            }
            ObjectClassUpdate.update( ObjectClasstable );
            //percentage prop already exists            
            //mixture prop is relationship to material (parent)
            CswNbtMetaDataObjectClassProp mixtureOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialComponentClass, "Mixture", CswNbtMetaDataFieldType.NbtFieldType.Relationship, false, false, true, NbtViewRelatedIdType.ObjectClassId.ToString() );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( mixtureOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.iscompoundunique, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( mixtureOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fktype, NbtViewRelatedIdType.ObjectClassId.ToString() );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( mixtureOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fkvalue, _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass ) );

            //constituent prop is relationship to material (this component name)
            CswNbtMetaDataObjectClassProp constOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialComponentClass, "Constituent", CswNbtMetaDataFieldType.NbtFieldType.Relationship, false, false, true, NbtViewRelatedIdType.ObjectClassId.ToString() );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( constOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.iscompoundunique, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( constOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fktype, NbtViewRelatedIdType.ObjectClassId.ToString() );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( constOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fkvalue, _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass ) );

/*
            //case 25449
            string MaterialsCategory = "Materials";
            CswNbtMetaDataNodeType MaterialComponentNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType("Material Component");
            CswNbtMetaDataObjectClassProp MaterialComponentNameObjClassProp = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialComponentClass, CswNbtObjClassMaterialComponent.NamePropertyName, CswNbtMetaDataFieldType.NbtFieldType.Text );

            CswNbtMetaDataObjectClass MaterialObjectClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            string ChemicalNodeTypeName = "Chemical";
            CswNbtMetaDataNodeType ChemicalNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( ChemicalNodeTypeName );
            if( null == ChemicalNodeType )
            {
               
                //Identity Tab
                ChemicalNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass.ToString(), ChemicalNodeTypeName, MaterialsCategory );
                CswNbtMetaDataNodeTypeTab ChemicalIdentityTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( ChemicalNodeType, "Identity", 0 );

                //Case 25253 is assigned to deal with the fact that with this property, you cannot actually instance a Chemical node
                CswNbtMetaDataNodeTypeProp ComponentsProp = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.Grid, "Components", ChemicalIdentityTab.TabId );
                CswNbtView ComponentGridPropView = _CswNbtSchemaModTrnsctn.restoreView( ComponentsProp.ViewId ); //Create view id on the ComponentGridProp
                ComponentGridPropView.ViewMode = NbtViewRenderingMode.Grid;
                CswNbtViewRelationship ChemRel = ComponentGridPropView.AddViewRelationship( ChemicalNodeType, true );

                CswNbtMetaDataNodeTypeProp SynToMaterialProp = MaterialComponentNodeType.getNodeTypePropByObjectClassProp( MaterialComponentMObjClassProp.PropName );
                CswNbtViewRelationship SynRel = ComponentGridPropView.AddViewRelationship( ChemRel, NbtViewPropOwnerType.Second, SynToMaterialProp, false ); //"First": relationshiop from parent view relationship to the child; "Second": the child is defining the relationship to the parent
                CswNbtMetaDataNodeTypeProp ComponentnNameProp = MaterialComponentNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterialComponent.NamePropertyName );
                CswNbtViewProperty NameViewProp = ComponentGridPropView.AddViewProperty( SynRel, ComponentnNameProp );
                ComponentGridPropView.save();
                ComponentsProp.ViewId = ComponentGridPropView.ViewId;
                ComponentsProp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
 
            }
*/        
        }//Update()

    }//class CswUpdateSchemaCase25253

}//namespace ChemSW.Nbt.Schema