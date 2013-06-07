using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26531B
    /// </summary>
    public class CswUpdateSchema_02B_Case26531B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 26531; }
        }

        public override void update()
        {
            //Drop the BlobData column in Jct_Nodes_Props - it will not be used anymore
            if( _CswNbtSchemaModTrnsctn.isColumnDefined( "jct_nodes_props", "blobdata" ) )
            {
                _CswNbtSchemaModTrnsctn.dropColumn( "jct_nodes_props", "blobdata" );
            }
        } // update()

    }//class CswUpdateSchema_02B_Case26531B

}//namespace ChemSW.Nbt.Schema