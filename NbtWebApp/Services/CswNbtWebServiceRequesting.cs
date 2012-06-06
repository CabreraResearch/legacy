using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceRequesting
    {
        private readonly CswNbtResources _CswNbtResources;

        private CswNbtActSubmitRequest _RequestAct;

        private void _initOrderingResources( CswNbtActSystemViews.SystemViewName ViewName )
        {
            _RequestAct = new CswNbtActSubmitRequest( _CswNbtResources, ViewName );
        }

        public CswNbtWebServiceRequesting( CswNbtResources CswNbtResources, CswNbtActSystemViews.SystemViewName ViewName = null )
        {
            _CswNbtResources = CswNbtResources;
            if( false == _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.CISPro ) )
            {
                throw new CswDniException( ErrorType.Error, "The CISPro module is required to complete this action.", "Attempted to use the Ordering service without the CISPro module." );
            }
            if( null == ViewName || ( ViewName != CswNbtActSystemViews.SystemViewName.CISProRequestCart && ViewName != CswNbtActSystemViews.SystemViewName.CISProRequestHistory ) )
            {
                ViewName = CswNbtActSystemViews.SystemViewName.CISProRequestCart;
            }
            _initOrderingResources( ViewName );

        } //ctor

        public JObject getCurrentRequest()
        {
            JObject ret = new JObject();
            CswNbtWebServiceGrid GridWs = new CswNbtWebServiceGrid( _CswNbtResources, _RequestAct.CurrentCartView, ForReport: false );
            ret = GridWs.runGrid( IncludeInQuickLaunch: false, GetAllRowsNow: true );
            ret["cartnodeid"] = _RequestAct.CurrentRequestNode.NodeId.ToString();
            ret["cartviewid"] = _RequestAct.CurrentCartView.SessionViewId.get();
            return ret;
        }

        public JObject getRequestHistory()
        {
            return _RequestAct.getRequestHistory();
        }

        public JObject submitRequest( CswPrimaryKey NodeId )
        {
            return _RequestAct.submitRequest( NodeId );
        }

    } // class CswNbtWebServiceRequesting

} // namespace ChemSW.Nbt.WebServices
