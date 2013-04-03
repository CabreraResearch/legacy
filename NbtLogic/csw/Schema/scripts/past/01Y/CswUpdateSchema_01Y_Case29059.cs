using ChemSW.Nbt.csw.Dev;
using System.Data;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29059
    /// </summary>
    public class CswUpdateSchema_01Y_Case29059 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 29059; }
        }

        public override void update()
        {

            DataTable dt = _CswNbtSchemaModTrnsctn.ViewSelect.getView( "Work Units", NbtViewVisibility.Hidden, null, null );
            foreach( DataRow row in dt.Rows )
            {
                CswNbtView workUnitsView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( row["viewxml"].ToString() );
                workUnitsView.ViewVisibility = NbtViewVisibility.Global.ToString();
                workUnitsView.save();
            }


        } //Update()

    }//class CswUpdateSchema_01Y_Case29059

}//namespace ChemSW.Nbt.Schema