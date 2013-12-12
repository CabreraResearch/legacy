using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateDDL_02I_Case31056 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31056; }
        }

        public override string Title
        {
            get { return "nodetypes.mergeable"; }
        }

        public override void update()
        {
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetypes", "mergeable" ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "nodetypes", "mergeable", "whether nodes of this nodetype may be merged", false );
            }
        } // update()
    }
}//namespace ChemSW.Nbt.Schema