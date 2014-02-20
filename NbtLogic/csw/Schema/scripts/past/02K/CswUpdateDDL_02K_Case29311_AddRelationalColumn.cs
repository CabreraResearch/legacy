using System;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class CswUpdateDDL_02K_Case29311_AddRelationalColumn : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 29311; }
        }

        public override string AppendToScriptName()
        {
            return "AddRelationalColumn";
        }

        public override void update()
        {
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodes", "relationalid" ) )
            {
                _CswNbtSchemaModTrnsctn.addLongColumn( "nodes", "relationalid", "Foreign key to relational-model copy of this node", false );
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodes", "relationaltable" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "nodes", "relationaltable", "Table of relational-model copy of this node", false, 50 );
            }
        } // update()

    }//class CswUpdateMetaData_02K_Case29311_AddRelationalColumn
}//namespace ChemSW.Nbt.Schema


