using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.ImportExport
{
    public class CswNbt2DImporter
    {
        public Collection<CswNbt2DBinding> Bindings = new Collection<CswNbt2DBinding>();
        public SortedList<Int32, CswNbtMetaDataNodeType> ImportOrder = new SortedList<int, CswNbtMetaDataNodeType>();
        public Collection<CswNbtMetaDataNodeTypeProp> RowRelationships = new Collection<CswNbtMetaDataNodeTypeProp>();
        public bool Overwrite = false;

        public string ImportDataTableName;

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
        /// Stores data in a temporary Oracle table
        /// Returns the name of the table
        /// </summary>
        public void storeData( string DataFilePath )
        {
            DataSet ExcelDataSet = _readExcel( DataFilePath );
            DataTable ExcelDataTable = ExcelDataSet.Tables[0];

            // Generate an Oracle table for storing and manipulating data
            Int32 i = 1;
            ImportDataTableName = "importdata" + i.ToString();
            while( _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( ImportDataTableName ) )
            {
                i++;
                ImportDataTableName = "importdata" + i.ToString();
            }
            _CswNbtSchemaModTrnsctn.addTable( ImportDataTableName, "importdataid" );
            _CswNbtSchemaModTrnsctn.addBooleanColumn( ImportDataTableName, "error", "", false, false );
            _CswNbtSchemaModTrnsctn.addClobColumn( ImportDataTableName, "errorlog", "", false, false );
            foreach( DataColumn ExcelColumn in ExcelDataTable.Columns )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( ImportDataTableName, CswNbt2DBinding.SafeColName( ExcelColumn.ColumnName ), "", false, false, 4000 );
            }
            foreach( CswNbtMetaDataNodeType NodeType in ImportOrder.Values )
            {
                _CswNbtSchemaModTrnsctn.addLongColumn( ImportDataTableName, _NodeTypePkColName( NodeType ), "", false, false );
            }
            _CswNbtResources.commitTransaction();
            _CswNbtResources.beginTransaction();

            // Copy the Excel data into the Oracle table
            CswTableUpdate ImportDataUpdate = _CswNbtResources.makeCswTableUpdate( "Importer_Update", ImportDataTableName );
            DataTable ImportDataTable = ImportDataUpdate.getEmptyTable();
            foreach( DataRow ExcelRow in ExcelDataTable.Rows )
            {
                DataRow ImportRow = ImportDataTable.NewRow();
                ImportRow["error"] = CswConvert.ToDbVal( false );
                foreach( DataColumn ExcelColumn in ExcelDataTable.Columns )
                {
                    ImportRow[CswNbt2DBinding.SafeColName( ExcelColumn.ColumnName )] = ExcelRow[ExcelColumn];
                }
                ImportDataTable.Rows.Add( ImportRow );
            }
            ImportDataUpdate.update( ImportDataTable );

            _CswNbtResources.commitTransaction();
            _CswNbtResources.beginTransaction();
        } // storeData()


        /// <summary>
        /// Returns errors encountered
        /// </summary>
        public bool readBindings( string BindingFilePath )
        {
            bool ret = true;
            DataSet ExcelDataSet = _readExcel( BindingFilePath );

            if( ExcelDataSet.Tables.Count == 3 )
            {
                // Order
                DataTable OrderDataTable = ExcelDataSet.Tables["Order$"];
                foreach( DataRow OrderRow in OrderDataTable.Rows )
                {
                    string NTName = OrderRow["nodetype"].ToString();
                    CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NTName );
                    if( null != NodeType )
                    {
                        ImportOrder.Add( CswConvert.ToInt32( OrderRow["order"] ), NodeType );
                    }
                    else
                    {
                        OnError( "Error reading bindings: invalid NodeType defined in 'Order' sheet: " + NTName );
                        ret = false;
                    }
                } // foreach( DataRow OrderRow in OrderDataTable.Rows )


                // Bindings
                DataTable BindingsDataTable = ExcelDataSet.Tables["Bindings$"];
                foreach( DataRow BindingRow in BindingsDataTable.Rows )
                {
                    //CswNbtMetaDataObjectClass DestObjectClass = null;
                    CswNbtMetaDataNodeType DestNodeType = null;
                    CswNbtMetaDataNodeTypeProp DestProp = null;

                    string DestNTName = BindingRow["destnodetype"].ToString();
                    if( false == string.IsNullOrEmpty( DestNTName ) )
                    {
                        DestNodeType = _CswNbtResources.MetaData.getNodeType( DestNTName );
                        if( null != DestNodeType )
                        {
                            DestProp = DestNodeType.getNodeTypeProp( BindingRow["destproperty"].ToString() );
                            if( null != DestProp )
                            {
                                CswNbtSubField DestSubfield = DestProp.getFieldTypeRule().SubFields[(CswNbtSubField.SubFieldName) BindingRow["destsubfield"].ToString()];
                                if( DestSubfield == null )
                                {
                                    DestSubfield = DestProp.getFieldTypeRule().SubFields.Default;
                                }

                                Bindings.Add( new CswNbt2DBinding
                                    {
                                        SourceColumnName = BindingRow["sourcecolumnname"].ToString(),
                                        DestNodeType = DestNodeType,
                                        DestProperty = DestProp,
                                        DestSubfield = DestSubfield
                                    } );
                            }
                            else
                            {
                                OnError( "Error reading bindings: invalid destproperty defined in 'Bindings' sheet: " + BindingRow["destproperty"].ToString() );
                                ret = false;
                            }
                        }
                        else
                        {
                            OnError( "Error reading bindings: invalid destnodetype defined in 'Bindings' sheet: " + DestNTName );
                            ret = false;
                        }
                    } // if( false == string.IsNullOrEmpty( DestNTName ) )
                } // foreach( DataRow BindingRow in BindingsDataTable.Rows )


                // Row Relationships
                DataTable RelationshipsDataTable = ExcelDataSet.Tables["Relationships$"];
                foreach( DataRow RelRow in RelationshipsDataTable.Rows )
                {
                    CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( RelRow["nodetype"].ToString() );
                    if( null != NodeType )
                    {
                        CswNbtMetaDataNodeTypeProp Relationship = NodeType.getNodeTypeProp( RelRow["relationship"].ToString() );
                        if( null != Relationship )
                        {
                            RowRelationships.Add( Relationship );
                        }
                        else
                        {
                            OnError( "Error reading bindings: invalid Relationship defined in 'Relationships' sheet: " + RelRow["relationship"].ToString() );
                            ret = false;
                        }
                    }
                    else
                    {
                        OnError( "Error reading bindings: invalid NodeType defined in 'Relationships' sheet: " + RelRow["nodetype"].ToString() );
                        ret = false;
                    }
                } // foreach( DataRow RelRow in RelationshipsDataTable.Rows )
            } // if( ExcelDataSet.Tables.Count == 3 )
            else
            {
                OnError( "Error reading bindings: 3 sheets not found in spreadsheet" );
                ret = false;
            }

            return ret;
        } // readBindings()


        private string _NodeTypePkColName( CswNbtMetaDataNodeType NodeType )
        {
            return CswNbt2DBinding.SafeColName( NodeType.NodeTypeName ) + "_nodeid";
        }

        //public void Import( string FilePath )
        //{
        //    storeData( FilePath );
        //    ImportRows( 10 );
        //    _CswNbtResources.commitTransaction();
        //}

        public void ImportRows( Int32 RowsToImport )
        {
            Int32 RowsImported = 0;
            try
            {

                foreach( CswNbtMetaDataNodeType NodeType in ImportOrder.Values )
                {
                    CswTableUpdate ImportDataUpdate = _CswNbtResources.makeCswTableUpdate( "Importer_Update", ImportDataTableName );

                    // Fetch the next row to process
                    bool moreRows = true;
                    while( moreRows )
                    {
                        DataTable ImportDataTable = ImportDataUpdate.getTable( "where error = '" + CswConvert.ToDbVal( false ) + "' and " + _NodeTypePkColName( NodeType ) + " is null",
                                                                               new Collection<OrderByClause> { new OrderByClause( "importdataid", OrderByType.Ascending ) },
                                                                               0, 1 );
                        moreRows = ( ImportDataTable.Rows.Count > 0 );
                        if( moreRows )
                        {
                            DataRow ImportRow = ImportDataTable.Rows[0];
                            try
                            {
                                CswNbtNode Node = null;

                                // Check for non-null values for all required properties
                                IEnumerable<CswNbt2DBinding> RequiredBindings = Bindings.Where( b => b.DestNodeType == NodeType && b.DestProperty.IsRequired );
                                foreach( CswNbt2DBinding Binding in RequiredBindings )
                                {
                                    if( string.IsNullOrEmpty( ImportRow[Binding.ImportDataColumnName].ToString() ) )
                                    {
                                        throw new Exception( "Required property value is missing: " + Binding.SourceColumnName );
                                    }
                                }

                                // Find matching nodes using a view on unique properties
                                IEnumerable<CswNbt2DBinding> UniqueBindings = Bindings.Where( b => b.DestNodeType == NodeType && ( b.DestProperty.IsUnique() || b.DestProperty.IsCompoundUnique() ) );
                                if( false == UniqueBindings.Any() )
                                {
                                    // If no unique properties, use properties in the name template
                                    UniqueBindings = Bindings.Where( b => b.DestNodeType == NodeType && ( NodeType.NameTemplatePropIds.Contains( b.DestProperty.FirstPropVersionId ) ) );
                                }

                                CswNbtView UniqueView = new CswNbtView( _CswNbtResources );
                                UniqueView.ViewName = "Check Unique";
                                CswNbtViewRelationship NTRel = UniqueView.AddViewRelationship( NodeType, false );

                                if( UniqueBindings.Any() )
                                {
                                    foreach( CswNbt2DBinding Binding in UniqueBindings )
                                    {
                                        string Value = ImportRow[Binding.ImportDataColumnName].ToString();
                                        if( Value != string.Empty )
                                        {
                                            UniqueView.AddViewPropertyAndFilter( NTRel, Binding.DestProperty,
                                                                                 Conjunction: CswNbtPropFilterSql.PropertyFilterConjunction.And,
                                                                                 SubFieldName: Binding.DestSubfield.Name,
                                                                                 FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals,
                                                                                 Value: ImportRow[Binding.ImportDataColumnName].ToString(),
                                                                                 CaseSensitive: false );
                                        }
                                        else
                                        {
                                            UniqueView.AddViewPropertyAndFilter( NTRel, Binding.DestProperty,
                                                                                 Conjunction: CswNbtPropFilterSql.PropertyFilterConjunction.And,
                                                                                 SubFieldName: Binding.DestSubfield.Name,
                                                                                 FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Null );
                                        }
                                    }

                                    ICswNbtTree UniqueTree = _CswNbtResources.Trees.getTreeFromView( UniqueView, false, true, true );
                                    if( UniqueTree.getChildNodeCount() > 0 )
                                    {
                                        UniqueTree.goToNthChild( 0 );
                                        Node = UniqueTree.getNodeForCurrentPosition();
                                    }
                                }

                                bool isNewNode = false;
                                if( null == Node )
                                {
                                    // Make a new node
                                    Node = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeType.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                                    isNewNode = true;
                                }


                                // Save the nodeid in this row
                                ImportRow[_NodeTypePkColName( NodeType )] = CswConvert.ToDbVal( Node.NodeId.PrimaryKey );
                                ImportDataUpdate.update( ImportDataTable );


                                // Import property values
                                if( isNewNode || Overwrite )
                                {
                                    foreach( CswNbt2DBinding Binding in Bindings.Where( b => b.DestNodeType == NodeType ) )
                                    {
                                        Node.Properties[Binding.DestProperty].SetPropRowValue( Binding.DestSubfield.Column, ImportRow[Binding.ImportDataColumnName].ToString() );
                                        Node.Properties[Binding.DestProperty].SyncGestalt();
                                    }

                                    foreach( CswNbtMetaDataNodeTypeProp Relationship in RowRelationships.Where( r => r.NodeTypeId == NodeType.NodeTypeId ) )
                                    {
                                        CswNbtMetaDataNodeType TargetNodeType = null;
                                        if( Relationship.FKType == NbtViewRelatedIdType.NodeTypeId.ToString() )
                                        {
                                            TargetNodeType = ImportOrder.Values.FirstOrDefault( n => n.NodeTypeId == Relationship.FKValue );
                                        }
                                        else if( Relationship.FKType == NbtViewRelatedIdType.ObjectClassId.ToString() )
                                        {
                                            TargetNodeType = ImportOrder.Values.FirstOrDefault( n => n.ObjectClassId == Relationship.FKValue );
                                        }
                                        if( null != TargetNodeType )
                                        {
                                            Node.Properties[Relationship].SetPropRowValue(
                                                Relationship.getFieldTypeRule().SubFields[CswNbtSubField.SubFieldName.NodeID].Column,
                                                ImportRow[_NodeTypePkColName( TargetNodeType )]
                                                );
                                        }
                                    } // foreach( CswNbtMetaDataNodeTypeProp Relationship in _Relationships.Where( r => r.NodeTypeId == NodeType.NodeTypeId ) )
                                    Node.postChanges( false );

                                    OnMessage( "Imported " + NodeType.NodeTypeName + ": " + Node.NodeName + " (" + Node.NodeId.PrimaryKey.ToString() + ")" );
                                } // if(isNewNode || Overwrite )
                                else
                                {
                                    OnMessage( "Skipped  " + NodeType.NodeTypeName + ": " + Node.NodeName + " (" + Node.NodeId.PrimaryKey.ToString() + ")" );
                                }


                                // Simplify future imports by saving this nodeid on matching rows
                                if( UniqueBindings.Any() )
                                {
                                    string WhereClause = "where error = '" + CswConvert.ToDbVal( false ) + "' and " + _NodeTypePkColName( NodeType ) + " is null";
                                    foreach( CswNbt2DBinding Binding in UniqueBindings )
                                    {
                                        WhereClause += " and lower(" + Binding.ImportDataColumnName + ") = '" + CswTools.SafeSqlParam( ImportRow[Binding.ImportDataColumnName].ToString().ToLower() ) + "' ";
                                    }

                                    DataTable OtherImportDataTable = ImportDataUpdate.getTable( WhereClause );
                                    foreach( DataRow OtherImportRow in OtherImportDataTable.Rows )
                                    {
                                        OtherImportRow[_NodeTypePkColName( NodeType )] = CswConvert.ToDbVal( Node.NodeId.PrimaryKey );
                                    }
                                    ImportDataUpdate.update( OtherImportDataTable );
                                } // if( UniqueBindings.Any() )

                                RowsImported += 1;
                            }
                            catch( Exception ex )
                            {
                                string ErrorMsg = ex.Message + "\r\n" + ex.StackTrace;
                                ImportRow["error"] = CswConvert.ToDbVal( true );
                                ImportRow["errorlog"] = ErrorMsg;
                                OnError( ErrorMsg );
                                ImportDataUpdate.update( ImportDataTable );
                            }
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
            }
            catch( Exception ex )
            {
                OnError( ex.Message + "\r\n" + ex.StackTrace );
            }
        } // ImportRows()

    } // class CswNbt2DImporter
} // namespace ChemSW.Nbt.ImportExport
