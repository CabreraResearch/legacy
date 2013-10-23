using System;
using System.Collections.Generic;
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

            Dictionary<string, DataTable> Tables = _retrieveBindings( _CswNbtResources, ImportDefName );

            //in a perfect world this would be done in a loop, but its probably not worth the time
               Ret.Data.Order = new CswNbtGrid( _CswNbtResources ).DataTableToGrid( Tables["Order"] );
               Ret.Data.Bindings = new CswNbtGrid( _CswNbtResources ).DataTableToGrid( Tables["Bindings"] );
               Ret.Data.Relationships = new CswNbtGrid( _CswNbtResources ).DataTableToGrid( Tables["Relationships"] );


        }

        private static Dictionary<string, DataTable> _retrieveBindings( CswNbtResources NbtResources, string ImportDefName )
        {

            Dictionary<string, DataTable> Ret = new Dictionary<string, DataTable>();

            foreach( string TableName in new[] {"Order", "Bindings", "Relationships"} )
            {
                string sql = " select sheetname, io.* from import_def_" + TableName + " io, import_def id " +
                             "where io.importdefid = id.importdefid " +
                             "and definitionname = :importdefname";

                CswArbitrarySelect TableSelect = new CswArbitrarySelect( NbtResources.CswResources, "ImportDefinitions"+TableName, sql );
                TableSelect.addParameter( "importdefname", ImportDefName );

                Ret[TableName] = TableSelect.getTable();
                Ret[TableName].Columns.Remove( "importdefid" );
                Ret[TableName].Columns.Remove( "importdef" + TableName.TrimEnd( new[]{'s'} ) + "id" );
            }

            return Ret;
        } 



    } // class CswNbtWebServiceImport

} // namespace ChemSW.Nbt.WebServices

