using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.LandingPage;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28523B
    /// </summary>
    public class CswUpdateSchema_01W_Case28523B : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 28523; }
        }

        public override void update()
        {
            // Set icon for Create Material landing page buttons

            CswTableUpdate LandingPageUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "28523B_LandingPage_Update", "landingpage" );
            {
                DataTable ReceiveTable = LandingPageUpdate.getTable( "where displaytext = 'Receive this Material'" );
                foreach( DataRow ReceiveRow in ReceiveTable.Rows )
                {
                    ReceiveRow["buttonicon"] = "bottlebox.png";
                }
                LandingPageUpdate.update( ReceiveTable );
            }
            {
                DataTable ReceiveTable = LandingPageUpdate.getTable( "where displaytext = 'Request this Material'" );
                foreach( DataRow ReceiveRow in ReceiveTable.Rows )
                {
                    ReceiveRow["buttonicon"] = "cartplus.png";
                }
                LandingPageUpdate.update( ReceiveTable );
            }
            
            
        }

        //Update()

    }//class CswUpdateSchema_01V_Case28523B

}//namespace ChemSW.Nbt.Schema