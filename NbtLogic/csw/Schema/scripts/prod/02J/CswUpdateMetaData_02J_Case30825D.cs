using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02J_Case30825D : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30825; }
        }

        public override string Title
        {
            get { return "Update RegListListCode ListCode property fieldtype"; }
        }

        public override string AppendToScriptName()
        {
            return "D";
        }

        public override void update()
        {
            // REGULATORY LIST LIST CODE: Change ListCode OCP from Number to Text
            CswNbtMetaDataObjectClass RegListListCodeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListListCodeClass );
            CswNbtMetaDataObjectClassProp ListCodeOCP = RegListListCodeOC.getObjectClassProp( CswNbtObjClassRegulatoryListListCode.PropertyName.ListCode );

            CswNbtMetaDataFieldType TextFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswEnumNbtFieldType.Text );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ListCodeOCP, CswEnumNbtObjectClassPropAttributes.fieldtypeid, TextFT.FieldTypeId );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema