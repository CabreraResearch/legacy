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
    public class CswNbtWebServiceImport
    {
        public static void getImportDefs( ICswResources CswResources, ImportDefsReturn ret, object parms )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtImporter Importer = new CswNbtImporter( CswNbtResources );
            ret.Data = Importer.getDefinitionNames().ToString();
        }

        public static void getImportStatus( ICswResources CswResources, ImportStatusReturn ret, object parms )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtImporter Importer = new CswNbtImporter( CswNbtResources );

            foreach( string ImportDataTableName in Importer.getImportDataTableNames( true ) )
            {
                Int32 PendingRows;
                Int32 ErrorRows;
                Importer.getCounts( ImportDataTableName, out PendingRows, out ErrorRows );

                ret.Data.PendingCount += PendingRows;
                ret.Data.ErrorCount += ErrorRows;
            }
        }

        public static void uploadImportData( ICswResources CswResources, ImportDataReturn ret, ImportDataParams parms )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtImporter Importer = new CswNbtImporter( CswNbtResources );

            // Write uploaded file to temp dir
            CswTempFile myTempFile = new CswTempFile( CswResources );
            string path = myTempFile.saveToTempFile( parms.PostedFile.InputStream, DateTime.Now.Ticks + "_" + parms.PostedFile.FileName );
            Importer.storeData( path, parms.ImportDefName, parms.Overwrite );
        }

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
        public class ImportDataReturn : CswWebSvcReturn
        {
        }

        [DataContract]
        public class ImportDefsReturn : CswWebSvcReturn
        {
            [DataMember( IsRequired = true )]
            [Description( "Collection of import definitions" )]
            public string Data;
        }

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
                //    [DataMember( IsRequired = true )]
                //    [Description( "Name of import definition" )]
                //    public string ImportDefName;

                //    [DataMember( IsRequired = true )]
                //    [Description( "Sheet name of source data" )]
                //    public string SheetName;

                //    [DataMember( IsRequired = true )]
                //    [Description( "Name of import data Oracle table" )]
                //    public string ImportDataTableName;

                [DataMember( IsRequired = true )]
                [Description( "Number of pending rows to process" )]
                public Int32 PendingCount;

                [DataMember( IsRequired = true )]
                [Description( "Number of rows in error state" )]
                public Int32 ErrorCount;

            } // class ImportStatusReturnData
        } // class ImportStatusReturn


    } // class CswNbtWebServiceImport

} // namespace ChemSW.Nbt.WebServices

