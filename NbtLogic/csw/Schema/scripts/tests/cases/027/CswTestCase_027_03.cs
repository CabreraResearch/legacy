using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using System.Diagnostics;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.DB;
using ChemSW.Nbt.Schema;
using ChemSW.Core;
using ChemSW.Log;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_027_03 : CswUpdateSchemaTo
    {

        public override string Description { get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_027.Purpose, "Do table update " + _TotalUpdateTestLoops.ToString() + " times" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_027 _CswTstCaseRsrc_027 = null;

        private Int32 _TotalUpdateTestLoops = 1000;

        private CswSchemaVersion _CswSchemaVersion = null;
        public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_027_03( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_027 = (CswTstCaseRsrc_027) CswTstCaseRsc;

        }//ctor


        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_027.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;
			
			for( int idx = 0; idx < _TotalUpdateTestLoops; idx++ )
            {

                Int32 TestEveryNth = 10;
                bool DoMemoryTest = ( 0 == ( idx % TestEveryNth ) );

                if( DoMemoryTest )
                {
                    _CswTstCaseRsrc_027.memoryTestBegin();
                }

                CswTableUpdate CswTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( Description, _CswTstCaseRsrc_027.ArbitraryTableName_01 );
                DataTable DataTable = CswTableUpdate.getTable();
                foreach( DataRow CurrentRow in DataTable.Rows )
                {
                    CurrentRow[_CswTstCaseRsrc_027.ArbitraryColumnName_02] = idx.ToString() + "_" + _CswTstCaseRsrc_027.ArbitraryColumnValue;
                    CurrentRow[_CswTstCaseRsrc_027.ArbitraryColumnName_02] = idx.ToString() + "_" + _CswTstCaseRsrc_027.ArbitraryColumnValue;
                }

                CswTableUpdate.update( DataTable );

                if( DoMemoryTest )
                {
                    _CswTstCaseRsrc_027.memoryTestEnd();
                    _CswNbtSchemaModTrnsctn.CswLogger.reportAppState( Description + ": Total Process Memory after " + idx.ToString() + "iterations: " + _CswTstCaseRsrc_027.TotalProcessMemory );
                    _CswNbtSchemaModTrnsctn.CswLogger.reportAppState( Description + ": Total GC Memory after: " + idx.ToString() + "iterations: " + _CswTstCaseRsrc_027.TotalGCMemorySansCollection );
                    _CswNbtSchemaModTrnsctn.CswLogger.reportAppState( Description + ": ProcessMemory Delta after: " + idx.ToString() + "iterations:  " + _CswTstCaseRsrc_027.ProcessMemoryDelta );
                    _CswNbtSchemaModTrnsctn.CswLogger.reportAppState( Description + ": GCMemory Delta: after " + _CswTstCaseRsrc_027.GCMemoryDelta );
                }

            }

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
