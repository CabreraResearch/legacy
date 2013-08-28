using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Nbt.ImportExport;
using ChemSW.Nbt.Sched;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.csw.Schema
{
    class CswNbtSchemaUpdateImportMgr
    {
        public CswNbtSchemaModTrnsctn SchemaModTrnsctn;
        public const string LegacyID = "Legacy ID";

        private Dictionary<string, Int32> _Order = new Dictionary<string, int>
            {
                //{SourceTableName, ImportOrder}
                {"cispro_controlzones", 1},
                {"sites", 2},
                {"locations", 3},
                {"business_units", 4},
                {"work_units", 5},
                {"inventory_groups", 6},
                {"units_of_measure", 7},
                {"vendors", 8},
                {"roles", 9},
                {"users", 10},
                {"regulatory_lists", 11},
                {"regulated_casnos", 12}
            };


        private DataTable _importOrderTable;
        private DataTable _importBindingsTable;
        private DataTable _importRelationshipsTable;

        private string _DestNodeTypeName;
        private string _SourceTableName;
        private string _ViewName;
        private Int32 _ImportOrder;

        //public CswNbtSchemaUpdateImportMgr( CswNbtSchemaModTrnsctn SchemaModTrnsctn, Int32 ImportOrder, string SourceTableName, string DestNodeTypeName )
        public CswNbtSchemaUpdateImportMgr( CswNbtSchemaModTrnsctn SchemaModTrnsctn, string SourceTableName, string DestNodeTypeName, string ViewName = "" )
        {
            this.SchemaModTrnsctn = SchemaModTrnsctn;

            _importOrderTable = CswNbtImportDefOrder.getDataTableForNewOrderEntries();
            _importBindingsTable = CswNbtImportDefBinding.getDataTableForNewBindingEntries();
            _importRelationshipsTable = CswNbtImportDefRelationship.getDataTableForNewRelationshipEntries();

            _SourceTableName = SourceTableName;
            _DestNodeTypeName = DestNodeTypeName;
            _ViewName = ViewName;
            _ImportOrder = _Order[SourceTableName];
            //TODO: Provide error messsage/throw exception if the sourcetablename isn't in the dictionary
            //"Could not find the SourceTableName " + SourceTableName + " in the _Order dictionary."

            _importOrder( _ImportOrder, _SourceTableName, _DestNodeTypeName );
        }//ctor

        private void _importOrder( Int32 Order, string SheetName = null, string NodeTypeName = null, Int32 Instance = Int32.MinValue )
        {
            SheetName = SheetName ?? _SourceTableName;
            NodeTypeName = NodeTypeName ?? _DestNodeTypeName;
            if( false == string.IsNullOrEmpty( NodeTypeName ) )
            {
                DataRow row = _importOrderTable.NewRow();
                row["sheet"] = SheetName;
                row["nodetype"] = NodeTypeName;
                row["order"] = Order;
                row["instance"] = Instance;
                _importOrderTable.Rows.Add( row );
            }
        } // _importOrder()

        public void importBinding( string SourceColumnName, string DestPropertyName, string DestSubFieldName, string SheetName = null, string DestNodeTypeName = null, Int32 Instance = Int32.MinValue )
        {
            SheetName = SheetName ?? _SourceTableName;
            DestNodeTypeName = DestNodeTypeName ?? _DestNodeTypeName;
            if( false == string.IsNullOrEmpty( DestNodeTypeName ) )
            {
                DataRow row = _importBindingsTable.NewRow();
                row["sheet"] = SheetName;
                row["destnodetype"] = DestNodeTypeName;
                row["destproperty"] = DestPropertyName;
                row["destsubfield"] = DestSubFieldName;
                row["sourcecolumnname"] = SourceColumnName;
                row["instance"] = Instance;
                _importBindingsTable.Rows.Add( row );
            }
        } // _importBinding()

        public void importRelationship( string SheetName, string NodetypeName, string RelationshipPropName, Int32 Instance = Int32.MinValue )
        {
            DataRow row = _importRelationshipsTable.NewRow();
            row["sheet"] = SheetName;
            row["nodetype"] = NodetypeName;
            row["relationship"] = RelationshipPropName;
            row["instance"] = Instance;
            _importRelationshipsTable.Rows.Add( row );
        } // _importRelationship()

        public void finalize( string CafDbLink = null, string WhereClause = null, string DefinitionName = null )
        {
            string ExceptionText = string.Empty;
            string State = CswScheduleLogicNbtCAFImport.State.N.ToString();
            CafDbLink = CafDbLink ?? CswScheduleLogicNbtCAFImport.CAFDbLink;
            DefinitionName = DefinitionName ?? CswScheduleLogicNbtCAFImport.DefinitionName;

            if( SchemaModTrnsctn.IsDbLinkConnectionHealthy( CafDbLink, ref ExceptionText ) )
            {
                CswNbtImporter Importer = SchemaModTrnsctn.makeCswNbtImporter();

                //Add the Legacy ID before storing the definition
                string SourceTablePkColumnName = Importer.getRemoteDataDictionaryPkColumnName( _SourceTableName, CafDbLink );
                importBinding( SourceTablePkColumnName, LegacyID, "" );

                //Save the bindings in the DB
                Importer.storeDefinition( _importOrderTable, _importBindingsTable, _importRelationshipsTable, DefinitionName );

                //Optional extension to where clause. Logical deletes already excluded.
                WhereClause = WhereClause ?? string.Empty;
                if( false == string.IsNullOrEmpty( WhereClause ) && false == WhereClause.Trim().StartsWith( "and" ) )
                {
                    WhereClause = " and " + WhereClause;
                }

                //Populate the import queue
                string SqlText = "insert into nbtimportqueue@" + CafDbLink + " ( nbtimportqueueid, state, itempk, tablename, priority, errorlog, viewname ) " +
                                 @" select seq_nbtimportqueueid.nextval@" + CafDbLink + ", '" + State + "', " + SourceTablePkColumnName + ", '" + _SourceTableName + "',0, '', '" + _ViewName + "' from " + _SourceTableName + "@" + CafDbLink + " where deleted='0' " + WhereClause;
                SchemaModTrnsctn.execArbitraryPlatformNeutralSql( SqlText );
            }
            else
            {
                SchemaModTrnsctn.logError( ExceptionText );
            }
        }
    }


}
