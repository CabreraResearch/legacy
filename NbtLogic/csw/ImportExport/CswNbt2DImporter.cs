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
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.ImportExport
{
    public class CswNbt2DImporter
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswNbt2DImporter( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtSchemaModTrnsctn = new CswNbtSchemaModTrnsctn( _CswNbtResources );
        }

        public delegate void ErrorHandler( string ErrorMessage );
        public ErrorHandler OnError = delegate( string ErrorMessage )
        {
            // Default: throw
            throw new Exception( ErrorMessage );
        };

        public delegate void MessageHandler( string Message );

        public MessageHandler OnMessage = delegate( string Message ) { };

        private DataSet _readExcel( string FilePath )
        {
            DataSet ret = new DataSet();
            try
            {
                //Set up ADO connection to spreadsheet
                string ConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + FilePath + ";Extended Properties=Excel 8.0;";
                OleDbConnection ExcelConn = new OleDbConnection( ConnStr );
                ExcelConn.Open();

                DataTable ExcelMetaDataTable = ExcelConn.GetOleDbSchemaTable( OleDbSchemaGuid.Tables, null );
                if( null == ExcelMetaDataTable )
                {
                    OnError( "Could not process the excel file: " + FilePath );
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
            }
            catch( Exception ex )
            {
                OnError( ex.Message + "\r\n" + ex.StackTrace );
            }
            return ret;
        } // _readExcel

        /// <summary>
        /// Stores data in temporary Oracle tables
        /// </summary>
        public StringCollection storeData( string DataFilePath, string ImportDefinitionName, bool Overwrite )
        {
            StringCollection ret = new StringCollection();
            DataSet ExcelDataSet = _readExcel( DataFilePath );
            foreach( DataTable ExcelDataTable in ExcelDataSet.Tables )
            {
                string SheetName = ExcelDataTable.TableName;

                // Determine Oracle table name
                Int32 i = 1;
                string ImportDataTableName = "importdata" + i.ToString();
                while( _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( ImportDataTableName ) )
                {
                    i++;
                    ImportDataTableName = "importdata" + i.ToString();
                }

                // Generate an Oracle table for storing and manipulating data
                _CswNbtSchemaModTrnsctn.addTable( ImportDataTableName, "importdataid" );
                _CswNbtSchemaModTrnsctn.addBooleanColumn( ImportDataTableName, "error", "", false, false );
                _CswNbtSchemaModTrnsctn.addClobColumn( ImportDataTableName, "errorlog", "", false, false );
                foreach( DataColumn ExcelColumn in ExcelDataTable.Columns )
                {
                    _CswNbtSchemaModTrnsctn.addStringColumn( ImportDataTableName, CswNbt2DBinding.SafeColName( ExcelColumn.ColumnName ), "", false, false, 4000 );
                }
                CswNbt2DDefinition Definition = new CswNbt2DDefinition( _CswNbtResources, ImportDefinitionName, SheetName );
                foreach( CswNbt2DOrder Order in Definition.ImportOrder.Values )
                {
                    _CswNbtSchemaModTrnsctn.addLongColumn( ImportDataTableName, Order.PkColName, "", false, false );
                }
                _CswNbtResources.commitTransaction();
                _CswNbtResources.beginTransaction();

                ret.Add( ImportDataTableName );

                // Store the sheet reference in import_data_map
                CswTableUpdate ImportDataMapUpdate = _CswNbtResources.makeCswTableUpdate( "Importer_Sheet_Update", CswNbt2DImportTables.ImportDataMap.TableName );
                DataTable ImportDataMapTable = ImportDataMapUpdate.getEmptyTable();
                DataRow DataMapRow = ImportDataMapTable.NewRow();
                DataMapRow[CswNbt2DImportTables.ImportDataMap.datatablename] = ImportDataTableName;
                DataMapRow[CswNbt2DImportTables.ImportDataMap.importdefinitionid] = Definition.ImportDefinitionId;
                DataMapRow[CswNbt2DImportTables.ImportDataMap.overwrite] = CswConvert.ToDbVal( Overwrite );
                ImportDataMapTable.Rows.Add( DataMapRow );
                ImportDataMapUpdate.update( ImportDataMapTable );

                // Copy the Excel data into the Oracle table
                CswTableUpdate ImportDataUpdate = _CswNbtResources.makeCswTableUpdate( "Importer_Update", ImportDataTableName );
                DataTable ImportDataTable = ImportDataUpdate.getEmptyTable();
                foreach( DataRow ExcelRow in ExcelDataTable.Rows )
                {
                        bool hasData = false;
                    DataRow ImportRow = ImportDataTable.NewRow();
                    ImportRow["error"] = CswConvert.ToDbVal( false );
                    foreach( DataColumn ExcelColumn in ExcelDataTable.Columns )
                    {
                            if( ExcelRow[ExcelColumn] != DBNull.Value )
                            {
                                hasData = true;
                            }
                        ImportRow[CswNbt2DBinding.SafeColName( ExcelColumn.ColumnName )] = ExcelRow[ExcelColumn];
                    }
                        if( hasData == true )
                        {
                            ImportDataTable.Rows.Add( ImportRow );
                        }
                    }
                ImportDataUpdate.update( ImportDataTable );

                OnMessage( "Sheet '" + ExcelDataTable.TableName + "' is stored in Table '" + ImportDataTableName + "'" );
            } // foreach( DataTable ExcelDataTable in ExcelDataSet.Tables )

            _CswNbtResources.commitTransaction();
            _CswNbtResources.beginTransaction();
            
            return ret;
        } // storeData()

        /// <summary>
        /// Returns the set of available Import Definition Names
        /// </summary>
        public CswCommaDelimitedString getDefinitionNames()
        {
            CswCommaDelimitedString ret = new CswCommaDelimitedString();
            CswTableSelect DefSelect = _CswNbtResources.makeCswTableSelect( "loadBindings_def_select1", CswNbt2DImportTables.ImportDefinitions.TableName );
            DataTable DefDataTable = DefSelect.getTable();
            foreach( DataRow defrow in DefDataTable.Rows )
            {
                ret.Add( defrow[CswNbt2DImportTables.ImportDefinitions.definitionname].ToString(), false, true );
            }
            return ret;
        } // getDefinitions()

        /// <summary>
        /// Import a number of rows
        /// </summary>
        /// <param name="RowsToImport">Number of rows to import</param>
        /// <param name="ImportDataTableName">Source Oracle table to import</param>
        /// <returns>True if there are more rows to import from this source data, false otherwise</returns>
        public bool ImportRows( Int32 RowsToImport, string ImportDataTableName )
        {
            Int32 RowsImported = 0;
            try
            {
                if( false == string.IsNullOrEmpty( ImportDataTableName ) && _CswNbtResources.isTableDefinedInDataBase( ImportDataTableName ) )
                {
                    // Lookup the binding definition
                    CswNbt2DImportDataMap DataMap = new CswNbt2DImportDataMap( _CswNbtResources, ImportDataTableName );
                    CswNbt2DDefinition BindingDef = new CswNbt2DDefinition( _CswNbtResources, DataMap.ImportDefinitionId );

                    //if( null != BindingDef && BindingDef.Bindings.Count > 0 && BindingDef.ImportOrder.Count > 0 ) //original

                    if( null != BindingDef && ( BindingDef.Bindings.Count > 0 || BindingDef.RowRelationships.Count > 0 ) && BindingDef.ImportOrder.Count > 0 ) //dch
                    {
                        foreach( CswNbt2DOrder Order in BindingDef.ImportOrder.Values )
                        {
                            string msgPrefix = Order.NodeType.NodeTypeName + " Import: ";
                            CswTableUpdate ImportDataUpdate = _CswNbtResources.makeCswTableUpdate( "Importer_Update", ImportDataTableName );

                            // Fetch the next row to process
                            bool moreRows = true;
                            while( moreRows )
                            {
                                DataTable ImportDataTable = ImportDataUpdate.getTable( "where error = '" + CswConvert.ToDbVal( false ) + "' and " + Order.PkColName + " is null",
                                                                                       new Collection<OrderByClause> { new OrderByClause( "importdataid", CswEnumOrderByType.Ascending ) },
                                                                                       0, 1 );
                                moreRows = ( ImportDataTable.Rows.Count > 0 );
                                if( moreRows )
                                {
                                    DataRow ImportRow = ImportDataTable.Rows[0];
                                    try
                                    {
                                        msgPrefix = Order.NodeType.NodeTypeName + " Import (" + ImportRow["importdataid"].ToString() + "): ";
                                        CswNbtNode Node = null;

                                        IEnumerable<CswNbt2DBinding> NodeTypeBindings = BindingDef.Bindings.Where( b => b.DestNodeType == Order.NodeType && b.Instance == Order.Instance );
                                        IEnumerable<CswNbt2DRowRelationship> RowRelationships = BindingDef.RowRelationships.Where( r => r.NodeType.NodeTypeId == Order.NodeType.NodeTypeId ); //&& r.Instance == Order.Instance );
                                        IEnumerable<CswNbt2DRowRelationship> UniqueRelationships = RowRelationships.Where( r => r.Relationship.IsUnique() ||
                                                                                                              r.Relationship.IsCompoundUnique() ||
                                                                                                              Order.NodeType.NameTemplatePropIds.Contains( r.Relationship.FirstPropVersionId ) );

                                        //IEnumerable<CswNbt2DBinding> RequiredBindings = NodeTypeBindings.Where( b => b.DestProperty.IsRequired );
                                        //IEnumerable<CswNbt2DBinding> UniqueBindings = NodeTypeBindings.Where( b => ( b.DestProperty.IsUnique() || b.DestProperty.IsCompoundUnique() ) );
                                        //IEnumerable<CswNbtMetaDataNodeTypeProp> Props = Order.NodeType.getNodeTypeProps();
                                        //IEnumerable<CswNbtMetaDataNodeTypeProp> RequiredProps = Props.Where( p => p.IsRequired && false == p.HasDefaultValue() );
                                        IEnumerable<CswNbt2DBinding> UniqueBindings = NodeTypeBindings.Where( b => b.DestProperty.IsUnique() ||
                                                                                                              b.DestProperty.IsCompoundUnique() ||
                                                                                                              Order.NodeType.NameTemplatePropIds.Contains( b.DestProperty.FirstPropVersionId ) );
                                        //IEnumerable<CswNbtMetaDataNodeTypeProp> UniqueProps = Props.Where( p => UniqueBindings.Any( b => b.DestProperty == p ) );

                                        // Skip rows with null values for all unique properties
                                        bool allEmpty = true;
                                        foreach( CswNbt2DBinding Binding in UniqueBindings )
                                        {
                                            allEmpty = allEmpty && string.IsNullOrEmpty( ImportRow[Binding.ImportDataColumnName].ToString() );
                                        }
                                        foreach( CswNbt2DRowRelationship Relation in UniqueRelationships )
                                        {
                                            CswNbt2DOrder thisTargetOrder = BindingDef.ImportOrder.Values.FirstOrDefault( o => Relation.Relationship.FkMatches( o.NodeType ) && o.Instance == Relation.Instance );
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
                                                foreach( CswNbt2DBinding Binding in UniqueBindings )
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

                                                foreach( CswNbt2DRowRelationship Relation in UniqueRelationships )
                                                {
                                                    CswNbt2DOrder thisTargetOrder = BindingDef.ImportOrder.Values.FirstOrDefault( o => Relation.Relationship.FkMatches( o.NodeType ) && o.Instance == Relation.Instance );
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
                                                foreach( CswNbt2DBinding Binding in NodeTypeBindings )
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

                                                foreach( CswNbt2DRowRelationship RowRelationship in RowRelationships )
                                                {
                                                    CswNbt2DOrder TargetOrder = null;

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
                                                IEnumerable<CswNbt2DOrder> AllInstanceNodeTypeOrders = BindingDef.ImportOrder.Values.Where( o => o.NodeType == Order.NodeType );
                                                foreach( CswNbt2DOrder OtherOrder in AllInstanceNodeTypeOrders )
                                                {
                                                    string WhereClause = "where error = '" + CswConvert.ToDbVal( false ) + "' and " + OtherOrder.PkColName + " is null";
                                                    foreach( CswNbt2DBinding UniqueBinding in UniqueBindings )
                                                    {
                                                        CswNbt2DBinding OtherUniqueBinding = BindingDef.Bindings.byProp( OtherOrder.Instance, UniqueBinding.DestProperty, UniqueBinding.DestSubfield ).FirstOrDefault();
                                                        if( null != OtherUniqueBinding )
                                                        {
                                                            WhereClause += " and lower(" + OtherUniqueBinding.ImportDataColumnName + ") = '" + CswTools.SafeSqlParam( ImportRow[UniqueBinding.ImportDataColumnName].ToString().ToLower() ) + "' ";
                                                        }
                                                    }

                                                    foreach( CswNbt2DRowRelationship Relation in UniqueRelationships )
                                                    {
                                                        CswNbt2DOrder thisTargetOrder = BindingDef.ImportOrder.Values.FirstOrDefault( o => Relation.Relationship.FkMatches( o.NodeType ) && o.Instance == Relation.Instance );
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
                                        string ErrorMsg = msgPrefix + ex.Message; //+ "\r\n" + ex.StackTrace;
                                        OnError( ErrorMsg );

                                        ImportRow["error"] = CswConvert.ToDbVal( true );
                                        ImportRow["errorlog"] = ErrorMsg;
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
            }
            catch( Exception ex )
            {
                OnError( ex.Message + "\r\n" + ex.StackTrace );
            }
            return ( RowsImported != 0 );
        } // ImportRows()


        public void getCounts( string ImportDataTableName, out Int32 PendingRows, out Int32 ErrorRows )
        {
            PendingRows = Int32.MinValue;
            ErrorRows = Int32.MinValue;
            if( false == string.IsNullOrEmpty( ImportDataTableName ) && _CswNbtResources.isTableDefinedInDataBase( ImportDataTableName ) )
            {
                CswTableSelect ImportDataSelect = _CswNbtResources.makeCswTableSelect( "Importer_Select", ImportDataTableName );
                ErrorRows = ImportDataSelect.getRecordCount( "where error = '" + CswConvert.ToDbVal( true ) + "'" );

                CswNbt2DImportDataMap DataMap = new CswNbt2DImportDataMap( _CswNbtResources, ImportDataTableName );
                CswNbt2DDefinition BindingDef = new CswNbt2DDefinition( _CswNbtResources, DataMap.ImportDefinitionId );
                if( null != BindingDef && BindingDef.ImportOrder.Count > 0 )
                {
                    string PendingWhereClause = string.Empty;
                    foreach( CswNbt2DOrder Order in BindingDef.ImportOrder.Values )
                    {
                        if( string.Empty != PendingWhereClause )
                        {
                            PendingWhereClause += " or ";
                        }
                        PendingWhereClause += Order.PkColName + " is null";
                    }
                    PendingRows = ImportDataSelect.getRecordCount( "where error = '" + CswConvert.ToDbVal( false ) + "' and (" + PendingWhereClause + ") " );
                } // if( null != BindingDef && BindingDef.ImportOrder.Count > 0 )
            } // if( false == string.IsNullOrEmpty( ImportDataTableName ) && _CswNbtResources.isTableDefinedInDataBase( ImportDataTableName ) )
        } // getCounts()

    } // class CswNbt2DImporter
} // namespace ChemSW.Nbt.ImportExport
