using System.Data;
using ChemSW.DB;
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
            {
                string Tablename = "import_definitions";
                if( false == _CswNbtSchemaModTrnsctn.isTableDefined( Tablename ) )
                {
                    _CswNbtSchemaModTrnsctn.addTable( Tablename, "importdefinitionid" );
                    _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, "definitionname", "Name of import definition", false, true, 30 );
                }
            }
            {
                string Tablename = "import_order";
                if( false == _CswNbtSchemaModTrnsctn.isTableDefined( Tablename ) )
                {
                    _CswNbtSchemaModTrnsctn.addTable( Tablename, "importorderid" );
                    _CswNbtSchemaModTrnsctn.addForeignKeyColumn( Tablename, "importdefinitionid", "FK to import definition", false, true, "import_definition", "importdefinitionid" );
                    _CswNbtSchemaModTrnsctn.addLongColumn( Tablename, "importorder", "Numerical order of import", false, true );
                    _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, "sourcesheetname", "Name of source sheet", false, true, 50 );
                    _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, "nodetypename", "NodeType Name to import", false, true, 50 );
                    _CswNbtSchemaModTrnsctn.addLongColumn( Tablename, "instance", "Instance for mapping imported relationships", false, true );
                }
            }
            {
                string Tablename = "import_bindings";
                if( false == _CswNbtSchemaModTrnsctn.isTableDefined( Tablename ) )
                {
                    _CswNbtSchemaModTrnsctn.addTable( Tablename, "importbindingid" );
                    _CswNbtSchemaModTrnsctn.addForeignKeyColumn( Tablename, "importdefinitionid", "FK to import definition", false, true, "import_definition", "importdefinitionid" );
                    _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, "sourcesheetname", "Name of source sheet", false, true, 50 );
                    _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, "sourcecolumnname", "Name of column in source data", false, true, 50 );
                    _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, "destnodetypename", "NodeType Name of property in destination data", false, true, 50 );
                    _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, "destpropname", "Property name of property in destination data", false, true, 512 );
                    _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, "destsubfield", "Subfield of property in destination data", false, true, 50 );
                    _CswNbtSchemaModTrnsctn.addLongColumn( Tablename, "instance", "Instance for mapping imported relationships", false, true );
                }
            }
            {
                string Tablename = "import_relationships";
                if( false == _CswNbtSchemaModTrnsctn.isTableDefined( Tablename ) )
                {
                    _CswNbtSchemaModTrnsctn.addTable( Tablename, "importrelationshipid" );
                    _CswNbtSchemaModTrnsctn.addForeignKeyColumn( Tablename, "importdefinitionid", "FK to import definition", false, true, "import_definition", "importdefinitionid" );
                    _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, "sourcesheetname", "Name of source sheet", false, true, 50 );
                    _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, "nodetypename", "NodeType Name to import", false, true, 50 );
                    _CswNbtSchemaModTrnsctn.addStringColumn( Tablename, "relationship", "Property name of relationship property", false, true, 512 );
                    _CswNbtSchemaModTrnsctn.addLongColumn( Tablename, "instance", "Instance for mapping imported relationships", false, true );
                }
            }
        } // update()

    }//class RunBeforeEveryExecutionOfUpdater_02F_Case30040
}//namespace ChemSW.Nbt.Schema


