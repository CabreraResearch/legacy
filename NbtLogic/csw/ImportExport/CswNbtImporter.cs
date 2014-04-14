using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.ImportExport;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Schema;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Security;

namespace ChemSW.Nbt.ImportExport
{
    public partial class CswNbtImporter
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswNbtImporter( string AccessId, CswEnumSetupMode SetupMode )
        {
            if( CswEnumSetupMode.NbtExe == SetupMode || CswEnumSetupMode.NbtWeb == SetupMode )
            {
                _CswNbtResources = CswNbtResourcesFactory.makeCswNbtResources( CswEnumAppType.Nbt, SetupMode, false ); //ExcludeDisabledModules needs to be false
                _CswNbtResources.AccessId = AccessId;
                _CswNbtResources.InitCurrentUser = _initUser;
                _CswNbtSchemaModTrnsctn = new CswNbtSchemaModTrnsctn( _CswNbtResources );
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Error, "Error initializing CswNbtImporter", "CswNbtImporter was passed an invalid SetupMode: " + SetupMode.ToString() );
            }
        }

        private ICswNbtUser _initUser( ICswResources Resources )
        {
            return new CswNbtSystemUser( Resources, CswEnumSystemUserNames.SysUsr__SchemaImport );
        }

        public void Finish()
        {
            _CswNbtResources.finalize();
        }

        public void refreshMetaData()
        {
            _CswNbtResources.MetaData.refreshAll();
        }

        public delegate void MessageHandler( string Message );

        public MessageHandler OnMessage = delegate( string Message ) { };

        /// <summary>
        /// Stores new import definition
        /// </summary>
        public void storeDefinition( string FullFilePath, string ImportDefinitionName )
        {
            //construct the excel tables from the uploaded bindings. If this worked correctly,
            //we should get three tables: "Order$", "Bindings$", and "Relationships$"
            DataSet ExcelDataSet = CswNbtImportTools.ReadExcel( FullFilePath );

            if( ExcelDataSet.Tables.Count == 3 )
            {

                Dictionary<string, Int32> DefIdsBySheetName = CswNbtImportDef.addDefinitionEntriesFromExcel( _CswNbtResources, ImportDefinitionName, ExcelDataSet.Tables["Order$"], null );

                //create new tables to hold the data, and index them similarly to ReadExcel() so we can talk about the two in pairs
                //we seem to need this so that the pk column is handled correctly; I'm sure there's an in-place way to do it,
                //but good luck figuring it out.
                Dictionary<string, DataTable> TableUpdates = new Dictionary<string, DataTable>();
                TableUpdates["Order$"] = _CswNbtResources.makeCswTableUpdate( "importdeforder", "import_def_order" ).getTable();
                TableUpdates["Bindings$"] = _CswNbtResources.makeCswTableUpdate( "importdefbinding", "import_def_bindings" ).getTable();
                TableUpdates["Relationships$"] = _CswNbtResources.makeCswTableUpdate( "importdefrelationship", "import_def_relationships" ).getTable();

                //loop through each pair of tables by name
                foreach( string Table in new[] { "Order$", "Bindings$", "Relationships$" } )
                {
                    //for each row in the old table
                    foreach( DataRow Row in ExcelDataSet.Tables[Table].Rows )
                    {
                        if( false == Row.IsNull( "sheetname" ) )
                        {
                            //create a new row from the equivalent new table
                            DataRow NewRow = TableUpdates[Table].NewRow();

                            //foreach column of the old row
                            foreach( DataColumn Column in ExcelDataSet.Tables[Table].Columns )
                            {
                                //if the cell is sheetname convert it to the appropriate importdefid, otherwise copy it directly if there's something there
                                if( Column.ColumnName.ToLower() == "sheetname" )
                                {
                                    NewRow["importdefid"] = DefIdsBySheetName[Row["sheetname"].ToString()];
                                }
                                else if( false == string.IsNullOrEmpty( Row[Column].ToString() ) )
                                {
                                    NewRow[Column.ColumnName] = Row[Column];
                                }
                            } //for each column in the row

                            //add the new row to the new table
                            TableUpdates[Table].Rows.Add( NewRow );
                        }//if the row isn't blank
                    }//for each row in the table
                }//for each table in the excel sheet

                storeDefinition( TableUpdates["Order$"], TableUpdates["Bindings$"], TableUpdates["Relationships$"] );
            } // if( ExcelDataSet.Tables.Count == 3 )
            else
            {
                throw new CswDniException( CswEnumErrorType.Error, "Error reading file", "3 sheets not found in uploaded spreadsheet" );
            }
        }

        public void storeDefinition( DataTable OrderDataTable, DataTable BindingsDataTable, DataTable RelationshipsDataTable )
        {
            CswNbtImportDefOrder.addOrderEntries( _CswNbtResources, OrderDataTable );
            CswNbtImportDefBinding.addBindingEntries( _CswNbtResources, BindingsDataTable );
            CswNbtImportDefRelationship.addRelationshipEntries( _CswNbtResources, RelationshipsDataTable );

        }

        /// <summary>
        /// Import a single row (for CAF imports)
        /// </summary>
        /// <returns>Non-null string indicates an error</returns>
        public string ImportRow( DataRow SourceRow, string ImportDefinitionName, string NodeType, bool Overwrite )
        {
            string Error = string.Empty;

            if( false == string.IsNullOrEmpty( ImportDefinitionName ) )
            {
                CswNbtImportDef BindingDef = new CswNbtImportDef( _CswNbtResources, ImportDefinitionName, "CAF" );
                if( null != BindingDef && ( BindingDef.Bindings.Count > 0 || BindingDef.RowRelationships.Count > 0 ) && BindingDef.ImportOrder.Count > 0 )
                {
                    foreach( CswNbtImportDefOrder Order in BindingDef.ImportOrder.Values.Where( order => order.NodeTypeName == NodeType ) )
                    {
                        try
                        {
                            _ImportOneRow( SourceRow, BindingDef, Order, Overwrite, null, OverrideUniqueValidation: true );
                        }
                        catch( Exception e )
                        {
                            Error = "Failed to import row: " + e.Message;
                        }
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
                // case 31008 - Checking DataMap.Completed here allows us to cancel
                CswNbtImportDataMap DataMap = new CswNbtImportDataMap( _CswNbtResources, ImportDataTableName );
                if( false == DataMap.Completed )
                {
                    // Lookup the binding definition
                    CswNbtImportDef BindingDef = new CswNbtImportDef( _CswNbtResources, DataMap.ImportDefinitionId );
                    if( ( BindingDef.Bindings.Count > 0 || BindingDef.RowRelationships.Count > 0 ) && BindingDef.ImportOrder.Count > 0 )
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
                } // if( false == DataMap.Completed )
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
        private CswPrimaryKey _ImportOneRow( DataRow ImportRow, CswNbtImportDef BindingDef, CswNbtImportDefOrder Order, bool Overwrite, CswTableUpdate ImportDataUpdate, bool OverrideUniqueValidation = false )
        {
            CswPrimaryKey ImportedNodeId;
            CswNbtNode Node = null;

            IEnumerable<CswNbtImportDefBinding> NodeTypeBindings = BindingDef.Bindings.Where( b => null != b.DestNodeType && 
                                                                                                   b.DestNodeType == Order.NodeType && 
                                                                                                   b.Instance == Order.Instance );
            CswNbtImportDefBinding LegacyIdBinding = NodeTypeBindings.FirstOrDefault( Binding => Binding.DestPropName == "Legacy Id" );
            NodeTypeBindings = NodeTypeBindings.Where( Binding => Binding.DestPropName != "Legacy Id" );

            if( false == NodeTypeBindings.Any() )
            {
                throw new CswDniException( CswEnumErrorType.Error, "No bindings are defined for Row.", "No bindings passed the filter. Are appropriate nodetypes enabled in NBT?" );
            }
            IEnumerable<CswNbtImportDefRelationship> RowRelationships = BindingDef.RowRelationships.Where( r => r.NodeType.NodeTypeId == Order.NodeType.NodeTypeId ); //&& r.Instance == Order.Instance );
            IEnumerable<CswNbtImportDefRelationship> UniqueRelationships = RowRelationships.Where( r => r.Relationship.IsUnique() ||
                                                                                                        r.Relationship.IsCompoundUnique() ||
                                                                                                        Order.NodeType.NameTemplatePropIds.Contains( r.Relationship.FirstPropVersionId ) );
            IEnumerable<CswNbtImportDefBinding> UniqueBindings = NodeTypeBindings.Where( b => b.DestProperty.IsUnique() || b.DestProperty.IsCompoundUnique() );
            if( false == UniqueBindings.Any() ) // case 30821
            {
                UniqueBindings = NodeTypeBindings.Where( b => Order.NodeType.NameTemplatePropIds.Contains( b.DestProperty.FirstPropVersionId ) );
            }

            bool allEmpty = true;
            // Skip rows with null values for all unique properties
            foreach( CswNbtImportDefBinding Binding in UniqueBindings )
            {
                allEmpty = allEmpty && string.IsNullOrEmpty( ImportRow[Binding.ImportDataColumnName].ToString() );
            }
            foreach( CswNbtImportDefRelationship Relation in UniqueRelationships )
            {
                Int32 Value = _getRelationValue( BindingDef, Relation, ImportRow );
                allEmpty = allEmpty && ( Value != Int32.MinValue );
            }

            string LegacyId = string.Empty;
            if( null != LegacyIdBinding )
            {
                LegacyId = ImportRow[LegacyIdBinding.ImportDataColumnName].ToString();
                allEmpty = false;
            }

            if( false == allEmpty )
            {
                bool foundMatch = false;
                if( false == string.IsNullOrEmpty( LegacyId ) ) //Check for matching nodes using a view on legacy id
                {
                    CswTableSelect LegacyNodeSelect = _CswNbtResources.makeCswTableSelect( "LegacyNodeSelect", "nodes" );
                    DataTable LegacyNodesTable = LegacyNodeSelect.getTable( "where nodetypeid = " + Order.NodeType.NodeTypeId + " and legacyid = '" + LegacyId + "'" );
                    if( LegacyNodesTable.Rows.Count > 0 )
                    {
                        if( Overwrite )
                        {
                            CswPrimaryKey NodeId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( LegacyNodesTable.Rows[0]["nodeid"] ) );
                            Node = _CswNbtResources.Nodes[NodeId];
                            _importPropertyValues( BindingDef, NodeTypeBindings, RowRelationships, ImportRow, Node );
                            Node.postChanges( false, false, true );
                        }
                        foundMatch = true;
                    }
                }

                if( false == foundMatch && false == OverrideUniqueValidation )
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
                            Int32 Value = _getRelationValue( BindingDef, Relation, ImportRow );

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
                                Node.LegacyId = LegacyId;
                                if( Overwrite )
                                {
                                    _importPropertyValues( BindingDef, NodeTypeBindings, RowRelationships, ImportRow, Node );
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
                            NewNode.LegacyId = LegacyId;
                            _importPropertyValues( BindingDef, NodeTypeBindings, RowRelationships, ImportRow, NewNode );

                            // Specific logic for Roles (see Case 31043)
                            if( Order.NodeType.NodeTypeName == "Role" )
                            {
                                _setRolePermissions( NewNode, CswEnumTristate.True == NewNode.Properties[CswNbtObjClassRole.PropertyName.Administrator].AsLogical.Checked ? "CISPro_Admin" : "CISPro_General" );
                            }
                            else if( Order.NodeTypeName == "Container" )// More Specific Logic (see CIS-52852)
                            {
                                CswNbtObjClassContainer Container = NewNode;
                                double Qty = Container.Quantity.Quantity;
                                Container.Quantity.Quantity = 0;
                                Container.Dispenser = new CswNbtContainerDispenser( _CswNbtResources, new CswNbtContainerDispenseTransactionBuilder( _CswNbtResources ), Container, IsImport: true );
                                Container.DispenseIn( CswEnumNbtContainerDispenseType.Receive, Qty, Container.Quantity.UnitId );
                            }

                        }, OverrideUniqueValidation: true, OverrideMailReportEvents: true ); //even when we care about uniqueness, we've already checked it above and this would be redundant
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
                            Int32 Value = _getRelationValue( BindingDef, Relation, ImportRow );
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

        private Int32 _getRelationValue( CswNbtImportDef BindingDef, CswNbtImportDefRelationship Relation, DataRow ImportRow )
        {
            CswNbtImportDefOrder thisTargetOrder = BindingDef.ImportOrder.Values.FirstOrDefault( o => Relation.Relationship.FkMatches( o.NodeType ) &&
                                                                                                      o.Instance == Relation.Instance );
            Int32 Value = Int32.MinValue;
            if( null != thisTargetOrder )
            {
                if( ImportRow.Table.Columns.Contains( Relation.SourceRelColumnName ) &&
                    null != ImportRow[Relation.SourceRelColumnName] )
                {
                    Value = CswConvert.ToInt32( ImportRow[Relation.SourceRelColumnName] );
                }
                else
                {
                    Value = CswConvert.ToInt32( ImportRow[thisTargetOrder.PkColName] );
                }
            }
            return Value;
        } // _getRelationValue()

        private void _importPropertyValues( CswNbtImportDef BindingDef, IEnumerable<CswNbtImportDefBinding> NodeTypeBindings, IEnumerable<CswNbtImportDefRelationship> RowRelationships, DataRow ImportRow, CswNbtNode Node )
        {
            // case 30821
            Node.IsDemo = false;

            // Iterate each binding 
            foreach( CswNbtImportDefBinding Binding in NodeTypeBindings )
            {
                // See CIS-53175: Legacy Id is no longer a property
                if( Binding.DestPropName != "Legacy Id" )
                {
                    string PropertyData = string.Empty;
                    byte[] BlobData = null;
                    if( String.IsNullOrEmpty( Binding.SourceBlobTableName ) && String.IsNullOrEmpty( Binding.SourceClobTableName ) )
                    {
                        PropertyData = ImportRow[Binding.ImportDataColumnName].ToString();
                    }
                    else //get lob data
                    {
                        _getLobData( Binding, ImportRow, out PropertyData, out BlobData );
                    }

                    CswEnumNbtFieldType DestPropertyFieldType = Binding.DestProperty.getFieldTypeValue();
                    // Special case for TimeInterval, specifically for IMCS imports
                    if( DestPropertyFieldType == CswEnumNbtFieldType.TimeInterval )
                    {
                        XElement input = XElement.Parse( "<rateintervalvalue>" + PropertyData.ToLower() + "</rateintervalvalue>" );
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load( input.CreateReader() );

                        CswRateInterval rateInterval = new CswRateInterval( _CswNbtResources );
                        rateInterval.ReadXml( xmlDoc.DocumentElement );

                        ( (CswNbtNodePropTimeInterval) Node.Properties[Binding.DestProperty] ).RateInterval = rateInterval;
                        Node.Properties[Binding.DestProperty].SyncGestalt();
                    }
                    else if( ( DestPropertyFieldType == CswEnumNbtFieldType.File && Binding.DestSubFieldName != CswEnumNbtSubFieldName.Href.ToString() )
                        || DestPropertyFieldType == CswEnumNbtFieldType.Image )
                    {
                        CswNbtSdBlobData sdBlobData = new CswNbtSdBlobData( _CswNbtResources );
                        if( Int32.MinValue == Node.Properties[Binding.DestProperty].JctNodePropId )
                        {
                            Node.Properties[Binding.DestProperty].makePropRow();
                        }
                        int BlobDataId = sdBlobData.GetBlobDataId( Node.Properties[Binding.DestProperty].JctNodePropId );

                        CswTableUpdate blobDataTblUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "importer.fileimport", "blob_data" );

                        DataTable blobDataTbl = ( Int32.MinValue != BlobDataId ? blobDataTblUpdate.getTable( "where blobdataid = " + BlobDataId ) : blobDataTblUpdate.getEmptyTable() );
                        DataRow blobDataRow = null;
                        if( blobDataTbl.Rows.Count > 0 )
                        {
                            blobDataRow = blobDataTbl.Rows[0];
                        }
                        else
                        {
                            blobDataRow = blobDataTbl.NewRow();
                            blobDataRow["jctnodepropid"] = Node.Properties[Binding.DestProperty].JctNodePropId;
                            blobDataTbl.Rows.Add( blobDataRow );
                        }

                        if( CswEnumNbtSubFieldName.Name.ToString() == Binding.DestSubFieldName )
                        {
                            blobDataRow["filename"] = PropertyData;
                        }
                        else if( CswEnumNbtSubFieldName.ContentType.ToString() == Binding.DestSubFieldName )
                        {
                            blobDataRow["contenttype"] = PropertyData;
                        }
                        else if( CswEnumNbtSubFieldName.Blob.ToString() == Binding.DestSubFieldName )
                        {
                            blobDataRow["blobdata"] = BlobData;
                        }

                        blobDataTblUpdate.update( blobDataTbl );
                    }
                    // NodeTypeSelect
                    else if( DestPropertyFieldType == CswEnumNbtFieldType.NodeTypeSelect )
                    {
                        CswNbtMetaDataNodeType nt = _CswNbtResources.MetaData.getNodeType( PropertyData.ToString() );
                        if( nt != null )
                        {
                            Node.Properties[Binding.DestProperty].AsNodeTypeSelect.SelectedNodeTypeIds = new CswCommaDelimitedString() { nt.NodeTypeId.ToString() };
                        }
                        else
                        {
                            OnMessage( "Skipped invalid nodetype: " + PropertyData );
                        }
                    }
                    // Quantity, Relationship, or Location
                    else if( ( DestPropertyFieldType == CswEnumNbtFieldType.Quantity &&
                                 ( Binding.DestSubfield.Column.ToString() == CswEnumNbtPropColumn.Field1_FK.ToString() || Binding.DestSubfield.Name.ToString().ToLower() == "name" ) )
                                || DestPropertyFieldType == CswEnumNbtFieldType.Relationship
                                || DestPropertyFieldType == CswEnumNbtFieldType.Location )
                    {
                        CswCommaDelimitedString inClause = new CswCommaDelimitedString();

                        if( Binding.DestProperty.FKType == CswEnumNbtViewRelatedIdType.NodeTypeId.ToString() )
                        {
                            inClause.Add( Binding.DestProperty.FKValue.ToString() );
                        }
                        else if( Binding.DestProperty.FKType == CswEnumNbtViewRelatedIdType.ObjectClassId.ToString() )
                        {
                            CswNbtMetaDataObjectClass oc = _CswNbtResources.MetaData.getObjectClass( Binding.DestProperty.FKValue );
                            foreach( CswNbtMetaDataNodeType nt in oc.getNodeTypes() )
                            {
                                inClause.Add( nt.NodeTypeId.ToString() );
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
                                                                               DestPropertyFieldType,
                                                                               Binding.DestProperty );
                        }
                        if( false == MatchedOnLegacyId )
                        {
                            // Alternatively, we try to search based on the Name property
                            _relationshipSearchViaName( Node, inClause, ImportRow, Binding );
                        }

                    }
                    else if( false == String.IsNullOrEmpty( PropertyData ) )
                    {
                        Node.Properties[Binding.DestProperty].SetSubFieldValue( Binding.DestSubfield, PropertyData );
                    }
                }
            }//foreach( CswNbtImportDefBinding Binding in NodeTypeBindings )

            #region CswNbtImportDefRelationship

            foreach( CswNbtImportDefRelationship RowRelationship in RowRelationships )
            {
                CswNbtImportDefOrder TargetOrder = BindingDef.ImportOrder.Values.FirstOrDefault( o => RowRelationship.Relationship.FkMatches( o.NodeType ) && o.Instance == RowRelationship.Instance );

                // If we have a value for the SourceRelColumnName
                if( false == string.IsNullOrEmpty( RowRelationship.SourceRelColumnName ) &&
                    ImportRow.Table.Columns.Contains( RowRelationship.SourceRelColumnName ) &&
                    null != ImportRow[RowRelationship.SourceRelColumnName] )
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
                                                        RowRelationship.Relationship );
                    }
                }
                else
                {
                    // In this case, we are matching on NodeId
                    if( null != TargetOrder && null != ImportRow[TargetOrder.PkColName] && CswConvert.ToInt32( ImportRow[TargetOrder.PkColName] ) > 0 )
                    {
                        Node.Properties[RowRelationship.Relationship].SetSubFieldValue( CswEnumNbtSubFieldName.NodeID, ImportRow[TargetOrder.PkColName] );

                        if( RowRelationship.Relationship.getFieldTypeValue() == CswEnumNbtFieldType.Relationship )
                        {
                            Node.Properties[RowRelationship.Relationship].AsRelationship.RefreshNodeName();
                        }
                        if( RowRelationship.Relationship.getFieldTypeValue() == CswEnumNbtFieldType.Location )
                        {
                            Node.Properties[RowRelationship.Relationship].AsLocation.RefreshNodeName();
                        }
                        if( RowRelationship.Relationship.getFieldTypeValue() == CswEnumNbtFieldType.Quantity )
                        {
                            Node.Properties[RowRelationship.Relationship].AsQuantity.RefreshNodeName();
                        }

                        Node.Properties[RowRelationship.Relationship].SyncGestalt();
                    }
                }
            } // foreach( CswNbtMetaDataNodeTypeProp Relationship in RowRelationships )

            #endregion CswNbtImportDefRelationship

        } // _importPropertyValues()

        // This should actually set the value if there is one and return true if it was set and false if not
        private bool _relationshipSearchViaLegacyId( CswNbtNode Node, string LegacyId, CswEnumNbtFieldType FieldType, CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            bool Ret = false;

            if( false == string.IsNullOrEmpty( LegacyId ) )
            {
                //CIS-53123 - use this instead of treeloader for performance
                String MetaDataClause = "";
                switch( NodeTypeProp.FKType )
                {
                    case "PropertySetId":
                        CswNbtMetaDataPropertySet PropertySet = _CswNbtResources.MetaData.getPropertySet( NodeTypeProp.FKValue );
                        MetaDataClause = @" join nodetypes t on n.nodetypeid = t.nodetypeid
                                            join object_class o on t.objectclassid = o.objectclassid
                                            join jct_propertyset_objectclass jpo on (o.objectclassid = jpo.objectclassid) 
                                            join property_set ps on (jpo.propertysetid = ps.propertysetid) 
                                            where (ps.propertysetid = " + PropertySet.PropertySetId + ") ";
                        break;
                    case "ObjectClassId":
                        CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( NodeTypeProp.FKValue );
                        MetaDataClause = @" join nodetypes t on n.nodetypeid = t.nodetypeid
                                            join object_class o on t.objectclassid = o.objectclassid
                                            where (o.objectclassid = " + ObjectClass.ObjectClassId + ") ";
                        break;
                    case "NodeTypeId":
                        CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeProp.FKValue );
                        MetaDataClause = @" join nodetypes t on n.nodetypeid = t.nodetypeid
                                            where (t.firstversionid = " + NodeType.NodeTypeId + ") ";
                        break;
                }
                String Sql = @"select n.nodeid from nodes n 
                                " + MetaDataClause + @"
                                and n.legacyid = '" + LegacyId + "'";
                CswArbitrarySelect LegacyNodeIdSelect = _CswNbtResources.makeCswArbitrarySelect( "LegacyIdRel_select", Sql );
                DataTable LegacyNodeIdTable = LegacyNodeIdSelect.getTable();
                if( LegacyNodeIdTable.Rows.Count > 0 )
                {
                    int LegacyNodeId = CswConvert.ToInt32( LegacyNodeIdTable.Rows[0]["nodeid"] );
                    // Set the relationship property to the nodeid of the found node
                    Node.Properties[NodeTypeProp].SetSubFieldValue( CswEnumNbtSubFieldName.NodeID, LegacyNodeId );

                    // Refresh
                    if( FieldType == CswEnumNbtFieldType.Relationship )
                    {
                        Node.Properties[NodeTypeProp].AsRelationship.RefreshNodeName();
                    }
                    if( FieldType == CswEnumNbtFieldType.Location )
                    {
                        Node.Properties[NodeTypeProp].AsLocation.RefreshNodeName();
                    }
                    if( FieldType == CswEnumNbtFieldType.Quantity )
                    {
                        Node.Properties[NodeTypeProp].AsQuantity.RefreshNodeName();
                    }
                    Node.Properties[NodeTypeProp].SyncGestalt();

                    Ret = true;
                }
            }
            return Ret;
        }

        private void _relationshipSearchViaName( CswNbtNode Node, CswCommaDelimitedString inClause, DataRow ImportRow, CswNbtImportDefBinding Binding )
        {
            if( false == String.IsNullOrEmpty( ImportRow[Binding.ImportDataColumnName].ToString() ) )
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
        }

        private void _importBlobData( string SourceTableName, Int32 Pk, string SourceColName, string SourcePkColName )
        {
            int ImportLobPk = _CswNbtResources.getNewPrimeKey( "import_lob" );
            string sql = @"insert into Import_Lob (ImportLobId, Blobdata, Tablename, Cafpk) select " + ImportLobPk + ", " + SourceColName + ", '" + SourceTableName + "', " + Pk + " from " + SourceTableName + "@caflink where " + SourcePkColName + " = " + Pk;
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( sql );
        }

        private void _importClobData( string SourceTableName, Int32 Pk, string SourceColName, string SourcePkColName )
        {
            int ImportLobPk = _CswNbtResources.getNewPrimeKey( "import_lob" );
            string sql = @"insert into Import_Lob (ImportLobId, Clobdata, Tablename, Cafpk) select " + ImportLobPk + ", " + SourceColName + ", '" + SourceTableName + "', " + Pk + " from " + SourceTableName + "@caflink where " + SourcePkColName + " = " + Pk;
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( sql );
        }

        private void _getLobData( CswNbtImportDefBinding Binding, DataRow ImportRow, out string PropertyData, out byte[] BlobData )
        {
            PropertyData = string.Empty;
            BlobData = null;

            int LobDataPK = CswConvert.ToInt32( ImportRow[Binding.SourceLobDataPkColOverride] );
            if( Int32.MinValue != LobDataPK )
            {
                string SourceTableName = ( String.IsNullOrEmpty( Binding.SourceBlobTableName ) ? Binding.SourceClobTableName : Binding.SourceBlobTableName );
                string SourceColName = ( false == String.IsNullOrEmpty( Binding.LobDataPkColName ) ? Binding.LobDataPkColName : Binding.SourceLobDataPkColOverride );

                if( String.IsNullOrEmpty( Binding.SourceBlobTableName ) )
                {
                    _importClobData( SourceTableName, LobDataPK, Binding.SourceColumnName, SourceColName );
                }
                else
                {
                    _importBlobData( SourceTableName, LobDataPK, Binding.SourceColumnName, SourceColName );
                }

                //get the Lob data
                CswTableUpdate lobDataUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "Importer.GetLobData", "import_lob" );
                DataTable importLobTbl = lobDataUpdate.getTable( "where CafPk = " + LobDataPK + " and Tablename = '" + SourceTableName + "'" );
                if( importLobTbl.Rows.Count > 0 )
                {
                    if( String.IsNullOrEmpty( Binding.SourceBlobTableName ) )
                    {
                        PropertyData = importLobTbl.Rows[0]["clobdata"].ToString();
                    }
                    else
                    {
                        BlobData = importLobTbl.Rows[0]["blobdata"] as byte[];
                        if( null != BlobData )
                        {
                            PropertyData = Encoding.UTF8.GetString( BlobData );
                        }
                    }
                    foreach( DataRow LobDataRow in importLobTbl.Rows )
                    {
                        LobDataRow.Delete();
                    }
                    lobDataUpdate.update( importLobTbl );
                }
            }
        }

        #region NodeType specific import logic

        private void _setRolePermissions( CswNbtNode NewNode, string RoleName )
        {
            CswNbtObjClassRole RoleNode = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( RoleName );
            if( null != RoleNode )
            {
                NewNode.Properties[CswNbtObjClassRole.PropertyName.NodeTypePermissions].SetSubFieldValue(
                    CswEnumNbtSubFieldName.Value,
                    RoleNode.NodeTypePermissions.GetSubFieldValue( CswEnumNbtSubFieldName.Value )
                    );

                NewNode.Properties[CswNbtObjClassRole.PropertyName.ActionPermissions].SetSubFieldValue(
                    CswEnumNbtSubFieldName.Value,
                    RoleNode.ActionPermissions.GetSubFieldValue( CswEnumNbtSubFieldName.Value )
                    );
            }
        }//_setRolePermissions()

        #endregion

    } // class CswNbt2DImporter
} // namespace ChemSW.Nbt.ImportExport
