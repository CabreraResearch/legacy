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
            [DataMember]
            public String Material = String.Empty;
            [DataMember]
            public String PhysicalState = String.Empty;
            [DataMember]
            public String FireClass = String.Empty;
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
            [DataMember]
            public HMISQty Solid;
            [DataMember]
            public HMISQty Liquid;
        }

        [DataContract]
        public class HMISQty
        {
            [DataMember]
            public Double MAQ = 0.0;
            [DataMember]
            public Double Qty = 0.0;
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

        public CswNbtActHMISReporting( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            Data = new HMISData();
        }

        #endregion Properties and ctor

        #region Public Methods

        public HMISData getHMISData( HMISData.HMISDataRequest Request )
        {
            IEnumerable<CswNbtObjClassFireClassExemptAmount> FireClasses = _getFireClasses( Request );
            foreach( CswNbtObjClassFireClassExemptAmount FireClass in FireClasses )
            {
                CswNbtView FireClassView = _getFireClassView( Request, FireClass.FireHazardClassType.Value );
                ICswNbtTree FireClassTree = _CswNbtResources.Trees.getTreeFromView( FireClassView, false, true, false );
                if( FireClassTree.getChildNodeCount() > 0 )
                {
                    for( int i = 0; i < FireClassTree.getChildNodeCount(); i++ )//Location Nodes
                    {
                        FireClassTree.goToNthChild( i );
                        if( FireClassTree.getChildNodeCount() > 0 )
                        {
                            for( int j = 0; j < FireClassTree.getChildNodeCount(); j++ )//Container Nodes
                            {
                                FireClassTree.goToNthChild( j );
                                if( FireClassTree.getChildNodeCount() > 0 )//Material Node Exists
                                {
                                    CswNbtNode ContainerNode = FireClassTree.getNodeForCurrentPosition();//TODO - revert to CswObjClassContainer when Case 27520 is resolved
                                    if( false == String.IsNullOrEmpty( ContainerNode.Properties[CswNbtObjClassContainer.PropertyName.UseType].AsList.Value ) )
                                    {
                                        String MaterialName = ContainerNode.Properties[CswNbtObjClassContainer.PropertyName.Material].AsRelationship.PropName;
                                        bool MaterialIsNew = true;
                                        foreach ( HMISData.HMISMaterial HMISMaterial in Data.Materials.Where( HMISMaterial => HMISMaterial.Material == MaterialName ) )
                                        {
                                            MaterialIsNew = false;
                                            _addQuantityDataToHMISMaterial( HMISMaterial, ContainerNode );
                                            break;
                                        }
                                        if( MaterialIsNew )
                                        {
                                            FireClassTree.goToNthChild( 0 );
                                            HMISData.HMISMaterial HMISMaterial = new HMISData.HMISMaterial();
                                            HMISMaterial.FireClass = FireClass.FireHazardClassType.Value;
                                            HMISMaterial.Material = MaterialName;
                                            CswNbtObjClassMaterial MaterialNode = FireClassTree.getNodeForCurrentPosition();
                                            HMISMaterial.PhysicalState = MaterialNode.PhysicalState.Value;
                                            _setFireClassMAQData( HMISMaterial, FireClass );
                                            _addQuantityDataToHMISMaterial( HMISMaterial, ContainerNode );
                                            Data.Materials.Add( HMISMaterial );
                                            FireClassTree.goToParentNode();
                                        }
                                    }
                                }//Material Node Exists
                                FireClassTree.goToParentNode();
                            }//Container Nodes
                        }
                        FireClassTree.goToParentNode();
                    }//Location Nodes
                }
            }            
            return Data;
        }

        #endregion Public Methods

        #region Private Methods

        private IEnumerable<CswNbtObjClassFireClassExemptAmount> _getFireClasses( HMISData.HMISDataRequest Request )
        {
            Collection<CswNbtObjClassFireClassExemptAmount> FireClasses = new Collection<CswNbtObjClassFireClassExemptAmount>();
            CswPrimaryKey ControlZoneId = CswConvert.ToPrimaryKey( Request.ControlZoneId );
            if( null != ControlZoneId )
            {
                CswNbtNode ControlZone = _CswNbtResources.Nodes.GetNode( ControlZoneId );
                CswNbtMetaDataNodeTypeProp FireClassSetNameNTP = _CswNbtResources.MetaData.getNodeTypeProp( ControlZone.NodeTypeId, "Fire Class Set Name" );
                CswPrimaryKey FCEASId = ControlZone.Properties[FireClassSetNameNTP].AsRelationship.RelatedNodeId;
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
            }
            return FireClasses;
        }

        private CswNbtView _getFireClassView( HMISData.HMISDataRequest Request, String HazardClass )
        {
            CswNbtView HMISView = new CswNbtView( _CswNbtResources );
            CswNbtMetaDataNodeType ControlZoneNT = _CswNbtResources.MetaData.getNodeType( "Control Zone" );
            if( null != ControlZoneNT )
            {
                CswNbtMetaDataObjectClass LocationOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.LocationClass );
                CswNbtMetaDataObjectClassProp ControlZoneOCP = LocationOC.getObjectClassProp( CswNbtObjClassLocation.PropertyName.ControlZone );
                CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
                CswNbtMetaDataObjectClassProp LocationOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Location );
                CswNbtMetaDataObjectClassProp MaterialOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Material );
                CswNbtMetaDataObjectClassProp QuantityOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Quantity );

                CswNbtViewRelationship LocationVR = HMISView.AddViewRelationship( LocationOC, true );
                CswNbtViewProperty ControlZoneVP = HMISView.AddViewProperty( LocationVR, ControlZoneOCP );
                HMISView.AddViewPropertyFilter( ControlZoneVP, 
                    CswNbtPropFilterSql.PropertyFilterConjunction.And,
                    CswNbtPropFilterSql.FilterResultMode.Hide,
                    CswNbtSubField.SubFieldName.NodeID,
                    CswNbtPropFilterSql.PropertyFilterMode.Equals,
                    Request.ControlZoneId );
                CswNbtViewRelationship ContainerVR = HMISView.AddViewRelationship( LocationVR, NbtViewPropOwnerType.Second, LocationOCP, false );
                CswNbtViewProperty QuantityVP = HMISView.AddViewProperty( ContainerVR, QuantityOCP );
                HMISView.AddViewPropertyFilter( QuantityVP,
                    CswNbtPropFilterSql.PropertyFilterConjunction.And,
                    CswNbtPropFilterSql.FilterResultMode.Hide,
                    CswNbtSubField.SubFieldName.Value,
                    CswNbtPropFilterSql.PropertyFilterMode.GreaterThan,
                    "0" );
                CswNbtViewRelationship MaterialVR = HMISView.AddViewRelationship( ContainerVR, NbtViewPropOwnerType.First, MaterialOCP, false );
                CswNbtMetaDataNodeType ChemicalNT = _CswNbtResources.MetaData.getNodeType("Chemical");
                if( null != ChemicalNT )
                {
                    CswNbtMetaDataNodeTypeProp SpecialFlagsNTP = ChemicalNT.getNodeTypeProp("Special Flags");
                    if ( null != SpecialFlagsNTP )
                    {
                        CswNbtViewProperty SpecialFlagsVP = HMISView.AddViewProperty( MaterialVR, SpecialFlagsNTP );
                        HMISView.AddViewPropertyFilter( SpecialFlagsVP,
                            CswNbtPropFilterSql.PropertyFilterConjunction.And,
                            CswNbtPropFilterSql.FilterResultMode.Unknown,
                            CswNbtSubField.SubFieldName.Value,
                            CswNbtPropFilterSql.PropertyFilterMode.NotContains,
                            "Not Reportable");
                    }
                    CswNbtMetaDataNodeTypeProp HazardClassesNTP = ChemicalNT.getNodeTypeProp("Hazard Classes");
                    if ( null != HazardClassesNTP )
                    {
                        CswNbtViewProperty HazardClassesVP = HMISView.AddViewProperty( MaterialVR, HazardClassesNTP );
                        HMISView.AddViewPropertyFilter( HazardClassesVP,
                            CswNbtPropFilterSql.PropertyFilterConjunction.And,
                            CswNbtPropFilterSql.FilterResultMode.Unknown,
                            CswNbtSubField.SubFieldName.Value,
                            CswNbtPropFilterSql.PropertyFilterMode.Contains,
                            HazardClass);
                    }
                }
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
                    switch( Material.PhysicalState )
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
                    switch( Material.PhysicalState )
                    {
                        case CswNbtObjClassMaterial.PhysicalStates.Solid:
                            Material.Closed.Solid.Qty += ConvertedQty;
                            break;
                        case CswNbtObjClassMaterial.PhysicalStates.Liquid:
                            Material.Closed.Liquid.Qty += ConvertedQty;
                            break;
                        case CswNbtObjClassMaterial.PhysicalStates.Gas:
                            Material.Closed.Gas.Qty += ConvertedQty;
                            break;
                    }
                    break;
                case CswNbtObjClassContainer.UseTypes.Open:
                    switch( Material.PhysicalState )
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
            switch( PhysicalState )
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
            CswPrimaryKey BaseUnitId = ( 
                from CswNbtObjClassUnitOfMeasure UoMNode 
                    in UoMOC.getNodes( false, false ) 
                where UoMNode.Name.Text == UnitName 
                select UoMNode.NodeId 
                ).FirstOrDefault();
            return BaseUnitId;
        }

        #endregion Private Methods
    }
}
