using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02G_Case30813 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30813; }
        }

        public override string ScriptName
        {
            get { return "02G_Case" + CaseNo; }
        }

        public override string Title
        {
            get { return "Add LegacyID prop to all OCs"; }
        }

        public override void update()
        {
            // Add the LegacyID property to all object classes
            foreach( CswNbtMetaDataObjectClass ObjectClass in _CswNbtSchemaModTrnsctn.MetaData.getObjectClasses() )
            {
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ObjectClass )
                    {
                        PropName = CswNbtObjClass.PropertyName.LegacyId,
                        FieldType = CswEnumNbtFieldType.Text
                    } );
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema