using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28053
    /// </summary>
    public class CswUpdateSchema_01W_Case28053 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 28053; }
        }

        public override void update()
        {
            CswNbtMetaDataNodeType MaterialDocumentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Material Document" );
            if( null != MaterialDocumentNT )
            {
                CswNbtMetaDataNodeTypeProp OwnerNTP = MaterialDocumentNT.getNodeTypeProp( "Owner" );
                if( null != OwnerNTP )
                {
                    // Change prop name
                    OwnerNTP.PropName = "Tradename";

                    // Change view name
                    CswNbtView View = _CswNbtSchemaModTrnsctn.restoreView( OwnerNTP.ViewId );
                    View.ViewName = "Tradename";
                    View.save();
                }
            }

        } //Update()

    }//class CswUpdateSchema_01V_Case28053

}//namespace ChemSW.Nbt.Schema