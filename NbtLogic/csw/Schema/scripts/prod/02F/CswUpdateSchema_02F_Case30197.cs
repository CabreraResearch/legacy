using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30197 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 30197; }
        }

        public override void update()
        {
            //if( _CswNbtSchemaModTrnsctn.isMaster() )
            //{
                foreach( CswNbtView View in _CswNbtSchemaModTrnsctn.restoreViews( "Inventory Levels" ) )
                {
                    if( View.ViewMode == CswEnumNbtViewRenderingMode.Grid && 
                        View.Visibility == CswEnumNbtViewVisibility.Property )
                    {
                        View.Root.eachRelationship( Relationship =>
                        {
                            Relationship.AllowAdd = true;
                        }, null );
                        View.save();
                    }
                }
            //}
        } // update()

    }

}//namespace ChemSW.Nbt.Schema