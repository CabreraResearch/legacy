
using ChemSW.Nbt.MetaData;
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27274
    /// </summary>
    public class CswUpdateSchemaCase27274 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataNodeType ContainerNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Container" );
            if( ContainerNT != null )
            {
                CswNbtMetaDataNodeTypeProp DisposeNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( ContainerNT.NodeTypeId, "Dispose" );
                if( DisposeNTP != null )
                {
                    DisposeNTP.ValueOptions = "Are you sure you want to dispose this container?";
                }
            }
        }//Update()

    }//class CswUpdateSchemaCase27274

}//namespace ChemSW.Nbt.Schema