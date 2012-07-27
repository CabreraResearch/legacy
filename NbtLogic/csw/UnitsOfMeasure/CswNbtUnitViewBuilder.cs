using System;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.UnitsOfMeasure
{
    /// <summary>
    /// Utility class to create the appropriate Unit view for a Quantity
    /// </summary>
    public class CswNbtUnitViewBuilder
    {
        #region Properties and ctor

        private CswNbtResources _CswNbtResources;

        public CswNbtUnitViewBuilder( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        #endregion

        /// <summary>
        /// Build a Unit View for a Quantity property using a Material NodeId
        /// </summary>
        public CswNbtView getQuantityUnitOfMeasureView( CswPrimaryKey MaterialNodeId )
        {
            CswNbtView Ret = null;
            if( null != MaterialNodeId && Int32.MinValue != MaterialNodeId.PrimaryKey )
            {
                CswNbtNode MaterialNode = _CswNbtResources.Nodes.GetNode( MaterialNodeId );
                Ret = getQuantityUnitOfMeasureView( MaterialNode );
            }
            return Ret;
        }

        /// <summary>
        /// Build a Unit View for a Quantity property using a Material NodeId
        /// </summary>
        public CswNbtView getQuantityUnitOfMeasureView( CswNbtNode MaterialNode )
        {
            CswNbtView Ret = null;

            CswNbtObjClassMaterial MaterialNodeAsMaterial = MaterialNode;
            if( null != MaterialNode &&
                false == string.IsNullOrEmpty( MaterialNodeAsMaterial.PhysicalState.Value ) )
            {
                CswNbtMetaDataObjectClass UnitOfMeasureOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UnitOfMeasureClass );
                Ret = new CswNbtView( _CswNbtResources );

                foreach( CswNbtMetaDataNodeType UnitOfMeasureNodeType in UnitOfMeasureOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp UnitTypeProp = UnitOfMeasureNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassUnitOfMeasure.UnitTypePropertyName );
                    CswNbtObjClassUnitOfMeasure.UnitTypes UnitType = (CswNbtObjClassUnitOfMeasure.UnitTypes) UnitTypeProp.DefaultValue.AsList.Value;
                    if( _physicalStateMatchesUnitType( MaterialNodeAsMaterial.PhysicalState.Value, UnitType ) )
                    {
                        Ret.AddViewRelationship( UnitOfMeasureNodeType, true );
                    }
                }
            }
            return Ret;
        }

        public CswNbtView getQuantityUnitOfMeasureView( string PhysicalState )
        {
            CswNbtView Ret = null;
            CswNbtMetaDataObjectClass UnitOfMeasureOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UnitOfMeasureClass );
            Ret = new CswNbtView( _CswNbtResources );

            foreach( CswNbtMetaDataNodeType UnitOfMeasureNodeType in UnitOfMeasureOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp UnitTypeProp = UnitOfMeasureNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassUnitOfMeasure.UnitTypePropertyName );
                CswNbtObjClassUnitOfMeasure.UnitTypes UnitType = (CswNbtObjClassUnitOfMeasure.UnitTypes) UnitTypeProp.DefaultValue.AsList.Value;
                if( _physicalStateMatchesUnitType( PhysicalState, UnitType ) )
                {
                    Ret.AddViewRelationship( UnitOfMeasureNodeType, true );
                }
            }
            return Ret;
        }

        private bool _physicalStateMatchesUnitType( string PhysicalState, CswNbtObjClassUnitOfMeasure.UnitTypes UnitType )
        {
            bool matchFound = false;

            switch( PhysicalState )
            {
                case "n/a":
                    matchFound = UnitType == CswNbtObjClassUnitOfMeasure.UnitTypes.Each;
                    break;
                case "solid":
                    matchFound = UnitType == CswNbtObjClassUnitOfMeasure.UnitTypes.Weight;
                    break;
                case "liquid":
                case "gas":
                    matchFound = UnitType == CswNbtObjClassUnitOfMeasure.UnitTypes.Weight ||
                                    UnitType == CswNbtObjClassUnitOfMeasure.UnitTypes.Volume;
                    break;
            }

            return matchFound;
        }

    }
}
