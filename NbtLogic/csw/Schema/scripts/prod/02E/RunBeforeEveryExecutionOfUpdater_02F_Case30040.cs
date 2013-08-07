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
            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( CswNbt2DImportTables.ImportDefinitions.TableName ) )
            {
                string Tablename = CswNbt2DImportTables.ImportDefinitions.TableName;
                _CswNbtSchemaModTrnsctn.addTable( Tablename, CswNbt2DImportTables.ImportDefinitions.PkColumnName );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbt2DImportTables.ImportDefinitions.definitionname, "Name of import definition", false, true, 30 );
            }
            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( CswNbt2DImportTables.ImportOrder.TableName ) )
            {
                string Tablename = CswNbt2DImportTables.ImportOrder.TableName;
                _CswNbtSchemaModTrnsctn.addTable( Tablename, CswNbt2DImportTables.ImportOrder.PkColumnName );
                _CswNbtSchemaModTrnsctn.addForeignKeyColumn( Tablename, CswNbt2DImportTables.ImportOrder.importdefinitionid, "FK to import definition", false, true, "import_definition", "importdefinitionid" );
                _CswNbtSchemaModTrnsctn.addLongColumn( Tablename, CswNbt2DImportTables.ImportOrder.importorder, "Numerical order of import", false, true );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbt2DImportTables.ImportOrder.sourcesheetname, "Name of source sheet", false, true, 50 );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbt2DImportTables.ImportOrder.nodetypename, "NodeType Name to import", false, true, 50 );
                _CswNbtSchemaModTrnsctn.addLongColumn( Tablename, CswNbt2DImportTables.ImportOrder.instance, "Instance for mapping imported relationships", false, true );
            }
            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( CswNbt2DImportTables.ImportBindings.TableName ) )
            {
                string Tablename = CswNbt2DImportTables.ImportBindings.TableName;
                _CswNbtSchemaModTrnsctn.addTable( Tablename, CswNbt2DImportTables.ImportBindings.PkColumnName );
                _CswNbtSchemaModTrnsctn.addForeignKeyColumn( Tablename, CswNbt2DImportTables.ImportBindings.importdefinitionid, "FK to import definition", false, true, "import_definition", "importdefinitionid" );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbt2DImportTables.ImportBindings.sourcesheetname, "Name of source sheet", false, true, 50 );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbt2DImportTables.ImportBindings.sourcecolumnname, "Name of column in source data", false, true, 50 );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbt2DImportTables.ImportBindings.destnodetypename, "NodeType Name of property in destination data", false, true, 50 );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbt2DImportTables.ImportBindings.destpropname, "Property name of property in destination data", false, true, 512 );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbt2DImportTables.ImportBindings.destsubfield, "Subfield of property in destination data", false, true, 50 );
                _CswNbtSchemaModTrnsctn.addLongColumn( Tablename, CswNbt2DImportTables.ImportBindings.instance, "Instance for mapping imported relationships", false, true );
            }
            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( CswNbt2DImportTables.ImportRelationships.TableName ) )
            {
                string Tablename = CswNbt2DImportTables.ImportRelationships.TableName;
                _CswNbtSchemaModTrnsctn.addTable( Tablename, CswNbt2DImportTables.ImportRelationships.PkColumnName );
                _CswNbtSchemaModTrnsctn.addForeignKeyColumn( Tablename, CswNbt2DImportTables.ImportRelationships.importdefinitionid, "FK to import definition", false, true, "import_definition", "importdefinitionid" );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbt2DImportTables.ImportRelationships.sourcesheetname, "Name of source sheet", false, true, 50 );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbt2DImportTables.ImportRelationships.nodetypename, "NodeType Name to import", false, true, 50 );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbt2DImportTables.ImportRelationships.relationship, "Property name of relationship property", false, true, 512 );
                _CswNbtSchemaModTrnsctn.addLongColumn( Tablename, CswNbt2DImportTables.ImportRelationships.instance, "Instance for mapping imported relationships", false, true );
            }
        } // update()

    }//class RunBeforeEveryExecutionOfUpdater_02F_Case30040
}//namespace ChemSW.Nbt.Schema


