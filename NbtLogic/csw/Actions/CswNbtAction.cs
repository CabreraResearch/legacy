using System;

namespace ChemSW.Nbt.Actions
{
    public enum CswNbtActionName
    {
        Unknown,
        Create_Material,
        Design,
        Edit_View,
        Future_Scheduling,
        Create_Inspection,
        Multi_Edit,
        Quotas,
        Sessions,
        View_Scheduled_Rules,
        Modules,
        Submit_Request,
        DispenseContainer,
        DisposeContainer,
        UndisposeContainer,
        Receiving,
        Subscriptions,
        Reconciliation
    }

    public class CswNbtAction
    {
        public const string PermissionXValue = "Allow";

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
            CswNbtActionName Ret;
            Enum.TryParse( ActionName.Replace( ' ', '_' ), true, out Ret );
            return Ret;
        }

        public CswNbtSessionDataId SaveToCache( bool IncludeInQuickLaunch, bool KeepInQuickLaunch )
        {
            return _CswNbtResources.SessionDataMgr.saveSessionData( this, IncludeInQuickLaunch, KeepInQuickLaunch );
        }
    }
}
