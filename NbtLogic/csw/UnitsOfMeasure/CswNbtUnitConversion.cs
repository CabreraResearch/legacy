using System;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.csw.Conversion
{
    public class CswNbtUnitConversion
    {
        #region Properties and ctor

        private CswNbtResources _CswNbtResources;
        private CswNbtObjClassUnitOfMeasure.UnitTypes _OldUnitType = CswNbtObjClassUnitOfMeasure.UnitTypes.Unknown;
        private CswNbtObjClassUnitOfMeasure.UnitTypes _NewUnitType = CswNbtObjClassUnitOfMeasure.UnitTypes.Unknown;
        private Double _OldConversionFactor = Double.NaN;
        private Double _NewConversionFactor = Double.NaN;
        private Double _MaterialSpecificGravity = Double.NaN;

        public CswNbtUnitConversion()
        { }

        public CswNbtUnitConversion( CswNbtResources _CswNbtResourcesIn, CswPrimaryKey OldUnitNodeId, CswPrimaryKey NewUnitNodeId, CswPrimaryKey MaterialNodeId = null )
        {
            _CswNbtResources = _CswNbtResourcesIn;

            if( OldUnitNodeId != null && NewUnitNodeId != null )
            {
                CswNbtObjClassUnitOfMeasure OldUnitNode = _CswNbtResources.Nodes.GetNode( OldUnitNodeId );
                CswNbtObjClassUnitOfMeasure NewUnitNode = _CswNbtResources.Nodes.GetNode( NewUnitNodeId );
                setOldUnitProps( OldUnitNode );
                setNewUnitProps( NewUnitNode );
                if( MaterialNodeId != null )
                {
                    CswNbtObjClassMaterial MaterialNode = _CswNbtResources.Nodes.GetNode( MaterialNodeId );
                    setMaterialProps( MaterialNode );
                }
            }
        }

        #endregion

        #region Setters

        public void setOldUnitProps( CswNbtObjClassUnitOfMeasure OldUnitNode )
        {
            if( OldUnitNode != null )
            {
                _OldConversionFactor = _getScientificValue( OldUnitNode.ConversionFactor );
                _OldUnitType = (CswNbtObjClassUnitOfMeasure.UnitTypes) OldUnitNode.UnitType.Value;
            }
        }

        public void setNewUnitProps( CswNbtObjClassUnitOfMeasure NewUnitNode )
        {
            if( NewUnitNode != null )
            {
                _NewConversionFactor = _getScientificValue( NewUnitNode.ConversionFactor );
                _NewUnitType = (CswNbtObjClassUnitOfMeasure.UnitTypes) NewUnitNode.UnitType.Value;
            }
        }

        public void setMaterialProps( CswNbtObjClassMaterial MaterialNode )
        {
            if( MaterialNode != null )
            {
                //_MaterialSpecificGravity = CswConvert.ToDouble( MaterialNode.SpecificGravity );
                _MaterialSpecificGravity = MaterialNode.SpecificGravity.Value;
            }
        }

        #endregion

        #region Unit Conversion Functions

        /// <summary>
        /// Takes a numeric value and interconverts it between different Unit Types using the class-set UnitOfMeasure and Material prop values.
        /// If unit conversion cannot be applied, an error is thrown.
        /// </summary>
        public Double convertUnit( Double ValueToConvert )
        {
            Double ConvertedValue = ValueToConvert;

            if( CswTools.IsDouble( _OldConversionFactor ) && CswTools.IsDouble( _NewConversionFactor ) )
            {
                CswNbtUnitConversionEnums.UnitTypeRelationship UnitRelationship = _getUnitTypeRelationship( _OldUnitType, _NewUnitType );

                if( UnitRelationship == CswNbtUnitConversionEnums.UnitTypeRelationship.Same )
                {
                    ConvertedValue = applyUnitConversion( ValueToConvert, _OldConversionFactor, _NewConversionFactor );
                }
                else if( UnitRelationship != CswNbtUnitConversionEnums.UnitTypeRelationship.NotSupported )
                {
                    if( CswTools.IsDouble( _MaterialSpecificGravity ) && _MaterialSpecificGravity > 0 )
                    {
                        //UnitType-specific logic (Operator logic defined in W1005)
                        if( UnitRelationship == CswNbtUnitConversionEnums.UnitTypeRelationship.WeightToVolume )
                        {
                            ConvertedValue = applyUnitConversion( ValueToConvert, _OldConversionFactor, _NewConversionFactor, 1.0 / _MaterialSpecificGravity );
                        }
                        else if( UnitRelationship == CswNbtUnitConversionEnums.UnitTypeRelationship.VolumeToWeight )
                        {
                            ConvertedValue = applyUnitConversion( ValueToConvert, _OldConversionFactor, _NewConversionFactor, _MaterialSpecificGravity );
                        }
                    }
                    else
                    {
                        throw new CswDniException( ErrorType.Error, "Conversion failed: The Container Material's specific gravity is zero, negative, or undefined.", "Specific Gravity must be defined as a positive number." );
                    }
                }
                else
                {
                    _CswNbtResources.logMessage( "Conversion failed: Unable to apply unit conversion between the selected unit types." );
                }
            }
            else
            {
                string UserMessage = "Conversion failed: Insufficient data supplied.";
                if( false == CswTools.IsDouble( ValueToConvert ) )
                {
                    UserMessage = "Conversion failed: Container has no Quantity defined.";
                }
                else if( false == CswTools.IsDouble( _NewConversionFactor ) )
                {
                    UserMessage = "Conversion failed: The unit of measurement with which to convert is undefined.";
                }
                _CswNbtResources.logMessage( UserMessage + " Conversion failed: Unable to determine appropriate conversion factors." );
            }
            return ConvertedValue;
        }

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

                CswNbtUnitConversion UnitConverter = new CswNbtUnitConversion();

                UnitConverter.setOldUnitProps( OldUnitNode );
                UnitConverter.setNewUnitProps( NewUnitNode );
                if( MaterialNodeIn != null )
                {
                    CswNbtObjClassMaterial MaterialNode = MaterialNodeIn;
                    UnitConverter.setMaterialProps( MaterialNode );
                }
                ConvertedValue = UnitConverter.convertUnit( ValueToConvert );
            }
            else
            {
                throw new CswDniException( ErrorType.Error, "Conversion failed: Insufficient data supplied.", "Conversion failed: Null Node(s) provided." );
            }

            return ConvertedValue;
        }

        #endregion

        #region Conversion Logic

        /// <summary>
        /// Takes a numeric value and converts it from one Unit of Measurement into another using the given Conversion Factor values
        /// If unit conversion cannot be applied, an error is thrown.
        /// </summary>
        public static double applyUnitConversion( Double ValueToConvert, Double OldConversionFactor, Double NewConversionFactor, Double SpecificGravity = 1 )
        {
            Double ConvertedValue;
            if( CswTools.IsDouble( OldConversionFactor ) && OldConversionFactor != 0 && CswTools.IsDouble( NewConversionFactor ) && NewConversionFactor != 0 )
            {
                //Operator logic defined in W1005 - Math is hard.
                ConvertedValue = ValueToConvert / OldConversionFactor * SpecificGravity * NewConversionFactor;
            }
            else
            {
                throw new CswDniException( ErrorType.Error, "Conversion Factor must be defined as a positive number.", "Conversion Factor must be defined as a positive number." );
            }
            if( false == CswTools.IsDouble( ConvertedValue ) )
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

        #endregion

        #region UnitType Relationship

        /// <summary>
        /// Identifies the UnitType relationship between two UnitTypes.
        /// </summary>
        private static CswNbtUnitConversionEnums.UnitTypeRelationship _getUnitTypeRelationship( CswNbtObjClassUnitOfMeasure.UnitTypes OldUnitType, CswNbtObjClassUnitOfMeasure.UnitTypes NewUnitType )
        {
            CswNbtUnitConversionEnums.UnitTypeRelationship UnitRelationship = CswNbtUnitConversionEnums.UnitTypeRelationship.Unknown;

            if( OldUnitType.ToString() == NewUnitType.ToString() )
            {
                UnitRelationship = CswNbtUnitConversionEnums.UnitTypeRelationship.Same;
            }
            else if( OldUnitType == CswNbtObjClassUnitOfMeasure.UnitTypes.Weight && NewUnitType == CswNbtObjClassUnitOfMeasure.UnitTypes.Volume )
            {
                UnitRelationship = CswNbtUnitConversionEnums.UnitTypeRelationship.WeightToVolume;
            }
            else if( OldUnitType == CswNbtObjClassUnitOfMeasure.UnitTypes.Volume && NewUnitType == CswNbtObjClassUnitOfMeasure.UnitTypes.Weight )
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
                    return ret ?? UnitTypeRelationship.Unknown;
                }

                public static readonly UnitTypeRelationship Unknown = new UnitTypeRelationship( "Unknown" );
                public static readonly UnitTypeRelationship Same = new UnitTypeRelationship( "Same" );
                public static readonly UnitTypeRelationship NotSupported = new UnitTypeRelationship( "NotSupported" );
                public static readonly UnitTypeRelationship WeightToVolume = new UnitTypeRelationship( "WeightToVolume" );
                public static readonly UnitTypeRelationship VolumeToWeight = new UnitTypeRelationship( "VolumeToWeight" );
            }
        }

        #endregion
    }
}
