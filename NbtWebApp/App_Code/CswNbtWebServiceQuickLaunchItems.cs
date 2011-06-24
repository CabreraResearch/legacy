﻿using System;
using System.Data;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.SessionState;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.PropTypes;
using System.Collections.Generic;
using ChemSW.Session;
using ChemSW.Nbt.Security;

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
			if(User != null)
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
													select _CswNbtResources.Actions[CswConvert.ToInt32(ActionRow[ActionPk])]
														into ThisAction
														where null != ThisAction
														select ThisAction ))
				{
					Action.SaveToCache( true );
				}
			} // if(User != null)
		} // initQuickLaunchItems()
		
		/// <summary>
		/// Returns Quick Launch Items including History in Session
		/// </summary>
		public XElement getQuickLaunchItems()
		{
			return _CswNbtResources.SessionDataMgr.getQuickLaunchXml();

		} // getQuickLaunchItems()

	}
} // namespace ChemSW.Nbt.WebServices

