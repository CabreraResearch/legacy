using System;
using System.Collections.Generic;
using ChemSW;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.Config;
using ChemSW.Nbt.csw.Conversion;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NbtUnitConversion.Test
{
    [TestClass]
    public class CswNbtUnitConversionTest
    {
        #region Setup and Teardown

        private CswNbtResources _CswNbtResources = null;
        private ICswDbCfgInfo _CswDbCfgInfoNbt = null;
        private List<CswPrimaryKey> TestNodeIds = new List<CswPrimaryKey>();
        private string UserName = "TestUser";

        [TestInitialize()]
        public void MyTestInitialize()
        {
            _CswNbtResources = CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, SetupMode.NbtExe, true, false );
            _CswDbCfgInfoNbt = new CswDbCfgInfoNbt( SetupMode.NbtExe, IsMobile: false );
            _CswNbtResources.InitCurrentUser = InitUser;
            _CswNbtResources.AccessId = _CswDbCfgInfoNbt.MasterAccessId;
        }

        public ICswUser InitUser( ICswResources Resources )
        {
            return new CswNbtSystemUser( Resources, UserName );
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            foreach( CswPrimaryKey NodeId in TestNodeIds )
            {
                CswNbtNode Node = _CswNbtResources.Nodes.GetNode( NodeId );
                Node.delete();
            }
        }

        #endregion

        #region Test Data

        private int _getNodeTypeId( string NodeTypeName )
        {
            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeName );
            return NodeType.NodeTypeId;
        }

        private CswNbtNode _createUnitOfMeasureNode( string NodeTypeName, string Name, double ConversionFactorBase, int ConversionFactorExponent, Tristate Fractional )
        {
            CswNbtNode UnitOfMeasureNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( NodeTypeName + " Unit" ), CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            CswNbtObjClassUnitOfMeasure NodeAsUnitOfMeasure = UnitOfMeasureNode;
            NodeAsUnitOfMeasure.Name.Text = Name + "Test";
            if( ConversionFactorBase != Int32.MinValue )
                NodeAsUnitOfMeasure.ConversionFactor.Base = ConversionFactorBase;
            if( ConversionFactorExponent != Int32.MinValue )
                NodeAsUnitOfMeasure.ConversionFactor.Exponent = ConversionFactorExponent;
            NodeAsUnitOfMeasure.Fractional.Checked = Fractional;
            NodeAsUnitOfMeasure.postChanges( true );

            TestNodeIds.Add( NodeAsUnitOfMeasure.NodeId );

            return UnitOfMeasureNode;
        }

        private CswNbtNode _createMaterialNode( string NodeTypeName, string State, double SpecificGravity )
        {
            CswNbtNode MaterialNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( NodeTypeName ), CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            CswNbtObjClassMaterial NodeAsMaterial = MaterialNode;
            NodeAsMaterial.PhysicalState.Value = State;
            NodeAsMaterial.postChanges( true );

            TestNodeIds.Add( NodeAsMaterial.NodeId );

            return MaterialNode;
        }

        //private CswNbtNode _createMaterialNode( string NodeTypeName, string State, double SpecificGravityBase, int SpecificGravityExponent )
        //{
        //    CswNbtNode MaterialNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( _getNodeTypeId( NodeTypeName ), CswNbtNodeCollection.MakeNodeOperation.WriteNode );
        //    CswNbtObjClassMaterial NodeAsMaterial = MaterialNode;
        //    if( SpecificGravityBase != Int32.MinValue )
        //        NodeAsMaterial.SpecificGravity.Base = SpecificGravityBase;
        //    if( SpecificGravityExponent != Int32.MinValue )
        //        NodeAsMaterial.SpecificGravity.Exponent = SpecificGravityExponent;
        //    NodeAsMaterial.PhysicalState.Value = State;
        //    NodeAsMaterial.postChanges( true );

        //    TestNodeIds.Add( NodeAsMaterial.NodeId );

        //    return MaterialNode;
        //}

        #endregion

        /// <summary>
        /// See W1005
        /// </summary>
        [TestMethod]
        public void convertUnitTestWikiExample()
        {
            Double ValueToConvert = 3;
            CswNbtNode OuncesNode = _createUnitOfMeasureNode( "Weight", "ounces", 3.527396, 1, Tristate.True );
            CswNbtNode MilligramNode = _createUnitOfMeasureNode( "Weight", "mg", 1.0, 6, Tristate.True );
            Double Expected = 85048.574076;
            //Rounding to sixth significant digit since the numbers are stored in the DB as number (15,6)
            Double Actual = Math.Round( CswNbtUnitConversion.convertUnit( ValueToConvert, OuncesNode, MilligramNode ), 6 );
            Assert.AreEqual( Expected, Actual, "Conversion applied incorrectly." );
        }

        /// <summary>
        /// Given a numeric value and two UnitOfMeasure Nodes of the Same Unit Type, when unit conversion is applied, 
        /// the returning number should be the product of the given value and the quotient of the old and new conversion factors.
        /// </summary>
        [TestMethod]
        public void convertUnitTestStaticSameUnitType()
        {
            Double ValueToConvert = 4;
            CswNbtNode LiterNode = _createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode MilliliterNode = _createUnitOfMeasureNode( "Volume", "mL", 1.0, 3, Tristate.True );
            Double Expected = 4000;
            Double Actual = CswNbtUnitConversion.convertUnit( ValueToConvert, LiterNode, MilliliterNode );
            Assert.AreEqual( Expected, Actual, "Conversion applied incorrectly." );
        }

        /// <summary>
        /// Given a numeric value, two UnitOfMeasure Nodes of types Volume and Weight, and a MaterialNode, when unit conversion is applied, 
        /// the returning number should be the product of the given value and the interconversion of the old and new conversion factors 
        /// with respect to the material's specific gravity.
        /// </summary>
        [TestMethod]
        public void convertUnitTestStaticVolumeToWeightUnitTypes()
        {
            Double ValueToConvert = 4;
            CswNbtNode LiterNode = _createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode KilogramNode = _createUnitOfMeasureNode( "Weight", "kg", 1.0, 0, Tristate.True );
            //CswNbtNode ChemicalNode = _createMaterialNode( "Chemical", "Liquid", 1, -1 );
            CswNbtNode ChemicalNode = _createMaterialNode( "Chemical", "Liquid", 1 );
            Double Expected = 0.4;
            Double Actual = CswNbtUnitConversion.convertUnit( ValueToConvert, LiterNode, KilogramNode, ChemicalNode );
            Assert.AreEqual( Expected, Actual, "Conversion applied incorrectly." );
        }

        /// <summary>
        /// Given a numeric value, two UnitOfMeasure Nodes of types Weight and Volume, and a MaterialNode, when unit conversion is applied, 
        /// the returning number should be the product of the given value and the interconversion of the old and new conversion factors 
        /// with respect to the material's specific gravity.
        /// </summary>
        [TestMethod]
        public void convertUnitTestStaticWeightToVolumeUnitTypes()
        {
            Double ValueToConvert = 4;
            CswNbtNode KilogramNode = _createUnitOfMeasureNode( "Weight", "kg", 1.0, 0, Tristate.True );
            CswNbtNode LiterNode = _createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            //CswNbtNode ChemicalNode = _createMaterialNode( "Chemical", "Liquid", 1, -1 );
            CswNbtNode ChemicalNode = _createMaterialNode( "Chemical", "Liquid", 1 );
            Double Expected = 40;
            Double Actual = CswNbtUnitConversion.convertUnit( ValueToConvert, KilogramNode, LiterNode, ChemicalNode );
            Assert.AreEqual( Expected, Actual, "Conversion applied incorrectly." );
        }

        /// <summary>
        /// Given a numeric value, two UnitOfMeasure Nodes of types Weight and Volume, with no MaterialNode, when unit conversion is attempted, 
        /// an exception is thrown.
        /// </summary>
        [TestMethod]
        public void convertUnitTestStaticNullMaterialUnitTypes()
        {
            Double ValueToConvert = 4;
            CswNbtNode KilogramNode = _createUnitOfMeasureNode( "Weight", "kg", 1.0, 0, Tristate.True );
            CswNbtNode LiterNode = _createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode ChemicalNode = null;
            Double Expected = 40;
            Double Actual = -1;
            try
            {
                Actual = CswNbtUnitConversion.convertUnit( ValueToConvert, KilogramNode, LiterNode, ChemicalNode );
                Assert.Fail( "Exception should have been thrown." );
            }
            catch( Exception e )
            {
                Assert.AreNotEqual( Expected, Actual, "Conversion should not have been applied here! " + e.Message );
            }
        }

        /// <summary>
        /// Given a numeric value and two UnitOfMeasure Nodes with incompatible conversion types, when unit conversion is attempted, 
        /// an exception is thrown.
        /// </summary>
        [TestMethod]
        public void convertUnitTestStaticNotSupportedUnitTypes()
        {
            Double ValueToConvert = 4;
            CswNbtNode LiterNode = _createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode DaysNode = _createUnitOfMeasureNode( "Time", "Days", 1.0, 0, Tristate.True );
            Double Expected = 4000;
            Double Actual = -1;
            try
            {
                Actual = CswNbtUnitConversion.convertUnit( ValueToConvert, LiterNode, DaysNode );
                Assert.Fail( "Exception should have been thrown." );
            }
            catch( Exception e )
            {
                Assert.AreNotEqual( Expected, Actual, "Conversion should not have been applied here! " + e.Message );
            }
        }

        /// <summary>
        /// Given a numeric value and two conversion factors, when unit conversion is applied, 
        /// the returning number should be the product of the given value and the quotient of the old and new conversion factors.
        /// </summary>
        [TestMethod]
        public void applyUnitConversionTest()
        {
            Double ValueToConvert = 4;
            Double LiterConversionFactor = 1;
            Double MilliliterConversionFactor = 1000;
            Double Expected = 4000;
            Double Actual = CswNbtUnitConversion.applyUnitConversion( ValueToConvert, LiterConversionFactor, MilliliterConversionFactor );
            Assert.AreEqual( Expected, Actual, "Conversion applied incorrectly." );
        }

        /// <summary>
        /// Given a numeric value and two UnitOfMeasure Nodes of the Same Unit Type, when unit conversion is applied, 
        /// the returning number should be the product of the given value and the quotient of the old and new conversion factors.
        /// </summary>
        [TestMethod]
        public void convertUnitTestSameUnitType()
        {
            Double ValueToConvert = 4;
            CswNbtNode LiterNode = _createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode MilliliterNode = _createUnitOfMeasureNode( "Volume", "mL", 1.0, 3, Tristate.True );
            Double Expected = 4000;

            CswNbtUnitConversion ConversionObj = new CswNbtUnitConversion( _CswNbtResources, LiterNode.NodeId, MilliliterNode.NodeId );

            Double Actual = ConversionObj.convertUnit( ValueToConvert );
            Assert.AreEqual( Expected, Actual, "Conversion applied incorrectly." );
        }

        /// <summary>
        /// Given a numeric value, two UnitOfMeasure Nodes of types Volume and Weight, and a MaterialNode, when unit conversion is applied, 
        /// the returning number should be the product of the given value and the interconversion of the old and new conversion factors 
        /// with respect to the material's specific gravity.
        /// </summary>
        [TestMethod]
        public void convertUnitTestVolumeToWeightUnitTypes()
        {
            Double ValueToConvert = 4;
            CswNbtNode LiterNode = _createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode KilogramNode = _createUnitOfMeasureNode( "Weight", "kg", 1.0, 0, Tristate.True );
            //CswNbtNode ChemicalNode = _createMaterialNode( "Chemical", "Liquid", 1, -1 );
            CswNbtNode ChemicalNode = _createMaterialNode( "Chemical", "Liquid", 1 );
            Double Expected = 0.4;

            CswNbtUnitConversion ConversionObj = new CswNbtUnitConversion( _CswNbtResources, LiterNode.NodeId, KilogramNode.NodeId, ChemicalNode.NodeId );

            Double Actual = ConversionObj.convertUnit( ValueToConvert );
            Assert.AreEqual( Expected, Actual, "Conversion applied incorrectly." );
        }

        /// <summary>
        /// Given a numeric value, two UnitOfMeasure Nodes of types Weight and Volume, and a MaterialNode, when unit conversion is applied, 
        /// the returning number should be the product of the given value and the interconversion of the old and new conversion factors 
        /// with respect to the material's specific gravity.
        /// </summary>
        [TestMethod]
        public void convertUnitTestWeightToVolumeUnitTypes()
        {
            Double ValueToConvert = 4;
            CswNbtNode KilogramNode = _createUnitOfMeasureNode( "Weight", "kg", 1.0, 0, Tristate.True );
            CswNbtNode LiterNode = _createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            //CswNbtNode ChemicalNode = _createMaterialNode( "Chemical", "Liquid", 1, -1 );
            CswNbtNode ChemicalNode = _createMaterialNode( "Chemical", "Liquid", 1 );
            Double Expected = 40;

            CswNbtUnitConversion ConversionObj = new CswNbtUnitConversion( _CswNbtResources, KilogramNode.NodeId, LiterNode.NodeId, ChemicalNode.NodeId );

            Double Actual = ConversionObj.convertUnit( ValueToConvert );
            Assert.AreEqual( Expected, Actual, "Conversion applied incorrectly." );
        }
    }
}
