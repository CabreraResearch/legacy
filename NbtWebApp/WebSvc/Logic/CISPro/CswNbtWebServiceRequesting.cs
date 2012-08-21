using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceRequesting
    {
        private readonly CswNbtResources _CswNbtResources;

        private CswNbtActSubmitRequest _RequestAct;

        private void _initOrderingResources( CswNbtActSystemViews.SystemViewName ViewName, CswPrimaryKey RequestNodeId = null )
        {
            _RequestAct = new CswNbtActSubmitRequest( _CswNbtResources, CreateDefaultRequestNode: true, RequestViewName: ViewName, RequestNodeId: RequestNodeId );
        }

        public CswNbtWebServiceRequesting( CswNbtResources CswNbtResources, CswNbtActSystemViews.SystemViewName ViewName = null, CswPrimaryKey RequestNodeId = null )
        {
            _CswNbtResources = CswNbtResources;
            if( false == _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.CISPro ) )
            {
                throw new CswDniException( ErrorType.Error, "The CISPro module is required to complete this action.", "Attempted to use the Ordering service without the CISPro module." );
            }
            if( ViewName != CswNbtActSystemViews.SystemViewName.CISProRequestCart && ViewName != CswNbtActSystemViews.SystemViewName.CISProRequestHistory )
            {
                ViewName = CswNbtActSystemViews.SystemViewName.CISProRequestCart;
            }
            _initOrderingResources( ViewName, RequestNodeId );

        } //ctor

        public JObject getCurrentRequest( CswNbtActSubmitRequest RequestAct = null )
        {
            RequestAct = RequestAct ?? _RequestAct;
            JObject ret = new JObject();
            CswNbtWebServiceGrid GridWs = new CswNbtWebServiceGrid( _CswNbtResources, RequestAct.CurrentCartView, ForReport: false );
            ret = GridWs.runGrid( IncludeInQuickLaunch: false, GetAllRowsNow: true );
            ret["cartnodeid"] = RequestAct.CurrentRequestNode().NodeId.ToString();
            ret["cartviewid"] = RequestAct.CurrentCartView.SessionViewId.ToString();
            return ret;
        }

        public JObject getCurrentRequestId()
        {
            JObject ret = new JObject();
            CswNbtNode CurrentRequest = _RequestAct.CurrentRequestNode();
            ret["cartnodeid"] = CurrentRequest.NodeId.ToString();
            ret["cartitemnodetypeid"] = _RequestAct.RequestItemNt.NodeTypeId;
            return ret;
        }

        public JObject getRequestHistory()
        {
            return _RequestAct.getRequestHistory();
        }

        public JObject submitRequest( CswPrimaryKey NodeId, string NodeName )
        {
            return _RequestAct.submitRequest( NodeId, NodeName );
        }

        public JObject copyRequest( CswPrimaryKey CopyFromNodeId, CswPrimaryKey CopyToNodeId )
        {
            /* We're need two instances of CswNbtActSubmitRequest. 
             * The current instance was loaded with CopyFromNodeId
             * For the response we need a new instance with the current RequestNodeId, CopyToNodeId */
            CswNbtActSubmitRequest CopyRequest = _RequestAct.copyRequest( CopyFromNodeId, CopyToNodeId );
            return getCurrentRequest( CopyRequest );
        }
        
    } // class CswNbtWebServiceRequesting

} // namespace ChemSW.Nbt.WebServices
