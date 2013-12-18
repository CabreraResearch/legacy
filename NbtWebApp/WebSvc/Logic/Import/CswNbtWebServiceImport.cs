using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Schema;
using ChemSW.Nbt.Security;
using ChemSW.Security;
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
            Importer.Finish();
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
            CswNbtSchemaUpdateImportMgr ImportUpdater = new CswNbtSchemaUpdateImportMgr( new CswNbtSchemaModTrnsctn( (CswNbtResources) CswResources ), "CAF", ImporterSetUpMode: CswEnumSetupMode.NbtWeb );

            foreach( CswNbtImportWcf.DefinitionUpdateRow Row in Params )
            {
                ImportUpdater.updateDefinitionElementByPK( Row.definitionType, Row.editMode, Row.row );
            }

            ImportUpdater.finalize();
        }


        public static void startCAFImport( ICswResources CswResources, CswWebSvcReturn Ret, CswNbtImportWcf.StartImportParams Params )
        {
            CswNbtImportTools.startCAFImportImpl( CswResources, Params.CAFDatabase, Params.CAFSchema, Params.CAFPassword, CswEnumSetupMode.NbtWeb );
        }




        public static void getBindingsForDefinition( ICswResources CswResources, CswNbtImportWcf.ImportBindingsReturn Ret, string ImportDefName )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;

            Dictionary<string, DataTable> Tables = _retrieveBindings( _CswNbtResources, ImportDefName );

            //in a perfect world this would be done in a loop, but its probably not worth the time
            Ret.Data.Order = new CswNbtGrid( _CswNbtResources ).DataTableToGrid( Tables["Order"] );
            Ret.Data.Bindings = new CswNbtGrid( _CswNbtResources ).DataTableToGrid( Tables["Bindings"] );
            Ret.Data.Relationships = new CswNbtGrid( _CswNbtResources ).DataTableToGrid( Tables["Relationships"] );


        }

        public static void getExistingNodes( ICswResources CswResources, CswNbtImportWcf.DltExistingNodesReturn Ret, object EmptyObject )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;

            Ret.Data = new CswNbtImportWcf.DltExistingNodesReturn.DltExistingNodesReturnData();
            Ret.Data.NodesToDelete = _retriveDoomedNodes( _CswNbtResources );
        }//getExistingNodes()

        public static void deleteExistingNodes( ICswResources CswResources, CswNbtImportWcf.DltExistingNodesReturn Ret, object EmptyObject )
        {
            // In this case, we need to create a new instance of CswNbtResources
            // so that we can delete all nodes (including MLM nodes)
            CswNbtResources _CswNbtResources = CswNbtResourcesFactory.makeCswNbtResources( CswEnumAppType.Nbt, CswEnumSetupMode.NbtWeb, false ); //ExcludeDisabledModules needs to be false
            _CswNbtResources.AccessId = CswResources.AccessId;
            _CswNbtResources.InitCurrentUser = _initUser;

            Collection<CswNbtImportWcf.DltExistingNodesReturn.DltExistingNodesReturnData.DoomedNode> DoomedNodes = _retriveDoomedNodes( _CswNbtResources );
            foreach( CswNbtImportWcf.DltExistingNodesReturn.DltExistingNodesReturnData.DoomedNode DoomedNode in DoomedNodes )
            {
                try
                {
                    CswPrimaryKey NodePrimeKey = new CswPrimaryKey();
                    NodePrimeKey.FromString( "nodes_" + DoomedNode.NodeId );
                    CswNbtNode CurrentNode = _CswNbtResources.Nodes[NodePrimeKey];
                    if( null != CurrentNode )
                    {
                        CurrentNode.delete( true, true, false );
                    }

                }
                catch( Exception exception )
                {
                    throw new CswDniException( CswEnumErrorType.Error, "Error deleting node: ", exception.Message );
                }
            }

            _CswNbtResources.finalize();
            Ret.Data.DeleteSuccessful = true;
        }

        #region Private helper methods

        private static ICswNbtUser _initUser( ICswResources CswResources )
        {
            return new CswNbtSystemUser( CswResources, CswEnumSystemUserNames.SysUsr__SchemaImport );
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

        private static Collection<CswNbtImportWcf.DltExistingNodesReturn.DltExistingNodesReturnData.DoomedNode> _retriveDoomedNodes( CswNbtResources NbtResources )
        {
            Collection<CswNbtImportWcf.DltExistingNodesReturn.DltExistingNodesReturnData.DoomedNode> DoomedNodes = new Collection<CswNbtImportWcf.DltExistingNodesReturn.DltExistingNodesReturnData.DoomedNode>();

            CswArbitrarySelect NodeTblSelect = NbtResources.makeCswArbitrarySelect( "getDoomedNodes_nbtImporter",
                            @"select t.nodeid, t.nodename, t.nodetypeid, nt.nodetypename
                                from nodes t
                                join nodetypes nt on (nt.nodetypeid = t.nodetypeid)
                                where nodename not in ('chemsw_admin', 'chemsw_admin_role', 'Each', 'Days', 'Ci', 'kg',
                                    'Liters', 'lb', 'gal', 'cu.ft.', 'CISPro_Admin',
                                    'CISPro_General', 'Default Jurisdiction')
                                and t.nodetypeid not in
                                    (1130, 1212, 1329, 1330, 1369, 33, 114, 659, 1052, 1053, 1290, 1291, 1292, 1293, 1211)" );

            DataTable NodesTbl = NodeTblSelect.getTable();
            foreach( DataRow Row in NodesTbl.Rows )
            {
                DoomedNodes.Add( new CswNbtImportWcf.DltExistingNodesReturn.DltExistingNodesReturnData.DoomedNode
                    {
                        NodeId = CswConvert.ToInt32( Row["nodeid"] ),
                        NodeName = CswConvert.ToString( Row["nodename"] ),
                        NodeType = CswConvert.ToString( Row["nodetypename"] )
                    } );
            }

            return DoomedNodes;
        }//_retriveDoomedNodes()

        #endregion Private helper methods

    } // class CswNbtWebServiceImport

} // namespace ChemSW.Nbt.WebServices

