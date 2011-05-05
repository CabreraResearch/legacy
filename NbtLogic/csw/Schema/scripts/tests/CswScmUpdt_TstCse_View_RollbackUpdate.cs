using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
using ChemSW.DB;
using ChemSW.Nbt.Schema;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{

    public class CswScmUpdt_TstCse_View_RollbackUpdate : CswScmUpdt_TstCse
    {
        public CswScmUpdt_TstCse_View_RollbackUpdate( )
            : base( "Rollback view updates" )
        {
        }//ctor

        private string _ViewTable = "node_views";

        public override void runTest()
        {

            _CswNbtSchemaModTrnsctn.beginTransaction();

            CswTableSelect ViewsSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "viewwidth_select", _ViewTable );
            Int32 TotalRowsBeforeRollback = ViewsSelect.getTable( @" where viewxml like '%width=""100%'" ).Rows.Count;


            try
            {
                DataTable ViewsTable = _CswNbtSchemaModTrnsctn.getAllViews();
                foreach ( DataRow ViewsRow in ViewsTable.Rows )
                {
                    CswNbtView ThisView = _CswNbtSchemaModTrnsctn.restoreView(
                              CswConvert.ToInt32( ViewsRow[ "nodeviewid" ] ) );

                    if ( ThisView.Width != Int32.MinValue && ThisView.Width != 100 )
                        ThisView.Width = ThisView.Width / 6;   // change from pixels to characters

                    ThisView.save();
                }
            }

            catch ( Exception Exception )
            {
                //ignore exception
            }//


            _CswNbtSchemaModTrnsctn.rollbackTransaction();
                
            Int32 TotalRowsAfterRollback = ViewsSelect.getTable( @" where viewxml like '%width=""100%'"  ).Rows.Count;

            if( TotalRowsAfterRollback != TotalRowsBeforeRollback )
                throw( new CswDniException( "Change applied to node_views table persists after rollback" ) );



        }//runTest()

    }//CswSchemaUpdaterTestCaseRollbackViewUpdates

}//ChemSW.Nbt.Schema
