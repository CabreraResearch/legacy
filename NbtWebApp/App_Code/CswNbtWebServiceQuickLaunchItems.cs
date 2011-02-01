using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.PropTypes;

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

        public string getQuickLaunchItems( string UserId )
        {
            string ret = string.Empty;

            CswPrimaryKey UserPk = new CswPrimaryKey();
            UserPk.FromString( UserId );

            CswNbtNode UserNode = _CswNbtResources.Nodes.GetNode( UserPk );
            CswNbtObjClassUser UserOC = CswNbtNodeCaster.AsUser( UserNode );
            CswCommaDelimitedString QuickLaunchViews = UserOC.QuickLaunchViews.SelectedViewIds;
            Int32 DisplayRow = 0;
            foreach( CswNbtView QuickLaunchView in QuickLaunchViews.Where( View => !String.IsNullOrEmpty( View ) )
                                                  .Select( View => CswConvert.ToInt32( View ) )
                                                         .Where( ViewId => Int32.MinValue != ViewId )
                                                  .Select( ViewId => CswNbtViewFactory.restoreView( _CswNbtResources, ViewId ) )
                                                         .Where( QuickLaunchView => null != QuickLaunchView && QuickLaunchView.IsFullyEnabled() ) )
            {
                ret += "<item";
                ret += "      mode=\"" + QuickLaunchView.ViewMode + "\"";
                ret += "      type=\"" + QuickLaunchType.View  +"\"";
                ret += "      viewid=\"" + QuickLaunchView.ViewId + "\"";
                ret += "      text=\"" + QuickLaunchView.ViewName + "\"";
                ret += "      displayrow=\"" + DisplayRow + "\"";
                ret += "      displaycol=\"0\"";
                ret += "/>";
                DisplayRow++;
            } // foreach( CswNbtView QuickLaunchView...

            CswNbtNodePropLogicalSet ActionsLogicalSet = ( (CswNbtObjClassUser) _CswNbtResources.CurrentNbtUser.UserNode ).QuickLaunchActions;
            DataTable ActionsTable = ActionsLogicalSet.GetDataAsTable( ActionName, ActionPk );
            foreach( CswNbtAction ThisAction in from DataRow ActionRow in ActionsTable.Rows where CswConvert.ToBoolean( ActionRow[ActionSelected] ) 
                                                select _CswNbtResources.Actions[CswNbtAction.ActionNameStringToEnum( ActionRow[ActionName].ToString() )] into ThisAction 
                                                       where null != ThisAction 
                                                select ThisAction )
            {
                ret += "<item";
                ret += "      type=\"" + QuickLaunchType.Action + "\"";
                ret += "      actionid=\"" + ThisAction.ActionId + "\"";
                ret += "      text=\"" +  ThisAction.Name + "\"";
                ret += "      url=\"" +  ThisAction.Url + "\"";
                ret += "      displayrow=\"" + DisplayRow + "\"";
                ret += "      displaycol=\"0\"";
                ret += "/>";
                DisplayRow++;
            } // foreach( CswNbtAction ThisAction...

            return ( "<quicklaunch>" + ret + "</quicklaunch>" );

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

