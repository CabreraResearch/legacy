using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 25737
    /// </summary>
    public class CswUpdateSchemaCase25737 : CswUpdateSchemaTo
    {
        public override void update()
        {
            //Delete all 'search' welcome components
            CswTableUpdate WelcomeUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "25737_welcome_update", "welcome" );
            DataTable WelcomeTable = WelcomeUpdate.getTable( "where componenttype = 'Search'" );
            foreach( DataRow WelcomeRow in WelcomeTable.Rows )
            {
                WelcomeRow.Delete();
            }
            WelcomeUpdate.update( WelcomeTable );

        }//Update()

    }//class CswUpdateSchemaCase25737

}//namespace ChemSW.Nbt.Schema