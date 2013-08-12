using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.ImportExport;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for OC changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02F_Case30040 : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: Case 30040";

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 30040; }
        }

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( CswNbt2DImportTables.ImportDef.TableName ) )
            {
                string Tablename = CswNbt2DImportTables.ImportDef.TableName;
                _CswNbtSchemaModTrnsctn.addTable( Tablename, CswNbt2DImportTables.ImportDef.PkColumnName );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbt2DImportTables.ImportDef.definitionname, "Name of import definition", false, true, 30 );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbt2DImportTables.ImportDef.sheetname, "Name of source sheet", false, true, 50 );
                _CswNbtSchemaModTrnsctn.addLongColumn( Tablename, CswNbt2DImportTables.ImportDef.sheetorder, "Numerical order of sheet import", false, true );
            }
            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( CswNbt2DImportTables.ImportDefOrder.TableName ) )
            {
                string Tablename = CswNbt2DImportTables.ImportDefOrder.TableName;
                _CswNbtSchemaModTrnsctn.addTable( Tablename, CswNbt2DImportTables.ImportDefOrder.PkColumnName );
                _CswNbtSchemaModTrnsctn.addForeignKeyColumn( Tablename, CswNbt2DImportTables.ImportDefOrder.importdefinitionid, "FK to import definition", false, true, "import_definition", "importdefinitionid" );
                _CswNbtSchemaModTrnsctn.addLongColumn( Tablename, CswNbt2DImportTables.ImportDefOrder.importorder, "Numerical order of import", false, true );
                //_CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbt2DImportTables.ImportOrder.sourcesheetname, "Name of source sheet", false, true, 50 );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbt2DImportTables.ImportDefOrder.nodetypename, "NodeType Name to import", false, true, 50 );
                _CswNbtSchemaModTrnsctn.addLongColumn( Tablename, CswNbt2DImportTables.ImportDefOrder.instance, "Instance for mapping imported relationships", false, true );
            }
            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( CswNbt2DImportTables.ImportDefBindings.TableName ) )
            {
                string Tablename = CswNbt2DImportTables.ImportDefBindings.TableName;
                _CswNbtSchemaModTrnsctn.addTable( Tablename, CswNbt2DImportTables.ImportDefBindings.PkColumnName );
                _CswNbtSchemaModTrnsctn.addForeignKeyColumn( Tablename, CswNbt2DImportTables.ImportDefBindings.importdefinitionid, "FK to import definition", false, true, "import_definition", "importdefinitionid" );
                //_CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbt2DImportTables.ImportBindings.sourcesheetname, "Name of source sheet", false, true, 50 );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbt2DImportTables.ImportDefBindings.sourcecolumnname, "Name of column in source data", false, true, 50 );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbt2DImportTables.ImportDefBindings.destnodetypename, "NodeType Name of property in destination data", false, true, 50 );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbt2DImportTables.ImportDefBindings.destpropname, "Property name of property in destination data", false, true, 512 );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbt2DImportTables.ImportDefBindings.destsubfield, "Subfield of property in destination data", false, true, 50 );
                _CswNbtSchemaModTrnsctn.addLongColumn( Tablename, CswNbt2DImportTables.ImportDefBindings.instance, "Instance for mapping imported relationships", false, true );
            }
            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( CswNbt2DImportTables.ImportDefRelationships.TableName ) )
            {
                string Tablename = CswNbt2DImportTables.ImportDefRelationships.TableName;
                _CswNbtSchemaModTrnsctn.addTable( Tablename, CswNbt2DImportTables.ImportDefRelationships.PkColumnName );
                _CswNbtSchemaModTrnsctn.addForeignKeyColumn( Tablename, CswNbt2DImportTables.ImportDefRelationships.importdefinitionid, "FK to import definition", false, true, "import_definition", "importdefinitionid" );
                //_CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbt2DImportTables.ImportRelationships.sourcesheetname, "Name of source sheet", false, true, 50 );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbt2DImportTables.ImportDefRelationships.nodetypename, "NodeType Name to import", false, true, 50 );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbt2DImportTables.ImportDefRelationships.relationship, "Property name of relationship property", false, true, 512 );
                _CswNbtSchemaModTrnsctn.addLongColumn( Tablename, CswNbt2DImportTables.ImportDefRelationships.instance, "Instance for mapping imported relationships", false, true );
            }
            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( CswNbt2DImportTables.ImportDataMap.TableName ) )
            {
                string Tablename = CswNbt2DImportTables.ImportDataMap.TableName;
                _CswNbtSchemaModTrnsctn.addTable( Tablename, CswNbt2DImportTables.ImportDataMap.PkColumnName );
                _CswNbtSchemaModTrnsctn.addForeignKeyColumn( Tablename, CswNbt2DImportTables.ImportDataMap.importdefinitionid, "FK to import definition", false, true, "import_definition", "importdefinitionid" );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbt2DImportTables.ImportDataMap.datatablename, "Oracle table name for data", false, true, 50 );
                //_CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbt2DImportTables.ImportDataMap.sheetname, "Name of source sheet", false, true, 50 );
                _CswNbtSchemaModTrnsctn.addBooleanColumn( Tablename, CswNbt2DImportTables.ImportDataMap.overwrite, "When importing, whether to overwrite existing nodes", false, true );
                _CswNbtSchemaModTrnsctn.addBooleanColumn( Tablename, CswNbt2DImportTables.ImportDataMap.completed, "If true, the import is completed", false, true );
            }
        } // update()

    }//class RunBeforeEveryExecutionOfUpdater_02F_Case30040
}//namespace ChemSW.Nbt.Schema


