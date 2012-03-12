using System;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.MetaData;


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 24520
    /// </summary>
    public class CswUpdateSchemaCase24520 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // also in 01M-01

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "audit_transactions", "transactionfirstname" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "audit_transactions", "transactionfirstname", "First name of transaction user", false, false, 50 );
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "audit_transactions", "transactionlastname" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "audit_transactions", "transactionlastname", "Last name of transaction user", false, false, 50 );
            }



        }//Update()

    }//class CswUpdateSchemaCase24520

}//namespace ChemSW.Nbt.Schema