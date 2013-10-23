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
                {"chemicals_view", 13},
                {"packdetail_view", 14},
                {"sds_view", 15},
                {"docs_view", 15},
                {"receipt_lots_view", 16},
                {"cofa_docs_view", 17},
                {"container_groups", 18},
                {"containers_view", 19},
                {"inventory_view", 20},
                {"ghs_phrases", 21}
            };

        private DataTable _importDefTable;
        private DataTable _importOrderTable;
        private DataTable _importBindingsTable;
        private DataTable _importRelationshipsTable;

        private string _DestNodeTypeName;
        private string _SourceTableName;
        private string _ViewName;
        private string _SheetName;
        private string SheetName
        {
            get
            {
                string Ret;
                if( string.IsNullOrEmpty( _SheetName ) )
                {
                    Ret = string.IsNullOrEmpty( _ViewName ) ? _SourceTableName : _ViewName;
                }
                else
                {
                    Ret = _SheetName;
                }
                return Ret.ToLower();
            }
            set { _SheetName = value; }
        }
        private Int32 _ImportOrder;

        private string _CAFDbLink;
        private CswNbtImporter _NbtImporter;

        private CswCommaDelimitedString _SourceColumns;

        public CswNbtSchemaUpdateImportMgr( CswNbtSchemaModTrnsctn SchemaModTrnsctn, string SourceTableName, string DestNodeTypeName, string ViewName = "", string SourceColumn = "", string Sheet = "", string CafDbLink = null, Int32 ImportOrder = 1 )
        {
            string ExceptionText = string.Empty;
            _CAFDbLink = CafDbLink ?? CswScheduleLogicNbtCAFImport.CAFDbLink;
            if( SchemaModTrnsctn.IsDbLinkConnectionHealthy( _CAFDbLink, ref ExceptionText ) )
            {
                _NbtImporter = SchemaModTrnsctn.makeCswNbtImporter();
                this.SchemaModTrnsctn = SchemaModTrnsctn;

                _importDefTable = CswNbtImportDef.getDataTableForNewOrderEntries();
                _importOrderTable = CswNbtImportDefOrder.getDataTableForNewOrderEntries();
                _importBindingsTable = CswNbtImportDefBinding.getDataTableForNewBindingEntries();
                _importRelationshipsTable = CswNbtImportDefRelationship.getDataTableForNewRelationshipEntries();

                _SourceTableName = SourceTableName.ToLower();
                _DestNodeTypeName = DestNodeTypeName.ToLower();
                _ViewName = ViewName.ToLower();
                SheetName = Sheet.ToLower();
                _ImportOrder = ImportOrder;
                _SourceColumns = new CswCommaDelimitedString();
                SourceTablePkColumnName = SourceColumn.ToLower();

                _importOrder( _ImportOrder, _DestNodeTypeName );
                _importDef();
            }
            else
            {
                SchemaModTrnsctn.logError( ExceptionText );
            }
        }//ctor

        public CswNbtSchemaUpdateImportMgr( CswNbtSchemaModTrnsctn SchemaModTrnsctn, string SourceTableName, List<Tuple<string, Int32>> DestNodeTypesAndInstances, string ViewName = "", string SourceColumn = "", string Sheet = "", string CafDbLink = null )
        {
            string ExceptionText = string.Empty;
            _CAFDbLink = CafDbLink ?? CswScheduleLogicNbtCAFImport.CAFDbLink;
            if( SchemaModTrnsctn.IsDbLinkConnectionHealthy( _CAFDbLink, ref ExceptionText ) )
            {
                _NbtImporter = SchemaModTrnsctn.makeCswNbtImporter();
                this.SchemaModTrnsctn = SchemaModTrnsctn;

                _importDefTable = CswNbtImportDef.getDataTableForNewOrderEntries();
                _importOrderTable = CswNbtImportDefOrder.getDataTableForNewOrderEntries();
                _importBindingsTable = CswNbtImportDefBinding.getDataTableForNewBindingEntries();
                _importRelationshipsTable = CswNbtImportDefRelationship.getDataTableForNewRelationshipEntries();

                _SourceTableName = SourceTableName.ToLower();
                _ViewName = ViewName.ToLower();
                SheetName = Sheet.ToLower();
                _SourceColumns = new CswCommaDelimitedString();
                SourceTablePkColumnName = SourceColumn.ToLower();

                for( int i = 0; i < DestNodeTypesAndInstances.Count; i++ )
                {
                    _importOrder( i + 1, DestNodeTypesAndInstances[i].Item1, DestNodeTypesAndInstances[i].Item2 );
                }

                _importDef();
            }
            else
            {
                SchemaModTrnsctn.logError( ExceptionText );
            }
        }

        private void _importDef()
        {
            DataRow row = _importDefTable.NewRow();
            row["sheet"] = SheetName;
            row["sheetorder"] = _SheetOrder[SheetName];
            row["tablename"] = _SourceTableName;
            row["viewname"] = _ViewName;
            row["pkcolumnname"] = SourceTablePkColumnName;
            _importDefTable.Rows.Add( row );
        }

        private void _importOrder( Int32 Order, string NodeTypeName = null, Int32 Instance = Int32.MinValue )
        {
            NodeTypeName = NodeTypeName ?? _DestNodeTypeName;
            if( CswAll.AreStrings( _SourceTableName, NodeTypeName ) )
            {
                DataRow row = _importOrderTable.NewRow();
                row["sheet"] = SheetName;
                row["nodetype"] = NodeTypeName;
                row["order"] = Order;
                row["instance"] = Instance;
                _importOrderTable.Rows.Add( row );
            }
        } // _importOrder()

        public void importBinding( string SourceColumnName, string DestPropertyName, string DestSubFieldName, string SheetName = null, string DestNodeTypeName = null, Int32 Instance = Int32.MinValue, string BlobTableName = "", string ClobTableName = "", string LobDataPkColOverride = "" )
        {
            if( null != _NbtImporter )
            {
                SheetName = SheetName ?? this.SheetName;
                DestNodeTypeName = DestNodeTypeName ?? _DestNodeTypeName;
                if( CswAll.AreStrings( SheetName, DestNodeTypeName, DestPropertyName, SourceColumnName ) )
                {
                    _SourceColumns.Add( SourceColumnName, AllowNullOrEmpty: false, IsUnique: true );

                    DataRow row = _importBindingsTable.NewRow();
                    row["sheet"] = SheetName;
                    row["destnodetype"] = DestNodeTypeName;
                    row["destproperty"] = DestPropertyName;
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
                    row["sheet"] = SheetName;
                    row["nodetype"] = NodetypeName;
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
                    Ret = _NbtImporter.getRemoteDataDictionaryPkColumnName( _SourceTableName, _CAFDbLink );
                }

                return Ret;
            }
            set { _SourceTablePkColumnName = value; }
        }

        public void finalize( string DefinitionName = null )
        {
            if( null != _NbtImporter )
            {
                DefinitionName = DefinitionName ?? CswScheduleLogicNbtCAFImport.DefinitionName;

                //Add the Legacy ID before storing the definition
                // We check to see if someone already added the Legacy Id
                bool AlreadyExists = _importBindingsTable.Rows.Cast<DataRow>().Any( Row => Row["destproperty"].ToString() == LegacyID );
                if( false == AlreadyExists ) { importBinding( SourceTablePkColumnName, LegacyID, "" ); }

                //Save the bindings in the DB
                _NbtImporter.storeDefinition( _importOrderTable, _importBindingsTable, _importRelationshipsTable, DefinitionName, _importDefTable );

                //_populateImportQueueTable( WhereClause, UseView );
                //_createTriggerOnImportTable();
            }
        }//finalize()
    }
}