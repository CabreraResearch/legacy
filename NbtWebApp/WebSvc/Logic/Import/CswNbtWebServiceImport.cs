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
            ret.DefinitionNames = Importer.getDefinitionNames();
        }

        public static void uploadImportData( ICswResources CswResources, ImportDataReturn ret, ImportDataParams parms )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtImporter Importer = new CswNbtImporter( CswNbtResources );

            Importer.storeData( parms.PostedFile.FileName, parms.ImportDefName, parms.Overwrite );
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
            public CswCommaDelimitedString DefinitionNames;
        }


    } // class CswNbtWebServiceImport

} // namespace ChemSW.Nbt.WebServices

