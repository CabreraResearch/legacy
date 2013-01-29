using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
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
            [DataMember]
            public Double MaxQty = 0.0;
            [DataMember]
            public Double AverageQty = 0.0;
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
            DataTable MaterialsTable = _getTierIIMaterials( Request );
            foreach( DataRow MaterialRow in MaterialsTable.Rows )
            {
                CswPrimaryKey BaseUnitId = CswConvert.ToPrimaryKey( "nodes_" + MaterialRow["unitid"] );
                if( null != BaseUnit && BaseUnit.NodeId != BaseUnitId )
                {
                    //Theoretically, this should never happen 
                    //(unless we decide, one day, to change the unit in which we're storing TierII quantity data)
                    BaseUnit = _CswNbtResources.Nodes.GetNode( BaseUnitId );
                }
                TierIIData.TierIIMaterial Material = new TierIIData.TierIIMaterial
                {
                    MaterialId = MaterialRow["materialid"].ToString(),
                    TradeName = MaterialRow["tradename"].ToString(),
                    CASNo = MaterialRow["casno"].ToString(),
                    MaterialType = MaterialRow["materialtype"].ToString(),
                    PhysicalState = MaterialRow["physicalstate"].ToString(),
                    EHS = MaterialRow["specialflags"].ToString().Contains("EHS"),
                    TradeSecret = MaterialRow["specialflags"].ToString().Contains( "Trade Secret" ),
                    MaxQty = CswConvert.ToDouble( MaterialRow["maxqty"] ),
                    AverageQty = CswConvert.ToDouble( MaterialRow["avgqty"] ),
                    DaysOnSite = CswConvert.ToInt32( MaterialRow["daysonsite"] ),
                    Unit = BaseUnit != null ? BaseUnit.Name.Text : "kg"
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
                  and dateadded >= to_date('{7}', 'mm/dd/yyyy')
                  and dateadded < to_date('{8}', 'mm/dd/yyyy') + 1
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
                    CswConvert.ToDbVal( DateTime.Parse( Request.StartDate ).ToShortDateString() ),
                    CswConvert.ToDbVal( DateTime.Parse( Request.EndDate ).ToShortDateString() )
                );
                CswArbitrarySelect CswArbitrarySelect = _CswNbtResources.makeCswArbitrarySelect( "Tier II Material Select", SelectText );
                TargetTable = CswArbitrarySelect.getTable();
            }
            return TargetTable;
        }

        private DataTable _getContainerStorageProps( String MaterialId, TierIIData.TierIIDataRequest Request )
        {
            DataTable TargetTable = null;
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            CswNbtMetaDataNodeType ContainerNT = ContainerOC.FirstNodeType;
            if( null != ContainerNT )
            {
                CswNbtMetaDataNodeTypeProp MaterialProp = _CswNbtResources.MetaData.getNodeTypePropByObjectClassProp( ContainerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.Material );
                CswNbtMetaDataNodeTypeProp PressureProp = _CswNbtResources.MetaData.getNodeTypePropByObjectClassProp( ContainerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.StoragePressure );
                CswNbtMetaDataNodeTypeProp TemperatureProp = _CswNbtResources.MetaData.getNodeTypePropByObjectClassProp( ContainerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.StorageTemperature );
                CswNbtMetaDataNodeTypeProp UseTypeProp = _CswNbtResources.MetaData.getNodeTypePropByObjectClassProp( ContainerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.UseType );
                String SelectText = @"with containerids
                    as (select nodeid from jct_nodes_props where nodetypepropid = " + MaterialProp.PropId + " and field1_fk = " + MaterialId + @")
                select unique codes.pressure, codes.temperature, codes.usetype from (
                    select unique jnpa.nodeid as ContainerId,
                        case when p.pressure is null 
                            then lag(p.pressure) over (order by jnpa.audittransactionid) 
                            else p.pressure end pressure,
                        case when t.temperature is null 
                            then lag(t.temperature) over (order by jnpa.audittransactionid) 
                            else t.temperature end temperature,
                        case when u.usetype is null 
                            then lag(u.usetype) over (order by jnpa.audittransactionid) 
                            else u.usetype end usetype,
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
                        and jnpa.recordcreated >= to_date('" + CswConvert.ToDbVal( DateTime.Parse( Request.StartDate ).ToShortDateString() ) + @"', 'mm/dd/yyyy')
                        and jnpa.recordcreated < to_date('" + CswConvert.ToDbVal( DateTime.Parse( Request.EndDate ).ToShortDateString() ) + @"', 'mm/dd/yyyy') + 1
                ) codes
                    where codes.pressure is not null and codes.temperature is not null and codes.usetype is not null";
                CswArbitrarySelect CswArbitrarySelect = _CswNbtResources.makeCswArbitrarySelect( "Tier II Container Props Select", SelectText );
                TargetTable = CswArbitrarySelect.getTable();
            }
            return TargetTable;
        }

        private DataTable _getContainerLocations( String MaterialId, TierIIData.TierIIDataRequest Request )
        {
            DataTable TargetTable = null;
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            CswNbtMetaDataNodeType ContainerNT = ContainerOC.FirstNodeType;
            if( null != ContainerNT )
            {
                CswNbtMetaDataNodeTypeProp MaterialProp = _CswNbtResources.MetaData.getNodeTypePropByObjectClassProp( ContainerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.Material );
                CswNbtMetaDataNodeTypeProp LocationProp = _CswNbtResources.MetaData.getNodeTypePropByObjectClassProp( ContainerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.Location );
                String SelectText = @"with containerids
                    as (select nodeid from jct_nodes_props where nodetypepropid = " + MaterialProp.PropId + " and field1_fk = " + MaterialId + @")
                select unique locationid, fulllocation from (
                    select unique jnp.field1_fk as locationid, jnp.field4 as fulllocation, jnp.recordcreated
                        from jct_nodes_props_audit jnp
                    where jnp.nodetypepropid = " + LocationProp.PropId + @"
                        and jnp.field1_fk is not null
                        and exists (select nodeid from containerids where nodeid = jnp.nodeid)
                        and jnp.recordcreated >= to_date('" + CswConvert.ToDbVal(DateTime.Parse(Request.StartDate).ToShortDateString()) + @"', 'mm/dd/yyyy')
                        and jnp.recordcreated < to_date('" + CswConvert.ToDbVal(DateTime.Parse(Request.EndDate).ToShortDateString()) + @"', 'mm/dd/yyyy') + 1
                )";
                CswArbitrarySelect CswArbitrarySelect = _CswNbtResources.makeCswArbitrarySelect( "Tier II Container Locations Select", SelectText );
                TargetTable = CswArbitrarySelect.getTable();
            }
            return TargetTable;
        }

        #endregion Private Methods
    }
}
