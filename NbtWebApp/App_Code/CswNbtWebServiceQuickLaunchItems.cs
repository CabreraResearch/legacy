using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    /// <summary>
    /// Webservice for the list of Quick Launch links
    /// </summary>
    public class CswNbtWebServiceQuickLaunchItems
    {
        private readonly CswNbtResources _CswNbtResources;

        private const string ActionName = "actionname";
        private const string ActionPk = "actionid";
        private const string ActionSelected = "Include";

        public CswNbtWebServiceQuickLaunchItems( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }


        public void initQuickLaunchItems()
        {
            ICswNbtUser User = _CswNbtResources.CurrentNbtUser;
            if( User != null )
            {
                CswNbtObjClassUser UserOc = User.UserNode;

                //Add the user's stored views to QuickLaunchHistory
                Collection<Int32> UserQuickLaunchViews = UserOc.QuickLaunchViews.SelectedViewIds.ToIntCollection();
                foreach( Int32 ViewIdNum in UserQuickLaunchViews )
                {
                    CswNbtViewId ThisViewId = new CswNbtViewId( ViewIdNum );
                    CswNbtView ThisView = _CswNbtResources.ViewSelect.restoreView( ThisViewId );
                    if( ThisView.IsFullyEnabled() )
                    {
                        ThisView.SaveToCache( true );
                    }
                }

                //Add the user's stored actions to QuickLaunchHistory
                CswNbtNodePropLogicalSet ActionsLogicalSet = UserOc.QuickLaunchActions;
                DataTable ActionsTable = ActionsLogicalSet.GetDataAsTable( ActionName, ActionPk );
                foreach( CswNbtAction Action in ( from DataRow ActionRow in ActionsTable.Rows
                                                  where CswConvert.ToBoolean( ActionRow[ActionSelected] )
                                                  select _CswNbtResources.Actions[CswConvert.ToInt32( ActionRow[ActionPk] )]
                                                      into ThisAction
                                                      where null != ThisAction
                                                      select ThisAction ) )
                {
                    Action.SaveToCache( true );
                }
            } // if(User != null)
        } // initQuickLaunchItems()

        /// <summary>
        /// Returns Quick Launch Items including History in Session
        /// </summary>
        public JObject getQuickLaunchItems()
        {
            JObject Ret = new JObject();
            Dictionary<int, CswNbtView> UserQuickLaunchViews = new Dictionary<int, CswNbtView>();
            Dictionary<int, CswNbtAction> UserQuickLaunchActions = new Dictionary<int, CswNbtAction>();

            ICswNbtUser User = _CswNbtResources.CurrentNbtUser;
            if( User != null )
            {
                CswNbtObjClassUser UserOc = User.UserNode;
                foreach( CswNbtView View in UserOc.QuickLaunchViews.SelectedViews )
                {
                    Int32 ViewId = null == View.SessionViewId ? View.ViewId.get() : View.SessionViewId.get();
                    UserQuickLaunchViews.Add( ViewId, View );
                }

                DataTable ActionsTable = UserOc.QuickLaunchActions.GetDataAsTable( ActionName, ActionPk );
                foreach( CswNbtAction Action in ( from DataRow ActionRow in ActionsTable.Rows
                                                  where CswConvert.ToBoolean( ActionRow[ActionSelected] )
                                                  select CswNbtAction.ActionNameStringToEnum( CswConvert.ToString( ActionRow[ActionPk] ) )
                                                      into NbtActionName
                                                      select _CswNbtResources.Actions[NbtActionName]
                                                          into ThisAction
                                                          where null != ThisAction
                                                          select ThisAction ) )
                {
                    UserQuickLaunchActions.Add( Action.ActionId, Action );
                }
            }
            Ret = _CswNbtResources.SessionDataMgr.getQuickLaunchJson( UserQuickLaunchViews, UserQuickLaunchActions );
            return Ret;
        } // getQuickLaunchItems()

    }
} // namespace ChemSW.Nbt.WebServices

