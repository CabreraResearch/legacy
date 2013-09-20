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
                //{SourceTableName (SheetName), Order}
                {"cispro_controlzones", 1},
                {"work_units", 2},
                {"inventory_groups", 3},
                {"locations", 4},
                {"units_of_measure", 5},
                {"vendors", 6},
                {"roles", 7},
                {"users", 8},
                {"regulatory_lists", 9},
                {"regulated_casnos", 10},
                {"packdetails", 12},
            };

        private DataTable _importDefTable;
        private DataTable _importOrderTable;
        private DataTable _importBindingsTable;
        private DataTable _importRelationshipsTable;

        private string _DestNodeTypeName;
        private string _SourceTableName;
        private string _ViewName;
        private Int32 _ImportOrder;

        private string _CAFDbLink;
        private CswNbtImporter _NbtImporter;

        private CswCommaDelimitedString _SourceColumns;

        public CswNbtSchemaUpdateImportMgr( CswNbtSchemaModTrnsctn SchemaModTrnsctn, string SourceTableName, string DestNodeTypeName, string ViewName = "", string CafDbLink = null, Int32 ImportOrder = 1 )
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

                _SourceTableName = SourceTableName;
                _DestNodeTypeName = DestNodeTypeName;
                _ViewName = ViewName;
                _ImportOrder = ImportOrder;
                _SourceColumns = new CswCommaDelimitedString();

                _importOrder( _ImportOrder, _DestNodeTypeName );
                _importDef();
            }
            else
            {
                SchemaModTrnsctn.logError( ExceptionText );
            }
        }//ctor

        public CswNbtSchemaUpdateImportMgr( CswNbtSchemaModTrnsctn SchemaModTrnsctn, string SourceTableName, List<Tuple<string, Int32>> DestNodeTypesAndInstances, string ViewName = "", string CafDbLink = null )
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

                _SourceTableName = SourceTableName;
                _ViewName = ViewName;
                _SourceColumns = new CswCommaDelimitedString();

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
            row["sheet"] = _SourceTableName;
            row["sheetorder"] = _SheetOrder[_SourceTableName];
            _importDefTable.Rows.Add( row );
        }

        private void _importOrder( Int32 Order, string NodeTypeName = null, Int32 Instance = Int32.MinValue )
        {
            NodeTypeName = NodeTypeName ?? _DestNodeTypeName;
            if( CswAll.AreStrings( _SourceTableName, NodeTypeName ) )
            {
                DataRow row = _importOrderTable.NewRow();
                row["sheet"] = _SourceTableName;
                row["nodetype"] = NodeTypeName;
                row["order"] = Order;
                row["instance"] = Instance;
                _importOrderTable.Rows.Add( row );
            }
        } // _importOrder()

        public void importBinding( string SourceColumnName, string DestPropertyName, string DestSubFieldName, string SheetName = null, string DestNodeTypeName = null, Int32 Instance = Int32.MinValue )
        {
            if( null != _NbtImporter )
            {
                SheetName = SheetName ?? _SourceTableName;
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

        private string SourceTablePkColumnName
        {
            get
            {
                string Ret = null;
                if( null != _NbtImporter )
                {
                    Ret = _NbtImporter.getRemoteDataDictionaryPkColumnName( _SourceTableName, _CAFDbLink );
                }
                return Ret;
            }
        }

        public void finalize( string WhereClause = null, string DefinitionName = null, bool UseView = false )
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

                _populateImportQueueTable( WhereClause, UseView );

                _createTriggerOnImportTable();
            }
        }//finalize()
    }
}