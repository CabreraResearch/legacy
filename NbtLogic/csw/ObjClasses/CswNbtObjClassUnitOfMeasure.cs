using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassUnitOfMeasure : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Name = "Name";
            public const string BaseUnit = "Base Unit";
            public const string ConversionFactor = "Conversion Factor";
            public const string Fractional = "Fractional";
            public const string UnitType = "Unit Type";
            public const string UnitConversion = "Unit Conversion";
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassUnitOfMeasure( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.UnitOfMeasureClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassUnitOfMeasure
        /// </summary>
        public static implicit operator CswNbtObjClassUnitOfMeasure( CswNbtNode Node )
        {
            CswNbtObjClassUnitOfMeasure ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.UnitOfMeasureClass ) )
            {
                ret = (CswNbtObjClassUnitOfMeasure) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            if( false == IsTemp )
            {
                _validateConversionFactor();
            }
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );
        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        protected override void afterPopulateProps()
        {
            ConversionFactor.SetOnPropChange( OnConversionFactorPropChange );
            _CswNbtObjClassDefault.triggerAfterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }

        #endregion

        private void _validateConversionFactor()
        {
            if( UnitType.Value != UnitTypes.Each.ToString() && false == CswTools.IsDouble( ConversionFactor.RealValue ) )
            {
                throw new CswDniException
                (
                    ErrorType.Warning,
                    "Units of type " + UnitType.Value + " must have a Conversion Factor.",
                    "Unit of Measure cannot be used for unit conversion."
                );
            }
        }

        #region Object class specific properties

        public CswNbtNodePropText Name { get { return ( _CswNbtNode.Properties[PropertyName.Name] ); } }
        public CswNbtNodePropText BaseUnit { get { return ( _CswNbtNode.Properties[PropertyName.BaseUnit] ); } }
        public CswNbtNodePropScientific ConversionFactor { get { return ( _CswNbtNode.Properties[PropertyName.ConversionFactor] ); } }
        private void OnConversionFactorPropChange( CswNbtNodeProp Prop )
        {
            _validateConversionFactor();
        }
        public CswNbtNodePropLogical Fractional { get { return ( _CswNbtNode.Properties[PropertyName.Fractional] ); } }
        public CswNbtNodePropList UnitType { get { return ( _CswNbtNode.Properties[PropertyName.UnitType] ); } }
        public CswNbtNodePropStatic UnitConversion { get { return ( _CswNbtNode.Properties[PropertyName.UnitConversion] ); } }

        #endregion

        /// <summary>
        /// Enum: Used to identify the UnitType of a UnitOfMeasure Node/NodeType in order to apply correct unit conversion logic
        /// </summary>
        public sealed class UnitTypes : CswEnum<UnitTypes>
        {
            private UnitTypes( string Name ) : base( Name ) { }
            public static IEnumerable<UnitTypes> _All { get { return All; } }

            public static explicit operator UnitTypes( string str )
            {
                UnitTypes ret = Parse( str );
                return ret ?? Unknown;
            }

            public static readonly UnitTypes Unknown = new UnitTypes( "Unknown" );
            public static readonly UnitTypes Weight = new UnitTypes( "Weight" );
            public static readonly UnitTypes Volume = new UnitTypes( "Volume" );
            public static readonly UnitTypes Time = new UnitTypes( "Time" );
            public static readonly UnitTypes Each = new UnitTypes( "Each" );
            public static readonly UnitTypes Radiation = new UnitTypes( "Radiation" );
        }

    }//CswNbtObjClassUnitOfMeasure

}//namespace ChemSW.Nbt.ObjClasses
