using System;
using System.Collections.Generic;
using ChemSW.Core;
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

            private PackageHeaders( string Dev, Int32 CaseNo, string Name ) : base( Name )
            {
                _Dev = Dev;
                _CaseNo = CaseNo;
            }
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

            public static readonly PackageHeaders TIER_II_DATA_MANAGER_HEAD = new PackageHeaders( CswDeveloper.BV, 28247,
            @"create or replace
PACKAGE TIER_II_DATA_MANAGER AS 

  DATE_ADDED date;
  WEIGHT_BASE_UNIT_ID number;

  procedure SET_TIER_II_DATA;

END TIER_II_DATA_MANAGER;" );

            #endregion TIER_II_DATA_MANAGER

            #region UNIT_CONVERSION

            public static readonly PackageHeaders UNIT_CONVERSION_HEAD = new PackageHeaders( CswDeveloper.BV, 28247,
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

END UNIT_CONVERSION;" );

            #endregion UNIT_CONVERSION_MANAGER
        }

        public sealed class PackageBodies : CswEnum<PackageBodies>
        {
            #region Properties and ctor

            private PackageBodies( string Dev, Int32 CaseNo, string Name ) : base( Name )
            {
                _Dev = Dev;
                _CaseNo = CaseNo;
            }
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

            public static readonly PackageBodies TIER_II_DATA_MANAGER_BODY = new PackageBodies( CswDeveloper.BV, 28247,
            @"create or replace
PACKAGE BODY TIER_II_DATA_MANAGER AS
  
  procedure SET_PACKAGE_PROPERTIES is
  begin
    DATE_ADDED := sysdate;
    WEIGHT_BASE_UNIT_ID := UNIT_CONVERSION.GET_BASE_UNIT('Unit (Weight)');
  end SET_PACKAGE_PROPERTIES;
  
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
    left join (select n.nodeid, cas.casno, t2.istier2, sg.spec_grav
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
      left join (select jnp.nodeid, jnp.field1 as istier2
        from jct_nodes_props jnp
        inner join nodetype_props ntp on ntp.nodetypepropid = jnp.nodetypepropid
        inner join object_class_props ocp on ocp.objectclasspropid = ntp.objectclasspropid
        where ocp.propname = 'Is Tier II') t2 on n.nodeid = t2.nodeid
      inner join nodetypes nt on n.nodetypeid = nt.nodetypeid
        inner join object_class oc on nt.objectclassid = oc.objectclassid
        where oc.objectclass = 'MaterialClass') mat on m.materialid = mat.nodeid
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
      and mat.istier2 = '1';

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
          if containers(i).materialid = materials(j).materialid then
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
    Locations := GET_LOCATIONS();
    for loc in 1..Locations.count loop
      Materials := GET_MATERIALS(Locations(loc).LocationId);
      if Materials.exists(Materials.first) then
        for mat in Materials.first..Materials.last loop
          if Materials.exists(mat) then
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
  end SET_TIER_II_DATA;

END TIER_II_DATA_MANAGER;" );

            #endregion TIER_II_DATA_MANAGER

            #region UNIT_CONVERSION

            public static readonly PackageBodies UNIT_CONVERSION_BODY = new PackageBodies( CswDeveloper.BV, 28247,
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
    converted_value := value_to_convert / old_conversion_factor * specific_gravity * new_conversion_factor;
    return converted_value;
  end CONVERT_UNIT;

END UNIT_CONVERSION;" );

            #endregion UNIT_CONVERSION
        }

    }//class CswUpdateSchemaPLSQLPackages

}//namespace ChemSW.Nbt.Schema