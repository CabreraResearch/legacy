﻿using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using NUnit.Framework;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Test.PropTypes
{
    [TestFixture]
    public class CswNbtNodePropMultiListTest
    {
        #region Setup and Teardown

        private TestData TestData;

        [SetUp]
        public void MyTestInitialize()
        {
            TestData = new TestData{ FinalizeNodes = true };
        }

        [TearDown]
        public void MyTestCleanup()
        {
            TestData.Destroy();
        }

        #endregion

        /// <summary>
        /// Given a Chemical Node with one PPE option selected, assert that its ReadOnly value is formatted correctly.
        /// </summary>
        [Test]
        public void setReadOnlyValuesTestOneValue()
        {
            TestData.SetPPENodeTypeProp( "Goggles,Gloves,Clothing,Fume Hood" );
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( PPE: "Goggles" );
            CswNbtMetaDataNodeTypeProp PPENTP = TestData.CswNbtResources.MetaData.getNodeTypeProp( ChemicalNode.NodeTypeId, "PPE" );
            String Expected = "Goggles";
            JObject SerializedPPE = new JObject();

            ChemicalNode.Properties[PPENTP].AsMultiList.ToJSON( SerializedPPE );
            Assert.AreEqual( Expected, SerializedPPE["readonlyless"].ToString(), "Readonly serialization does not match." );
        }

        /// <summary>
        /// Given a Chemical Node with two PPE options selected with comma as the default delimiter, assert that its ReadOnly value is formatted correctly.
        /// </summary>
        [Test]
        public void setReadOnlyValuesTestTwoValuesCommaDelimited()
        {
            TestData.SetPPENodeTypeProp( "Goggles,Gloves,Clothing,Fume Hood" );
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( PPE: "Goggles,Clothing" );
            CswNbtMetaDataNodeTypeProp PPENTP = TestData.CswNbtResources.MetaData.getNodeTypeProp( ChemicalNode.NodeTypeId, "PPE" );
            String Expected = "Goggles, Clothing";
            JObject SerializedPPE = new JObject();

            ChemicalNode.Properties[PPENTP].AsMultiList.ToJSON( SerializedPPE );
            Assert.AreEqual( Expected, SerializedPPE["readonlyless"].ToString(), "Readonly serialization does not match." );
        }

        /// <summary>
        /// Given a Chemical Node with two PPE options selected with HTML's newline as the default delimiter, assert that its ReadOnly value is formatted correctly.
        /// </summary>
        [Test]
        public void setReadOnlyValuesTestTwoValuesNewlineDelimiter()
        {
            TestData.SetPPENodeTypeProp( "Goggles,Gloves,Clothing,Fume Hood", "<br/>" );
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( PPE: "Goggles,Clothing" );
            CswNbtMetaDataNodeTypeProp PPENTP = TestData.CswNbtResources.MetaData.getNodeTypeProp( ChemicalNode.NodeTypeId, "PPE" );
            String Expected = "Goggles<br/> Clothing";
            JObject SerializedPPE = new JObject();

            ChemicalNode.Properties[PPENTP].AsMultiList.ToJSON( SerializedPPE );
            Assert.AreEqual( Expected, SerializedPPE["readonlyless"].ToString(), "Readonly serialization does not match." );
        }

        /// <summary>
        /// Given a Chemical Node with four PPE options selected with a HideThreshold of 2, 
        /// assert that its ReadOnly value only shows the first two values in readonlyless,
        /// and assert that the remaining values are listed in readonlymore.
        /// </summary>
        [Test]
        public void setReadOnlyValuesTestFourValuesExceedingHideThreshold()
        {
            TestData.SetPPENodeTypeProp( "Goggles,Gloves,Clothing,Fume Hood", ",", 2 );
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( PPE: "Goggles,Gloves,Clothing,Fume Hood" );
            CswNbtMetaDataNodeTypeProp PPENTP = TestData.CswNbtResources.MetaData.getNodeTypeProp( ChemicalNode.NodeTypeId, "PPE" );
            String ExpectedLess = "Goggles, Gloves";
            String ExpectedMore = "Clothing, Fume Hood";
            JObject SerializedPPE = new JObject();
            ChemicalNode.Properties[PPENTP].AsMultiList.ToJSON( SerializedPPE );
            Assert.AreEqual( ExpectedLess, SerializedPPE["readonlyless"].ToString(), "Readonly serialization does not match." );
            Assert.AreEqual( ExpectedMore, SerializedPPE["readonlymore"].ToString(), "Readonly serialization does not match." );
        }

        /// <summary>
        /// Given a Chemical Node with four PPE options selected under two unique subjects selected with a HideThreshold of 2,
        /// assert that all values uner each subject are concatenated to a single value,
        /// and assert that the values do not exceed the HideThreshold.
        /// </summary>
        [Test]
        public void setReadOnlyValuesTestCollapsedSubjects()
        {
            TestData.SetPPENodeTypeProp( "Hazard Gear: Goggles,Hazard Gear: Gloves,Hazard Wear: Clothing,Hazard Wear: Fume Hood", "<br/>", 2 );
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( PPE: "Hazard Gear: Goggles,Hazard Gear: Gloves,Hazard Wear: Clothing,Hazard Wear: Fume Hood" );
            CswNbtMetaDataNodeTypeProp PPENTP = TestData.CswNbtResources.MetaData.getNodeTypeProp( ChemicalNode.NodeTypeId, "PPE" );
            String Expected = "Hazard Gear: Goggles, Gloves<br/> Hazard Wear: Clothing, Fume Hood";
            JObject SerializedPPE = new JObject();
            ChemicalNode.Properties[PPENTP].AsMultiList.ToJSON( SerializedPPE );
            Assert.AreEqual( Expected, SerializedPPE["readonlyless"].ToString(), "Readonly serialization does not match." );
            Assert.IsTrue( String.IsNullOrEmpty( SerializedPPE["readonlymore"].ToString() ), "Readonly serialization does not match." );
        }

        /// <summary>
        /// Given a Chemical Node with four PPE options selected under two unique subjects selected with a HideThreshold of 1,
        /// assert that all values uner each subject are concatenated to a single value,
        /// and assert that the two resulting rows are split into the readonlyless and readonlymore values.
        /// </summary>
        [Test]
        public void setReadOnlyValuesTestCollapsedSubjectsExceedingHideThreshold()
        {
            TestData.SetPPENodeTypeProp( "Hazard Gear: Goggles,Hazard Gear: Gloves,Hazard Wear: Clothing,Hazard Wear: Fume Hood", "<br/>", 1 );
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( PPE: "Hazard Gear: Goggles,Hazard Gear: Gloves,Hazard Wear: Clothing,Hazard Wear: Fume Hood" );
            CswNbtMetaDataNodeTypeProp PPENTP = TestData.CswNbtResources.MetaData.getNodeTypeProp( ChemicalNode.NodeTypeId, "PPE" );
            String ExpectedLess = "Hazard Gear: Goggles, Gloves";
            String ExpectedMore = "Hazard Wear: Clothing, Fume Hood";
            JObject SerializedPPE = new JObject();
            ChemicalNode.Properties[PPENTP].AsMultiList.ToJSON( SerializedPPE );
            Assert.AreEqual( ExpectedLess, SerializedPPE["readonlyless"].ToString(), "Readonly serialization does not match." );
            Assert.AreEqual( ExpectedMore, SerializedPPE["readonlymore"].ToString(), "Readonly serialization does not match." );
        }
    }
}
