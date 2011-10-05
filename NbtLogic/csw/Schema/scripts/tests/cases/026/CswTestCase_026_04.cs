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

    public class CswTestCase_026_04 : CswUpdateSchemaTo
    {
        public override string Description { get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_026.Purpose, "update the base data" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_026 _CswTstCaseRsrc_026 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_026_04( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_026 = (CswTstCaseRsrc_026) CswTstCaseRsc;
		}//ctor


        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_026.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;


            //_CswTstCaseRsrc_026.TestNode.Properties[_CswTstCaseRsrc_026.BuiltInProp].AsText.Text = _CswTstCaseRsrc_026.UpdateValOfBuiltInProp;
            //_CswTstCaseRsrc_026.TestNode.Properties[_CswTstCaseRsrc_026.AddedProp].AsText.Text = _CswTstCaseRsrc_026.UpdateValOfAddedProp;
            //_CswTstCaseRsrc_026.TestNode.postChanges( true );


            //_CswTstCaseRsrc_026.TestNode.Properties[_CswTstCaseRsrc_026.BuiltInProp].ClearValue();
            //_CswTstCaseRsrc_026.TestNode.Properties[_CswTstCaseRsrc_026.AddedProp].ClearValue();
            //_CswTstCaseRsrc_026.TestNode.postChanges( true ); 

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
