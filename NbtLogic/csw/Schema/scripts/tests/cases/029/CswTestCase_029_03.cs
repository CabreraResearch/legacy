using System;
using System.Data;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_029_03 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_029.Purpose, "Test for constraint violation" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_029 _CswTstCaseRsrc_029 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_029_03( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
            _CswTstCaseRsrc_029 = (CswTstCaseRsrc_029) CswTstCaseRsc;

        }//ctor


        public override void update()
        {
            _CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            _CswTstCaseRsrc_029.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;



            if( false == _CswNbtSchemaModTrnsctn.doesUniqueConstraintExistInDb( _CswTstCaseRsrc_029.CreatedConstraintName ) )
            {
                throw( new CswDniException( "The constraint created in the previous step named " + _CswTstCaseRsrc_029.CreatedConstraintName + " is not detected now" ) ); 
            }

            string RetrievedConstraintName = _CswNbtSchemaModTrnsctn.getUniqueConstraintName( _CswTstCaseRsrc_029.ArbitraryTableName_01, _CswTstCaseRsrc_029.ArbitraryColumnName_01_Unique );
            if( _CswTstCaseRsrc_029.CreatedConstraintName.ToLower() != RetrievedConstraintName.ToLower() )
            {
                throw ( new CswDniException( "The unique constraint created in the previous step named " + _CswTstCaseRsrc_029.CreatedConstraintName + " is not detected now for table.column " + _CswTstCaseRsrc_029.ArbitraryTableName_01 + "." + _CswTstCaseRsrc_029.ArbitraryColumnName_01_Unique ) );
            }

            bool CorrectExceptionWasThrown = true; 
            try
            {
                CswTableUpdate CswTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "testuniqueconstrainttable", _CswTstCaseRsrc_029.ArbitraryTableName_01 );
                DataTable DataTable = CswTableUpdate.getEmptyTable();

                DataRow DataRow1 = DataTable.NewRow();
                DataRow1[_CswTstCaseRsrc_029.ArbitraryColumnName_01_Unique] = _CswTstCaseRsrc_029.ArbitraryColumnValue;
                DataTable.Rows.Add( DataRow1 );

                DataRow DataRow2 = DataTable.NewRow();
                DataRow2[_CswTstCaseRsrc_029.ArbitraryColumnName_01_Unique] = _CswTstCaseRsrc_029.ArbitraryColumnValue;
                DataTable.Rows.Add( DataRow2 );

                CswTableUpdate.update( DataTable );
            }

            catch( Exception Exception )
            {
                if( Exception.Message.Contains( "ORA-00001: unique constraint" ) )
                {
                    CorrectExceptionWasThrown = true;
                }
                else
                {
                    throw ( new CswDniException( "Exception thrown while testing for unique constraint violation was not the exception expected : " + Exception.Message ) );
                }
            }

            if( false == CorrectExceptionWasThrown )
            {
                throw( new CswDniException( "An exception should have been thrown when trying to add the same value to a uniquely constrained column, but, alas, 'twas not thrown!" ) ); 
            }

        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.PG; }
        }

        public override int CaseNo
        {
            get { return 0; }
        }

        //runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
