﻿using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Core;
using NbtWebApp.WebSvc.Returns;

namespace NbtWebApp.Actions.ChemWatch
{
    [DataContract]
    public class CswNbtChemWatchListItem
    {
        [DataMember( Name = "display" )]
        public string Name { get; set; }

        [DataMember( Name = "value" )]
        public string Id { get; set; }
    }

    [DataContract]
    public class CswNbtChemWatchRequest
    {
        [DataMember]
        public Collection<CswNbtChemWatchListItem> Countries = new Collection<CswNbtChemWatchListItem>();

        [DataMember]
        public Collection<CswNbtChemWatchListItem> Languages = new Collection<CswNbtChemWatchListItem>();

        [DataMember]
        public string Supplier { get; set; }

        [DataMember]
        public Collection<CswNbtChemWatchListItem> Suppliers = new Collection<CswNbtChemWatchListItem>();

        [DataMember]
        public string PartNo { get; set; }

        [DataMember]
        public string MaterialName { get; set; }

        [DataMember]
        public int ChemWatchMaterialId { get; set; }

        [DataMember]
        public Collection<CswNbtChemWatchListItem> Materials = new Collection<CswNbtChemWatchListItem>();

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
    public class CswNbtChemWatchReturn: CswWebSvcReturn
    {
        public CswNbtChemWatchReturn()
        {
            Data = new CswNbtChemWatchRequest();
        }

        [DataMember]
        public CswNbtChemWatchRequest Data;
    }

}