using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
//using ChemSW.TblDn;
using ChemSW.Nbt.Schema;
using ChemSW.Core;

namespace ChemSW.Nbt.SchemaUpdaterAutoTest
{

    public class CswScmUpdt_TstCse_UniqueSequence_RollbackCreate : CswScmUpdt_TstCse
    {
        public CswScmUpdt_TstCse_UniqueSequence_RollbackCreate( )
            : base( "Rollback unqiue sequence creation" )
        {
        }//ctor

        private string _UniqueSequenceName = "testsequence";

        public override void runTest()
        {
            //Setup data

            _CswNbtSchemaModTrnsctn.beginTransaction();

            _CswNbtSchemaModTrnsctn.makeSequence( _UniqueSequenceName, string.Empty, string.Empty, string.Empty, 1 );

            _CswNbtSchemaModTrnsctn.rollbackTransaction();

            if ( _CswNbtSchemaModTrnsctn.doesSequenceExist( _UniqueSequenceName ) )
                throw ( new CswDniException( "Sequence " + _UniqueSequenceName + " was not rolled back after creation" ) );


        }//runTest()

    }//CswSchemaUpdaterTestCaseRollbackUniqueSequenceCreation

}//ChemSW.Nbt.Schema
