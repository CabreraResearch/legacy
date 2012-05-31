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
        /// Takes a numeric value and interconverts it between different Unit Types using the given UnitOfMeasure and Material nodes.
        /// If unit conversion cannot be applied, an error is thrown.
        /// </summary>
        public static Double convertUnit( Double ValueToConvert, CswNbtNode OldUnitOfMeasureNode, CswNbtNode NewUnitOfMeasureNode, CswNbtNode MaterialNodeIn = null )
        {
            Double ConvertedUnit = ValueToConvert;

            if( OldUnitOfMeasureNode != null && NewUnitOfMeasureNode != null )
            {
                CswNbtObjClassUnitOfMeasure OldUnitNode = OldUnitOfMeasureNode;
                CswNbtObjClassUnitOfMeasure NewUnitNode = NewUnitOfMeasureNode;
                Double OldConversionFactor = _getScientificValue( OldUnitNode.ConversionFactor );
                Double NewConversionFactor = _getScientificValue( NewUnitNode.ConversionFactor );

                CswNbtUnitConversionEnums.UnitTypeRelationship UnitRelationship = _getUnitTypeRelationship( OldUnitNode.NodeType, NewUnitNode.NodeType );

                if( UnitRelationship == CswNbtUnitConversionEnums.UnitTypeRelationship.Same )
                {
                    ConvertedUnit = applyUnitConversion( ValueToConvert, OldConversionFactor, NewConversionFactor );
                }
                else if( UnitRelationship != CswNbtUnitConversionEnums.UnitTypeRelationship.NotSupported && MaterialNodeIn != null )
                {
                    CswNbtObjClassMaterial MaterialNode = MaterialNodeIn;
                    Double SpecificGravity = _getScientificValue( MaterialNode.SpecificGravity );

                    if( SpecificGravity != 0 && SpecificGravity != Double.NaN )
                    {
                        //NodeType-specific logic (Operator logic defined in W1005)
                        if( UnitRelationship == CswNbtUnitConversionEnums.UnitTypeRelationship.WeightToVolume )
                        {
                            ConvertedUnit = applyUnitConversion( ValueToConvert, OldConversionFactor * SpecificGravity, NewConversionFactor );
                        }
                        else if( UnitRelationship == CswNbtUnitConversionEnums.UnitTypeRelationship.VolumeToWeight )
                        {
                            ConvertedUnit = applyUnitConversion( ValueToConvert, OldConversionFactor / SpecificGravity, NewConversionFactor );
                        }
                    }
                    else
                    {
                        throw ( new DivideByZeroException( "Specific Gravity must be defined as a positive number." ) );
                    }
                }
                else
                {
                    throw ( new Exception( "Conversion failed: Unsupported unit types." ) );
                }
            }
            return ConvertedUnit;
        }

        /// <summary>
        /// Takes a numeric value and converts it from one Unit of Measurement into another using the given Conversion Factor values
        /// If unit conversion cannot be applied, an error is thrown.
        /// </summary>
        public static double applyUnitConversion( Double ValueToConvert, Double OldConversionFactor, Double NewConversionFactor )
        {
            Double NewUnitValue;
            if( ( OldConversionFactor != 0 && OldConversionFactor != Double.NaN ) && ( NewConversionFactor != 0 && NewConversionFactor != Double.NaN ) )
            {
                //Operator logic defined in W1005
                NewUnitValue = ValueToConvert / OldConversionFactor * NewConversionFactor;
            }
            else
            {
                throw ( new DivideByZeroException( "Conversion Factor must be defined as a positive number." ) );
            }
            if( NewUnitValue == Double.NaN || NewUnitValue < 0 )
            {
                throw ( new Exception( "Conversion failed: Insufficient data provided." ) );
            }
            return NewUnitValue;
        }

        /// <summary>
        /// Evaluates the numeric value of a Node's Scientific Property.
        /// </summary>
        private static Double _getScientificValue( CswNbtNodePropScientific ScientificNodeProp )
        {
            return ScientificNodeProp.Base * Math.Pow( 10, ScientificNodeProp.Exponent );
        }

        /// <summary>
        /// Identifies the UnitType relationship between two UnitOfMeasure Nodes.
        /// </summary>
        private static CswNbtUnitConversionEnums.UnitTypeRelationship _getUnitTypeRelationship( CswNbtMetaDataNodeType OldNodeType, CswNbtMetaDataNodeType NewNodeType )
        {
            CswNbtUnitConversionEnums.UnitTypeRelationship UnitRelationship = CswNbtUnitConversionEnums.UnitTypeRelationship.Unknown;

            if( OldNodeType.NodeTypeName == NewNodeType.NodeTypeName )
            {
                UnitRelationship = CswNbtUnitConversionEnums.UnitTypeRelationship.Same;
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
