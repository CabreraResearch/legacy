using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.csw.Conversion
{
    public class CswNbtUnitConversion
    {
        /// <summary>
        /// Takes a numeric value and converts it from one Unit of Measurement into another using the Conversion Factors of the given UnitOfMeasure nodes.
        /// If unit conversion cannot be applied, ValueToConvert is returned.
        /// </summary>
        public static Double convertUnit( CswNbtResources CswNbtResources, Double ValueToConvert, CswNbtNode OldUnitOfMeasureNode, CswNbtNode NewUnitOfMeasureNode, CswNbtNode MaterialNodeIn = null )
        {
            Double ConvertedUnit = ValueToConvert;

            if( OldUnitOfMeasureNode != null && NewUnitOfMeasureNode != null )
            {
                CswNbtObjClassUnitOfMeasure OldUnitNode = CswNbtResources.Nodes[OldUnitOfMeasureNode.NodeId];
                CswNbtObjClassUnitOfMeasure NewUnitNode = CswNbtResources.Nodes[NewUnitOfMeasureNode.NodeId];

                if( MaterialNodeIn != null )
                {
                    CswNbtObjClassMaterial MaterialNode = CswNbtResources.Nodes[MaterialNodeIn.NodeId];
                    ConvertedUnit = convertUnit( ValueToConvert, OldUnitNode, NewUnitNode, MaterialNode );
                }
                else
                {
                    ConvertedUnit = convertUnit( ValueToConvert, OldUnitNode, NewUnitNode );
                }
            }
            return ConvertedUnit;
        }

        /// <summary>
        /// Takes a numeric value and interconverts it between different Unit Types using the given UnitOfMeasure and Material nodes.
        /// If unit conversion cannot be applied, ValueToConvert is returned.
        /// </summary>
        public static Double convertUnit( Double ValueToConvert, CswNbtObjClassUnitOfMeasure OldUnitNode, CswNbtObjClassUnitOfMeasure NewUnitNode, CswNbtObjClassMaterial MaterialNode )
        {
            Double ConvertedUnit = ValueToConvert;

            if( OldUnitNode != null && NewUnitNode != null )
            {
                CswNbtUnitConversionEnums.UnitTypeRelationship UnitRelationship = _getUnitTypeRelationship( OldUnitNode.NodeType, NewUnitNode.NodeType );

                if( UnitRelationship == CswNbtUnitConversionEnums.UnitTypeRelationship.Equal )
                {
                    ConvertedUnit = convertUnit( ValueToConvert, OldUnitNode, NewUnitNode );
                }
                else if( UnitRelationship != CswNbtUnitConversionEnums.UnitTypeRelationship.NotSupported && MaterialNode != null )
                {
                    Double OldConversionFactor = _getScientificValue( OldUnitNode.ConversionFactor );
                    Double NewConversionFactor = _getScientificValue( NewUnitNode.ConversionFactor );
                    Double SpecificGravity = _getScientificValue( MaterialNode.SpecificGravity );

                    //NodeType-specific logic
                    if( UnitRelationship == CswNbtUnitConversionEnums.UnitTypeRelationship.WeightToVolume )
                    {
                        ConvertedUnit = applyUnitConversion( ValueToConvert, OldConversionFactor / SpecificGravity, NewConversionFactor );
                    }
                    else if( UnitRelationship == CswNbtUnitConversionEnums.UnitTypeRelationship.VolumeToWeight )
                    {
                        ConvertedUnit = applyUnitConversion( ValueToConvert, OldConversionFactor * SpecificGravity, NewConversionFactor );
                    }
                }
            }
            return ConvertedUnit;
        }

        /// <summary>
        /// Takes a numeric value and converts it from one Unit of Measurement into another using the Conversion Factors of the given UnitOfMeasure nodes.
        /// If unit conversion cannot be applied, ValueToConvert is returned.
        /// </summary>
        public static Double convertUnit( Double ValueToConvert, CswNbtObjClassUnitOfMeasure OldUnitNode, CswNbtObjClassUnitOfMeasure NewUnitNode )
        {
            Double ConvertedUnit = ValueToConvert;

            if( OldUnitNode != null && NewUnitNode != null )
            {
                CswNbtUnitConversionEnums.UnitTypeRelationship UnitRelationship = _getUnitTypeRelationship( OldUnitNode.NodeType, NewUnitNode.NodeType );

                if( UnitRelationship == CswNbtUnitConversionEnums.UnitTypeRelationship.Equal )
                {
                    Double OldConversionFactor = _getScientificValue( OldUnitNode.ConversionFactor );
                    Double NewConversionFactor = _getScientificValue( NewUnitNode.ConversionFactor );
                    ConvertedUnit = applyUnitConversion( ValueToConvert, OldConversionFactor, NewConversionFactor );
                }
            }
            return ConvertedUnit;
        }

        /// <summary>
        /// Takes a numeric value and converts it from one Unit of Measurement into another using the given Conversion Factor values
        /// If unit conversion cannot be applied, ValueToConvert is returned.
        /// </summary>
        public static double applyUnitConversion( Double ValueToConvert, Double OldConversionFactor, Double NewConversionFactor )
        {
            Double NewUnitValue = ValueToConvert * OldConversionFactor / NewConversionFactor;
            return NewUnitValue == Double.NaN ? ValueToConvert : NewUnitValue;
        }

        /// <summary>
        /// Evaluates the numeric value of a Node's Scientific Property.
        /// </summary>
        private static Double _getScientificValue( CswNbtNodePropScientific ScientificNodeProp )
        {
            return _getScientificValue( ScientificNodeProp.Base, ScientificNodeProp.Exponent );
        }

        /// <summary>
        /// Evaluates a number represented in scientific notation.
        /// </summary>
        private static Double _getScientificValue( Double Base, int Exponent )
        {
            return Base * Math.Pow( 10, Exponent );
        }

        /// <summary>
        /// Identifies the UnitType relationship between two UnitOfMeasure Nodes.
        /// </summary>
        private static CswNbtUnitConversionEnums.UnitTypeRelationship _getUnitTypeRelationship( CswNbtMetaDataNodeType OldNodeType, CswNbtMetaDataNodeType NewNodeType )
        {
            CswNbtUnitConversionEnums.UnitTypeRelationship UnitRelationship = CswNbtUnitConversionEnums.UnitTypeRelationship.Unknown;

            if( OldNodeType.NodeTypeName == NewNodeType.NodeTypeName )
            {
                UnitRelationship = CswNbtUnitConversionEnums.UnitTypeRelationship.Equal;
            }
            else if( OldNodeType.NodeTypeName == "Weight" && NewNodeType.NodeTypeName == "Volume" )
            {
                UnitRelationship = CswNbtUnitConversionEnums.UnitTypeRelationship.WeightToVolume;
            }
            else if( OldNodeType.NodeTypeName == "Volume" && NewNodeType.NodeTypeName == "Weight" )
            {
                UnitRelationship = CswNbtUnitConversionEnums.UnitTypeRelationship.VolumeToWeight;
            }
            else
            {
                UnitRelationship = CswNbtUnitConversionEnums.UnitTypeRelationship.NotSupported;
            }

            return UnitRelationship;
        }
    }
}
