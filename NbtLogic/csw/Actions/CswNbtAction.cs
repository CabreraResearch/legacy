using System;
using System.Data;
using ChemSW.DB;

namespace ChemSW.Nbt.Actions
{
    public class CswNbtAction
    {
        public const string PermissionXValue = "Allow";

        public Int32 ActionId;
        public string Url;
        public string IconFileName = "wizard.png";
        public CswEnumNbtActionName Name;
        public bool ShowInList;
        public string Category;

        public string DisplayName
        {
            get { return Name.ToString().Replace( '_', ' ' ); }
        }

        private CswNbtResources _CswNbtResources;

        public CswNbtAction( CswNbtResources CswNbtResources, Int32 TheActionId, string ActionUrl, CswEnumNbtActionName ActionName, bool ActionShowInList, string ActionCategory, string ActionIconFileName )
        {
            _CswNbtResources = CswNbtResources;
            ActionId = TheActionId;
            Url = ActionUrl;
            Name = ActionName;
            ShowInList = ActionShowInList;
            Category = ActionCategory;
            if( false == string.IsNullOrEmpty( ActionIconFileName ) )
            {
                IconFileName = ActionIconFileName;
            }
        }

        public static string ActionNameEnumToString( CswEnumNbtActionName ActionName )
        {
            return ActionName.ToString().Replace( '_', ' ' );
        }
        public static CswEnumNbtActionName ActionNameStringToEnum( string ActionName )
        {
            CswEnumNbtActionName Ret;
            Enum.TryParse( ActionName.Replace( ' ', '_' ), true, out Ret );
            return Ret;
        }

        public CswNbtSessionDataId SaveToCache( bool IncludeInQuickLaunch, bool KeepInQuickLaunch )
        {
            return _CswNbtResources.SessionDataMgr.saveSessionData( this, IncludeInQuickLaunch, KeepInQuickLaunch );
        }

        public void SetCategory( string Category )
        {
            CswTableUpdate actionsTU = _CswNbtResources.makeCswTableUpdate( "setActionCategory", "actions" );
            DataTable actionsDT = actionsTU.getTable( "where actionid = " + ActionId );
            foreach( DataRow row in actionsDT.Rows )
            {
                row["category"] = Category;
            }
            actionsTU.update( actionsDT );
        }
    }
}
