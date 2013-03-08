using ChemSW.Nbt.csw.Dev;
using ChemSW.DB;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using System;
using System.Data;
using System.Collections.Generic;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28355
    /// </summary>
    public class CswUpdateSchema_02A_Case28355 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28355; }
        }

        public override void update()
        {

            //Get the Node count for each OC/NT
            Dictionary<Int32, Int32> NodeCountsForNodeType;
            Dictionary<Int32, Int32> NodeCountsForObjectClass;
            _getCounts( out NodeCountsForNodeType, out NodeCountsForObjectClass );

            CswTableUpdate nodetypes_TU = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "initNodeCountNT28355", "nodetypes" );
            DataTable nodetypesTbl = nodetypes_TU.getTable();
            foreach( DataRow Row in nodetypesTbl.Rows )
            {
                int NodeTypeId = CswConvert.ToInt32( Row["nodetypeid"] );
                if( NodeCountsForNodeType.ContainsKey( NodeTypeId ) )
                {
                    Row["nodecount"] = NodeCountsForNodeType[NodeTypeId];
                }
                else
                {
                    Row["nodecount"] = 0;
                }
            }
            nodetypes_TU.update( nodetypesTbl );

            CswTableUpdate objclass_TU = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "initNodeCountOC28355", "object_class" );
            DataTable objClassTbl = objclass_TU.getTable();
            foreach( DataRow Row in objClassTbl.Rows )
            {
                int objclassid = CswConvert.ToInt32( Row["objectclassid"] );
                if( NodeCountsForObjectClass.ContainsKey( objclassid ) )
                {
                    Row["nodecount"] = NodeCountsForObjectClass[objclassid];
                }
                else
                {
                    Row["nodecount"] = 0;
                }
            }
            objclass_TU.update( objClassTbl );

        } // update()

        private void _getCounts( out Dictionary<Int32, Int32> NodeCountsForNodeType, out Dictionary<Int32, Int32> NodeCountsForObjectClass )
        {
            NodeCountsForNodeType = new Dictionary<Int32, Int32>();
            NodeCountsForObjectClass = new Dictionary<Int32, Int32>();

            // Look up the object class of all nodes (deleted or no)
            string SqlSelect = @"select count(distinct nodeid) cnt, firstversionid, objectclassid 
                                   from (select n.nodeid, t.firstversionid, o.objectclassid, n.istemp
                                           from nodes n
                                           left outer join nodetypes t on n.nodetypeid = t.nodetypeid
                                           left outer join object_class o on t.objectclassid = o.objectclassid
                                        UNION
                                         select n.nodeid, ta.firstversionid, o.objectclassid, n.istemp
                                           from nodes_audit n
                                           left outer join nodetypes_audit ta on n.nodetypeid = ta.nodetypeid
                                           left outer join object_class o on ta.objectclassid = o.objectclassid)";
            SqlSelect += "where istemp = 0";
            SqlSelect += "group by objectclassid, firstversionid";

            CswArbitrarySelect NodeCountSelect = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "CswNbtActQuotas_historicalNodeCount", SqlSelect );
            DataTable NodeCountTable = NodeCountSelect.getTable();
            foreach( DataRow NodeCountRow in NodeCountTable.Rows )
            {
                Int32 ThisObjectClassId = CswConvert.ToInt32( NodeCountRow["objectclassid"] );
                Int32 ThisNodeTypeId = CswConvert.ToInt32( NodeCountRow["firstversionid"] );

                Int32 NodeCount = CswConvert.ToInt32( NodeCountRow["cnt"] );
                if( NodeCountsForNodeType.ContainsKey( ThisNodeTypeId ) )
                {
                    NodeCountsForNodeType[ThisNodeTypeId] += NodeCount;
                }
                else
                {
                    NodeCountsForNodeType.Add( ThisNodeTypeId, NodeCount );
                }

                if( NodeCountsForObjectClass.ContainsKey( ThisObjectClassId ) )
                {
                    NodeCountsForObjectClass[ThisObjectClassId] += NodeCount;
                }
                else
                {
                    NodeCountsForObjectClass.Add( ThisObjectClassId, NodeCount );
                }
            }
        }

    }//class CswUpdateSchema_02A_Case28355

}//namespace ChemSW.Nbt.Schema