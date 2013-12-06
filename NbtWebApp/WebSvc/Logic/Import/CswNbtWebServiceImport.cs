using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.csw.ImportExport;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.Grid;
using ChemSW.Nbt.ImportExport;
using ChemSW.Nbt.Sched;
using ChemSW.Nbt.Schema;
using ChemSW.RscAdo;
using NbtWebApp.WebSvc.Returns;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceImport
    {
        public static void getImportDefs( ICswResources CswResources, CswNbtImportWcf.ImportDefsReturn ret, object parms )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            ret.Data = CswNbtImportTools.getDefinitionNames( CswNbtResources ).ToString();
        }

        public static void getImportJobs( ICswResources CswResources, CswNbtImportWcf.ImportJobsReturn ret, object parms )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            foreach( CswNbtImportDataJob Job in CswNbtImportTools.getJobs( CswNbtResources ) )
            {
                ret.Data.Add( Job );
            }
        }

        public static void getImportStatus( ICswResources CswResources, CswNbtImportWcf.ImportStatusReturn ret, CswNbtImportWcf.JobRequest parms )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            if( Int32.MinValue != parms.JobId )
            {
                CswNbtImportDataJob Job = new CswNbtImportDataJob( CswNbtResources, parms.JobId );
                ret.Data.DateEnded = Job.DateEnded;
                Job.getStatus( out ret.Data.RowsDone,
                               out ret.Data.RowsTotal,
                               out ret.Data.RowsError,
                               out ret.Data.ItemsDone,
                               out ret.Data.ItemsTotal );
            }
        }

        /// <summary>
        /// Cancel a running job
        /// </summary>
        public static void cancelJob( ICswResources CswResources, CswWebSvcReturn ret, CswNbtImportWcf.JobRequest parms )
        {
            CswNbtImportTools.CancelJob( (CswNbtResources) CswResources, parms.JobId );
        }

        public static void uploadImportData( ICswResources CswResources, CswNbtImportWcf.ImportDataReturn ret, CswNbtImportWcf.ImportFileParams parms )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;

            // Write uploaded file to temp dir
            CswFilePath FilePathMgr = new CswFilePath( CswResources );
            string FullFilePath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "\\import\\" + FilePathMgr.getFileNameForSchema( parms.PostedFile.FileName );

            FileStream ImportDataFile = File.Create( FullFilePath );
            parms.PostedFile.InputStream.CopyTo( ImportDataFile );
            ImportDataFile.Close();
            parms.PostedFile.InputStream.Close();

            ret.JobId = CswNbtImportTools.storeData( CswNbtResources, parms.PostedFile.FileName, FullFilePath, parms.ImportDefName, parms.Overwrite );
        }


        public static void downloadImportData( ICswResources CswResources, CswNbtImportWcf.GenerateSQLReturn Ret, string Filename )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;

            CswFilePath FilePathMgr = new CswFilePath( CswResources );
            string FullFilePath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "\\import\\" + FilePathMgr.getFileNameForSchema( Filename );

            FileStream DataFile = File.OpenRead( FullFilePath );
            DataFile.CopyTo( Ret.stream );
            DataFile.Close();
        }


        public static void uploadImportDefinition( ICswResources CswResources, CswWebSvcReturn ret, CswNbtImportWcf.ImportFileParams parms )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtImporter Importer = new CswNbtImporter( CswNbtResources.AccessId, CswEnumSetupMode.NbtWeb );

            // Write uploaded file to temp dir
            CswTempFile myTempFile = new CswTempFile( CswResources );
            string path = myTempFile.saveToTempFile( parms.PostedFile.InputStream, DateTime.Now.Ticks + "_" + parms.PostedFile.FileName );
            Importer.storeDefinition( path, parms.ImportDefName );
        }



        public static void downloadImportDefinition( ICswResources CswResources, CswNbtImportWcf.GenerateSQLReturn Ret, string ImportDefName )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            Ret.stream = new MemoryStream();
            StreamWriter sw = new StreamWriter( Ret.stream );

            #region document_header
            sw.Write( @"<?xml version=""1.0""?>
<?mso-application progid=""Excel.Sheet""?>
<Workbook
   xmlns=""urn:schemas-microsoft-com:office:spreadsheet""
   xmlns:o=""urn:schemas-microsoft-com:office:office""
   xmlns:x=""urn:schemas-microsoft-com:office:excel""
   xmlns:ss=""urn:schemas-microsoft-com:office:spreadsheet""
   xmlns:html=""http://www.w3.org/TR/REC-html40"">
  <DocumentProperties xmlns=""urn:schemas-microsoft-com:office:office"">
    <Author>Accelrys Inc.</Author>
    <LastAuthor>ChemSW Live</LastAuthor>
    <Created>2007-03-15T23:04:04Z</Created>
    <Company>Accelrys Inc.</Company>
    <Version>1</Version>
  </DocumentProperties>
  <ExcelWorkbook xmlns=""urn:schemas-microsoft-com:office:excel"">
    <WindowHeight>6795</WindowHeight>
    <WindowWidth>8460</WindowWidth>
    <WindowTopX>120</WindowTopX>
    <WindowTopY>15</WindowTopY>
    <ProtectStructure>False</ProtectStructure>
    <ProtectWindows>False</ProtectWindows>
  </ExcelWorkbook>
  <Styles>
    <Style ss:ID=""Default"" ss:Name=""Normal"">
      <Alignment ss:Vertical=""Bottom"" />
      <Borders />
      <Font />
      <Interior />
      <NumberFormat />
      <Protection />
    </Style>
    <Style ss:ID=""s21"">
      <Font x:Family=""Swiss"" ss:Bold=""1"" />
    </Style>
  </Styles>" );

            #endregion


            Dictionary<string, DataTable> BindingsTables = _retrieveBindings( NbtResources, ImportDefName );

            DataTable dt = BindingsTables["Order"];

            foreach( KeyValuePair<string, DataTable> Table in BindingsTables )
            {
                //remove the PK column so its not exposed in the excel file
                Table.Value.Columns.Remove( "importdef" + Table.Key.TrimEnd( new[] { 's' } ) + "id" );


                //name each worksheet tab after the table
                sw.Write( "<Worksheet ss:Name=\"" + Table.Key + "\"><Table>" );

                //add the column headers along the first row of cells
                sw.Write( "<Row>" );
                foreach( DataColumn Column in Table.Value.Columns )
                {
                    sw.Write( "<Cell ss:StyleID=\"s21\"><Data ss:Type=\"String\">" + Column.ColumnName + "</Data></Cell>" );
                }
                sw.Write( "</Row>" );
                sw.Write( "<Row />" );

                //after an extra blank space, add the data for each row
                foreach( DataRow Row in Table.Value.Rows )
                {
                    sw.Write( "<Row>" );
                    foreach( DataColumn Column in Table.Value.Columns )
                    {
                        sw.Write( "<Cell><Data ss:Type=\"String\">" + Row[Column] + "</Data></Cell>" );
                    }
                    sw.Write( "</Row>" );
                }

                sw.Write( "</Table></Worksheet>" );
            }

            sw.Write( "</Workbook>" );
            sw.Flush();
            Ret.stream.Position = 0;
        }

        public static void updateImportDefinition( ICswResources CswResources, CswWebSvcReturn Ret, CswNbtImportWcf.DefinitionUpdateRow[] Params )
        {
            //NOTE: if we decide to use definitions other than CAF in the future, we're going to need a way to discern what definition we're working with
            CswNbtSchemaUpdateImportMgr ImportUpdater = new CswNbtSchemaUpdateImportMgr( new CswNbtSchemaModTrnsctn((CswNbtResources)CswResources), "CAF", ImporterSetUpMode: CswEnumSetupMode.NbtWeb );

            foreach( CswNbtImportWcf.DefinitionUpdateRow Row in Params )
            {
                ImportUpdater.updateDefinitionElementByPK( Row.definitionType, Row.editMode, Row.row );
            }

            ImportUpdater.finalize();
        }


        public static void startCAFImport( ICswResources CswResources, CswWebSvcReturn Ret, CswNbtImportWcf.StartImportParams Params )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;

            //connect to the CAF database
            CswDbVendorOpsOracle CAFConnection = new CswDbVendorOpsOracle( "CAFImport", Params.CAFDatabase, Params.CAFSchema, Params.CAFPassword, (CswDataDictionary) _CswNbtResources.DataDictionary, _CswNbtResources.CswLogger, CswEnumPooledConnectionState.Open, "" );

            string Error = "";
            if( false == CAFConnection.IsDbConnectionHealthy( ref Error ) )
            {
                throw new CswDniException(CswEnumErrorType.Error, "Check the supplied parameters for the CAF database.", Error);
            }

            //Run the SQL to generate the table, views, triggers, and other setup operations.
            //there is no clean solution for running the contents of .SQL file from inside C#, so please forgive the horrible hacks that follow.
            //Assumptions made here: 
            //   the only PL/SQL blocks are the deletes at the top of the script and the triggers at the bottom, 
            //   the / at the end of PL/SQL is always at the beginning of a line, 
            //   triggers always have two lines of spaces before them, except the very first trigger, which has 3

            string CAFSql = generateCAFSql( _CswNbtResources );

            //add a / before the first trigger and split the file into an array of strings on / chars (breaking off potential PL/SQL blocks)
            string[] SQLCommands = CAFSql
                                      .Replace( ");\r\n\r\n\r\ncreate or replace trigger", ");\r\n\r\n\r\n/\r\ncreate or replace trigger" )
                                      .Replace( "create or replace procedure", "\r\n/\r\ncreate or replace procedure" )
                                      .Split( new[] { "\r\n/" }, StringSplitOptions.RemoveEmptyEntries );

            foreach( string SQLCommand in SQLCommands )
            {   //if the string starts with any of these, it's a PL/SQL block and can be sent as-is
                if( SQLCommand.Trim().StartsWith( "begin" ) || SQLCommand.Trim().StartsWith( "create or replace trigger" ) || SQLCommand.Trim().StartsWith( "create or replace procedure" ) )
                {
                    CAFConnection.execArbitraryPlatformNeutralSql( SQLCommand );
                }
                //otherwise, we need to further split out each command on ; chars
                else
                {
                    foreach( string SingleCommand in SQLCommand.Split( ';' ) )
                    {
                        if( SingleCommand.Trim() != String.Empty )
                        {
                            CAFConnection.execArbitraryPlatformNeutralSql( SingleCommand.Trim() );
                        }
                    }
                }
            }//foreach PL/SQL block in CAF.sql


            //create the database link
            _CswNbtResources.execArbitraryPlatformNeutralSql( "create database link caflink connect to " + Params.CAFSchema + " identified by " + Params.CAFPassword + " using '" + Params.CAFDatabase + "'" );

            //Create custom NodeTypeProps from CAF Properties collections and set up bindings for them
            CswNbtImportTools.CreateAllCAFProps( _CswNbtResources, CswEnumSetupMode.NbtWeb );

            // Enable the CAFImport rule
            CswTableUpdate TableUpdate = _CswNbtResources.makeCswTableUpdate( "enableCafImportRule", "scheduledrules" );
            DataTable DataTable = TableUpdate.getTable( "where rulename = '" + CswEnumNbtScheduleRuleNames.CAFImport + "'" );
            if( DataTable.Rows.Count > 0 )
            {
                DataTable.Rows[0]["disabled"] = CswConvert.ToDbVal( false );
                TableUpdate.update( DataTable );
            }
        }


        private static string generateCAFSql( ICswResources CswResources )
        {
            //CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;

            string ViewSql = CswScheduleLogicNbtCAFImport.generateCAFViewSQL();
            string ImportQueueSql = CswScheduleLogicNbtCAFImport.generateImportQueueTableSQL( CswResources );
            string CAFCleanupSQL = CswScheduleLogicNbtCAFImport.generateCAFCleanupSQL( CswResources );
            string TriggersSql = CswScheduleLogicNbtCAFImport.generateTriggerSQL( CswResources );

            return ViewSql + "\r\n" + ImportQueueSql + "\r\n" + CAFCleanupSQL + "\r\n" + TriggersSql;

        }//generateCAFSql()


        public static void getBindingsForDefinition( ICswResources CswResources, CswNbtImportWcf.ImportBindingsReturn Ret, string ImportDefName )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;

            Dictionary<string, DataTable> Tables = _retrieveBindings( _CswNbtResources, ImportDefName );

            //in a perfect world this would be done in a loop, but its probably not worth the time
            Ret.Data.Order = new CswNbtGrid( _CswNbtResources ).DataTableToGrid( Tables["Order"] );
            Ret.Data.Bindings = new CswNbtGrid( _CswNbtResources ).DataTableToGrid( Tables["Bindings"] );
            Ret.Data.Relationships = new CswNbtGrid( _CswNbtResources ).DataTableToGrid( Tables["Relationships"] );


        }

        private static Dictionary<string, DataTable> _retrieveBindings( CswNbtResources NbtResources, string ImportDefName )
        {

            Dictionary<string, DataTable> Ret = new Dictionary<string, DataTable>();

            foreach( string TableName in new[] { "Order", "Bindings", "Relationships" } )
            {
                string sql = " select sheetname, io.* from import_def_" + TableName + " io, import_def id " +
                             "where io.importdefid = id.importdefid " +
                             "and definitionname = :importdefname";

                CswArbitrarySelect TableSelect = new CswArbitrarySelect( NbtResources.CswResources, "ImportDefinitions" + TableName, sql );
                TableSelect.addParameter( "importdefname", ImportDefName );

                Ret[TableName] = TableSelect.getTable();
                Ret[TableName].Columns.Remove( "importdefid" );

                foreach( DataRow Row in Ret[TableName].AsEnumerable().Where( row => ( Int32.MinValue == Convert.ToInt32( row["instance"] ) ) ) )
                {
                    Row["instance"] = DBNull.Value;
                }

            }

            return Ret;
        }




    } // class CswNbtWebServiceImport

} // namespace ChemSW.Nbt.WebServices

