using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27801
    /// </summary>
    public class CswUpdateSchema_01T_Case27801 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass materialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClassProp specificGravityOCP = materialOC.getObjectClassProp( CswNbtObjClassMaterial.PropertyName.SpecificGravity );
            CswNbtMetaDataFieldType numberFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Number );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( specificGravityOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fieldtypeid, numberFT.FieldTypeId );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( specificGravityOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.numberminvalue, 0 );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( specificGravityOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.numberprecision, 3 );
        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 27801; }
        }

        //Update()

    }//class CswUpdateSchemaCase27801

}//namespace ChemSW.Nbt.Schema