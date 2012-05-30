using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.csw.Conversion;
using ChemSW.Nbt.TreeEvents;
using ChemSW.Nbt.Config;
using ChemSW.Nbt;
using ChemSW.Nbt.Actions;
using ChemSW.Config;
using ChemSW.RscAdo;
using ChemSW.Nbt.Security;
using ChemSW;
using ChemSW.Security;

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
            _CswNbtResources = CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, SetupMode.NbtWeb, true, false );
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

        private CswNbtNode _createUnitOfMeasureNode( int NodeTypeId, string Name, double ConversionFactorBase, int ConversionFactorExponent, Tristate Fractional )
        {
            CswNbtNode UnitOfMeasureNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            CswNbtObjClassUnitOfMeasure NodeAsUnitOfMeasure = _CswNbtResources.Nodes[UnitOfMeasureNode.NodeId];
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

        private CswNbtObjClassUnitOfMeasure _createUnitOfMeasureNode( string NodeTypeName, string Name, double ConversionFactorBase, int ConversionFactorExponent, Tristate Fractional )
        {
            CswNbtNode GenericNode = _createUnitOfMeasureNode( _getNodeTypeId( NodeTypeName ), Name, ConversionFactorBase, ConversionFactorExponent, Fractional );
            CswNbtObjClassUnitOfMeasure UnitOfMeasureNode = _CswNbtResources.Nodes[GenericNode.NodeId];
            return UnitOfMeasureNode;
        }

        private CswNbtNode _createMaterialNode( int NodeTypeId, string State, double SpecificGravityBase, int SpecificGravityExponent )
        {
            CswNbtNode MaterialNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            CswNbtObjClassMaterial NodeAsMaterial = _CswNbtResources.Nodes[MaterialNode.NodeId];
            if( SpecificGravityBase != Int32.MinValue )
                NodeAsMaterial.SpecificGravity.Base = SpecificGravityBase;
            if( SpecificGravityExponent != Int32.MinValue )
                NodeAsMaterial.SpecificGravity.Exponent = SpecificGravityExponent;
            NodeAsMaterial.PhysicalState.Value = State;
            NodeAsMaterial.postChanges( true );

            TestNodeIds.Add( NodeAsMaterial.NodeId );

            return MaterialNode;
        }

        private CswNbtObjClassMaterial _createMaterialNode( string NodeTypeName, string State, double SpecificGravityBase, int SpecificGravityExponent )
        {
            CswNbtNode GenericNode = _createMaterialNode( _getNodeTypeId( NodeTypeName ), State, SpecificGravityBase, SpecificGravityExponent );
            CswNbtObjClassMaterial MaterialNode = _CswNbtResources.Nodes[GenericNode.NodeId];
            return MaterialNode;
        }

        #endregion

        /// <summary>
        /// See W1005
        /// </summary>
        [TestMethod]
        public void convertUnitTestWikiExample()
        {
            Double ValueToConvert = 3;
            CswNbtNode OuncesNode = _createUnitOfMeasureNode( _getNodeTypeId( "Weight" ), "ounces", 2.83495231, -2, Tristate.True );
            CswNbtNode MilligramNode = _createUnitOfMeasureNode( _getNodeTypeId( "Weight" ), "mg", 1.0, -6, Tristate.True );
            Double Expected = 85048.5693;
            Double Actual = CswNbtUnitConversion.convertUnit( _CswNbtResources, ValueToConvert, OuncesNode, MilligramNode );
            Assert.AreEqual( Expected, Actual, "Conversion applied incorrectly." );
        }

        /// <summary>
        /// Given a numeric value and two CswNbtNodes (assumed to be of type UnitOfMeasure), when unit conversion is applied, 
        /// the returning number should be the product of the given value and the quotient of the old and new conversion factors.
        /// </summary>
        [TestMethod]
        public void convertUnitTestGenericNodes()
        {
            Double ValueToConvert = 4;
            CswNbtNode LiterNode = _createUnitOfMeasureNode( _getNodeTypeId( "Volume" ), "Liters", 1.0, 0, Tristate.True );
            CswNbtNode MilliliterNode = _createUnitOfMeasureNode( _getNodeTypeId( "Volume" ), "mL", 1.0, -3, Tristate.True );
            Double Expected = 4000;
            Double Actual = CswNbtUnitConversion.convertUnit( _CswNbtResources, ValueToConvert, LiterNode, MilliliterNode );
            Assert.AreEqual( Expected, Actual, "Conversion applied incorrectly." );
        }

        /// <summary>
        /// Given a numeric value and three CswNbtNodes (two assumed to be of type UnitOfMeasure, the other Material), when unit conversion is applied, 
        /// the returning number should be the product of the given value and the interconversion of the old and new conversion factors 
        /// with respect to the material's specific gravity.
        /// </summary>
        [TestMethod]
        public void convertUnitTestGenericNodesDifferentUnitTypes()
        {
            Double ValueToConvert = 4;
            CswNbtNode LiterNode = _createUnitOfMeasureNode( _getNodeTypeId( "Volume" ), "Liters", 1.0, 0, Tristate.True );
            CswNbtNode KilogramNode = _createUnitOfMeasureNode( _getNodeTypeId( "Weight" ), "kg", 1.0, 0, Tristate.True );
            CswNbtNode ChemicalNode = _createMaterialNode( _getNodeTypeId( "Chemical" ), "Liquid", 1, -1 );
            Double Expected = 0.4;
            Double Actual = CswNbtUnitConversion.convertUnit( _CswNbtResources, ValueToConvert, LiterNode, KilogramNode, ChemicalNode );
            Assert.AreEqual( Expected, Actual, "Conversion applied incorrectly." );
        }

        /// <summary>
        /// Given a numeric value and two UnitOfMeasure Nodes, when unit conversion is applied, 
        /// the returning number should be the product of the given value and the quotient of the old and new conversion factors.
        /// </summary>
        [TestMethod]
        public void convertUnitTestEqualUnitTypes()
        {
            Double ValueToConvert = 4;
            CswNbtObjClassUnitOfMeasure LiterNode = _createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtObjClassUnitOfMeasure MilliliterNode = _createUnitOfMeasureNode( "Volume", "mL", 1.0, -3, Tristate.True );
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
        public void convertUnitTestVolumeToWeightUnitTypes()
        {
            Double ValueToConvert = 4;
            CswNbtObjClassUnitOfMeasure LiterNode = _createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtObjClassUnitOfMeasure KilogramNode = _createUnitOfMeasureNode( "Weight", "kg", 1.0, 0, Tristate.True );
            CswNbtObjClassMaterial ChemicalNode = _createMaterialNode( "Chemical", "Liquid", 1, -1 );
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
        public void convertUnitTestWeightToVolumeUnitTypes()
        {
            Double ValueToConvert = 4;
            CswNbtObjClassUnitOfMeasure KilogramNode = _createUnitOfMeasureNode( "Weight", "kg", 1.0, 0, Tristate.True );
            CswNbtObjClassUnitOfMeasure LiterNode = _createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtObjClassMaterial ChemicalNode = _createMaterialNode( "Chemical", "Liquid", 1, -1 );
            Double Expected = 40;
            Double Actual = CswNbtUnitConversion.convertUnit( ValueToConvert, KilogramNode, LiterNode, ChemicalNode );
            Assert.AreEqual( Expected, Actual, "Conversion applied incorrectly." );
        }

        /// <summary>
        /// Given a numeric value, two UnitOfMeasure Nodes of types Weight and Volume, and a MaterialNode, when unit conversion is applied, 
        /// the returning number should be the product of the given value and the interconversion of the old and new conversion factors 
        /// with respect to the material's specific gravity.
        /// </summary>
        [TestMethod]
        public void convertUnitTestNullMaterialUnitTypes()
        {
            Double ValueToConvert = 4;
            CswNbtObjClassUnitOfMeasure KilogramNode = _createUnitOfMeasureNode( "Weight", "kg", 1.0, 0, Tristate.True );
            CswNbtObjClassUnitOfMeasure LiterNode = _createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtObjClassMaterial ChemicalNode = null;
            Double Expected = 40;
            Double Actual = CswNbtUnitConversion.convertUnit( ValueToConvert, KilogramNode, LiterNode, ChemicalNode );
            Assert.AreNotEqual( Expected, Actual, "Conversion should not have been applied here!" );
            Assert.AreEqual( ValueToConvert, Actual, "ValueToConvert has unexpectantly changed." );
        }

        /// <summary>
        /// Given a numeric value and two UnitOfMeasure Nodes, when unit conversion is applied, 
        /// the returning number should be the product of the given value and the quotient of the old and new conversion factors.
        /// </summary>
        [TestMethod]
        public void convertUnitTestNotSupportedUnitTypes()
        {
            Double ValueToConvert = 4;
            CswNbtObjClassUnitOfMeasure LiterNode = _createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtObjClassUnitOfMeasure DaysNode = _createUnitOfMeasureNode( "Time", "Days", 1.0, 0, Tristate.True );
            Double Expected = 4000;
            Double Actual = CswNbtUnitConversion.convertUnit( ValueToConvert, LiterNode, DaysNode );
            Assert.AreNotEqual( Expected, Actual, "Conversion should not have been applied here!" );
            Assert.AreEqual( ValueToConvert, Actual, "ValueToConvert has unexpectantly changed." );
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
            Double MilliliterConversionFactor = .001;
            Double Expected = 4000;
            Double Actual = CswNbtUnitConversion.applyUnitConversion( ValueToConvert, LiterConversionFactor, MilliliterConversionFactor );
            Assert.AreEqual( Expected, Actual, "Conversion applied incorrectly." );
        }
    }
}
