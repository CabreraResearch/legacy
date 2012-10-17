using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27578
    /// </summary>
    public class CswUpdateSchema_01S_Case27578_part2 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 27578; }
        }

        public override void update()
        {

            CswNbtMetaDataNodeType sizeNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Size" );
            if( null != sizeNT )
            {
                CswNbtMetaDataNodeTypeProp containerTypeNTP = sizeNT.getNodeTypeProp( "Container Type" );
                if( null != containerTypeNTP )
                {
                    containerTypeNTP.ListOptions = "Aboveground Tank [A]," +
                                                       "Bag [J]," +
                                                       "Belowground Tank [B]," +
                                                       "Box [K]," +
                                                       "Can [F]," +
                                                       "Carboy [G]," +
                                                       "Cylinder [L]," +
                                                       "Fiberdrum [I]," +
                                                       "Glass Bottle or Jug [M]," +
                                                       "Plastic [N]," +
                                                       "Plastic or Non-Metal Drum [E]," +
                                                       "Steel Drum [D]," +
                                                       "Tank Inside Building [C]," +
                                                       "Tank Wagon [P]," +
                                                       "Tote Bin [O]";
                }
            }

        }//Update()

    }//class CswUpdateSchema_01S_Case27578

}//namespace ChemSW.Nbt.Schema