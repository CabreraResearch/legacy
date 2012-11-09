using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 25449
    /// </summary>
    public class CswUpdateSchemaCase25449 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // add linkGrid property to chemical nodetype.identity tab using view: root.chemical > component(s)
            CswNbtMetaDataObjectClass compOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialComponentClass );
            CswNbtMetaDataNodeType chemNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
            CswNbtMetaDataNodeType compNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Material Component" );

            if( null != chemNT && null != compOC && null != compNT )
            {
                CswNbtMetaDataNodeTypeTab idTab = chemNT.getNodeTypeTab( "Identity" );
                if( null != idTab )
                {

                    CswNbtMetaDataNodeTypeProp compProp = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( chemNT, CswNbtMetaDataFieldType.NbtFieldType.Grid, "Components", idTab.TabId );

                    CswNbtMetaDataObjectClassProp constitNameOcp = compOC.getObjectClassProp( CswNbtObjClassMaterialComponent.ConstituentPropertyName );
                    CswNbtMetaDataObjectClassProp percentOcp = compOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PercentagePropertyName );
                    CswNbtMetaDataObjectClassProp mixtureOcp = compOC.getObjectClassProp( CswNbtObjClassMaterialComponent.MixturePropertyName );
                    CswNbtMetaDataNodeTypeProp mixtProp = compNT.getNodeTypePropByObjectClassProp( mixtureOcp.PropName );

                    CswNbtView aView = _CswNbtSchemaModTrnsctn.restoreView( compProp.ViewId );
                    if( null == aView )
                    {
                        aView = _CswNbtSchemaModTrnsctn.makeView();
                        compProp.ViewId = aView.ViewId;
                    }
                    compProp.Extended = CswNbtNodePropGrid.GridPropMode.Link.ToString();


                    aView.Root.ChildRelationships.Clear();
                    aView.ViewMode = NbtViewRenderingMode.Grid;
                    aView.Visibility = NbtViewVisibility.Property;

                    CswNbtViewRelationship mixtureRelationship = aView.AddViewRelationship( chemNT, true );
                    CswNbtViewRelationship compRelationship = aView.AddViewRelationship( mixtureRelationship, NbtViewPropOwnerType.Second, mixtProp, true );
                    aView.AddViewProperty( compRelationship, compNT.getNodeTypePropByObjectClassProp( constitNameOcp.PropName ) );
                    aView.AddViewProperty( compRelationship, compNT.getNodeTypePropByObjectClassProp( percentOcp.PropName ) );
                    aView.save();

                }
            }


        }//Update()

    }//class CswUpdateSchemaCase25449

}//namespace ChemSW.Nbt.Schema