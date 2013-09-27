-- Create nbtimportqueue table
create table nbtimportqueue (
	nbtimportqueueid number(12) NOT NULL PRIMARY KEY,
  state varchar(1),
  itempk number(12) NOT NULL,
  sheetname varchar2(50),
  priority number(12),
  errorlog varchar2(2000)
);

-- Create unique index
create unique index unqidx_nbtimportqueue on NBTIMPORTQUEUE (state, itempk, tablename);

-- Create pk sequence for nbtimportqueue table
create sequence seq_nbtimportqueueid start with 1 increment by 1;
commit;

-- Create views ( these are in order of creation)
-- Locations level 1
create or replace view locationslevel1_view as
select l.locationid,
l1.locationlevel1id
    from locations l
  join locations_level1 l1 on (l1.locationlevel1id = l.locationlevel1id)
 where l.locationlevel2id = 0
   and l.deleted = 0
   and l1.deleted = 0;

-- Locations level 2
create or replace view locationslevel2_view as
select l.locationid,
l2.locationlevel2id
  from locations l
  join locations_level2 l2 on (l2.locationlevel2id = l.locationlevel2id)
 where l.locationlevel2id != 0
   and l.locationlevel3id = 0
   and l.deleted = 0
   and l2.deleted = 0;

-- Locations level 3
create or replace view locationslevel3_view as
select l.locationid,l3.locationlevel3id
  from locations l
  join locations_level3 l3 on (l3.locationlevel3id = l.locationlevel3id)
 where l.deleted = 0
   and l3.deleted = 0
   and l.locationlevel3id != 0
   and l.locationlevel4id = 0;

-- Locations level 4
create or replace view locationslevel4_view as
select l.locationid,
       l4.locationlevel4id
  from locations l
  join locations_level4 l4 on (l4.locationlevel4id = l.locationlevel4id)
 where l.deleted = 0 and l4.deleted = 0 and l.locationlevel4id != 0 and l.locationlevel5id = 0;
 
-- Locations level 5
create or replace view locationslevel5_view as
select l.locationid,
       l5.locationlevel5id
  from locations l
  join locations_level5 l5 on (l5.locationlevel5id = l.locationlevel5id)
 where l.deleted = 0 and l5.deleted = 0 and l.locationlevel5id != 0;
 
-- Locations
create or replace view locations_view as
with temp as (select 1 as allowinventory from dual)
select s.siteid,
       s.sitename,
       s.sitecode,
       l.locationid,
       l.inventorygroupid,
       l.controlzoneid,
       l.locationcode,
       ll1.locationlevel1name,
              ll1v.locationid as buildingid,
       ll2.locationlevel2name,
       ll2v.locationid as roomid,
       ll3.locationlevel3name,
       ll3v.locationid as cabinetid,
       ll4.locationlevel4name,
       ll4v.locationid as shelfid,
       ll5.locationlevel5name,
       ll5v.locationid as boxid,
       l.deleted,
       t.allowinventory
  from temp t, locations l
  full outer join locationslevel1_view ll1v on (ll1v.locationlevel1id = l.locationlevel1id)
  full outer join locationslevel2_view ll2v on (ll2v.locationlevel2id = l.locationlevel2id)
  full outer join locationslevel3_view ll3v on (ll3v.locationlevel3id = l.locationlevel3id)
  full outer join locationslevel4_view ll4v on (ll4v.locationlevel4id = l.locationlevel4id)
  full outer join locationslevel5_view ll5v on (ll5v.locationlevel5id = l.locationlevel5id)
  join locations_level1 ll1 on l.locationlevel1id = ll1.locationlevel1id
  join sites s on ll1.siteid = s.siteid
  left outer join locations_level2 ll2 on l.locationlevel2id = ll2.locationlevel2id
  left outer join locations_level3 ll3 on l.locationlevel3id = ll3.locationlevel3id
  left outer join locations_level4 ll4 on l.locationlevel4id = ll4.locationlevel4id
  left outer join locations_level5 ll5 on l.locationlevel5id = ll5.locationlevel5id
 where l.deleted = 0;
 
-- Work Units
create or replace view workunits_view as
select w.businessunitid,
       w.deleted,
       w.expiryinterval,
       w.HOMEINVENTORYGROUPID,
       w.RETESTINTERVALDEFAULT,
       w.RETESTWARNDAYS,
       w.SITEID,
       w.SKIPLOTCODEDEFAULT,
       w.STDAPPROVALMODE,
       w.WORKUNITID,
       w.EXPIRYINTERVALUNITS,
       w.MININVENTORYLEVEL,
       w.MININVENTORYUNITOFMEASUREID,
       w.RETAINCOUNT,
       w.RETAINKEEPYEARS,
       w.RETAINQUANTITY,
       w.RETAINUNITOFMEASUREID,
       w.AUTOLOTAPPROVAL,
       w.STDEXPIRYINTERVAL,
       w.STDEXPIRYINTERVALUNITS,
       w.SAMPLECOLLECTIONREQUIRED,
       w.CANOVERREQ,
       w.CANORDERDRAFT,
       w.DISPENSE_PERCENT,
       w.AMOUNTENABLE,
       w.CANSYNCHCONTAINERS,
       w.SRIREVIEWGROUPID,
       w.DEF_REQASCHILD,
       w.REMOVEGROUPONDISPENSE,
       w.REMOVEGROUPONMOVE,
       w.ALLOC1,
       w.ALLOC2,
       w.ALLOC3,
       w.ALLOC4,
       w.ALLOC5,
       s.sitename || ' ' || b.businessunitname as workunitname
  from work_units w
  left outer join business_units b on (b.businessunitid = w.businessunitid)
  left outer join sites s on (s.siteid = w.siteid);

--Users
create or replace view users_view as
(select "AUDITFLAG","DEFAULTCATEGORYID","DEFAULTLANGUAGE","DEFAULTLOCATIONID","DEFAULTPRINTERID","DELETED","DISABLED","EMAIL","EMPLOYEEID","FAILEDLOGINCOUNT","HIDEHINTS","HOMEINVENTORYGROUPID","ISSYSTEMUSER","LOCKED","MYSTARTURL","NAMEFIRST","NAMELAST","NAVROWS","NODEVIEWID","PASSWORD","PASSWORD_DATE","PHONE","ROLEID","SUPERVISORID","TITLE","USERID","USERNAME","WELCOMEREDIRECT","WORKUNITID" from users);

---Packdetail
create or replace view packdetail_view as
select packdetailid,
       packagedescription,
       packageid,
       catalogno,
       capacity,
       unitofmeasureid,
       deleted,
       dispenseonly,
       unitcount,
       upc,
       CASE containertype
            when 'A' then 'Aboveground Tank [A]'
            when 'B' then 'Belowground Tank [B]'
            when 'C' then 'Tank Inside Building [C]'
            when 'D' then 'Steel Drum [D]'
            when 'E' then 'Plastic or Non-Metal Drum [E]'
            when 'F' then 'Can [F]'
            when 'G' then 'Carboy [G]'
            when 'I' then 'Fiberdrum [I]'
            when 'J' then 'Bag [J]'
            when 'K' then 'Box [K]'
            when 'L' then 'Cylinder [L]'
            when 'M' then 'Glass Bottle or Jug [M]'
            when 'N' then 'Plastic [N]'
            when 'O' then 'Tote Bin [O]'
            when 'P' then 'Tank Wagon [P]'
       END as containertype
       from packdetail;

--Chemicals
create or replace view chemicals_view as
(select v.vendorid,
 p.packageid,
 p.productno,
 m."AQUEOUS_SOLUBILITY",
m."BOILING_POINT",
m."CASNO",
m."COLOR",
m."COMPRESSED_GAS",
m."CREATIONDATE",
m."CREATIONSITEID",
m."DELETED",
m."DOT_CODE",
m."EXPIREINTERVAL",
m."EXPIREINTERVALUNITS",
m."EXPOSURE_LIMITS",
m."FIRECODE",
m."FLASH_POINT",
m."FORMULA",
m."HAZARDS",
m."HEALTHCODE",
m."INVENTORYREQUIRED",
m."KEYWORDS",
m."LOB_TYPE",
m."MANUFACTURER",
m."MATERIAL_FINISH",
m."MATERIAL_SIZEVOL",
m."MATERIAL_TYPE",
m."MATERIAL_USE",
m."MATERIALID",
m."MATERIALNAME",
m."MATERIALSUBCLASSID",
m."MELTING_POINT",
m."MODEL",
m."MOLECULAR_WEIGHT",
m."OTHERREFERENCENO",
m."PH",
m."PHYSICAL_DESCRIPTION",
m."PHYSICAL_STATE",
m."PPE",
m."REACTIVECODE",
m."REVIEWSTATUSCHANGEDATE",
m."REVIEWSTATUSNAME",
m."SPEC_NO",
m."SPECIFIC_CODE",
m."SPECIFIC_GRAVITY",
m."SPECIFICCODE",
m."VAPOR_DENSITY",
m."VAPOR_PRESSURE",
m."KEEPATSTATUS",
m."TARGET_ORGANS",
m."CREATIONUSERID",
m."AUDITFLAG",
m."CREATIONWORKUNITID",
m."EINECS",
m."CONST_UBA_CODE",
m."CONST_COLOR_INDEX",
m."CONST_SIMPLE_NAME",
m."CONST_CHEM_GROUP",
m."CONST_INGRED_CLASS",
m."CONST_CHEM_REACT",
m."LASTUPDATED",
m."OPENEXPIREINTERVAL",
m."OPENEXPIREINTERVALUNITS",
m."CONST_FEMA_NO",
m."CONST_COE_NO",
m."PRODUCTTYPE",
m."PRODUCTBRAND",
m."PRODUCTCATEGORY",
m."NFPACODE",
m."CONST_MAT_FUNCTION",
m."HAS_ACTIVITY",
m."REFNO",
m."TYPE",
m."SPECIES",
m."VARIETY",
m."GOI",
m."TRANSGENIC",
m."VECTORS",
m."BIOSAFETY",
m."CONST_MAT_ORIGIN",
m."REVIEWSTATUSTYPE",
m."STORAGE_CONDITIONS",
m."PENDINGUPDATE",
m."ISTIER2",
m."MATERIALVARIETYID",
m."NONHAZARDOUS3E",
m."ASSETCREATIONNAME",

(case m.physical_state
  when 'S' then 'Solid'
  when 'L' then 'Liquid'
  when 'G' then 'Gas'
end) as physical_state_trans,

(case m.nonhazardous3e
  when '1' then '0'
  when '0' then '1'
end) as nonhazardous3e_trans

from materials m
             join packages p on p.materialid = m.materialid
             join vendors v on p.supplierid = v.vendorid
             join materials_subclass ms on ms.materialsubclassid = m.materialsubclassid
             join materials_class mc on mc.materialclassid = ms.materialclassid
     where m.deleted = 0 and p.deleted = 0 and mc.classname = 'CHEMICAL');
	 
---Weight
create or replace view weight_view as
select unitofmeasurename,
       unittype,
       convertfromkgs_base as conversionfactor,
       deleted,
       unitofmeasureid
from units_of_measure
where lower(unittype)='weight';

---Volume
create or replace view volume_view as
select unitofmeasurename,
       unittype,
       convertfromliters_base as conversionfactor,
       deleted,
       unitofmeasureid
from units_of_measure
where lower(unittype)='volume';

---Each
create or replace view each_view as
select unitofmeasurename,
       unittype,
       convertfromeaches_base as conversionfactor,
       deleted,
       unitofmeasureid
from units_of_measure
where lower(unittype)='each';