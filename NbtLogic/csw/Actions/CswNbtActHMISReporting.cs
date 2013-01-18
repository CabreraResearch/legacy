﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Conversion;

namespace ChemSW.Nbt.Actions
{
    #region DataContract

    [DataContract]
    public class HMISData
    {
        public HMISData()
        {
            Materials = new Collection<HMISMaterial>();
        }

        [DataMember]
        public String FireClassExemptAmountSet;
        [DataMember]
        public Collection<HMISMaterial> Materials;

        [DataContract]
        public class HMISMaterial
        {
            public HMISMaterial()
            {
                Storage = new StorageData();
                Closed = new ClosedData();
                Open = new OpenData();
            }

            [DataMember]
            public String Material = String.Empty;
            [DataMember]
            public String PhysicalState = String.Empty;
            [DataMember]
            public String HazardClass = String.Empty;
            [DataMember]
            public StorageData Storage;
            [DataMember]
            public ClosedData Closed;
            [DataMember]
            public OpenData Open;
        }

        [DataContract]
        public class StorageData
        {
            public StorageData()
            {
                Solid = new HMISQty();
                Liquid = new HMISQty();
                Gas = new HMISQty();
            }

            [DataMember]
            public HMISQty Solid;
            [DataMember]
            public HMISQty Liquid;
            [DataMember]
            public HMISQty Gas;
        }

        [DataContract]
        public class ClosedData
        {
            public ClosedData()
            {
                Solid = new HMISQty();
                Liquid = new HMISQty();
                Gas = new HMISQty();
            }

            [DataMember]
            public HMISQty Solid;
            [DataMember]
            public HMISQty Liquid;
            [DataMember]
            public HMISQty Gas;
        }

        [DataContract]
        public class OpenData
        {
            public OpenData()
            {
                Solid = new HMISQty();
                Liquid = new HMISQty();
            }

            [DataMember]
            public HMISQty Solid;
            [DataMember]
            public HMISQty Liquid;
        }

        [DataContract]
        public class HMISQty
        {
            private Int32 Precision = 6;
            private Double _Qty;
            [DataMember]
            public String MAQ = String.Empty;
            [DataMember]
            public Double Qty
            {
                get { return _Qty; }
                set { _Qty = CswTools.IsDouble( value ) ? Math.Round( value, Precision, MidpointRounding.AwayFromZero ) : 0.0; }
            }
        }

        [DataContract]
        public class HMISDataRequest
        {
            [DataMember]
            public String ControlZoneId = String.Empty;
        }

    } // HMISData

    #endregion DataContract

    public class CswNbtActHMISReporting
    {
        #region Properties and ctor

        private CswNbtResources _CswNbtResources;
        private HMISData Data;
        private Collection<CswNbtObjClassFireClassExemptAmount> FireClasses;
        private CswPrimaryKey ControlZoneId;

        public CswNbtActHMISReporting( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            Data = new HMISData();
            FireClasses = new Collection<CswNbtObjClassFireClassExemptAmount>();
        }

        #endregion Properties and ctor

        #region Public Methods

        public HMISData getHMISData( HMISData.HMISDataRequest Request )
        {
            ControlZoneId = CswConvert.ToPrimaryKey( Request.ControlZoneId );
            FireClasses = _setFireClasses();
            CswNbtView HMISView = _getHMISView();
            ICswNbtTree HMISTree = _CswNbtResources.Trees.getTreeFromView( HMISView, false, true, false );
            Int32 LocationCount = HMISTree.getChildNodeCount();
            for( int i = 0; i < LocationCount; i++ )//Location Nodes
            {
                HMISTree.goToNthChild( i );
                Int32 ContainerCount = HMISTree.getChildNodeCount();
                for( int j = 0; j < ContainerCount; j++ )//Container Nodes
                {
                    HMISTree.goToNthChild( j );
                    if( HMISTree.getChildNodeCount() > 0 )//Material Node Exists
                    {
                        Double Quantity = Double.NaN;
                        CswPrimaryKey UnitId = null;
                        String MaterialName = String.Empty;
                        String UseType = String.Empty;
                        foreach( CswNbtTreeNodeProp ContainerProp in HMISTree.getChildNodePropsOfNode() )
                        {
                            CswNbtMetaDataNodeTypeProp ContainerNTP = _CswNbtResources.MetaData.getNodeTypeProp( ContainerProp.NodeTypePropId );
                            CswNbtMetaDataObjectClassProp ContainerOCP = ContainerNTP.getObjectClassProp();
                            if( null != ContainerOCP )
                            {
                                switch( ContainerOCP.PropName )
                                {
                                    case CswNbtObjClassContainer.PropertyName.Quantity:
                                        Quantity = ContainerProp.Field1_Numeric;
                                        UnitId = CswConvert.ToPrimaryKey( "nodes_" + ContainerProp.Field1_Fk );
                                        break;
                                    case CswNbtObjClassContainer.PropertyName.Material:
                                        MaterialName = ContainerProp.Field1;
                                        break;
                                    case CswNbtObjClassContainer.PropertyName.UseType:
                                        UseType = ContainerProp.Field1;
                                        break;
                                }
                            }
                        }
                        if( false == String.IsNullOrEmpty( UseType ) )
                        {
                            HMISData.HMISMaterial HMISMaterial = Data.Materials.FirstOrDefault( ExistingMaterial => ExistingMaterial.Material == MaterialName );
                            if( null != HMISMaterial )
                            {
                                _addQuantityDataToHMISMaterial( HMISMaterial, UseType, Quantity, UnitId );
                            }
                            else
                            {
                                HMISTree.goToNthChild( 0 );
                                CswNbtObjClassMaterial MaterialNode = HMISTree.getNodeForCurrentPosition();
                                CswNbtMetaDataNodeTypeProp HazardClassesNTP = _CswNbtResources.MetaData.getNodeTypeProp( MaterialNode.NodeTypeId, "Hazard Classes" );
                                IEnumerable<CswNbtObjClassFireClassExemptAmount> HazardClasses = _getRelevantHazardClasses( MaterialNode.Node.Properties[HazardClassesNTP].AsMultiList.Value );
                                foreach (CswNbtObjClassFireClassExemptAmount HazardClass in HazardClasses)
                                {
                                    HMISMaterial = new HMISData.HMISMaterial();
                                    HMISMaterial.Material = MaterialName;
                                    HMISMaterial.HazardClass = HazardClass.HazardClass.Value;
                                    HMISMaterial.PhysicalState = MaterialNode.PhysicalState.Value;
                                    _setFireClassMAQData( HMISMaterial, HazardClass );
                                    _addQuantityDataToHMISMaterial( HMISMaterial, UseType, Quantity, UnitId );
                                    Data.Materials.Add( HMISMaterial );
                                }
                                HMISTree.goToParentNode();
                            }
                        }
                    }//Material Node Exists
                    HMISTree.goToParentNode();
                }//Container Nodes
                HMISTree.goToParentNode();
            }//Location Nodes          
            return Data;
        }

        #endregion Public Methods

        #region Private Methods

        private Collection<CswNbtObjClassFireClassExemptAmount> _setFireClasses()
        {
            CswNbtNode ControlZone = _CswNbtResources.Nodes.GetNode( ControlZoneId );
            CswNbtMetaDataNodeTypeProp FireClassSetNameNTP = _CswNbtResources.MetaData.getNodeTypeProp( ControlZone.NodeTypeId, "Fire Class Set Name" );
            CswPrimaryKey FCEASId = ControlZone.Properties[FireClassSetNameNTP].AsRelationship.RelatedNodeId;
            Data.FireClassExemptAmountSet = ControlZone.Properties[FireClassSetNameNTP].AsRelationship.CachedNodeName;
            if( null != FCEASId )
            {
                CswNbtMetaDataObjectClass FCEAOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.FireClassExemptAmountClass );
                foreach ( CswNbtObjClassFireClassExemptAmount FCEANode in FCEAOC.getNodes( false, false ) )
                {
                    if( FCEANode.SetName.RelatedNodeId == FCEASId )
                    {
                        FireClasses.Add(FCEANode);
                    }
                }
            }
            return FireClasses;
        }

        private IEnumerable<CswNbtObjClassFireClassExemptAmount> _getRelevantHazardClasses( CswCommaDelimitedString MaterialHazards )
        {
            Collection<CswNbtObjClassFireClassExemptAmount> RelevantHazardClasses = new Collection<CswNbtObjClassFireClassExemptAmount>();
            foreach(String Hazard in MaterialHazards)
            {
                foreach( CswNbtObjClassFireClassExemptAmount FireClassNode in FireClasses )
                {
                    if( Hazard == FireClassNode.HazardClass.Value )
                    {
                        RelevantHazardClasses.Add( FireClassNode );
                    }
                }
            }
            return RelevantHazardClasses;
        }

        private CswNbtView _getHMISView()
        {
            CswNbtView HMISView = new CswNbtView( _CswNbtResources );
            CswNbtMetaDataNodeType ControlZoneNT = _CswNbtResources.MetaData.getNodeType( "Control Zone" );
            if( null != ControlZoneNT )
            {
                CswNbtMetaDataObjectClass LocationOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.LocationClass );
                CswNbtViewRelationship LocationVR = HMISView.AddViewRelationship( LocationOC, true );

                CswNbtViewProperty ControlZoneVP = null;
                foreach( CswNbtMetaDataNodeType LocationNT in LocationOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp ControlZoneNTP = LocationNT.getNodeTypeProp( "Control Zone" );
                    if( null != ControlZoneNTP )
                    {
                        ControlZoneVP = HMISView.AddViewProperty( LocationVR, ControlZoneNTP );
                        break;
                    }
                }
                HMISView.AddViewPropertyFilter( ControlZoneVP,
                    CswNbtPropFilterSql.PropertyFilterConjunction.And,
                    CswNbtPropFilterSql.FilterResultMode.Hide,
                    CswNbtSubField.SubFieldName.NodeID,
                    CswNbtPropFilterSql.PropertyFilterMode.Equals,
                    ControlZoneId.PrimaryKey.ToString() );

                CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
                CswNbtMetaDataObjectClassProp LocationOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Location );
                CswNbtViewRelationship ContainerVR = HMISView.AddViewRelationship( LocationVR, NbtViewPropOwnerType.Second, LocationOCP, true );

                CswNbtMetaDataObjectClassProp QuantityOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Quantity );
                CswNbtViewProperty QuantityVP = HMISView.AddViewProperty( ContainerVR, QuantityOCP );
                HMISView.AddViewPropertyFilter( QuantityVP,
                    CswNbtPropFilterSql.PropertyFilterConjunction.And,
                    CswNbtPropFilterSql.FilterResultMode.Hide,
                    CswNbtSubField.SubFieldName.Value,
                    CswNbtPropFilterSql.PropertyFilterMode.GreaterThan,
                    "0" );

                CswNbtMetaDataObjectClassProp UseTypeOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.UseType );
                HMISView.AddViewProperty( ContainerVR, UseTypeOCP );

                CswNbtMetaDataObjectClassProp MaterialOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Material );
                CswNbtMetaDataObjectClass MaterialOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
                HMISView.AddViewProperty( ContainerVR, MaterialOCP );
                CswNbtViewRelationship MaterialVR = HMISView.AddViewRelationship( ContainerVR, NbtViewPropOwnerType.First, MaterialOCP, true );                

                CswNbtViewProperty HazardClassesVP = null;
                foreach( CswNbtMetaDataNodeType MaterialNT in MaterialOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp HazardClassesNTP = MaterialNT.getNodeTypeProp( "Hazard Classes" );
                    if( null != HazardClassesNTP )
                    {
                        HazardClassesVP = HMISView.AddViewProperty( MaterialVR, HazardClassesNTP );
                        break;
                    }
                }
                HMISView.AddViewPropertyFilter( HazardClassesVP,
                    CswNbtPropFilterSql.PropertyFilterConjunction.And,
                    CswNbtPropFilterSql.FilterResultMode.Hide,
                    CswNbtSubField.SubFieldName.Value,
                    CswNbtPropFilterSql.PropertyFilterMode.NotNull );

                CswNbtViewProperty SpecialFlagsVP = null;
                foreach( CswNbtMetaDataNodeType MaterialNT in MaterialOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp SpecialFlagsNTP = MaterialNT.getNodeTypeProp( "Special Flags" );
                    if( null != SpecialFlagsNTP )
                    {
                        SpecialFlagsVP = HMISView.AddViewProperty( MaterialVR, SpecialFlagsNTP );
                        break;
                    }
                }
                HMISView.AddViewPropertyFilter( SpecialFlagsVP,
                    CswNbtPropFilterSql.PropertyFilterConjunction.And,
                    CswNbtPropFilterSql.FilterResultMode.Hide,
                    CswNbtSubField.SubFieldName.Value,
                    CswNbtPropFilterSql.PropertyFilterMode.NotContains,
                    "Not Reportable" );
            }
            return HMISView;
        }

        private void _setFireClassMAQData( HMISData.HMISMaterial Material, CswNbtObjClassFireClassExemptAmount FireClass )
        {
            Material.Storage.Solid.MAQ = FireClass.StorageSolidExemptAmount.Text;
            Material.Storage.Liquid.MAQ = FireClass.StorageLiquidExemptAmount.Text;
            Material.Storage.Gas.MAQ = FireClass.StorageGasExemptAmount.Text;
            Material.Closed.Solid.MAQ = FireClass.ClosedSolidExemptAmount.Text;
            Material.Closed.Liquid.MAQ = FireClass.ClosedLiquidExemptAmount.Text;
            Material.Closed.Gas.MAQ = FireClass.ClosedGasExemptAmount.Text;
            Material.Open.Solid.MAQ = FireClass.OpenSolidExemptAmount.Text;
            Material.Open.Liquid.MAQ = FireClass.OpenLiquidExemptAmount.Text;
        }

        private void _addQuantityDataToHMISMaterial( HMISData.HMISMaterial Material, String UseType, Double Quantity, CswPrimaryKey UnitId )
        {
            CswPrimaryKey NewUnitId = _getBaseUnitId( Material.PhysicalState );
            CswNbtUnitConversion Conversion = new CswNbtUnitConversion( _CswNbtResources, UnitId, NewUnitId );
            Double ConvertedQty = Conversion.convertUnit( Quantity );
            switch( UseType )
            {
                case CswNbtObjClassContainer.UseTypes.Storage:
                    switch( Material.PhysicalState.ToLower() )
                    {
                        case CswNbtObjClassMaterial.PhysicalStates.Solid:
                            Material.Storage.Solid.Qty += ConvertedQty;
                            break;
                        case CswNbtObjClassMaterial.PhysicalStates.Liquid:
                            Material.Storage.Liquid.Qty += ConvertedQty;
                            break;
                        case CswNbtObjClassMaterial.PhysicalStates.Gas:
                            Material.Storage.Gas.Qty += ConvertedQty;
                            break;
                    }
                    break;
                case CswNbtObjClassContainer.UseTypes.Closed:
                    switch( Material.PhysicalState.ToLower() )
                    {
                        case CswNbtObjClassMaterial.PhysicalStates.Solid:
                            Material.Storage.Solid.Qty += ConvertedQty;
                            Material.Closed.Solid.Qty += ConvertedQty;
                            break;
                        case CswNbtObjClassMaterial.PhysicalStates.Liquid:
                            Material.Storage.Liquid.Qty += ConvertedQty;
                            Material.Closed.Liquid.Qty += ConvertedQty;
                            break;
                        case CswNbtObjClassMaterial.PhysicalStates.Gas:
                            Material.Storage.Gas.Qty += ConvertedQty;
                            Material.Closed.Gas.Qty += ConvertedQty;
                            break;
                    }
                    break;
                case CswNbtObjClassContainer.UseTypes.Open:
                    switch( Material.PhysicalState.ToLower() )
                    {
                        case CswNbtObjClassMaterial.PhysicalStates.Solid:
                            Material.Open.Solid.Qty += ConvertedQty;
                            break;
                        case CswNbtObjClassMaterial.PhysicalStates.Liquid:
                            Material.Open.Liquid.Qty += ConvertedQty;
                            break;
                    }
                    break;
            }
        }

        private CswPrimaryKey _getBaseUnitId( String PhysicalState )
        {
            String UnitName = "";
            switch( PhysicalState.ToLower() )
            {                
                case CswNbtObjClassMaterial.PhysicalStates.Liquid:
                    UnitName = "gal";
                    break;
                case CswNbtObjClassMaterial.PhysicalStates.Gas:
                    UnitName = "cu.ft.";
                    break;
                case CswNbtObjClassMaterial.PhysicalStates.Solid:
                case CswNbtObjClassMaterial.PhysicalStates.NA:
                default:
                    UnitName = "lb";
                    break;
            }
            CswNbtMetaDataObjectClass UoMOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.UnitOfMeasureClass );
            CswPrimaryKey BaseUnitId = null;
            foreach (CswNbtObjClassUnitOfMeasure UoMNode in UoMOC.getNodes(false, false))
            {
                if( UoMNode.Name.Text == UnitName )
                {
                    BaseUnitId = UoMNode.NodeId;
                    break;
                }
            }
            return BaseUnitId;
        }

        #endregion Private Methods
    }
}
