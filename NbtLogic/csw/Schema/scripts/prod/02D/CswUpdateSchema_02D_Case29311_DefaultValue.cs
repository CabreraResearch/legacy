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
    public class CswUpdateSchema_02D_Case29311_DefaultValue : CswUpdateSchemaTo
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
            // In order to handle default values in this bold new world

            // 1. override attributes with current property's attributes (done as code change)

            // Map:
            string MapSql = @"select p.nodetypepropid, p.defaultvalueid sourcejctid, dn.nodeid, dp.nodetypepropid designntpid, dj.jctnodepropid destjctid
                             from nodetype_props p 
                             join jct_nodes_props j on p.defaultvalueid = j.jctnodepropid
                             join nodes dn on p.nodetypepropid = dn.relationalid and dn.relationaltable = 'nodetype_props'
                             join nodetypes dt on dn.nodetypeid = dt.nodetypeid
                             join nodetype_props dp on dp.nodetypeid = dn.nodetypeid and dp.propname = 'Default Value'
                             left outer join jct_nodes_props dj on dp.nodetypepropid = dj.nodetypepropid and dj.nodeid = dn.nodeid";
            CswArbitrarySelect mapSelect = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "29311_defval_select", MapSql );
            DataTable mapTable = mapSelect.getTable();


            // 2. copy over null-nodeid rows to default values and remove the old rows
            {
                // Source:
                CswTableUpdate sourceUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "29311_jct_src_update", "jct_nodes_props" );
                DataTable sourceTable = sourceUpdate.getTable( @"where jctnodepropid in (select defaultvalueid from nodetype_props) and nodeid is null" );

                // Dest:
                CswTableUpdate destUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "29311_jct_dst_update", "jct_nodes_props" );
                DataTable destTable = destUpdate.getTable( @"where jctnodepropid in (select dj.jctnodepropid
                             from nodetype_props p 
                             join jct_nodes_props j on p.defaultvalueid = j.jctnodepropid
                             join nodes dn on p.nodetypepropid = dn.relationalid and dn.relationaltable = 'nodetype_props'
                             join nodetypes dt on dn.nodetypeid = dt.nodetypeid
                             join nodetype_props dp on dp.nodetypeid = dn.nodetypeid and dp.propname = 'Default Value'
                             left outer join jct_nodes_props dj on dp.nodetypepropid = dj.nodetypepropid and dj.nodeid = dn.nodeid)" );

                foreach( DataRow mapRow in mapTable.Rows )
                {
                    DataRow sourceRow = sourceTable.Rows.Cast<DataRow>().FirstOrDefault( r => CswConvert.ToInt32( r["jctnodepropid"] ) == CswConvert.ToInt32( mapRow["sourcejctid"] ) );
                    DataRow destRow = destTable.Rows.Cast<DataRow>().FirstOrDefault( r => CswConvert.ToInt32( r["jctnodepropid"] ) == CswConvert.ToInt32( mapRow["destjctid"] ) );
                    if( null != sourceRow && null != destRow )
                    {
                        destRow["field1"] = sourceRow["field1"];
                        destRow["field2"] = sourceRow["field2"];
                        destRow["field3"] = sourceRow["field3"];
                        destRow["field1_fk"] = sourceRow["field1_fk"];
                        destRow["gestalt"] = sourceRow["gestalt"];
                        destRow["field4"] = sourceRow["field4"];
                        destRow["field5"] = sourceRow["field5"];
                        destRow["clobdata"] = sourceRow["clobdata"];
                        destRow["field1_date"] = sourceRow["field1_date"];
                        destRow["field1_numeric"] = sourceRow["field1_numeric"];
                        destRow["field2_date"] = sourceRow["field2_date"];
                        destRow["isdemo"] = sourceRow["isdemo"];
                        destRow["field2_numeric"] = sourceRow["field2_numeric"];
                        destRow["gestaltsearch"] = sourceRow["gestaltsearch"];
                    }
                    
                } // foreach( DataRow mapRow in mapTable.Rows )

                // remove the old rows
                foreach( DataRow sourceRow in sourceTable.Rows )
                {
                    sourceRow.Delete();
                }

                sourceUpdate.update( sourceTable );
                destUpdate.update( destTable );
            } // end step 2


            // 3. set defaultvalueid to point to design's jctnodepropid
            {               
                CswTableUpdate ntpUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "29311_ntp_update", "nodetype_props" );
                DataTable ntpTable = ntpUpdate.getTable( @"where defaultvalueid is not null" );
                foreach( DataRow mapRow in mapTable.Rows )
                {
                    DataRow ntpRow = ntpTable.Rows.Cast<DataRow>().FirstOrDefault( r => CswConvert.ToInt32( r["nodetypepropid"] ) == CswConvert.ToInt32( mapRow["nodetypepropid"] ) );
                    if( null != ntpRow )
                    {
                        ntpRow["defaultvalueid"] = mapRow["destjctid"];
                    }
                } // foreach( DataRow mapRow in mapTable.Rows )
                ntpUpdate.update( ntpTable );
            } // end step 3
            
        } // update()

    }//class CswUpdateSchema_02D_Case29311_DefaultValue

}//namespace ChemSW.Nbt.Schema