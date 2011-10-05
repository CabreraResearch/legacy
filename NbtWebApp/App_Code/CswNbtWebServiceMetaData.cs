using ChemSW.Nbt.MetaData;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceMetaData
    {
        private readonly CswNbtResources _CswNbtResources;

        public CswNbtWebServiceMetaData( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        } //ctor

        public JObject getNodeTypes()
        {
            JObject ReturnVal = new JObject();
            foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.LatestVersionNodeTypes )
            {

                ReturnVal.Add( new JProperty( "nodetype_" + NodeType.NodeTypeId,
                                              new JObject(
                                                  new JProperty( "id", NodeType.NodeTypeId ),
                                                  new JProperty( "name", NodeType.NodeTypeName ),
                                                  new JProperty( "objectclass", NodeType.ObjectClass.ObjectClass.ToString() ) ) ) );
            } // foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.LatestVersionNodeTypes )

            return ReturnVal;
        } // getNodeTypes()

    } // class CswNbtWebServiceMetaData

} // namespace ChemSW.Nbt.WebServices
