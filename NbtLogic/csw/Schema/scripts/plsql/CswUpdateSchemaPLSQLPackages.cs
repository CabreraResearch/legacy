using System;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// CswUpdateSchemaPLSQLPackages
    /// </summary>    
    public class CswUpdateSchemaPLSQLPackages
    {
        public sealed class PackageHeaders : CswEnum<PackageHeaders>
        {
            #region Properties and ctor

            private PackageHeaders( string Dev, Int32 CaseNo, string Name )
                : base( Name )
            {
                _Dev = Dev;
                _CaseNo = CaseNo;
            }
            static PackageHeaders() { }
            public string _Dev { get; private set; }
            public Int32 _CaseNo { get; private set; }
            public static IEnumerable<PackageHeaders> _All { get { return All; } }
            public static implicit operator PackageHeaders( string str )
            {
                PackageHeaders ret = Parse( str );
                return ret;
            }

            #endregion Properties and ctor

            #region TIER_II_DATA_MANAGER

            public static readonly PackageHeaders TIER_II_DATA_MANAGER_HEAD = new PackageHeaders( CswEnumDeveloper.BV, 28247,
            @"create or replace
PACKAGE TIER_II_DATA_MANAGER AS 

  DATE_ADDED date;
  LAST_RUN_INTERVAL number;
  WEIGHT_BASE_UNIT_ID number;

  function GET_LOCATIONS_UNDER (locationid in number) return tier_ii_location_table pipelined;
  procedure SET_TIER_II_DATA;
  function GET_TIER_II_DATA (locationid in number, start_date in date, end_date in date) return TIER_II_TABLE pipelined;

END TIER_II_DATA_MANAGER;" );

            #endregion TIER_II_DATA_MANAGER

            #region UNIT_CONVERSION

            public static readonly PackageHeaders UNIT_CONVERSION_HEAD = new PackageHeaders( CswEnumDeveloper.BV, 28247,
            @"create or replace
PACKAGE UNIT_CONVERSION AS 

  function GET_BASE_UNIT (unit_name varchar2) return number;
  function GET_CONVERSION_FACTOR (unit_id number) return number;
  function CONVERT_UNIT (
    value_to_convert in number, 
    old_conversion_factor in number, 
    new_conversion_factor in number, 
    specific_gravity in number default 1
    ) return number;
  function CONVERT_UNIT_TO_LBS (
    value_to_convert in number, 
    unitid in number, 
    specific_gravity in number default 1
    ) return number;

END UNIT_CONVERSION;" );

            #endregion UNIT_CONVERSION_MANAGER

            #region NBT_VIEW_BUILDER

            public static readonly PackageHeaders NBT_VIEW_BUILDER = new PackageHeaders( CswEnumDeveloper.BV, 52432,
            @"create or replace
PACKAGE NBT_VIEW_BUILDER AS 

  procedure CREATE_ALL_OC_VIEWS;
  procedure CREATE_OC_VIEW (ocid number, oc_name varchar2);
  procedure DROP_OLD_OC_VIEWS;
  function MAKE_INTO_VALID_OC_VIEW_NAME (objectclassname varchar2) return varchar2;

END NBT_VIEW_BUILDER;" );

            #endregion NBT_VIEW_BUILDER
        }

        public sealed class PackageBodies : CswEnum<PackageBodies>
        {
            #region Properties and ctor

            private PackageBodies( string Dev, Int32 CaseNo, string Name )
                : base( Name )
            {
                _Dev = Dev;
                _CaseNo = CaseNo;
            }
            static PackageBodies() { }
            public string _Dev { get; private set; }
            public Int32 _CaseNo { get; private set; }
            public static IEnumerable<PackageBodies> _All { get { return All; } }
            public static implicit operator PackageBodies( string str )
            {
                PackageBodies ret = Parse( str );
                return ret;
            }

            #endregion Properties and ctor

            #region TIER_II_DATA_MANAGER

            public static readonly PackageBodies TIER_II_DATA_MANAGER_BODY = new PackageBodies( CswEnumDeveloper.BV, 28247,
            @"create or replace
PACKAGE BODY TIER_II_DATA_MANAGER AS
  
  procedure SET_PACKAGE_PROPERTIES is
    LAST_RUN_DATE date;
  begin
    DATE_ADDED := sysdate;
    select nvl(max(dateadded), sysdate-1)
      into LAST_RUN_DATE
      from tier2;
    LAST_RUN_INTERVAL := trunc(DATE_ADDED) - trunc(LAST_RUN_DATE);
    WEIGHT_BASE_UNIT_ID := UNIT_CONVERSION.GET_BASE_UNIT('Unit_Weight');
  end SET_PACKAGE_PROPERTIES;

  function GET_LOCATIONS_UNDER(locationid in number) return tier_ii_location_table pipelined is
    unsorted_locations tier_ii_location_table;
    sorted_locations tier_ii_location_table;
    temp_locs tier_ii_location_table;
    isFinished number := 0;
  begin
    --Get all locationids and their parentlocationids
    select tier_ii_location(n.nodeid, loc.field1_fk) 
      bulk collect into unsorted_locations
      from nodes n
      left join
        (select n.nodeid, jnp.field1_fk
          from jct_nodes_props JNP
            inner join nodes n on n.nodeid = jnp.nodeid
            inner join nodetype_props ntp on jnp.nodetypepropid = ntp.nodetypepropid        
            inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
            inner join object_class oc on ocp.objectclassid = oc.objectclassid
          where oc.objectclass = 'LocationClass'
          and ocp.propname = 'Location') loc on n.nodeid = loc.nodeid
        inner join nodetypes nt on n.nodetypeid = nt.nodetypeid
        inner join object_class oc on nt.objectclassid = oc.objectclassid
        where oc.objectclass = 'LocationClass';
      
    --Store root base case
    select tier_ii_location(locationid, null) 
      bulk collect into sorted_locations 
      from dual;
      
    --Grab all locations under the given locationid one tree level at a time (starting at the root)
    while isFinished = 0 loop
      select tier_ii_location(LOCATIONID, parentlocationid)
        bulk collect into temp_locs 
        from table(unsorted_locations)
        where PARENTLOCATIONID in (select locationid from table(sorted_locations))
        and LOCATIONID not in (select locationid from table(sorted_locations));
      if(temp_locs.count = 0) then
        isFinished := 1;
      else
        for x in 1..temp_locs.count loop
          sorted_locations.extend(1);
          sorted_locations(sorted_locations.count) := temp_locs(x);
        end loop;
      end if;
    end loop;
    
    select tier_ii_location(LOCATIONID, parentlocationid) 
      bulk collect into temp_locs 
      from table(sorted_locations);
    for x in 1..temp_locs.count loop
      pipe row(temp_locs(x));
    end loop;
    
    return;
  end GET_LOCATIONS_UNDER;
  
  function GET_LOCATIONS return tier_ii_location_table is
    unsorted_locations tier_ii_location_table;
    sorted_locations tier_ii_location_table;
    temp_locs tier_ii_location_table;
  begin
    --Get all locationids and their parentlocationids
    select tier_ii_location(n.nodeid, loc.field1_fk) 
      bulk collect into unsorted_locations
      from nodes n
      left join
        (select n.nodeid, jnp.field1_fk
          from jct_nodes_props JNP
            inner join nodes n on n.nodeid = jnp.nodeid
            inner join nodetype_props ntp on jnp.nodetypepropid = ntp.nodetypepropid        
            inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
            inner join object_class oc on ocp.objectclassid = oc.objectclassid
          where oc.objectclass = 'LocationClass'
          and ocp.propname = 'Location') loc on n.nodeid = loc.nodeid
        inner join nodetypes nt on n.nodetypeid = nt.nodetypeid
        inner join object_class oc on nt.objectclassid = oc.objectclassid
        where oc.objectclass = 'LocationClass';
      
    --Store null base case
    select tier_ii_location(ul.LOCATIONID, ul.parentlocationid) 
      bulk collect into sorted_locations 
      from table(unsorted_locations) ul
      where ul.PARENTLOCATIONID is null;
      
    --Grab all locations one tree level at a time (starting at root)
    while sorted_locations.count < unsorted_locations.count loop
      select tier_ii_location(LOCATIONID, parentlocationid)
        bulk collect into temp_locs 
        from table(unsorted_locations)
        where PARENTLOCATIONID in (select locationid from table(sorted_locations))
        and LOCATIONID not in (select locationid from table(sorted_locations));
      for x in 1..temp_locs.count loop
        sorted_locations.extend(1);
        sorted_locations(sorted_locations.count) := temp_locs(x);
      end loop;
    end loop;

    --Reverse order
    select tier_ii_location(LOCATIONID, parentlocationid) 
      bulk collect into temp_locs 
      from table(sorted_locations);
    for x in 1..temp_locs.count loop
      sorted_locations(x) := temp_locs(temp_locs.count + 1 - x);
    end loop;
    
    return sorted_locations;
  end GET_LOCATIONS;
  
  function GET_MATERIALS (LocId in number) return tier_ii_material_table is
    materials tier_ii_material_table;
    containers tier_ii_material_table;
    components tier_ii_material_table;
    conversion_factor number;
    specific_gravity number := 1;
    found number := 0;
  begin
    --Take the contents of the child locations' Materials and add them to the list (setting every Material's Quantity value to 0)
    select tier_ii_material(t.materialid, t.casno, 0, t.totalquantity, t.unitid, null, null)
      bulk collect into materials
      from Tier2 t
      where t.parentlocationid = LocId
      and t.dateadded = DATE_ADDED;
    
    if materials.exists(materials.first) then
      for i in materials.first..materials.last loop
        if materials.exists(i) then--If the MaterialQty to add's MaterialId already exists in the list,
          for j in materials.first..materials.last loop--add the MaterialQty to add's CollectiveQty to the existing MaterialQty's CollectiveQty
            if materials.exists(j) and i != j and materials(i).materialid = materials(j).materialid then
              materials(i).totalquantity := materials(i).totalquantity + materials(j).totalquantity;
              materials.delete(j);
            end if;
          end loop;
        end if;
      end loop;
    end if;
    
    --Grab all Containers in the current Location where qty > 0 and material's tierII is true     
    select tier_ii_material(m.materialid, mat.casno, qty.quantity, qty.quantity, qty.unitid, uom.unit_type, mat.spec_grav)
      bulk collect into containers
      from nodes n
    left join (select jnp.nodeid, jnp.field1_numeric as quantity, jnp.field1_fk as unitid
      from jct_nodes_props jnp
      inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
      inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
      where ocp.propname = 'Quantity') qty on n.nodeid = qty.nodeid
    left join (select n.nodeid, ut.unit_type
      from nodes n
      left join (select jnp.nodeid, jnp.field1 as unit_type
        from jct_nodes_props jnp
        inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
        inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
        where ocp.propname = 'Unit Type') ut on n.nodeid = ut.nodeid
      inner join nodetypes nt on n.nodetypeid = nt.nodetypeid
        inner join object_class oc on nt.objectclassid = oc.objectclassid
        where oc.objectclass = 'UnitOfMeasureClass') uom on qty.unitid = uom.nodeid
    left join (select jnp.nodeid, jnp.field1_fk as materialid
      from jct_nodes_props jnp
      inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
      inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
      where ocp.propname = 'Material') m on n.nodeid = m.nodeid
    left join (select n.nodeid, cas.casno, sg.spec_grav
      from nodes n
      left join (select jnp.nodeid, jnp.field1 as casno
        from jct_nodes_props jnp
        inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
        inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
        where ocp.propname = 'CAS No') cas on n.nodeid = cas.nodeid
      left join (select jnp.nodeid, jnp.field1_numeric as spec_grav
        from jct_nodes_props jnp
        inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
        inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
        where ocp.propname = 'Specific Gravity') sg on n.nodeid = sg.nodeid
      inner join nodetypes nt on n.nodetypeid = nt.nodetypeid
        inner join object_class oc on nt.objectclassid = oc.objectclassid
        where oc.objectclass = 'ChemicalClass') mat on m.materialid = mat.nodeid
    left join (select jnp.nodeid, jnp.field1_fk as locationid
      from jct_nodes_props jnp
      left join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
      inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
      where ocp.propname = 'Location') loc on n.nodeid = loc.nodeid
    inner join nodetypes nt on n.nodetypeid = nt.nodetypeid
      inner join object_class oc on nt.objectclassid = oc.objectclassid
      where oc.objectclass = 'ContainerClass'
      and qty.quantity > 0
      and loc.locationid = LocId
      and mat.spec_grav is not null;
      
    for i in 1..containers.count loop
      --For each Container, get all of its Material's Constituent amounts based on their percentage and add them to containers
      select
        tier_ii_material
        (
          c.constid, 
          mat.casno, 
          containers(i).quantity * per.percentage / 100, 
          containers(i).quantity * per.percentage / 100, 
          containers(i).unitid, 
          containers(i).unittype, 
          containers(i).specificgravity
        )
      bulk collect into components
        from nodes n
        left join (select jnp.nodeid, jnp.field1_numeric as percentage
      from jct_nodes_props jnp
        inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
        inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
        where ocp.propname = 'Percentage') per on n.nodeid = per.nodeid
      left join (select jnp.nodeid, jnp.field1_fk as materialid
        from jct_nodes_props jnp
        inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
        inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
        where ocp.propname = 'Mixture') m on n.nodeid = m.nodeid
      left join (select jnp.nodeid, jnp.field1_fk as constid
        from jct_nodes_props jnp
        inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
        inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
        where ocp.propname = 'Constituent') c on n.nodeid = c.nodeid
      left join (select n.nodeid, cas.casno
        from nodes n
        left join (select jnp.nodeid, jnp.field1 as casno
          from jct_nodes_props jnp
          inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
          inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
          where ocp.propname = 'CAS No') cas on n.nodeid = cas.nodeid
        inner join nodetypes nt on n.nodetypeid = nt.nodetypeid
          inner join object_class oc on nt.objectclassid = oc.objectclassid
          where oc.objectclass = 'ChemicalClass') mat on mat.nodeid = c.constid
      inner join nodetypes nt on n.nodetypeid = nt.nodetypeid
        inner join object_class oc on nt.objectclassid = oc.objectclassid
        where oc.objectclass = 'MaterialComponentClass'
          and c.constid is not null
          and per.percentage is not null
          and m.materialid = containers(i).materialid;
          
      for j in 1..components.count loop
        containers.extend(1);
        containers(containers.count) := components(j);
      end loop;
    end loop;

    for i in 1..containers.count loop
      if containers(i).unitid != WEIGHT_BASE_UNIT_ID then
        conversion_factor := UNIT_CONVERSION.GET_CONVERSION_FACTOR(containers(i).unitid);
        if containers(i).unittype = 'Volume' then
          specific_gravity := containers(i).specificgravity;
        end if;
        containers(i).quantity := UNIT_CONVERSION.CONVERT_UNIT(containers(i).quantity, conversion_factor, 1, specific_gravity);
        containers(i).totalquantity := containers(i).quantity;
        containers(i).unitid := WEIGHT_BASE_UNIT_ID;
      end if;
      found := 0;
      if materials.count > 0 then
        for j in 1..materials.count loop
          --If a MaterialQty with the container's MaterialId already exists, 
          --add the container's Qty to both the MaterialQty's Qty and CollectiveQty value
          if materials.exists(j) and containers(i).materialid = materials(j).materialid then
            materials(j).quantity := materials(j).quantity + containers(i).quantity;
            materials(j).totalquantity := materials(j).totalquantity + containers(i).totalquantity;
            found := 1;
            exit;
          end if;
        end loop;
      end if;
      if found = 0 then
        materials.extend(1);
        materials(materials.count) := containers(i);
      end if;
    end loop;
    
    return materials;
  end GET_MATERIALS;
  
  procedure SET_TIER_II_DATA is
    Locations tier_ii_location_table;
    Materials tier_ii_material_table;
  begin
    SET_PACKAGE_PROPERTIES();
    if LAST_RUN_INTERVAL > 0 then
      Locations := GET_LOCATIONS();
      for loc in 1..Locations.count loop
        Materials := GET_MATERIALS(Locations(loc).LocationId);
        if Materials.exists(Materials.first) then
          for mat in Materials.first..Materials.last loop
            if Materials.exists(mat) and Materials(mat).materialid is not null then
              insert
                into TIER2
                (
                  TIER2ID,
                  DATEADDED, 
                  LOCATIONID, 
                  PARENTLOCATIONID, 
                  MATERIALID, 
                  CASNO, 
                  QUANTITY, 
                  TOTALQUANTITY,
                  UNITID
                ) 
                values 
                (
                  SEQ_TIER2ID.nextval,
                  DATE_ADDED, 
                  Locations(loc).LOCATIONID, 
                  Locations(loc).PARENTLOCATIONID, 
                  Materials(mat).MATERIALID,
                  Materials(mat).CASNO,
                  Materials(mat).QUANTITY,
                  Materials(mat).TOTALQUANTITY,
                  Materials(mat).UNITID
                );
            end if;
          end loop;
          commit;
        end if;
      end loop;
    end if;
  end SET_TIER_II_DATA;
  
  function GET_TIER_II_DATA (locationid in number, start_date in date, end_date in date) return TIER_II_TABLE pipelined is
    TIER_II_MATERIALS TIER_II_TABLE;
  begin

    with
    --Chemical Props
    istierii as (
    select jnp.nodeid, jnp.field1 as istierii
            from jct_nodes_props jnp
            inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
            inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
            where ocp.propname = 'Is Tier II'
    ),
    chemicals as (
    select n.nodeid, t2.istierii
          from nodes n
          left join istierii t2 on n.nodeid = t2.nodeid
          inner join nodetypes nt on n.nodetypeid = nt.nodetypeid
            inner join object_class oc on nt.objectclassid = oc.objectclassid
            where oc.objectclass = 'ChemicalClass'
    ),
    --Components
    components as (
    select c.constid, mat.istierii, m.materialid, per.percentage
      from nodes n
      left join (select jnp.nodeid, greatest(nvl(jnp.field1_numeric,0),nvl(jnp.field2_numeric,0),nvl(jnp.field3_numeric,0)) as percentage
    from jct_nodes_props jnp
      inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
      inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
      where ocp.propname = '" + CswNbtObjClassMaterialComponent.PropertyName.PercentageRange + @"') per on n.nodeid = per.nodeid
    left join (select jnp.nodeid, jnp.field1_fk as materialid
      from jct_nodes_props jnp
      inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
      inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
      where ocp.propname = '" + CswNbtObjClassMaterialComponent.PropertyName.Mixture + @"') m on n.nodeid = m.nodeid
    left join (select jnp.nodeid, jnp.field1_fk as constid
      from jct_nodes_props jnp
      inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
      inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
      where ocp.propname = '" + CswNbtObjClassMaterialComponent.PropertyName.Constituent + @"') c on n.nodeid = c.nodeid
    left join (select n.nodeid, istierii.istierii
      from nodes n
      left join (select jnp.nodeid, jnp.field1 as istierii
        from jct_nodes_props jnp
        inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
        inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
        where ocp.propname = 'Is Tier II') istierii on n.nodeid = istierii.nodeid
      inner join nodetypes nt on n.nodetypeid = nt.nodetypeid
        inner join object_class oc on nt.objectclassid = oc.objectclassid
        where oc.objectclass = 'ChemicalClass') mat on mat.nodeid = c.constid
    inner join nodetypes nt on n.nodetypeid = nt.nodetypeid
      inner join object_class oc on nt.objectclassid = oc.objectclassid
      where oc.objectclass = 'MaterialComponentClass'
        and c.constid is not null
        and per.percentage is not null
    ),
    allMats as (
    select constid, istierii, materialid, percentage, decode(constid, null, materialid, constid) matid from 
        (select null as constid, istierii, nodeid as materialid, 100 as percentage from chemicals where nodeid not in
          (select constid from components)
        union all 
        select * from components) 
        where istierii = 1
    ),
    --ContainerDispenseTransaction Props
    DispensedDate as (
    select jnp.nodeid, jnp.field1_date as DispensedDate
            from jct_nodes_props jnp
            inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
            inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
            where ocp.propname = 'Dispensed Date'
    ),
    DispenseType as (
    select jnp.nodeid, jnp.field1 as DispenseType
            from jct_nodes_props jnp
            inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
            inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
            where ocp.propname = 'Dispense Type'
    ),
    QuantityDispensed as (
    select jnp.nodeid, jnp.field1_numeric as QuantityDispensed, jnp.field1_fk as QuantityDispensedUnit
            from jct_nodes_props jnp
            inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
            inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
            where ocp.propname = 'Quantity Dispensed'
    ),
    RemainingQuantity as (
    select jnp.nodeid, jnp.field1_numeric as RemainingQuantity, jnp.field1_fk as RemainingQuantityUnit
            from jct_nodes_props jnp
            inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
            inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
            where ocp.propname = 'Remaining Source Container Quantity'
    ),
    SourceContainer as (
    select jnp.nodeid, jnp.field1_fk as containerid
            from jct_nodes_props jnp
            inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
            inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
            where ( ocp.propname = 'Source Container' or ocp.propname = 'Destination Container' )
    ),
    --Container Props
    material as (
    select jnp.nodeid, jnp.field1_fk as materialid
          from jct_nodes_props jnp
          inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
          inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
          where ocp.propname = 'Material'
    ),
    usetype as (
    select jnp.nodeid, jnp.field1 as usetype
          from jct_nodes_props jnp
          inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
          inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
          where ocp.propname = 'Use Type'
    ),
    pressure as (
    select jnp.nodeid, jnp.field1 as pressure
          from jct_nodes_props jnp
          inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
          inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
          where ocp.propname = 'Storage Pressure'
    ),
    temperature as (
    select jnp.nodeid, jnp.field1 as temperature
          from jct_nodes_props jnp
          inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
          inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
          where ocp.propname = 'Storage Temperature'
    ),
    containers as (
    select n.nodeid, 
    am.matid materialid,
    am.percentage,
    ut.usetype, sp.pressure, st.temperature
        from nodes n
        left join material mat on n.nodeid = mat.nodeid
        left join allMats am on mat.materialid = am.materialid
        left join usetype ut on n.nodeid = ut.nodeid
        left join pressure sp on n.nodeid = sp.nodeid
        left join temperature st on n.nodeid = st.nodeid
        inner join nodetypes nt on n.nodetypeid = nt.nodetypeid
          inner join object_class oc on nt.objectclassid = oc.objectclassid
          where oc.objectclass = 'ContainerClass'
    ),
    dispenses as (
    select 
        --Use component's id if present - else, use materialid (TODO - fix)
        c.materialid,  c.nodeid containerid,  dt.DispenseType,
        --Shift the day forward for Dispose records so that they are counted on the day they're disposed
        decode(dt.DispenseType, 'Dispose', dd.DispensedDate+1, dd.DispensedDate) DispensedDate, 
        trunc(decode(dt.DispenseType, 'Dispose', dd.DispensedDate+1, dd.DispensedDate)) dispenseday, 
        qd.QuantityDispensed, 
        qd.QuantityDispensedUnit,
        --Brings QuantityDispensed values over to RemainingQuantity column for Receiving dispenses
        decode(rq.RemainingQuantity, null, qd.QuantityDispensed, rq.RemainingQuantity) RemainingQuantity,
        --Picks the final quantity value for all dispense records for a given day and, 
        --if it's for a constituent, recalculates the quantity based on the constituent's percentage
        (first_value(decode(rq.RemainingQuantity, null, qd.QuantityDispensed, rq.RemainingQuantity)) 
        over(partition by containerid, trunc(decode(dt.DispenseType, 'Dispose', dd.DispensedDate+1, dd.DispensedDate)) 
        order by decode(dt.DispenseType, 'Dispose', dd.DispensedDate+1, dd.DispensedDate) desc)) * c.percentage / 100 as qty
        ,c.percentage
            from nodes n
            left join DispenseType dt on n.nodeid = dt.nodeid
            left join DispensedDate dd on n.nodeid = dd.nodeid
            left join QuantityDispensed qd on n.nodeid = qd.nodeid
            left join RemainingQuantity rq on n.nodeid = rq.nodeid
            left join SourceContainer sc on n.nodeid = sc.nodeid
            left join containers c on sc.containerid = c.nodeid
            inner join nodetypes nt on n.nodetypeid = nt.nodetypeid
            inner join object_class oc on nt.objectclassid = oc.objectclassid
            where oc.objectclass = 'ContainerDispenseTransactionClass'
            and sc.containerid is not null
            and c.materialid is not null
    ),
    cal as (
    select start_date + level - 1 the_date, 0 qty 
        from dual 
        connect by level <= end_date - start_date + 1
    ),
    dispensesPerDay as (
    select 
        cdt.materialid, cdt.containerid, max(cdt.dispenseddate), cdt.dispenseday, cdt.qty, cdt.QuantityDispensedUnit, cal.the_date
        from dispenses cdt
        join  cal  on (cal.the_date >= cdt.dispenseday AND cal.the_date <= end_date)
        group by cdt.materialid, cdt.containerid, cdt.dispenseday, cdt.qty, cdt.QuantityDispensedUnit, cal.the_date
    ),
    --select * from dispensesPerDay;
    LocLocation as (
    select jnp.nodeid, jnp.field4 as locationPath, jnp.field1_fk as locationid
            from jct_nodes_props jnp
            inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
            inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
            where ocp.propname = 'Location'
    ),
    LocContainer as (
    select jnp.nodeid, jnp.field1_fk as containerid
            from jct_nodes_props jnp
            inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
            inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
            where ocp.propname = 'Container'
    ),
    LocStatus as (
    select jnp.nodeid, jnp.field1 as status
            from jct_nodes_props jnp
            inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
            inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
            where ocp.propname = 'Status'
    ),
    LocScandate as (
    select jnp.nodeid, jnp.field1_date as scandate
            from jct_nodes_props jnp
            inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
            inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
            where ocp.propname = 'Scan Date'
    ),
    locations as (
    select 
        c.materialid, c.nodeid containerid, ll.locationid, lsd.scandate, cal.the_date, c.usetype, c.pressure, c.temperature
        from nodes n
        left join LocStatus ls on n.nodeid = ls.nodeid
        left join LocLocation ll on n.nodeid = ll.nodeid
        left join LocContainer lc on n.nodeid = lc.nodeid
        left join LocScanDate lsd on n.nodeid = lsd.nodeid
        left join containers c on lc.containerid = c.nodeid
        join cal on (cal.the_date >= trunc(lsd.scandate) AND cal.the_date <= end_date)
        inner join nodetypes nt on n.nodetypeid = nt.nodetypeid
          inner join object_class oc on nt.objectclassid = oc.objectclassid
          where oc.objectclass = 'ContainerLocationClass'
          and c.materialid is not null
          and c.nodeid is not null
          and ls.status = 'Moved, Dispensed, or Disposed/Undisposed'--This filters out scan and placeholder records
    ),
    locationscope as (
      select locationid from table(TIER_II_DATA_MANAGER.GET_LOCATIONS_UNDER(locationid))
    ),
    containerlocations as (
    select unique materialid, containerid, the_date, usetype, pressure, temperature,
        (first_value(locationid) over( partition by containerid, the_date order by scandate desc)) as locationid
        from locations locs
        order by the_date asc, materialid, containerid, locationid
    ),
    tier2info as (
    select unique dpd.materialid, dpd.containerid, dpd.the_date,
        (first_value(dpd.qty) over( partition by dpd.materialid, dpd.containerid, dpd.the_date order by dpd.dispenseday desc)) as curqty,
        dpd.quantitydispensedunit qtyunit, cl.locationid, cl.usetype, cl.pressure, cl.temperature
        from dispensesPerDay dpd 
        left join containerLocations cl on cl.materialid = dpd.materialid and cl.containerid = dpd.containerid and cl.the_date = dpd.the_date
        where cl.locationid in (select locationid from locationscope)
    ),
    --select * from tier2info;
    casno as (
    select jnp.nodeid, jnp.field1 as casno
            from jct_nodes_props jnp
            inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
            inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
            where ocp.propname = 'CAS No'
    ),
    tier2infoByCASNo as (
      select first_value(t2.materialid) over(partition by cn.casno order by t2.the_date) materialid, 
      t2.containerid, t2.the_date, t2.curqty, t2.qtyunit, t2.locationid, t2.usetype, t2.pressure, t2.temperature
      from tier2info t2 
      left join casno cn on cn.nodeid = t2.materialid
    ),
    --Comment out this block of container props to run any anteceding subqueries
    uniqueusetypes as (
      select unique materialid, listagg(usetype, ',') within group (order by usetype) usetype
      from (select unique materialid, usetype from tier2info) 
      group by materialid
    ),
    uniquepressures as (
      select unique materialid, listagg(pressure, ',') within group (order by pressure) pressure
      from (select unique materialid, pressure from tier2info) 
      group by materialid
    ),
    uniquetemperatures as (
      select unique materialid, listagg(temperature, ',') within group (order by temperature) temperature
      from (select unique materialid, temperature from tier2info) 
      group by materialid
    ),
    containerprops as (
      select uut.materialid, uut.usetype, up.pressure, ut.temperature
      from uniqueusetypes uut
      left join uniquepressures up on uut.materialid = up.materialid
      left join uniquetemperatures ut on uut.materialid = ut.materialid
    ),
    --select * from containerprops;
    tier2qtyByUnit as (
    select materialid, the_date, sum(curqty) as qty, qtyunit, locationid
        from tier2infoByCASNo
        group by materialid, the_date, qtyunit, locationid
    ),
    specgrav as (
    select jnp.nodeid, jnp.field1_numeric as spec_grav
            from jct_nodes_props jnp
            inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
            inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
            where ocp.propname = 'Specific Gravity'
    ),
    --select * from tier2qtyByUnit;
    tier2qtyPerLocation as (
    select t2.materialid, t2.the_date, t2.locationid, sum(unit_conversion.convert_unit_to_lbs(t2.qty, t2.qtyunit, sg.spec_grav)) qty
        from tier2qtyByUnit t2
        join specgrav sg on sg.nodeid = t2.materialid
        group by t2.materialid, t2.the_date, t2.locationid
    ),
    tier2qtyPerDate as (
    select t2.materialid, sum(t2.qty) as qty, t2.the_date
      from tier2qtyPerLocation t2
      group by t2.materialid, t2.the_date
    ),
    --select * from tier2qtyPerDate;
    tier2quantities as (
    select 
      t2.materialid, max(t2.qty) as maxqty, round(avg(t2.qty), 6) as avgqty
      from tier2qtyPerDate t2
      where t2.qty > 0
      group by t2.materialid
    ),
    --select * from tier2quantities;
    storagelocations as (
    select materialid, listagg(locationPath, ', ') within group (order by locationPath) storagelocations 
      from (select unique ml.*, ll.locationPath 
            from (select unique ml.materialid, ml.locationid from tier2qtyPerLocation ml) ml
            left join LocLocation ll on ll.locationid = ml.locationid)
      group by materialid
    ),
    daysonsite as (
    select materialid, count(*) as daysonsite 
        from ( select unique materialid, the_date from tier2qtyPerLocation where qty > 0 ) 
        group by materialid
    ),
    --select * from daysonsite;
    tradename as (
    select jnp.nodeid, jnp.field1 as tradename
            from jct_nodes_props jnp
            inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
            inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
            where ocp.propname = 'Tradename'
    ),
    materialtype as (
    select jnp.nodeid, jnp.field1 as materialtype
            from jct_nodes_props jnp
            inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
            inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
            where ocp.propname = 'Material Type'
    ),
    physicalstate as (
    select jnp.nodeid, jnp.field1 as physicalstate
            from jct_nodes_props jnp
            inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
            inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
            where ocp.propname = 'Physical State'
    ),
    specialflags as (
    select jnp.nodeid, jnp.gestaltsearch as specialflags
            from jct_nodes_props jnp
            inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
            inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
            where ocp.propname = 'Special Flags'
    ),
    hazardcategories as (
    select jnp.nodeid, jnp.gestaltsearch as hazardcategories
            from jct_nodes_props jnp
            inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
            inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
            where ocp.propname = 'Hazard Categories'
    )
    select 
      TIER_II_ROW(t2.materialid, tn.tradename, cn.casno, mt.materialtype, ps.physicalstate,
      case when sf.specialflags like '%ehs%' then '1' else '0' end, 
      case when sf.specialflags like '%trade secret%' then '1' else '0' end,
      hc.hazardcategories,
      t2.maxqty, mq.range_code, t2.avgqty, aq.range_code, md.daysonsite, 
      cp.usetype, cp.pressure, cp.temperature, ml.storagelocations)
      bulk collect into TIER_II_MATERIALS
      from tier2quantities t2
      left join storagelocations ml on ml.materialid = t2.materialid
      left join daysonsite md on md.materialid = t2.materialid
      left join containerprops cp on cp.materialid = t2.materialid
      left join tradename tn on tn.nodeid = t2.materialid
      left join casno cn on cn.nodeid = t2.materialid
      left join materialtype mt on mt.nodeid = t2.materialid
      left join physicalstate ps on ps.nodeid = t2.materialid
      left join specialflags sf on sf.nodeid = t2.materialid
      left join hazardcategories hc on hc.nodeid = t2.materialid
      left join tier2_rangecodes mq on t2.maxqty >= mq.lower_bound and t2.maxqty < mq.upper_bound
      left join tier2_rangecodes aq on t2.avgqty >= aq.lower_bound and t2.avgqty < aq.upper_bound
      order by t2.materialid
    ;
    
    for i in 1..TIER_II_MATERIALS.count loop
      pipe row(TIER_II_MATERIALS(i));
    end loop;

    return;
  end GET_TIER_II_DATA;

END TIER_II_DATA_MANAGER;" );

            #endregion TIER_II_DATA_MANAGER

            #region UNIT_CONVERSION

            public static readonly PackageBodies UNIT_CONVERSION_BODY = new PackageBodies( CswEnumDeveloper.BV, 28247,
            @"create or replace
PACKAGE BODY UNIT_CONVERSION AS

  function GET_BASE_UNIT (unit_name varchar2) return number is
    unit_id number;
  begin
    select n.nodeid
    into unit_id
    from nodes n
      left join (
        select n.nodeid, jnp.field1 as unit
        from nodes n
        left join jct_nodes_props jnp on n.nodeid = jnp.nodeid    
        left join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
        where ntp.propname = 'Base Unit'
        ) base on base.nodeid = n.nodeid
      left join (
        select n.nodeid, jnp.field1 as unit
        from nodes n
        left join jct_nodes_props jnp on n.nodeid = jnp.nodeid    
        left join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
        where ntp.propname = 'Name'
        ) nm on base.nodeid = nm.nodeid
      inner join nodetypes nt on n.nodetypeid = nt.nodetypeid
      inner join object_class oc on nt.objectclassid = oc.objectclassid
      where oc.objectclass = 'UnitOfMeasureClass'
      and base.unit = nm.unit
      and nt.nodetypename = unit_name;
    return unit_id;
  end GET_BASE_UNIT;

  function GET_CONVERSION_FACTOR (unit_id number) return number is
    base number;
    exponent number;
    conversion_factor number;
  begin
    select jnp.field1_numeric, jnp.field2_numeric
      into base, exponent
      from jct_nodes_props jnp
        inner join nodes n on n.nodeid = jnp.nodeid            
        inner join nodetype_props ntp on jnp.nodetypepropid = ntp.nodetypepropid
        inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
        inner join object_class oc on ocp.objectclassid = oc.objectclassid
      where oc.objectclass = 'UnitOfMeasureClass'
      and ocp.propname = 'Conversion Factor'
      and n.nodeid = unit_id;
    conversion_factor := base * power(10, exponent);
    return conversion_factor;
  end GET_CONVERSION_FACTOR;

  function CONVERT_UNIT (value_to_convert in number, old_conversion_factor in number, new_conversion_factor in number, specific_gravity in number default 1) 
  return number is
    converted_value number;
  begin
    converted_value := value_to_convert * old_conversion_factor * specific_gravity / new_conversion_factor;
    return converted_value;
  end CONVERT_UNIT;

  function CONVERT_UNIT_TO_LBS (value_to_convert in number, unitid in number, specific_gravity in number default 1) 
  return number is
    conversion_factor number;
    lb_conversion_factor number := 0.453592;--This assumes the base unit for weight is kg
    converted_value number;
  begin
    conversion_factor := GET_CONVERSION_FACTOR(unitid);
    converted_value := value_to_convert * conversion_factor * specific_gravity / lb_conversion_factor;
    return round(converted_value, 3);
  end CONVERT_UNIT_TO_LBS;

END UNIT_CONVERSION;" );

            #endregion UNIT_CONVERSION

            #region NBT_VIEW_BUILDER

            public static readonly PackageBodies NBT_VIEW_BUILDER_BODY = new PackageBodies( CswEnumDeveloper.BV, 52432,
            @"create or replace
PACKAGE BODY NBT_VIEW_BUILDER AS

  --Creates and updates relational data views for all objectclasses
  procedure CREATE_ALL_OC_VIEWS is
    cursor ocs is
      select objectclassid,objectclass from object_class
      order by lower(objectclass);
  begin
    DROP_OLD_OC_VIEWS();
    for rec in ocs loop
      --dbms_output.put_line('createntview(' || to_char(rec.nodetypeid) || ',' || rec.nodetypename || ')');
      CREATE_OC_VIEW(rec.objectclassid, rec.objectclass);
    end loop;
  end CREATE_ALL_OC_VIEWS;

  --Creates or updates a relational data view for the given objectclass
  procedure CREATE_OC_VIEW (ocid number, oc_name varchar2) is
    cursor ocps is
      select ocp.objectclasspropid, ocp.oraviewcolname, ft.fieldtype, fts.propcolname, fts.subfieldname, fts.is_default
        from object_class_props ocp
        join field_types ft on ft.fieldtypeid=ocp.fieldtypeid
        join field_types_subfields fts on fts.fieldtypeid=ft.fieldtypeid
        where fts.reportable='1' and fts.is_default='1' and ocp.objectclassid=ocid 
        order by fts.is_default desc;
    var_sql clob;
    var_line varchar(2000);
    viewname varchar2(30);
    colname varchar2(30);
  begin
    --dbms_output.enable(32000);
    --dbms_output.put_line('Before Transform: ' || oc_name);
    viewname := MAKE_INTO_VALID_OC_VIEW_NAME(oc_name);
    --dbms_output.put_line('After Transform: ' || viewname);

    var_line:='create or replace view ' || viewname || ' as select n.nodeid ';
    --dbms_output.put_line('creating ' || viewname || '...');
    var_sql := var_sql || var_line;

    for rec in ocps loop
      --dbms_output.put_line(to_char(pcount) || '|' || safeSqlParam(rec.propname) || '|' || rec.subfieldname || '|' || rec.fieldtype || '|' || rec.objectclass || '|' || rec.nodetypename);
      colname := makeintovalidname(rec.oraviewcolname);

      --the gestalt
      var_line := ',(select gestalt from vwNpv where nid=n.nodeid and ocpid=' || to_char(rec.objectclasspropid);
      var_line := var_line || ') ' || colname || chr(13);
      var_sql := var_sql || var_line;
      
      --dbms_output.put_line(var_line);
      if(rec.fieldtype='Relationship' or rec.fieldtype='Location') then
        var_line := ',(select field1_fk from vwNpv where nid=n.nodeid and ocpid=' || to_char(rec.objectclasspropid);
        var_line := var_line || ') ' || substr(colname,1,27) || '_id';            
        var_sql := var_sql || var_line;
      elsif(rec.fieldtype='Quantity') then
        var_line := ',(select field1 from vwNpv where nid=n.nodeid and ntpid=' || to_char(rec.objectclasspropid);
        var_line := var_line || ') ' || substr(colname,1,26) || '_uom';            
        var_line := var_line || ',(select field1_numeric from vwNpv where nid=n.nodeid and ocpid=' || to_char(rec.objectclasspropid);
        var_line := var_line || ') ' || substr(colname,1,26) || '_val';
        var_line := var_line || ',(select field2_numeric from vwNpv where nid=n.nodeid and ocpid=' || to_char(rec.objectclasspropid);
        var_line := var_line || ') ' || substr(colname,1,23) || '_val_KG';
        var_line := var_line || ',(select field3_numeric from vwNpv where nid=n.nodeid and ocpid=' || to_char(rec.objectclasspropid);
        var_line := var_line || ') ' || substr(colname,1,24) || '_val_L';
        var_line := var_line || ',(select field1_fk from vwNpv where nid=n.nodeid and ocpid=' || to_char(rec.objectclasspropid);
        var_line := var_line || ') ' || substr(colname,1,24) || '_uomid';              
        var_sql := var_sql || var_line;
      elsif(rec.fieldtype='NFPA') then
        var_line := ',(select field1 from vwNpv where nid=n.nodeid and ocpid=' || to_char(rec.objectclasspropid);
        var_line := var_line || ') ' || substr(colname,1,27) || '_f';            
        var_sql := var_sql || var_line;
        var_line := ',(select field2 from vwNpv where nid=n.nodeid and ocpid=' || to_char(rec.objectclasspropid);
        var_line := var_line || ') ' || substr(colname,1,27) || '_r';            
        var_sql := var_sql || var_line;
        var_line := ',(select field3 from vwNpv where nid=n.nodeid and ocpid=' || to_char(rec.objectclasspropid);
        var_line := var_line || ') ' || substr(colname,1,27) || '_h';            
        var_sql := var_sql || var_line;
        var_line := ',(select field4 from vwNpv where nid=n.nodeid and ocpid=' || to_char(rec.objectclasspropid);
        var_line := var_line || ') ' || substr(colname,1,27) || '_s';            
        var_sql := var_sql || var_line;
      end if;
    end loop;
    var_line := ' from nodes n JOIN nodetypes nt ON nt.nodetypeid = n.nodetypeid AND nt.objectclassid = ' || to_char(ocid) || 
      ' where n.istemp = 0 and n.hidden = 0';
    var_sql := var_sql || var_line;
    --dbms_output.put_line(var_sql);
    execute immediate (var_sql);
    commit;
  end CREATE_OC_VIEW;
  
  --Removes any deprecated views leftover from deleted objectclasses (but ignore the old OC views)
  procedure DROP_OLD_OC_VIEWS is
    cursor ntsdel is
      select object_name from user_objects 
      where object_type='VIEW' and object_name not like 'OC%' and object_name like '%CLASS' and object_name not in
      (select MAKE_INTO_VALID_OC_VIEW_NAME(objectclass) from object_class);
    var_sql varchar2(200);
  begin
    for delrec in ntsdel loop
      var_sql := 'drop view ' || delrec.object_name;
      execute immediate (var_sql);
    end loop;
    commit;
  end DROP_OLD_OC_VIEWS;
  
  --Truncates objectclass name to 30 characters, keeping 'CLASS' at the end
  function MAKE_INTO_VALID_OC_VIEW_NAME (objectclassname varchar2) return varchar2 is
    valid_oc_name varchar2(30);
  begin
    valid_oc_name := upper(substr(substr(objectclassname, 0, INSTR(lower(objectclassname),'class')-1),0,25)) || 'CLASS';
    return valid_oc_name;
  end MAKE_INTO_VALID_OC_VIEW_NAME;

END NBT_VIEW_BUILDER;" );

            #endregion NBT_VIEW_BUILDER
        }

    }//class CswUpdateSchemaPLSQLPackages

}//namespace ChemSW.Nbt.Schema