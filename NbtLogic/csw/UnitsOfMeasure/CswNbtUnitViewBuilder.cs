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

        public String getPhysicalState( CswNbtPropertySetMaterial MaterialNode )
        {
            String PhysicalState = "n/a";
            if( MaterialNode.ObjectClass.ObjectClass == CswEnumNbtObjectClass.ChemicalClass )
            {
                CswNbtObjClassChemical ChemicalNode = MaterialNode.Node;
                PhysicalState = ChemicalNode.PhysicalState.Value;
            }
            return PhysicalState;
        }

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
        public void setQuantityUnitOfMeasureView( CswNbtNode MaterialNode, CswNbtNodePropQuantity Size, bool ExcludeEach = false )
        {
            if( null != Size )
            {
                getQuantityUnitOfMeasureView( MaterialNode, ExcludeEach, Size.View );
                if( null != Size.View )
                {
                    Size.View.save();
                }
            }
        }

        /// <summary>
        /// Build a Unit View for a Quantity property using a Material NodeId
        /// </summary>
        public CswNbtView getQuantityUnitOfMeasureView( CswNbtNode MaterialNode, bool ExcludeEach = false, CswNbtView View = null )
        {
            CswNbtView Ret = View;
            if( null != MaterialNode )
            {
                string PhysicalState = getPhysicalState( MaterialNode );
                if( null == Ret )
                {
                    Ret = new CswNbtView( _CswNbtResources );
                    Ret.saveNew( MaterialNode.NodeName + " Units Of Measure View", CswEnumNbtViewVisibility.Property );
                }
                else
                {
                    Ret.Root.ChildRelationships.Clear();
                }
                _populateUnitViewRelationships( Ret, PhysicalState, ExcludeEach );
            }
            return Ret;
        }

        /// <summary>
        /// Build a Unit View for a Quantity property using a PhysicalState
        /// </summary>
        public CswNbtView getQuantityUnitOfMeasureView( string PhysicalState )
        {
            CswNbtView Ret = new CswNbtView( _CswNbtResources );
            _populateUnitViewRelationships( Ret, PhysicalState, false );
            return Ret;
        }

        private void _populateUnitViewRelationships( CswNbtView UnitView, string PhysicalState, bool ExcludeEach )
        {
            CswNbtMetaDataObjectClass UnitOfMeasureOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.UnitOfMeasureClass );
            CswNbtMetaDataNodeType EachNT = null;
            foreach( CswNbtMetaDataNodeType UnitOfMeasureNodeType in UnitOfMeasureOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp UnitTypeProp = UnitOfMeasureNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassUnitOfMeasure.PropertyName.UnitType );
                CswEnumNbtUnitTypes UnitType = (CswEnumNbtUnitTypes) UnitTypeProp.DefaultValue.AsList.Value;
                if( _physicalStateMatchesUnitType( PhysicalState, UnitType, ExcludeEach ) )
                {
                    if( UnitType == CswEnumNbtUnitTypes.Each )
                    {
                        EachNT = UnitOfMeasureNodeType;
                    }
                    else
                    {
                        UnitView.AddViewRelationship( UnitOfMeasureNodeType, true );
                    }
                }
            }
            if( null != EachNT )//Case 29933 - Each units always go at the end of the list
            {
                UnitView.AddViewRelationship( EachNT, true );
            }
        }

        private bool _physicalStateMatchesUnitType( string PhysicalState, CswEnumNbtUnitTypes UnitType, bool ExcludeEach = false )
        {
            //Case 29589 - add Each units to all Size qty options, unless explicitly stated otherwise (dispensing)
            bool matchFound = UnitType == CswEnumNbtUnitTypes.Each && false == ExcludeEach;
            if( false == matchFound )
            {
                switch( PhysicalState )
                {
                    case CswNbtPropertySetMaterial.CswEnumPhysicalState.NA:
                        matchFound = UnitType == CswEnumNbtUnitTypes.Each;
                        break;
                    case CswNbtPropertySetMaterial.CswEnumPhysicalState.Solid:
                        matchFound = UnitType == CswEnumNbtUnitTypes.Weight;
                        break;
                    case CswNbtPropertySetMaterial.CswEnumPhysicalState.Liquid:
                    case CswNbtPropertySetMaterial.CswEnumPhysicalState.Gas:
                        matchFound = UnitType == CswEnumNbtUnitTypes.Weight ||
                                     UnitType == CswEnumNbtUnitTypes.Volume;
                        break;
                }
            }
            return matchFound;
        }

    }
}
