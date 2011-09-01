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
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_026_02 : CswUpdateSchemaTo
    {
        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_026.Purpose, "create test data to populate table" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_026 _CswTstCaseRsrc_026 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_026_02( CswSchemaVersion CswSchemaVersion )
        {
            _CswSchemaVersion = CswSchemaVersion;

        }//ctor


        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_026 = new CswTstCaseRsrc_026( _CswNbtSchemaModTrnsctn );

            //_CswTstCaseRsrc_026.TestNode.Properties[_CswTstCaseRsrc_026.BuiltInProp].AsText.Text = _CswTstCaseRsrc_026.InsertValOfBuiltInProp;
            //_CswTstCaseRsrc_026.TestNode.Properties[_CswTstCaseRsrc_026.AddedProp].AsText.Text = _CswTstCaseRsrc_026.InsertValOfAddedProp;
            //_CswTstCaseRsrc_026.TestNode.postChanges( true );

            //_CswTstCaseRsrc_026.JctNodePropIdOfBuiltInProp = _CswTstCaseRsrc_026.TestNode.Properties[_CswTstCaseRsrc_026.BuiltInProp].JctNodePropId;
            //_CswTstCaseRsrc_026.JctNodePropIdOfAddedProp = _CswTstCaseRsrc_026.TestNode.Properties[_CswTstCaseRsrc_026.AddedProp].JctNodePropId;

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
