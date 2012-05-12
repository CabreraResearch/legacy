using ChemSW.Core;
// supports ScriptService attribute
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtServiceLogicGetProps : CswNbtServiceLogic
    {

        string _EditMode = string.Empty; string _NodeId = string.Empty; string _SafeNodeKey = string.Empty; string _TabId = string.Empty; string _NodeTypeId = string.Empty; string _Date = string.Empty; string _filterToPropId = string.Empty; string _Multi = string.Empty; string _ConfigMode;



        public CswNbtServiceLogicGetProps( string EditMode, string NodeId, string SafeNodeKey, string TabId, string NodeTypeId, string Date, string filterToPropId, string Multi, string ConfigMode )
        {
            _EditMode = EditMode; _NodeId = NodeId; _SafeNodeKey = SafeNodeKey; _TabId = TabId; _NodeTypeId = NodeTypeId; _Date = Date; _filterToPropId = filterToPropId; _Multi = Multi; _ConfigMode = ConfigMode;
        }//ctor

        public override JObject doLogic( CswNbtServiceLogicResources CswNbtServiceLogicResources )
        {
            JObject ReturnVal = new JObject();


            CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( CswNbtServiceLogicResources.CswNbtResources, CswNbtServiceLogicResources.CswNbtStatisticsEvents, CswConvert.ToBoolean( _Multi ), CswConvert.ToBoolean( _ConfigMode ) );
            CswNbtServiceLogicResources.setEditMode( _EditMode );
            CswDateTime InDate = new CswDateTime( CswNbtServiceLogicResources.CswNbtResources );
            InDate.FromClientDateTimeString( _Date );
            ReturnVal = ws.getProps( _NodeId, _SafeNodeKey, _TabId, CswConvert.ToInt32( _NodeTypeId ), InDate, _filterToPropId );

            return ( ReturnVal );
        }//doLogic



    } // class CswNbtServiceLogicGetProps

} // namespace ChemSW.Nbt.WebServices
