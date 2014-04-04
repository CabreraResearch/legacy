using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;

namespace NbtWebApp.WebSvc.Logic.API.DataContracts
{
    [DataContract(Name = "Request")]
    public class CswNbtAPIGenericRequest
    {
        [DataMember( Name = "metadataname" )]
        public string MetaDataName { get; set; }


        public CswPrimaryKey NodeId = new CswPrimaryKey();
        [DataMember( Name = "nodeid" )]
        private int _nodeIdStr
        {
            get { return NodeId.PrimaryKey; }
            set { NodeId = new CswPrimaryKey( "nodes", value ); }
        }

        public JObject PropData = new JObject();
        [DataMember( Name = "propdata" )]
        private string _propData
        {
            get { return PropData.ToString(); }
            set { PropData = CswConvert.ToJObject( value ); }
        }

        //Not a data member
        public Collection<CswNbtWcfProperty> Properties = new Collection<CswNbtWcfProperty>();

        //Not a data member
        public NameValueCollection PropertyFilters = new NameValueCollection();

        public CswNbtAPIGenericRequest( string MetaDataNameIn, string Id )
        {
            NodeId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( Id ) );
            MetaDataName = MetaDataNameIn;
        }
    }
}