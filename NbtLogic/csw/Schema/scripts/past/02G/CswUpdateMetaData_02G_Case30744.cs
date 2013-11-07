using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02G_Case30744: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 30744; }
        }

        public override string ScriptName
        {
            get { return "02G_30744"; }
        }

        public override string Title
        {
            get { return "Add Size Props Needed by CAF Import"; }
        }

        public override void update()
        {

            CswNbtMetaDataObjectClass SizeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.SizeClass );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( SizeOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassSize.PropertyName.Description,
                FieldType = CswEnumNbtFieldType.Text,
                IsFk = false,
                ServerManaged = false,
                ReadOnly = false,
                IsUnique = false,
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( SizeOC, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassSize.PropertyName.Barcode,
                FieldType = CswEnumNbtFieldType.Barcode,
                IsFk = false,
                ServerManaged = false,
                ReadOnly = false,
                IsUnique = false,
            } );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema

