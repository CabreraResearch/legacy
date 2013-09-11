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
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Conversion;

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

            [DataMember] 
            public String MaxQtyRangeCode = String.Empty;
            private Double _AvgQty;
            [DataMember]
            public Double AverageQty
            {
                get { return _AvgQty; }
                set { _AvgQty = CswTools.IsDouble( value ) ? Math.Round( value, Precision, MidpointRounding.AwayFromZero ) : 0.0; }
            }
            [DataMember]
            public String AverageQtyRangeCode = String.Empty;
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
        private Collection<TierIIRangeCode> RangeCodes;

        public CswNbtActTierIIReporting( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            Data = new TierIIData();
            _setDefaultRangeCodes();
        }

        #endregion Properties and ctor

        #region Public Methods

        public TierIIData getTierIIData( TierIIData.TierIIDataRequest Request )
        {
            BaseUnit = _setBaseUnit( "kg", "Unit_Weight" );
            CswNbtObjClassUnitOfMeasure PoundsUnit = _setBaseUnit( "lb", "Unit_Weight" );
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
                Double MaxQty = Math.Round( Conversion.convertUnit( CswConvert.ToDouble( MaterialRow["maxqty"] ) ), 3 );
                Double AverageQty = Math.Round( Conversion.convertUnit( CswConvert.ToDouble( MaterialRow["maxqty"] ) ), 3 );

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
                    MaxQtyRangeCode = _getRangeCode( MaxQty ),
                    AverageQty = AverageQty,
                    AverageQtyRangeCode = _getRangeCode( AverageQty ),
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
            #region SQL Query
        
            String SqlText = @"
              select t.materialid, t.casno, max(t.totalquantity) as maxqty, round(avg(t.totalquantity), 6) as avgqty, t.unitid, count(*) as daysonsite, 
                p.tradename, p.materialtype, p.physicalstate, p.specialflags, p.hazardcategories, p.istierII
                from tier2 t
                  left join (select 
                  n.nodeid as MaterialId,
                  (select jnp.field1 from jct_nodes_props jnp 
                    inner join nodetype_props ntp on jnp.nodetypepropid = ntp.nodetypepropid
                    inner join object_class_props ocp on ntp.objectclasspropid = ocp.objectclasspropid
                    where n.nodeid = jnp.nodeid and ocp.objectclasspropid = :tradenameid) as Tradename,
                  (select jnp.field1 from jct_nodes_props jnp 
                    inner join nodetype_props ntp on jnp.nodetypepropid = ntp.nodetypepropid
                    inner join object_class_props ocp on ntp.objectclasspropid = ocp.objectclasspropid
                    where n.nodeid = jnp.nodeid and ocp.objectclasspropid = :materialtypeid) as MaterialType,
                  (select jnp.field1 from jct_nodes_props jnp 
                    inner join nodetype_props ntp on jnp.nodetypepropid = ntp.nodetypepropid
                    inner join object_class_props ocp on ntp.objectclasspropid = ocp.objectclasspropid
                    where n.nodeid = jnp.nodeid and ocp.objectclasspropid = :physicalstateid) as PhysicalState,
                  (select dbms_lob.substr(jnp.gestalt) from jct_nodes_props jnp 
                    inner join nodetype_props ntp on jnp.nodetypepropid = ntp.nodetypepropid
                    inner join object_class_props ocp on ntp.objectclasspropid = ocp.objectclasspropid
                    where n.nodeid = jnp.nodeid and ocp.objectclasspropid = :specialflagsid) as SpecialFlags, 
                  (select dbms_lob.substr(jnp.gestalt) from jct_nodes_props jnp 
                    inner join nodetype_props ntp on jnp.nodetypepropid = ntp.nodetypepropid
                    inner join object_class_props ocp on ntp.objectclasspropid = ocp.objectclasspropid
                    where n.nodeid = jnp.nodeid and ocp.objectclasspropid = :hazardcategoriesid) as HazardCategories,
                  (select jnp.field1 from jct_nodes_props jnp
                    inner join nodetype_props ntp on jnp.nodetypepropid = ntp.nodetypepropid
                    inner join object_class_props ocp on ntp.objectclasspropid = ocp.objectclasspropid
                    where n.nodeid = jnp.nodeid and ocp.objectclasspropid = :istier2id) as IsTierII
                from nodes n) p on p.materialid = t.materialid
                where locationid = :locationid
                  and istierii = 1
                  and casno is not null
                  and dateadded >= " + _CswNbtResources.getDbNativeDate( DateTime.Parse( Request.StartDate ) ) + @"
                  and dateadded < " + _CswNbtResources.getDbNativeDate( DateTime.Parse( Request.EndDate ) ) + @" + 1
                  group by t.materialid, t.casno, t.unitid, 
                    p.tradename, p.materialtype, p.physicalstate, p.specialflags, p.hazardcategories, p.istierII";

            #endregion SQL Query

            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtMetaDataObjectClassProp TradeNameProp = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.TradeName );
            CswNbtMetaDataObjectClassProp MaterialTypeProp = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.MaterialType );
            CswNbtMetaDataObjectClassProp PhysicalStateProp = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.PhysicalState );
            CswNbtMetaDataObjectClassProp SpecialFlagsProp = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.SpecialFlags );
            CswNbtMetaDataObjectClassProp HazardCategoriesProp = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.HazardCategories );
            CswNbtMetaDataObjectClassProp IsTierIIProp = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.IsTierII );

            CswArbitrarySelect TierIIMaterialsSelect = _CswNbtResources.makeCswArbitrarySelect( "Tier II Material Select", SqlText );
            TierIIMaterialsSelect.addParameter( "tradenameid", TradeNameProp.PropId.ToString() );
            TierIIMaterialsSelect.addParameter( "materialtypeid", MaterialTypeProp.PropId.ToString() );
            TierIIMaterialsSelect.addParameter( "physicalstateid", PhysicalStateProp.PropId.ToString() );
            TierIIMaterialsSelect.addParameter( "specialflagsid", SpecialFlagsProp.PropId.ToString() );
            TierIIMaterialsSelect.addParameter( "hazardcategoriesid", HazardCategoriesProp.PropId.ToString() );
            TierIIMaterialsSelect.addParameter( "istier2id", IsTierIIProp.PropId.ToString() );
            TierIIMaterialsSelect.addParameter( "locationid", CswConvert.ToPrimaryKey( Request.LocationId ).PrimaryKey.ToString() );
            DataTable TargetTable = TierIIMaterialsSelect.getTable();
            return TargetTable;
        }

        private DataTable _getContainerStorageProps( String MaterialId, TierIIData.TierIIDataRequest Request )
        {
            #region SQL Query

            String SelectText =
            @"with containerids as (
                select n.nodeid 
                from jct_nodes_props n
                left join (select jnp.nodeid, jnp.field1_numeric as quantity
                    from jct_nodes_props jnp
                    inner join nodetype_props ntp on jnp.nodetypepropid = ntp.nodetypepropid
                    inner join object_class_props ocp on ntp.objectclasspropid = ocp.objectclasspropid
                    where ocp.objectclasspropid = :quantityid) q
                    on n.nodeid = q.nodeid
                inner join nodetype_props ntp on n.nodetypepropid = ntp.nodetypepropid
                inner join object_class_props ocp on ntp.objectclasspropid = ocp.objectclasspropid
                where ocp.objectclasspropid = :materialpropid 
                    and q.quantity > 0 
                    and field1_fk in 
                        (select jnp.field1_fk as materials 
                                from jct_nodes_props jnp
                                inner join nodetype_props ntp on jnp.nodetypepropid = ntp.nodetypepropid
                                inner join object_class_props ocp on ntp.objectclasspropid = ocp.objectclasspropid
                                where ocp.objectclasspropid = :mixtureid 
                                and nodeid in 
                                (select jnp.nodeid 
                                from jct_nodes_props  jnp
                                inner join nodetype_props ntp on jnp.nodetypepropid = ntp.nodetypepropid
                                inner join object_class_props ocp on ntp.objectclasspropid = ocp.objectclasspropid
                                where ocp.objectclasspropid = :constituentid
                                and jnp.field1_fk = :materialid)
                                union 
                                (select to_number(:materialid) from dual) 
                        ) 
            )
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
                            inner join nodetype_props ntp on jnp.nodetypepropid = ntp.nodetypepropid
                            inner join object_class_props ocp on ntp.objectclasspropid = ocp.objectclasspropid
                            where ocp.objectclasspropid = :pressureid) p 
                            on jnpa.nodeid = p.nodeid and jnpa.audittransactionid = p.audittransactionid
                        left join (select jnp.nodeid, jnp.field1 as temperature, jnp.audittransactionid
                            from jct_nodes_props_audit jnp
                            inner join nodetype_props ntp on jnp.nodetypepropid = ntp.nodetypepropid
                            inner join object_class_props ocp on ntp.objectclasspropid = ocp.objectclasspropid
                            where ocp.objectclasspropid = :temperatureid) t 
                            on jnpa.nodeid = t.nodeid and jnpa.audittransactionid = t.audittransactionid
                        left join (select jnp.nodeid, jnp.field1 as usetype, jnp.audittransactionid
                            from jct_nodes_props_audit jnp
                            inner join nodetype_props ntp on jnp.nodetypepropid = ntp.nodetypepropid
                            inner join object_class_props ocp on ntp.objectclasspropid = ocp.objectclasspropid
                            where ocp.objectclasspropid = :usetypeid) u 
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
                    inner join nodetype_props ntp on jnp.nodetypepropid = ntp.nodetypepropid
                    inner join object_class_props ocp on ntp.objectclasspropid = ocp.objectclasspropid
                    where ocp.objectclasspropid = :pressureid) p 
                    on jnpa.nodeid = p.nodeid
                left join (select jnp.nodeid, jnp.field1 as temperature
                    from jct_nodes_props jnp
                    inner join nodetype_props ntp on jnp.nodetypepropid = ntp.nodetypepropid
                    inner join object_class_props ocp on ntp.objectclasspropid = ocp.objectclasspropid
                    where ocp.objectclasspropid = :temperatureid) t 
                    on jnpa.nodeid = t.nodeid
                left join (select jnp.nodeid, jnp.field1 as usetype
                    from jct_nodes_props jnp
                    inner join nodetype_props ntp on jnp.nodetypepropid = ntp.nodetypepropid
                    inner join object_class_props ocp on ntp.objectclasspropid = ocp.objectclasspropid
                    where ocp.objectclasspropid = :usetypeid) u 
                    on jnpa.nodeid = u.nodeid
                where exists (select nodeid from containerids where nodeid = jnpa.nodeid)
            )";

            #endregion SQL Query

            CswNbtMetaDataObjectClass MaterialComponentOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialComponentClass );
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp MixtureProp = MaterialComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Mixture );
            CswNbtMetaDataObjectClassProp ConstituentProp = MaterialComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Constituent );
            CswNbtMetaDataObjectClassProp MaterialProp = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Material );
            CswNbtMetaDataObjectClassProp PressureProp = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.StoragePressure );
            CswNbtMetaDataObjectClassProp TemperatureProp = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.StorageTemperature );
            CswNbtMetaDataObjectClassProp UseTypeProp = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.UseType );
            CswNbtMetaDataObjectClassProp QuantityProp = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Quantity );

            CswArbitrarySelect TierIIContainerPropsSelect = _CswNbtResources.makeCswArbitrarySelect( "Tier II Container Props Select", SelectText );
            TierIIContainerPropsSelect.addParameter( "quantityid", QuantityProp.PropId.ToString() );
            TierIIContainerPropsSelect.addParameter( "materialpropid", MaterialProp.PropId.ToString() );
            TierIIContainerPropsSelect.addParameter( "mixtureid", MixtureProp.PropId.ToString() );
            TierIIContainerPropsSelect.addParameter( "constituentid", ConstituentProp.PropId.ToString() );
            TierIIContainerPropsSelect.addParameter( "materialid", MaterialId );
            TierIIContainerPropsSelect.addParameter( "pressureid", PressureProp.PropId.ToString() );
            TierIIContainerPropsSelect.addParameter( "temperatureid", TemperatureProp.PropId.ToString() );
            TierIIContainerPropsSelect.addParameter( "usetypeid", UseTypeProp.PropId.ToString() );
            DataTable TargetTable = TierIIContainerPropsSelect.getTable();
            return TargetTable;
        }

        private DataTable _getContainerLocations( String MaterialId, TierIIData.TierIIDataRequest Request )
        {
            #region SQL Query

            String SelectText =
            @"with containerids as (
                select n.nodeid 
                from jct_nodes_props n
                left join (select jnp.nodeid, jnp.field1_numeric as quantity
                    from jct_nodes_props jnp
                    inner join nodetype_props ntp on jnp.nodetypepropid = ntp.nodetypepropid
                    inner join object_class_props ocp on ntp.objectclasspropid = ocp.objectclasspropid
                    where ocp.objectclasspropid = :quantityid) q
                    on n.nodeid = q.nodeid
                inner join nodetype_props ntp on n.nodetypepropid = ntp.nodetypepropid
                inner join object_class_props ocp on ntp.objectclasspropid = ocp.objectclasspropid
                where ocp.objectclasspropid = :materialpropid 
                    and q.quantity > 0 
                    and field1_fk in 
                        (select jnp.field1_fk as materials 
                                from jct_nodes_props jnp
                                inner join nodetype_props ntp on jnp.nodetypepropid = ntp.nodetypepropid
                                inner join object_class_props ocp on ntp.objectclasspropid = ocp.objectclasspropid
                                where ocp.objectclasspropid = :mixtureid 
                                and nodeid in 
                                (select jnp.nodeid 
                                from jct_nodes_props  jnp
                                inner join nodetype_props ntp on jnp.nodetypepropid = ntp.nodetypepropid
                                inner join object_class_props ocp on ntp.objectclasspropid = ocp.objectclasspropid
                                where ocp.objectclasspropid = :constituentid
                                and jnp.field1_fk = :materialid)
                                union 
                                (select to_number(:materialid) from dual) 
                        ) 
            )
            select unique locationid, fulllocation from (
                (select unique jnp.field1_fk as locationid, jnp.field4 as fulllocation
                    from jct_nodes_props_audit jnp
                    inner join nodetype_props ntp on jnp.nodetypepropid = ntp.nodetypepropid
                    inner join object_class_props ocp on ntp.objectclasspropid = ocp.objectclasspropid
                    where ocp.objectclasspropid = :locationpropid
                        and exists (select nodeid from containerids where nodeid = jnp.nodeid)
                        and jnp.recordcreated < " + _CswNbtResources.getDbNativeDate( DateTime.Parse( Request.EndDate ) ) + @" + 1)
                union
                (select unique jnp.field1_fk as locationid, jnp.field4 as fulllocation
                    from jct_nodes_props jnp
                    inner join nodetype_props ntp on jnp.nodetypepropid = ntp.nodetypepropid
                    inner join object_class_props ocp on ntp.objectclasspropid = ocp.objectclasspropid
                    where ocp.objectclasspropid = :locationpropid
                        and exists (select nodeid from containerids where nodeid = jnp.nodeid))
            ) where locationid member of csw_number_table(:locationids) and fulllocation is not null";

            #endregion SQL Query

            CswNbtMetaDataObjectClass MaterialComponentOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialComponentClass );
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp MixtureProp = MaterialComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Mixture );
            CswNbtMetaDataObjectClassProp ConstituentProp = MaterialComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Constituent );
            CswNbtMetaDataObjectClassProp MaterialProp = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Material );
            CswNbtMetaDataObjectClassProp LocationProp = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Location );
            CswNbtMetaDataObjectClassProp QuantityProp = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Quantity );

            CswArbitrarySelect TierIILocationsSelect = _CswNbtResources.makeCswArbitrarySelect( "Tier II Container Locations Select", SelectText );
            TierIILocationsSelect.addParameter( "quantityid", QuantityProp.PropId.ToString() );
            TierIILocationsSelect.addParameter( "materialpropid", MaterialProp.PropId.ToString() );
            TierIILocationsSelect.addParameter( "mixtureid", MixtureProp.PropId.ToString() );
            TierIILocationsSelect.addParameter( "constituentid", ConstituentProp.PropId.ToString() );
            TierIILocationsSelect.addParameter( "materialid", MaterialId );
            TierIILocationsSelect.addParameter( "locationpropid", LocationProp.PropId.ToString() );
            TierIILocationsSelect.addParameter( "locationids", LocationIds.ToString() );
            DataTable TargetTable = TierIILocationsSelect.getTable();
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

        private String _getRangeCode( Double QuantityInPounds )
        {
            String RangeCode = "00";
            TierIIRangeCode Code = RangeCodes.FirstOrDefault( 
                RC => 
                    QuantityInPounds >= RC.LowerBound && 
                    QuantityInPounds < RC.UpperBound + 1 
                );
            if( null != Code )
            {
                RangeCode = Code.RangeCode;
            }
            return RangeCode;
        }

        /// <summary>
        /// Default list of Tier II Reporting Ranges
        /// Note that this list is subject to change
        /// </summary>
        private void _setDefaultRangeCodes()
        {
            RangeCodes = new Collection<TierIIRangeCode>
            {
                new TierIIRangeCode { RangeCode = "01", LowerBound = 0, UpperBound = 99 }, 
                new TierIIRangeCode { RangeCode = "02", LowerBound = 100, UpperBound = 499 }, 
                new TierIIRangeCode { RangeCode = "03", LowerBound = 500, UpperBound = 999 }, 
                new TierIIRangeCode { RangeCode = "04", LowerBound = 1000, UpperBound = 4999 }, 
                new TierIIRangeCode { RangeCode = "05", LowerBound = 5000, UpperBound = 9999 }, 
                new TierIIRangeCode { RangeCode = "06", LowerBound = 10000, UpperBound = 24999 }, 
                new TierIIRangeCode { RangeCode = "07", LowerBound = 25000, UpperBound = 49999 }, 
                new TierIIRangeCode { RangeCode = "08", LowerBound = 50000, UpperBound = 74999 }, 
                new TierIIRangeCode { RangeCode = "09", LowerBound = 75000, UpperBound = 99999 }, 
                new TierIIRangeCode { RangeCode = "10", LowerBound = 100000, UpperBound = 499999 }, 
                new TierIIRangeCode { RangeCode = "11", LowerBound = 500000, UpperBound = 999999 }, 
                new TierIIRangeCode { RangeCode = "12", LowerBound = 1000000, UpperBound = 9999999 }, 
                new TierIIRangeCode { RangeCode = "13", LowerBound = 10000000, UpperBound = Int32.MaxValue - 1 }
            };
        }

        #endregion Private Methods
    }

    internal class TierIIRangeCode
    {
        public String RangeCode = String.Empty;
        public Int32 LowerBound = 0;
        public Int32 UpperBound = Int32.MaxValue;
    }
}
