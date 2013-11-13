using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Core;
using NbtWebApp.WebSvc.Returns;

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
    public class CswNbtChemWatchRequest
    {
        [DataMember]
        public Collection<ChemWatchMultiSlctListItem> Countries = new Collection<ChemWatchMultiSlctListItem>();

        [DataMember]
        public Collection<ChemWatchMultiSlctListItem> Languages = new Collection<ChemWatchMultiSlctListItem>();

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
        public Collection<ChemWatchSDSDoc> SDSDocuments = new Collection<ChemWatchSDSDoc>();

        public CswPrimaryKey NbtMaterialId = null;
        [DataMember( Name = "NbtMaterialId" )]
        public string NbtMaterialIdStr
        {
            get
            {
                if( null == NbtMaterialId )
                {
                    return "";
                }
                else
                {
                    return NbtMaterialId.ToString();
                }
            }

            set
            {
                NbtMaterialId = CswConvert.ToPrimaryKey( value );
            }
        }
    }

    [DataContract]
    public class CswNbtChemWatchReturn : CswWebSvcReturn
    {
        public CswNbtChemWatchReturn()
        {
            Data = new CswNbtChemWatchRequest();
        }

        [DataMember]
        public CswNbtChemWatchRequest Data;
    }

}