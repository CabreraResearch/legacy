using System;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;

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
                _OldConversionFactor = OldUnitNode.ConversionFactor.RealValue;
                _OldUnitType = (CswNbtObjClassUnitOfMeasure.UnitTypes) OldUnitNode.UnitType.Value;
            }
        }

        public void setNewUnitProps( CswNbtObjClassUnitOfMeasure NewUnitNode )
        {
            if( NewUnitNode != null )
            {
                _NewConversionFactor = NewUnitNode.ConversionFactor.RealValue;
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
            if( false == CswTools.IsDouble( ConvertedValue ) )
            {
                ConvertedValue = 0.0;
            }
            else if( CswTools.IsDouble( _OldConversionFactor ) && CswTools.IsDouble( _NewConversionFactor ) )
            {
                CswNbtUnitConversionEnums.UnitTypeRelationship UnitRelationship = _getUnitTypeRelationship( _OldUnitType, _NewUnitType );
                if( UnitRelationship == CswNbtUnitConversionEnums.UnitTypeRelationship.Same )
                {
                    ConvertedValue = applyUnitConversion( ValueToConvert, _OldConversionFactor, _NewConversionFactor );
                }
                else if( UnitRelationship != CswNbtUnitConversionEnums.UnitTypeRelationship.NotSupported )
                {
                    if( CswTools.IsDouble( _MaterialSpecificGravity ) )
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
                        _CswNbtResources.logMessage( "Conversion failed: The Container Material's specific gravity is undefined." );
                    }
                }
                else
                {
                    _CswNbtResources.logMessage( "Conversion failed: Unable to apply unit conversion between the selected unit types." );
                }
            }
            else
            {
                _CswNbtResources.logMessage( "Conversion failed: Unable to determine appropriate conversion factors." );
            }
            return ConvertedValue;
        }

        /// <summary>
        /// Takes a numeric value and converts it from one Unit of Measurement into another using the given Conversion Factor values
        /// If unit conversion cannot be applied, an error is thrown.
        /// </summary>
        public double applyUnitConversion( Double ValueToConvert, Double OldConversionFactor, Double NewConversionFactor, Double SpecificGravity = 1 )
        {
            _validateValuesForConversion( ValueToConvert, OldConversionFactor, NewConversionFactor, SpecificGravity );
            Double ConvertedValue = ValueToConvert / OldConversionFactor * SpecificGravity * NewConversionFactor; //See W1005 for more details
            return ConvertedValue;
        }

        #endregion

        #region Validation

        /// <summary>
        /// Determines if a set of values are valid for unit conversion.
        /// </summary>
        private void _validateValuesForConversion( Double ValueToConvert, Double OldConversionFactor, Double NewConversionFactor, Double SpecificGravity )
        {
            string ErrorMessage = String.Empty;
            if( false == CswTools.IsDouble( ValueToConvert ) )
            {
                ErrorMessage = "Value to convert is undefined: " + ValueToConvert;
            }
            else if( false == CswTools.IsDouble( OldConversionFactor ) || OldConversionFactor <= 0 )
            {
                ErrorMessage = "Current unit's conversion factor is invalid: " + OldConversionFactor;
            }
            else if( false == CswTools.IsDouble( NewConversionFactor ) || NewConversionFactor <= 0 )
            {
                ErrorMessage = "New unit's conversion factor is invalid: " + NewConversionFactor;
            }
            else if( false == CswTools.IsDouble( SpecificGravity ) || SpecificGravity <= 0 )
            {
                ErrorMessage = "Material's specific gravity is invalid: " + SpecificGravity;
            }
            if( false == String.IsNullOrEmpty( ErrorMessage ) )
            {
                throw new CswDniException( ErrorType.Warning, ErrorMessage, "Unit conversion failed." );
            }
        }

        #endregion

        #region UnitType Relationship

        /// <summary>
        /// Identifies the UnitType relationship between two UnitTypes.
        /// </summary>
        private CswNbtUnitConversionEnums.UnitTypeRelationship _getUnitTypeRelationship( CswNbtObjClassUnitOfMeasure.UnitTypes OldUnitType, CswNbtObjClassUnitOfMeasure.UnitTypes NewUnitType )
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
