using System;
using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27858
    /// </summary>
    public class CswUpdateSchema_01S_Case27858 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswTableUpdate ConfigVarUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "orphan_defaultvalueid_update", "nodetype_props" );
            DataTable ConfigVarTable = ConfigVarUpdate.getTable( " where defaultvalueid = 507071" );
            foreach( DataRow ConfigVarRow in ConfigVarTable.Rows )
            {
                ConfigVarRow["defaultvalueid"] = DBNull.Value;
            }
            ConfigVarUpdate.update( ConfigVarTable );
        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 27858; }
        }

        //Update()

    }//class CswUpdateSchemaCase27858

}//namespace ChemSW.Nbt.Schema