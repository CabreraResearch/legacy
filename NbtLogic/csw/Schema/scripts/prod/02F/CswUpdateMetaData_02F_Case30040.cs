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
    public class CswUpdateMetaData_02F_Case30040 : CswUpdateSchemaTo
    {
        public override string Title { get { return "Pre-Script: Case 30040"; } }
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 30040; }
        }

        public override string ScriptName
        {
            get { throw new System.NotImplementedException(); }
        }

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( CswNbtImportTables.ImportDef.TableName ) )
            {
                string Tablename = CswNbtImportTables.ImportDef.TableName;
                _CswNbtSchemaModTrnsctn.addTable( Tablename, CswNbtImportTables.ImportDef.PkColumnName );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbtImportTables.ImportDef.definitionname, "Name of import definition", false, true, 30 );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbtImportTables.ImportDef.sheetname, "Name of source sheet", false, true, 50 );
                _CswNbtSchemaModTrnsctn.addLongColumn( Tablename, CswNbtImportTables.ImportDef.sheetorder, "Numerical order of sheet import", false, true );
            }
            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( CswNbtImportTables.ImportDefOrder.TableName ) )
            {
                string Tablename = CswNbtImportTables.ImportDefOrder.TableName;
                _CswNbtSchemaModTrnsctn.addTable( Tablename, CswNbtImportTables.ImportDefOrder.PkColumnName );
                _CswNbtSchemaModTrnsctn.addForeignKeyColumn( Tablename, CswNbtImportTables.ImportDefOrder.importdefid, "FK to import definition", false, true, CswNbtImportTables.ImportDef.TableName, CswNbtImportTables.ImportDef.importdefid );
                _CswNbtSchemaModTrnsctn.addLongColumn( Tablename, CswNbtImportTables.ImportDefOrder.importorder, "Numerical order of import", false, true );
                //_CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbt2DImportTables.ImportOrder.sourcesheetname, "Name of source sheet", false, true, 50 );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbtImportTables.ImportDefOrder.nodetypename, "NodeType Name to import", false, true, 50 );
                _CswNbtSchemaModTrnsctn.addLongColumn( Tablename, CswNbtImportTables.ImportDefOrder.instance, "Instance for mapping imported relationships", false, true );
            }
            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( CswNbtImportTables.ImportDefBindings.TableName ) )
            {
                string Tablename = CswNbtImportTables.ImportDefBindings.TableName;
                _CswNbtSchemaModTrnsctn.addTable( Tablename, CswNbtImportTables.ImportDefBindings.PkColumnName );
                _CswNbtSchemaModTrnsctn.addForeignKeyColumn( Tablename, CswNbtImportTables.ImportDefBindings.importdefid, "FK to import definition", false, true, CswNbtImportTables.ImportDef.TableName, CswNbtImportTables.ImportDef.importdefid );
                //_CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbt2DImportTables.ImportBindings.sourcesheetname, "Name of source sheet", false, true, 50 );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbtImportTables.ImportDefBindings.sourcecolumnname, "Name of column in source data", false, true, 50 );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbtImportTables.ImportDefBindings.destnodetypename, "NodeType Name of property in destination data", false, true, 50 );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbtImportTables.ImportDefBindings.destpropname, "Property name of property in destination data", false, true, 512 );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbtImportTables.ImportDefBindings.destsubfield, "Subfield of property in destination data", false, true, 50 );
                _CswNbtSchemaModTrnsctn.addLongColumn( Tablename, CswNbtImportTables.ImportDefBindings.instance, "Instance for mapping imported relationships", false, true );
            }
            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( CswNbtImportTables.ImportDefRelationships.TableName ) )
            {
                string Tablename = CswNbtImportTables.ImportDefRelationships.TableName;
                _CswNbtSchemaModTrnsctn.addTable( Tablename, CswNbtImportTables.ImportDefRelationships.PkColumnName );
                _CswNbtSchemaModTrnsctn.addForeignKeyColumn( Tablename, CswNbtImportTables.ImportDefRelationships.importdefid, "FK to import definition", false, true, CswNbtImportTables.ImportDef.TableName, CswNbtImportTables.ImportDef.importdefid );
                //_CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbt2DImportTables.ImportRelationships.sourcesheetname, "Name of source sheet", false, true, 50 );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbtImportTables.ImportDefRelationships.nodetypename, "NodeType Name to import", false, true, 50 );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbtImportTables.ImportDefRelationships.relationship, "Property name of relationship property", false, true, 512 );
                _CswNbtSchemaModTrnsctn.addLongColumn( Tablename, CswNbtImportTables.ImportDefRelationships.instance, "Instance for mapping imported relationships", false, true );
            }
            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( CswNbtImportTables.ImportDataMap.TableName ) )
            {
                string Tablename = CswNbtImportTables.ImportDataMap.TableName;
                _CswNbtSchemaModTrnsctn.addTable( Tablename, CswNbtImportTables.ImportDataMap.PkColumnName );
                _CswNbtSchemaModTrnsctn.addForeignKeyColumn( Tablename, CswNbtImportTables.ImportDataMap.importdefid, "FK to import definition", false, true, CswNbtImportTables.ImportDef.TableName, CswNbtImportTables.ImportDef.importdefid );
                _CswNbtSchemaModTrnsctn.addForeignKeyColumn( Tablename, CswNbtImportTables.ImportDataMap.importdatajobid, "FK to import job", false, true, CswNbtImportTables.ImportDataJob.TableName, CswNbtImportTables.ImportDataJob.importdatajobid );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbtImportTables.ImportDataMap.datatablename, "Oracle table name for data", false, true, 50 );
                //_CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbt2DImportTables.ImportDataMap.sheetname, "Name of source sheet", false, true, 50 );
                _CswNbtSchemaModTrnsctn.addBooleanColumn( Tablename, CswNbtImportTables.ImportDataMap.overwrite, "When importing, whether to overwrite existing nodes", false, true );
                _CswNbtSchemaModTrnsctn.addBooleanColumn( Tablename, CswNbtImportTables.ImportDataMap.completed, "If true, the import is completed", false, true );
            }
            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( CswNbtImportTables.ImportDataJob.TableName ) )
            {
                string Tablename = CswNbtImportTables.ImportDataJob.TableName;
                _CswNbtSchemaModTrnsctn.addTable( Tablename, CswNbtImportTables.ImportDataJob.PkColumnName );
                _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, CswNbtImportTables.ImportDataJob.filename, "Name of original excel file", false, true, 50 );
                _CswNbtSchemaModTrnsctn.addDateColumn( Tablename, CswNbtImportTables.ImportDataJob.datestarted, "Date import started", false, false );
                _CswNbtSchemaModTrnsctn.addDateColumn( Tablename, CswNbtImportTables.ImportDataJob.dateended, "Date import ended", false, false );
                _CswNbtSchemaModTrnsctn.addForeignKeyColumn( Tablename, CswNbtImportTables.ImportDataJob.userid, "FK to user who uploaded the job", false, true, "nodes", "nodeid" );
            }

        } // update()

    }//class RunBeforeEveryExecutionOfUpdater_02F_Case30040
}//namespace ChemSW.Nbt.Schema


