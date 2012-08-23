using ChemSW.Nbt.MetaData;
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27431
    /// </summary>
    public class CswUpdateSchemaCase27431 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataNodeType ChemicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
            if( ChemicalNT != null )
            {
                CswNbtMetaDataNodeTypeProp PPENTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( ChemicalNT.NodeTypeId, "PPE" );
                if( PPENTP != null )
                {
                    PPENTP.Extended = ",";
                }
            }
        }//Update()

    }//class CswUpdateSchemaCase27431

}//namespace ChemSW.Nbt.Schema