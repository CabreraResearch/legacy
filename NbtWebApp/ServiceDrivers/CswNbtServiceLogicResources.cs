using System.Web;
using ChemSW.Config;
using ChemSW.Nbt.Statistics;
// supports ScriptService attribute
// supports ScriptService attribute


namespace ChemSW.Nbt.WebServices
{
    public class CswNbtServiceLogicResources
    {

        public CswNbtResources CswNbtResources = null;
        public CswNbtStatisticsEvents CswNbtStatisticsEvents = null;
        public CswSessionResourcesNbt CswSessionResourcesNbt = null;
        public CswNbtServiceLogicResources( HttpApplicationState HttpApplicationState, HttpRequest HttpRequest, HttpResponse HttpResponse, HttpContext Context, SetupMode SetupMode )
        {
            CswSessionResourcesNbt = new CswSessionResourcesNbt( HttpApplicationState, HttpRequest, HttpResponse, Context, string.Empty, SetupMode.NbtWeb );

            CswNbtResources = CswSessionResourcesNbt.CswNbtResources;
            CswNbtStatisticsEvents = CswSessionResourcesNbt.CswNbtStatisticsEvents;

        }//ctor

        public void setEditMode( string EditModeStr )
        {
            CswNbtResources.EditMode = EditModeStr;
        }



    } // class CswNbtServiceDriverResources

} // namespace ChemSW.Nbt.WebServices
