using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassUnitOfMeasure : CswNbtObjClass
    {
        public static string NamePropertyName { get { return "Name"; } }
        public static string BaseUnitPropertyName { get { return "Base Unit"; } }
        public static string ConversionFactorPropertyName { get { return "Conversion Factor"; } }
        public static string FractionalPropertyName { get { return "Fractional"; } }
        public static string UnitTypePropertyName { get { return "Unit Type"; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassUnitOfMeasure( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UnitOfMeasureClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassUnitOfMeasure
        /// </summary>
        public static implicit operator CswNbtObjClassUnitOfMeasure( CswNbtNode Node )
        {
            CswNbtObjClassUnitOfMeasure ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.UnitOfMeasureClass ) )
            {
                ret = (CswNbtObjClassUnitOfMeasure) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events
        
        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
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

        public override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
        {
            
            
            
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropText Name { get { return ( _CswNbtNode.Properties[NamePropertyName] ); } }
        public CswNbtNodePropText BaseUnit { get { return ( _CswNbtNode.Properties[BaseUnitPropertyName] ); } }
        public CswNbtNodePropScientific ConversionFactor { get { return ( _CswNbtNode.Properties[ConversionFactorPropertyName] ); } }
        public CswNbtNodePropLogical Fractional { get { return ( _CswNbtNode.Properties[FractionalPropertyName] ); } }
        public CswNbtNodePropList UnitType { get { return ( _CswNbtNode.Properties[UnitTypePropertyName] ); } }

        #endregion

        /// <summary>
        /// Enum: Used to identify the UnitType of a UnitOfMeasure Node/NodeType in order to apply correct unit conversion logic
        /// </summary>
        public sealed class UnitTypes : CswEnum<UnitTypes>
        {
            private UnitTypes( string Name ) : base( Name ) { }
            public static IEnumerable<UnitTypes> _All { get { return CswEnum<UnitTypes>.All; } }

            public static explicit operator UnitTypes( string str )
            {
                UnitTypes ret = Parse( str );
                return ( ret != null ) ? ret : UnitTypes.Unknown;
            }

            public static readonly UnitTypes Unknown = new UnitTypes( "Unknown" );
            public static readonly UnitTypes Weight = new UnitTypes( "Weight" );
            public static readonly UnitTypes Volume = new UnitTypes( "Volume" );
            public static readonly UnitTypes Time = new UnitTypes( "Time" );
            public static readonly UnitTypes Each = new UnitTypes( "Each" );
        }

    }//CswNbtObjClassUnitOfMeasure

}//namespace ChemSW.Nbt.ObjClasses
