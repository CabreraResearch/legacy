
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_026_02 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_026.Purpose, "create test data to populate table" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_026 _CswTstCaseRsrc_026 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_026_02( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_026 = (CswTstCaseRsrc_026) CswTstCaseRsc;

        }//ctor


        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_026.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;

            //_CswTstCaseRsrc_026.TestNode.Properties[_CswTstCaseRsrc_026.BuiltInProp].AsText.Text = _CswTstCaseRsrc_026.InsertValOfBuiltInProp;
            //_CswTstCaseRsrc_026.TestNode.Properties[_CswTstCaseRsrc_026.AddedProp].AsText.Text = _CswTstCaseRsrc_026.InsertValOfAddedProp;
            //_CswTstCaseRsrc_026.TestNode.postChanges( true );

            //_CswTstCaseRsrc_026.JctNodePropIdOfBuiltInProp = _CswTstCaseRsrc_026.TestNode.Properties[_CswTstCaseRsrc_026.BuiltInProp].JctNodePropId;
            //_CswTstCaseRsrc_026.JctNodePropIdOfAddedProp = _CswTstCaseRsrc_026.TestNode.Properties[_CswTstCaseRsrc_026.AddedProp].JctNodePropId;

        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.PG; }
        }

        //runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
