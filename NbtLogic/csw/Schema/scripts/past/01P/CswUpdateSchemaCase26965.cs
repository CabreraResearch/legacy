using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26965
    /// </summary>
    public class CswUpdateSchemaCase26965 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass SizeOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass(CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass);
            CswNbtMetaDataObjectClassProp QeOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.QuantityEditablePropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp(QeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true);
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue(QeOcp, QeOcp.getFieldTypeRule().SubFields.Default.Name, Tristate.False );

            CswNbtMetaDataObjectClassProp DspOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.DispensablePropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( DspOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( DspOcp, DspOcp.getFieldTypeRule().SubFields.Default.Name, Tristate.True );

        }//Update()

    }//class CswUpdateSchemaCase26965

}//namespace ChemSW.Nbt.Schema