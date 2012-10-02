using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27779_part2
    /// </summary>
    public class CswUpdateSchema_01S_Case27779_part2 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {

            //delete the length column
            if( _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetype_props", "length" ) )
            {
                _CswNbtSchemaModTrnsctn.dropColumn( "nodetype_props", "length" );
            }

        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        //Update()

    }

}//namespace ChemSW.Nbt.Schema