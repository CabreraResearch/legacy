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
            DataTable ObjectClasstable = ObjectClassUpdate.getTable( " where objectclass='ComponentClass' " );
            foreach( DataRow r in ObjectClasstable.Rows )
            {
                r["objectclass"] = CswNbtMetaDataObjectClass.NbtObjectClass.MaterialComponentClass.ToString();
            }
            ObjectClassUpdate.update( ObjectClasstable );
            //percentage prop already exists            
            //mixture prop is relationship to material (parent)
            CswNbtMetaDataObjectClassProp mixtureOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialComponentClass, CswNbtObjClassMaterialComponent.MixturePropertyName, CswNbtMetaDataFieldType.NbtFieldType.Relationship, false, false, true, NbtViewRelatedIdType.ObjectClassId.ToString() );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( mixtureOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.iscompoundunique, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( mixtureOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fktype, NbtViewRelatedIdType.ObjectClassId.ToString() );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( mixtureOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fkvalue, _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass ) );

            //constituent prop is relationship to material (this component name)
            CswNbtMetaDataObjectClassProp constOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialComponentClass, CswNbtObjClassMaterialComponent.ConstituentPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Relationship, false, false, true, NbtViewRelatedIdType.ObjectClassId.ToString() );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( constOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.iscompoundunique, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( constOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fktype, NbtViewRelatedIdType.ObjectClassId.ToString() );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( constOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fkvalue, _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass ) );


            //case 25449
            string MaterialsCategory = "Materials";
            CswNbtMetaDataNodeType MaterialComponentNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType("Material Component");

            CswNbtMetaDataObjectClass MaterialObjectClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            string ChemicalNodeTypeName = "Chemical";
            CswNbtMetaDataNodeType ChemicalNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( ChemicalNodeTypeName );
            if( null == ChemicalNodeType )
            {
               
                //Identity Tab
                ChemicalNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass.ToString(), ChemicalNodeTypeName, MaterialsCategory );
                CswNbtMetaDataNodeTypeTab ChemicalIdentityTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( ChemicalNodeType, "Identity", 0 );

                //grid components
                 //list of components for currently selected material
                   //component name & percentage
                //grid view is: (root level relationship must match owner of grid property. only true of grid views for property grids)
                  // chemical (relationship) --parent "first"
                    // components (by mixture relationship)  --child "second"
                        // constituent (property)
                        // percentage (property)
                        
                // dch CREATE GRID VIEW PROPERTY
                CswNbtMetaDataNodeTypeProp CompToConstituentProp = MaterialComponentNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterialComponent.ConstituentPropertyName );
                CswNbtMetaDataNodeTypeProp percentageProp = MaterialComponentNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterialComponent.PercentagePropertyName );
                CswNbtMetaDataNodeTypeProp CompToMixtureProp = MaterialComponentNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterialComponent.MixturePropertyName );
                
                CswNbtMetaDataNodeTypeProp ComponentsProp = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( ChemicalNodeType, CswNbtMetaDataFieldType.NbtFieldType.Grid, "Components", ChemicalIdentityTab.TabId );
                ComponentsProp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );

                CswNbtView ComponentGridPropView = _CswNbtSchemaModTrnsctn.restoreView( ComponentsProp.ViewId ); //Create view id on the ComponentGridProp
                ComponentGridPropView.ViewMode = NbtViewRenderingMode.Grid;
                CswNbtViewRelationship ChemRel = ComponentGridPropView.AddViewRelationship( ChemicalNodeType, true );

                CswNbtViewRelationship CompRel = ComponentGridPropView.AddViewRelationship( ChemRel, NbtViewPropOwnerType.Second, CompToMixtureProp, false ); //"First": relationshiop from parent view relationship to the child; "Second": the child is defining the relationship to the parent
                
                CswNbtViewProperty constituentViewProp = ComponentGridPropView.AddViewProperty( CompRel, CompToConstituentProp );
                CswNbtViewProperty percentageViewProp = ComponentGridPropView.AddViewProperty( CompRel, percentageProp );

                ComponentGridPropView.save();
 
            }
        
        }//Update()

    }//class CswUpdateSchemaCase25253

}//namespace ChemSW.Nbt.Schema