-- Create nbtimportqueue table
begin
  execute immediate 'drop table nbtimportqueue';
  exception when others then null;
end;
/

create table nbtimportqueue (
  nbtimportqueueid number(12) NOT NULL PRIMARY KEY,
  state varchar(1),
  itempk varchar(255) NOT NULL,
  sheetname varchar2(50),
  priority number(12),
  errorlog varchar2(2000)
);

-- Create unique index
create unique index unqidx_nbtimportqueue on NBTIMPORTQUEUE (state, itempk, sheetname);

-- Create pk sequence for nbtimportqueue table
begin
  execute immediate 'drop sequence seq_nbtimportqueueid';
  exception when others then null;
end;
/

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
(select "AUDITFLAG","DEFAULTCATEGORYID","DEFAULTLANGUAGE","DEFAULTLOCATIONID","DEFAULTPRINTERID","DELETED","DISABLED","EMAIL","EMPLOYEEID","FAILEDLOGINCOUNT","HIDEHINTS","HOMEINVENTORYGROUPID","ISSYSTEMUSER","LOCKED","MYSTARTURL","NAMEFIRST","NAMELAST","NAVROWS","NODEVIEWID","PASSWORD","PASSWORD_DATE","PHONE","ROLEID","SUPERVISORID","TITLE","USERID","USERNAME","WELCOMEREDIRECT","WORKUNITID" from users where issystemuser != 1);

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
CREATE OR REPLACE VIEW CHEMICALS_VIEW AS
WITH dsd_phrases 
     AS (SELECT MATERIALID, 
                DELETED, 
                Listagg(CODE, ',') 
                  within GROUP( ORDER BY code) AS labelcodes 
         FROM   (SELECT DISTINCT p.MATERIALID, 
                                 r.CODE, 
                                 p.DELETED 
                 FROM   jct_rsphrases_materials p 
                        join rs_phrases r 
                          ON ( r.RSPHRASEID = p.RSPHRASEID ) 
                 WHERE  p.DELETED = 0) 
         GROUP  BY MATERIALID, 
                   DELETED), 
     dsd_pictos 
     AS (SELECT MATERIALID, 
                DELETED, 
                Listagg(GRAPHICFILENAME, Chr(10)) 
                  within GROUP( ORDER BY graphicfilename) AS pictograms 
         FROM   (SELECT DISTINCT p.MATERIALID, 
                                 'Images/cispro/DSD/' 
                                 || r.GRAPHICFILENAME AS graphicfilename, 
                                 p.DELETED 
                 FROM   jct_pictograms_materials p 
                        join pictograms r 
                          ON ( r.PICTOGRAMID = p.PICTOGRAMID ) 
                 WHERE  p.DELETED = 0) 
         GROUP  BY MATERIALID, 
                   DELETED), 
     storagecompat 
     AS (SELECT MATERIALID, 
                DELETED, 
                Listagg(GRAPHICFILENAME, Chr(10)) 
                  within GROUP( ORDER BY graphicfilename) AS storagecompat 
         FROM   (SELECT DISTINCT p.MATERIALID, 
                                 'Images/cispro/' 
                                 || r.GRAPHICFILENAME AS graphicfilename, 
                                 p.DELETED 
                 FROM   jct_graphics_materials p 
                        join graphic_sets r 
                          ON ( r.GRAPHICSETID = p.GRAPHICSETID ) 
                 WHERE  p.DELETED = 0) 
         GROUP  BY MATERIALID, 
                   DELETED) 
SELECT v.VENDORID, 
       p.PACKAGEID, 
       p.PRODUCTNO, 
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
       m.PPE, 
       Replace(Replace(Replace(m.PPE, 'Eye Protection', 'Goggles'), 
               'Hand Protection', 
               'Gloves' 
               ), 'Ventilation', 'Fume Hood') AS ppe_trans, 
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
       ms.SUBCLASSNAME, 
       ( CASE m.PHYSICAL_STATE 
           WHEN 'S' THEN 'Solid' 
           WHEN 'L' THEN 'Liquid' 
           WHEN 'G' THEN 'Gas' 
         END )                                AS physical_state_trans, 
       ( CASE m.NONHAZARDOUS3E 
           WHEN '1' THEN '0' 
           WHEN '0' THEN '1' 
         END )                                AS nonhazardous3e_trans, 
       sc.STORAGECOMPAT                       AS storagecompatibility, 
       ph.LABELCODES, 
       pc.PICTOGRAMS 
FROM   materials m 
       join packages p 
         ON p.MATERIALID = m.MATERIALID 
       join vendors v 
         ON p.SUPPLIERID = v.VENDORID 
       join materials_subclass ms 
         ON ms.MATERIALSUBCLASSID = m.MATERIALSUBCLASSID 
       join materials_class mc 
         ON mc.MATERIALCLASSID = ms.MATERIALCLASSID 
       left outer join storagecompat sc 
                    ON ( sc.MATERIALID = p.MATERIALID ) 
       left outer join dsd_phrases ph 
                    ON ( ph.MATERIALID = p.MATERIALID ) 
       left outer join dsd_pictos pc 
                    ON ( pc.MATERIALID = p.MATERIALID ) 
WHERE  m.DELETED = 0 
       AND p.DELETED = 0 
       AND mc.CLASSNAME = 'CHEMICAL'; 
	 
---Weight
create or replace view weight_view as
select unitofmeasurename,
       unittype,
       converttokgs_base as conversionfactor,
       decode(converttokgs_exp, 0,1, converttokgs_exp) as conversionfactorexp,
       deleted,
       unitofmeasureid
from units_of_measure
where lower(unittype)='weight';

---Volume
create or replace view volume_view as
select unitofmeasurename,
       unittype,
       converttoliters_base as conversionfactor,
       decode(converttoliters_exp, 0,1, converttoliters_exp) as conversionfactorexp,
       deleted,
       unitofmeasureid
from units_of_measure
where lower(unittype)='volume';

---Each
create or replace view each_view as
select unitofmeasurename,
       unittype,
       converttoeaches_base as conversionfactor,
       decode(converttoeaches_exp, 0,1, converttoeaches_exp) as conversionfactorexp,
       deleted,
       unitofmeasureid
from units_of_measure
where lower(unittype)='each';

create or replace view sds_view as(
  select * from (select 
  d.documentid,
  d.packageid,
  d.acquisitiondate,
  d.captureddate,
  d.content_type,
  d.deleted,
  nvl(d.description, '[blank]') as description,
  d.docisexternal,
  d.doctype,
  d.fileextension,
  d.filename,
  d.language,
  d.materialid,
  d.documentid || '_' || d.packageid as legacyid,
  (case d.fileextension
        when 'URL' then 'Link'
        else 'File'
   end) as fileextension_trans,
   (case d.language
   when 'english'    then 'en'
   when 'french'     then 'fr'
   when 'german'     then 'de'
   when 'danish'     then 'da'
   when 'dutch'      then 'nl'
   when 'spanish'    then 'es'
   when 'italian'    then 'it'
   when 'chinese'    then 'zh'
   when 'portuguese' then 'pt'
   when 'USA/en'     then 'en'
  end) as language_trans
 from documents d 
 where d.packageid is not null and doctype = 'MSDS' and d.deleted = 0
union all
select 
  d2.documentid,
  p.packageid,
  d2.acquisitiondate,
  d2.captureddate,
  d2.content_type,
  d2.deleted,
  nvl(d2.description, '[blank]') as description,
  d2.docisexternal,
  d2.doctype,
  d2.fileextension,
  d2.filename,
  d2.language,
  d2.materialid,
  d2.documentid || '_' || p.packageid as legacyid,
  (case d2.fileextension
   when 'URL' then 'Link'
   else 'File'
  end) as fileextension_trans,
  (case d2.language
   when 'english'    then 'en'
   when 'french'     then 'fr'
   when 'german'     then 'de'
   when 'danish'     then 'da'
   when 'dutch'      then 'nl'
   when 'spanish'    then 'es'
   when 'italian'    then 'it'
   when 'chinese'    then 'zh'
   when 'portuguese' then 'pt'
   when 'USA/en'     then 'en'
  end) as language_trans
 from documents d2 
       join materials m on m.materialid = d2.materialid
       join packages p on m.materialid = p.packageid
       where d2.packageid is null and doctype = 'MSDS' and d2.deleted = 0
)
);

create or replace view docs_view as
(
  (select
  d.documentid,
  d.packageid,
  d.acquisitiondate,
  d.captureddate,
  d.content_type,
  d.deleted,
  nvl(d.description, '[blank]') as description,
  d.docisexternal,
  d.doctype,
  d.fileextension,
  d.filename,
  d.language,
  d.materialid,
  d.documentid || '_' || d.packageid as legacyid,
  (case d.fileextension
        when 'URL' then 'Link'
        else 'File'
   end) as fileextension_trans,
   (case d.language
   when 'english'    then 'en'
   when 'french'     then 'fr'
   when 'german'     then 'de'
   when 'danish'     then 'da'
   when 'dutch'      then 'nl'
   when 'spanish'    then 'es'
   when 'italian'    then 'it'
   when 'chinese'    then 'zh'
   when 'portuguese' then 'pt'
   when 'USA/en'     then 'en'
  end) as language_trans
 from documents d
 where d.packageid is not null and doctype = 'DOC' and d.deleted = 0
union all
select
  d2.documentid,
  p.packageid,
  d2.acquisitiondate,
  d2.captureddate,
  d2.content_type,
  d2.deleted,
  nvl(d2.description, '[blank]') as description,
  d2.docisexternal,
  d2.doctype,
  d2.fileextension,
  d2.filename,
  d2.language,
  d2.materialid,
  d2.documentid || '_' || p.packageid as legacyid,
  (case d2.fileextension
   when 'URL' then 'Link'
   else 'File'
  end) as fileextension_trans,
  (case d2.language
   when 'english'    then 'en'
   when 'french'     then 'fr'
   when 'german'     then 'de'
   when 'danish'     then 'da'
   when 'dutch'      then 'nl'
   when 'spanish'    then 'es'
   when 'italian'    then 'it'
   when 'chinese'    then 'zh'
   when 'portuguese' then 'pt'
   when 'USA/en'     then 'en'
  end) as language_trans
 from documents d2
       join materials m on m.materialid = d2.materialid
       join packages p on m.materialid = p.packageid
       where d2.packageid is null and doctype = 'DOC' and d2.deleted = 0
)
);

---Receipt Lots
create or replace view receipt_lots_view as
select 
     rl.ReceiptLotNo,
	   rl.CreatedDate,
	   rl.ReceiptLotId,
	   rl.Deleted,
	   p.PackageId
  from receipt_lots rl
	join packages p 
       on p.packageid = (
	                    select pd.packageid 
                            from packdetail pd, containers c 
								where c.receiptlotid = rl.receiptlotid and 
										c.packdetailid = pd.packdetailid and
										rownum=1
						);
	
	
---C of A Documents
create or replace view cofa_docs_view as
select ReceiptLotId,
	    CA_FileName,
		CA_AcquisitionDate,
		CA_Content_Type,
        (case CA_fileextension
         when 'URL' then 'Link'
         else 'File'
         end) as FileExtension,
		Deleted
	from receipt_lots;
	 
---Containers
create or replace view containers_view as
select
	 c.ContainerId,
	  c.Deleted,
	  c.BarcodeId,
	  c.PackDetailId,
	  c.ParentId,
	  cg.ContainerGroupId,
	  c.OwnerId,
	  c.ContainerStatus,
	  c.ReceiptLotId,
	  p.PackageId,
	  c.NetQuantity,
	  pd.UnitOfMeasureId,
	  c.ExpirationDate,
	  c.LocationId,
	  c.StorPress,
	  c.StorTemp,
	  c.UseType,
	  c.ReceivedDate,
	  c.OpenedDate,
	  c.Concentration || ' ' || c.Conc_Mass_UoMId || '/' || c.Conc_Vol_UoMId as Concentration,
	  c.LocationIdHome as HomeLocation,
	  c.Notes,
	  c.ProjectId,
	  c.SpecificActivity,
	  c.TareQuantity
	from containers c
	  left outer join container_groups cg on cg.containergroupcode = c.containergroupcode
	  join packdetail pd on c.packdetailid = pd.packdetailid
	  join packages p on pd.packageid = p.packageid
	order by c.ContainerId;
	  
---Inventory Levels
create or replace view inventory_view as
	select
	m.DELETED,
	'min_' || m.MININVENTORYBASICID as inventorybasicid,
	m.MININVENTORYLEVEL as inventorylevel,
	m.MININVENTORYUNITOFMEASUREID as unitofmeasureid,
	m.PACKAGEID,
	loc.locationid,
	'Minimum' as inventorytype
	from mininventory_basic m
	inner join
	(
	  select 
	  toploc.inventorygroupid, 
	  toploc.locationid 
	  from (
		select 
		l.locationid, 
		l.inventorygroupid, 
		row_number() over (partition by l.inventorygroupid order by l.locationid) as rn 
		from locations l
		order by l.locationlevel5id, l.locationlevel4id, l.locationlevel3id, l.locationlevel2id, l.locationlevel1id
	  ) toploc where toploc.rn = 1
	) loc on loc.inventorygroupid = m.inventorygroupid
union
	select
		m.DELETED,
		'max_' || m.MAXINVENTORYBASICID as inventorybasicid,
		m.MAXINVENTORYLEVEL as inventorylevel,
		m.UNITOFMEASUREID,
		max(p.packageid) as packageid,
	  m.LOCATIONID,
		'Maximum' as inventorytype
	from 
		maxinventory_basic m
		join packages p on p.materialid = m.materialid
	group by 
		m.DELETED,
		m.LOCATIONID,
		m.MATERIALID,
		m.MAXINVENTORYBASICID,
		m.MAXINVENTORYLEVEL,
		m.UNITOFMEASUREID;
		
---GHS
create or replace view regions_view as
select distinct region, deleted from sites where deleted = 0;

CREATE OR REPLACE VIEW GHS_VIEW AS
SELECT PACKAGEID,
         PACKAGEID
         || '_'
         || REGION AS legacyid,
         MATERIALID,
         REGION,
         GHSCODES,
         PICTOS,
         SIGNAL,
         DELETED
  FROM   (WITH mappedpictos
               AS (SELECT GHSPICTOID,
                          CASE PICTOFILENAME
                            WHEN 'acide.gif' THEN
                            'Images/cispro/ghs/512/acid.jpg'
                            WHEN 'bottle.gif' THEN
                            'Images/cispro/ghs/512/bottle.jpg'
                            WHEN 'exclam.gif' THEN
                            'Images/cispro/ghs/512/exclam.jpg'
                            WHEN 'explos.gif' THEN
                            'Images/cispro/ghs/512/explos.jpg'
                            WHEN 'flamme.gif' THEN
                            'Images/cispro/ghs/512/flamme.jpg'
                            WHEN 'pollu.gif' THEN
                            'Images/cispro/ghs/512/pollut.jpg'
                            WHEN 'rondfl.gif' THEN
                            'Images/cispro/ghs/512/rondflam.jpg'
                            WHEN 'siloue.gif' THEN
                            'Images/cispro/ghs/512/silhouet.jpg'
                            WHEN 'skull.gif' THEN
                            'Images/cispro/ghs/512/skull.jpg'
                          END AS pictofilename
                   FROM   ghs_pictos),
               pictos
               AS (SELECT REGION,
                          MATERIALID,
                          DELETED,
                          Listagg(PICTOFILENAME, Chr(10))
                            within GROUP( ORDER BY pictofilename) AS pictos
                   FROM   (SELECT DISTINCT s.REGION,
                                           p.MATERIALID,
                                           g.PICTOFILENAME,
                                           p.DELETED
                           FROM   jct_ghspictos_matsite p
                                  join sites s
                                    ON ( s.SITEID = p.SITEID )
                                  join mappedpictos g
                                    ON ( g.GHSPICTOID = p.GHSPICTOID )
                           WHERE  p.DELETED = 0)
                   GROUP  BY REGION,
                             MATERIALID,
                             DELETED),
               phrases
               AS (SELECT REGION,
                          MATERIALID,
                          DELETED,
                          Listagg(GHSCODE, ',')
                            within GROUP( ORDER BY ghscode) AS ghscodes
                   FROM   (SELECT DISTINCT s.REGION,
                                           p.MATERIALID,
                                           g.GHSCODE,
                                           p.DELETED
                           FROM   jct_ghsphrase_matsite p
                                  join sites s
                                    ON ( s.SITEID = p.SITEID )
                                  join ghs_phrases g
                                    ON ( g.GHSPHRASEID = p.GHSPHRASEID )
                           WHERE  p.DELETED = 0)
                   GROUP  BY REGION,
                             MATERIALID,
                             DELETED),
               signals
               AS (SELECT DISTINCT s.REGION,
                                   p.MATERIALID,
                                   g.ENGLISH,
                                   p.DELETED
                   FROM   jct_ghssignal_matsite p
                          join sites s
                            ON ( s.SITEID = p.SITEID )
                          join ghs_signal g
                            ON ( g.GHSSIGNALID = p.GHSSIGNALID )
                   WHERE  p.DELETED = 0),
               classes
               AS (SELECT DISTINCT MATERIALID,
                                   GHSCATEGORYNAME,
                                   m.DELETED
                   FROM   jct_ghs_materials m
                          join ghs_classes c
                            ON ( c.GHSCLASSID = m.GHSCLASSID )
                   WHERE  m.DELETED = 0)
          SELECT p.PACKAGEID,
                 CASE
                   WHEN ph.MATERIALID IS NOT NULL THEN ph.MATERIALID
                   WHEN pc.MATERIALID IS NOT NULL THEN pc.MATERIALID
                   WHEN s.MATERIALID IS NOT NULL THEN s.MATERIALID
                 END AS materialid,
                 CASE
                   WHEN ph.REGION IS NOT NULL THEN ph.REGION
                   WHEN pc.REGION IS NOT NULL THEN pc.REGION
                   WHEN s.REGION IS NOT NULL THEN s.REGION
                 END AS region,
                 ph.GHSCODES,
                 pc.PICTOS,
                 CASE
                   WHEN s.ENGLISH IS NULL THEN 'Warning'
                   ELSE s.ENGLISH
                 END AS signal,
                 ph.DELETED
           FROM   phrases ph
                  full outer join pictos pc
                               ON ( pc.MATERIALID = ph.MATERIALID
                                    AND pc.REGION = ph.REGION )
                  full outer join signals s
                               ON ( s.MATERIALID = ph.MATERIALID
                                    AND s.REGION = ph.REGION )
                  join packages p
                    ON ( p.MATERIALID = ph.MATERIALID ));