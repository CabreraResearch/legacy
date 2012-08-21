using System;
using System.Linq;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27549
    /// </summary>
    public class CswUpdateSchemaCase27549 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {

            CswNbtMetaDataNodeType materialComponentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Material Component" );
            if( null != materialComponentNT )
            {
                CswNbtMetaDataObjectClass materialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
                CswNbtMetaDataObjectClassProp tradeNameOCP = materialOC.getObjectClassProp( CswNbtObjClassMaterial.TradenamePropertyName );

                CswNbtMetaDataObjectClass materialComponentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialComponentClass );
                CswNbtMetaDataObjectClassProp constituentOCP = materialComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.ConstituentPropertyName );

                CswNbtMetaDataNodeTypeTab materialCompTab = _getTab( materialComponentNT, "Material Component", 1 );

                CswNbtMetaDataNodeTypeProp matCompConstituentNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp(
                    materialComponentNT,
                    CswNbtMetaDataFieldType.NbtFieldType.PropertyReference,
                    "Constituent Tradename",
                    materialCompTab.TabId );
                matCompConstituentNTP.SetFK( NbtViewPropType.ObjectClassPropId.ToString(), constituentOCP.PropId, NbtViewPropType.ObjectClassPropId.ToString(), tradeNameOCP.PropId );

                matCompConstituentNTP.removeFromAllLayouts();

                string templateText = CswNbtMetaData.MakeTemplateEntry( matCompConstituentNTP.PropName );
                materialComponentNT.setNameTemplateText( templateText );

            }


        }//Update()

        private CswNbtMetaDataNodeTypeTab _getTab( CswNbtMetaDataNodeType nodetype, string tabName, Int32 order )
        {
            CswNbtMetaDataNodeTypeTab tab = nodetype.getNodeTypeTab( tabName );
            if( null == tab )
            {
                tab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( nodetype, tabName, order );
            }
            else
            {
                tab.TabOrder = order;
            }
            return tab;
        } // _getTab()

    }

}//namespace ChemSW.Nbt.Schema