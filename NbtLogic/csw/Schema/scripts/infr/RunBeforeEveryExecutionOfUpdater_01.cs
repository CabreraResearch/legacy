using System;
using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_01: CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: DDL";

        #region Blame Logic

        private CswEnumDeveloper _Author = CswEnumDeveloper.NBT;

        public override CswEnumDeveloper Author
        {
            get { return _Author; }
        }

        private Int32 _CaseNo;

        public override int CaseNo
        {
            get { return _CaseNo; }
        }

        private void _acceptBlame( CswEnumDeveloper BlameMe, Int32 BlameCaseNo )
        {
            _Author = BlameMe;
            _CaseNo = BlameCaseNo;
        }

        private void _resetBlame()
        {
            _Author = CswEnumDeveloper.NBT;
            _CaseNo = 0;
        }

        #endregion Blame Logic

        public override void update()
        {
            // This script is for changes to schema structure,
            // or other changes that must take place before any other schema script.

            // NOTE: This script will be run many times, so make sure your changes are safe!
            
            #region BUCKEYE

            _propSetTable( CswEnumDeveloper.SS, 28160 );
            _addIsSearchableColumn( CswEnumDeveloper.PG, 28753 );
            _createBlobDataTable( CswEnumDeveloper.MB, 26531 );
            _addNewScheduledRulesColumns( CswEnumDeveloper.BV, 29287 );
            _addColumnsToSessionListTable( CswEnumDeveloper.CM, 29127 );
            
            #endregion BUCKEYE

            //This BUCKEYE method goes last - it's not a DDL change, 
            //but it has to occur before anything in the BeforeOC script, 
            //and it has to run in a separate transaction from the BeforeOC script
            _renameMaterialObjClassToChemical( CswEnumDeveloper.BV, 28690 );

        }//Update()
        
        #region BUCKEYE Methods
        
        private void _addIsSearchableColumn( CswEnumDeveloper Dev, Int32 CaseNo )
        {

            _acceptBlame( Dev, CaseNo );

            string table_nodes = "nodes";
            string column_searchable = "searchable";

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( table_nodes, column_searchable ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( table_nodes, column_searchable, "when set to '0' will not be included in searches", false, true );
            }

            _resetBlame();
        }

        private void _propSetTable( CswEnumDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );

            string PropSetTableName = "property_set";
            string PropSetPkName = "propertysetid";
            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( PropSetTableName ) )
            {
                _CswNbtSchemaModTrnsctn.addTable( PropSetTableName, PropSetPkName );
                _CswNbtSchemaModTrnsctn.addStringColumn( PropSetTableName, "name", "Name of property set", false, false, 50 );
                _CswNbtSchemaModTrnsctn.addStringColumn( PropSetTableName, "iconfilename", "Icon for property set", false, false, 50 );
            }

            string JctPsOcTableName = "jct_propertyset_objectclass";
            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( JctPsOcTableName ) )
            {
                _CswNbtSchemaModTrnsctn.addTable( JctPsOcTableName, "jctpropsetobjclassid" );
                _CswNbtSchemaModTrnsctn.addForeignKeyColumn( JctPsOcTableName, "objectclassid", "Object class foreign key", false, true, "object_class", "objectclassid" );
                _CswNbtSchemaModTrnsctn.addForeignKeyColumn( JctPsOcTableName, PropSetPkName, "Property Set foreign key", false, true, PropSetTableName, PropSetPkName );
            }

            string JctPsOcpTableName = "jct_propertyset_ocprop";
            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( JctPsOcpTableName ) )
            {
                _CswNbtSchemaModTrnsctn.addTable( JctPsOcpTableName, "jctpropsetocpropid" );
                _CswNbtSchemaModTrnsctn.addForeignKeyColumn( JctPsOcpTableName, "objectclasspropid", "Object class prop foreign key", false, true, "object_class_prop", "objectclasspropid" );
                _CswNbtSchemaModTrnsctn.addForeignKeyColumn( JctPsOcpTableName, PropSetPkName, "Property Set foreign key", false, true, PropSetTableName, PropSetPkName );
            }

            _CswNbtSchemaModTrnsctn.UpdateS4( "getRelationsForNodeTypeId",
                @"select distinct 'NodeTypePropId' proptype, t.firstversionid typeid, p.firstpropversionid propid, p.fktype, p.fkvalue
                  from nodetype_props p
                  join nodetypes t on p.nodetypeid = t.nodetypeid
                  left outer join nodetypes f on p.fkvalue = f.nodetypeid
                 where fieldtypeid in (select fieldtypeid from field_types 
                                        where fieldtype in ('Relationship', 'Location'))
                   and ((t.firstversionid = :getnodetypeid) or 
                        (fktype = 'PropertySetId' and fkvalue = (select propertysetid from jct_propertyset_objectclass 
                                                                  where objectclassid = (select objectclassid from nodetypes 
                                                                                          where nodetypeid = :getnodetypeid))) or
                        (fktype = 'ObjectClassId' and fkvalue = (select objectclassid from nodetypes 
                                                                  where nodetypeid = :getnodetypeid)) or
                        (fktype = 'NodeTypeId' and f.firstversionid = :getnodetypeid))
                   and t.enabled = 1" );


            _CswNbtSchemaModTrnsctn.UpdateS4( "getRelationsForObjectClassId",
                @"select distinct 'NodeTypePropId' proptype,
                       t.firstversionid typeid,
                       p.firstpropversionid propid,
                       p.fktype,
                       p.fkvalue
                  from nodetype_props p
                  join nodetypes t on p.nodetypeid = t.nodetypeid
                 where fieldtypeid in (select fieldtypeid from field_types where fieldtype in ('Relationship', 'Location'))
                   and ( (fktype = 'ObjectClassId' and fkvalue = :getobjectclassid) or
                         (fktype = 'PropertySetId' and fkvalue = (select propertysetid from jct_propertyset_objectclass 
                                                                   where objectclassid = :getobjectclassid) ) )
                   and t.enabled = 1 
                 union
                 select 'ObjectClassPropId' proptype,
                       op.objectclassid typeid,
                       op.objectclasspropid propid,
                       op.fktype,
                       op.fkvalue
                  from object_class_props op
                 where fieldtypeid in (select fieldtypeid from field_types where fieldtype in ('Relationship', 'Location'))
                   and ( (objectclassid = :getobjectclassid) or
                         ( (fktype = 'ObjectClassId' and fkvalue = :getobjectclassid) or
                           (fktype = 'PropertySetId' and fkvalue = (select propertysetid from jct_propertyset_objectclass 
                                                                     where objectclassid = :getobjectclassid) ) ) )           
                   and (exists (select j.jctmoduleobjectclassid
                                  from jct_modules_objectclass j
                                  join modules m on j.moduleid = m.moduleid
                                 where j.objectclassid = op.objectclassid
                                   and m.enabled = '1')
                        or not exists (select j.jctmoduleobjectclassid
                                         from jct_modules_objectclass j
                                         join modules m on j.moduleid = m.moduleid
                                        where j.objectclassid = op.objectclassid))" );
            
            if( false == _CswNbtSchemaModTrnsctn.doesS4Exist( "getRelationsForPropertySetId" ) )
            {
                _CswNbtSchemaModTrnsctn.InsertS4( "getRelationsForPropertySetId",
                                                  @"select distinct 'NodeTypePropId' proptype,
                       t.firstversionid typeid,
                       p.firstpropversionid propid,
                       p.fktype,
                       p.fkvalue, 
                       jpo.propertysetid
                  from nodetype_props p
                  join nodetypes t on p.nodetypeid = t.nodetypeid
                  left outer join object_class_props ocp on p.objectclasspropid = ocp.objectclasspropid
                  left outer join jct_propertyset_ocprop jpo on ocp.objectclasspropid = jpo.objectclasspropid
                 where p.fieldtypeid in (select fieldtypeid from field_types where fieldtype in ('Relationship', 'Location'))
                   and ( ( jpo.propertysetid = :getpropertysetid ) or
                         ( p.fktype = 'PropertySetId' and p.fkvalue = :getpropertysetid ) )
                   and t.enabled = 1 
                 union
                 select 'ObjectClassPropId' proptype,
                       op.objectclassid typeid,
                       op.objectclasspropid propid,
                       op.fktype,
                       op.fkvalue, 
                       jpo.propertysetid
                  from object_class_props op
                  left outer join jct_propertyset_ocprop jpo on op.objectclasspropid = jpo.objectclasspropid
                 where fieldtypeid in (select fieldtypeid from field_types where fieldtype in ('Relationship', 'Location'))
                   and ( ( jpo.propertysetid = :getpropertysetid ) or
                         ( op.fktype = 'PropertySetId' and op.fkvalue = :getpropertysetid ) )
                   and (exists (select j.jctmoduleobjectclassid
                                  from jct_modules_objectclass j
                                  join modules m on j.moduleid = m.moduleid
                                 where j.objectclassid = op.objectclassid
                                   and m.enabled = '1')
                        or not exists (select j.jctmoduleobjectclassid
                                         from jct_modules_objectclass j
                                         join modules m on j.moduleid = m.moduleid
                                        where j.objectclassid = op.objectclassid))",
                                                  "nodetype_props" );
            }
            _resetBlame();
        }

        private void _addColumnsToSessionListTable( CswEnumDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );

                // Add LastAccessId column
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "sessionlist", "nbtmgraccessid" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "sessionlist", "nbtmgraccessid", "Last AccessId that the Session was associated with. Used when switching schemata on NBTManager.", false, false, 50 );
            }
                // Add NbtMgrUserName
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "sessionlist", "nbtmgrusername" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "sessionlist", "nbtmgrusername", "Username of user logged into schema with NBTManager enabled. Used when switching schemata on NBTManager.", false, false, 50 );
            }
                // Add NbtMgrUserId
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "sessionlist", "nbtmgruserid" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "sessionlist", "nbtmgruserid", "UserId of user logged into schema with NBTManager enabled. Used when switching schemata on NBTManager.", false, false, 50 );
            }
            _resetBlame();

        }

        private void _createBlobDataTable( CswEnumDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );

            const string blobdatatblname = "blob_data";

            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( blobdatatblname ) )
            {
                _CswNbtSchemaModTrnsctn.addTable( blobdatatblname, "blobdataid" );
            }
            if( _CswNbtSchemaModTrnsctn.isTableDefined( blobdatatblname ) )
            {
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( blobdatatblname, "blobdata" ) )
                {
                    _CswNbtSchemaModTrnsctn.addBlobColumn( blobdatatblname, "blobdata", "The blob data", false, false );
                }

                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( blobdatatblname, "jctnodepropid" ) )
                {
                    _CswNbtSchemaModTrnsctn.addForeignKeyColumn( blobdatatblname, "jctnodepropid", "The property row this blob data belongs to", false, true, "jct_nodes_props", "jctnodepropid" );
                }

                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( blobdatatblname, "contenttype" ) )
                {
                    _CswNbtSchemaModTrnsctn.addStringColumn( blobdatatblname, "contenttype", "The content type of this blob", false, false, 120 );
                }

                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( blobdatatblname, "filename" ) )
                {
                    _CswNbtSchemaModTrnsctn.addStringColumn( blobdatatblname, "filename", "The name of this blob", false, false, 120 );
                }

                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( blobdatatblname, "caption" ) )
                {
                    _CswNbtSchemaModTrnsctn.addClobColumn( blobdatatblname, "caption", "A caption for this blob", false, false );
                }
            }

            _resetBlame();
        }

        private void _addNewScheduledRulesColumns( CswEnumDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );

            string ScheduledRulesTableName = "scheduledrules";
            string NextRunColumnName = "nextrun";
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( ScheduledRulesTableName, NextRunColumnName ) )
            {
                _CswNbtSchemaModTrnsctn.addDateColumn( ScheduledRulesTableName, NextRunColumnName, "The next time the rule is scheduled to run", false, false );
            }

            string PriorityColumnName = "priority";
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( ScheduledRulesTableName, PriorityColumnName ) )
            {
                _CswNbtSchemaModTrnsctn.addLongColumn( ScheduledRulesTableName, PriorityColumnName, "Priority of the rule", false, false );
            }

            string LoadCountColumnName = "loadcount";
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( ScheduledRulesTableName, LoadCountColumnName ) )
            {
                _CswNbtSchemaModTrnsctn.addLongColumn( ScheduledRulesTableName, LoadCountColumnName, "The amount of work the rule currently has to do", false, false );
            }

            string LastLoadCheckColumnName = "lastloadcheck";
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( ScheduledRulesTableName, LastLoadCheckColumnName ) )
            {
                _CswNbtSchemaModTrnsctn.addDateColumn( ScheduledRulesTableName, LastLoadCheckColumnName, "The last time the rule's load count was recalculated", false, false );
            }

            _resetBlame();
        }


        private void _renameMaterialObjClassToChemical( CswEnumDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );
             
            //Change ObjClassMaterial to ObjClassChemical
            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( "MaterialClass" );
            if( null != MaterialOC )
            {
                CswTableUpdate OCUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "28690_oc_update", "object_class" );
                DataTable OCTable = OCUpdate.getTable( "where objectclassid = " + MaterialOC.ObjectClassId );
                if( OCTable.Rows.Count > 0 )
                {
                    OCTable.Rows[0]["objectclass"] = CswEnumNbtObjectClass.ChemicalClass;
                }
                OCUpdate.update( OCTable );
            }
            _resetBlame();
        }

        #endregion BUCKEYE Methods

    }//class RunBeforeEveryExecutionOfUpdater_01
}//namespace ChemSW.Nbt.Schema


