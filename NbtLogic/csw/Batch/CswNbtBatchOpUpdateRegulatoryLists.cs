using System;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Batch
{
    public class CswNbtBatchOpUpdateRegulatoryLists : ICswNbtBatchOp
    {
        private CswNbtResources _CswNbtResources;
        private NbtBatchOpName _BatchOpName = NbtBatchOpName.UpdateRegulatoryLists;

        public CswNbtBatchOpUpdateRegulatoryLists( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        /// <summary>
        /// Returns the percentage of the task that is complete
        /// </summary>
        public Double getPercentDone( CswNbtObjClassBatchOp BatchNode )
        {
            Double ret = 0;
            if( BatchNode != null && BatchNode.OpNameValue == NbtBatchOpName.UpdateRegulatoryLists )
            {
                RegulatoryListsBatchData BatchData = BatchNode.BatchData.Text;
                ret = ( BatchData.CASNosProcessed / BatchData.totalCASNos ) * 100;
            }
            return ret;
        } // getPercentDone()

        /// <summary>
        /// Create a new batch operation to update materials regulatory lists property
        /// </summary>
        public CswNbtObjClassBatchOp makeBatchOp( CswCommaDelimitedString CASNos, string listName )
        {
            CswNbtObjClassBatchOp BatchNode = null;
            if( false == CASNos.IsEmpty && null != listName )
            {
                RegulatoryListsBatchData BatchData = new RegulatoryListsBatchData( CASNos, listName );
                BatchNode = CswNbtBatchManager.makeNew( _CswNbtResources, _BatchOpName, BatchData );
            }
            return BatchNode;
        } // makeBatchOp()


        /// <summary>
        /// Run the next iteration of this batch operation
        /// </summary>
        public void runBatchOp( CswNbtObjClassBatchOp BatchNode )
        {
            try
            {
                if( BatchNode != null && BatchNode.OpNameValue == NbtBatchOpName.UpdateRegulatoryLists )
                {
                    BatchNode.start();
                    RegulatoryListsBatchData BatchData = BatchNode.BatchData.Text;

                    if( BatchData.MatchingMaterialIDs.Count > 0 ) //update materials
                    {
                        //update the current material
                        CswPrimaryKey currentMaterialID = new CswPrimaryKey();
                        currentMaterialID.FromString( BatchData.MatchingMaterialIDs[0] );
                        BatchData.MatchingMaterialIDs.RemoveAt( 0 );
                        CswNbtNode materialNode = _CswNbtResources.Nodes.GetNode( currentMaterialID );
                        CswNbtObjClassMaterial nodeAsMaterial = (CswNbtObjClassMaterial) materialNode;
                        nodeAsMaterial.RegulatoryLists.StaticText += "," + BatchData.ListName; //update the node
                        nodeAsMaterial.postChanges( false );

                        //get materials using the current material as a component
                        _getParentMaterials( currentMaterialID.PrimaryKey.ToString(), BatchData.MatchingMaterialIDs );

                        //save the updated batch data
                        BatchNode.appendToLog( "Updated " + currentMaterialID.ToString() );
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
                            CswNbtObjClassMaterial nodeAsMaterial = (CswNbtObjClassMaterial) materialsByCASNoTree.getNodeForCurrentPosition();
                            if( false == _materialHasList( BatchData.ListName, nodeAsMaterial ) )
                            {
                                BatchData.MatchingMaterialIDs.Add( nodeAsMaterial.NodeId.ToString() );
                            }
                            materialsByCASNoTree.goToParentNode();
                        }

                        //save the batch data
                        BatchNode.appendToLog( "Finishing processing CASNo: " + BatchData.CurrentCASNo );
                    }
                    else //we have no materials to update and no more CASNumbers to process, we're done
                    {
                        BatchNode.finish();
                    }
                    BatchNode.PercentDone.Value = getPercentDone( BatchNode );
                    BatchData.CASNosProcessed = BatchData.CASNosProcessed + 1;
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
            public string CurrentCASNo = "";
            public double totalCASNos;
            public double CASNosProcessed;

            public RegulatoryListsBatchData( CswCommaDelimitedString CASNos, string listName )
            {
                this.ListName = listName;
                this.CASNos = CASNos;
                totalCASNos = CASNos.Count;
                CASNosProcessed = 0;
            }

            private RegulatoryListsBatchData( CswCommaDelimitedString CASNos, string listName, CswCommaDelimitedString MatchingMaterialIDs, string CurrentCASNo, double processed, double total )
            {
                this.ListName = listName;
                this.CASNos = CASNos;
                this.CurrentCASNo = CurrentCASNo;
                this.MatchingMaterialIDs = MatchingMaterialIDs;
                this.CASNosProcessed = processed;
                this.totalCASNos = total;
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

                double CASNosProcessed = CswConvert.ToDouble( Obj["CASNosProcessed"] );
                double total = CswConvert.ToDouble( Obj["totalCASNos"] );

                return new RegulatoryListsBatchData( CASNos, listName, MatchingMaterialIDs, currentCASNo, CASNosProcessed, total );
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
                Obj["totalCASNos"] = totalCASNos.ToString();
                Obj["CASNosProcessed"] = CASNosProcessed.ToString();
                return Obj.ToString();
            }
        } // class InventoryLevelsBatchData

        #endregion InventoryLevelsBatchData

        #region private helper functions

        private void _getParentMaterials( string nodeid, CswCommaDelimitedString matchingNodesIDs )
        {
            CswNbtMetaDataObjectClass materialComponentOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialComponentClass );

            foreach( CswNbtMetaDataNodeType materialComponentNT in materialComponentOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp constituentNTP = materialComponentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterialComponent.ConstituentPropertyName );
                CswNbtView componentsView = new CswNbtView( _CswNbtResources );
                CswNbtViewRelationship parent = componentsView.AddViewRelationship( materialComponentNT, false );
                componentsView.AddViewPropertyAndFilter( parent, constituentNTP, Value: nodeid, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals, SubFieldName: CswNbtSubField.SubFieldName.NodeID );

                ICswNbtTree componentsTree = _CswNbtResources.Trees.getTreeFromView( componentsView, false );
                int nodesCount = componentsTree.getChildNodeCount();
                for( int i = 0; i < nodesCount; i++ )
                {
                    componentsTree.goToNthChild( i );
                    CswNbtObjClassMaterialComponent nodeAsMaterialComponent = (CswNbtObjClassMaterialComponent) componentsTree.getNodeForCurrentPosition();
                    matchingNodesIDs.Add( nodeAsMaterialComponent.Mixture.RelatedNodeId.ToString() );
                    componentsTree.goToParentNode();
                }
            }
        }

        private bool _materialHasList( string name, CswNbtObjClassMaterial material )
        {
            CswCommaDelimitedString lists = new CswCommaDelimitedString();
            lists.FromString( material.RegulatoryLists.StaticText );
            foreach( string listName in lists )
            {
                if( listName.Equals( name ) )
                {
                    return true;
                }
            }
            return false;
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

        #endregion

    } // class CswNbtBatchOpUpdateRegulatoryLists
} // namespace ChemSW.Nbt.Batch
