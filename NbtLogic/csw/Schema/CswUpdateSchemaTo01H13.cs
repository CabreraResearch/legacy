using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Actions;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-13
    /// </summary>
    public class CswUpdateSchemaTo01H13 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 13 ); } }
        public CswUpdateSchemaTo01H13( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
        }

        public void update()
        {
            // case 20509 - add #addclause to S4
            _CswNbtSchemaModTrnsctn.UpdateS4( "getVisibleViewInfo",
@"select v.nodeviewid, v.viewname, v.visibility, v.roleid, v.userid,
v.category, r.nodename rolename, u.nodename username, v.viewxml
,lower(NVL(v.category, v.viewname)) mssqlorder
from node_views v
left outer join nodes r on v.roleid = r.nodeid
left outer join nodes u on v.userid = u.nodeid
where ((visibility = 'Global') or
       (visibility = 'Role' and roleid = :getroleid) or
       (visibility = 'User' and userid = :getuserid))
       #addclause
order by #orderbyclause" );

        } // update()

    }//class CswUpdateSchemaTo01H13

}//namespace ChemSW.Nbt.Schema


