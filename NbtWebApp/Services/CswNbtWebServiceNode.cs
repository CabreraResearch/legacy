using System;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.Statistics;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceNode
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly CswNbtStatisticsEvents _CswNbtStatisticsEvents;
        private readonly CswNbtSdNode _NodeSd;
        public CswNbtWebServiceNode( CswNbtResources CswNbtResources, CswNbtStatisticsEvents CswNbtStatisticsEvents )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtStatisticsEvents = CswNbtStatisticsEvents;
            _NodeSd = new CswNbtSdNode( _CswNbtResources, _CswNbtStatisticsEvents );
        }

        public CswPrimaryKey CopyNode( CswPrimaryKey NodePk )
        {
            return _NodeSd.CopyNode( NodePk );
        }

        public bool DeleteNode( CswPrimaryKey NodePk, bool DeleteAllRequiredRelatedNodes = false )
        {
            return _NodeSd.DeleteNode( NodePk, DeleteAllRequiredRelatedNodes );
        }

        public JObject doObjectClassButtonClick( CswPropIdAttr PropId )
        {
            return _NodeSd.doObjectClassButtonClick( PropId );
        }

        public JObject deleteDemoDataNodes()
        {
            JObject Ret = new JObject();
            Int32 Succeeded = 0;
            Int32 Total = 0;
            Int32 Failed = 0;
            if( _CswNbtResources.CurrentNbtUser.IsAdministrator() )
            {
                /* Get a new CswNbtResources as the System User */
                CswNbtWebServiceMetaData wsMd = new CswNbtWebServiceMetaData( _CswNbtResources );
                CswNbtResources NbtSystemResources = wsMd.makeSystemUserResources( _CswNbtResources.AccessId, false, false );

                //CswTableSelect NodesSelect = new CswTableSelect( NbtSystemResources.CswResources, "delete_demodata_nodes", "nodes" );
                CswTableSelect NodesSelect = _CswNbtResources.makeCswTableSelect( "delete_demodata_nodes", "nodes" );

                DataTable NodesTable = NodesSelect.getTable( new CswCommaDelimitedString { "nodeid" },
                                                            " where isdemo='" + CswConvert.ToDbVal( true ) + "' " );
                Total = NodesTable.Rows.Count;
                Collection<Exception> Exceptions = new Collection<Exception>();
                foreach( DataRow NodeRow in NodesTable.Rows )
                {
                    try
                    {
                        CswPrimaryKey NodePk = new CswPrimaryKey( "nodes", CswConvert.ToInt32( NodeRow["nodeid"] ) );
                        if( _NodeSd.DeleteNode( NodePk, DeleteAllRequiredRelatedNodes: true ) )
                        {
                            Succeeded += 1;
                        }
                    }
                    catch( Exception Exception )
                    {
                        Failed += 1;
                        Exceptions.Add( Exception );
                    }
                }
                wsMd.finalizeOtherResources( NbtSystemResources );
                if( Exceptions.Count > 0 )
                {
                    string ExceptionText = "";
                    foreach( Exception ex in Exceptions )
                    {
                        ExceptionText += ex.Message + " " + ex.InnerException + " /n";
                    }
                    throw new CswDniException( ErrorType.Warning, "Not all demo data nodes were deleted. " + Failed + " failed out of " + Total + " total.", "The following exception(s) occurred: " + ExceptionText );
                }
            }
            Ret["succeeded"] = Succeeded;
            Ret["total"] = Total;
            Ret["failed"] = Failed;

            return Ret;
        }

        /// <summary>
        /// Create a new node
        /// </summary>
        public void addNodeProps( CswNbtNode Node, JObject PropsObj, CswNbtMetaDataNodeTypeTab Tab )
        {
            _NodeSd.addNodeProps( Node, PropsObj, Tab );
        }

        public void addSingleNodeProp( CswNbtNode Node, JObject PropObj, CswNbtMetaDataNodeTypeTab Tab )
        {
            _NodeSd.addSingleNodeProp( Node, PropObj, Tab );

        } // _applyPropJson

    } // class CswNbtWebServiceNode

} // namespace ChemSW.Nbt.WebServices
