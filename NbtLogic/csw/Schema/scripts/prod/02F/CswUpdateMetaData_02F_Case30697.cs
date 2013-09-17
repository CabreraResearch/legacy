using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02F_Case30697 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30697; }
        }

        public override string ScriptName
        {
            get { return "02F_Case30697_MetaData"; }
        }

        public override void update()
        {

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetype_props_audit", "hidden" ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "nodetype_props_audit", "hidden", "Is this NodeTypeProp hidden", false, true );
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema