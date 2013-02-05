using System;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// CswUpdateSchemaPLSQLViews
    /// </summary>    
    public class CswUpdateSchemaPLSQLViews
    {
        public sealed class Views : CswEnum<Views>
        {
            #region Properties and ctor

            private Views( string Dev, Int32 CaseNo, string Name ) : base( Name )
            {
                _Dev = Dev;
                _CaseNo = CaseNo;
            }
            static Views() { }
            public string _Dev { get; private set; }
            public Int32 _CaseNo { get; private set; }
            public static IEnumerable<Views> _All { get { return All; } }
            public static implicit operator Views( string str )
            {
                Views ret = Parse( str );
                return ret;
            }

            #endregion Properties and ctor

            #region NBTDATA

            public static readonly Views NBTDATA = new Views( CswDeveloper.NBT, 0,
            @"CREATE OR REPLACE NOFORCE VIEW NBTDATA
  AS select n.nodeid, n.nodename,  t.nodetypeid, t.nodetypename, o.objectclassid, o.objectclass,
p.nodetypepropid, p.propname, op.objectclasspropid, op.propname objectclasspropname, f.fieldtype,
j.jctnodepropid, j.field1, j.field2, j.field3, j.field4, j.field5, j.field1_fk, j.gestalt, j.clobdata
from nodes n
join nodetypes t on n.nodetypeid = t.nodetypeid
join object_class o on t.objectclassid = o.objectclassid
join nodetype_props p on p.nodetypeid = t.nodetypeid
join field_types f on p.fieldtypeid = f.fieldtypeid
left outer join object_class_props op on p.objectclasspropid = op.objectclasspropid
left outer join jct_nodes_props j on (j.nodeid = n.nodeid and j.nodetypepropid = p.nodetypepropid)
order by lower(n.nodename), lower(p.propname)" );

            #endregion NBTDATA
        }

    }//class CswUpdateSchemaPLSQLViews

}//namespace ChemSW.Nbt.Schema