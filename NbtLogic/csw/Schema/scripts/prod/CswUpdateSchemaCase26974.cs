using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using System.Collections.Generic;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26974
    /// </summary>
    public class CswUpdateSchemaCase26974 : CswUpdateSchemaTo
    {
        public override void update()
        {

            //add supplier and catalog number to size name template
            CswNbtMetaDataNodeType sizeNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Size" );
            if( null != sizeNT )
            {
                CswNbtMetaDataNodeTypeProp materialNTP = sizeNT.getNodeTypeProp( CswNbtObjClassSize.MaterialPropertyName );
                CswNbtMetaDataObjectClass materialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( materialNTP.FKValue );
                CswNbtMetaDataObjectClassProp supplierOCP = materialOC.getObjectClassProp( CswNbtObjClassMaterial.SupplierPropertyName );

                CswNbtMetaDataObjectClass sizeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );
                CswNbtMetaDataObjectClassProp materialOCP = sizeOC.getObjectClassProp( CswNbtObjClassSize.MaterialPropertyName );

                CswNbtMetaDataNodeTypeTab sizeTab = _getTab( sizeNT, "Size", 1 );

                CswNbtMetaDataNodeTypeProp sizeSupplierNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( sizeNT, CswNbtMetaDataFieldType.NbtFieldType.PropertyReference, "Supplier", sizeTab.TabId );
                sizeSupplierNTP.SetFK( NbtViewPropIdType.ObjectClassPropId.ToString(), materialOCP.PropId, NbtViewPropIdType.ObjectClassPropId.ToString(), supplierOCP.PropId );

                string templateText = sizeNT.NameTemplateValue;
                templateText += CswNbtMetaData.MakeTemplateEntry( sizeSupplierNTP.PropName ); //add supplier (NTP) name to template
                templateText += " " + CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassSize.CatalogNoPropertyName ); //add catalog number to name template
                sizeNT.setNameTemplateText( templateText );

            }

        }//Update()

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

    }//class CswUpdateSchemaCase26974

}//namespace ChemSW.Nbt.Schema