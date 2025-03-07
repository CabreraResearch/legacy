using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for OC changes
    /// </summary>
    public class CswUpdateMetaData_02F_Case30252 : CswUpdateSchemaTo
    {
        public override string Title { get { return "Pre-Script: Case 30252: MetaData Changes"; } }

        public override string ScriptName
        {
            get { return "02F_Case30252_MetaData"; }
        }

        #region Blame Logic

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.DH; }
        }

        public override int CaseNo
        {
            get { return 30252; }
        }

        #endregion Blame Logic

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            if( _CswNbtSchemaModTrnsctn.isColumnDefined( "object_class", "oraviewname" ) != true )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "object_class", "oraviewname", "stable oracle dbview name for this object class", false, false, 30 );
            }
            if( _CswNbtSchemaModTrnsctn.isColumnDefined( "object_class_props", "oraviewcolname" ) != true )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "object_class_props", "oraviewcolname", "stable oracle dbview column name for this object class property", false, false, 30 );
            }
            if( _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetypes", "oraviewname" ) != true )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "nodetypes", "oraviewname", "stable oracle dbview name for this nodetype", false, false, 30 );
            }
            if( _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetype_props", "oraviewcolname" ) != true )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "nodetype_props", "oraviewcolname", "stable oracle dbview column name for this nodetype property", false, false, 30 );
            }

        }

    }//class RunBeforeEveryExecutionOfUpdater_02F_Case30252
}//namespace ChemSW.Nbt.Schema


