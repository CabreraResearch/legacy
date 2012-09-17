using System;
using ChemSW.Core;
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
        /// Create a Unit View for a Quantity property using a Material NodeId and a Quantity property
        /// </summary>
        public void setQuantityUnitOfMeasureView( CswNbtNode MaterialNode, CswNbtNodePropQuantity Size )
        {
            if( null != Size )
            {
                getQuantityUnitOfMeasureView( MaterialNode, Size.View );
                if( null != Size.View )
                {
                    Size.View.save();
                }
            }
        }

        /// <summary>
        /// Build a Unit View for a Quantity property using a Material NodeId
        /// </summary>
        public CswNbtView getQuantityUnitOfMeasureView( CswNbtNode MaterialNode, CswNbtView View = null )
        {
            CswNbtView Ret = View;

            CswNbtObjClassMaterial MaterialNodeAsMaterial = MaterialNode;
            if( null != MaterialNode &&
                ( false == string.IsNullOrEmpty( MaterialNodeAsMaterial.PhysicalState.Value ) || false == string.IsNullOrEmpty( MaterialNodeAsMaterial.PhysicalState.DefaultValue.AsList.Value ) ) )
            {
                string PhysicalState = MaterialNodeAsMaterial.PhysicalState.Value;
                if( string.IsNullOrEmpty( PhysicalState ) )
                {
                    PhysicalState = MaterialNodeAsMaterial.PhysicalState.DefaultValue.AsList.Value; //some materials do only use one default phys state
                }

                CswNbtMetaDataObjectClass UnitOfMeasureOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UnitOfMeasureClass );
                if( null == Ret )
                {
                    Ret = new CswNbtView( _CswNbtResources );
                    Ret.saveNew( MaterialNodeAsMaterial.Node.NodeName + " Units Of Measure View", NbtViewVisibility.Property );
                }
                else
                {
                    Ret.Root.ChildRelationships.Clear();
                }
                foreach( CswNbtMetaDataNodeType UnitOfMeasureNodeType in UnitOfMeasureOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp UnitTypeProp = UnitOfMeasureNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassUnitOfMeasure.PropertyName.UnitType );
                    CswNbtObjClassUnitOfMeasure.UnitTypes UnitType = (CswNbtObjClassUnitOfMeasure.UnitTypes) UnitTypeProp.DefaultValue.AsList.Value;
                    if( _physicalStateMatchesUnitType( PhysicalState, UnitType ) )
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
                CswNbtMetaDataNodeTypeProp UnitTypeProp = UnitOfMeasureNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassUnitOfMeasure.PropertyName.UnitType );
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
                case CswNbtObjClassMaterial.PhysicalStates.NA:
                    matchFound = UnitType == CswNbtObjClassUnitOfMeasure.UnitTypes.Each;
                    break;
                case CswNbtObjClassMaterial.PhysicalStates.Solid:
                    matchFound = UnitType == CswNbtObjClassUnitOfMeasure.UnitTypes.Weight;
                    break;
                case CswNbtObjClassMaterial.PhysicalStates.Liquid:
                case CswNbtObjClassMaterial.PhysicalStates.Gas:
                    matchFound = UnitType == CswNbtObjClassUnitOfMeasure.UnitTypes.Weight ||
                                    UnitType == CswNbtObjClassUnitOfMeasure.UnitTypes.Volume;
                    break;
            }

            return matchFound;
        }

    }
}
