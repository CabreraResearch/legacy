using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28890
    /// </summary>
    public class CswUpdateSchema_02A_Case28890 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 28890; }
        }

        public override void update()
        {
            CswNbtView LocationsView = _CswNbtSchemaModTrnsctn.restoreView( "Locations", NbtViewVisibility.Global );
            if( null != LocationsView && LocationsView.Category == "System" )
            {
                LocationsView.IsSystem = true;
                LocationsView.save();
            }
        } // update()

    }//class CswUpdateSchema_02A_Case28890
}//namespace ChemSW.Nbt.Schema