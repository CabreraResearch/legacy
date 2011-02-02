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
            Int32 DisplayRow = 0;

            // Add Recent Views from Session First
            if( null != Session["ViewHistory"] )
            {
                Dictionary<Int32, string> ViewHistory = (Dictionary<Int32, string>) Session["ViewHistory"];
                foreach( Int32 ViewId in ViewHistory.Keys )
                {
                    ret += "<item";
                    ret += "      type=\"" + QuickLaunchType.View + "\"";
                    ret += "      viewid=\"" + ViewId + "\"";
                    ret += "      text=\"" + ViewHistory[ViewId].ToString() + "\"";
                    ret += "      displayrow=\"" + DisplayRow + "\"";
                    ret += "      displaycol=\"0\"";
                    ret += "/>";
                    DisplayRow++;
                }
            }

            CswNbtNode UserNode = _CswNbtResources.Nodes.GetNode( UserId );
            if( null != UserNode )
            {
                // Add Stored Views Next
                CswNbtObjClassUser UserOC = CswNbtNodeCaster.AsUser( UserNode );
                CswCommaDelimitedString QuickLaunchViews = UserOC.QuickLaunchViews.SelectedViewIds;
                foreach( CswNbtView QuickLaunchView in QuickLaunchViews.Where( View => !String.IsNullOrEmpty( View ) )
                            .Select( View => CswConvert.ToInt32( View ) )
                            .Where( ViewId => Int32.MinValue != ViewId )
                            .Select( ViewId => CswNbtViewFactory.restoreView( _CswNbtResources, ViewId ) )
                            .Where( QuickLaunchView => null != QuickLaunchView && QuickLaunchView.IsFullyEnabled() ) )
                {
                    ret += "<item";
                    ret += "      mode=\"" + QuickLaunchView.ViewMode + "\"";
                    ret += "      type=\"" + QuickLaunchType.View + "\"";
                    ret += "      viewid=\"" + QuickLaunchView.ViewId + "\"";
                    ret += "      text=\"" + QuickLaunchView.ViewName + "\"";
                    ret += "      displayrow=\"" + DisplayRow + "\"";
                    ret += "      displaycol=\"0\"";
                    ret += "/>";
                    DisplayRow++;
                } // foreach( CswNbtView QuickLaunchView...

                // Add Stored Actions Last
                CswNbtNodePropLogicalSet ActionsLogicalSet = ( (CswNbtObjClassUser) _CswNbtResources.CurrentNbtUser.UserNode ).QuickLaunchActions;
                DataTable ActionsTable = ActionsLogicalSet.GetDataAsTable( ActionName, ActionPk );
                foreach( CswNbtAction ThisAction in from DataRow ActionRow in ActionsTable.Rows
                                                    where CswConvert.ToBoolean( ActionRow[ActionSelected] )
                                                    select _CswNbtResources.Actions[CswNbtAction.ActionNameStringToEnum( ActionRow[ActionName].ToString() )]
                                                    into ThisAction
                                                    where null != ThisAction
                                                    select ThisAction )
                {
                    ret += "<item";
                    ret += "      type=\"" + QuickLaunchType.Action + "\"";
                    ret += "      actionid=\"" + ThisAction.ActionId + "\"";
                    ret += "      text=\"" + ThisAction.Name + "\"";
                    ret += "      url=\"" + ThisAction.Url + "\"";
                    ret += "      displayrow=\"" + DisplayRow + "\"";
                    ret += "      displaycol=\"0\"";
                    ret += "/>";
                    DisplayRow++;
                } // foreach( CswNbtAction ThisAction...
            }

            if( !string.IsNullOrEmpty(ret) )
            {
                ret = "<quicklaunch>" + ret + "</quicklaunch>";
            }

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

