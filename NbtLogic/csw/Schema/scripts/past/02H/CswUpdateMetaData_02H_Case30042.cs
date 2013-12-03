using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02H_Case30042 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30042; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override string Title
        {
            get { return "New Chemical property for CAF imports"; }
        }

        public override void update()
        {
            // Add AddLabelCodes property to Chemical for CAF imports - hide from UI
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            if( null != ChemicalOC )
            {
                CswNbtMetaDataObjectClassProp AddLabelCodesOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( ChemicalOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        FieldType = CswEnumNbtFieldType.Memo,
                        PropName = CswNbtObjClassChemical.PropertyName.AddLabelCodes
                    } );
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema