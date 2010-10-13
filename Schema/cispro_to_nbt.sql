
insert into materials (materialid, nodetypeid, nodename, deleted, pendingupdate, 
auditflag, biosafety, color, compressed_gas, const_chem_group, const_chem_react, const_coe_no, const_color_index, 
const_fema_no, const_ingred_class, const_mat_function, const_mat_origin, const_simple_name, const_uba_code, creationworkunitid, 
dot_code, goi, has_activity, istier2, keepatstatus, lastupdated, lob_type, manufacturer, material_finish, material_sizevol, material_type, 
material_use, model, productbrand, productcategory, producttype, refno, reviewstatuschangedate, reviewstatusname, 
reviewstatustype, species, specific_code, specificcode, transgenic, type, variety, vectors, otherreferenceno, 
aqueous_solubility, boiling_point, casno, creationdate, creationsiteid, creationsite, creationuserid, creationuser, einecs, 
expireinterval, expireintervalunits, exposure_limits, firecode, flash_point, formula, hazards, healthcode, inventoryrequired, 
keywords, materialname, melting_point, molecular_weight, nfpacode, openexpireinterval, openexpireintervalunits, ph, physical_description, 
physical_state, ppe, reactivecode, specific_gravity, storage_conditions, target_organs, vapor_density, vapor_pressure, materialsubclassid, 
materialsubclass)
select materialid, 679, materialname, deleted, pendingupdate, 
auditflag, biosafety, color, compressed_gas, const_chem_group, const_chem_react, const_coe_no, const_color_index, 
const_fema_no, const_ingred_class, const_mat_function, const_mat_origin, const_simple_name, const_uba_code, creationworkunitid, 
dot_code, goi, has_activity, istier2, keepatstatus, lastupdated, lob_type, manufacturer, material_finish, material_sizevol, material_type, 
material_use, model, productbrand, productcategory, producttype, refno, reviewstatuschangedate, reviewstatusname, 
reviewstatustype, species, specific_code, specificcode, transgenic, type, variety, vectors, otherreferenceno, 
aqueous_solubility, boiling_point, casno, creationdate, creationsiteid, 
(select sitename from sites@cisprolink where siteid = materials.creationsiteid), 
creationuserid,
(select username from users@cisprolink where userid = materials.creationuserid),
einecs, 
expireinterval, expireintervalunits, exposure_limits, firecode, flash_point, formula, hazards, healthcode, inventoryrequired, 
keywords, materialname, melting_point, molecular_weight, nfpacode, openexpireinterval, openexpireintervalunits, ph, physical_description, 
physical_state, ppe, reactivecode, specific_gravity, storage_conditions, target_organs, vapor_density, vapor_pressure, materialsubclassid, 
(select subclassname from materials_subclass@cisprolink where materialsubclassid = materials.materialsubclassid)
from materials@cisprolink;

insert into packages (packageid, nodetypeid, nodename, deleted, pendingupdate, 
materialid, materialname, manufacturerid, manufacturer, supplierid, supplier, 
productno, productdescription, obsolete)
select packageid, 674, productno, deleted, 0, 
materialid, 
(select SUBSTR(materialname, 0, 100) from materials@cisprolink where materialid = packages.materialid), 
manufacturerid, 
(select vendorname from vendors@cisprolink where vendorid = packages.manufacturerid),
supplierid, 
(select vendorname from vendors@cisprolink where vendorid = packages.supplierid), 
productno, productdescription, obsolete from packages@cisprolink;

insert into vendors (vendorid, nodetypeid, nodename, deleted, pendingupdate, 
vendorname, division, accountno, contactname, phone, fax, email, 
street1, street2, city, state, zip, country, isapprovedvendor, obsolete)
select vendorid, 675, vendorname, deleted, 0, 
vendorname, division, accountno, contactname, phone, fax, email, 
street1, street2, city, state, zip, country, isapprovedvendor, obsolete
from vendors@cisprolink;

insert into packdetail (packdetailid, nodetypeid, nodename, deleted, pendingupdate, 
qtypereach, uompereach, containertype, unitcount, catalogno, upc, packageid, packagename, capacity, unitofmeasureid, 
unitofmeasurename, packagedescription, dispenseonly)
select packdetailid, 676, catalogno, deleted, 0, 
qtypereach, uompereach, containertype, unitcount, catalogno, upc, packageid,
(select productno from packages@cisprolink where packageid = packdetail.packageid), 
capacity, unitofmeasureid, 
(select unitofmeasurename from units_of_measure@cisprolink where unitofmeasureid = packdetail.unitofmeasureid), 
packagedescription, dispenseonly
from packdetail@cisprolink;

insert into containers (containerid,nodetypeid, nodename, deleted, pendingupdate, 
auditflag, conc_mass_uomid, conc_vol_uomid, concentration, customstatus, esigflag, receiptlotid, reconcileddate, 
reconcileduserid, reconcilestate, reconciliationid, reservegroupid, reservetype, retestdate, serialno, specificactivity, 
specificactivity_mass_uom, storpress, stortemp, useridmovereq, usetype, assetno, barcodeid, containerclass, containergroupcode, 
containerstatus, expirationdate, locationid, locationname, locationidhome, homelocationname, locationidmovereq, movereqlocationname, 
manufactureddate, manufacturerlotno, materialsynonymid, synonymname, netquantity, unitofmeasurename, tarequantity, tareunitofmeasurename, 
notes, openeddate, parentid, parentname, originalparentid, originalparentname, ownerid, ownername, packdetailid, packdetail, projectid, 
project, receivedcondition, receiveddate)
select containerid, 677, barcodeid, deleted, 0, 
auditflag, conc_mass_uomid, conc_vol_uomid, concentration, customstatus, esigflag, receiptlotid, reconcileddate, 
reconcileduserid, reconcilestate, reconciliationid, reservegroupid, reservetype, retestdate, serialno, specificactivity, 
specificactivity_mass_uom, storpress, stortemp, useridmovereq, usetype, assetno, barcodeid, containerclass, containergroupcode, 
containerstatus, expirationdate, locationid, 
(select pathname from locations@cisprolink where locationid = containers.locationid),
locationidhome, 
(select pathname from locations@cisprolink where locationid = containers.locationidhome),
locationidmovereq, 
(select pathname from locations@cisprolink where locationid = containers.locationidmovereq),
manufactureddate, manufacturerlotno, materialsynonymid, 
(select synonymname from materials_synonyms@cisprolink where materialsynonymid = containers.materialsynonymid), 
netquantity, 
(select unitofmeasurename from units_of_measure@cisprolink u join packdetail@cisprolink pd on pd.unitofmeasureid = u.unitofmeasureid where pd.packdetailid = containers.packdetailid), 
tarequantity, 
(select unitofmeasurename from units_of_measure@cisprolink u join packdetail@cisprolink pd on pd.unitofmeasureid = u.unitofmeasureid where pd.packdetailid = containers.packdetailid), 
notes, openeddate, parentid, 
(select c.barcodeid from containers@cisprolink c where c.containerid = containers.parentid), 
originalparentid, 
(select c.barcodeid from containers@cisprolink c where c.containerid = containers.originalparentid),
ownerid, 
(select username from users@cisprolink where userid = ownerid), 
packdetailid, 
(select catalogno from packdetail@cisprolink where packdetailid = containers.packdetailid), 
projectid, 
(select projectname from projects@cisprolink where projectid = containers.projectid), 
receivedcondition, receiveddate
from containers@cisprolink;

insert into materials_subclass (materialsubclassid, nodetypeid, nodename, deleted, pendingupdate, subclassname)
select materialsubclassid, 678, subclassname, deleted, 0, subclassname 
from materials_subclass@cisprolink;

insert into materials_synonyms (materialsynonymid, nodetypeid, nodename, deleted, pendingupdate, 
auditflag, charset, synonymname, materialid, materialname, sortorder, synonymclass)
select materialsynonymid, 680, synonymname, deleted, 0, 
auditflag, charset, synonymname, materialid, 
(select materialname from materials@cisprolink where materialid = materials_synonyms.materialid), 
sortorder, synonymclass
from materials_synonyms@cisprolink;

insert into inventory_groups (inventorygroupid, nodetypeid, nodename, deleted, pendingupdate, 
workunitid, inventorygroupname, inventorygroupcode, iscentralgroup)
select inventorygroupid, 681, inventorygroupname, deleted, 0, 
workunitid, inventorygroupname, inventorygroupcode, iscentralgroup
from inventory_groups@cisprolink;

insert into locations (locationid, nodetypeid, nodename, deleted, pendingupdate, 
controlzoneid, locationlevel1id, locationlevel2id, locationlevel3id, locationlevel4id, locationlevel5id, descript, 
inventorygroupid, inventorygroup, ishomelocation, istransferlocation, locationcode, locationisinactive, pathname, 
reqdeliverylocation, selfservecode)
select locationid, 682, pathname, deleted, 0, 
controlzoneid, locationlevel1id, locationlevel2id, locationlevel3id, locationlevel4id, locationlevel5id, descript, 
inventorygroupid, 
(select inventorygroupname from inventory_groups@cisprolink where inventorygroupid = locations.inventorygroupid), 
ishomelocation, istransferlocation, locationcode, locationisinactive, pathname, 
reqdeliverylocation, selfservecode
from locations@cisprolink;

insert into units_of_measure (unitofmeasureid, nodetypeid, nodename, deleted, pendingupdate, 
unitofmeasurename, unittype, is_activity_type, convertfromeaches_base, convertfromeaches_exp, 
convertfromkgs_base, convertfromkgs_exp, convertfromliters_base, convertfromliters_exp, converttoeaches_base, 
converttoeaches_exp, converttokgs_base, converttokgs_exp, converttoliters_base, converttoliters_exp)
select unitofmeasureid, 683, unitofmeasurename, deleted, 0, 
unitofmeasurename, unittype, is_activity_type, convertfromeaches_base, convertfromeaches_exp, 
convertfromkgs_base, convertfromkgs_exp, convertfromliters_base, convertfromliters_exp, converttoeaches_base, 
converttoeaches_exp, converttokgs_base, converttokgs_exp, converttoliters_base, converttoliters_exp
from units_of_measure@cisprolink;

insert into users (userid, nodetypeid, nodename, deleted, pendingupdate, 
auditflag, defaultcategoryid, defaultprinterid, licenseagreementanddate, mystarturl, nodeviewid, 
password_old, welcomeredirect, workunitid, password_date, title, phone, email, supervisorid, supervisor, 
homeinventorygroupid, homeinventorygroup, defaultlocationid, defaultlocation, defaultlanguage, hidehints, 
issystemuser, disabled, navrows, username, password, roleid, namefirst, namelast, locked, failedlogincount)
select userid, 684, username, deleted, 0, 
auditflag, defaultcategoryid, defaultprinterid, licenseagreementanddate, mystarturl, nodeviewid, 
password_old, welcomeredirect, workunitid, password_date, title, phone, email, supervisorid, 
(select username from users@cisprolink u where u.userid = users.supervisorid), 
homeinventorygroupid, 
(select inventorygroupname from inventory_groups@cisprolink where inventorygroupid = users.homeinventorygroupid), 
defaultlocationid, 
(select pathname from locations@cisprolink where locationid = defaultlocationid), 
defaultlanguage, hidehints, 
issystemuser, disabled, navrows, username, password, roleid, namefirst, namelast, locked, failedlogincount
from users@cisprolink;


commit;
