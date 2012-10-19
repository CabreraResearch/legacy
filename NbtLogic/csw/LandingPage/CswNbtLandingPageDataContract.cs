using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace ChemSW.Nbt.LandingPage
{
    [DataContract]
    public class LandingPageData
    {
        public LandingPageData()
        {
            LandingPageItems = new Collection<LandingPageItem>();
        }

        [DataContract]
        public class Request
        {
            [DataMember]
            public string RoleId = string.Empty;
            [DataMember]
            public string ActionId = string.Empty;
            [DataMember]
            public Int32 LandingPageId = Int32.MinValue;
            [DataMember]
            public Int32 NewRow = Int32.MinValue;
            [DataMember]
            public Int32 NewColumn = Int32.MinValue;
            [DataMember]
            public string NodeTypeId = string.Empty;
            [DataMember]
            public string Type = string.Empty;
            [DataMember]
            public string ViewType = string.Empty;
            [DataMember]
            public string PkValue = string.Empty;
            [DataMember]
            public string Text = string.Empty;
            [DataMember]
            public string NodeViewId = string.Empty;
            [DataMember]
            public string NodeId = string.Empty;
            [DataMember]
            public string ButtonIcon = string.Empty;
        }

        [DataContract]
        public class LandingPageItem
        {
            [DataMember]
            public string LandingPageId = string.Empty;
            [DataMember]
            public string Text = string.Empty;
            [DataMember]
            public string DisplayRow = string.Empty;
            [DataMember]
            public string DisplayCol = string.Empty;
            [DataMember]
            public string ButtonIcon = string.Empty;
            [DataMember]
            public string Type = string.Empty;
            [DataMember]
            public string LinkType = string.Empty;
            [DataMember]
            public string NodeTypeId = string.Empty;
            [DataMember]
            public string ViewId = string.Empty;
            [DataMember]
            public string ViewMode = string.Empty;
            [DataMember]
            public string ActionId = string.Empty;
            [DataMember]
            public string ActionName = string.Empty;
            [DataMember]
            public string ActionUrl = string.Empty;
            [DataMember]
            public string ReportId = string.Empty;
        }

        [DataMember]
        public Collection<LandingPageItem> LandingPageItems;

    } // LandingPageData
}
