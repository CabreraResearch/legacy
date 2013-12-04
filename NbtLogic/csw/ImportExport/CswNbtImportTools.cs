
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Data.OleDb;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.ImportExport;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.csw.ImportExport
{
    public static class CswNbtImportTools
    {
        public static void CreateAllCAFProps( CswNbtResources NbtResources, CswEnumSetupMode SetupMode )
        {
            CreateCafProps( NbtResources, CswEnumNbtObjectClass.ChemicalClass, "properties_values", "propertiesvaluesid", SetupMode );
            CreateCafProps( NbtResources, CswEnumNbtObjectClass.ContainerClass, "properties_values_cont", "contpropsvaluesid", SetupMode );
            CreateCafProps( NbtResources, CswEnumNbtObjectClass.ContainerClass, "properties_values_lot", "lotpropsvaluesid", SetupMode );
        }

        public static void CreateCafProps( CswNbtResources NbtResources, CswEnumNbtObjectClass ObjClass, string PropsValsTblName, string PropsValsPKName, CswEnumSetupMode SetupMode )
        {
            CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( new CswNbtSchemaModTrnsctn( NbtResources ), "CAF", ImporterSetUpMode : SetupMode );

            CswNbtMetaDataObjectClass MetaDataObjClass = NbtResources.MetaData.getObjectClass( ObjClass );
            string sql = GetCAFPropertiesSQL( PropsValsTblName );
            CswArbitrarySelect cafChemPropAS = NbtResources.makeCswArbitrarySelect( "cafProps_" + ObjClass.Value, sql );
            DataTable cafChemPropsDT = cafChemPropAS.getTable();

            foreach( DataRow row in cafChemPropsDT.Rows )
            {
                foreach( CswNbtMetaDataNodeType NodeType in MetaDataObjClass.getNodeTypes() )
                {
                    string PropName = row["propertyname"].ToString();
                    int PropId = CswConvert.ToInt32( row["propertyid"] );
                    PropName = GetUniquePropName( NodeType, PropName ); //keep appending numbers until we have a unique prop name

                    CswEnumNbtFieldType propFT = GetFieldTypeFromCAFPropTypeCode( row["propertytype"].ToString() );

                    CswNbtMetaDataNodeTypeProp newProp = NbtResources.MetaData.makeNewProp( NodeType, propFT, PropName, Int32.MinValue );
                    newProp.IsRequired = CswConvert.ToBoolean( row["required"] );
                    newProp.ReadOnly = CswConvert.ToBoolean( row["readonly"] );
                    newProp.ListOptions = row["listopts"].ToString();
                    newProp.removeFromAllLayouts();

                    string cafColPropName = "prop" + row["propertyid"];
                    string cafSourceCol = "propvaltext";
                    if( CswEnumNbtFieldType.DateTime == propFT )
                    {
                        cafSourceCol = "propvaldate";
                    }
                    else if( CswEnumNbtFieldType.Number == propFT )
                    {
                        cafSourceCol = "propvalnumber";
                    }

                    ImpMgr.importBinding( cafSourceCol, PropName, "", "CAF", NodeType.NodeTypeName,
                        ClobTableName : PropsValsTblName,
                        LobDataPkColOverride : cafColPropName,
                        LobDataPkColName : PropsValsPKName,
                        LegacyPropId : PropId );
                }
            }

            NbtResources.commitTransaction();
            ImpMgr.finalize();
        }

        /// <summary>
        /// Get all the custom properties in a CAF schema
        /// </summary>
        private static string GetCAFPropertiesSQL( string propValsTblName )
        {
            string sql = @"select distinct p.propertyid,
                             p.propertyname,
                             p.propertytype,
                             p.required, 
                             p.readonly,
                             (select listopts from (select propertyid, listagg(replace(lkpitem, ',', ''), ',') within group (order by lkpitem) as listopts
                                   from properties_list_lkps@caflink
                                        group by propertyid) where propertyid = p.propertyid) listopts
                            from properties@caflink p
                                   join " + propValsTblName + @"@caflink pv on p.propertyid = pv.propertyid
                             where p.propertyid not in (select legacypropid from import_def_bindings idb
                                         join properties@caflink p on p.propertyid = idb.legacypropid)
                            order by propertyid";

            return sql;
        }

        /// <summary>
        /// Get a unique NTP name for a given NT
        /// </summary>
        private static string GetUniquePropName( CswNbtMetaDataNodeType NodeType, string PropName )
        {
            bool isUnique = true;
            int idx = 1;
            string OrigPropName = PropName;
            while( isUnique )
            {
                CswNbtMetaDataNodeTypeProp ntp = NodeType.getNodeTypeProp( PropName );
                if( null != ntp )
                {
                    PropName = OrigPropName + idx;
                    idx++;
                }
                else
                {
                    isUnique = false;
                }
            }
            return PropName;
        }

        /// <summary>
        /// Get the NBT FieldType equivalent for a CAF custom prop code
        /// </summary>
        private static CswEnumNbtFieldType GetFieldTypeFromCAFPropTypeCode( string CafPropTypeCode )
        {
            CswEnumNbtFieldType Ret = null;

            switch( CafPropTypeCode )
            {
                case "T":
                    Ret = CswEnumNbtFieldType.Text;
                    break;
                case "L":
                    Ret = CswEnumNbtFieldType.List;
                    break;
                case "M": //Multi-List
                    Ret = CswEnumNbtFieldType.MultiList;
                    break;
                case "D":
                    Ret = CswEnumNbtFieldType.DateTime;
                    break;
                case "V": //Multi-Value
                    Ret = CswEnumNbtFieldType.MultiList;
                    break;
                case "E":
                    Ret = CswEnumNbtFieldType.Memo;
                    break;
                case "O":
                    Ret = CswEnumNbtFieldType.Logical;
                    break;
                case "N":
                    Ret = CswEnumNbtFieldType.Number;
                    break;
                case "Q":
                    //Query - we don't import this prop
                    break;
            }

            return Ret;
        }

        public static DataSet ReadExcel( string FilePath )
        {
            DataSet ret = new DataSet();

            //Set up ADO connection to spreadsheet
            string ConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + FilePath + ";Extended Properties=Excel 8.0;";
            OleDbConnection ExcelConn = new OleDbConnection( ConnStr );
            ExcelConn.Open();

            DataTable ExcelMetaDataTable = ExcelConn.GetOleDbSchemaTable( OleDbSchemaGuid.Tables, null );
            if( null == ExcelMetaDataTable )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "Invalid File", "Could not process the excel file: " + FilePath );
            }

            foreach( DataRow ExcelMetaDataRow in ExcelMetaDataTable.Rows )
            {
                string SheetName = ExcelMetaDataRow["TABLE_NAME"].ToString();

                OleDbDataAdapter DataAdapter = new OleDbDataAdapter();
                OleDbCommand SelectCommand = new OleDbCommand( "SELECT * FROM [" + SheetName + "]", ExcelConn );
                DataAdapter.SelectCommand = SelectCommand;

                DataTable ExcelDataTable = new DataTable( SheetName );
                DataAdapter.Fill( ExcelDataTable );

                ret.Tables.Add( ExcelDataTable );
            }
            return ret;
        }

        /// <summary>
        /// Stores data in temporary Oracle tables
        /// </summary>
        public static Int32 storeData( CswNbtResources CswNbtResources, string FileName, string FullFilePath, string ImportDefinitionName, bool Overwrite )
        {
            CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn = new CswNbtSchemaModTrnsctn( CswNbtResources );

            //StringCollection ret = new StringCollection();
            DataSet ExcelDataSet = CswNbtImportTools.ReadExcel( FullFilePath );

            // Store the job reference in import_data_job
            CswTableUpdate ImportDataJobUpdate = CswNbtResources.makeCswTableUpdate( "Importer_DataJob_Insert", CswNbtImportTables.ImportDataJob.TableName );
            DataTable ImportDataJobTable = ImportDataJobUpdate.getEmptyTable();
            DataRow DataJobRow = ImportDataJobTable.NewRow();
            DataJobRow[CswNbtImportTables.ImportDataJob.filename] = FileName;
            DataJobRow[CswNbtImportTables.ImportDataJob.userid] = CswNbtResources.CurrentNbtUser.UserId.PrimaryKey;
            DataJobRow[CswNbtImportTables.ImportDataJob.datestarted] = CswConvert.ToDbVal( DateTime.Now );
            ImportDataJobTable.Rows.Add( DataJobRow );
            Int32 JobId = CswConvert.ToInt32( DataJobRow[CswNbtImportTables.ImportDataJob.importdatajobid] );
            ImportDataJobUpdate.update( ImportDataJobTable );

            foreach( DataTable ExcelDataTable in ExcelDataSet.Tables )
            {
                string SheetName = ExcelDataTable.TableName;

                // Determine Oracle table name
                Int32 i = 1;
                string ImportDataTableName = CswNbtImportTables.ImportDataN.TableNamePrefix + i.ToString();
                while( _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( ImportDataTableName ) )
                {
                    i++;
                    ImportDataTableName = CswNbtImportTables.ImportDataN.TableNamePrefix + i.ToString();
                }

                // Generate an Oracle table for storing and manipulating data
                _CswNbtSchemaModTrnsctn.addTable( ImportDataTableName, CswNbtImportTables.ImportDataN.PkColumnName );
                _CswNbtSchemaModTrnsctn.addBooleanColumn( ImportDataTableName, CswNbtImportTables.ImportDataN.error, "", false );
                _CswNbtSchemaModTrnsctn.addClobColumn( ImportDataTableName, CswNbtImportTables.ImportDataN.errorlog, "", false );
                foreach( DataColumn ExcelColumn in ExcelDataTable.Columns )
                {
                    _CswNbtSchemaModTrnsctn.addStringColumn( ImportDataTableName, CswNbtImportDefBinding.SafeColName( ExcelColumn.ColumnName ), "", false, 4000 );
                }
                CswNbtImportDef Definition = new CswNbtImportDef( CswNbtResources, ImportDefinitionName, SheetName );
                foreach( CswNbtImportDefOrder Order in Definition.ImportOrder.Values )
                {
                    _CswNbtSchemaModTrnsctn.addLongColumn( ImportDataTableName, Order.PkColName, "", false );
                }
                CswNbtResources.commitTransaction();
                CswNbtResources.beginTransaction();

                //ret.Add( ImportDataTableName );

                // Store the sheet reference in import_data_map
                CswTableUpdate ImportDataMapUpdate = CswNbtResources.makeCswTableUpdate( "Importer_DataMap_Insert", CswNbtImportTables.ImportDataMap.TableName );
                DataTable ImportDataMapTable = ImportDataMapUpdate.getEmptyTable();
                DataRow DataMapRow = ImportDataMapTable.NewRow();
                DataMapRow[CswNbtImportTables.ImportDataMap.datatablename] = ImportDataTableName;
                DataMapRow[CswNbtImportTables.ImportDataMap.importdefid] = Definition.ImportDefinitionId;
                DataMapRow[CswNbtImportTables.ImportDataMap.importdatajobid] = JobId;
                DataMapRow[CswNbtImportTables.ImportDataMap.overwrite] = CswConvert.ToDbVal( Overwrite );
                DataMapRow[CswNbtImportTables.ImportDataMap.completed] = CswConvert.ToDbVal( false );
                ImportDataMapTable.Rows.Add( DataMapRow );
                ImportDataMapUpdate.update( ImportDataMapTable );

                // Copy the Excel data into the Oracle table
                CswTableUpdate ImportDataUpdate = CswNbtResources.makeCswTableUpdate( "Importer_Update", ImportDataTableName );
                DataTable ImportDataTable = ImportDataUpdate.getEmptyTable();
                foreach( DataRow ExcelRow in ExcelDataTable.Rows )
                {
                    bool hasData = false;
                    DataRow ImportRow = ImportDataTable.NewRow();
                    ImportRow[CswNbtImportTables.ImportDataN.error] = CswConvert.ToDbVal( false );
                    foreach( DataColumn ExcelColumn in ExcelDataTable.Columns )
                    {
                        if( ExcelRow[ExcelColumn] != DBNull.Value )
                        {
                            hasData = true;
                        }
                        ImportRow[CswNbtImportDefBinding.SafeColName( ExcelColumn.ColumnName )] = ExcelRow[ExcelColumn];
                    }
                    if( hasData == true )
                    {
                        ImportDataTable.Rows.Add( ImportRow );
                    }
                }
                ImportDataUpdate.update( ImportDataTable );

                //OnMessage( "Sheet '" + SheetName + "' is stored in Table '" + ImportDataTableName + "'" );
            } // foreach( DataTable ExcelDataTable in ExcelDataSet.Tables )

            CswNbtResources.commitTransaction();
            CswNbtResources.beginTransaction();

            //return ret;
            return JobId;
        }

        /// <summary>
        /// Returns the set of available Import Definition Names
        /// </summary>
        public static CswCommaDelimitedString getDefinitionNames( CswNbtResources CswNbtResources )
        {
            CswCommaDelimitedString ret = new CswCommaDelimitedString();
            //CswTableSelect DefSelect = _CswNbtResources.makeCswTableSelect( "loadBindings_def_select1", CswNbtImportTables.ImportDef.TableName );
            string Sql = @"select distinct(" + CswNbtImportTables.ImportDef.definitionname + ") from " + CswNbtImportTables.ImportDef.TableName + "";
            CswArbitrarySelect DefSelect = CswNbtResources.makeCswArbitrarySelect( "loadBindings_def_select1", Sql );
            DataTable DefDataTable = DefSelect.getTable();
            foreach( DataRow defrow in DefDataTable.Rows )
            {
                ret.Add( defrow[CswNbtImportTables.ImportDef.definitionname].ToString(), false, true );
            }
            return ret;
        }

        // getDefinitions()

        /// <summary>
        /// Returns the set of available Import Jobs
        /// </summary>
        public static Collection<CswNbtImportDataJob> getJobs( CswNbtResources CswNbtResources )
        {
            Collection<CswNbtImportDataJob> ret = new Collection<CswNbtImportDataJob>();
            CswTableSelect JobSelect = CswNbtResources.makeCswTableSelect( "getJobs_select", CswNbtImportTables.ImportDataJob.TableName );
            DataTable JobDataTable = JobSelect.getTable();
            foreach( DataRow jobrow in JobDataTable.Rows )
            {
                ret.Add( new CswNbtImportDataJob( CswNbtResources, jobrow ) );
            }
            return ret;
        }

        /// <summary>
        /// Returns import data table names, in import order
        /// </summary>
        /// <param name="IncludeCompleted">If true, also include table names for already completed imports</param>
        public static StringCollection getImportDataTableNames( CswNbtResources CswNbtResources, bool IncludeCompleted = false )
        {
            StringCollection ret = new StringCollection();

            string Sql = @"select m." + CswNbtImportTables.ImportDataMap.datatablename +
                         " from " + CswNbtImportTables.ImportDataMap.TableName + " m " +
                         " join " + CswNbtImportTables.ImportDataJob.TableName + " j on m." + CswNbtImportTables.ImportDataMap.importdatajobid + " = j." + CswNbtImportTables.ImportDataJob.importdatajobid +
                         " join " + CswNbtImportTables.ImportDef.TableName + " d on m." + CswNbtImportTables.ImportDataMap.importdefid + " = d." + CswNbtImportTables.ImportDef.importdefid;
            if( false == IncludeCompleted )
            {
                Sql += "  where " + CswNbtImportTables.ImportDataJob.dateended + " is null";
                Sql += "    and m." + CswNbtImportTables.ImportDataMap.completed + " = '" + CswConvert.ToDbVal( false ) + "'";
            }
            Sql += @"     order by j." + CswNbtImportTables.ImportDataJob.datestarted + ", d." + CswNbtImportTables.ImportDef.sheetorder + ", m." + CswNbtImportTables.ImportDataMap.PkColumnName;

            CswArbitrarySelect ImportDataSelect = CswNbtResources.makeCswArbitrarySelect( "getImportDataTableNames_Select", Sql );
            DataTable ImportDataTable = ImportDataSelect.getTable();
            foreach( DataRow Row in ImportDataTable.Rows )
            {
                ret.Add( CswConvert.ToString( Row[CswNbtImportTables.ImportDataMap.datatablename] ) );
            }

            return ret;
        } // getImportDataTableNames()

        public static void CancelJob( CswNbtResources CswNbtResources, Int32 JobId )
        {
            // Set Job's dateended to now
            CswNbtImportDataJob Job = new CswNbtImportDataJob( CswNbtResources, JobId );
            Job.DateEnded = DateTime.Now;

            // Set all DataMaps to Completed=1
            foreach( CswNbtImportDataMap DataMap in Job.Maps )
            {
                DataMap.Completed = true;
            }
        } // CancelJob()
    }
}
