using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02J_Case30825C : CswUpdateSchemaTo
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
            get { return "Update OC property names for Reg. Lists"; }
        }

        public override string AppendToScriptName()
        {
            return "C";
        }

        public override void update()
        {
            // REGULATORY LIST: Rename LOLI List Codes to List Codes
            CswNbtMetaDataObjectClass RegListOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListClass );
            CswNbtMetaDataObjectClassProp ListCodesOCP = RegListOC.getObjectClassProp( "LOLI List Codes" );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ListCodesOCP, CswEnumNbtObjectClassPropAttributes.propname, "List Codes" );

            // REGULATORY LIST LIST CODE: Rename LOLI List Name to List Name and LOLI List Code to List Code
            CswNbtMetaDataObjectClass RegListListCodeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListListCodeClass );
            CswNbtMetaDataObjectClassProp ListNameOCP = RegListListCodeOC.getObjectClassProp( "LOLI List Name" );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ListNameOCP, CswEnumNbtObjectClassPropAttributes.propname, "List Name" );
            CswNbtMetaDataObjectClassProp ListCodeOCP = RegListListCodeOC.getObjectClassProp( "LOLI List Code" );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ListCodeOCP, CswEnumNbtObjectClassPropAttributes.propname, "List Code" );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema