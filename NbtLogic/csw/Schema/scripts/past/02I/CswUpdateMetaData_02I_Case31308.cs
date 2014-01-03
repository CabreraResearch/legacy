using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02I_Case31308 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31308; }
        }

        public override string Title
        {
            get { return "Add SqlScript to Print Labels"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass PrintLabelOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.PrintLabelClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PrintLabelOC )
                {
                    PropName = CswNbtObjClassPrintLabel.PropertyName.SqlScript,
                    FieldType = CswEnumNbtFieldType.Memo
                } );
        } // update()
    }

}//namespace ChemSW.Nbt.Schema