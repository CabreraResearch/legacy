using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.DB;
using ChemSW.Nbt.Schema;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_014_01 : ICswUpdateSchemaTo
    {


        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_014.Purpose, "Add test row to materials table" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_014 _CswTstCaseRsrc_014 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_014_01( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn, CswSchemaVersion CswSchemaVersion, object CswTstCaseRsrc )
        {
            _CswSchemaVersion = CswSchemaVersion;
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            _CswTstCaseRsrc_014 = (CswTstCaseRsrc_014) CswTstCaseRsrc;
        }//ctor

        public void update()
        {


            CswTableUpdate CswTableUpdateSetup = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( _CswTstCaseRsrc_014.Purpose + "_setupdata", "materials" );
            DataTable MaterialsTable = CswTableUpdateSetup.getEmptyTable();
            DataRow NewMaterialsRow = MaterialsTable.NewRow();
            MaterialsTable.Rows.Add( NewMaterialsRow );
            _CswTstCaseRsrc_014.InsertedMaterialsRecordPk = Convert.ToInt32( MaterialsTable.Rows[0]["materialid"] );
            MaterialsTable.Rows[0]["materialname"] = _CswTstCaseRsrc_014.Purpose + "--test row";
            CswTableUpdateSetup.update( MaterialsTable );

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
