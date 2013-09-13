using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.ImportExport
{
    public partial class CswNbtImporter
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswNbtImporter( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtSchemaModTrnsctn = new CswNbtSchemaModTrnsctn( _CswNbtResources );
        }

        public delegate void MessageHandler( string Message );

        public MessageHandler OnMessage = delegate( string Message ) { };

        private DataSet _readExcel( string FilePath )
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

        // _readExcel()


        /// <summary>
        /// Stores new import definition
        /// </summary>
        public void storeDefinition( string FullFilePath, string ImportDefinitionName )
        {
            DataSet ExcelDataSet = _readExcel( FullFilePath );

            if( ExcelDataSet.Tables.Count == 3 )
            {
                DataTable OrderDataTable = ExcelDataSet.Tables["Order$"];
                DataTable BindingsDataTable = ExcelDataSet.Tables["Bindings$"];
                DataTable RelationshipsDataTable = ExcelDataSet.Tables["Relationships$"];

                storeDefinition( OrderDataTable, BindingsDataTable, RelationshipsDataTable, ImportDefinitionName );
            } // if( ExcelDataSet.Tables.Count == 3 )
            else
            {
                throw new CswDniException( CswEnumErrorType.Error, "Error reading file", "3 sheets not found in uploaded spreadsheet" );
            }
        }

        public void storeDefinition( DataTable OrderDataTable, DataTable BindingsDataTable, DataTable RelationshipsDataTable, string ImportDefinitionName, DataTable DefDataTable = null )
        {
            Dictionary<string, Int32> DefIdsBySheetName = CswNbtImportDef.addDefinitionEntries( _CswNbtResources, ImportDefinitionName, OrderDataTable, DefDataTable );
            CswNbtImportDefOrder.addOrderEntries( _CswNbtResources, OrderDataTable, DefIdsBySheetName );
            CswNbtImportDefBinding.addBindingEntries( _CswNbtResources, BindingsDataTable, DefIdsBySheetName );
            CswNbtImportDefRelationship.addRelationshipEntries( _CswNbtResources, RelationshipsDataTable, DefIdsBySheetName );

        }

        // storeDefinition()


        /// <summary>
        /// Stores data in temporary Oracle tables
        /// </summary>
        public Int32 storeData( string FileName, string FullFilePath, string ImportDefinitionName, bool Overwrite )
        {
            //StringCollection ret = new StringCollection();
            DataSet ExcelDataSet = _readExcel( FullFilePath );

            // Store the job reference in import_data_job
            CswTableUpdate ImportDataJobUpdate = _CswNbtResources.makeCswTableUpdate( "Importer_DataJob_Insert", CswNbtImportTables.ImportDataJob.TableName );
            DataTable ImportDataJobTable = ImportDataJobUpdate.getEmptyTable();
            DataRow DataJobRow = ImportDataJobTable.NewRow();
            DataJobRow[CswNbtImportTables.ImportDataJob.filename] = FileName;
            DataJobRow[CswNbtImportTables.ImportDataJob.userid] = _CswNbtResources.CurrentNbtUser.UserId.PrimaryKey;
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
                _CswNbtSchemaModTrnsctn.addBooleanColumn( ImportDataTableName, CswNbtImportTables.ImportDataN.error, "", false, false );
                _CswNbtSchemaModTrnsctn.addClobColumn( ImportDataTableName, CswNbtImportTables.ImportDataN.errorlog, "", false, false );
                foreach( DataColumn ExcelColumn in ExcelDataTable.Columns )
                {
                    _CswNbtSchemaModTrnsctn.addStringColumn( ImportDataTableName, CswNbtImportDefBinding.SafeColName( ExcelColumn.ColumnName ), "", false, false, 4000 );
                }
                CswNbtImportDef Definition = new CswNbtImportDef( _CswNbtResources, ImportDefinitionName, SheetName );
                foreach( CswNbtImportDefOrder Order in Definition.ImportOrder.Values )
                {
                    _CswNbtSchemaModTrnsctn.addLongColumn( ImportDataTableName, Order.PkColName, "", false, false );
                }
                _CswNbtResources.commitTransaction();
                _CswNbtResources.beginTransaction();

                //ret.Add( ImportDataTableName );

                // Store the sheet reference in import_data_map
                CswTableUpdate ImportDataMapUpdate = _CswNbtResources.makeCswTableUpdate( "Importer_DataMap_Insert", CswNbtImportTables.ImportDataMap.TableName );
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
                CswTableUpdate ImportDataUpdate = _CswNbtResources.makeCswTableUpdate( "Importer_Update", ImportDataTableName );
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

                OnMessage( "Sheet '" + SheetName + "' is stored in Table '" + ImportDataTableName + "'" );
            } // foreach( DataTable ExcelDataTable in ExcelDataSet.Tables )

            _CswNbtResources.commitTransaction();
            _CswNbtResources.beginTransaction();

            //return ret;
            return JobId;
        }

        // storeData()

        /// <summary>
        /// Returns the set of available Import Definition Names
        /// </summary>
        public CswCommaDelimitedString getDefinitionNames()
        {
            CswCommaDelimitedString ret = new CswCommaDelimitedString();
            //CswTableSelect DefSelect = _CswNbtResources.makeCswTableSelect( "loadBindings_def_select1", CswNbtImportTables.ImportDef.TableName );
            string Sql = @"select distinct(" + CswNbtImportTables.ImportDef.definitionname + ") from " + CswNbtImportTables.ImportDef.TableName + "";
            CswArbitrarySelect DefSelect = _CswNbtResources.makeCswArbitrarySelect( "loadBindings_def_select1", Sql );
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
        public Collection<CswNbtImportDataJob> getJobs()
        {
            Collection<CswNbtImportDataJob> ret = new Collection<CswNbtImportDataJob>();
            CswTableSelect JobSelect = _CswNbtResources.makeCswTableSelect( "getJobs_select", CswNbtImportTables.ImportDataJob.TableName );
            DataTable JobDataTable = JobSelect.getTable();
            foreach( DataRow jobrow in JobDataTable.Rows )
            {
                ret.Add( new CswNbtImportDataJob( _CswNbtResources, jobrow ) );
            }
            return ret;
        }

        // getJobs()



        /// <summary>
        /// Returns import data table names, in import order
        /// </summary>
        /// <param name="IncludeCompleted">If true, also include table names for already completed imports</param>
        public StringCollection getImportDataTableNames( bool IncludeCompleted = false )
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

            CswArbitrarySelect ImportDataSelect = _CswNbtResources.makeCswArbitrarySelect( "getImportDataTableNames_Select", Sql );
            DataTable ImportDataTable = ImportDataSelect.getTable();
            foreach( DataRow Row in ImportDataTable.Rows )
            {
                ret.Add( CswConvert.ToString( Row[CswNbtImportTables.ImportDataMap.datatablename] ) );
            }

            return ret;
        } // getImportDataTableNames()

        /// <summary>
        /// Import a single row (for CAF imports)
        /// </summary>
        /// <returns>Non-null string indicates an error</returns>
        public string ImportRow( DataRow SourceRow, string ImportDefinitionName, string SheetName, bool Overwrite )
        {
            string Error = string.Empty;

            if( false == string.IsNullOrEmpty( ImportDefinitionName ) &&
                false == string.IsNullOrEmpty( SheetName ) )
            {
                CswNbtImportDef BindingDef = new CswNbtImportDef( _CswNbtResources, ImportDefinitionName, SheetName );
                if( null != BindingDef && ( BindingDef.Bindings.Count > 0 || BindingDef.RowRelationships.Count > 0 ) && BindingDef.ImportOrder.Count > 0 )
                {
                    foreach( CswNbtImportDefOrder Order in BindingDef.ImportOrder.Values )
                    {
                        _ImportOneRow( SourceRow, BindingDef, Order, Overwrite, null );
                    }
                } // if( Bindings.Count > 0 && ImportOrder.count > 0)
                else
                {
                    Error = "No Bindings defined";
                }
            } // if( _CswNbtResources.isTableDefinedInDataBase(ImportDataTableName) ) 
            else
            {
                Error = "Null Definition Name or Sheet Name";
            }
            return Error;
        } // ImportRow()

        /// <summary>
        /// Import a number of rows
        /// </summary>
        /// <param name="RowsToImport">Number of rows to import</param>
        /// <param name="ImportDataTableName">Source Oracle table to import</param>
        /// <param name="RowsImported">(out) Number of rows processed in this execution</param>
        /// <returns>True if there are more rows to import from this source data, false otherwise</returns>
        public bool ImportRows( Int32 RowsToImport, string ImportDataTableName, out Int32 RowsImported )
        {
            RowsImported = 0;
            if( false == string.IsNullOrEmpty( ImportDataTableName ) && _CswNbtResources.isTableDefinedInDataBase( ImportDataTableName ) )
            {
                // Lookup the binding definition
                CswNbtImportDataMap DataMap = new CswNbtImportDataMap( _CswNbtResources, ImportDataTableName );
                CswNbtImportDef BindingDef = new CswNbtImportDef( _CswNbtResources, DataMap.ImportDefinitionId );

                if( null != BindingDef && ( BindingDef.Bindings.Count > 0 || BindingDef.RowRelationships.Count > 0 ) && BindingDef.ImportOrder.Count > 0 ) //dch
                {
                    foreach( CswNbtImportDefOrder Order in BindingDef.ImportOrder.Values )
                    {
                        CswTableUpdate ImportDataUpdate = _CswNbtResources.makeCswTableUpdate( "Importer_Update", ImportDataTableName );

                        // Fetch the next row to process
                        DataTable ImportDataTable;
                        do
                        {
                            ImportDataTable = ImportDataUpdate.getTable( "where " + CswNbtImportTables.ImportDataN.error + " = '" + CswConvert.ToDbVal( false ) + "' and " + Order.PkColName + " is null",
                                                                         new Collection<OrderByClause> { new OrderByClause( CswNbtImportTables.ImportDataN.importdataid, CswEnumOrderByType.Ascending ) },
                                                                         0, 1 );
                            if( ImportDataTable.Rows.Count > 0 )
                            {
                                DataRow Row = ImportDataTable.Rows[0];
                                string msgPrefix = Order.NodeType.NodeTypeName + " Import (" + Row[CswNbtImportTables.ImportDataN.importdataid].ToString() + "): ";
                                try
                                {
                                    CswPrimaryKey ImportedNodeId = _ImportOneRow( Row, BindingDef, Order, DataMap.Overwrite, ImportDataUpdate );

                                    // Save the nodeid in this row
                                    Row[Order.PkColName] = CswConvert.ToDbVal( ImportedNodeId.PrimaryKey );
                                    ImportDataUpdate.update( ImportDataTable );

                                }
                                catch( Exception ex )
                                {
                                    // Swallow and store the error on the row
                                    string ErrorMsg = msgPrefix + ex.Message; //+ "\r\n" + ex.StackTrace;
                                    Row[CswNbtImportTables.ImportDataN.error] = CswConvert.ToDbVal( true );
                                    Row[CswNbtImportTables.ImportDataN.errorlog] = ErrorMsg;
                                    ImportDataUpdate.update( ImportDataTable );
                                }

                                RowsImported += 1;
                            }

                        } while( ImportDataTable.Rows.Count > 0 && RowsImported < RowsToImport );

                        if( RowsImported >= RowsToImport )
                        {
                            break;
                        }
                    } // foreach( CswNbtMetaDataNodeType NodeType in _ImportOrder.Values )

                    if( RowsImported == 0 )
                    {
                        DataMap.Completed = true;

                        // If all datamaps are completed, set dateended on job
                        CswNbtImportDataJob Job = new CswNbtImportDataJob( _CswNbtResources, DataMap.ImportDataJobId );
                        if( null == Job.Maps.FirstOrDefault( map => map.Completed == false ) )
                        {
                            Job.DateEnded = DateTime.Now;
                        }
                    }

                } // if( Bindings.Count > 0 && ImportOrder.count > 0)
                else
                {
                    throw new Exception( "No Bindings or Order defined" );
                }
            } // if( _CswNbtResources.isTableDefinedInDataBase(ImportDataTableName) ) 
            else
            {
                throw new Exception( "Invalid Source Table: " + ImportDataTableName );
            }

            return ( RowsImported != 0 );
        } // ImportRows()

        /// <summary>
        /// Import a single node for a single row
        /// </summary>
        /// <returns>NodeId of imported node</returns>
        private CswPrimaryKey _ImportOneRow( DataRow ImportRow, CswNbtImportDef BindingDef, CswNbtImportDefOrder Order, bool Overwrite, CswTableUpdate ImportDataUpdate )
        {
            CswPrimaryKey ImportedNodeId = null;
            //string msgPrefix = Order.NodeType.NodeTypeName + " Import (" + ImportRow[CswNbtImportTables.ImportDataN.importdataid].ToString() + "): ";

            CswNbtNode Node = null;

            IEnumerable<CswNbtImportDefBinding> NodeTypeBindings = BindingDef.Bindings.Where(
                delegate( CswNbtImportDefBinding b )
                {
                    return b.DestNodeType == Order.NodeType && b.Instance == Order.Instance;
                } );
            IEnumerable<CswNbtImportDefRelationship> RowRelationships = BindingDef.RowRelationships.Where( r => r.NodeType.NodeTypeId == Order.NodeType.NodeTypeId ); //&& r.Instance == Order.Instance );
            IEnumerable<CswNbtImportDefRelationship> UniqueRelationships = RowRelationships.Where( r => r.Relationship.IsUnique() ||
                                                                                                        r.Relationship.IsCompoundUnique() ||
                                                                                                        Order.NodeType.NameTemplatePropIds.Contains( r.Relationship.FirstPropVersionId ) );

            IEnumerable<CswNbtImportDefBinding> UniqueBindings = NodeTypeBindings.Where( b => b.DestProperty.IsUnique() ||
                                                                                              b.DestProperty.IsCompoundUnique() ||
                                                                                              Order.NodeType.NameTemplatePropIds.Contains( b.DestProperty.FirstPropVersionId ) );

            bool allEmpty = true;
            // Skip rows with null values for all unique properties

            foreach( CswNbtImportDefBinding Binding in UniqueBindings )
            {
                allEmpty = allEmpty && string.IsNullOrEmpty( ImportRow[Binding.ImportDataColumnName].ToString() );
            }
            foreach( CswNbtImportDefRelationship Relation in UniqueRelationships )
            {
                CswNbtImportDefOrder thisTargetOrder = BindingDef.ImportOrder.Values.FirstOrDefault( o => Relation.Relationship.FkMatches( o.NodeType ) && o.Instance == Relation.Instance );
                Int32 Value = Int32.MinValue;
                Value = null != ImportRow[Relation.SourceRelColumnName] ? CswConvert.ToInt32( ImportRow[Relation.SourceRelColumnName] ) : CswConvert.ToInt32( ImportRow[thisTargetOrder.PkColName] );

                allEmpty = allEmpty && ( Value != Int32.MinValue );
            }

            string LegacyId = string.Empty;
            foreach( CswNbtImportDefBinding Binding in NodeTypeBindings )
            {
                if( Binding.DestPropName == "Legacy ID" )
                {
                    LegacyId = ImportRow[Binding.ImportDataColumnName].ToString();
                }
            }

            if( string.IsNullOrEmpty( LegacyId ) && BindingDef.DefinitionName == "CAF" )
            {
                allEmpty = true;
            }

            if( false == allEmpty )
            {
                //Check for matching nodes using a view on legacy id
                CswNbtView LegacyIdView = new CswNbtView( _CswNbtResources );
                LegacyIdView.ViewName = "Check Legacy Id";
                CswNbtViewRelationship NTRel1 = LegacyIdView.AddViewRelationship( Order.NodeType, false );

                CswNbtMetaDataNodeTypeProp LegacyIdNTP = Order.NodeType.getNodeTypeProp( "Legacy Id" );
                LegacyIdView.AddViewPropertyAndFilter( ParentViewRelationship: NTRel1, MetaDataProp: LegacyIdNTP,
                                                      Value: LegacyId,
                                                      SubFieldName: CswEnumNbtSubFieldName.Value, CaseSensitive: false );

                ICswNbtTree LegacyIdTree = _CswNbtResources.Trees.getTreeFromView( LegacyIdView, false, true, true );
                if( LegacyIdTree.getChildNodeCount() > 0 )
                {
                    LegacyIdTree.goToNthChild( 0 );
                    Node = LegacyIdTree.getNodeForCurrentPosition();
                    if( Overwrite )
                    {
                        _importPropertyValues( BindingDef, NodeTypeBindings, RowRelationships, ImportRow, Node );
                        Node.postChanges( false );
                    }
                }
                else
                {

                    // Find matching nodes using a view on unique properties
                    CswNbtView UniqueView = new CswNbtView( _CswNbtResources );
                    UniqueView.ViewName = "Check Unique";
                    CswNbtViewRelationship NTRel = UniqueView.AddViewRelationship( Order.NodeType, false );

                    if( UniqueBindings.Any() )
                    {
                        bool atLeastOneFilter = false;
                        foreach( CswNbtImportDefBinding Binding in UniqueBindings )
                        {
                            string Value = ImportRow[Binding.ImportDataColumnName].ToString();
                            if( Value != string.Empty )
                            {
                                UniqueView.AddViewPropertyAndFilter( NTRel, Binding.DestProperty,
                                                                    Conjunction: CswEnumNbtFilterConjunction.And,
                                                                    SubFieldName: Binding.DestSubfield.Name,
                                                                    FilterMode: CswEnumNbtFilterMode.Equals,
                                                                    Value: Value,
                                                                    CaseSensitive: false );
                            }
                            else
                            {
                                UniqueView.AddViewPropertyAndFilter( NTRel, Binding.DestProperty,
                                                                    Conjunction: CswEnumNbtFilterConjunction.And,
                                                                    SubFieldName: Binding.DestSubfield.Name,
                                                                    FilterMode: CswEnumNbtFilterMode.Null );
                            }
                            atLeastOneFilter = true;
                        }

                        foreach( CswNbtImportDefRelationship Relation in UniqueRelationships )
                        {
                            CswNbtImportDefOrder thisTargetOrder =
                                BindingDef.ImportOrder.Values.FirstOrDefault(
                                    o => Relation.Relationship.FkMatches( o.NodeType ) && o.Instance == Relation.Instance );
                            Int32 Value = null != ImportRow[Relation.SourceRelColumnName]
                                              ? CswConvert.ToInt32( ImportRow[Relation.SourceRelColumnName] )
                                              : CswConvert.ToInt32( ImportRow[thisTargetOrder.PkColName] );

                            if( Value != Int32.MinValue )
                            {
                                UniqueView.AddViewPropertyAndFilter( NTRel, Relation.Relationship,
                                                                    Conjunction: CswEnumNbtFilterConjunction.And,
                                                                    SubFieldName: CswEnumNbtSubFieldName.NodeID,
                                                                    FilterMode: CswEnumNbtFilterMode.Equals,
                                                                    Value: Value.ToString(),
                                                                    CaseSensitive: false );
                            }
                            else
                            {
                                UniqueView.AddViewPropertyAndFilter( NTRel, Relation.Relationship,
                                                                    Conjunction: CswEnumNbtFilterConjunction.And,
                                                                    SubFieldName: CswEnumNbtSubFieldName.NodeID,
                                                                    FilterMode: CswEnumNbtFilterMode.Null );
                            }
                            atLeastOneFilter = true;
                        }

                        if( atLeastOneFilter )
                        {
                            ICswNbtTree UniqueTree = _CswNbtResources.Trees.getTreeFromView( UniqueView, false, true,
                                                                                            true );
                            if( UniqueTree.getChildNodeCount() > 0 )
                            {
                                UniqueTree.goToNthChild( 0 );
                                Node = UniqueTree.getNodeForCurrentPosition();
                                if( Overwrite )
                                {
                                    _importPropertyValues( BindingDef, NodeTypeBindings, RowRelationships, ImportRow,
                                                          Node );
                                    Node.postChanges( false );
                                }
                            }
                        }
                    } // if( UniqueProps.Any() )
                }

                if( null == Node )
                {
                    // Make a new node
                    //Node = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( Order.NodeType.NodeTypeId, CswEnumNbtMakeNodeOperation.WriteNode );
                    //need to get around blank conversion factors for UOM nodes not yet filled in here
                    Node = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( Order.NodeType.NodeTypeId, delegate( CswNbtNode NewNode )
                        {
                            _importPropertyValues( BindingDef, NodeTypeBindings, RowRelationships, ImportRow, NewNode );
                        } );
                }

                ImportedNodeId = Node.NodeId;


                // Simplify future imports by saving this nodeid on matching rows
                if( UniqueBindings.Any() && null != ImportDataUpdate )
                {
                    // We have to check for repeats amongst all instances
                    IEnumerable<CswNbtImportDefOrder> AllInstanceNodeTypeOrders = BindingDef.ImportOrder.Values.Where( o => o.NodeType == Order.NodeType );
                    foreach( CswNbtImportDefOrder OtherOrder in AllInstanceNodeTypeOrders )
                    {
                        string WhereClause = "where " + CswNbtImportTables.ImportDataN.error + " = '" + CswConvert.ToDbVal( false ) + "' and " + OtherOrder.PkColName + " is null";
                        foreach( CswNbtImportDefBinding UniqueBinding in UniqueBindings )
                        {
                            CswNbtImportDefBinding OtherUniqueBinding = BindingDef.Bindings.byProp( OtherOrder.Instance, UniqueBinding.DestProperty, UniqueBinding.DestSubfield ).FirstOrDefault();
                            if( null != OtherUniqueBinding )
                            {
                                WhereClause += " and lower(" + OtherUniqueBinding.ImportDataColumnName + ") = '" + CswTools.SafeSqlParam( ImportRow[UniqueBinding.ImportDataColumnName].ToString().ToLower() ) + "' ";
                            }
                        }

                        foreach( CswNbtImportDefRelationship Relation in UniqueRelationships )
                        {
                            CswNbtImportDefOrder thisTargetOrder = BindingDef.ImportOrder.Values.FirstOrDefault( o => Relation.Relationship.FkMatches( o.NodeType ) && o.Instance == Relation.Instance );
                            //Int32 Value = CswConvert.ToInt32( ImportRow[thisTargetOrder.PkColName] );
                            //Int32 Value = CswConvert.ToInt32( false == string.IsNullOrEmpty( ImportRow[Relation.SourceRelColumnName].ToString() ) ? ImportRow[Relation.SourceRelColumnName] : ImportRow[thisTargetOrder.PkColName] );
                            Int32 Value = null != ImportRow[Relation.SourceRelColumnName] ? CswConvert.ToInt32( ImportRow[Relation.SourceRelColumnName] ) : CswConvert.ToInt32( ImportRow[thisTargetOrder.PkColName] );

                            if( Value != Int32.MinValue )
                            {
                                WhereClause += " and " + thisTargetOrder.PkColName + "=" + Value.ToString();
                            }
                        }

                        DataTable OtherImportDataTable = ImportDataUpdate.getTable( WhereClause );
                        foreach( DataRow OtherImportRow in OtherImportDataTable.Rows )
                        {
                            OtherImportRow[OtherOrder.PkColName] = CswConvert.ToDbVal( Node.NodeId.PrimaryKey );
                        }
                        ImportDataUpdate.update( OtherImportDataTable );
                    } // foreach( CswNbt2DOrder OtherOrder in AllInstanceNodeTypeOrders )
                } // if( UniqueBindings.Any() )
            } // if(false == allEmpty)
            else
            {
                //OnMessage( msgPrefix + "Skipped.  No property values to import." );
                // Set a fake nodeid in this row so we can move on
                //ImportRow[Order.PkColName] = CswConvert.ToDbVal( 0 );
                //ImportDataUpdate.update( ImportDataTable );
                ImportedNodeId = new CswPrimaryKey( "nodes", 0 );
            }
            return ImportedNodeId;
        } // _ImportOneRow()


        private void _importPropertyValues( CswNbtImportDef BindingDef, IEnumerable<CswNbtImportDefBinding> NodeTypeBindings, IEnumerable<CswNbtImportDefRelationship> RowRelationships, DataRow ImportRow, CswNbtNode Node )
        {
            // Iterate each binding 
            foreach( CswNbtImportDefBinding Binding in NodeTypeBindings )
            {
                // Special case for TimeInterval, specifically for IMCS imports
                if( Binding.DestProperty.getFieldTypeValue() == CswEnumNbtFieldType.TimeInterval )
                {
                    XElement input = XElement.Parse( "<rateintervalvalue>" + ImportRow[Binding.ImportDataColumnName].ToString().ToLower() + "</rateintervalvalue>" );
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load( input.CreateReader() );

                    CswRateInterval rateInterval = new CswRateInterval( _CswNbtResources );
                    rateInterval.ReadXml( xmlDoc.DocumentElement );

                    ( (CswNbtNodePropTimeInterval) Node.Properties[Binding.DestProperty] ).RateInterval = rateInterval;
                    Node.Properties[Binding.DestProperty].SyncGestalt();
                }
                // NodeTypeSelect
                else if( Binding.DestProperty.getFieldTypeValue() == CswEnumNbtFieldType.NodeTypeSelect )
                {
                    CswNbtMetaDataNodeType nt = _CswNbtResources.MetaData.getNodeType( ImportRow[Binding.ImportDataColumnName].ToString() );
                    if( nt != null )
                    {
                        Node.Properties[Binding.DestProperty].AsNodeTypeSelect.SelectedNodeTypeIds = new CswCommaDelimitedString() { nt.NodeTypeId.ToString() };
                    }
                    else
                    {
                        OnMessage( "Skipped invalid nodetype: " + ImportRow[Binding.ImportDataColumnName].ToString() );
                    }
                }
                // Quantity or Relationship
                else if( ( Binding.DestProperty.getFieldTypeValue() == CswEnumNbtFieldType.Quantity && Binding.DestSubfield.Column.ToString().ToLower() == "name" )
                            || Binding.DestProperty.getFieldTypeValue() == CswEnumNbtFieldType.Relationship
                            || Binding.DestProperty.getFieldTypeValue() == CswEnumNbtFieldType.Location )
                {
                    CswCommaDelimitedString inClause = new CswCommaDelimitedString();
                    Dictionary<string, int> FKNodeTypes = new Dictionary<string, int>();
                    if( Binding.DestProperty.FKType == CswEnumNbtViewRelatedIdType.NodeTypeId.ToString() )
                    {
                        inClause.Add( Binding.DestProperty.FKValue.ToString() );
                        FKNodeTypes.Add( _CswNbtResources.MetaData.getNodeType( Binding.DestProperty.FKValue ).NodeTypeName, Binding.DestProperty.FKValue );
                    }
                    else if( Binding.DestProperty.FKType == CswEnumNbtViewRelatedIdType.ObjectClassId.ToString() )
                    {
                        CswNbtMetaDataObjectClass oc = _CswNbtResources.MetaData.getObjectClass( Binding.DestProperty.FKValue );
                        foreach( CswNbtMetaDataNodeType nt in oc.getNodeTypes() )
                        {
                            inClause.Add( nt.NodeTypeId.ToString() );
                            FKNodeTypes.Add( nt.NodeTypeName, nt.NodeTypeId );
                        }
                    }
                    else if( Binding.DestProperty.FKType == CswEnumNbtViewRelatedIdType.PropertySetId.ToString() )
                    {
                        CswNbtMetaDataPropertySet ps = _CswNbtResources.MetaData.getPropertySet( Binding.DestProperty.FKValue );
                        foreach( CswNbtMetaDataObjectClass oc in ps.getObjectClasses() )
                        {
                            foreach( CswNbtMetaDataNodeType nt in oc.getNodeTypes() )
                            {
                                inClause.Add( nt.NodeTypeId.ToString() );
                                FKNodeTypes.Add( nt.NodeTypeName, nt.NodeTypeId );
                            }
                        }
                    }

                    // If the subfield isn't set to NodeID, then we don't need to look up the Legacy Id
                    bool MatchedOnLegacyId = false;
                    if( Binding.DestSubFieldName == CswEnumNbtSubFieldName.NodeID.ToString() )
                    {
                        // First we use a view to search on the Legacy Id and if it returns no results then we search on the Name
                        MatchedOnLegacyId = _relationshipSearchViaLegacyId( Node,
                                                                           ImportRow[Binding.SourceColumnName].ToString(),
                                                                           Binding.DestProperty.getFieldTypeValue(),
                                                                           Binding.DestNodeTypeName,
                                                                           Binding.DestProperty,
                                                                           FKNodeTypes );
                    }
                    if( false == MatchedOnLegacyId )
                    {
                        // Alternatively, we try to search based on the Name property
                        _relationshipSearchViaName( Node, inClause, ImportRow, Binding );
                    }

                }
                else
                {
                    Node.Properties[Binding.DestProperty].SetPropRowValue( Binding.DestSubfield.Column, ImportRow[Binding.ImportDataColumnName].ToString() );
                    Node.Properties[Binding.DestProperty].SyncGestalt();
                }
            }//foreach( CswNbtImportDefBinding Binding in NodeTypeBindings )

            #region CswNbtImportDefRelationship

            foreach( CswNbtImportDefRelationship RowRelationship in RowRelationships )
            {
                CswNbtImportDefOrder TargetOrder = BindingDef.ImportOrder.Values.FirstOrDefault( o => RowRelationship.Relationship.FkMatches( o.NodeType ) && o.Instance == RowRelationship.Instance );

                // If we have a value for the SourceRelColumnName
                if( null != ImportRow[RowRelationship.SourceRelColumnName] )
                {
                    // In this case, we are matching on Legacy Id
                    // If the value in the column isn't null and it is actually an integer value (A legacy id)
                    if( false == string.IsNullOrEmpty( ImportRow[RowRelationship.SourceRelColumnName].ToString() ) && CswConvert.ToInt32( ImportRow[RowRelationship.SourceRelColumnName] ) > 0 )
                    {
                        // We need to search for a node with a legacy id = ImportRow[RowRelationship.SourceRelColumnName]
                        Dictionary<string, int> FKNodeTypes = new Dictionary<string, int>();
                        FKNodeTypes.Add( TargetOrder.NodeType.NodeTypeName, TargetOrder.NodeType.NodeTypeId );
                        _relationshipSearchViaLegacyId( Node,
                                                        ImportRow[RowRelationship.SourceRelColumnName].ToString(),
                                                        RowRelationship.Relationship.getFieldTypeValue(),
                                                        RowRelationship.NodeTypeName,
                                                        RowRelationship.Relationship,
                                                        FKNodeTypes );
                    }
                }
                else
                {
                    // In this case, we are matching on NodeId
                    if( null != TargetOrder && null != ImportRow[TargetOrder.PkColName] && CswConvert.ToInt32( ImportRow[TargetOrder.PkColName] ) > 0 )
                    {
                        Node.Properties[RowRelationship.Relationship].SetPropRowValue(
                            RowRelationship.Relationship.getFieldTypeRule().SubFields[CswEnumNbtSubFieldName.NodeID].Column,
                            ImportRow[TargetOrder.PkColName]
                            );
                        if( RowRelationship.Relationship.getFieldTypeValue() == CswEnumNbtFieldType.Relationship )
                        {
                            Node.Properties[RowRelationship.Relationship].AsRelationship.RefreshNodeName();
                        }
                        if( RowRelationship.Relationship.getFieldTypeValue() == CswEnumNbtFieldType.Location )
                        {
                            Node.Properties[RowRelationship.Relationship].AsLocation.RefreshNodeName();
                        }

                        Node.Properties[RowRelationship.Relationship].SyncGestalt();
                    }
                }
            } // foreach( CswNbtMetaDataNodeTypeProp Relationship in RowRelationships )

            #endregion CswNbtImportDefRelationship

        } // _importPropertyValues()


        // This should actually set the value if there is one and return true if it was set and false if not
        //private bool _relationshipSearchViaLegacyId( CswNbtNode Node, string LegacyId, CswNbtImportDefBinding Binding, Dictionary<string, int> FKNodeTypes )
        private bool _relationshipSearchViaLegacyId( CswNbtNode Node, string LegacyId, CswEnumNbtFieldType FieldType, string DestNodeTypeName, CswNbtMetaDataNodeTypeProp NodeTypeProp, Dictionary<string, int> FKNodeTypes )
        {
            bool Ret = false;

            if( false == string.IsNullOrEmpty( LegacyId ) )
            {
                // TODO: Create view using relationship target (PS, OC, NT)
                Int32 NodeTypeId = FKNodeTypes[FKNodeTypes.Keys.First()];

                CswNbtView View = new CswNbtView( _CswNbtResources );
                View.ViewName = "MatchingLegacyId_View";

                CswNbtMetaDataNodeType CurrentNodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                if( null != CurrentNodeType )
                {
                    CswNbtMetaDataNodeTypeProp LegacyIdNTP = CurrentNodeType.getNodeTypeProp( "Legacy Id" );
                    CswNbtViewRelationship ParentRelationship = View.AddViewRelationship( CurrentNodeType, false );
                    View.AddViewPropertyAndFilter( ParentViewRelationship: ParentRelationship,
                                                  MetaDataProp: LegacyIdNTP,
                                                  Conjunction: CswEnumNbtFilterConjunction.And,
                                                  SubFieldName: CswEnumNbtSubFieldName.Value,
                                                  FilterMode: CswEnumNbtFilterMode.Equals,
                                                  Value: LegacyId );

                    ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, false, true, true );
                    if( Tree.getChildNodeCount() > 0 )
                    {
                        // Get the Node
                        Tree.goToNthChild( 0 );

                        // Set the relationship property to the nodeid of the found node
                        Node.Properties[NodeTypeProp].SetPropRowValue( NodeTypeProp.getFieldTypeRule().SubFields[CswEnumNbtSubFieldName.NodeID].Column, Tree.getNodeIdForCurrentPosition().PrimaryKey );

                        // Refresh
                        if( FieldType == CswEnumNbtFieldType.Relationship )
                        {
                            Node.Properties[NodeTypeProp].AsRelationship.RefreshNodeName();
                        }
                        if( FieldType == CswEnumNbtFieldType.Location )
                        {
                            Node.Properties[NodeTypeProp].AsLocation.RefreshNodeName();
                        }
                        Node.Properties[NodeTypeProp].SyncGestalt();

                        Ret = true;

                    }//if (Tree.getChildNodeCount() > 0)

                }//if( null != CurrentNodeType )

            }//if( false == string.IsNullOrEmpty( LegacyId ) )

            return Ret;
        }

        private void _relationshipSearchViaName( CswNbtNode Node, CswCommaDelimitedString inClause, DataRow ImportRow, CswNbtImportDefBinding Binding )
        {
            string sql = "select min(nodeid) nodeid from nodes where nodetypeid in ("
                                   + inClause.ToString()
                                   + ") and lower(nodename)='"
                                   + ImportRow[Binding.ImportDataColumnName].ToString().ToLower() + "'";
            CswArbitrarySelect relNodeSel = _CswNbtResources.makeCswArbitrarySelect( "getRelatedNode", sql );
            DataTable relNodeTbl = relNodeSel.getTable();
            if( relNodeTbl.Rows.Count > 0 )
            {
                // Because the sql query is using 'min' it will always return a row; we only want the row if it has a value
                if( false == string.IsNullOrEmpty( relNodeTbl.Rows[0][0].ToString() ) )
                {
                    CswPrimaryKey pk = new CswPrimaryKey( "nodes", CswConvert.ToInt32( relNodeTbl.Rows[0]["nodeid"] ) );
                    if( Binding.DestProperty.getFieldTypeValue() == CswEnumNbtFieldType.Quantity )
                    {
                        Node.Properties[Binding.DestProperty].AsQuantity.UnitId = pk;
                    }
                    else
                    {
                        Node.Properties[Binding.DestProperty].AsRelationship.RelatedNodeId = pk;
                    }
                }
            }
            else
            {
                OnMessage( "No matching " + Binding.DestNodeType.NodeTypeName + " for " +
                          ImportRow[Binding.ImportDataColumnName] );
            }
        }

    } // class CswNbt2DImporter
} // namespace ChemSW.Nbt.ImportExport
