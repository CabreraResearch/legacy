using System;
using System.Data;
using System.Linq;
using System.Web.SessionState;
using System.Web.UI.WebControls;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.PropTypes;
using System.Collections.Generic;
using System.Xml;

namespace ChemSW.Nbt.WebServices
{
    /// <summary>
    /// Webservice for the list of Quick Launch links
    /// </summary>
    public class CswNbtWebServiceQuickLaunchItems : CompositeControl
    {
        private CswNbtResources _CswNbtResources;
        private const string ActionName = "actionname";
        private const string ActionPk = "actionid";
        private const string ActionSelected = "Include";
        private const string QuickLaunchViews = "QuickLaunchViews";

        public CswNbtWebServiceQuickLaunchItems( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;

        }

        /// <summary>
        /// Types of Quick Launch Links
        /// </summary>
        public enum QuickLaunchType
        {
            /// <summary>
            /// Link to a View
            /// </summary>
            View,
            /// <summary>
            /// Link to an Action
            /// </summary>
            Action,
            /// <summary>
            /// Undefined
            /// </summary>
            Unknown
        }

        public string getQuickLaunchItems( CswPrimaryKey UserId, HttpSessionState Session )
        {
            string ret = string.Empty;
            var ReturnXML = new XmlDocument();
            bool isNewSession = ( null == Session[QuickLaunchViews] );
            XmlNode QuickLaunchNode = CswXmlDocument.SetDocumentElement( ReturnXML, "quicklaunch" );

            // Add Recent Views from Session First
            Stack<KeyValuePair<Int32, string>> QuickLaunchHistory = null;
            if( !isNewSession )
            {
                QuickLaunchHistory = (Stack<KeyValuePair<Int32, string>>) Session[QuickLaunchViews];
            }
            else
            {
                QuickLaunchHistory = new Stack<KeyValuePair<Int32, string>>();
            }

            CswNbtNode UserNode = _CswNbtResources.Nodes.GetNode( UserId );
            if( null != UserNode && isNewSession )
            {
                // Add Stored Views Next
                CswNbtObjClassUser UserOC = CswNbtNodeCaster.AsUser( UserNode );
                CswCommaDelimitedString UserQuickLaunchViews = UserOC.QuickLaunchViews.SelectedViewIds;
                foreach( CswNbtView QuickLaunchView in UserQuickLaunchViews.Where( View => !String.IsNullOrEmpty( View ) )
                    .Select( CswConvert.ToInt32 )
                    .Where( ViewId => Int32.MinValue != ViewId )
                    .Select( ViewId => CswNbtViewFactory.restoreView( _CswNbtResources, ViewId ) )
                    .Where( QuickLaunchView => null != QuickLaunchView && QuickLaunchView.IsFullyEnabled() ) )
                {
                    var ThisView = new KeyValuePair<int, string>( QuickLaunchView.ViewId, QuickLaunchView.ViewName );
                    if( !QuickLaunchHistory.Contains( ThisView ) )
                    {
                        QuickLaunchHistory.Push( ThisView );
                    }
                } // foreach( CswNbtView QuickLaunchView...
            } // if( null != UserNode )

            //This ensures that the user's Quick Launch views stay at bottom of the stack
            Session[QuickLaunchViews] = QuickLaunchHistory;

            foreach( KeyValuePair<Int32, string> pair in QuickLaunchHistory )
            {
                XmlNode ThisItem = CswXmlDocument.AppendXmlNode( ReturnXML, "item" );
                CswXmlDocument.AppendXmlAttribute( ThisItem, "type", QuickLaunchType.View.ToString() );
                CswXmlDocument.AppendXmlAttribute( ThisItem, "viewid", pair.Key.ToString() );
                CswXmlDocument.AppendXmlAttribute( ThisItem, "text", pair.Value );

            } // foreach( Int32 ViewId in QuickLaunchDict.Keys )

            if( isNewSession )  // Add Stored Actions Last
            {
                CswNbtNodePropLogicalSet ActionsLogicalSet = ( (CswNbtObjClassUser) _CswNbtResources.CurrentNbtUser.UserNode ).QuickLaunchActions;
                DataTable ActionsTable = ActionsLogicalSet.GetDataAsTable( ActionName, ActionPk );
                foreach( CswNbtAction ThisAction in from DataRow ActionRow in ActionsTable.Rows
                                                    where CswConvert.ToBoolean( ActionRow[ActionSelected] )
                                                    select _CswNbtResources.Actions[CswNbtAction.ActionNameStringToEnum( ActionRow[ActionName].ToString() )]
                                                    into ThisAction
                                                    where null != ThisAction
                                                    select ThisAction )
                {
                    XmlNode ThisItem = CswXmlDocument.AppendXmlNode( ReturnXML, "item" );
                    CswXmlDocument.AppendXmlAttribute( ThisItem, "type", QuickLaunchType.Action.ToString() );
                    CswXmlDocument.AppendXmlAttribute( ThisItem, "viewid", ThisAction.ActionId.ToString() );
                    CswXmlDocument.AppendXmlAttribute( ThisItem, "text", ThisAction.Name.ToString() );
                    CswXmlDocument.AppendXmlAttribute( ThisItem, "url", ThisAction.Url );
                } // foreach( CswNbtAction ThisAction...
            } // if( isNewSession )

            ret = ReturnXML.ToString();

            return ret;

        } // getQuickLaunchItems()


        public void resetQuickLaunchItems( string UserId )
        {
            
        } // ResetQuickLaunchItems()

        /// <summary>
        /// Adds a quick launch item to the Quick Launch bar
        /// </summary>
        public void addQuickLaunchItem(  )
        {
            
        } // addQuickLaunchItem()

        private void _addQuickLaunchItem( )
        {

        } // _addQuickLaunchItem()

    } // class CswNbtWebServiceWelcomeItems
} // namespace ChemSW.Nbt.WebServices

