using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Schema;
using NUnit.Framework;

namespace ChemSW.Nbt.Test.MetaData
{
    [TestFixture]
    public class CswNbtMetaDataTest
    {
        private TestData _TestData;
        private CswNbtSchemaModTrnsctn _SchemaModTrnsctn;
        private CswNbtMetaDataObjectClass _fakeTestOC;

        [SetUp]
        public void MyTestInitialize()
        {
            _TestData = new TestData();
            _SchemaModTrnsctn = new CswNbtSchemaModTrnsctn( _TestData.CswNbtResources );
        }

        [TearDown]
        public void MyTestCleanup()
        {
            // As long as we don't commit, this isn't necessary (and in fact doesn't work)

            //if( null != _fakeTestOC )
            //{
            //    _SchemaModTrnsctn.MetaData.DeleteObjectClass( _fakeTestOC );
            //}
            //_TestData.Destroy();
        }

        [Test]
        public void testMetaData()
        {
            CswEnumNbtObjectClass fakeObjectClassName = CswEnumNbtObjectClass.FakeClass;
            string fakeNodeTypeName = "fake Test";

            // new object class
            _fakeTestOC = _SchemaModTrnsctn.createObjectClass( fakeObjectClassName, "doc.png", AuditLevel: false );
            Assert.IsNotNull( _fakeTestOC, "fakeTestOC was null" );

            // new object class prop
            CswNbtMetaDataObjectClassProp fakeTestNameOCP = _SchemaModTrnsctn.createObjectClassProp( _fakeTestOC, new CswNbtWcfMetaDataModel.ObjectClassProp()
                {
                    PropName = "Name",
                    FieldType = CswEnumNbtFieldType.Text
                } );
            Assert.IsNotNull( fakeTestNameOCP, "fakeTestNameOCP was null" );

            // new nodetype
            CswNbtMetaDataNodeType fakeTestNT = _SchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( _fakeTestOC )
                {
                    NodeTypeName = fakeNodeTypeName,
                    Category = "fake cat"
                } );
            Assert.IsNotNull( fakeTestNT, "fakeTestNT was null" );
            Assert.IsNotNull( fakeTestNT.DesignNode, "fakeTestNT.DesignNode was null" );
            Assert.IsNotNull( fakeTestNT.getNodeTypeProp( "Name" ), "fakeTestNT.getNodeTypeProp( Name ) was null" );
            Assert.IsNotNull( fakeTestNT.getNodeTypeProp( "Name" ).DesignNode, "fakeTestNT.getNodeTypeProp( Name ).DesignNode was null" );

            // new nodetype tab
            CswNbtMetaDataNodeTypeTab fakeTabNTT = _SchemaModTrnsctn.MetaData.makeNewTab( fakeTestNT, "faketab" );
            Assert.IsNotNull( fakeTabNTT, "fakeTabNTT was null" );
            Assert.IsNotNull( fakeTabNTT.DesignNode, "fakeTabNTT.DesignNode was null" );

            // new nodetype prop
            CswNbtMetaDataNodeTypeProp fakeTestStatusNTP = _SchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( fakeTestNT, _SchemaModTrnsctn.MetaData.getFieldType( CswEnumNbtFieldType.List ), "Status" ) );
            Assert.IsNotNull( fakeTestStatusNTP, "fakeTestStatusNTP was null" );
            Assert.IsNotNull( fakeTestStatusNTP.DesignNode, "fakeTestStatusNTP.DesignNode was null" );

            // another new object class prop
            CswNbtMetaDataObjectClassProp fakeTestNumOCP = _SchemaModTrnsctn.createObjectClassProp( _fakeTestOC, new CswNbtWcfMetaDataModel.ObjectClassProp()
                {
                    PropName = "Num",
                    FieldType = CswEnumNbtFieldType.Number
                } );
            Assert.IsNotNull( fakeTestNumOCP, "fakeTestNumOCP was null" );

            // makemissingnodetypeprops
            _SchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();
            Assert.IsNotNull( fakeTestNT.getNodeTypeProp( "Num" ), "fakeTestNT was null" );
            Assert.IsNotNull( fakeTestNT.getNodeTypeProp( "Num" ).DesignNode, "fakeTestNT.DesignNode was null" );

        } // testMetaData()

    } // class CswNbtMetaDataTest
} // namespace
