using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using ChemSW.Nbt;

namespace NbtWebApp.WebSvc.Logic.CISPro
{
    [DataContract]
    public class MolData
    {
        [DataMember]
        public Collection<MolImgData> MolImgDataCollection;
        [DataMember]
        public Collection<StructureSearchViewData> StructureSearchViewDataCollection;

        public MolData()
        {
            MolImgDataCollection = new Collection<MolData.MolImgData>();
            StructureSearchViewDataCollection = new Collection<StructureSearchViewData>();
        }

        [DataContract]
        public class MolImgData
        {
            [DataMember]
            public String molString = String.Empty;
            [DataMember]
            public string nodeId = String.Empty;
            [DataMember]
            public string molImgAsBase64String = String.Empty;
        }

        [DataContract]
        public class StructureSearchViewData
        {
            [DataMember]
            public String viewId = String.Empty;
            [DataMember]
            public String viewMode = NbtViewRenderingMode.Table.ToString();
            [DataMember]
            public String molString = String.Empty;
            [DataMember]
            public bool exact = false;
        }

    }
}