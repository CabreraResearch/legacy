using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.DB;
using ChemSW.Nbt.Schema;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_004_04_015 : ICswUpdateSchemaTo
    {


        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'T', 015 ); } }

        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_004.Purpose, "verify test data tear-down" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_004 _CswTstCaseRsrc_004 = null;
        public CswTestCase_004_04_015( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            _CswTstCaseRsrc_004 = new CswTstCaseRsrc_004( _CswNbtSchemaModTrnsctn );
        }//ctor

        public void update()
        {
            List<PkFkPair> PairList = _CswTstCaseRsrc_004.getPkFkPairs();

            //Verify that we cleaned up after ourselves
            foreach( PkFkPair CurrentPair in PairList )
            {
                if( _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( CurrentPair.PkTableName ) )
                    throw ( new CswDniException( "Table " + CurrentPair.PkTableName + " was not dropped from the database" ) );

                if( _CswNbtSchemaModTrnsctn.isTableDefinedInDataBase( CurrentPair.FkTableName ) )
                    throw ( new CswDniException( "Table " + CurrentPair.FkTableName + " was not dropped from the database" ) );
            }


        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
