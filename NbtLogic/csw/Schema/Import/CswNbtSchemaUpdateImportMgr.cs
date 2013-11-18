using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.ImportExport;
using ChemSW.Nbt.Sched;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.csw.Schema
{
    partial class CswNbtSchemaUpdateImportMgr
    {
        public CswNbtSchemaModTrnsctn SchemaModTrnsctn;
        public const string LegacyID = "Legacy Id";

        private Dictionary<string, Int32> _CAFOrder = new Dictionary<string, int>
            {
                //{Nodetype, Order}
                {"Control Zone", 1},
                {"Work Unit", 2},
                {"Inventory Group", 3},
                {"Site", 4},
                {"Building", 5},
                {"Room", 6},
                {"Cabinet", 7},
                {"Shelf", 8},
                {"Box", 9},
                {"Vendor", 10},
                {"Role", 11},
                {"User", 12},
                {"Regulatory List", 13},
                {"Regulatory List CAS", 14},
                {"Unit_Weight", 15},
                {"Unit_Volume", 16},
                {"Unit_Each", 17},
                {"DSD Phrase", 18}, //DSD Phrases
                {"Chemical", 19},
                {"Size", 20},
                {"SDS Document", 21},
                {"Material Document", 22},
                {"Receipt Lot", 23},
                {"C of A Document", 24},
                {"Container Group", 25},
                {"Container", 26},
                {"Inventory Level", 27},
                {"Jurisdiction", 28},
                {"GHS Phrase", 29},
                {"GHS", 30},
                {"Material Synonym", 31}
            };

        private DataTable _importDefTable;
        private DataTable _importOrderTable;
        private DataTable _importBindingsTable;
        private DataTable _importRelationshipsTable;

        private Dictionary<string, int> _SheetDefinitions;
        private string _DefinitionName;
        private string _DestNodeTypeName;

        private string _CAFDbLink;
        private CswNbtImporter _NbtImporter;

        private CswCommaDelimitedString _SourceColumns;

        public CswNbtSchemaUpdateImportMgr( CswNbtSchemaModTrnsctn SchemaModTrnsctn, string DefinitionName, string CafDbLink = null )
        {
            _CAFDbLink = CafDbLink ?? CswScheduleLogicNbtCAFImport.CAFDbLink;
            _NbtImporter = SchemaModTrnsctn.makeCswNbtImporter();
            this.SchemaModTrnsctn = SchemaModTrnsctn;

            _importDefTable = SchemaModTrnsctn.makeCswTableUpdate( "Import_getDefs", "import_def" ).getTable();
            _importOrderTable = SchemaModTrnsctn.makeCswTableUpdate( "Import_getOrder", "import_def_order" ).getTable();
            _importBindingsTable = SchemaModTrnsctn.makeCswTableUpdate( "Import_getBindings", "import_def_bindings" ).getTable();
            _importRelationshipsTable = SchemaModTrnsctn.makeCswTableUpdate( "Import_getRelationships", "import_def_relationships" ).getTable();

            
            _DefinitionName = DefinitionName;
            _SourceColumns = new CswCommaDelimitedString();

            _SheetDefinitions = SchemaModTrnsctn.createImportDefinitionEntries( DefinitionName, _importDefTable );

        }//ctor

        /// <summary>
        /// Declare a new import sheet in IMPORT_DEF. 
        /// </summary>
        /// <param name="SheetOrder"></param>
        /// <param name="SheetName"></param>
        public void importDef(int SheetOrder, string SheetName)
        {
            DataRow row = _importDefTable.NewRow();
            row["sheetname"] = SheetName;
            row["sheetorder"] = SheetOrder;
            row["definitionname"] = _DefinitionName;
            _importDefTable.Rows.Add( row );
            _SheetDefinitions = SchemaModTrnsctn.createImportDefinitionEntries( _DefinitionName, _importDefTable );
        }


        public void importOrder( Int32 Order, string NodeTypeName, string SheetName, Int32 Instance = Int32.MinValue)
        {
            _DestNodeTypeName = NodeTypeName;
            DataRow row = _importOrderTable.NewRow();
            row["importdefid"] = _SheetDefinitions[SheetName];
            row["nodetypename"] = NodeTypeName;
            row["importorder"] = Order;
            row["instance"] = Instance;
            _importOrderTable.Rows.Add( row );
        } // importOrder()

        public void CAFimportOrder( string NodeTypeName, string TableName, string ViewName = null, string PkColumnName = null, bool createLegacyId = true, Int32 Instance = Int32.MinValue )
        {
            _DestNodeTypeName = NodeTypeName;
            PkColumnName = PkColumnName ?? _getPKColumnForTable( TableName );
            if( CswAll.AreStrings( NodeTypeName, TableName, PkColumnName ) )
            {
                DataRow row = _importOrderTable.NewRow();
                row["importdefid"] = _SheetDefinitions[CswScheduleLogicNbtCAFImport.DefinitionName];
                row["nodetypename"] = NodeTypeName;
                row["importorder"] = _CAFOrder[NodeTypeName];
                row["instance"] = Instance;
                row["tablename"] = TableName;
                row["viewname"] = ViewName;
                row["pkcolumnname"] = PkColumnName;
                _importOrderTable.Rows.Add( row );

                if( createLegacyId )
                {
                    importBinding( PkColumnName, LegacyID, "" );
                }
            }
            else
            {
                throw new CswDniException(CswEnumErrorType.Error, "Failed to validate inputs for CAF import order: " + NodeTypeName, "One of the required fields was missing.");
            }
        }// CAFimportOrder()

        public void importBinding( string SourceColumnName, string DestPropertyName, string DestSubFieldName, string SheetName = null, string DestNodeTypeName = null, Int32 Instance = Int32.MinValue, string BlobTableName = "", string ClobTableName = "", string LobDataPkColOverride = "" )
        {
            if( null != _NbtImporter )
            {
                SheetName = SheetName ?? CswScheduleLogicNbtCAFImport.DefinitionName;
                DestNodeTypeName = DestNodeTypeName ?? _DestNodeTypeName; //default to the last nodetype defined in ImportOrder
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

        private string _getPKColumnForTable( string SourceTable )
        {
            string ret = "";
            string ExceptionText = "";
            if( SchemaModTrnsctn.IsDbLinkConnectionHealthy( _CAFDbLink, ref ExceptionText ) )
            {
                ret = _NbtImporter.getRemoteDataDictionaryPkColumnName( SourceTable, _CAFDbLink );
            }
            else
            {
                SchemaModTrnsctn.logError( ExceptionText );
            }

            return ret;
        }//_getPKColumnForTable( string SourceTable )


        public void finalize()
        {
            if( null != _NbtImporter )
            {
                _NbtImporter.storeDefinition( _importOrderTable, _importBindingsTable, _importRelationshipsTable );

                //_populateImportQueueTable( WhereClause, UseView );
                //_createTriggerOnImportTable();
            }
        }//finalize()
    }
}