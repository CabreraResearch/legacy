using System;
using System.Collections.Generic;
using System.Text;

namespace ChemSW.Nbt.Actions
{
    public enum CswNbtActionName
    {
        Assign_Tests,
        Design,
        Edit_View,
        Enter_Results,
        Future_Scheduling,
        Inspection_Design,
        Create_Inspection,
        Load_Mobile_Data,
        Split_Samples,
        View_By_Location,
        Assign_Inspection,
        Receiving,
        Import_Fire_Extinguisher_Data
    }

    public class CswNbtAction
    {
        public static string PermissionXValue = "Allow";

        public Int32 ActionId;
        public string Url;
        public CswNbtActionName Name;
        public bool ShowInList;
        public string Category;

        public string DisplayName
        {
            get { return Name.ToString().Replace( '_', ' ' ); }
        }

		private CswNbtResources _CswNbtResources;
		
		public CswNbtAction( CswNbtResources CswNbtResources, Int32 TheActionId, string ActionUrl, CswNbtActionName ActionName, bool ActionShowInList, string ActionCategory )
        {
			_CswNbtResources = CswNbtResources;
            ActionId = TheActionId;
            Url = ActionUrl;
            Name = ActionName;
            ShowInList = ActionShowInList;
            Category = ActionCategory;
        }

        public static string ActionNameEnumToString( CswNbtActionName ActionName )
        {
            return ActionName.ToString().Replace( '_', ' ' );
        }
        public static CswNbtActionName ActionNameStringToEnum( string ActionName )
        {
            return (CswNbtActionName) Enum.Parse( typeof( CswNbtActionName ), ActionName.Replace( ' ', '_' ) );
        }

		public CswNbtSessionDataId SaveToCache( bool IncludeInQuickLaunch )
		{
			return _CswNbtResources.SessionDataMgr.saveSessionData( this, IncludeInQuickLaunch );
		}
    }
}
