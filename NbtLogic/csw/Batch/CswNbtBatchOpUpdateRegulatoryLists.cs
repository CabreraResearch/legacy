﻿using System;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using ChemSW.Nbt.MetaData;
using ChemSW.Config;

namespace ChemSW.Nbt.Batch
{
    public class CswNbtBatchOpUpdateRegulatoryLists : ICswNbtBatchOp
    {
        private CswNbtResources _CswNbtResources;
        private CswEnumNbtBatchOpName _BatchOpName = CswEnumNbtBatchOpName.UpdateRegulatoryLists;

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
            if( BatchNode != null && BatchNode.OpNameValue == CswEnumNbtBatchOpName.UpdateRegulatoryLists )
            {
                RegulatoryListsBatchData BatchData = BatchNode.BatchData.Text;
                ret = ( ( BatchData.totalCASNos - BatchData.CASNos.Count ) / BatchData.totalCASNos ) * 100;
            }
            return ret;
        } // getPercentDone()

        /// <summary>
        /// Create a new batch operation to update materials regulatory lists property
        /// </summary>
        public CswNbtObjClassBatchOp makeBatchOp( string listName, CswCommaDelimitedString CASNos )
        {
            CswNbtObjClassBatchOp BatchNode = null;
            if( false == CASNos.IsEmpty && null != listName )
            {
                RegulatoryListsBatchData BatchData = new RegulatoryListsBatchData( listName, CASNos );
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
                if( BatchNode != null && BatchNode.OpNameValue == CswEnumNbtBatchOpName.UpdateRegulatoryLists )
                {
                    //BatchNode.start();
                    //RegulatoryListsBatchData BatchData = BatchNode.BatchData.Text;

                    //int processed = 0;
                    //int NodesPerCycle = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.NodesProcessedPerCycle ) );
                    //if( BatchData.MatchingMaterialIDs.Count > 0 ) //update materials
                    //{
                    //    //loop until we hit the limit of nodes processed per iteration or the list is empty
                    //    while( BatchData.MatchingMaterialIDs.Count > 0 && processed <= NodesPerCycle )
                    //    {
                    //        CswPrimaryKey currentMaterialID = new CswPrimaryKey();
                    //        currentMaterialID.FromString( BatchData.MatchingMaterialIDs[0] );
                    //        BatchData.MatchingMaterialIDs.RemoveAt( 0 );
                    //        CswNbtNode materialNode = _CswNbtResources.Nodes.GetNode( currentMaterialID );
                    //        CswNbtObjClassChemical nodeAsMaterial = (CswNbtObjClassChemical) materialNode;
                    //        if( false == _materialHasList( BatchData.ListName, nodeAsMaterial ) )
                    //        {
                    //            //update the current material
                    //            CswCommaDelimitedString RegLists = new CswCommaDelimitedString();
                    //            RegLists.FromString( nodeAsMaterial.RegulatoryLists.StaticText );
                    //            RegLists.Add( BatchData.ListName );
                    //            nodeAsMaterial.RegulatoryLists.StaticText = RegLists.ToString(); //update the node
                    //            nodeAsMaterial.postChanges( false );

                    //            //get materials using the current material as a component
                    //            nodeAsMaterial.getParentMaterials( ref BatchData.MatchingMaterialIDs );
                    //        }
                    //        processed++;
                    //    }
                    //}
                    //else if( BatchData.CASNos.Count > 0 ) //we have more CASNos to process
                    //{
                    //    //get the next CASNo to process
                    //    BatchData.CurrentCASNo = BatchData.CASNos[0];
                    //    BatchData.CASNos.RemoveAt( 0 );

                    //    //build the view
                    //    CswNbtView materialsByCASNoView = _getMaterialsByCASNoView( BatchData.CurrentCASNo );

                    //    //add all the materials that match the current CASNo and add them to the list if they don't already have the List on their regulatory lists property
                    //    ICswNbtTree materialsByCASNoTree = _CswNbtResources.Trees.getTreeFromView( materialsByCASNoView, false, false, false );
                    //    int nodeCount = materialsByCASNoTree.getChildNodeCount();
                    //    for( int i = 0; i < nodeCount; i++ )
                    //    {
                    //        materialsByCASNoTree.goToNthChild( i );
                    //        BatchData.MatchingMaterialIDs.Add( materialsByCASNoTree.getNodeIdForCurrentPosition().ToString() );
                    //        materialsByCASNoTree.goToParentNode();
                    //    }
                    //}
                    //else //we have no materials to update and no more CASNumbers to process, we're done
                    //{
                    //    BatchNode.finish();
                    //}
                    //BatchNode.PercentDone.Value = getPercentDone( BatchNode );
                    //BatchNode.BatchData.Text = BatchData.ToString();
                    //BatchNode.postChanges( false );
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

            public RegulatoryListsBatchData( string listName, CswCommaDelimitedString CASNos )
            {
                this.ListName = listName;
                this.CASNos = CASNos;
                totalCASNos = CASNos.Count;
            }

            private RegulatoryListsBatchData( CswCommaDelimitedString CASNos, string listName, CswCommaDelimitedString MatchingMaterialIDs, string CurrentCASNo, double total )
            {
                this.ListName = listName;
                this.CASNos = CASNos;
                this.CurrentCASNo = CurrentCASNo;
                this.MatchingMaterialIDs = MatchingMaterialIDs;
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

                double total = CswConvert.ToDouble( Obj["totalCASNos"] );

                return new RegulatoryListsBatchData( CASNos, listName, MatchingMaterialIDs, currentCASNo, total );
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
                return Obj.ToString();
            }
        } // class InventoryLevelsBatchData

        #endregion

        #region private helper functions

        //private bool _materialHasList( string name, CswNbtObjClassChemical material )
        //{
        //    CswCommaDelimitedString lists = new CswCommaDelimitedString();
        //    lists.FromString( material.RegulatoryLists.StaticText );
        //    return lists.Contains( name );
        //}

        private CswNbtView _getMaterialsByCASNoView( string CASNo )
        {
            CswNbtMetaDataObjectClass materialOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtMetaDataObjectClassProp casNoOCP = materialOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.CasNo );

            CswNbtView view = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship parent = view.AddViewRelationship( materialOC, false ); //add material to root
            view.AddViewPropertyAndFilter( parent, casNoOCP, Value: CASNo, FilterMode: CswEnumNbtFilterMode.Equals ); //add CASNo property with filter
            return view;
        }

        #endregion

    } // class CswNbtBatchOpUpdateRegulatoryLists
} // namespace ChemSW.Nbt.Batch
