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

    public class CswTestCase_018_03 : CswUpdateSchemaTo
    {
        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_018.Purpose, "rename foreign key column and cause rollback" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_018 _CswTstCaseRsrc_018 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_018_03( CswSchemaVersion CswSchemaVersion )
        {
            _CswSchemaVersion = CswSchemaVersion;
        }//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_018 = new CswTstCaseRsrc_018( _CswNbtSchemaModTrnsctn );
			
			List<PkFkPair> PairList = _CswTstCaseRsrc_018.getPkFkPairs();
            foreach( PkFkPair CurrentPair in PairList )
            {
                _CswNbtSchemaModTrnsctn.renameColumn( CurrentPair.FkTableName, CurrentPair.FkTablePkColumnName, CurrentPair.FkTablePkColumnName + _CswTstCaseRsrc_018.PkTablePkColumnNameNewNameSuffix );
            }


            throw ( new CswDniExceptionIgnoreDeliberately() ); 
        
        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
