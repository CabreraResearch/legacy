using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27178
    /// </summary>
    public class CswUpdateSchemaCase27178 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataNodeType WeightUnitNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Weight Unit" );
            WeightUnitNodeType.NodeTypeName = "Unit (Weight)";

            CswNbtMetaDataNodeType VolumeUnitNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Volume Unit" );
            VolumeUnitNodeType.NodeTypeName = "Unit (Volume)";

            CswNbtMetaDataNodeType EachUnitNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Each Unit" );
            EachUnitNodeType.NodeTypeName = "Unit (Each)";

            CswNbtMetaDataNodeType TimeUnitNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Time Unit" );
            TimeUnitNodeType.NodeTypeName = "Unit (Time)";
        }//Update()

    }//class CswUpdateSchemaCase27178

}//namespace ChemSW.Nbt.Schema