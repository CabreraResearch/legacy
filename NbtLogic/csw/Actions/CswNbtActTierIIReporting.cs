﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.csw.Conversion;

namespace ChemSW.Nbt.Actions
{
    #region DataContract

    [DataContract]
    public class TierIIData
    {
        public TierIIData()
        {
            Materials = new Collection<TierIIMaterial>();
        }

        [DataMember]
        public Collection<TierIIMaterial> Materials;

        [DataContract]
        public class TierIIMaterial
        {
            public TierIIMaterial()
            {
                Storage = new Collection<StorageCodes>();
                Locations = new Collection<StorageLocations>();
                HazardCategories = new Collection<String>();
            }

            //Internal
            [DataMember]
            public String MaterialId = String.Empty;
            //Chemical Description
            [DataMember]
            public String TradeName = String.Empty;
            [DataMember]
            public String CASNo = String.Empty;
            [DataMember]
            public String MaterialType = String.Empty;//Pure,Mixture
            [DataMember]
            public String PhysicalState = String.Empty;//Solid,Liquid,Gas
            [DataMember]
            public bool EHS = false;
            [DataMember]
            public bool TradeSecret = false;
            //Physical and Health Hazards
            [DataMember]
            public Collection<String> HazardCategories;//Fire,Pressure,Reactive,Immediate,Delayed
            //Inventory
            private Int32 Precision = 6;
            private Double _MaxQty;
            [DataMember]
            public Double MaxQty
            {
                get { return _MaxQty; }
                set { _MaxQty = CswTools.IsDouble( value ) ? Math.Round( value, Precision, MidpointRounding.AwayFromZero ) : 0.0; }
            }
            private Double _AvgQty;
            [DataMember]
            public Double AverageQty
            {
                get { return _AvgQty; }
                set { _AvgQty = CswTools.IsDouble( value ) ? Math.Round( value, Precision, MidpointRounding.AwayFromZero ) : 0.0; }
            }
            [DataMember]
            public Int32 DaysOnSite = 0;
            [DataMember]
            public String Unit = String.Empty;
            //Storage Codes and Locations
            [DataMember]
            public Collection<StorageCodes> Storage;
            [DataMember]
            public Collection<StorageLocations> Locations;
        }

        [DataContract]
        public class StorageCodes
        {
            [DataMember]
            public String UseType = String.Empty;//Storage,Closed,Open
            [DataMember]
            public String Pressure = String.Empty;//Atmos,Subatmos,Pressurized
            [DataMember]
            public String Temperature = String.Empty;//RT,>RT,<RT,Cryogenic
        }

        [DataContract]
        public class StorageLocations
        {
            [DataMember]
            public String LocationId = String.Empty;
            [DataMember]
            public String Location = String.Empty;
        }

        [DataContract]
        public class TierIIDataRequest
        {
            [DataMember]
            public String LocationId = String.Empty;
            [DataMember]
            public String StartDate = String.Empty;
            [DataMember]
            public String EndDate = String.Empty;
        }
    }

    #endregion DataContract

    public class CswNbtActTierIIReporting
    {
        #region Properties and ctor

        private CswNbtResources _CswNbtResources;
        private TierIIData Data;
        private CswNbtObjClassUnitOfMeasure BaseUnit;
        private CswCommaDelimitedString LocationIds;

        public CswNbtActTierIIReporting( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            Data = new TierIIData();
        }

        #endregion Properties and ctor

        #region Public Methods

        public TierIIData getTierIIData( TierIIData.TierIIDataRequest Request )
        {
            BaseUnit = _setBaseUnit( "kg", "Unit (Weight)" );
            CswNbtObjClassUnitOfMeasure PoundsUnit = _setBaseUnit( "lb", "Unit (Weight)" );
            CswNbtUnitConversion Conversion = ( BaseUnit != null && PoundsUnit != null ) ?
                new CswNbtUnitConversion( _CswNbtResources, BaseUnit.NodeId, PoundsUnit.NodeId ) : 
                new CswNbtUnitConversion();
            LocationIds = _setLocationIds( Request.LocationId );
            DataTable MaterialsTable = _getTierIIMaterials( Request );
            foreach( DataRow MaterialRow in MaterialsTable.Rows )
            {
                CswPrimaryKey BaseUnitId = CswConvert.ToPrimaryKey( "nodes_" + MaterialRow["unitid"] );
                if( null != BaseUnit && BaseUnit.NodeId != BaseUnitId )
                {
                    //Theoretically, this should never happen 
                    //(unless we decide, one day, to change the unit in which we're storing TierII quantity data)
                    BaseUnit = _CswNbtResources.Nodes.GetNode( BaseUnitId );
                    Conversion.setOldUnitProps( BaseUnit );
                }
                Double MaxQty = Conversion.convertUnit( CswConvert.ToDouble( MaterialRow["maxqty"] ) );
                Double AverageQty = Conversion.convertUnit( CswConvert.ToDouble( MaterialRow["maxqty"] ) );

                TierIIData.TierIIMaterial Material = new TierIIData.TierIIMaterial
                {
                    MaterialId = MaterialRow["materialid"].ToString(),
                    TradeName = MaterialRow["tradename"].ToString(),
                    CASNo = MaterialRow["casno"].ToString(),
                    MaterialType = MaterialRow["materialtype"].ToString(),
                    PhysicalState = MaterialRow["physicalstate"].ToString(),
                    EHS = MaterialRow["specialflags"].ToString().Contains("EHS"),
                    TradeSecret = MaterialRow["specialflags"].ToString().Contains( "Trade Secret" ),
                    MaxQty = MaxQty,
                    AverageQty = AverageQty,
                    DaysOnSite = CswConvert.ToInt32( MaterialRow["daysonsite"] ),
                    Unit = PoundsUnit != null ? PoundsUnit.Name.Text : "lb"
                };
                CswCommaDelimitedString Hazards = new CswCommaDelimitedString();
                Hazards.FromString( MaterialRow["hazardcategories"].ToString() );
                foreach( String Hazard in Hazards )
                {
                    Material.HazardCategories.Add( Hazard );
                }
                DataTable ContainerStorageCodesTable = _getContainerStorageProps( Material.MaterialId, Request );
                foreach( DataRow ContainerPropsRow in ContainerStorageCodesTable.Rows )
                {
                    TierIIData.StorageCodes StorageCodes = new TierIIData.StorageCodes
                    {
                        Pressure = ContainerPropsRow["pressure"].ToString(),
                        Temperature = ContainerPropsRow["temperature"].ToString(),
                        UseType = ContainerPropsRow["usetype"].ToString()
                    };
                    Material.Storage.Add( StorageCodes );
                }
                DataTable ContainerLocationsTable = _getContainerLocations( Material.MaterialId, Request );
                foreach( DataRow ContainerLocsRow in ContainerLocationsTable.Rows )
                {
                    TierIIData.StorageLocations Location = new TierIIData.StorageLocations
                    {
                        LocationId = ContainerLocsRow["locationid"].ToString(),
                        Location = ContainerLocsRow["fulllocation"].ToString()
                    };
                    Material.Locations.Add( Location );
                }
                Data.Materials.Add( Material );
            }
                                        
            return Data;
        }

        #endregion Public Methods

        #region Private Methods

        private CswNbtObjClassUnitOfMeasure _setBaseUnit( String UnitName, String NodeTypeName )
        {
            CswNbtObjClassUnitOfMeasure Unit = null;
            CswNbtMetaDataNodeType WeightNT = _CswNbtResources.MetaData.getNodeType( NodeTypeName );
            if( null != WeightNT )
            {
                foreach( CswNbtObjClassUnitOfMeasure WeightNode in WeightNT.getNodes( false, false ) )
                {
                    if (UnitName == WeightNode.Name.Text)
                    {
                        Unit = WeightNode;
                    }
                }
            }
            return Unit;
        }

        private DataTable _getTierIIMaterials( TierIIData.TierIIDataRequest Request )
        {
            #region SQL Query Template
        
            String SqlText = @"
              select t.materialid, t.casno, max(t.totalquantity) as maxqty, round(avg(t.totalquantity), 6) as avgqty, t.unitid, count(*) as daysonsite, 
                p.tradename, p.materialtype, p.physicalstate, p.specialflags, p.hazardcategories, p.istierII
                from tier2 t
                  left join (select 
                  n.nodeid as MaterialId,
                  (select jnp.field1 as materialid from jct_nodes_props jnp 
                    where n.nodeid = jnp.nodeid and jnp.nodetypepropid = {0}) as Tradename,
                  (select jnp.field1 as materialid from jct_nodes_props jnp 
                    where n.nodeid = jnp.nodeid and jnp.nodetypepropid = {1}) as MaterialType,
                  (select jnp.field1 as materialid from jct_nodes_props jnp 
                    where n.nodeid = jnp.nodeid and jnp.nodetypepropid = {2}) as PhysicalState,
                  (select dbms_lob.substr(jnp.gestalt) as materialid from jct_nodes_props jnp 
                    where n.nodeid = jnp.nodeid and jnp.nodetypepropid = {3}) as SpecialFlags, 
                  (select dbms_lob.substr(jnp.gestalt) as materialid from jct_nodes_props jnp 
                    where n.nodeid = jnp.nodeid and jnp.nodetypepropid = {4}) as HazardCategories,
                  (select jnp.field1 as istierII from jct_nodes_props jnp 
                    where n.nodeid = jnp.nodeid and jnp.nodetypepropid = {5}) as IsTierII
                from nodes n) p on p.materialid = t.materialid
                where locationid = {6} 
                  and istierii = 1
                  and casno is not null
                  and dateadded >= {7}
                  and dateadded < {8} + 1
                  group by t.materialid, t.casno, t.unitid, 
                    p.tradename, p.materialtype, p.physicalstate, p.specialflags, p.hazardcategories, p.istierII";

            #endregion SQL Query Template

            DataTable TargetTable = null;
            CswNbtMetaDataNodeType ChemicalNT = _CswNbtResources.MetaData.getNodeType( "Chemical" );
            if( null != ChemicalNT )
            {
                CswNbtMetaDataNodeTypeProp TradeNameProp = _CswNbtResources.MetaData.getNodeTypePropByObjectClassProp( ChemicalNT.NodeTypeId, CswNbtObjClassMaterial.PropertyName.Tradename );
                CswNbtMetaDataNodeTypeProp MaterialTypeProp = _CswNbtResources.MetaData.getNodeTypeProp( ChemicalNT.NodeTypeId, "Material Type" );
                CswNbtMetaDataNodeTypeProp PhysicalStateProp = _CswNbtResources.MetaData.getNodeTypePropByObjectClassProp( ChemicalNT.NodeTypeId, CswNbtObjClassMaterial.PropertyName.PhysicalState );
                CswNbtMetaDataNodeTypeProp SpecialFlagsProp = _CswNbtResources.MetaData.getNodeTypeProp( ChemicalNT.NodeTypeId, "Special Flags" );
                CswNbtMetaDataNodeTypeProp HazardCategoriesProp = _CswNbtResources.MetaData.getNodeTypeProp( ChemicalNT.NodeTypeId, "Hazard Categories" );
                CswNbtMetaDataNodeTypeProp IsTierIIProp = _CswNbtResources.MetaData.getNodeTypePropByObjectClassProp( ChemicalNT.NodeTypeId, CswNbtObjClassMaterial.PropertyName.IsTierII );
                String SelectText = String.Format( SqlText,
                    TradeNameProp.PropId,
                    null != MaterialTypeProp ? MaterialTypeProp.PropId : 0,
                    PhysicalStateProp.PropId,
                    null != SpecialFlagsProp ? SpecialFlagsProp.PropId : 0,
                    null != HazardCategoriesProp ? HazardCategoriesProp.PropId : 0,
                    IsTierIIProp.PropId,
                    CswConvert.ToPrimaryKey( Request.LocationId ).PrimaryKey,
                    _CswNbtResources.getDbNativeDate( DateTime.Parse( Request.StartDate ) ),
                    _CswNbtResources.getDbNativeDate( DateTime.Parse( Request.EndDate ) )
                );
                CswArbitrarySelect CswArbitrarySelect = _CswNbtResources.makeCswArbitrarySelect( "Tier II Material Select", SelectText );
                TargetTable = CswArbitrarySelect.getTable();
            }
            return TargetTable;
        }

        private DataTable _getContainerStorageProps( String MaterialId, TierIIData.TierIIDataRequest Request )
        {
            DataTable TargetTable = null;
            CswNbtMetaDataObjectClass MaterialComponentOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.MaterialComponentClass );
            CswNbtMetaDataNodeType MaterialComponentNT = MaterialComponentOC.FirstNodeType;
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            CswNbtMetaDataNodeType ContainerNT = ContainerOC.FirstNodeType;
            if( null != ContainerNT && null != MaterialComponentNT )
            {
                CswNbtMetaDataNodeTypeProp MixtureProp = _CswNbtResources.MetaData.getNodeTypePropByObjectClassProp( MaterialComponentNT.NodeTypeId, CswNbtObjClassMaterialComponent.PropertyName.Mixture );
                CswNbtMetaDataNodeTypeProp ConstituentProp = _CswNbtResources.MetaData.getNodeTypePropByObjectClassProp( MaterialComponentNT.NodeTypeId, CswNbtObjClassMaterialComponent.PropertyName.Constituent );
                CswNbtMetaDataNodeTypeProp MaterialProp = _CswNbtResources.MetaData.getNodeTypePropByObjectClassProp( ContainerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.Material );
                CswNbtMetaDataNodeTypeProp PressureProp = _CswNbtResources.MetaData.getNodeTypePropByObjectClassProp( ContainerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.StoragePressure );
                CswNbtMetaDataNodeTypeProp TemperatureProp = _CswNbtResources.MetaData.getNodeTypePropByObjectClassProp( ContainerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.StorageTemperature );
                CswNbtMetaDataNodeTypeProp UseTypeProp = _CswNbtResources.MetaData.getNodeTypePropByObjectClassProp( ContainerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.UseType );
                CswNbtMetaDataNodeTypeProp QuantityProp = _CswNbtResources.MetaData.getNodeTypePropByObjectClassProp( ContainerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.Quantity );
                String SelectText =
                @"with containerids as (
                    select n.nodeid 
                    from jct_nodes_props n
                    left join (select jnp.nodeid, jnp.field1_numeric as quantity
                        from jct_nodes_props jnp
                        where jnp.nodetypepropid = " + QuantityProp.PropId + @") q
                        on n.nodeid = q.nodeid
                    where nodetypepropid = " + MaterialProp.PropId + @" 
                        and q.quantity > 0 
                        and field1_fk in 
                            (select field1_fk as materials from jct_nodes_props where nodetypepropid = " + MixtureProp.PropId + @" and nodeid in 
                            (select nodeid from jct_nodes_props where nodetypepropid = " + ConstituentProp.PropId + @" and field1_fk = " + MaterialId + @")
                                union (select " + MaterialId + @" from dual) ) )
                select unique pressure, temperature, usetype from (
                    select props.* from (
                        select codes.*, dense_rank() over(partition by codes.containerid order by recordcreated desc) rank from (
                            select unique jnpa.nodeid as ContainerId,
                                last_value(p.pressure ignore nulls) OVER (ORDER BY jnpa.audittransactionid) pressure,
                                last_value(t.temperature ignore nulls) OVER (ORDER BY jnpa.audittransactionid) temperature,
                                last_value(u.usetype ignore nulls) OVER (ORDER BY jnpa.audittransactionid) usetype,
                                jnpa.audittransactionid,
                                jnpa.recordcreated
                            from jct_nodes_props_audit jnpa
                            left join (select jnp.nodeid, jnp.field1 as pressure, jnp.audittransactionid
                                from jct_nodes_props_audit jnp
                                where jnp.nodetypepropid = " + PressureProp.PropId + @") p 
                                on jnpa.nodeid = p.nodeid and jnpa.audittransactionid = p.audittransactionid
                            left join (select jnp.nodeid, jnp.field1 as temperature, jnp.audittransactionid
                                from jct_nodes_props_audit jnp
                                where jnp.nodetypepropid = " + TemperatureProp.PropId + @") t 
                                on jnpa.nodeid = t.nodeid and jnpa.audittransactionid = t.audittransactionid
                            left join (select jnp.nodeid, jnp.field1 as usetype, jnp.audittransactionid
                                from jct_nodes_props_audit jnp
                                where jnp.nodetypepropid = " + UseTypeProp.PropId + @") u 
                                on jnpa.nodeid = u.nodeid and jnpa.audittransactionid = u.audittransactionid
                            where exists (select nodeid from containerids where nodeid = jnpa.nodeid)
                            order by containerid, audittransactionid
                        ) codes 
                    ) props
                    where props.rank=1
                        and props.recordcreated < " + _CswNbtResources.getDbNativeDate(DateTime.Parse(Request.EndDate)) + @" + 1
                ) union (
                    select unique 
                        last_value(p.pressure ignore nulls) OVER (ORDER BY jnpa.jctnodepropid) pressure,
                        last_value(t.temperature ignore nulls) OVER (ORDER BY jnpa.jctnodepropid) temperature,
                        last_value(u.usetype ignore nulls) OVER (ORDER BY jnpa.jctnodepropid) usetype
                    from jct_nodes_props jnpa
                    left join (select jnp.nodeid, jnp.field1 as pressure
                        from jct_nodes_props jnp
                        where jnp.nodetypepropid = " + PressureProp.PropId + @") p 
                        on jnpa.nodeid = p.nodeid
                    left join (select jnp.nodeid, jnp.field1 as temperature
                        from jct_nodes_props jnp
                        where jnp.nodetypepropid = " + TemperatureProp.PropId + @") t 
                        on jnpa.nodeid = t.nodeid
                    left join (select jnp.nodeid, jnp.field1 as usetype
                        from jct_nodes_props jnp
                        where jnp.nodetypepropid = " + UseTypeProp.PropId + @") u 
                        on jnpa.nodeid = u.nodeid
                    where exists (select nodeid from containerids where nodeid = jnpa.nodeid)
                )";

                CswArbitrarySelect CswArbitrarySelect = _CswNbtResources.makeCswArbitrarySelect( "Tier II Container Props Select", SelectText );
                TargetTable = CswArbitrarySelect.getTable();
            }
            return TargetTable;
        }

        private DataTable _getContainerLocations( String MaterialId, TierIIData.TierIIDataRequest Request )
        {
            DataTable TargetTable = null;
            CswNbtMetaDataObjectClass MaterialComponentOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.MaterialComponentClass );
            CswNbtMetaDataNodeType MaterialComponentNT = MaterialComponentOC.FirstNodeType;
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            CswNbtMetaDataNodeType ContainerNT = ContainerOC.FirstNodeType;
            if( null != ContainerNT && null != MaterialComponentNT )
            {
                CswNbtMetaDataNodeTypeProp MixtureProp = _CswNbtResources.MetaData.getNodeTypePropByObjectClassProp( MaterialComponentNT.NodeTypeId, CswNbtObjClassMaterialComponent.PropertyName.Mixture );
                CswNbtMetaDataNodeTypeProp ConstituentProp = _CswNbtResources.MetaData.getNodeTypePropByObjectClassProp( MaterialComponentNT.NodeTypeId, CswNbtObjClassMaterialComponent.PropertyName.Constituent );

                CswNbtMetaDataNodeTypeProp MaterialProp = _CswNbtResources.MetaData.getNodeTypePropByObjectClassProp( ContainerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.Material );
                CswNbtMetaDataNodeTypeProp LocationProp = _CswNbtResources.MetaData.getNodeTypePropByObjectClassProp( ContainerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.Location );
                CswNbtMetaDataNodeTypeProp QuantityProp = _CswNbtResources.MetaData.getNodeTypePropByObjectClassProp( ContainerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.Quantity );
                String SelectText = 
                @"with containerids as (
                    select n.nodeid 
                    from jct_nodes_props n
                    left join (select jnp.nodeid, jnp.field1_numeric as quantity
                        from jct_nodes_props jnp
                        where jnp.nodetypepropid = " + QuantityProp.PropId + @") q
                        on n.nodeid = q.nodeid
                    where nodetypepropid = " + MaterialProp.PropId + @" 
                        and q.quantity > 0 
                        and field1_fk in 
                            (select field1_fk as materials from jct_nodes_props where nodetypepropid = " + MixtureProp.PropId + @" and nodeid in 
                            (select nodeid from jct_nodes_props where nodetypepropid = " + ConstituentProp.PropId + @" and field1_fk = " + MaterialId + @")
                                union (select " + MaterialId + @" from dual) ) )
                select unique locationid, fulllocation from (
                    (select unique jnp.field1_fk as locationid, jnp.field4 as fulllocation
                        from jct_nodes_props_audit jnp
                        where jnp.nodetypepropid = " + LocationProp.PropId + @"
                            and exists (select nodeid from containerids where nodeid = jnp.nodeid)
                            and jnp.recordcreated < " + _CswNbtResources.getDbNativeDate( DateTime.Parse( Request.EndDate ) ) + @" + 1)
                    union
                    (select unique jnp.field1_fk as locationid, jnp.field4 as fulllocation
                        from jct_nodes_props jnp
                        where jnp.nodetypepropid = " + LocationProp.PropId + @"
                            and exists (select nodeid from containerids where nodeid = jnp.nodeid))
                ) where locationid in (" + LocationIds + @") and fulllocation is not null";

                CswArbitrarySelect CswArbitrarySelect = _CswNbtResources.makeCswArbitrarySelect( "Tier II Container Locations Select", SelectText );
                TargetTable = CswArbitrarySelect.getTable();
            }
            return TargetTable;
        }

        private CswCommaDelimitedString _setLocationIds( String LocationId )
        {
            CswCommaDelimitedString LocationIdCDS = new CswCommaDelimitedString();
            IEnumerable<CswPrimaryKey> LocationPKs = _getLocationIds( LocationId );
            foreach(CswPrimaryKey LocationPK in LocationPKs)
            {
                LocationIdCDS.Add( LocationPK.PrimaryKey.ToString() );
            }
            return LocationIdCDS;
        }

        private IEnumerable<CswPrimaryKey> _getLocationIds( String LocationId )
        {
            Collection<CswPrimaryKey> LocationPKs = new Collection<CswPrimaryKey>();
            CswPrimaryKey RootLocationId = CswConvert.ToPrimaryKey( LocationId );
            if( null != RootLocationId )
            {
                CswNbtView LocationTreeView = CswNbtNodePropLocation.LocationPropertyView( _CswNbtResources, null );
                ICswNbtTree LocationTree = _CswNbtResources.Trees.getTreeFromView( LocationTreeView, false, true, false );
                _addChildLocationIds( RootLocationId, LocationTree, LocationPKs );
            }
            return LocationPKs;
        }

        private void _addChildLocationIds( CswPrimaryKey LocationId, ICswNbtTree LocationTree, Collection<CswPrimaryKey> LocationIds )
        {
            LocationIds.Add( LocationId );
            LocationTree.makeNodeCurrent( LocationId );
            if( LocationTree.getChildNodeCount() > 0 )
            {
                for( int i = 0; i < LocationTree.getChildNodeCount(); i++ )
                {
                    LocationTree.goToNthChild( i );
                    _addChildLocationIds( LocationTree.getNodeIdForCurrentPosition(), LocationTree, LocationIds );
                    LocationTree.goToParentNode();
                }
            }
        }

        #endregion Private Methods
    }
}
