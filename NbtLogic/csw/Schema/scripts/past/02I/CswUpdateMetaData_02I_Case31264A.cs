using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    class CswUpdateMetaData_02I_Case31264A : CswUpdateSchemaTo
    {

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 31264; }
        }

        public override string Title
        {
            get { return "Add new UPC Size OCP"; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass SizeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.SizeClass );
            CswNbtMetaDataObjectClassProp UPCOCP = SizeOC.getObjectClassProp( CswNbtObjClassSize.PropertyName.UPC );
            if( null == UPCOCP )
            {
                _CswNbtSchemaModTrnsctn.createObjectClassProp( SizeOC, new CswNbtWcfMetaDataModel.ObjectClassProp( SizeOC )
                    {
                        PropName = CswNbtObjClassSize.PropertyName.UPC,
                        FieldType = CswEnumNbtFieldType.Text
                    } );
            }

        } // update()
    }

}//namespace ChemSW.Nbt.Schema