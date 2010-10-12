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

    public class CswScmUpdt_TstCse_UniqueSequence_RollbackDelete : CswScmUpdt_TstCse
    {
        public CswScmUpdt_TstCse_UniqueSequence_RollbackDelete( )
            : base( "Rollback unique sequence removal" )
        {
        }//ctor

        private string _SequenceName = "testsequence";
        private string _Prepend = "foo";
        private string _Postpend = "bar";
        private string _Pad = "5";
        private Int32 _InitialVal = 42;

        public override void runTest()
        {
            //Set up data
            _CswNbtSchemaModTrnsctn.beginTransaction();
            _CswNbtSchemaModTrnsctn.makeSequence( _SequenceName, _Prepend, _Postpend, _Pad, _InitialVal );
            _CswNbtSchemaModTrnsctn.commitTransaction();
            _testSequenceExistence( "in setup" );


            //run actual test
            _CswNbtSchemaModTrnsctn.beginTransaction();
            _CswNbtSchemaModTrnsctn.removeSequence( _SequenceName );
            _CswNbtSchemaModTrnsctn.rollbackTransaction();
            _testSequenceExistence( "after rollback" );

            //tear down data
            _CswNbtSchemaModTrnsctn.beginTransaction();
            _CswNbtSchemaModTrnsctn.removeSequence( _SequenceName );
            _CswNbtSchemaModTrnsctn.commitTransaction();

        }//runTest()

        private void _testSequenceExistence( string TestPhase )
        {
            if ( !_CswNbtSchemaModTrnsctn.doesSequenceExist( _SequenceName ) )
                throw ( new CswDniException( "Sequence " + _SequenceName + " was not created " + TestPhase ) );

            DataTable SequenceTable = _CswNbtSchemaModTrnsctn.getSequence( _SequenceName );
            if ( SequenceTable.Rows[ 0 ][ "prep" ].ToString() != _Prepend ||
                SequenceTable.Rows[ 0 ][ "post" ].ToString() != _Postpend ||
                SequenceTable.Rows[ 0 ][ "pad" ].ToString() != _Pad )
            {
                throw ( new CswDniException( "A sequence configuration parameter did not match in " + TestPhase ) );
            }

            if ( _InitialVal != _CswNbtSchemaModTrnsctn.getSequenceValue( _SequenceName ) )
                throw ( new CswDniException( "The initial sequence value was not set correctly in " + TestPhase ) );

        }//_testSequenceExistence()

    }//CswSchemaUpdaterTestCaseRollbackUniqueSequenceRemoval

}//ChemSW.Nbt.Schema
