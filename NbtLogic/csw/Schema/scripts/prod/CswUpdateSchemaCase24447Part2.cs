using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24447Part2
    /// </summary>
    public class CswUpdateSchemaCase24447Part2 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass SizeOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );

            CswNbtMetaDataObjectClassProp MaterialOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.MaterialPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( MaterialOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( MaterialOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );

            CswNbtMetaDataObjectClassProp CapacityOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.CapacityPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( CapacityOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( CapacityOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );

            CswNbtMetaDataObjectClassProp QuantityEditableOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.QuantityEditablePropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( QuantityEditableOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );

            CswNbtMetaDataObjectClassProp DispensableOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.DispensablePropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( DispensableOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );

            CswNbtMetaDataNodeType SizeNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Size" );
            if( null != SizeNt )
            {
                CswNbtMetaDataNodeTypeProp MaterialNtp = SizeNt.getNodeTypePropByObjectClassProp( MaterialOcp.PropName );
                SizeNt.setNameTemplateText( MaterialNtp.PropName );

                CswNbtMetaDataNodeTypeProp CapacityNtp = SizeNt.getNodeTypePropByObjectClassProp( CapacityOcp.PropName );
                SizeNt.addNameTemplateText( CapacityNtp.PropName );
            }


        }//Update()

    }//class CswUpdateSchemaCase24447Part2

}//namespace ChemSW.Nbt.Schema