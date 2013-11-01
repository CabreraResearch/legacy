using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case28832: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 28832; }
        }

        public override string ScriptName
        {
            get { return "02G_Case28832"; }
        }

        public override string Title
        {
            get { return "Chemical Formulas Use Formula Fieldtype"; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.MetaData.makeNewFieldType( CswEnumNbtFieldType.Formula, CswEnumNbtFieldTypeDataType.TEXT );

            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtMetaDataObjectClassProp FormulaOCP = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.Formula );

            CswNbtMetaDataFieldType FormulaFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswEnumNbtFieldType.Formula );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FormulaOCP, CswEnumNbtObjectClassPropAttributes.fieldtypeid, FormulaFT.FieldTypeId );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema