using System.Runtime.Serialization;
using ChemSW.Grid.ExtJs;
using NbtWebApp.WebSvc.Returns;

namespace ChemSW.Nbt.WebServices
{
    /// <summary>
    /// Label List Return Object
    /// </summary>
    [DataContract]
    public class GridContainer
    {
        [DataMember]
        public CswGridExtJsGrid grid = null;

    }

    [DataContract]
    public class CswNbtScheduledRulesReturn : CswWebSvcReturn
    {
        /// <summary> ctor </summary>
        public CswNbtScheduledRulesReturn()
        {
        }//ctor

        [DataMember]
        public GridContainer Data = new GridContainer();
        /// <summary> data </summary>

    }//CswNbtScheduledRulesReturn




} // namespace ChemSW.Nbt.WebServices
