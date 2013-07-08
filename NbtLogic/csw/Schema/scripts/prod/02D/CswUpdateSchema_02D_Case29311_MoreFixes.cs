using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29311
    /// </summary>
    public class CswUpdateSchema_02D_Case29311_MoreFixes : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 29311; }
        }

        public override void update()
        {
            // fix some inconsistent viewids
            string Sql = @"update nodetype_props set nodeviewid = (select rj.field1_fk
                                                                     from nodetype_props p
                                                                     join nodetypes t on p.nodetypeid = t.nodetypeid
                                                                     join field_types f on p.fieldtypeid = f.fieldtypeid
                                                                     join nodes r on p.nodetypepropid = r.relationalid
                                                                                 and r.relationaltable = 'nodetype_props'
                                                                     join nodetypes rt on r.nodetypeid = rt.nodetypeid
                                                                     join nodetype_props rp on rt.nodetypeid = rp.nodetypeid
                                                                     join jct_nodes_props rj on r.nodeid = rj.nodeid
                                                                                            and rp.nodetypepropid = rj.nodetypepropid
                                                                    where rp.propname = 'View'
                                                                      and p.nodeviewid is null
                                                                      and p.nodetypepropid = nodetype_props.nodetypepropid)
                            where nodetypepropid in (select p.nodetypepropid
                                                       from nodetype_props p
                                                       join nodetypes t on p.nodetypeid = t.nodetypeid
                                                       join field_types f on p.fieldtypeid = f.fieldtypeid
                                                       join nodes r on p.nodetypepropid = r.relationalid
                                                                   and r.relationaltable = 'nodetype_props'
                                                       join nodetypes rt on r.nodetypeid = rt.nodetypeid
                                                       join nodetype_props rp on rt.nodetypeid = rp.nodetypeid
                                                       join jct_nodes_props rj on r.nodeid = rj.nodeid
                                                                           and rp.nodetypepropid = rj.nodetypepropid
                                                      where rp.propname = 'View'
                                                        and p.nodeviewid is null)";
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( Sql );

        } // update()

    }//class CswUpdateSchema_02D_Case29311_MoreFixes

}//namespace ChemSW.Nbt.Schema
