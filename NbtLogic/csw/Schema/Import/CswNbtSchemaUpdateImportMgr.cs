using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.ImportExport;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Sched;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.csw.Schema
{
    partial class CswNbtSchemaUpdateImportMgr
    {
        private CswNbtSchemaModTrnsctn SchemaModTrnsctn;

        /// <summary>
        /// The ordering of CAF imported nodetypes. Used to make ordering imports convenient for CAFimportOrder.
        /// </summary>
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

        /// <summary>
        /// The nodetype which will be used by default for importBinding()
        /// </summary>
        public string DefaultNodetype;

        //TODO: if case 31124 is resolved and this comment is still here, figure out if this is used
        private string _CAFDbLink;

        /// <summary>
        /// The underlying Importer which is used to post changes to the database
        /// </summary>
        private CswNbtImporter _NbtImporter;

        private CswCommaDelimitedString _SourceColumns;

        /// <summary>
        /// Build a new UpdateImportMgr for a particular definition
        /// </summary>
        /// <param name="SchemaModTrnsctn">The schema script resources class</param>
        /// <param name="DefinitionName">The IMPORT_DEF definition (use "CAF" for caf imports)</param>
        /// <param name="CafDbLink">Legacy: this should probably change/go away after case 31124</param>
        public CswNbtSchemaUpdateImportMgr( CswNbtSchemaModTrnsctn SchemaModTrnsctn, string DefinitionName, string CafDbLink = null )
        {
            _CAFDbLink = CafDbLink ?? CswScheduleLogicNbtCAFImport.CAFDbLink;
            _NbtImporter = new CswNbtImporter( SchemaModTrnsctn.Accessid, CswEnumSetupMode.NbtExe );
            this.SchemaModTrnsctn = SchemaModTrnsctn;

            _importDefTable = SchemaModTrnsctn.makeCswTableUpdate( "Import_getDefs", "import_def" ).getTable();
            _importOrderTable = SchemaModTrnsctn.makeCswTableUpdate( "Import_getOrder", "import_def_order" ).getTable();
            _importBindingsTable = SchemaModTrnsctn.makeCswTableUpdate( "Import_getBindings", "import_def_bindings" ).getTable();
            _importRelationshipsTable = SchemaModTrnsctn.makeCswTableUpdate( "Import_getRelationships", "import_def_relationships" ).getTable();

            
            _DefinitionName = DefinitionName;
            _SourceColumns = new CswCommaDelimitedString();

            _SheetDefinitions = SchemaModTrnsctn.createImportDefinitionEntries( DefinitionName, _importDefTable );

        }//ctor

        #region Add definitions


        /// <summary>
        /// Declare a new import sheet in IMPORT_DEF and set that sheet as the default for new orders and bindings 
        /// with this ImportMgr. (Unnecessary for CAF imports -- all imports use the "CAF" sheet)
        /// </summary>
        /// <param name="SheetOrder">The order of this sheet relative to other sheets</param>
        /// <param name="SheetName">The name used to identify this sheet</param>
        /// <param name="OverrideImportDefId">Manually set the ImportDefId for this import. Use only when a sheet
        /// has been deleted with removeImportDef and a new one must be defined with the same importdefid.</param>
        public void importDef(int SheetOrder, string SheetName, int OverrideImportDefId = int.MinValue)
        {
            DataRow row = _importDefTable.NewRow();
            if( OverrideImportDefId != int.MinValue )
            {
                row["importdefid"] = OverrideImportDefId;
            }
            row["sheetname"] = SheetName;
            row["sheetorder"] = SheetOrder;
            row["definitionname"] = _DefinitionName;
            _importDefTable.Rows.Add( row );
            _SheetDefinitions = SchemaModTrnsctn.createImportDefinitionEntries( _DefinitionName, _importDefTable );
        }


        /// <summary>
        /// Declare a new entry in the list of imported nodetypes for this sheet in IMPORT_DEF_ORDER, and set
        /// that nodetype as the default for new bindings with this ImportMgr.
        /// NOTE: use .CAFimportOrder for declaring new CAF nodetypes
        /// </summary>
        /// <param name="Order">The order of this nodetype relative to other nodetypes in this sheet</param>
        /// <param name="NodeTypeName">The string used to identify this nodetype</param>
        /// <param name="SheetName">The sheet to which this order is attached</param>
        /// <param name="Instance">Used to differentiate the same nodetype imported from multiple places</param>
        public void importOrder( Int32 Order, string NodeTypeName, string SheetName, Int32 Instance = Int32.MinValue)
        {
            DefaultNodetype = NodeTypeName;
            DataRow row = _importOrderTable.NewRow();
            row["importdefid"] = _SheetDefinitions[SheetName];
            row["nodetypename"] = NodeTypeName;
            row["importorder"] = Order;
            row["instance"] = Instance;
            _importOrderTable.Rows.Add( row );
        } // importOrder()


        /// <summary>
        /// Declare a new entry in the list of imported nodetypes for CAF in IMPORT_DEF_ORDER, and set
        /// that nodetype as the default for new bindings with this ImportMgr.
        /// </summary>
        /// <param name="NodeTypeName">The string used to identify this nodetype</param>
        /// <param name="TableName">The source table where these nodes were originally stored in CAF (even if a view is used)</param>
        /// <param name="ViewName">The view where CAF importer should pull node data from, if one was used</param>
        /// <param name="PkColumnName">The PK column of the view/table that should be used to set legacy Ids for imported nodes. If blank,
        /// will attempt to do a lookup in DATA_DICTIONARY for the table's original PK column.</param>
        /// <param name="createLegacyId">Whether or not the Legacy Id column should be autogenerated from PKColumnName</param>
        /// <param name="Instance">Used to differentiate the same nodetype imported from multiple places</param>
        public void CAFimportOrder( string NodeTypeName, string TableName, string ViewName = null, string PkColumnName = null, bool createLegacyId = true, Int32 Instance = Int32.MinValue )
        {
            DefaultNodetype = NodeTypeName;
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
                    importBinding( PkColumnName, CswNbtObjClass.PropertyName.LegacyId, "" );
                }
            }
            else
            {
                throw new CswDniException(CswEnumErrorType.Error, "Failed to validate inputs for CAF import order: " + NodeTypeName, "One of the required fields was missing.");
            }
        }// CAFimportOrder()


        /// <summary>
        /// Declare a new binding from a column in the source data to a NTP in NBT in the IMPORT_DEF_BINDINGS table. 
        /// </summary>
        /// <param name="SourceColumnName">The column in the source data for this binding</param>
        /// <param name="DestPropertyName">The NTP to which the data will be imported</param>
        /// <param name="DestSubFieldName">The subfield on the NTP where the data should be stored</param>
        /// <param name="SheetName">The sheet being used for this import. If left out, default to "CAF"</param>
        /// <param name="DestNodeTypeName">The Nodetype where data will be imported. If left out, will default to the
        /// last nodetype set with importOrder or the last manually set DestNodeType.</param>
        /// <param name="Instance">Sets which entry in IMPORT_DEF_ORDER this binding should be associated with</param>
        /// <param name="BlobTableName">The source table for BLOB data being imported from CAF, if this binding is to a LOB</param>
        /// <param name="ClobTableName">The source table for CLOB data being imported from CAF, if this binding is to a LOB</param>
        /// <param name="LobDataPkColOverride">The column that stores the value of the PK for the LOB table</param>
        /// <param name="LobDataPkColName">The PK Column of the LOB table, if it differs from the column in the table the stores the PK used to access the LOB table</param>
        /// <param name="LegacyPropId">The CAF PropertyId (used for CAF Props only)</param>
        public void importBinding( string SourceColumnName, string DestPropertyName, string DestSubFieldName, string SheetName = null, string DestNodeTypeName = null, Int32 Instance = Int32.MinValue, string BlobTableName = "", string ClobTableName = "", string LobDataPkColOverride = "", string LobDataPkColName = "", Int32 LegacyPropId = Int32.MinValue )
        {
            if( null != _NbtImporter )
            {
                SheetName = SheetName ?? CswScheduleLogicNbtCAFImport.DefinitionName;
                DestNodeTypeName = DestNodeTypeName ?? DefaultNodetype; //default to the last nodetype defined in ImportOrder
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
                    row["lobdatapkcolname"] = LobDataPkColName;
                    row["legacypropid"] = LegacyPropId;
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


        #endregion Add definitions


        #region Remove definitions

        /// <summary>
        /// Delete the specified sheetname from this definition in IMPORT_DEF. Note that this removes the sheet from the sheetDefinitions index,
        /// making it impossible to access associated bindings unless a new IMPORT_DEF with that importdefid is defined
        /// </summary>
        /// <param name="SheetName">The sheet to be deleted</param>
        /// <param name="CascadeDelete">Whether to delete ALL things associated with this sheet in IMPORT_DEF_ORDER and IMPORT_DEF_BINDINGS (be careful)</param>
        /// <returns>The importdefid of the sheet that was deleted</returns>
        public int removeImportDef( string SheetName, bool CascadeDelete = false )
        {
            int DeletedImportDef = _SheetDefinitions[SheetName];

            //this select should only ever return one row
            DataRow SheetToDelete = _importDefTable.Select( "sheetname = '" + SheetName + "'" )[0];
            SheetToDelete.Delete();
                
            if( CascadeDelete )
            {
                DataRow[] OrdersToDelete = _importOrderTable.Select( "importdefid = " + DeletedImportDef );
                foreach( DataRow ImportOrder in OrdersToDelete )
                {
                    ImportOrder.Delete();
                }

                DataRow[] BindingsToDelete = _importBindingsTable.Select( "importdefid = " + DeletedImportDef );
                foreach( DataRow ImportBinding in BindingsToDelete )
                {
                    ImportBinding.Delete();
                }

                DataRow[] RelationshipsToDelete = _importRelationshipsTable.Select( "importdefid = " + DeletedImportDef );
                foreach( DataRow ImportRelationship in RelationshipsToDelete )
                {
                    ImportRelationship.Delete();
                }
            }//if CascadeDelete

            _SheetDefinitions = SchemaModTrnsctn.createImportDefinitionEntries( _DefinitionName, _importDefTable );

            return DeletedImportDef;

        }//removeImportDef()


        /// <summary>
        /// Remove the specified nodetype from the IMPORT_DEF_ORDER table.
        /// </summary>
        /// <param name="Sheetname">the name of the sheet where this import order is defined ("CAF" for all CAF bindings)</param>
        /// <param name="NodetypeName">the name of the nodetype to be removed</param>
        /// <param name="Instance">the instance associated with this nodetype</param>
        /// <param name="CascadeDelete">whether to delete any bindings associated with this import order</param>
        public void removeImportOrder(string Sheetname, string NodetypeName, int Instance = Int32.MinValue, bool CascadeDelete = false )
        {
            //this query should only ever return one row
            DataRow OrderToDelete = _importOrderTable.Select( "importdefid = " + _SheetDefinitions[Sheetname] + " and nodetypename = '" + NodetypeName + "' and instance = " + Instance )[0];
            OrderToDelete.Delete();

            if( CascadeDelete )
            {
                DataRow[] BindingsToDelete = _importBindingsTable.Select( "importdefid = " + _SheetDefinitions[Sheetname] + " and destnodetypename = '" + NodetypeName + "' and instance = " + Instance );
                foreach( DataRow Binding in BindingsToDelete )
                {
                    Binding.Delete();
                }

                DataRow[] RelationshipsToDelete = _importRelationshipsTable.Select( "importdefid = " + _SheetDefinitions[Sheetname] + " and nodetypename = '" + NodetypeName + "' and instance = " + Instance );
                foreach( DataRow Relationship in RelationshipsToDelete )
                {
                    Relationship.Delete();
                }
            }//if CascadeDelete
        }//removeImportOrder()


        /// <summary>
        /// Remove the specified binding from the IMPORT_DEF_BINDINGS table.
        /// </summary>
        /// <param name="Sheetname">The sheet where the specified binding is found ("CAF" for all CAF bindings)</param>
        /// <param name="SourceColumn">The column where the binding pulled data from</param>
        /// <param name="NodetypeName">The Nodetype that data was imported to</param>
        /// <param name="PropName">The prop where the source column data was stored</param>
        /// <param name="SubfieldName">The subfield on the prop of the nodetype for this binding</param>
        /// <param name="Instance">Which import order instance this binding was associated with</param>
        public void removeImportBinding(string Sheetname, string SourceColumn, string NodetypeName, string PropName, string SubfieldName, int Instance = int.MinValue)
        {
            //this should only ever return one result
            DataRow BindingToDelete = _importBindingsTable.Select( "importdefid = " + _SheetDefinitions[Sheetname] + " and sourcecolumnname = '" + SourceColumn + "' and destnodetypename = '" 
                                                                   + NodetypeName + "' and destpropname = '" + PropName + "'" + " and destsubfield = '" + SubfieldName + "' and instance = " + Instance )[0];
            BindingToDelete.Delete();
        }//removeImportBinding()


        /// <summary>
        /// Remove the specified binding from the IMPORT_DEF_RELATIONSHIPS table.
        /// </summary>
        /// <param name="Sheetname">The sheet where the specified binding is found ("CAF" for all CAF bindings)</param>
        /// <param name="NodetypeName">The Nodetype that data was imported to</param>
        /// <param name="Relationship">the relationship which was bound</param>
        /// <param name="Instance">Which import order instance this binding was associated with</param>
        public void removeImportRelationship(string Sheetname, string NodetypeName, string Relationship, int Instance = int.MinValue)
        {
            //this should only ever return one result
            DataRow RelationshipToDelete = _importRelationshipsTable.Select( "importdefid = " + _SheetDefinitions[Sheetname] + " and nodetypename = '" + NodetypeName + "' " +
                                                                             "and relationship = '" + Relationship + "' and instance = " + Instance )[0];
            RelationshipToDelete.Delete();
        }//removeImportRelationship()

        #endregion


        /// <summary>
        /// Use CAFDbLink to lookup the PK column of a table on a CAF schema in DATA_DICTIONARY
        /// </summary>
        /// <param name="SourceTable">The table to find a PK column for</param>
        /// <returns>The PK column of the SourceTable</returns>
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


        /// <summary>
        /// Post the new import definitions created in this ImportMgr to the database.
        /// </summary>
        public void finalize()
        {
            if( null != _NbtImporter )
            {
                _NbtImporter.storeDefinition( _importOrderTable, _importBindingsTable, _importRelationshipsTable );
                _NbtImporter.Finish();
            }
        }//finalize()
    }
}