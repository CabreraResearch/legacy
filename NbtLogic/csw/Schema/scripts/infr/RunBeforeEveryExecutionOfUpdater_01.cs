using System;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_01 : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: DDL";

        #region Blame Logic

        private CswDeveloper _Author = CswDeveloper.NBT;

        public override CswDeveloper Author
        {
            get { return _Author; }
        }

        private Int32 _CaseNo;

        public override int CaseNo
        {
            get { return _CaseNo; }
        }

        private void _acceptBlame( CswDeveloper BlameMe, Int32 BlameCaseNo )
        {
            _Author = BlameMe;
            _CaseNo = BlameCaseNo;
        }

        private void _resetBlame()
        {
            _Author = CswDeveloper.NBT;
            _CaseNo = 0;
        }

        #endregion Blame Logic

        public override void update()
        {
            // This script is for changes to schema structure,
            // or other changes that must take place before any other schema script.

            // NOTE: This script will be run many times, so make sure your changes are safe!

            #region ASPEN

            _createNodeCountColumns( CswDeveloper.MB, 28355 );
            _createLoginDataTable( CswDeveloper.BV, 27906 );
            _addViewIsSystemColumn( CswDeveloper.BV, 28890 );
            _fixKioskModeName( CswDeveloper.MB, 29274 );

            #endregion ASPEN

            #region BUCKEYE

            _propSetTable(CswDeveloper.SS, 28160 );
            _addIsSearchableColumn( CswDeveloper.PG, 28753 );

            #endregion BUCKEYE

        }//Update()

        #region ASPEN

        private void _createNodeCountColumns( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );

            string NodeCountColName = "nodecount";
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetypes", NodeCountColName ) )
            {
                _CswNbtSchemaModTrnsctn.addLongColumn( "nodetypes", NodeCountColName, "The number of nodes", false, false );
            }

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "object_class", NodeCountColName ) )
            {
                _CswNbtSchemaModTrnsctn.addLongColumn( "object_class", NodeCountColName, "The number of nodes", false, false );
            }

            _resetBlame();
        }

        private void _createLoginDataTable( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );

            const string LoginDataTableName = "login_data";

            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( LoginDataTableName ) )
            {
                _CswNbtSchemaModTrnsctn.addTable( LoginDataTableName, "loginid" );
                _CswNbtSchemaModTrnsctn.getNewPrimeKey( LoginDataTableName );
            }
            if( _CswNbtSchemaModTrnsctn.isTableDefined( LoginDataTableName ) )
            {
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( LoginDataTableName, "username" ) )
                {
                    _CswNbtSchemaModTrnsctn.addStringColumn( LoginDataTableName, "username", "User's Username", false, false, 50 );
                }
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( LoginDataTableName, "ipaddress" ) )
                {
                    _CswNbtSchemaModTrnsctn.addStringColumn( LoginDataTableName, "ipaddress", "User's IP Address", false, false, 30 );
                }
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( LoginDataTableName, "logindate" ) )
                {
                    _CswNbtSchemaModTrnsctn.addDateColumn( LoginDataTableName, "logindate", "Date of Login Attempt", false, false );
                }
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( LoginDataTableName, "loginstatus" ) )
                {
                    _CswNbtSchemaModTrnsctn.addStringColumn( LoginDataTableName, "loginstatus", "Status of Login Attempt", false, false, 50 );
                }
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( LoginDataTableName, "failurereason" ) )
                {
                    _CswNbtSchemaModTrnsctn.addStringColumn( LoginDataTableName, "failurereason", "Reason for Login Failure", false, false, 100 );
                }
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( LoginDataTableName, "failedlogincount" ) )
                {
                    _CswNbtSchemaModTrnsctn.addLongColumn( LoginDataTableName, "failedlogincount", "Number of times user login has failed this session", false, false );
                }
            }

            _resetBlame();
        }

        private void _addViewIsSystemColumn( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );

            string IsSystemColumnName = "issystem";
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "node_views", IsSystemColumnName ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "node_views", IsSystemColumnName, "When set to true, only ChemSWAdmin can edit this view", false, false );
            }

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "node_views_audit", IsSystemColumnName ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "node_views_audit", IsSystemColumnName, "When set to true, only ChemSWAdmin can edit this view", false, false );
            }

            _resetBlame();
        }

        private void _fixKioskModeName( CswDeveloper Dev, Int32 CaseNo )
        {
            if( null == _CswNbtSchemaModTrnsctn.Actions[CswNbtActionName.Kiosk_Mode] )
            {
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update actions set actionname = 'Kiosk Mode' where actionname = 'KioskMode'" );
            }
        }

        #endregion ASPEN

        #region BUCKEYE Methods
        private void _addIsSearchableColumn( CswDeveloper Dev, Int32 CaseNo )
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

        private void _propSetTable( CswDeveloper Dev, Int32 CaseNo )
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

            _resetBlame();
        }

        #endregion BUCKEYE Methods

    }//class RunBeforeEveryExecutionOfUpdater_01
}//namespace ChemSW.Nbt.Schema


