using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26965B
    /// </summary>
    public class CswUpdateSchemaCase26965B : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass SizeOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );
            CswNbtMetaDataObjectClassProp QeOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.QuantityEditablePropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( QeOcp, QeOcp.getFieldTypeRule().SubFields.Default.Name, Tristate.True );

        }//Update()

    }//class CswUpdateSchemaCase26965B

}//namespace ChemSW.Nbt.Schema