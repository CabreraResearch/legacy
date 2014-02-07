using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.Statistics;
using NbtWebApp;
using NbtWebApp.WebSvc.Returns;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceNode
    {
        #region Properties and ctor

        private readonly CswNbtResources _CswNbtResources;
        private readonly CswNbtStatisticsEvents _CswNbtStatisticsEvents;
        private readonly CswNbtSdNode _NodeSd;
        public CswNbtWebServiceNode( CswNbtResources CswNbtResources, CswNbtStatisticsEvents CswNbtStatisticsEvents )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtStatisticsEvents = CswNbtStatisticsEvents;
            _NodeSd = new CswNbtSdNode( _CswNbtResources, _CswNbtStatisticsEvents );
        }

        #endregion Properties and ctor

        #region Copy

        public CswNbtNode CopyNode( CswPrimaryKey NodePk )
        {
            return _NodeSd.CopyNode( NodePk );
        }

        [DataContract]
        public class CopyDataRequest
        {
            [DataMember]
            public string NodeId = string.Empty;

            [DataMember]
            public string CopyType = string.Empty;
        }

        [DataContract]
        public class CopyDataReturn : CswWebSvcReturn
        {
            public CopyDataReturn()
            {
                Data = new CopyTypeData();
            }

            [DataMember]
            public CopyTypeData Data;

            [DataContract]
            public class CopyTypeData
            {
                //If we get any more copy types in the future, add their Response objects here 
                //(and make sure they're named the same as the expected CopyType/CswEnumNbtActionName value)
                public CopyTypeData()
                {
                    Create_Material = new CswNbtWebServiceC3Search.C3CreateMaterialResponse();
                }
                [DataMember]
                public CswNbtWebServiceC3Search.C3CreateMaterialResponse Create_Material;
            }
        }

        public static void getCopyData( ICswResources _CswResources, CopyDataReturn Copy, CopyDataRequest Request )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) _CswResources;
            //If we get any more copy types in the future, extract them out into their own classes and instantiate them via factory
            #region Create_Material Copy Data

            if( Request.CopyType == CswEnumNbtActionName.Create_Material )
            {
                CswPrimaryKey OriginalNodeId = CswConvert.ToPrimaryKey( Request.NodeId );
                if( CswTools.IsPrimaryKey( OriginalNodeId ) )
                {
                    CswNbtPropertySetMaterial OriginalMaterial = _CswNbtResources.Nodes.GetNode( OriginalNodeId );
                    if( null != OriginalMaterial )
                    {
                        #region Material Properties

                        CswNbtPropertySetMaterial MaterialCopy = OriginalMaterial.CopyNode();
                        Copy.Data.Create_Material = new CswNbtWebServiceC3Search.C3CreateMaterialResponse
                        {
                            actionname = CswEnumNbtActionName.Create_Material,
                            state = new CswNbtWebServiceC3Search.C3CreateMaterialResponse.State
                            {
                                materialId = MaterialCopy.NodeId.ToString(),
                                materialType = new CswNbtWebServiceC3Search.C3CreateMaterialResponse.State.MaterialType
                                {
                                    name = MaterialCopy.NodeType.NodeTypeName,
                                    val = CswConvert.ToInt32( MaterialCopy.NodeTypeId )
                                },
                                tradeName = OriginalMaterial.TradeName.Text,
                                partNo = OriginalMaterial.PartNumber.Text,
                                supplier = new CswNbtWebServiceC3Search.C3CreateMaterialResponse.State.Supplier
                                {
                                    name = OriginalMaterial.Supplier.CachedNodeName,
                                    val = OriginalMaterial.Supplier.RelatedNodeId.ToString()
                                },
                                sizes = new Collection<CswNbtWebServiceC3Search.C3CreateMaterialResponse.State.SizeRecord>(),
                                showOriginalUoM = false
                            }
                        };

                        #endregion Material Properties

                        #region Sizes

                        CswNbtMetaDataObjectClass SizeOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.SizeClass );
                        CswNbtMetaDataObjectClassProp MaterialOCP = SizeOC.getObjectClassProp( CswNbtObjClassSize.PropertyName.Material );
                        CswNbtView SizesView = new CswNbtView( _CswNbtResources )
                        {
                            ViewName = "MaterialCopySizes"
                        };
                        CswNbtViewRelationship SizeVR = SizesView.AddViewRelationship( SizeOC, false );
                        SizesView.AddViewPropertyAndFilter( SizeVR, MaterialOCP, OriginalMaterial.NodeId.PrimaryKey.ToString(), CswEnumNbtSubFieldName.NodeID );
                        ICswNbtTree SizesTree = _CswNbtResources.Trees.getTreeFromView( SizesView, false, false, false );
                        for( int i = 0; i < SizesTree.getChildNodeCount(); i++ )
                        {
                            SizesTree.goToNthChild( i );
                            CswNbtObjClassSize SizeNode = SizesTree.getNodeForCurrentPosition();
                            Copy.Data.Create_Material.state.sizes.Add( new CswNbtWebServiceC3Search.C3CreateMaterialResponse.State.SizeRecord
                            {
                                nodeTypeId = new CswNbtWebServiceC3Search.C3CreateMaterialResponse.State.SizeRecord.SizeData
                                {
                                    value = SizeNode.NodeTypeId.ToString(),
                                    readOnly = true,
                                    hidden = true
                                },
                                unitCount = new CswNbtWebServiceC3Search.C3CreateMaterialResponse.State.SizeRecord.SizeData
                                {
                                    value = CswConvert.ToString( SizeNode.UnitCount.Value ),
                                    readOnly = true,
                                    hidden = false
                                },
                                quantity = new CswNbtWebServiceC3Search.C3CreateMaterialResponse.State.SizeRecord.SizeData
                                {
                                    value = CswConvert.ToString( SizeNode.InitialQuantity.Quantity ),
                                    readOnly = true,
                                    hidden = false
                                },
                                uom = new CswNbtWebServiceC3Search.C3CreateMaterialResponse.State.SizeRecord.SizeData
                                {
                                    value = SizeNode.InitialQuantity.CachedUnitName,
                                    readOnly = false == string.IsNullOrEmpty( SizeNode.InitialQuantity.CachedUnitName ),
                                    hidden = false
                                },
                                catalogNo = new CswNbtWebServiceC3Search.C3CreateMaterialResponse.State.SizeRecord.SizeData
                                {
                                    value = SizeNode.CatalogNo.Text,
                                    readOnly = true,
                                    hidden = false
                                },
                                quantityEditable = new CswNbtWebServiceC3Search.C3CreateMaterialResponse.State.SizeRecord.SizeData
                                {
                                    value = SizeNode.QuantityEditable.Checked,
                                },
                                dispensible = new CswNbtWebServiceC3Search.C3CreateMaterialResponse.State.SizeRecord.SizeData
                                {
                                    value = SizeNode.Dispensable.Checked
                                }
                            } );
                            SizesTree.goToParentNode();
                        }

                        #endregion Sizes

                        #region Synonyms

                        CswNbtMetaDataObjectClass MaterialSynonymOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialSynonymClass );
                        CswNbtMetaDataObjectClassProp SynMaterialOCP = MaterialSynonymOC.getObjectClassProp( CswNbtObjClassMaterialSynonym.PropertyName.Material );
                        CswNbtView SynonymsView = new CswNbtView( _CswNbtResources )
                        {
                            ViewName = "MaterialCopySynonyms"
                        };
                        CswNbtViewRelationship SynonymsVR = SynonymsView.AddViewRelationship( MaterialSynonymOC, false );
                        SynonymsView.AddViewPropertyAndFilter( SynonymsVR, SynMaterialOCP, OriginalMaterial.NodeId.PrimaryKey.ToString(), CswEnumNbtSubFieldName.NodeID );
                        ICswNbtTree SynonymsTree = _CswNbtResources.Trees.getTreeFromView( SynonymsView, false, false, false );
                        for( int i = 0; i < SynonymsTree.getChildNodeCount(); i++ )
                        {
                            SynonymsTree.goToNthChild( i );
                            CswNbtObjClassMaterialSynonym SynonymNode = SynonymsTree.getNodeForCurrentPosition();
                            CswNbtObjClassMaterialSynonym SynonymCopy = SynonymNode.CopyNode();
                            SynonymCopy.Material.RelatedNodeId = MaterialCopy.NodeId;
                            SynonymCopy.postChanges( false );
                            SynonymsTree.goToParentNode();
                        }

                        #endregion Synonyms

                        if( MaterialCopy.ObjectClass.ObjectClass == CswEnumNbtObjectClass.ChemicalClass )
                        {
                            if( CswEnumTristate.False == MaterialCopy.IsConstituent.Checked )
                            {
                                #region SDS

                                CswNbtView SDSView = CswNbtObjClassSDSDocument.getAssignedSDSDocumentsView( _CswNbtResources, OriginalMaterial.NodeId );
                                ICswNbtTree SDSTree = _CswNbtResources.Trees.getTreeFromView( SDSView, false, false, false );
                                SDSTree.goToNthChild( 0 );
                                for( int i = 0; i < SDSTree.getChildNodeCount(); i++ )
                                {
                                    SDSTree.goToNthChild( i );
                                    CswNbtObjClassSDSDocument SDSDoc = SDSTree.getNodeForCurrentPosition();
                                    CswNbtObjClassSDSDocument SDSCopy = SDSDoc.CopyNode( IsNodeTemp: true );

                                    SDSCopy.Owner.RelatedNodeId = MaterialCopy.NodeId;
                                    SDSCopy.postChanges( false );

                                    if( i == 0 )
                                    {
                                        Copy.Data.Create_Material.state.sds.sdsDocId = SDSCopy.NodeId.ToString();
                                    }
                                    SDSTree.goToParentNode();
                                }

                                #endregion SDS

                                #region Components

                                CswNbtMetaDataObjectClass MaterialComponentOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialComponentClass );
                                CswNbtMetaDataObjectClassProp CompMaterialOCP = MaterialComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Mixture );
                                CswNbtView ComponentsView = new CswNbtView( _CswNbtResources )
                                {
                                    ViewName = "MaterialCopyComponents"
                                };
                                CswNbtViewRelationship ComponentsVR = ComponentsView.AddViewRelationship( MaterialComponentOC, false );
                                ComponentsView.AddViewPropertyAndFilter( ComponentsVR, CompMaterialOCP, OriginalMaterial.NodeId.PrimaryKey.ToString(), CswEnumNbtSubFieldName.NodeID );
                                ICswNbtTree ComponentsTree = _CswNbtResources.Trees.getTreeFromView( ComponentsView, false, false, false );
                                for( int i = 0; i < ComponentsTree.getChildNodeCount(); i++ )
                                {
                                    ComponentsTree.goToNthChild( i );
                                    CswNbtObjClassMaterialComponent ComponentNode = ComponentsTree.getNodeForCurrentPosition();
                                    CswNbtObjClassMaterialComponent ComponentCopy = ComponentNode.CopyNode();
                                    ComponentCopy.Mixture.RelatedNodeId = MaterialCopy.NodeId;
                                    ComponentCopy.Constituent.RelatedNodeId = ComponentNode.Constituent.RelatedNodeId;
                                    ComponentCopy.postChanges( false );
                                    ComponentsTree.goToParentNode();
                                }

                                #endregion Components
                            }

                            #region GHS

                            CswNbtMetaDataObjectClass GHSOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSClass );
                            CswNbtMetaDataObjectClassProp GHSMaterialOCP = GHSOC.getObjectClassProp( CswNbtObjClassGHS.PropertyName.Material );
                            CswNbtView GHSView = new CswNbtView( _CswNbtResources )
                            {
                                ViewName = "MaterialCopyGHS"
                            };
                            CswNbtViewRelationship GHSVR = GHSView.AddViewRelationship( GHSOC, false );
                            GHSView.AddViewPropertyAndFilter( GHSVR, GHSMaterialOCP, OriginalMaterial.NodeId.PrimaryKey.ToString(), CswEnumNbtSubFieldName.NodeID );
                            ICswNbtTree GHSTree = _CswNbtResources.Trees.getTreeFromView( GHSView, false, false, false );
                            for( int i = 0; i < GHSTree.getChildNodeCount(); i++ )
                            {
                                GHSTree.goToNthChild( i );
                                CswNbtObjClassGHS GHSNode = GHSTree.getNodeForCurrentPosition();
                                CswNbtObjClassGHS GHSCopy = GHSNode.CopyNode();
                                GHSCopy.Material.RelatedNodeId = MaterialCopy.NodeId;
                                GHSCopy.postChanges( false );
                                GHSTree.goToParentNode();
                            }

                            #endregion GHS
                        }
                    }
                }
            }

            #endregion Create_Material Copy Data
        }

        #endregion Copy

        #region Delete

        public JObject DeleteNodes( string[] NodePks, string[] NodeKeys )
        {
            JObject ret = new JObject();
            Collection<CswPrimaryKey> NodePrimaryKeys = new Collection<CswPrimaryKey>();

            if( NodeKeys.Length > 0 )
            {
                foreach( string NodeKey in NodeKeys )
                {
                    CswNbtNodeKey NbtNodeKey = new CswNbtNodeKey( NodeKey );
                    if( null != NbtNodeKey &&
                        null != NbtNodeKey.NodeId &&
                        CswTools.IsPrimaryKey( NbtNodeKey.NodeId ) &&
                        false == NodePrimaryKeys.Contains( NbtNodeKey.NodeId ) )
                    {
                        NodePrimaryKeys.Add( NbtNodeKey.NodeId );
                    }
                }
            }
            if( NodePks.Length > 0 )
            {
                foreach( string NodePk in NodePks )
                {
                    CswPrimaryKey PrimaryKey = CswConvert.ToPrimaryKey( NodePk );
                    if( CswTools.IsPrimaryKey( PrimaryKey ) &&
                        false == NodePrimaryKeys.Contains( PrimaryKey ) )
                    {
                        NodePrimaryKeys.Add( PrimaryKey );
                    }
                }
            }
            if( NodePrimaryKeys.Count > 0 )
            {
                if( NodePrimaryKeys.Count < CswNbtBatchManager.getBatchThreshold( _CswNbtResources ) )
                {
                    bool success = true;
                    string DeletedNodes = "";
                    foreach( CswPrimaryKey Npk in NodePrimaryKeys )
                    {
                        string DeletedNode = "";
                        success = DeleteNode( Npk, out DeletedNode ) && success;
                        if( success )
                        {
                            DeletedNodes += DeletedNode;
                        }
                    }
                    ret["Succeeded"] = success.ToString();
                }
                else
                {
                    CswNbtBatchOpMultiDelete op = new CswNbtBatchOpMultiDelete( _CswNbtResources );
                    CswNbtObjClassBatchOp BatchNode = op.makeBatchOp( NodePrimaryKeys );
                    ret["batch"] = BatchNode.NodeId.ToString();
                }
            }

            return ret;
        }

        public bool DeleteNode( CswPrimaryKey NodePk, out string DeletedNodeName, bool DeleteAllRequiredRelatedNodes = false )
        {
            return _NodeSd.DeleteNode( NodePk, out DeletedNodeName, DeleteAllRequiredRelatedNodes );
        }

        #endregion Delete

        public JObject doObjectClassButtonClick( CswPropIdAttr PropId, string SelectedText, string TabIds, JObject ReturnProps, string NodeIds, string PropIds )
        {
            return _NodeSd.doObjectClassButtonClick( PropId, SelectedText, TabIds, ReturnProps, NodeIds, PropIds );
        }

        #region Add

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

        /// <summary>
        /// Creates a temporary node of the given NodeTypeId and returns a view containing the temp node
        /// </summary>
        /// <param name="_CswResources">Resources</param>
        /// <param name="Response">Repsonse Object containing the ViewId</param>
        /// <param name="NodeTypeId">NodeTypeId of which to create a temp node</param>
        public static void createTempNode( ICswResources _CswResources, CswNbtViewIdReturn Response, string NodeTypeId )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) _CswResources;
            CswNbtMetaDataNodeType NT = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( NodeTypeId ) );
            CswNbtNode TempNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NT.NodeTypeId, null, true );
            CswNbtView TempView = TempNode.getViewOfNode( false );
            TempView.Root.ChildRelationships[0].AllowAdd = true;
            TempView.IncludeTempNodes = true;
            TempView.SaveToCache( false );
            Response.Data.ViewId = TempView.SessionViewId.ToString();
        }

        #endregion Add

        #region Get

        public JObject getQuantityFromSize( CswPrimaryKey SizeId, string Action )
        {
            JObject Ret = new JObject();

            CswNbtObjClassSize Size = _CswNbtResources.Nodes.GetNode( SizeId );
            if( null != Size )
            {
                CswNbtNodePropQuantity InitialQuantity = Size.InitialQuantity;
                InitialQuantity.ToJSON( Ret );
                Ret["unitName"] = Ret["name"];
                Ret["qtyReadonly"] = false;
                Ret["isUnitReadOnly"] = false;
                Ret["unitCount"] = "1";
                Ret["isRequired"] = InitialQuantity.Required.ToString();
                if( Action.ToLower() == ChemSW.Nbt.ObjClasses.CswEnumNbtButtonAction.receive.ToString() )
                {
                    Ret["isUnitReadOnly"] = true;
                    if( Size.QuantityEditable.Checked == CswEnumTristate.False )
                    {
                        Ret["qtyReadonly"] = true;
                    }
                    Ret["unitCount"] = CswTools.IsDouble( Size.UnitCount.Value ) ? Size.UnitCount.Value.ToString() : "";
                }
                else if( Action.ToLower() == ChemSW.Nbt.ObjClasses.CswEnumNbtButtonAction.dispense.ToString() )
                {
                    CswNbtObjClassUnitOfMeasure UnitNode = _CswNbtResources.Nodes.GetNode( Size.InitialQuantity.UnitId );
                    if( null != UnitNode &&
                    ( UnitNode.UnitType.Value == CswEnumNbtUnitTypes.Each.ToString() ||
                    false == CswTools.IsDouble( UnitNode.ConversionFactor.Base ) ) )
                    {
                        Ret["isUnitReadOnly"] = true;
                    }
                }
            }
            return Ret;
        }

        /// <summary>
        /// WCF wrapper around getNodes
        /// </summary>
        public static void getNodes( ICswResources CswResources, NodeResponse Response, NodeSelect.Request Request )
        {
            if( null != CswResources )
            {
                CswNbtResources NbtResources = (CswNbtResources) CswResources;
                CswNbtSdNode sd = new CswNbtSdNode( NbtResources );
                Response.Data = sd.getNodes( Request );
            }
        }

        public static void getSizes( ICswResources CswResources, NodeResponse Response, CswNbtNode.Node Request )
        {
            if( null != CswResources )
            {
                CswNbtResources NbtResources = (CswNbtResources) CswResources;

                CswPrimaryKey pk = CswConvert.ToPrimaryKey( Request.NodeId );
                if( CswTools.IsPrimaryKey( pk ) )
                {
                    CswNbtMetaDataObjectClass sizeOC = NbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.SizeClass );
                    CswNbtMetaDataObjectClassProp materialOCP = sizeOC.getObjectClassProp( CswNbtObjClassSize.PropertyName.Material );

                    CswNbtView sizesView = new CswNbtView( NbtResources );
                    CswNbtViewRelationship parent = sizesView.AddViewRelationship( sizeOC, true );
                    sizesView.AddViewPropertyAndFilter( parent,
                        MetaDataProp: materialOCP,
                        Value: pk.PrimaryKey.ToString(),
                        SubFieldName: CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID,
                        FilterMode: CswEnumNbtFilterMode.Equals );

                    ICswNbtTree tree = NbtResources.Trees.getTreeFromView( sizesView, true, false, false );
                    for( int i = 0; i < tree.getChildNodeCount(); i++ )
                    {
                        tree.goToNthChild( i );
                        Response.Data.Nodes.Add( new CswNbtNode.Node( null )
                        {
                            NodeId = tree.getNodeIdForCurrentPosition(),
                            NodeName = tree.getNodeNameForCurrentPosition()
                        } );
                        tree.goToParentNode();
                    }

                }
            }
        }

        #endregion Get

        #region Favorite

        /// <summary>
        /// Adds the node to the current user's Favorites
        /// </summary>
        /// <param name="_CswResources">Resources</param>
        /// <param name="Response">Empty Repsonse Object</param>
        /// <param name="NodeId">NodeId to Favorite</param>
        public static void addToFavorites( ICswResources _CswResources, CswWebSvcReturn Response, String NodeId )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) _CswResources;
            CswPrimaryKey NodePK = CswConvert.ToPrimaryKey( NodeId );
            Int32 UserId = _CswNbtResources.CurrentNbtUser.UserId.PrimaryKey;
            toggleFavorite( _CswNbtResources, NodePK.PrimaryKey, UserId, CswEnumTristate.True );
        }

        /// <summary>
        /// Removes the node from the current user's Favorites
        /// </summary>
        /// <param name="_CswResources">Resources</param>
        /// <param name="Response">Empty Repsonse Object</param>
        /// <param name="NodeId">NodeId to un-Favorite</param>
        public static void removeFromFavorites( ICswResources _CswResources, CswWebSvcReturn Response, String NodeId )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) _CswResources;
            CswPrimaryKey NodePK = CswConvert.ToPrimaryKey( NodeId );
            Int32 UserId = _CswNbtResources.CurrentNbtUser.UserId.PrimaryKey;
            toggleFavorite( _CswNbtResources, NodePK.PrimaryKey, UserId, CswEnumTristate.False );
        }

        /// <summary>
        /// Toggles the favorite status of the node for the given user
        /// </summary>
        /// <param name="_CswResources">Resources</param>
        /// <param name="NodeId">NodeId to Favorite</param>
        /// <param name="UserId">UserId with which to associate Favorite</param>
        /// <param name="Add">When true, force Add; when false, force remove; when null, toggle</param>
        public static void toggleFavorite( CswNbtResources _CswNbtResources, Int32 NodeId, Int32 UserId, CswEnumTristate Add )
        {
            CswTableUpdate FavoritesUpdate = _CswNbtResources.makeCswTableUpdate( "favoritesUpdate", "favorites" );
            DataTable FavoritesTable = FavoritesUpdate.getTable( "where itemid = " + NodeId + " and userid = " + UserId );
            if( Add != CswEnumTristate.False && FavoritesTable.Rows.Count == 0 )
            {
                DataRow FavoritesRow = FavoritesTable.NewRow();
                FavoritesRow["userid"] = UserId;
                FavoritesRow["itemid"] = NodeId;
                FavoritesTable.Rows.Add( FavoritesRow );
                FavoritesUpdate.update( FavoritesTable );
            }
            else if( Add != CswEnumTristate.True && FavoritesTable.Rows.Count > 0 )
            {
                FavoritesTable.Rows[0].Delete();
                FavoritesUpdate.update( FavoritesTable );
            }
        }

        #endregion Favorite

        #region Merge

        [DataContract]
        public class MergeInfoRequest
        {
            [DataMember]
            public string NodeId1 = string.Empty;
            [DataMember]
            public string NodeId2 = string.Empty;
        }

        [DataContract]
        public class MergeInfoReturn : CswWebSvcReturn
        {
            [DataMember]
            public CswNbtActMerge.MergeInfoData Data;
        }

        [DataContract]
        public class MergeChoicesRequest
        {
            [DataMember]
            public CswNbtActMerge.MergeInfoData Choices = null;
        }

        [DataContract]
        public class MergeFinishReturn : CswWebSvcReturn
        {
            public MergeFinishReturn()
            {
                Data = new MergeFinishData();
            }

            [DataMember]
            public MergeFinishData Data;

            [DataContract]
            public class MergeFinishData
            {
                [DataMember]
                public string ViewId;
            }
        }


        public static void getMergeInfo( ICswResources CswResources, MergeInfoReturn Return, MergeInfoRequest Request )
        {
            CswNbtActMerge Merge = new CswNbtActMerge( (CswNbtResources) CswResources );
            Return.Data = Merge.getMergeInfo( CswConvert.ToPrimaryKey( Request.NodeId1 ), CswConvert.ToPrimaryKey( Request.NodeId2 ) );
        }

        public static void applyMergeChoices( ICswResources CswResources, MergeInfoReturn Return, MergeChoicesRequest Request )
        {
            CswNbtActMerge Merge = new CswNbtActMerge( (CswNbtResources) CswResources );
            Return.Data = Merge.applyMergeChoices( Request.Choices );
        }

        public static void finishMerge( ICswResources CswResources, MergeFinishReturn Return, MergeChoicesRequest Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            CswNbtActMerge Merge = new CswNbtActMerge( NbtResources );
            CswNbtView view = Merge.finishMerge( Request.Choices );
            view.SaveToCache( IncludeInQuickLaunch: false );
            Return.Data.ViewId = view.SessionViewId.ToString();
        } // finishMerge()

        #endregion Merge


    } // class CswNbtWebServiceNode

} // namespace ChemSW.Nbt.WebServices
