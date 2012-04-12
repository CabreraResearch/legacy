using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 25768
    /// </summary>
    public class CswUpdateSchemaCase25768 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // Enable CISPro module on master schema only

            // This is kind of a kludgey way to determine whether we're on a fresh master, but I think it's valid.
            CswNbtNode AdminNode = _CswNbtSchemaModTrnsctn.Nodes.makeUserNodeFromUsername( "admin" );
            if( CswNbtNodeCaster.AsUser( AdminNode ).LastLogin.DateTimeValue.Date == new DateTime( 2011, 12, 9 ) )
            {
                CswTableUpdate ModulesUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "25768_modules_update", "modules" );
                DataTable ModulesTable = ModulesUpdate.getTable( "where name = 'CISPro'" );
                foreach( DataRow ModulesRow in ModulesTable.Rows )
                {
                    ModulesRow["enabled"] = CswConvert.ToDbVal( true );
                }
                ModulesUpdate.update( ModulesTable );
            }


        }//Update()

    }//class CswUpdateSchemaCase25768

}//namespace ChemSW.Nbt.Schema