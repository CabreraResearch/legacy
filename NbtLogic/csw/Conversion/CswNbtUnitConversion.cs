using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Exceptions;

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
            Double ConvertedValue = ValueToConvert;

            if( OldUnitOfMeasureNode != null && NewUnitOfMeasureNode != null )
            {
                CswNbtObjClassUnitOfMeasure OldUnitNode = OldUnitOfMeasureNode;
                CswNbtObjClassUnitOfMeasure NewUnitNode = NewUnitOfMeasureNode;
                Double OldConversionFactor = _getScientificValue( OldUnitNode.ConversionFactor );
                Double NewConversionFactor = _getScientificValue( NewUnitNode.ConversionFactor );

                CswNbtUnitConversionEnums.UnitTypeRelationship UnitRelationship = _getUnitTypeRelationship( OldUnitNode.NodeType, NewUnitNode.NodeType );

                if( UnitRelationship == CswNbtUnitConversionEnums.UnitTypeRelationship.Same )
                {
                    ConvertedValue = applyUnitConversion( ValueToConvert, OldConversionFactor, NewConversionFactor );
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
                            ConvertedValue = applyUnitConversion( ValueToConvert, OldConversionFactor, NewConversionFactor, 1.0 / SpecificGravity );
                        }
                        else if( UnitRelationship == CswNbtUnitConversionEnums.UnitTypeRelationship.VolumeToWeight )
                        {
                            ConvertedValue = applyUnitConversion( ValueToConvert, OldConversionFactor, NewConversionFactor, SpecificGravity );
                        }
                    }
                    else
                    {
                        throw new CswDniException( ErrorType.Error, "Specific Gravity must be defined as a positive number.", "Specific Gravity must be defined as a positive number." );
                    }
                }
                else
                {
                    throw new CswDniException( ErrorType.Error, "Conversion failed: Unable to apply unit conversion between the selected unit types.", "Conversion failed: Unsupported unit types." );
                }
            }
            return ConvertedValue;
        }

        /// <summary>
        /// Takes a numeric value and converts it from one Unit of Measurement into another using the given Conversion Factor values
        /// If unit conversion cannot be applied, an error is thrown.
        /// </summary>
        public static double applyUnitConversion( Double ValueToConvert, Double OldConversionFactor, Double NewConversionFactor, Double SpecificGravity = 1 )
        {
            Double ConvertedValue;
            if( ( OldConversionFactor != 0 && OldConversionFactor != Double.NaN ) && ( NewConversionFactor != 0 && NewConversionFactor != Double.NaN ) )
            {
                //Operator logic defined in W1005 - Math is hard.
                ConvertedValue = ValueToConvert / OldConversionFactor * SpecificGravity * NewConversionFactor;
            }
            else
            {
                throw new CswDniException( ErrorType.Error, "Conversion Factor must be defined as a positive number.", "Conversion Factor must be defined as a positive number." );
            }
            if( ConvertedValue == Double.NaN || ConvertedValue < 0 )
            {
                throw new CswDniException( ErrorType.Error, "Conversion failed: Insufficient data provided.", "Conversion failed: One or more parameters are negative or undefined." );
            }
            return ConvertedValue;
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

        private class CswNbtUnitConversionEnums
        {
            /// <summary>
            /// Enum: Used to identify the UnitType relationship between two UnitOfMeasure Nodes in order to apply the correct conversion logic
            /// </summary>
            public sealed class UnitTypeRelationship : CswEnum<UnitTypeRelationship>
            {
                private UnitTypeRelationship( string Name ) : base( Name ) { }
                public static IEnumerable<UnitTypeRelationship> _All { get { return CswEnum<UnitTypeRelationship>.All; } }

                public static explicit operator UnitTypeRelationship( string str )
                {
                    UnitTypeRelationship ret = Parse( str );
                    return ( ret != null ) ? ret : UnitTypeRelationship.Unknown;
                }

                public static readonly UnitTypeRelationship Unknown = new UnitTypeRelationship( "Unknown" );
                public static readonly UnitTypeRelationship Same = new UnitTypeRelationship( "Same" );
                public static readonly UnitTypeRelationship NotSupported = new UnitTypeRelationship( "NotSupported" );
                public static readonly UnitTypeRelationship WeightToVolume = new UnitTypeRelationship( "WeightToVolume" );
                public static readonly UnitTypeRelationship VolumeToWeight = new UnitTypeRelationship( "VolumeToWeight" );
            }
        }
    }
}
