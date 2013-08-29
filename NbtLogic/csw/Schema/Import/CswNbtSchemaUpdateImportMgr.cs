using System;
using System.Data;
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

        private DataTable _importOrderTable;
        private DataTable _importBindingsTable;
        private DataTable _importRelationshipsTable;

        private string _DestNodeTypeName;
        private string _SourceTableName;
        private Int32 _ImportOrder;

        private CswCommaDelimitedString _SourceColumns;

        public CswNbtSchemaUpdateImportMgr( CswNbtSchemaModTrnsctn SchemaModTrnsctn, Int32 ImportOrder, string SourceTableName, string DestNodeTypeName )
        {
            this.SchemaModTrnsctn = SchemaModTrnsctn;

            _importOrderTable = CswNbtImportDefOrder.getDataTableForNewOrderEntries();
            _importBindingsTable = CswNbtImportDefBinding.getDataTableForNewBindingEntries();
            _importRelationshipsTable = CswNbtImportDefRelationship.getDataTableForNewRelationshipEntries();

            _ImportOrder = ImportOrder;
            _SourceTableName = SourceTableName;
            _DestNodeTypeName = DestNodeTypeName;

            _SourceColumns = new CswCommaDelimitedString();

            _importOrder( _ImportOrder, _SourceTableName, _DestNodeTypeName );
        }

        private void _importOrder( Int32 Order, string SheetName = null, string NodeTypeName = null, Int32 Instance = Int32.MinValue )
        {
            SheetName = SheetName ?? _SourceTableName;
            NodeTypeName = NodeTypeName ?? _DestNodeTypeName;
            if( CswAll.AreStrings( SheetName, NodeTypeName ) )
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
        } // _importBinding()

        public void importRelationship( string SheetName, string NodetypeName, string RelationshipPropName, Int32 Instance = Int32.MinValue )
        {
            if( CswAll.AreStrings( SheetName, NodetypeName, RelationshipPropName ) )
            {
                DataRow row = _importRelationshipsTable.NewRow();
                row[ "sheet" ] = SheetName;
                row[ "nodetype" ] = NodetypeName;
                row[ "relationship" ] = RelationshipPropName;
                row[ "instance" ] = Instance;
                _importRelationshipsTable.Rows.Add( row );
            }
        } // _importRelationship()

        /*
         CREATE OR REPLACE TRIGGER TRIG_IMPORT_VENDORS 
AFTER INSERT OR DELETE OR UPDATE OF ACCOUNTNO,CITY,CONTACTNAME,DELETED,EMAIL,FAX,PHONE,STATE,STREET1,STREET2,VENDORID,VENDORNAME,ZIP,COUNTRY ON VENDORS 
FOR EACH ROW 
BEGIN
  
  IF INSERTING THEN
    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'I', :new.vendorid, 'vendors', '', '');
    
  ELSIF DELETING THEN
    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old.vendorid, 'vendors', '', '');
  
  ELSE
    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'U', :old.vendorid, 'vendors', '', '');
    
  END IF;
  
END;
         
         
         */

        public void finalize( string CafDbLink = null, string WhereClause = null )
        {
            string ExceptionText = string.Empty;
            CafDbLink = CafDbLink ?? CswScheduleLogicNbtCAFImport.CAFDbLink;

            if( SchemaModTrnsctn.IsDbLinkConnectionHealthy( CafDbLink, ref ExceptionText ) )
            {
                CswNbtImporter Importer = SchemaModTrnsctn.makeCswNbtImporter();

                //Add the Legacy ID before storing the definition
                string SourceTablePkColumnName = Importer.getRemoteDataDictionaryPkColumnName( _SourceTableName, CafDbLink );
                importBinding( SourceTablePkColumnName, LegacyID, "" );

                //Save the bindings in the DB
                Importer.storeDefinition( _importOrderTable, _importBindingsTable, _importRelationshipsTable, CafDbLink );

                //Optional extension to where clause. Logical deletes already excluded.
                WhereClause = WhereClause ?? string.Empty;
                if( false == string.IsNullOrEmpty( WhereClause ) && false == WhereClause.Trim().StartsWith( "and" ) )
                {
                    WhereClause = " and " + WhereClause;
                }

                //Populate the import queue
                string SqlText = "insert into nbtimportqueue@" + CafDbLink + " ( nbtimportqueueid, state, itempk, tablename, priority, errorlog ) " +
                                 @" select seq_nbtimportqueueid.nextval@" + CafDbLink + ", 'N', " + SourceTablePkColumnName + ", '" + _SourceTableName + "',0, '' from " + _SourceTableName + "@" + CafDbLink + " where deleted='0' " + WhereClause;
                SchemaModTrnsctn.execArbitraryPlatformNeutralSql( SqlText );
            }
            else
            {
                SchemaModTrnsctn.logError( ExceptionText );
            }
        }
    }


}
