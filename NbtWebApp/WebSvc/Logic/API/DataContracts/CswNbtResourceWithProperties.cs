using System.Runtime.Serialization;
using ChemSW.Core;
using Newtonsoft.Json.Linq;

namespace NbtWebApp.WebSvc.Logic.API.DataContracts
{
    [DataContract]
    public class CswNbtResourceWithProperties: CswNbtResource
    {
        public JObject PropertyData = new JObject();

        [DataMember( Name = "propdata" )]
        private string _propertyDataStr
        {
            get { return PropertyData.ToString(); }
            set { PropertyData = CswConvert.ToJObject( value ); }
        }
    }
}