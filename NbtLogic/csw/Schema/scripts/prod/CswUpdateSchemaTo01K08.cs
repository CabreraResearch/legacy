using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{
	/// <summary>
	/// Updates the schema to version 01K-08
	/// </summary>
	public class CswUpdateSchemaTo01K08 : CswUpdateSchemaTo
	{
		public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'K', 08 ); } }
		public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

		public override void update()
		{
			// case 24200
			// Remove nodetypename and propname from relation queries

			_CswNbtSchemaModTrnsctn.UpdateS4( "getRelationsForNodeTypeId", @"select distinct 'NodeTypePropId' proptype,
       t.firstversionid typeid,
       p.firstpropversionid propid,
       p.fktype,
       p.fkvalue
  from nodetype_props p
  join nodetypes t on p.nodetypeid = t.nodetypeid
  left outer join nodetypes f on p.fkvalue = f.nodetypeid
 where fieldtypeid in
       (select fieldtypeid
          from field_types
         where fieldtype in ('Relationship', 'Location'))
   and ((t.firstversionid = :getnodetypeid) or
        (fktype = 'ObjectClassId' and
         fkvalue = (select objectclassid
                      from nodetypes
                     where nodetypeid = :getnodetypeid)) or
        (fktype = 'NodeTypeId' and
         f.firstversionid = :getnodetypeid))
   and ((exists (select j.jctmoduleobjectclassid
                  from jct_modules_objectclass j
                  join modules m on j.moduleid = m.moduleid
                 where j.objectclassid = t.objectclassid
                   and m.enabled = '1')
    or not exists (select j.jctmoduleobjectclassid
                     from jct_modules_objectclass j
                     join modules m on j.moduleid = m.moduleid
                    where j.objectclassid = t.objectclassid) )
   and (exists (select j.jctmodulenodetypeid
                  from jct_modules_nodetypes j
                  join modules m on j.moduleid = m.moduleid
                 where j.nodetypeid = t.firstversionid
                   and m.enabled = '1')
    or not exists (select j.jctmodulenodetypeid
                     from jct_modules_nodetypes j
                     join modules m on j.moduleid = m.moduleid
                    where j.nodetypeid = t.firstversionid) ))" );

			_CswNbtSchemaModTrnsctn.UpdateS4( "getRelationsForObjectClassId", @"select distinct 'NodeTypePropId' proptype,
       t.firstversionid typeid,
       p.firstpropversionid propid,
       p.fktype,
       p.fkvalue
  from nodetype_props p
  join nodetypes t on p.nodetypeid = t.nodetypeid
 where fieldtypeid in (select fieldtypeid from field_types where fieldtype in ('Relationship', 'Location'))
   and (fktype = 'ObjectClassId' and fkvalue = :getobjectclassid)
   and ((exists (select j.jctmoduleobjectclassid
                  from jct_modules_objectclass j
                  join modules m on j.moduleid = m.moduleid
                 where j.objectclassid = t.objectclassid
                   and m.enabled = '1')
    or not exists (select j.jctmoduleobjectclassid
                     from jct_modules_objectclass j
                     join modules m on j.moduleid = m.moduleid
                    where j.objectclassid = t.objectclassid) )
   and (exists (select j.jctmodulenodetypeid
                  from jct_modules_nodetypes j
                  join modules m on j.moduleid = m.moduleid
                 where j.nodetypeid = t.firstversionid
                   and m.enabled = '1')
    or not exists (select j.jctmodulenodetypeid
                     from jct_modules_nodetypes j
                     join modules m on j.moduleid = m.moduleid
                    where j.nodetypeid = t.firstversionid) )) 
 union
 select 'ObjectClassPropId' proptype,
       op.objectclassid typeid,
       op.objectclasspropid propid,
       op.fktype,
       op.fkvalue
  from object_class_props op
 where fieldtypeid in (select fieldtypeid from field_types where fieldtype in ('Relationship', 'Location'))
   and ((objectclassid = :getobjectclassid) or
	   (fkvalue = :getobjectclassid and fktype = 'ObjectClassId'))
   and (exists (select j.jctmoduleobjectclassid
                  from jct_modules_objectclass j
                  join modules m on j.moduleid = m.moduleid
                 where j.objectclassid = op.objectclassid
                   and m.enabled = '1')
        or not exists (select j.jctmoduleobjectclassid
                         from jct_modules_objectclass j
                         join modules m on j.moduleid = m.moduleid
                        where j.objectclassid = op.objectclassid))" );

		}//Update()

	}//class CswUpdateSchemaTo01K08

}//namespace ChemSW.Nbt.Schema


