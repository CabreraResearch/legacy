﻿using System.Collections.Generic;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_018_03 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_018.Purpose, "rename foreign key column and cause rollback" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_018 _CswTstCaseRsrc_018 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_018_03( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_018 = (CswTstCaseRsrc_018) CswTstCaseRsc;
		}//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_018.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;
			
			List<PkFkPair> PairList = _CswTstCaseRsrc_018.getPkFkPairs();
            foreach( PkFkPair CurrentPair in PairList )
            {
                _CswNbtSchemaModTrnsctn.renameColumn( CurrentPair.FkTableName, CurrentPair.FkTablePkColumnName, CurrentPair.FkTablePkColumnName + _CswTstCaseRsrc_018.PkTablePkColumnNameNewNameSuffix );
            }


            throw ( new CswDniExceptionIgnoreDeliberately() ); 
        
        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
