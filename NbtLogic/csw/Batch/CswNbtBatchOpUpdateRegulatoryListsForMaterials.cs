using System;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Batch
{
    public class CswNbtBatchOpUpdateRegulatoryListsForMaterials : ICswNbtBatchOp
    {
        private CswNbtResources _CswNbtResources;
        private NbtBatchOpName _BatchOpName = NbtBatchOpName.UpdateRegulatoryListsForMaterials;

        public CswNbtBatchOpUpdateRegulatoryListsForMaterials( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        /// <summary>
        /// Returns the percentage of the task that is complete
        /// </summary>
        public Double getPercentDone( CswNbtObjClassBatchOp BatchNode )
        {
            Double ret = 0;
            if( BatchNode != null && BatchNode.OpNameValue == NbtBatchOpName.UpdateRegulatoryListsForMaterials )
            {
                RegulatoryListsBatchData BatchData = BatchNode.BatchData.Text;
                //update percent done based on how many Materials have been processed
            }
            return ret;
        } // getPercentDone()

        /// <summary>
        /// Create a new batch operation to update materials regulatory lists property
        /// </summary>
        public CswNbtObjClassBatchOp makeBatchOp( CswCommaDelimitedString MaterialIDs )
        {
            CswNbtObjClassBatchOp BatchNode = null;
            //get all existing of RegLists
            CswCommaDelimitedString nodeids = _getRegListsIDs();

            RegulatoryListsBatchData BatchData = new RegulatoryListsBatchData( MaterialIDs, nodeids );
            BatchNode = CswNbtBatchManager.makeNew( _CswNbtResources, _BatchOpName, BatchData );
            return BatchNode;
        } // makeBatchOp()

        /// <summary>
        /// Run the next iteration of this batch operation
        /// </summary>
        public void runBatchOp( CswNbtObjClassBatchOp BatchNode )
        {
            try
            {
                if( BatchNode != null && BatchNode.OpNameValue == NbtBatchOpName.UpdateRegulatoryListsForMaterials )
                {
                    BatchNode.start();
                    RegulatoryListsBatchData BatchData = BatchNode.BatchData.Text;
                    if( BatchData.MatchingMaterialIDs.Count > 0 ) //update materials
                    {
                        CswPrimaryKey currentMaterialID = new CswPrimaryKey();
                        currentMaterialID.FromString( BatchData.MatchingMaterialIDs[0] );
                        BatchData.MatchingMaterialIDs.RemoveAt( 0 );

                        CswNbtNode materialNode = _CswNbtResources.Nodes.GetNode( currentMaterialID );
                        CswNbtObjClassMaterial nodeAsMaterial = (CswNbtObjClassMaterial) materialNode;

                        if( false == BatchData.SeenIDs.Contains( nodeAsMaterial.NodeId.ToString() ) ) //if this is the first time we're seeing a material, delete it's regulatory lists
                        {                                                                    //we have to assume that since a CASNo was updated the chain of Regulatory lists is broken and need to be rebuilt
                            nodeAsMaterial.RegulatoryLists.StaticText = "";
                            BatchData.SeenIDs.Add( nodeAsMaterial.NodeId.ToString() );
                        }

                        if( false == _materialHasList( BatchData.ListName, nodeAsMaterial ) )
                        {
                            //update the current material
                            nodeAsMaterial.RegulatoryLists.StaticText += "," + BatchData.ListName; //update the node

                            //get materials using the current material as a component
                            nodeAsMaterial.getParentMaterials( ref BatchData.MatchingMaterialIDs );

                            //save the updated batch data
                            BatchNode.appendToLog( "Updated " + currentMaterialID.ToString() );
                        }
                        nodeAsMaterial.postChanges( false ); //update the node no matter what
                    }
                    else if( BatchData.CASNos.Count > 0 ) //we have more CASNos to process
                    {
                        //get the next CASNo to process
                        BatchData.CurrentCASNo = BatchData.CASNos[0];
                        BatchData.CASNos.RemoveAt( 0 );

                        //build the view
                        CswNbtView materialsByCASNoView = _getMaterialsByCASNoView( BatchData.CurrentCASNo );

                        //add all the materials that match the current CASNo and add them to the list if they don't already have the List on their regulatory lists property
                        ICswNbtTree materialsByCASNoTree = _CswNbtResources.Trees.getTreeFromView( materialsByCASNoView, false );
                        int nodeCount = materialsByCASNoTree.getChildNodeCount();
                        for( int i = 0; i < nodeCount; i++ )
                        {
                            materialsByCASNoTree.goToNthChild( i );
                            BatchData.MatchingMaterialIDs.Add( materialsByCASNoTree.getNodeIdForCurrentPosition().ToString() );
                            materialsByCASNoTree.goToParentNode();
                        }

                        //save the batch data
                        BatchNode.appendToLog( "Finishing processing CASNo: " + BatchData.CurrentCASNo );
                    }
                    else if( BatchData.RegListsNodeIDs.Count > 0 )
                    {
                        //get the next reglist to process
                        CswPrimaryKey pk = new CswPrimaryKey();
                        pk.FromString( BatchData.RegListsNodeIDs[0] );
                        BatchData.RegListsNodeIDs.RemoveAt( 0 );
                        CswNbtObjClassRegulatoryList nodeAsRegList = _CswNbtResources.Nodes.GetNode( pk );

                        //update the CASNos and ListName to process
                        BatchData.ListName = nodeAsRegList.Name.Text;
                        CswCommaDelimitedString nextCASNos = new CswCommaDelimitedString();
                        nextCASNos.FromString( nodeAsRegList.CASNumbers.Text );
                        BatchData.CASNos = nextCASNos;
                    }
                    else //we have no materials to update and no more CASNumbers to process, we're done
                    {
                        BatchNode.finish();
                    }
                    BatchNode.PercentDone.Value = getPercentDone( BatchNode );
                    BatchNode.BatchData.Text = BatchData.ToString();
                    BatchNode.postChanges( false );
                }
            }
            catch( Exception ex )
            {
                if( BatchNode != null ) BatchNode.error( ex );
            }
        } // runBatchOp()

        #region RegulatoryListsBatchData

        // This internal class is specific to this batch operation
        private class RegulatoryListsBatchData
        {
            public string ListName;
            public CswCommaDelimitedString MatchingMaterialIDs = new CswCommaDelimitedString();
            public CswCommaDelimitedString CASNos = new CswCommaDelimitedString();
            public CswCommaDelimitedString RegListsNodeIDs = new CswCommaDelimitedString();
            public CswCommaDelimitedString SeenIDs = new CswCommaDelimitedString();
            public string CurrentCASNo = "";

            public RegulatoryListsBatchData( CswCommaDelimitedString MatchingMaterialIDs, CswCommaDelimitedString RegListsNodeIDs )
            {
                this.MatchingMaterialIDs = MatchingMaterialIDs;
                this.RegListsNodeIDs = RegListsNodeIDs;
            }

            private RegulatoryListsBatchData( CswCommaDelimitedString CASNos,
                string listName,
                CswCommaDelimitedString MatchingMaterialIDs,
                string CurrentCASNo,
                CswCommaDelimitedString RegListsNodeIDs,
                CswCommaDelimitedString SeenIDs )
            {
                this.ListName = listName;
                this.CASNos = CASNos;
                this.CurrentCASNo = CurrentCASNo;
                this.MatchingMaterialIDs = MatchingMaterialIDs;
                this.RegListsNodeIDs = RegListsNodeIDs;
                this.SeenIDs = SeenIDs;
            }

            public static implicit operator RegulatoryListsBatchData( string item )
            {
                JObject Obj = CswConvert.ToJObject( item );
                string listName = Obj["listName"].ToString();

                string currentCASNo = Obj["CurrentCASNo"].ToString();

                CswCommaDelimitedString CASNos = new CswCommaDelimitedString();
                CASNos.FromString( Obj["CASNos"].ToString() );

                CswCommaDelimitedString MatchingMaterialIDs = new CswCommaDelimitedString();
                MatchingMaterialIDs.FromString( Obj["MatchingMaterialIDs"].ToString() );

                CswCommaDelimitedString RegListsNodeIDs = new CswCommaDelimitedString();
                RegListsNodeIDs.FromString( Obj["RegListsNodeIDs"].ToString() );

                CswCommaDelimitedString SeenIDs = new CswCommaDelimitedString();
                SeenIDs.FromString( Obj["SeenIDs"].ToString() );

                return new RegulatoryListsBatchData( CASNos, listName, MatchingMaterialIDs, currentCASNo, RegListsNodeIDs, SeenIDs );
            }

            public static implicit operator string( RegulatoryListsBatchData item )
            {
                return item.ToString();
            }

            public override string ToString()
            {
                JObject Obj = new JObject();
                Obj["listName"] = ListName;
                Obj["CASNos"] = CASNos.ToString();
                Obj["MatchingMaterialIDs"] = MatchingMaterialIDs.ToString();
                Obj["CurrentCASNo"] = CurrentCASNo;
                Obj["RegListsNodeIDs"] = RegListsNodeIDs.ToString();
                Obj["SeenIDs"] = SeenIDs.ToString();
                return Obj.ToString();
            }
        } // class InventoryLevelsBatchData

        #endregion

        #region private helper functions

        private bool _materialHasList( string name, CswNbtObjClassMaterial material )
        {
            CswCommaDelimitedString lists = new CswCommaDelimitedString();
            lists.FromString( material.RegulatoryLists.StaticText );
            return lists.Contains( name );
        }

        private CswNbtView _getMaterialsByCASNoView( string CASNo )
        {
            CswNbtMetaDataObjectClass materialOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClassProp casNoOCP = materialOC.getObjectClassProp( CswNbtObjClassMaterial.CasNoPropertyName );

            CswNbtView view = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship parent = view.AddViewRelationship( materialOC, false ); //add material to root
            view.AddViewPropertyAndFilter( parent, casNoOCP, Value: CASNo, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals ); //add CASNo property with filter
            return view;
        }

        private CswCommaDelimitedString _getRegListsIDs()
        {
            CswCommaDelimitedString RegListNodeIDs = new CswCommaDelimitedString();
            CswNbtMetaDataObjectClass regListOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RegulatoryListClass );
            foreach( CswNbtNode regList in regListOC.getNodes( false, false ) )
            {
                RegListNodeIDs.Add( regList.NodeId.ToString() );
            }
            return RegListNodeIDs;
        }

        #endregion

    } // class CswNbtBatchOpUpdateRegulatoryListsForMaterials
} // namespace ChemSW.Nbt.Batch
