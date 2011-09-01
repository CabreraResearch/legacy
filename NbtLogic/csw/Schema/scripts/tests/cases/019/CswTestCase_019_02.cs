﻿using System;
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

    public class CswTestCase_019_02 : CswUpdateSchemaTo
    {
        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_019.Purpose, "use test tables after a commmit and refresh cycle" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_019 _CswTstCaseRsrc_019 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_019_02( CswSchemaVersion CswSchemaVersion )
        {
            _CswSchemaVersion = CswSchemaVersion;

        }//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_019 = new CswTstCaseRsrc_019( _CswNbtSchemaModTrnsctn );
			
			CswTableSelect CswTableSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( Description, _CswTstCaseRsrc_019.ArbitraryTableName_01 );
            DataTable DataTable = CswTableSelect.getTable();

            if( DataTable.Rows.Count != _CswTstCaseRsrc_019.TotalTestRows )
                throw(new CswDniException("Number of rows in table did not match the number of inserted rows") );

            foreach( DataRow CurrentRow in DataTable.Rows )
            {
                if( CurrentRow[_CswTstCaseRsrc_019.ArbitraryColumnName_01].ToString() != _CswTstCaseRsrc_019.ArbitraryColumnValue )
                {
                    throw ( new CswDniException( "A row value in the test table did not match the inserted data" ) );
                }
            }

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
