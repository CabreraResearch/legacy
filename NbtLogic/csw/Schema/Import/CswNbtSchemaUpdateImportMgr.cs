using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.ImportExport;
using ChemSW.Nbt.Sched;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.csw.Schema
{
    partial class CswNbtSchemaUpdateImportMgr
    {
        public CswNbtSchemaModTrnsctn SchemaModTrnsctn;
        public const string LegacyID = "Legacy ID";

        private Dictionary<string, Int32> _SheetOrder = new Dictionary<string, int>
            {
                //{SheetName (view name or table name unless overridden), Order}
                {"cispro_controlzones", 1},
                {"workunits_view", 2},
                {"inventory_groups", 3},
                {"locations_view", 4},
                {"vendors", 5},
                {"roles", 6},
                {"users_view", 7},
                {"reglists_view", 8},
                {"regulated_casnos", 9},
                {"weight_view", 10},
                {"volume_view", 11},
                {"each_view", 12},
                {"rs_phrases", 13}, //DSD Phrases
                {"chemicals_view", 14},
                {"packdetail_view", 15},
                {"sds_view", 16},
                {"docs_view", 17},
                {"receipt_lots_view", 18},
                {"cofa_docs_view", 19},
                {"container_groups", 20},
                {"containers_view", 21},
                {"inventory_view", 22},
                {"regions_view", 23},
                {"ghs_phrases", 24},
                {"ghs_view", 25}
            };

        private DataTable _importDefTable;
        private DataTable _importOrderTable;
        private DataTable _importBindingsTable;
        private DataTable _importRelationshipsTable;

        private Dictionary<string, int> _SheetDefinitions; 
        private string _DestNodeTypeName;
        private string _SourceTableName;
        private string _ViewName;

        private string _CAFDbLink;
        private CswNbtImporter _NbtImporter;

        private CswCommaDelimitedString _SourceColumns;

        public CswNbtSchemaUpdateImportMgr( CswNbtSchemaModTrnsctn SchemaModTrnsctn, string SourceTableName = "", string DestNodeTypeName = "", string ViewName = "", string SourceColumn = "", string CafDbLink = null, string DefinitionName = CswScheduleLogicNbtCAFImport.DefinitionName )
        {
            _CAFDbLink = CafDbLink ?? CswScheduleLogicNbtCAFImport.CAFDbLink;
                _NbtImporter = SchemaModTrnsctn.makeCswNbtImporter();
                this.SchemaModTrnsctn = SchemaModTrnsctn;

                _importDefTable = SchemaModTrnsctn.makeCswTableSelect( "Import_getDefs", "import_def" ).getTable();
                _importOrderTable = SchemaModTrnsctn.makeCswTableSelect( "Import_getOrder", "import_def_order" ).getTable();
                _importBindingsTable = SchemaModTrnsctn.makeCswTableSelect( "Import_getBindings", "import_def_bindings" ).getTable();
                _importRelationshipsTable = SchemaModTrnsctn.makeCswTableSelect( "Import_getRelationships", "import_def_relationships" ).getTable();

                _SourceTableName = SourceTableName.ToLower();
                _DestNodeTypeName = DestNodeTypeName.ToLower();
                _ViewName = ViewName.ToLower();
                _SourceColumns = new CswCommaDelimitedString();
                SourceTablePkColumnName = SourceColumn.ToLower();

            _SheetDefinitions = SchemaModTrnsctn.createImportDefinitionEntries( DefinitionName, _importOrderTable, _importDefTable );

        }//ctor


        public void importDef(string DefinitionName, int SheetOrder, string SheetName = null)
        {
            DataRow row = _importDefTable.NewRow();
            row["sheetname"] = SheetName ?? _ViewName ?? _SourceTableName;
            row["sheetorder"] = SheetOrder;
            row["definitionname"] = DefinitionName;
            _importDefTable.Rows.Add( row );
        }

        public void importOrder( Int32 Order, string NodeTypeName = null, Int32 Instance = Int32.MinValue, string SheetName = null )
        {
            NodeTypeName = NodeTypeName ?? _DestNodeTypeName;
            SheetName = SheetName ?? _ViewName ?? _SourceTableName;
            if( CswAll.AreStrings( SheetName, NodeTypeName ) )
            {
                DataRow row = _importOrderTable.NewRow();
                row["importdefid"] = _SheetDefinitions[SheetName];
                row["nodetypename"] = NodeTypeName;
                row["importorder"] = Order;
                row["instance"] = Instance;
                _importOrderTable.Rows.Add( row );
            }
        } // _importOrder()

        public void importBinding( string SourceColumnName, string DestPropertyName, string DestSubFieldName, string SheetName = null, string DestNodeTypeName = null, Int32 Instance = Int32.MinValue, string BlobTableName = "", string ClobTableName = "", string LobDataPkColOverride = "" )
        {
            if( null != _NbtImporter )
            {
                SheetName = SheetName ?? _ViewName ?? _SourceTableName;
                DestNodeTypeName = DestNodeTypeName ?? _DestNodeTypeName;
                if( CswAll.AreStrings( SheetName, DestNodeTypeName, DestPropertyName, SourceColumnName ) )
                {
                    _SourceColumns.Add( SourceColumnName, AllowNullOrEmpty: false, IsUnique: true );

                    DataRow row = _importBindingsTable.NewRow();
                    row["importdefid"] = _SheetDefinitions[SheetName];
                    row["destnodetypename"] = DestNodeTypeName;
                    row["destpropname"] = DestPropertyName;
                    row["destsubfield"] = DestSubFieldName;
                    row["sourcecolumnname"] = SourceColumnName;
                    row["instance"] = Instance;
                    row["blobtablename"] = BlobTableName;
                    row["clobtablename"] = ClobTableName;
                    row["lobdatapkcoloverride"] = LobDataPkColOverride;
                    _importBindingsTable.Rows.Add( row );
                }
            }
        } // _importBinding()

        public void importRelationship( string SheetName, string NodetypeName, string RelationshipPropName, Int32 Instance = Int32.MinValue, string SourceRelColumnName = "" )
        {
            if( null != _NbtImporter )
            {
                if( CswAll.AreStrings( SheetName, NodetypeName, RelationshipPropName ) )
                {
                    DataRow row = _importRelationshipsTable.NewRow();
                    row["importdefid"] = _SheetDefinitions[SheetName];
                    row["nodetypename"] = NodetypeName;
                    row["relationship"] = RelationshipPropName;
                    row["instance"] = Instance;
                    row["sourcerelcolumnname"] = SourceRelColumnName;
                    _importRelationshipsTable.Rows.Add( row );
                }
            }
        } // _importRelationship()

        private string _SourceTablePkColumnName;
        private string SourceTablePkColumnName
        {
            get
            {
                string Ret = null;
                if( false == string.IsNullOrEmpty( _SourceTablePkColumnName ) )
                {
                    Ret = _SourceTablePkColumnName;
                }
                else if( null != _NbtImporter )
                {
                    string ExceptionText = "";
                    if( SchemaModTrnsctn.IsDbLinkConnectionHealthy( _CAFDbLink, ref ExceptionText ) )
                    {
                    Ret = _NbtImporter.getRemoteDataDictionaryPkColumnName( _SourceTableName, _CAFDbLink );
                }
                    else
                    {
                        SchemaModTrnsctn.logError( ExceptionText );
                    }
                }

                return Ret;
            }
            set { _SourceTablePkColumnName = value; }
        }

        public void finalize(string DefinitionName = null)
        {
            if( null != _NbtImporter )
            {

                DefinitionName = DefinitionName ?? CswScheduleLogicNbtCAFImport.DefinitionName;
                //Add the Legacy ID before storing the definition
                // We check to see if someone already added the Legacy Id
                bool AlreadyExists = _importBindingsTable.Rows.Cast<DataRow>().Any( Row => Row["destpropname"].ToString() == LegacyID );
                if( false == AlreadyExists ) { importBinding( SourceTablePkColumnName, LegacyID, "" ); }

                //Save the bindings in the DB
                _NbtImporter.storeDefinition( _importOrderTable, _importBindingsTable, _importRelationshipsTable, DefinitionName );

                //_populateImportQueueTable( WhereClause, UseView );
                //_createTriggerOnImportTable();
            }
        }//finalize()
    }
}