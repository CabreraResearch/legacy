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

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "object_class_audit", "oraviewname" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "object_class_audit", "oraviewname", "stable oracle dbview name for this object class", false, false, 30 );
            }

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "object_class_props_audit", "oraviewcolname" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "object_class_props_audit", "oraviewcolname", "stable oracle dbview column name for this object class property", false, false, 30 );
            }

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetypes_audit", "oraviewname" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "nodetypes_audit", "oraviewname", "stable oracle dbview name for this nodetype", false, false, 30 );
            }

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetype_props_audit", "oraviewcolname" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "nodetype_props_audit", "oraviewcolname", "stable oracle dbview column name for this nodetype property", false, false, 30 );
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema