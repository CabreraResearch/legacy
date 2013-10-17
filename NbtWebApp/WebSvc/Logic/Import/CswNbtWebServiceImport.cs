using System;
using System.Data;
using System.IO;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Grid;
using ChemSW.Nbt.ImportExport;
using ChemSW.Nbt.Sched;
using NbtWebApp.WebSvc.Returns;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceImport
    {
        public static void getImportDefs( ICswResources CswResources, CswNbtImportWcf.ImportDefsReturn ret, object parms )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtImporter Importer = new CswNbtImporter( CswNbtResources );
            ret.Data = Importer.getDefinitionNames().ToString();
        }

        public static void getImportJobs( ICswResources CswResources, CswNbtImportWcf.ImportJobsReturn ret, object parms )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtImporter Importer = new CswNbtImporter( CswNbtResources );
            foreach( CswNbtImportDataJob Job in Importer.getJobs() )
            {
                ret.Data.Add( Job );
            }
        }

        public static void getImportStatus( ICswResources CswResources, CswNbtImportWcf.ImportStatusReturn ret, CswNbtImportWcf.ImportStatusRequest parms )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtImporter Importer = new CswNbtImporter( CswNbtResources );

            if( Int32.MinValue != parms.JobId )
            {
                CswNbtImportDataJob Job = new CswNbtImportDataJob( CswNbtResources, parms.JobId );
                Job.getStatus( out ret.Data.RowsDone,
                               out ret.Data.RowsTotal,
                               out ret.Data.RowsError,
                               out ret.Data.ItemsDone,
                               out ret.Data.ItemsTotal );
            }
        }

        public static void uploadImportData( ICswResources CswResources, CswNbtImportWcf.ImportDataReturn ret, CswNbtImportWcf.ImportFileParams parms )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtImporter Importer = new CswNbtImporter( CswNbtResources );

            // Write uploaded file to temp dir
            CswTempFile myTempFile = new CswTempFile( CswResources );
            string path = myTempFile.saveToTempFile( parms.PostedFile.InputStream, DateTime.Now.Ticks + "_" + parms.PostedFile.FileName );
            ret.JobId = Importer.storeData( parms.PostedFile.FileName, path, parms.ImportDefName, parms.Overwrite );
        }


        public static void uploadImportDefinition( ICswResources CswResources, CswWebSvcReturn ret, CswNbtImportWcf.ImportFileParams parms )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtImporter Importer = new CswNbtImporter( CswNbtResources );

            // Write uploaded file to temp dir
            CswTempFile myTempFile = new CswTempFile( CswResources );
            string path = myTempFile.saveToTempFile( parms.PostedFile.InputStream, DateTime.Now.Ticks + "_" + parms.PostedFile.FileName );
            Importer.storeDefinition( path, parms.ImportDefName );
        }

        public static void startCAFImport( ICswResources CswResources, CswWebSvcReturn Ret, CswNbtImportWcf.StartImportParams Params )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;

            if( Params.ImportDefName.Equals( "CAF" ) )
            {
                // Enable the CAFImport rule
                CswTableUpdate TableUpdate = _CswNbtResources.makeCswTableUpdate( "enableCafImportRule", "scheduledrules" );
                DataTable DataTable = TableUpdate.getTable( "where rulename = '" + CswEnumNbtScheduleRuleNames.CAFImport + "'" );
                if( DataTable.Rows.Count > 0 )
                {
                    DataTable.Rows[0]["disabled"] = CswConvert.ToDbVal( false );
                    TableUpdate.update( DataTable );
                }
            }
        }

        public static void generateCAFSql( ICswResources CswResources, CswNbtImportWcf.GenerateSQLReturn Ret, string ImportDefName )
        {
            //CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;

            if( ImportDefName.Equals( "CAF" ) )
            {
                string ImportQueueSql = CswScheduleLogicNbtCAFImport.generateImportQueueTableSQL( CswResources );
                string CAFCleanupSQL = CswScheduleLogicNbtCAFImport.generateCAFCleanupSQL( CswResources );
                string TriggersSql = CswScheduleLogicNbtCAFImport.generateTriggerSQL( CswResources );

                // Create and return the stream
                MemoryStream stream = new MemoryStream();
                StreamWriter sw = new StreamWriter( stream );

                sw.Write( ImportQueueSql );
                sw.Write( "\r\n" );
                sw.Write( CAFCleanupSQL );
                sw.Write( "\r\n" );
                sw.Write( TriggersSql );

                sw.Flush();
                stream.Position = 0;

                Ret.stream = stream;
            }
        }//generateCAFSql()


        public static void getBindingsForDefinition( ICswResources CswResources, CswNbtImportWcf.ImportBindingsReturn Ret, string ImportDefName )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;

            string sql = @" select sheetname, io.* from import_def_order io, import_def id
                             where io.importdefid = id.importdefid
                               and definitionname = :importdefname";

            CswArbitrarySelect OrderTable = new CswArbitrarySelect( _CswNbtResources.CswResources, "ImportDefinitionsOrder", sql );
            OrderTable.addParameter( "importdefname", ImportDefName );
            Ret.Data.Order = new CswNbtGrid( _CswNbtResources ).DataTableToGrid( OrderTable.getTable() );
            Ret.Data.Order.columns.Remove( Ret.Data.Order.getColumn( "IMPORTDEFID" ) );
            Ret.Data.Order.columns.Remove( Ret.Data.Order.getColumn( "IMPORTDEFORDERID" ) );


            sql = @"select sheetname, ib.* from import_def_bindings ib, import_def id
                     where ib.importdefid = id.importdefid
                       and definitionname = :importdefname";

            CswArbitrarySelect BindingsTable = new CswArbitrarySelect( _CswNbtResources.CswResources, "ImportDefinitionsOrder", sql );
            BindingsTable.addParameter( "importdefname", ImportDefName );
            Ret.Data.Bindings = new CswNbtGrid( _CswNbtResources ).DataTableToGrid( BindingsTable.getTable() );
            Ret.Data.Bindings.columns.Remove( Ret.Data.Bindings.getColumn( "IMPORTDEFID" ) );
            Ret.Data.Bindings.columns.Remove( Ret.Data.Bindings.getColumn( "IMPORTDEFBINDINGID" ) );


            sql = @"select sheetname, ir.* from import_def_relationships ir, import_def id
                       where ir.importdefid = id.importdefid
                         and definitionname = :importdefname
";
            CswArbitrarySelect RelationshipsTable = new CswArbitrarySelect( _CswNbtResources.CswResources, "ImportDefinitionsOrder", sql );
            RelationshipsTable.addParameter( "importdefname", ImportDefName );
            Ret.Data.Relationships = new CswNbtGrid( _CswNbtResources ).DataTableToGrid( RelationshipsTable.getTable() );
            Ret.Data.Relationships.columns.Remove( Ret.Data.Relationships.getColumn( "IMPORTDEFID" ) );
            Ret.Data.Relationships.columns.Remove( Ret.Data.Relationships.getColumn( "IMPORTDEFRELATIONSHIPID" ) );
            
        }


    } // class CswNbtWebServiceImport

} // namespace ChemSW.Nbt.WebServices

