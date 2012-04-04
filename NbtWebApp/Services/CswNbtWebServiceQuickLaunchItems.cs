//using System;
//using System.Data;
//using System.Linq;
//using ChemSW.Core;
//using ChemSW.Nbt.Actions;
//using ChemSW.Nbt.ObjClasses;
//using ChemSW.Nbt.Security;
//using Newtonsoft.Json.Linq;

//namespace ChemSW.Nbt.WebServices
//{
//     <summary>
//     Webservice for the list of Quick Launch links
//     </summary>
//    public class CswNbtWebServiceQuickLaunchItems
//    {
//        private readonly CswNbtResources _CswNbtResources;

//        private const string ActionName = "actionname";
//        private const string ActionPk = "actionid";
//        private const string ActionSelected = "Include";

//        public CswNbtWebServiceQuickLaunchItems( CswNbtResources CswNbtResources )
//        {
//            _CswNbtResources = CswNbtResources;
//        }


//        public void initQuickLaunchItems()
//        {
//            ICswNbtUser User = _CswNbtResources.CurrentNbtUser;
//            if( User != null )
//            {
//                CswNbtObjClassUser UserOc = User.UserNode;

//                //Add the user's stored views to QuickLaunchHistory
//                foreach( CswNbtView View in UserOc.QuickLaunchViews.SelectedViews.Values.Where( View => View.IsFullyEnabled() ) )
//                {
//                    View.SaveToCache( true, false, true );
//                }

//                //Add the user's stored actions to QuickLaunchHistory
//                DataTable ActionsTable = UserOc.QuickLaunchActions.GetDataAsTable( ActionName, ActionPk );
//                foreach( CswNbtAction Action in ( from DataRow ActionRow in ActionsTable.Rows
//                                                  where CswConvert.ToBoolean( ActionRow[ActionSelected] )
//                                                  select CswNbtAction.ActionNameStringToEnum( CswConvert.ToString( ActionRow[ActionPk] ) )
//                                                      into NbtActionName
//                                                      select _CswNbtResources.Actions[NbtActionName]
//                                                          into ThisAction
//                                                          where null != ThisAction
//                                                          select ThisAction ) )
//                {
//                    Action.SaveToCache( true, true );
//                }

//            } // if(User != null)
//        } // initQuickLaunchItems()

//        /// <summary>
//        /// Returns Quick Launch Items including History in Session
//        /// </summary>
//        public JObject getQuickLaunchItems()
//        {
//            JObject Ret = new JObject();
//            ICswNbtUser User = _CswNbtResources.CurrentNbtUser;
//            if( User != null )
//            {
//                Ret = _CswNbtResources.SessionDataMgr.getQuickLaunchJson();
//            }
//            return Ret;
//        } // getQuickLaunchItems()

//    }
//} // namespace ChemSW.Nbt.WebServices

