using System;
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
            private Double _MAQ;
            private Double _Qty;
            [DataMember]
            public Double MAQ
            {
                get { return _MAQ; }
                set { _MAQ = CswTools.IsDouble( value ) ? Math.Round( value, Precision, MidpointRounding.AwayFromZero ) : 0.0; }
            }
            [DataMember]
            public Double Qty
            {
                get { return _Qty; }
                set { _Qty = CswTools.IsDouble( value ) ? Math.Round( value, 6, MidpointRounding.AwayFromZero ) : 0.0; }
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
            if( HMISTree.getChildNodeCount() > 0 )
            {
                for( int i = 0; i < HMISTree.getChildNodeCount(); i++ )//Location Nodes
                {
                    HMISTree.goToNthChild( i );
                    if( HMISTree.getChildNodeCount() > 0 )
                    {
                        for( int j = 0; j < HMISTree.getChildNodeCount(); j++ )//Container Nodes
                        {
                            HMISTree.goToNthChild( j );
                            if( HMISTree.getChildNodeCount() > 0 )//Material Node Exists
                            {
                                CswNbtNode ContainerNode = HMISTree.getNodeForCurrentPosition();//TODO - revert to CswObjClassContainer when Case 27520 is resolved
                                if( false == String.IsNullOrEmpty( ContainerNode.Properties[CswNbtObjClassContainer.PropertyName.UseType].AsList.Value ) )
                                {
                                    String MaterialName = ContainerNode.Properties[CswNbtObjClassContainer.PropertyName.Material].AsRelationship.CachedNodeName;
                                    bool MaterialIsNew = true;
                                    foreach ( HMISData.HMISMaterial HMISMaterial in Data.Materials.Where( HMISMaterial => HMISMaterial.Material == MaterialName ) )
                                    {
                                        MaterialIsNew = false;
                                        _addQuantityDataToHMISMaterial( HMISMaterial, ContainerNode );
                                    }
                                    if( MaterialIsNew )
                                    {
                                        HMISTree.goToNthChild( 0 );
                                        CswNbtObjClassMaterial MaterialNode = HMISTree.getNodeForCurrentPosition();
                                        CswNbtMetaDataNodeTypeProp HazardClassesNTP = _CswNbtResources.MetaData.getNodeTypeProp( MaterialNode.NodeTypeId, "Hazard Classes" );
                                        IEnumerable<CswNbtObjClassFireClassExemptAmount> HazardClasses = _getRelevantHazardClasses( MaterialNode.Node.Properties[HazardClassesNTP].AsMultiList.Value );
                                        foreach (CswNbtObjClassFireClassExemptAmount HazardClass in HazardClasses)
                                        {
                                            HMISData.HMISMaterial HMISMaterial = new HMISData.HMISMaterial();
                                            HMISMaterial.Material = MaterialName;
                                            HMISMaterial.HazardClass = HazardClass.FireHazardClassType.Value;
                                            HMISMaterial.PhysicalState = MaterialNode.PhysicalState.Value;
                                            _setFireClassMAQData( HMISMaterial, HazardClass );
                                            _addQuantityDataToHMISMaterial( HMISMaterial, ContainerNode );
                                            Data.Materials.Add( HMISMaterial );
                                        }
                                        HMISTree.goToParentNode();
                                    }
                                }
                            }//Material Node Exists
                            HMISTree.goToParentNode();
                        }//Container Nodes
                    }
                    HMISTree.goToParentNode();
                }//Location Nodes
            }           
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
                    if( Hazard == FireClassNode.FireHazardClassType.Value )
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

                CswNbtMetaDataObjectClassProp MaterialOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Material );
                CswNbtMetaDataObjectClass MaterialClass = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
                CswNbtViewRelationship MaterialVR = HMISView.AddViewRelationship( ContainerVR, NbtViewPropOwnerType.First, MaterialOCP, true );                

                CswNbtViewProperty HazardClassesVP = null;
                foreach( CswNbtMetaDataNodeType MaterialNT in MaterialClass.getNodeTypes() )
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
                foreach( CswNbtMetaDataNodeType MaterialNT in MaterialClass.getNodeTypes() )
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
            Material.Storage.Solid.MAQ = FireClass.StorageSolidExemptAmount.Quantity;
            Material.Storage.Liquid.MAQ = FireClass.StorageLiquidExemptAmount.Quantity;
            Material.Storage.Gas.MAQ = FireClass.StorageGasExemptAmount.Quantity;
            Material.Closed.Solid.MAQ = FireClass.ClosedSolidExemptAmount.Quantity;
            Material.Closed.Liquid.MAQ = FireClass.ClosedLiquidExemptAmount.Quantity;
            Material.Closed.Gas.MAQ = FireClass.ClosedGasExemptAmount.Quantity;
            Material.Open.Solid.MAQ = FireClass.OpenSolidExemptAmount.Quantity;
            Material.Open.Liquid.MAQ = FireClass.OpenLiquidExemptAmount.Quantity;
        }

        private void _addQuantityDataToHMISMaterial( HMISData.HMISMaterial Material, CswNbtObjClassContainer Container )
        {
            CswPrimaryKey NewUnitId = _getBaseUnitId( Material.PhysicalState );
            CswNbtUnitConversion Conversion = new CswNbtUnitConversion( _CswNbtResources, Container.Quantity.UnitId, NewUnitId );
            Double ConvertedQty = Conversion.convertUnit( Container.Quantity.Quantity );
            switch( Container.UseType.Value )
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
                case CswNbtObjClassMaterial.PhysicalStates.Solid:
                case CswNbtObjClassMaterial.PhysicalStates.NA:
                    UnitName = "lb";
                    break;
                case CswNbtObjClassMaterial.PhysicalStates.Liquid:
                    UnitName = "gal";
                    break;
                case CswNbtObjClassMaterial.PhysicalStates.Gas:
                    UnitName = "cu.ft.";
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
