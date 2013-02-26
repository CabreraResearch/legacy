using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
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

        private readonly CswNbtResources _CswNbtResources;
        private readonly CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswNbt2DImporter( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtSchemaModTrnsctn = new CswNbtSchemaModTrnsctn( _CswNbtResources );
        }

        /// <summary>
        /// Stores data in a temporary Oracle table
        /// Returns the name of the table
        /// </summary>
        public string _storeData( string FilePath )
        {
            //Set up ADO connection to spreadsheet
            string ConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + FilePath + ";Extended Properties=Excel 8.0;";
            OleDbConnection ExcelConn = new OleDbConnection( ConnStr );
            ExcelConn.Open();

            DataTable ExcelMetaDataTable = ExcelConn.GetOleDbSchemaTable( OleDbSchemaGuid.Tables, null );
            if( null == ExcelMetaDataTable )
            {
                throw new CswDniException( ErrorType.Error, "Could not process the uploaded file: " + FilePath, "GetOleDbSchemaTable failed to parse a valid XLS file." );
            }
            string FirstSheetName = ExcelMetaDataTable.Rows[0]["TABLE_NAME"].ToString();
            //string FirstSheetName = "Sheet1";

            OleDbDataAdapter DataAdapter = new OleDbDataAdapter();
            OleDbCommand SelectCommand = new OleDbCommand( "SELECT * FROM [" + FirstSheetName + "]", ExcelConn );
            DataAdapter.SelectCommand = SelectCommand;

            DataTable ExcelDataTable = new DataTable();
            DataAdapter.Fill( ExcelDataTable );


            // Generate an Oracle table for storing and manipulating data
            Int32 i = 1;
            string ImportDataTableName = "importdata" + i.ToString();
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

            return ImportDataTableName;
        } // _storeData()


        private string _NodeTypePkColName( CswNbtMetaDataNodeType NodeType )
        {
            return CswNbt2DBinding.SafeColName( NodeType.NodeTypeName ) + "_nodeid";
        }

        public void Import( string FilePath )
        {
            string ImportDataTableName = _storeData( FilePath );
            ImportRows( ImportDataTableName, 10 );
            _CswNbtResources.commitTransaction();
        }

        public void ImportRows( string ImportDataTableName, Int32 RowsToImport )
        {
            Int32 RowsImported = 0;

            foreach( CswNbtMetaDataNodeType NodeType in ImportOrder.Values )
            {
                CswTableUpdate ImportDataUpdate = _CswNbtResources.makeCswTableUpdate( "Importer_Update", ImportDataTableName );

                // Fetch the next row to process
                bool moreRows = true;
                while( moreRows )
                {
                    DataTable ImportDataTable = ImportDataUpdate.getTable( "where error = '" + CswConvert.ToDbVal( false ) + "' and " + _NodeTypePkColName( NodeType ) + " is null",
                                                                           new Collection<OrderByClause> {new OrderByClause( "importdataid", OrderByType.Ascending )},
                                                                           0, 1 );
                    moreRows = ( ImportDataTable.Rows.Count > 0 );
                    if( moreRows )
                    {
                        DataRow ImportRow = ImportDataTable.Rows[0];
                        try
                        {
                            CswNbtNode Node = null;

                            // Find matching nodes using a view on unique properties
                            IEnumerable<CswNbt2DBinding> UniqueBindings = Bindings.Where( b => b.DestNodeType == NodeType && ( b.DestProperty.IsUnique() || b.DestProperty.IsCompoundUnique() ) );

                            CswNbtView UniqueView = new CswNbtView( _CswNbtResources );
                            UniqueView.ViewName = "Check Unique";
                            CswNbtViewRelationship NTRel = UniqueView.AddViewRelationship( NodeType, false );

                            if( UniqueBindings.Any() )
                            {
                                foreach( CswNbt2DBinding Binding in UniqueBindings )
                                {
                                    UniqueView.AddViewPropertyAndFilter( NTRel, Binding.DestProperty,
                                                                         Conjunction: CswNbtPropFilterSql.PropertyFilterConjunction.And,
                                                                         SubFieldName: Binding.DestSubfield.Name,
                                                                         FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals,
                                                                         Value: ImportRow[Binding.ImportDataColumnName].ToString(),
                                                                         CaseSensitive: false );
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
                            } // if(isNewNode || Overwrite )


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
                            ImportRow["error"] = CswConvert.ToDbVal( true );
                            ImportRow["errorlog"] = ex.Message + "\r\n" + ex.StackTrace;
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
        } // ImportRows()

    } // class CswNbt2DImporter
} // namespace ChemSW.Nbt.ImportExport
