using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ImportExport;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using NbtWebApp.WebSvc.Returns;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtImportWcf
    {
        [DataContract]
        public class ImportDataParams
        {
            [DataMember( IsRequired = true )]
            [Description( "Excel file content for import" )]
            public HttpPostedFile PostedFile;

            [DataMember( IsRequired = true )]
            [Description( "Name of import definition" )]
            public string ImportDefName;

            [DataMember( IsRequired = true )]
            [Description( "True if imported content should overwrite existing content" )]
            public bool Overwrite;
        }


        [DataContract]
        public class ImportStatusRequest
        {
            [DataMember( IsRequired = true )]
            [Description( "Job Id" )]
            public Int32 JobId;
        }


        [DataContract]
        public class ImportDataReturn : CswWebSvcReturn
        {
            [DataMember( IsRequired = true )]
            [Description( "Job Id" )]
            public Int32 JobId;
        }

        [DataContract]
        public class ImportDefsReturn : CswWebSvcReturn
        {
            [DataMember( IsRequired = true )]
            [Description( "Collection of import definitions" )]
            public string Data;
        }

        [DataContract]
        public class ImportJobsReturn : CswWebSvcReturn
        {
            public ImportJobsReturn()
            {
                Data = new Collection<CswNbtImportDataJob>();
            }

            [DataMember( IsRequired = true )] 
            [Description( "Collection of import jobs" )] 
            public Collection<CswNbtImportDataJob> Data;

        } // class ImportJobsReturn

        [DataContract]
        public class ImportStatusReturn : CswWebSvcReturn
        {
            public ImportStatusReturn()
            {
                Data = new ImportStatusReturnData();
            }

            [DataMember( IsRequired = true )]
            [Description( "Import status information" )]
            public ImportStatusReturnData Data;

            [DataContract]
            public class ImportStatusReturnData
            {
                [DataMember( IsRequired = true )]
                [Description( "Number of pending items processed" )]
                public Int32 ItemsDone;

                [DataMember( IsRequired = true )]
                [Description( "Total number of items to process" )]
                public Int32 ItemsTotal;

                [DataMember( IsRequired = true )]
                [Description( "Number of pending rows processed" )]
                public Int32 RowsDone;

                [DataMember( IsRequired = true )]
                [Description( "Number of rows in error state" )]
                public Int32 RowsError;

                [DataMember( IsRequired = true )]
                [Description( "Total number of rows to process" )]
                public Int32 RowsTotal;


            } // class ImportStatusReturnData
        } // class ImportStatusReturn


    } // class CswNbtImportWcf

} // namespace ChemSW.Nbt.WebServices

