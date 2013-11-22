using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02I_Case31090A: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 31090; }
        }

        public override string Title
        {
            get { return "Add Code prop to Reg Lists"; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {

            CswNbtMetaDataObjectClass RegListOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListClass );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( RegListOC, new CswNbtWcfMetaDataModel.ObjectClassProp( RegListOC )
                {
                    PropName = CswNbtObjClassRegulatoryList.PropertyName.ListCode,
                    FieldType = CswEnumNbtFieldType.Text
                } );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema