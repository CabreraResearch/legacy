using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class CswUpdateMetaData_02K_Case31616B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31616; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            CswTableUpdate NodeTypesUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "31616_nodetypes_update", "nodetypes" );
            DataTable NodeTypesTable = NodeTypesUpdate.getTable();
            foreach( DataRow NodeTypeRow in NodeTypesTable.Rows )
            {
                if( NodeTypeRow["searchdeferpropid"].ToString() == "0" )
                {
                    NodeTypeRow["searchable"] = CswConvert.ToDbVal( false );
                    NodeTypeRow["searchdeferpropid"] = DBNull.Value;
                }
                else
                {
                    NodeTypeRow["searchable"] = CswConvert.ToDbVal( true );
                }
            }
            NodeTypesUpdate.update( NodeTypesTable );
        } // update()

    }//class CswUpdateMetaData_02K_Case31616B
}//namespace ChemSW.Nbt.Schema


