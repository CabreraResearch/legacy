using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using ChemSW.Core;

namespace NbtWebApp.Actions.ChemWatch
{
    [DataContract]
    public class ChemWatchListItem
    {
        [DataMember( Name = "display" )]
        public string Name { get; set; }

        [DataMember( Name = "value" )]
        public string Id { get; set; }
    }

    [DataContract]
    public class ChemWatchMultiSlctListItem
    {
        [DataMember( Name = "text" )]
        public string Name { get; set; }

        [DataMember( Name = "value" )]
        public string Id { get; set; }
    }

    [DataContract]
    public class ChemWatchSDSDoc
    {
        [DataMember( Name = "language" )]
        public string Language { get; set; }

        [DataMember( Name = "country" )]
        public string Country { get; set; }

        [DataMember( Name = "filename" )]
        public string FileName { get; set; }

        [DataMember( Name = "externalurl" )]
        public string ExternalUrl { get; set; }
    }

    [DataContract]
    public class ChemWatchLngCntryInfo
    {
        [DataMember]
        public List<ChemWatchMultiSlctListItem> options = new List<ChemWatchMultiSlctListItem>();

        [DataMember]
        public List<ChemWatchMultiSlctListItem> selected = new List<ChemWatchMultiSlctListItem>();

        // TODO: Implement these for Case 31313
        [DataMember]
        public string readonlyless { get; set; }

        // TODO: Implement these for Case 31313
        [DataMember]
        public string readonlymore { get; set; }
    }

    [DataContract]
    public class CswNbtChemWatchRequest
    {
        [DataMember]
        public ChemWatchLngCntryInfo Countries = new ChemWatchLngCntryInfo();

        [DataMember]
        public ChemWatchLngCntryInfo Languages = new ChemWatchLngCntryInfo();

        [DataMember]
        public string Supplier { get; set; }

        [DataMember]
        public Collection<ChemWatchListItem> Suppliers = new Collection<ChemWatchListItem>();

        [DataMember]
        public string PartNo { get; set; }

        [DataMember]
        public string MaterialName { get; set; }

        [DataMember]
        public int ChemWatchMaterialId { get; set; }

        [DataMember]
        public Collection<ChemWatchListItem> Materials = new Collection<ChemWatchListItem>();

        [DataMember]
        public Stream SDSDocument = null;

        [DataMember]
        public Collection<ChemWatchSDSDoc> SDSDocuments = new Collection<ChemWatchSDSDoc>();

        public CswPrimaryKey NbtMaterialId = null;
        [DataMember( Name = "NbtMaterialId" )]
        public string NbtMaterialIdStr
        {
            get { return null == NbtMaterialId ? "" : NbtMaterialId.ToString(); }
            set { NbtMaterialId = CswConvert.ToPrimaryKey( value ); }
        }

        [DataMember( Name = "message" )]
        public string Message { get; set; }
    }

}