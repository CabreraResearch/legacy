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
    public class CswNbtImporter
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
        } // _readExcel()

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
        } // storeData()

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
        } // getDefinitions()

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
        } // getJobs()



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

                //if( null != BindingDef && BindingDef.Bindings.Count > 0 && BindingDef.ImportOrder.Count > 0 ) //original

                if( null != BindingDef && ( BindingDef.Bindings.Count > 0 || BindingDef.RowRelationships.Count > 0 ) && BindingDef.ImportOrder.Count > 0 ) //dch
                {
                    foreach( CswNbtImportDefOrder Order in BindingDef.ImportOrder.Values )
                    {
                        string msgPrefix = Order.NodeType.NodeTypeName + " Import: ";
                        CswTableUpdate ImportDataUpdate = _CswNbtResources.makeCswTableUpdate( "Importer_Update", ImportDataTableName );

                        // Fetch the next row to process
                        bool moreRows = true;
                        while( moreRows )
                        {
                            DataTable ImportDataTable = ImportDataUpdate.getTable( "where " + CswNbtImportTables.ImportDataN.error + " = '" + CswConvert.ToDbVal( false ) + "' and " + Order.PkColName + " is null",
                                                                                   new Collection<OrderByClause> { new OrderByClause( CswNbtImportTables.ImportDataN.importdataid, CswEnumOrderByType.Ascending ) },
                                                                                   0, 1 );
                            moreRows = ( ImportDataTable.Rows.Count > 0 );
                            if( moreRows )
                            {
                                DataRow ImportRow = ImportDataTable.Rows[0];
                                try
                                {
                                    msgPrefix = Order.NodeType.NodeTypeName + " Import (" + ImportRow[CswNbtImportTables.ImportDataN.importdataid].ToString() + "): ";
                                    CswNbtNode Node = null;

                                    IEnumerable<CswNbtImportDefBinding> NodeTypeBindings = BindingDef.Bindings.Where( b => b.DestNodeType == Order.NodeType && b.Instance == Order.Instance );
                                    IEnumerable<CswNbtImportDefRelationship> RowRelationships = BindingDef.RowRelationships.Where( r => r.NodeType.NodeTypeId == Order.NodeType.NodeTypeId ); //&& r.Instance == Order.Instance );
                                    IEnumerable<CswNbtImportDefRelationship> UniqueRelationships = RowRelationships.Where( r => r.Relationship.IsUnique() ||
                                                                                                                                r.Relationship.IsCompoundUnique() ||
                                                                                                                                Order.NodeType.NameTemplatePropIds.Contains( r.Relationship.FirstPropVersionId ) );

                                    //IEnumerable<CswNbt2DBinding> RequiredBindings = NodeTypeBindings.Where( b => b.DestProperty.IsRequired );
                                    //IEnumerable<CswNbt2DBinding> UniqueBindings = NodeTypeBindings.Where( b => ( b.DestProperty.IsUnique() || b.DestProperty.IsCompoundUnique() ) );
                                    //IEnumerable<CswNbtMetaDataNodeTypeProp> Props = Order.NodeType.getNodeTypeProps();
                                    //IEnumerable<CswNbtMetaDataNodeTypeProp> RequiredProps = Props.Where( p => p.IsRequired && false == p.HasDefaultValue() );
                                    IEnumerable<CswNbtImportDefBinding> UniqueBindings = NodeTypeBindings.Where( b => b.DestProperty.IsUnique() ||
                                                                                                                      b.DestProperty.IsCompoundUnique() ||
                                                                                                                      Order.NodeType.NameTemplatePropIds.Contains( b.DestProperty.FirstPropVersionId ) );
                                    //IEnumerable<CswNbtMetaDataNodeTypeProp> UniqueProps = Props.Where( p => UniqueBindings.Any( b => b.DestProperty == p ) );

                                    // Skip rows with null values for all unique properties
                                    bool allEmpty = true;
                                    foreach( CswNbtImportDefBinding Binding in UniqueBindings )
                                    {
                                        allEmpty = allEmpty && string.IsNullOrEmpty( ImportRow[Binding.ImportDataColumnName].ToString() );
                                    }
                                    foreach( CswNbtImportDefRelationship Relation in UniqueRelationships )
                                    {
                                        CswNbtImportDefOrder thisTargetOrder = BindingDef.ImportOrder.Values.FirstOrDefault( o => Relation.Relationship.FkMatches( o.NodeType ) && o.Instance == Relation.Instance );
                                        Int32 Value = CswConvert.ToInt32( ImportRow[thisTargetOrder.PkColName] );

                                        allEmpty = allEmpty && ( Value != Int32.MinValue );
                                    }

                                    if( false == allEmpty )
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
                                                CswNbtImportDefOrder thisTargetOrder = BindingDef.ImportOrder.Values.FirstOrDefault( o => Relation.Relationship.FkMatches( o.NodeType ) && o.Instance == Relation.Instance );
                                                Int32 Value = CswConvert.ToInt32( ImportRow[thisTargetOrder.PkColName] );

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
                                                ICswNbtTree UniqueTree = _CswNbtResources.Trees.getTreeFromView( UniqueView, false, true, true );
                                                if( UniqueTree.getChildNodeCount() > 0 )
                                                {
                                                    UniqueTree.goToNthChild( 0 );
                                                    Node = UniqueTree.getNodeForCurrentPosition();
                                                }
                                            }
                                        } // if( UniqueProps.Any() )

                                        bool isNewNode = false;
                                        if( null == Node )
                                        {
                                            // Make a new node
                                            Node = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( Order.NodeType.NodeTypeId, CswEnumNbtMakeNodeOperation.WriteNode );
                                            isNewNode = true;


                                            //// Check for non-null values for all required properties
                                            ////foreach( CswNbt2DBinding Binding in RequiredBindings )
                                            //foreach( CswNbtMetaDataNodeTypeProp RequiredProp in RequiredProps )
                                            //{
                                            //    IEnumerable<CswNbt2DBinding> RequiredBindings = BindingDef.Bindings.byProp( Order.Instance, RequiredProp );
                                            //    bool hasValue = RequiredBindings.Aggregate( false, ( current, RequiredBinding ) => current || false == string.IsNullOrEmpty( ImportRow[RequiredBinding.ImportDataColumnName].ToString() ) );
                                            //    if( false == hasValue )
                                            //    {
                                            //        throw new Exception( msgPrefix + "Required property value is missing: " + RequiredProp.PropName );
                                            //    }
                                            //} // foreach( CswNbtMetaDataNodeTypeProp RequiredProp in RequiredProps )

                                        }


                                        // Save the nodeid in this row
                                        ImportRow[Order.PkColName] = CswConvert.ToDbVal( Node.NodeId.PrimaryKey );
                                        ImportDataUpdate.update( ImportDataTable );


                                        // Import property values
                                        if( isNewNode || DataMap.Overwrite )
                                        {
                                            foreach( CswNbtImportDefBinding Binding in NodeTypeBindings )
                                            {
                                                // special case for TimeInterval, specifically for IMCS imports
                                                if( Binding.DestProperty.getFieldTypeValue() == CswEnumNbtFieldType.TimeInterval )
                                                {
                                                    XElement input = XElement.Parse( "<rateintervalvalue>" + ImportRow[Binding.ImportDataColumnName].ToString().ToLower() + "</rateintervalvalue>" );
                                                    XmlDocument xmlDoc = new XmlDocument();
                                                    xmlDoc.Load( input.CreateReader() );

                                                    CswRateInterval rateInterval = new CswRateInterval( _CswNbtResources );
                                                    rateInterval.ReadXml( xmlDoc.DocumentElement );

                                                    ( (CswNbtNodePropTimeInterval) Node.Properties[Binding.DestProperty] ).RateInterval = rateInterval;
                                                    //Node.Properties[Binding.DestProperty].SetPropRowValue( CswEnumNbtPropColumn.ClobData, rateInterval.ToXmlString() );
                                                    Node.Properties[Binding.DestProperty].SyncGestalt();
                                                }
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
                                                else
                                                {
                                                    Node.Properties[Binding.DestProperty].SetPropRowValue( Binding.DestSubfield.Column, ImportRow[Binding.ImportDataColumnName].ToString() );
                                                    Node.Properties[Binding.DestProperty].SyncGestalt();
                                                }
                                            }

                                            foreach( CswNbtImportDefRelationship RowRelationship in RowRelationships )
                                            {
                                                CswNbtImportDefOrder TargetOrder = null;

                                                //if( RowRelationship.Relationship.FKType == NbtViewRelatedIdType.NodeTypeId.ToString() )
                                                //{
                                                //    TargetOrder = BindingDef.ImportOrder.Values.FirstOrDefault( o => o.NodeType.NodeTypeId == RowRelationship.Relationship.FKValue && o.Instance == RowRelationship.Instance );
                                                //}
                                                //else if( RowRelationship.Relationship.FKType == NbtViewRelatedIdType.ObjectClassId.ToString() )
                                                //{
                                                //    TargetOrder = BindingDef.ImportOrder.Values.FirstOrDefault( o => o.NodeType.ObjectClassId == RowRelationship.Relationship.FKValue && o.Instance == RowRelationship.Instance );
                                                //}
                                                //else if( RowRelationship.Relationship.FKType == NbtViewRelatedIdType.PropertySetId.ToString() )
                                                //{
                                                //    TargetOrder = BindingDef.ImportOrder.Values.FirstOrDefault( o => null != o.NodeType.getObjectClass().getPropertySet() && 
                                                //                                                                     o.NodeType.getObjectClass().getPropertySet().PropertySetId == RowRelationship.Relationship.FKValue && 
                                                //                                                                     o.Instance == RowRelationship.Instance );
                                                //}

                                                TargetOrder = BindingDef.ImportOrder.Values.FirstOrDefault( o => RowRelationship.Relationship.FkMatches( o.NodeType ) && o.Instance == RowRelationship.Instance );

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
                                                    /*
                                                                                                            if( RowRelationship.Relationship.getFieldTypeValue() == CswEnumNbtFieldType.Quantity )
                                                                                                            {
                                                                                                                Node.Properties[RowRelationship.Relationship].SetPropRowValue(
                                                                                                                    RowRelationship.Relationship.getFieldTypeRule().SubFields[CswEnumNbtSubFieldName.Name].Column,
                                                                                                                    ImportRow[RowRelationship.Relationship.]
                                                                                                                    );
                                                                                                            }
                                                     * */
                                                    Node.Properties[RowRelationship.Relationship].SyncGestalt();
                                                }
                                            } // foreach( CswNbtMetaDataNodeTypeProp Relationship in RowRelationships )
                                            Node.postChanges( false );

                                            OnMessage( msgPrefix + "Imported " + ( isNewNode ? "New " : "Existing " ) + Node.NodeName + " (" + Node.NodeId.PrimaryKey.ToString() + ")" );
                                        } // if(isNewNode || Overwrite )
                                        else
                                        {
                                            OnMessage( msgPrefix + "Skipped  " + Node.NodeName + " (" + Node.NodeId.PrimaryKey.ToString() + ")" );
                                        }


                                        // Simplify future imports by saving this nodeid on matching rows
                                        if( UniqueBindings.Any() )
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
                                                    Int32 Value = CswConvert.ToInt32( ImportRow[thisTargetOrder.PkColName] );

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
                                        OnMessage( msgPrefix + "Skipped.  No property values to import." );

                                        // Set a fake nodeid in this row so we can move on
                                        ImportRow[Order.PkColName] = CswConvert.ToDbVal( 0 );
                                        ImportDataUpdate.update( ImportDataTable );
                                    }
                                }
                                catch( Exception ex )
                                {
                                    // Swallow and store the error on the row
                                    string ErrorMsg = msgPrefix + ex.Message; //+ "\r\n" + ex.StackTrace;
                                    ImportRow[CswNbtImportTables.ImportDataN.error] = CswConvert.ToDbVal( true );
                                    ImportRow[CswNbtImportTables.ImportDataN.errorlog] = ErrorMsg;
                                    ImportDataUpdate.update( ImportDataTable );
                                }
                                RowsImported += 1;
                            } // if(moreRows)

                            if( RowsImported >= RowsToImport )
                            {
                                break;
                            }
                        } // while( moreRows )

                        if( RowsImported >= RowsToImport )
                        {
                            break;
                        }
                    } // foreach( CswNbtMetaDataNodeType NodeType in _ImportOrder.Values )
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

            bool MoreToDo = ( RowsImported != 0 );

            if( false == MoreToDo )
            {
                // Set 'completed' = true
                CswTableUpdate ImportDataMapUpdate = _CswNbtResources.makeCswTableUpdate( "ImportRows_DataMap_Update", CswNbtImportTables.ImportDataMap.TableName );
                DataTable ImportDataMapTable = ImportDataMapUpdate.getTable( "where " + CswNbtImportTables.ImportDataMap.datatablename + " = '" + ImportDataTableName + "'" );
                if( ImportDataMapTable.Rows.Count > 0 )
                {
                    ImportDataMapTable.Rows[0][CswNbtImportTables.ImportDataMap.completed] = CswConvert.ToDbVal( true );
                    ImportDataMapUpdate.update( ImportDataMapTable );
                }
            }
            return MoreToDo;
        } // ImportRows()


        //public void getCounts( string ImportDataTableName, out Int32 PendingRows, out Int32 ErrorRows )
        //{
        //    PendingRows = getCountPending( ImportDataTableName );
        //    ErrorRows = getCountError( ImportDataTableName );
        //} // getCounts()


        //public Int32 getCountPending( string ImportDataTableName )
        //{
        //    Int32 PendingRows = 0;
        //    if( false == string.IsNullOrEmpty( ImportDataTableName ) && _CswNbtResources.isTableDefinedInDataBase( ImportDataTableName ) )
        //    {
        //        CswTableSelect ImportDataSelect = _CswNbtResources.makeCswTableSelect( "Importer_Select", ImportDataTableName );
        //        CswNbtImportDataMap DataMap = new CswNbtImportDataMap( _CswNbtResources, ImportDataTableName );
        //        CswNbtImportDef BindingDef = new CswNbtImportDef( _CswNbtResources, DataMap.ImportDefinitionId );
        //        if( null != BindingDef && BindingDef.ImportOrder.Count > 0 )
        //        {
        //            string PendingWhereClause = string.Empty;
        //            foreach( CswNbtImportDefOrder Order in BindingDef.ImportOrder.Values )
        //            {
        //                if( string.Empty != PendingWhereClause )
        //                {
        //                    PendingWhereClause += " or ";
        //                }
        //                PendingWhereClause += Order.PkColName + " is null";
        //            }
        //            PendingRows = ImportDataSelect.getRecordCount( "where " + CswNbtImportTables.ImportDataN.error + " = '" + CswConvert.ToDbVal( false ) + "' and (" + PendingWhereClause + ") " );
        //        } // if( null != BindingDef && BindingDef.ImportOrder.Count > 0 )
        //    } // if( false == string.IsNullOrEmpty( ImportDataTableName ) && _CswNbtResources.isTableDefinedInDataBase( ImportDataTableName ) )
        //    return PendingRows;
        //} // getCountPending()

        //public Int32 getCountError( string ImportDataTableName )
        //{
        //    Int32 ErrorRows = 0;
        //    if( false == string.IsNullOrEmpty( ImportDataTableName ) && _CswNbtResources.isTableDefinedInDataBase( ImportDataTableName ) )
        //    {
        //        CswTableSelect ImportDataSelect = _CswNbtResources.makeCswTableSelect( "Importer_Select", ImportDataTableName );
        //        ErrorRows = ImportDataSelect.getRecordCount( "where error = '" + CswConvert.ToDbVal( true ) + "'" );
        //    }
        //    return ErrorRows;
        //} // getCountError()

    } // class CswNbt2DImporter
} // namespace ChemSW.Nbt.ImportExport
