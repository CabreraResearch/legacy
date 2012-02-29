using System;
using System.Data;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Actions;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceGenerators
    {
        private readonly CswNbtResources _CswNbtResources;
        public CswNbtWebServiceGenerators( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public JObject getGeneratorsTree()
        {
            CswNbtMetaDataObjectClass GeneratorOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
            CswNbtView GeneratorView = GeneratorOC.CreateDefaultView();
            GeneratorView.ViewName = "Generators";

            CswNbtWebServiceTree ws = new CswNbtWebServiceTree( _CswNbtResources, GeneratorView );
            return ws.runTree( null, null, false, false, string.Empty );
        }

        public JObject futureScheduling( CswCommaDelimitedString SelectedGeneratorNodeKeys, DateTime EndDate )
        {
            JObject ret = new JObject();

            CswNbtActGenerateFutureNodes CswNbtActGenerateFutureNodes = new CswNbtActGenerateFutureNodes( _CswNbtResources );

            Int32 TotalNodes = 0;
            Collection<CswNbtNode> SelectedGeneratorNodes = new Collection<CswNbtNode>();
            //foreach( CswNbtNodeKey CurrentNodeKey in ( from NodeIdStr in SelectedGeneratorNodeKeys
            //                                           select new CswNbtNodeKey( _CswNbtResources, NodeIdStr ) ) )
            foreach( string NodeKeyStr in SelectedGeneratorNodeKeys )
            {
                CswNbtNodeKey CurrentNodeKey = new CswNbtNodeKey( _CswNbtResources, NodeKeyStr );

                CswNbtNode CurrentGeneratorNode = _CswNbtResources.Nodes[CurrentNodeKey.NodeId];
                SelectedGeneratorNodes.Add( CurrentGeneratorNode );

                TotalNodes += CswNbtActGenerateFutureNodes.makeNodes( CurrentGeneratorNode, EndDate );

            }//iterate selected Generator notes

            ret["result"] = TotalNodes.ToString();
            if( TotalNodes > 0 )
            {
                CswNbtView NodesView = CswNbtActGenerateFutureNodes.getTreeViewOfFutureNodes( SelectedGeneratorNodes );
                CswNbtWebServiceTree ws = new CswNbtWebServiceTree( _CswNbtResources, NodesView );
                ret["treedata"] = ws.runTree( null, null, false, true, string.Empty );

                //NodesView.SaveToCache( true );
                //CswNbtSessionDataId SessionViewId = NodesView.SessionViewId;
                //Master.setSessionViewId( SessionViewId, true );
            }

            return ret;
        } // futureScheduling()

    } // class CswNbtWebServiceGenerators

} // namespace ChemSW.Nbt.WebServices
