using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateDDL_02G_Case30778 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 30778; }
        }

        public override string ScriptName
        {
            get { return "02G_Case30778"; }
        }

        public override string Title
        {
            get { return "Fix ocpa valuepropid"; }
        }

        public override void update()
        {
            // object_class_props_audit.valuepropid is varchar2(20), should be number(12)
            _CswNbtSchemaModTrnsctn.changeColumnDataType( "object_class_props_audit", "valuepropid", CswEnumDataDictionaryPortableDataType.Long, 12 );
            
            // object_class_props_audit.valueproptype is number(15,6), should be varchar2(40)
            _CswNbtSchemaModTrnsctn.changeColumnDataType( "object_class_props_audit", "valueproptype", CswEnumDataDictionaryPortableDataType.String, 40 );

        } // update()

    } // class CswUpdateDDL_02G_Case30778

}//namespace ChemSW.Nbt.Schema