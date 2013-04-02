using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28711
    /// </summary>
    public class CswUpdateSchema_01Y_Case28711 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28711; }
        }

        public override void update()
        {
            CswNbtMetaDataNodeType ChemicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
            if( null != ChemicalNT )
            {
                CswNbtMetaDataNodeTypeProp ChemicalSpecialFlagsNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp(ChemicalNT.NodeTypeId, "Special Flags");
                if (null != ChemicalSpecialFlagsNTP)
                {
                    ChemicalSpecialFlagsNTP.DefaultValue.AsMultiList.Value = new CswCommaDelimitedString();
                }
            }
        } //Update()
    }//class CswUpdateSchema_01Y_Case28711
}//namespace ChemSW.Nbt.Schema