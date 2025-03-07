using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Web;
using ChemSW.Grid.ExtJs;
using ChemSW.Nbt.ImportExport;
using NbtWebApp.WebSvc.Returns;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtImportWcf
    {
        [DataContract]
        public class ImportFileParams
        {
            [DataMember( IsRequired = true )]
            [Description( "Excel file content for import" )]
            public HttpPostedFile PostedFile;

            [DataMember( IsRequired = true )]
            [Description( "Name of import definition" )]
            public string ImportDefName;

            [DataMember( IsRequired = false )]
            [Description( "True if imported content should overwrite existing content" )]
            public bool Overwrite;
        }

        [DataContract]
        public class StartImportParams
        {
            [DataMember( IsRequired = true )]
            [Description( "The database username of the target CAF schema" )]
            public string CAFSchema;

            [DataMember( IsRequired = true )]
            [Description( "Database password for the target CAF schema" )]
            public string CAFPassword;

            [DataMember( IsRequired = true )]
            [Description( "TNS name for target CAF schema" )]
            public string CAFDatabase;
        }

        [DataContract]
        public class JobRequest
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
            [Description( "Import definitions" )]
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
                [Description( "End Date, if available" )]
                public DateTime DateEnded;

                [DataMember( IsRequired = true )]
                [Description( "Whether the import is complete" )]
                public bool Completed
                {
                    get { return ( DateTime.MinValue != DateEnded ); }
                    set { }
                }

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

        [DataContract]
        public class GenerateSQLReturn : CswWebSvcReturn
        {
            [DataMember]
            public Stream stream = null;
        }

        [DataContract]
        public class ImportBindingsReturn : CswWebSvcReturn
        {
            [DataMember( IsRequired = true )]
            [Description( "Information about import bindings" )]
            public ImportBindingReturnData Data = new ImportBindingReturnData();
            public class ImportBindingReturnData
            {
                public CswExtJsGrid Order;
                public CswExtJsGrid Bindings;
                public CswExtJsGrid Relationships;
            }
        }

        [DataContract]
        public class DefinitionUpdateRow
        {
                public DefinitionUpdateRow()
                {
                    row = new Dictionary<string, string>();
                }

                [DataMember( IsRequired = true)]
                public string editMode;

                [DataMember( IsRequired = true )]
                public string definitionType;

                [DataMember( IsRequired = true )]
                public  Dictionary<string, string> row;
        }

        [DataContract]
        public class DltExistingNodesReturn : CswWebSvcReturn
        {
            public DltExistingNodesReturn()
            {
                Data = new DltExistingNodesReturnData();
            }

            [DataMember( IsRequired = true )]
            [Description( "" )]
            public DltExistingNodesReturnData Data;

            [DataContract]
            public class DltExistingNodesReturnData
            {
                [DataMember]
                public bool DeleteSuccessful { get; set; }

                [DataMember]
                public Collection<DoomedNode> NodesToDelete = new Collection<DoomedNode>();

                [DataContract]
                public class DoomedNode
                {
                    [DataMember( Name = "nodeid" )]
                    public Int32 NodeId { get; set; }

                    [DataMember( Name = "nodename" )]
                    public string NodeName { get; set; }

                    [DataMember( Name = "nodetype" )]
                    public string NodeType { get; set; }
                }
            }
        }

    } // class CswNbtImportWcf

} // namespace ChemSW.Nbt.WebServices

