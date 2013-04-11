﻿using System;
using ChemSW.Core;
using ChemSW.Nbt.Conversion;
using ChemSW.Nbt.ObjClasses;
using NUnit.Framework;

namespace ChemSW.Nbt.Test.UnitsOfMeasure
{
    [TestFixture]
    public class CswNbtUnitConversionTest
    {
        #region Setup and Teardown

        private TestData TestData;

        [SetUp]
        public void MyTestInitialize()
        {
            TestData = new TestData();
        }

        [TearDown]
        public void MyTestCleanup()
        {
            TestData.Destroy();
        }

        #endregion

        /// <summary>
        /// See W1005
        /// </summary>
        [Test]
        public void convertUnitTestWikiExample()
        {
            Double ValueToConvert = 3;
            CswNbtNode OuncesNode = TestData.Nodes.createUnitOfMeasureNode( "Weight", "ounces", 2.83495231, -2, Tristate.True );
            CswNbtNode MilligramNode = TestData.Nodes.createUnitOfMeasureNode( "Weight", "mg", 1.0, -6, Tristate.True );
            Double Expected = 85048.56;
            //Rounding to sixth significant digit since the numbers are stored in the DB as number (15,6)
            CswNbtUnitConversion ConversionObj = new CswNbtUnitConversion( TestData.CswNbtResources, OuncesNode.NodeId, MilligramNode.NodeId );
            Double Actual = Math.Round( ConversionObj.convertUnit( ValueToConvert ), 6 );
            Assert.AreEqual( Expected, Actual, "Conversion applied incorrectly." );
        }

        /// <summary>
        /// Given a numeric value and two UnitOfMeasure Nodes of the Same Unit Type, when unit conversion is applied, 
        /// the returning number should be the product of the given value and the quotient of the old and new conversion factors.
        /// </summary>
        [Test]
        public void convertUnitTestSameUnitType()
        {
            Double ValueToConvert = 4;
            CswNbtNode LiterNode = TestData.Nodes.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode MilliliterNode = TestData.Nodes.createUnitOfMeasureNode( "Volume", "mL", 1.0, -3, Tristate.True );
            Double Expected = 4000;

            CswNbtUnitConversion ConversionObj = new CswNbtUnitConversion( TestData.CswNbtResources, LiterNode.NodeId, MilliliterNode.NodeId );

            Double Actual = ConversionObj.convertUnit( ValueToConvert );
            Assert.AreEqual( Expected, Actual, "Conversion applied incorrectly." );
        }

        /// <summary>
        /// Given a numeric value, two UnitOfMeasure Nodes of types Volume and Weight, and a MaterialNode, when unit conversion is applied, 
        /// the returning number should be the product of the given value and the interconversion of the old and new conversion factors 
        /// with respect to the material's specific gravity.
        /// </summary>
        [Test]
        public void convertUnitTestVolumeToWeightUnitTypes()
        {
            Double ValueToConvert = 4;
            CswNbtNode LiterNode = TestData.Nodes.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode KilogramNode = TestData.Nodes.createUnitOfMeasureNode( "Weight", "kg", 1.0, 0, Tristate.True );
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( "Chemical", "Liquid", .1 );
            Double Expected = 0.4;

            CswNbtUnitConversion ConversionObj = new CswNbtUnitConversion( TestData.CswNbtResources, LiterNode.NodeId, KilogramNode.NodeId, ChemicalNode.NodeId );

            Double Actual = ConversionObj.convertUnit( ValueToConvert );
            Assert.AreEqual( Expected, Actual, "Conversion applied incorrectly." );
        }

        /// <summary>
        /// Given a numeric value, two UnitOfMeasure Nodes of types Weight and Volume, and a MaterialNode, when unit conversion is applied, 
        /// the returning number should be the product of the given value and the interconversion of the old and new conversion factors 
        /// with respect to the material's specific gravity.
        /// </summary>
        [Test]
        public void convertUnitTestWeightToVolumeUnitTypes()
        {
            Double ValueToConvert = 4;
            CswNbtNode GramNode = TestData.Nodes.createUnitOfMeasureNode( "Weight", "g", 1.0, -3, Tristate.True );
            CswNbtNode LiterNode = TestData.Nodes.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode ChemicalNode = TestData.Nodes.createMaterialNode( "Chemical", "Liquid", .1 );
            Double Expected = 0.04;

            CswNbtUnitConversion ConversionObj = new CswNbtUnitConversion( TestData.CswNbtResources, GramNode.NodeId, LiterNode.NodeId, ChemicalNode.NodeId );

            Double Actual = ConversionObj.convertUnit( ValueToConvert );
            Assert.AreEqual( Expected, Actual, "Conversion applied incorrectly." );
        }


        /// <summary>
        /// Given a null UnitOfMeasure, Unit Conversion cannot be applied.
        /// </summary>
        [Test]
        public void convertUnitTestNullUnitOfMeasure()
        {
            Double ValueToConvert = 4;
            CswNbtNode LiterNode = TestData.Nodes.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtUnitConversion ConversionObj = new CswNbtUnitConversion( TestData.CswNbtResources, null, LiterNode.NodeId );
            double Actual = ConversionObj.convertUnit( ValueToConvert );
            Assert.AreEqual( ValueToConvert, Actual );
        }

        /// <summary>
        /// Given a null Quantity, Unit Conversion cannot be applied.
        /// </summary>
        [Test]
        public void convertUnitTestNullQuantity()
        {
            Double ValueToConvert = Double.NaN;
            CswNbtNode LiterNode = TestData.Nodes.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode MilliliterNode = TestData.Nodes.createUnitOfMeasureNode( "Volume", "mL", 1.0, -3, Tristate.True );
            Double Expected = 0;
            CswNbtUnitConversion ConversionObj = new CswNbtUnitConversion( TestData.CswNbtResources, LiterNode.NodeId, MilliliterNode.NodeId );
            Double Actual = ConversionObj.convertUnit( ValueToConvert );
            Assert.AreEqual( Expected, Actual );
        }

        /// <summary>
        /// Given two null Units of Measure, Unit Conversion cannot be applied.
        /// </summary>
        [Test]
        public void convertUnitTestNullUnitIds()
        {
            double convertedValue = Double.NaN;
            CswNbtUnitConversion ConversionObj = new CswNbtUnitConversion( TestData.CswNbtResources, null, null );
            convertedValue = ConversionObj.convertUnit( 1 );
            Assert.AreEqual( convertedValue, 1 );
        }
    }
}
