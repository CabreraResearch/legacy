begin
	execute immediate 'drop table nbtimportqueue';
  exception when others then null;
end;
/
begin
  execute immediate 'drop sequence seq_nbtimportqueueid';
  exception when others then null;
end;
/


-- Create nbtimportqueue table

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
create sequence seq_nbtimportqueueid start with 1 increment by 1;
commit;

create or replace procedure pivotPropertiesValues(viewname in varchar, propstblname in varchar, proptblpkcol in varchar, joincol in varchar, fromtbl in varchar) is

  props_sql VARCHAR2(200);
  TYPE cur_typ IS REF CURSOR;
  c       cur_typ;
  propid  varchar(200);
  viewsql clob;
  cols    clob;
  joins   clob;
  withs   clob;

begin
  props_sql := 'select distinct pv.propertyid from properties p join ' ||
               propstblname || ' pv on p.propertyid = pv.propertyid';

  open c for props_sql;
  loop
    fetch c
      into propid;
    exit when c%NOTFOUND;
  
    withs := withs || ' pv' || propid || ' as (select ' || joincol ||
             ', max(' || proptblpkcol || ') as prop' || propid || ' from ' ||
             propstblname || ' where propertyid = ' || propid ||
             ' group by ' || joincol || '),';
    cols  := cols || ', pv' || propid || '.prop' || propid;
    joins := joins || ' left outer join pv' || propid || ' on pv' || propid || '.' ||
             joincol || ' = m.' || joincol;
  
  end loop;
  close c;

  withs   := dbms_lob.substr(withs, (dbms_lob.getlength(withs) - 1), 1);
  viewsql := 'create or replace view ' || viewname || ' as ';
  if dbms_lob.getlength(withs) > 1 then
    viewsql := viewsql || ' with ' || withs;
  end if;
  viewsql := viewsql || '
    select m. ' || joincol || cols || '
      from ' || fromtbl || ' m ' || joins;

  execute immediate(viewsql);
end;
/

begin
  -- Call the procedure (Chemical Properties)
  pivotpropertiesvalues(viewname => 'chemicals_props_view',
                        propstblname => 'properties_values',
                        proptblpkcol => 'propertiesvaluesid',
                        joincol => 'materialid',
						fromtbl => 'materials');
end;
/
begin
  -- Call the procedure (Receipt Lot Properties)
  pivotpropertiesvalues(viewname => 'receiptlots_props_view',
                        propstblname => 'properties_values_lot',
                        proptblpkcol => 'lotpropsvaluesid',
                        joincol => 'receiptlotid',
						fromtbl => 'receipt_lots');
end;
/
begin
  -- Call the procedure (Container Properties)
  pivotpropertiesvalues(viewname => 'containers_props_view',
                        propstblname => 'properties_values_cont',
                        proptblpkcol => 'contpropsvaluesid',
                        joincol => 'containerid',
						fromtbl => 'containers');
end;
/	
begin
  -- Call the procedure (User Properties)
  pivotpropertiesvalues(viewname => 'user_props_view',
                        propstblname => 'properties_values_user',
                        proptblpkcol => 'userpropsvaluesid',
                        joincol => 'userid',
						fromtbl => 'users');
end;
/	

-- Create views ( these are in order of creation)

-- Sites
create or replace view site_view as
select siteid,
	   sitename,
       sitecode,
	   'LS' || siteid as barcode,
	   deleted
	from sites s;
	
	
-- Locations level 1
create or replace view building_view as
select l.locationid,
       'LS' || l.locationid as barcode,
       l1.locationlevel1name,
       l1.locationlevel1id,
       l1.siteid,
       l.inventorygroupid,
       l.controlzoneid,
	   l.locationcode,
       l.deleted,
       1 as allowinventory
    from locations l
    join locations_level1 l1 on (l1.locationlevel1id = l.locationlevel1id)
    where l.locationlevel2id = 0;

-- Locations level 2
create or replace view room_view as
select l.locationid,
       'LS' || l.locationid as barcode,
       l2.locationlevel2name,
       l2.locationlevel2id,
       l1l.locationid as buildingid,
       l.inventorygroupid,
       l.controlzoneid,
	   l.locationcode,
       l.deleted,
       1 as allowinventory
    from locations l
    join locations_level2 l2 on (l2.locationlevel2id = l.locationlevel2id)
    join locations l1l on (l2.locationlevel1id = l1l.locationlevel1id and l1l.locationlevel2id = 0)
    where l.locationlevel3id = 0;
	
	
-- Locations level 3
create or replace view cabinet_view as
select l.locationid,
       'LS' || l.locationid as barcode,
       l3.locationlevel3name,
       l3.locationlevel3id,
       l2l.locationid as roomid,
       l.inventorygroupid,
       l.controlzoneid,
	   l.locationcode,
       l.deleted,
       1 as allowinventory
    from locations l
    join locations_level3 l3 on (l3.locationlevel3id = l.locationlevel3id)
    join locations l2l on (l3.locationlevel2id = l2l.locationlevel2id and l2l.locationlevel3id = 0)
    where l.locationlevel4id = 0;
	
	
-- Locations level 4
create or replace view shelf_view as
select l.locationid,
       'LS' || l.locationid as barcode,
       l4.locationlevel4name,
       l4.locationlevel4id,
       l3l.locationid as cabinetid,
       l.inventorygroupid,
       l.controlzoneid,
	   l.locationcode,
       l.deleted,
       1 as allowinventory
    from locations l
    join locations_level4 l4 on (l4.locationlevel4id = l.locationlevel4id)
    join locations l3l on (l4.locationlevel3id = l3l.locationlevel3id and l3l.locationlevel4id = 0)
    where l.locationlevel5id = 0;
	
	
-- Locations level 5
create or replace view box_view as
select l.locationid,
       'LS' || l.locationid as barcode,
       l5.locationlevel5name,
       l5.locationlevel5id,
       l4l.locationid as shelfid,
       l.inventorygroupid,
       l.controlzoneid,
	   l.locationcode,
       l.deleted,
       1 as allowinventory
    from locations l
    join locations_level5 l5 on (l5.locationlevel5id = l.locationlevel5id)
    join locations l4l on (l5.locationlevel4id = l4l.locationlevel4id and l4l.locationlevel5id = 0);
	
	
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
select u.AUDITFLAG,
       u.DEFAULTCATEGORYID,
       u.DEFAULTLANGUAGE,
       u.DEFAULTLOCATIONID,
       u.DEFAULTPRINTERID,
       u.DELETED,
       u.DISABLED,
       u.EMAIL,
       u.EMPLOYEEID,
       u.FAILEDLOGINCOUNT,
       u.HIDEHINTS,
       u.HOMEINVENTORYGROUPID,
       u.ISSYSTEMUSER,
       u.LOCKED,
       u.MYSTARTURL,
       u.NAMEFIRST,
       u.NAMELAST,
       u.NAVROWS,
       u.NODEVIEWID,
       u.PASSWORD,
       u.PASSWORD_DATE,
       u.PHONE,
       u.ROLEID,
       u.SUPERVISORID,
       u.TITLE,
       u.USERNAME,
       u.WELCOMEREDIRECT,
       u.WORKUNITID,
	   upv.*
  from users u
  join user_props_view upv on u.userid = upv.userid
 where u.issystemuser != 1;

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
CREATE OR replace VIEW chemicals_view 
AS 
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
                 ), 'Ventilation', 'Fume Hood')    AS ppe_trans, 
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
           END )                                   AS physical_state_trans, 
         ( CASE m.NONHAZARDOUS3E 
             WHEN '1' THEN '0' 
             WHEN '0' THEN '1' 
           END )                                   AS nonhazardous3e_trans, 
         sc.STORAGECOMPAT                          AS storagecompatibility, 
         ph.LABELCODES, 
         pc.PICTOGRAMS, 
         p.PRODUCTDESCRIPTION, 
         cpv.*, 
         haz.CATEGORIES, 
         haz.CLASSES, 
         haz.CHEMTYPE, 
         ( CASE 
             WHEN (SELECT ENABLED 
                   FROM   modules 
                   WHERE  NAME = 'pkg_approval') = '0' THEN '1' 
             ELSE p.APPROVED 
           END )                                   approved_trans, 
         (SELECT Trim(both ',' FROM Nvl2(( CASE NOTREPORTABLE 
                                             WHEN '1' THEN 'Not Reportable' 
                                             WHEN '0' THEN NULL 
                                           END ), 'Not Reportable', '') 
                                    || ',' 
                                    || Nvl2(( CASE ISEHS 
                                                WHEN '1' THEN 'EHS' 
                                                WHEN '0' THEN NULL 
                                              END ), 'EHS', '') 
                                    || ',' 
                                    || Nvl2(( CASE ISWASTE 
                                                WHEN '1' THEN 'Waste' 
                                                WHEN '0' THEN NULL 
                                              END ), 'Waste', '')) 
          FROM   cispro_hazdata hazsub 
          WHERE  hazsub.MATERIALID = p.MATERIALID) AS special_flags 
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
         left outer join chemicals_props_view cpv 
           ON ( p.MATERIALID = cpv.MATERIALID ) 
         left outer join cispro_hazdata haz 
           ON ( p.MATERIALID = haz.MATERIALID ) 
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
       join packages p on p.materialid = d2.materialid
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
       join packages p on p.materialid = d2.materialid
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
     p.PackageId,
     c.manufacturerlotno
  from receipt_lots rl
  join containers c on c.receiptlotid = rl.receiptlotid and c.containerclass = 'lotholder'
  left outer join receiptlots_props_view rpv on rpv.receiptlotid = rl.receiptlotid
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
	  c.TareQuantity,
	  cpv.*
	from containers c
	  left outer join container_groups cg on cg.containergroupcode = c.containergroupcode
	  join packdetail pd on c.packdetailid = pd.packdetailid
	  join packages p on pd.packageid = p.packageid
	  left outer join containers_props_view cpv on cpv.containerid = c.containerid
	where c.containerclass != 'lotholder'
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
				 least(nvl(ph.deleted, 0), nvl(pc.deleted, 0), nvl(s.deleted, 0)) deleted
           FROM   phrases ph
                  full outer join pictos pc
                               ON ( pc.MATERIALID = ph.MATERIALID
                                    AND pc.REGION = ph.REGION )
                  full outer join signals s
                               ON ( s.MATERIALID = coalesce(ph.MATERIALID, pc.MATERIALID)
                                    AND s.REGION = coalesce(ph.REGION, pc.REGION) )
                  join packages p
                    ON ( p.MATERIALID = coalesce(ph.MATERIALID, pc.materialid, s.materialid )));
					
--Reglists
CREATE OR replace VIEW reglists_view 
AS 
  (SELECT rl.deleted, 
          rl.displayname, 
          rl.listmode, 
          rl.matchtype, 
          rl.reglistcode, 
          rl.regulatorylistid
   FROM   regulatory_lists rl
   WHERE  Lower(listmode) = 'cispro'   
   AND    (select count(r.regulatorylistid) from regulated_casnos r where r.regulatorylistid = rl.regulatorylistid) > 0
   ); 

--Material Synonyms
create or replace view synonyms_view as
(
  select ms.synonymname,
  ms.synonymclass,
  ms.materialsynonymid,
  cv.packageid,
  ms.materialsynonymid || '_' || cv.packageid as LegacyId,
  ms.deleted
  from materials_synonyms ms
       join chemicals_view cv on ms.materialid = cv.materialid
);

--Roles
create or replace view roles_view as
select distinct r.roleid,
                r.rolename,
                r.roledescription,
                r.timeout,
                max(case
                      when fpd.featurepermissionname = 'system' and nonepm = 1 then
                       1
                      when fpd.featurepermissionname = 'system_site' and
                           nonepm = 1 then
                       1
                      else
                       0
                    end) as administrator,
                r.deleted
  from roles r
  join permissions_by_role rp on (rp.roleid = r.roleid)
  join feature_permission_definitions fpd on (fpd.featurepermissiondefinitionid =
                                             rp.featurepermissiondefinitionid)
 where r.deleted = 0
   and r.issystem = 0
 group by r.roleid,
          r.rolename,
          r.roledescription,
          r.timeout,
          r.deleted;
		  
--Materials: Biologicals
create or replace view biologicals_view as
SELECT p.productno,
       m.materialname,
       v.vendorid,
       m.refno,
       m.type,
       m.species,
       m.biosafety,
       m.vectors,
       m.materialid,
       p.packageid,
       (CASE
         WHEN (SELECT ENABLED FROM modules WHERE NAME = 'pkg_approval') = '0' THEN
          '1'
         ELSE
          p.APPROVED
       END) approved_trans,
       m.storage_conditions,
       m.deleted
  FROM materials m
  join packages p ON p.MATERIALID = m.MATERIALID
  join vendors v ON p.SUPPLIERID = v.VENDORID
  join materials_subclass ms ON ms.MATERIALSUBCLASSID =
                                m.MATERIALSUBCLASSID
  join materials_class mc ON mc.MATERIALCLASSID = ms.MATERIALCLASSID
 WHERE m.DELETED = 0
   AND p.DELETED = 0
   AND mc.CLASSNAME = 'BIOLOGICAL';

--Materials: Supplies
create or replace view supplies_view as
SELECT p.productno,
       m.materialname,
       v.vendorid,
       p.productdescription,
       m.materialid,
       p.packageid,
       (CASE
         WHEN (SELECT ENABLED FROM modules WHERE NAME = 'pkg_approval') = '0' THEN
          '1'
         ELSE
          p.APPROVED
       END) approved_trans,
	   m.deleted
  FROM materials m
  join packages p ON p.MATERIALID = m.MATERIALID
  join vendors v ON p.SUPPLIERID = v.VENDORID
  join materials_subclass ms ON ms.MATERIALSUBCLASSID =
                                m.MATERIALSUBCLASSID
  join materials_class mc ON mc.MATERIALCLASSID = ms.MATERIALCLASSID
 WHERE m.DELETED = 0
   AND p.DELETED = 0
   AND mc.CLASSNAME = 'SUPPLY';
   
--Material Components
create or replace view materialcomps_view as
select componentcasnoid || '_' || pk.packageid as legacyid,
       mc.materialid || '_' || componentcasnoid as constituentid,
       pk.packageid,
       quantity,
       mc.deleted
  from component_casnos mc
  join packages pk on pk.materialid = mc.materialid
 where mc.deleted = 0
   and mc.componentmaterialid is null

 union

 select componentcasnoid || '_' || pk.packageid as legacyid,
       '' || mc.componentmaterialid as constituentid,
       pk.packageid a,
       quantity,
       mc.deleted
  from component_casnos mc
  join packages pk on pk.materialid = mc.materialid
 where mc.deleted = 0
   and pk.deleted = 0
   and mc.componentmaterialid is not null;

--Constituents
CREATE OR REPLACE VIEW CONSTITUENTS_VIEW AS
select mc.materialid || '_' || mc.componentcasnoid as legacyid,
       mc.componentname as name,
       mc.casno,
       '' as einecs,
       deleted
  from component_casnos mc
 where mc.deleted = 0
   and mc.componentmaterialid is null

 union

 SELECT '' || m.materialid as legacyid,
        m.materialname as name,
        m.casno,
        m.einecs,
        m.deleted
  FROM materials m
  join materials_subclass ms ON ms.MATERIALSUBCLASSID =
                                m.MATERIALSUBCLASSID
  join materials_class mc ON mc.MATERIALCLASSID = ms.MATERIALCLASSID
  join component_casnos cc on cc.componentmaterialid = m.materialid

   
