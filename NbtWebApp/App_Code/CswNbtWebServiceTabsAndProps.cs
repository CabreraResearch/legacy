﻿using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceTabsAndProps
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly ICswNbtUser _ThisUser;
        private readonly bool _IsMultiEdit;
        private string HistoryTabPrefix = "history_";

        public CswNbtWebServiceTabsAndProps( CswNbtResources CswNbtResources, bool Multi = false )
        {
            _CswNbtResources = CswNbtResources;
            _ThisUser = _CswNbtResources.CurrentNbtUser;
            _IsMultiEdit = Multi;
        }

        public JObject getTabs( NodeEditMode EditMode, string NodeId, string NodeKey, Int32 NodeTypeId, CswDateTime Date, string filterToPropId )
        {
            JObject Ret = new JObject();

            CswNbtNode Node = wsTools.getNode( _CswNbtResources, NodeId, NodeKey, Date );
            if( filterToPropId != string.Empty )
            {
                CswPropIdAttr PropId = new CswPropIdAttr( filterToPropId );
                CswNbtMetaDataNodeTypeProp Prop = _CswNbtResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId );
				CswNbtMetaDataNodeTypeTab Tab = _CswNbtResources.MetaData.getNodeTypeTab( Prop.EditLayout.TabId );
                if( _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.View, Prop.NodeType, false, Tab, _CswNbtResources.CurrentNbtUser, Node, Prop ) )
                {
                    _makeTab( Ret, Tab.TabOrder.ToString(), Tab.TabId.ToString(), Tab.TabName, false );
                }
            }
            else
            {
                //switch( EditMode )
                //{
                //    case NodeEditMode.AddInPopup:

                //        CswNbtMetaDataNodeType NodeType = null;
                //        if( NodeTypeId != Int32.MinValue )
                //        {
                //            NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                //        }
                //        else if( Node != null )
                //        {
                //            NodeType = Node.NodeType;
                //        }

                //        if( NodeType != null && _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Create, NodeType ) )
                //        {
                //            _makeTab( Ret, "0", "newtab", "Add New " + NodeType.NodeTypeName, false );
                //        }
                //        break;

                //    case NodeEditMode.Preview:
                //        if( Node != null )
                //        {
                //            _makeTab( Ret, "0", "previewtab", Node.NodeType.NodeTypeName, false );
                //        }
                //        break;

                //    default:
                if( Node != null )
                {
                    foreach( CswNbtMetaDataNodeTypeTab Tab in Node.NodeType.NodeTypeTabs
                                                                .Cast<CswNbtMetaDataNodeTypeTab>()
                                                                .Where( Tab => _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.View, Node.NodeType, false, Tab ) ) )
                    {
                        _makeTab( Ret, Tab.TabOrder.ToString(), Tab.TabId.ToString(), Tab.TabName, _canEditLayout() );
                    }

                    // History tab
                    if( false == CswConvert.ToBoolean( _IsMultiEdit ) &&
                        Date.IsNull &&
                                CswConvert.ToBoolean( _CswNbtResources.ConfigVbls.getConfigVariableValue( "auditing" ) ) )
                    {
                        if( _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.View, Node.NodeType ) )
                        {
                            _makeTab( Ret, Int32.MaxValue.ToString(), HistoryTabPrefix + NodeId, "History", false );
                        }
                    }

                } // if( Node != null )
                //        break;
                //} // switch(EditMode)

            } // if-else( filterToPropId != string.Empty )
            return Ret;
        } // getTabs()

        public void _makeTab( JObject ParentObj, string PropertyName, string Id, string Name, bool CanEditLayout )
        {
            ParentObj[PropertyName] = new JObject();
            ParentObj[PropertyName]["id"] = Id;
            ParentObj[PropertyName]["name"] = Name;
            ParentObj[PropertyName]["canEditLayout"] = CanEditLayout;
        }


        /// <summary>
        /// Returns JObject for all properties in a given tab
        /// </summary>
        public JObject getProps( NodeEditMode EditMode, string NodeId, string NodeKey, string TabId, Int32 NodeTypeId, CswDateTime Date, string filterToPropId )
        {
            JObject Ret = new JObject();

            CswPropIdAttr FilterPropIdAttr = null;
            if( filterToPropId != string.Empty )
            {
                FilterPropIdAttr = new CswPropIdAttr( filterToPropId );
            }

            if( false == _IsMultiEdit && TabId.StartsWith( HistoryTabPrefix ) )
            {
                CswNbtNode Node = wsTools.getNode( _CswNbtResources, NodeId, NodeKey, Date );
                if( _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.View, Node.NodeType ) )
                {
                    _getAuditHistoryGridProp( Ret, Node );
                }
            }
            else
            {
                CswNbtMetaDataNodeTypeLayoutMgr.LayoutType LayoutType = _CswNbtResources.MetaData.NodeTypeLayout.LayoutTypeForEditMode( EditMode );

                CswNbtNode Node = null;
                if( EditMode == NodeEditMode.AddInPopup && NodeTypeId != Int32.MinValue )
                {
                    Node = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
                }
                else
                {
                    Node = wsTools.getNode( _CswNbtResources, NodeId, NodeKey, Date );
                }

                if( Node != null )
                {
					//CswNbtMetaDataNodeTypeTab Tab = null;
					//if( TabId != string.Empty )
					//{
					//    Tab = Node.NodeType.getNodeTypeTab( CswConvert.ToInt32( TabId ) );
					//}
					Collection<CswNbtMetaDataNodeTypeProp> Props = _CswNbtResources.MetaData.NodeTypeLayout.getPropsInLayout( Node.NodeType, CswConvert.ToInt32( TabId ), LayoutType );

                    bool CanCreate = _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Create, Node.NodeType );

                    foreach( CswNbtMetaDataNodeTypeProp Prop in Props )
                    {
                        if( ( ( EditMode == NodeEditMode.AddInPopup && CanCreate && Prop.EditProp( Node, _ThisUser, true ) ) ||
                              ( EditMode != NodeEditMode.AddInPopup && Prop.ShowProp( Node, _ThisUser ) ) ) &&
                            ( FilterPropIdAttr == null || Prop.PropId == FilterPropIdAttr.NodeTypePropId ) )
                        {
                            _addProp( Ret, EditMode, Node, Prop );
                        }
                    }
                } // if(Node != null)
            } // if-else( TabId.StartsWith( HistoryTabPrefix ) )
            return Ret;
        } // getProps()

        /// <summary>
        /// Returns XML for a single property and its conditional properties
        /// </summary>
        public JObject getSingleProp( NodeEditMode EditMode, string NodeId, string NodeKey, string PropIdFromJson, Int32 NodeTypeId, string NewPropJson )
        {
            JObject Ret = new JObject();
            JObject PropObj = new JObject();
            CswNbtNode Node = null;
            Node = EditMode == NodeEditMode.AddInPopup && NodeTypeId != Int32.MinValue ?
                                    _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing ) :
                                    wsTools.getNode( _CswNbtResources, NodeId, NodeKey, new CswDateTime( _CswNbtResources ) );

            if( Node != null )
            {
                // removed for case 21695
                //// case 21209
                //if( Node.NodeSpecies == NodeSpecies.Plain )
                //{
                //    CswNbtActUpdatePropertyValue PropUpdater = new Actions.CswNbtActUpdatePropertyValue( _CswNbtResources );
                //    PropUpdater.UpdateNode( Node, true );
                //    Node.postChanges( false );
                //}

                if( false == string.IsNullOrEmpty( NewPropJson ) )
                {
                    // for prop filters, update node prop value but don't save the change
                    JObject PropJson = JObject.Parse( NewPropJson );
                    _applyPropJson( Node, PropJson, EditMode, null );
                }

                CswPropIdAttr PropIdAttr = new CswPropIdAttr( PropIdFromJson );
                CswNbtMetaDataNodeTypeProp Prop = Node.NodeType.getNodeTypeProp( PropIdAttr.NodeTypePropId );
                _addProp( PropObj, EditMode, Node, Prop );

                if( false == string.IsNullOrEmpty( NewPropJson ) )
                {
                    //Node.Rollback();
                }
            }
            if( PropObj.HasValues )
            {
                Ret = (JObject) PropObj.Properties().First().Value;
            }

            return Ret;
        } // getProp()

        private void _addProp( JObject ParentObj, NodeEditMode EditMode, CswNbtNode Node, CswNbtMetaDataNodeTypeProp Prop )
        {
            if( EditMode == NodeEditMode.AddInPopup )
            {
                _makePropJson( EditMode, ParentObj, Node, Prop, Prop.AddLayout.DisplayRow, Prop.AddLayout.DisplayColumn );
            }
            else
            {
                CswNbtMetaDataNodeTypeLayoutMgr.LayoutType LayoutType = _CswNbtResources.MetaData.NodeTypeLayout.LayoutTypeForEditMode( EditMode );
                CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout Layout = Prop.getLayout( LayoutType );

                JObject PropObj = _makePropJson( EditMode, ParentObj, Node, Prop, Layout.DisplayRow, Layout.DisplayColumn );

                // Handle conditional properties
                JObject SubPropsObj = new JObject();
                JProperty SubPropsJProp = new JProperty( "subprops", SubPropsObj );
                PropObj.Add( SubPropsJProp );
                bool HasSubProps = false;
                foreach( CswNbtMetaDataNodeTypeProp FilterProp in _CswNbtResources.MetaData.NodeTypeLayout.getPropsInLayout( Prop.NodeType, Layout.TabId, LayoutType ) )
                {
                    if( FilterProp.FilterNodeTypePropId == Prop.FirstPropVersionId )
                    {
                        HasSubProps = true;
                        CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout FilterPropLayout = _CswNbtResources.MetaData.NodeTypeLayout.getLayout( LayoutType, FilterProp );
                        JObject FilterPropXml = _makePropJson( EditMode, SubPropsObj, Node, FilterProp, FilterPropLayout.DisplayRow, FilterPropLayout.DisplayColumn );
                        // Hide those for whom the filter doesn't match
                        // (but we need the XML node to be there to store the value, for client-side changes)
                        FilterPropXml["display"] = FilterProp.CheckFilter( Node ).ToString().ToLower();

                    } // if( FilterProp.FilterNodeTypePropId == Prop.FirstPropVersionId )
                } // foreach( CswNbtMetaDataNodeTypeProp FilterProp in Tab.NodeTypePropsByDisplayOrder )
                PropObj["hassubprops"] = HasSubProps;

            } // if-else( EditMode == NodeEditMode.AddInPopup )
        } // addProp()


        private JObject _makePropJson( NodeEditMode EditMode, JObject ParentObj, CswNbtNode Node, CswNbtMetaDataNodeTypeProp Prop, Int32 Row, Int32 Column )
        {
            CswNbtNodePropWrapper PropWrapper = Node.Properties[Prop];

            CswPropIdAttr PropIdAttr = null;
            PropIdAttr = Node.NodeId != null ? new CswPropIdAttr( Node, Prop ) : new CswPropIdAttr( null, Prop );

            JObject PropObj = new JObject();
            ParentObj["prop_" + PropIdAttr] = PropObj;

            PropObj["id"] = PropIdAttr.ToString();
            PropObj["name"] = Prop.PropNameWithQuestionNo;
            PropObj["helptext"] = PropWrapper.HelpText;
            PropObj["fieldtype"] = Prop.FieldType.FieldType.ToString();
            if( Prop.ObjectClassProp != null )
            {
                PropObj["ocpname"] = Prop.ObjectClassProp.PropName;
            }
            PropObj["displayrow"] = Row.ToString();
            PropObj["displaycol"] = Column.ToString();
            PropObj["required"] = Prop.IsRequired.ToString().ToLower();

            CswNbtMetaDataNodeTypeTab Tab = null;
            if( ( EditMode == NodeEditMode.Edit || EditMode == NodeEditMode.EditInPopup ) && Prop.EditLayout != null )
            {
				Tab = _CswNbtResources.MetaData.getNodeTypeTab( Prop.EditLayout.TabId );
            }
            PropObj["gestalt"] = PropWrapper.Gestalt.Replace( "\"", "&quot;" );
            PropObj["copyable"] = Prop.IsCopyable().ToString().ToLower();
            PropObj["highlight"] = PropWrapper.AuditChanged.ToString().ToLower();

            PropWrapper.ToJSON( PropObj, EditMode, Tab );

            return PropObj;
        } // _makePropJson()


        public void _getAuditHistoryGridProp( JObject ParentObj, CswNbtNode Node )
        {
            if( Node != null )
            {
                JObject PropObj = new JObject();
                //Random Num = new Random();
                //ParentObj["prop" + Num.Next()] = PropObj;
                string FakePropIdAttr = Node.NodeId.ToString() + "_audit";
                ParentObj["prop_" + FakePropIdAttr] = PropObj;
                PropObj["name"] = "Audit History";
                PropObj["helptext"] = string.Empty;
                PropObj["fieldtype"] = "AuditHistoryGrid";
                PropObj["displayrow"] = "1";
                PropObj["displaycol"] = "1";
                PropObj["required"] = "false";
                PropObj["readonly"] = "true";
                PropObj["id"] = FakePropIdAttr;
            }
            //CswNbtWebServiceAuditing wsAuditing = new CswNbtWebServiceAuditing(_CswNbtResources);
            //PropXmlNode.InnerText = wsAuditing.getAuditHistoryGrid( Node ).ToString();

        } // _getAuditHistoryGridProp()

        public bool moveProp( string PropIdAttr, Int32 NewRow, Int32 NewColumn, NodeEditMode EditMode )
        {
            bool ret = false;
            if( _canEditLayout() )
            {
                CswPropIdAttr PropId = new CswPropIdAttr( PropIdAttr );
                Int32 NodeTypePropId = PropId.NodeTypePropId;
                if( NodeTypePropId != Int32.MinValue && NewRow > 0 && NewColumn > 0 )
                {
                    CswNbtMetaDataNodeTypeProp Prop = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );
                    Prop.updateLayout( _CswNbtResources.MetaData.NodeTypeLayout.LayoutTypeForEditMode( EditMode ), Int32.MinValue, NewRow, NewColumn );
                    ret = true;
                }
            } // if( _CswNbtResources.Permit.can( CswNbtActionName.Design ) || _CswNbtResources.CurrentNbtUser.IsAdministrator() )
            else
            {
                throw new CswDniException( ErrorType.Warning, "You do not have permission to configure layout", _CswNbtResources.CurrentNbtUser.Username + " tried to change property layout without administrative or Design privileges" );
            }
            return ret;
        } // moveProp()

        public bool removeProp( string PropIdAttr, NodeEditMode EditMode )
        {
            bool ret = false;
            if( _canEditLayout() )
            {
                CswPropIdAttr PropId = new CswPropIdAttr( PropIdAttr );
                Int32 NodeTypePropId = PropId.NodeTypePropId;
                if( NodeTypePropId != Int32.MinValue )
                {
                    CswNbtMetaDataNodeTypeProp Prop = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );
                    if( EditMode == NodeEditMode.AddInPopup && Prop.IsRequired && false == Prop.HasDefaultValue() )
                    {
                        throw new CswDniException( ErrorType.Warning, Prop.PropName + " may not be removed", Prop.PropName + " is required and has no unique value, and therefore cannot be removed from 'Add' layouts" );
                    }
                    Prop.removeFromLayout( _CswNbtResources.MetaData.NodeTypeLayout.LayoutTypeForEditMode( EditMode ) );
                    ret = true;
                }
            } // if( _CswNbtResources.Permit.can( CswNbtActionName.Design ) || _CswNbtResources.CurrentNbtUser.IsAdministrator() )
            else
            {
                throw new CswDniException( ErrorType.Warning, "You do not have permission to configure layout", _CswNbtResources.CurrentNbtUser.Username + " tried to change property layout without administrative or Design privileges" );
            }
            return ret;
        } // removeProp()

        public JObject saveProps( NodeEditMode EditMode, CswCommaDelimitedString NodeIds, CswCommaDelimitedString NodeKeys, Int32 TabId, string NewPropsJson, Int32 NodeTypeId, CswNbtView View )
        {
            JObject ret = new JObject();
            JObject PropsObj = JObject.Parse( NewPropsJson );
            CswNbtNodeKey RetNbtNodeKey = null;
            bool AllSucceeded = false;
            Int32 Succeeded = 0;
            CswNbtNode Node = null;
            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
            CswNbtMetaDataNodeTypeTab NodeTypeTab = _CswNbtResources.MetaData.getNodeTypeTab( TabId );
            if( null == NodeType && null != NodeTypeTab )
            {
                NodeType = NodeTypeTab.NodeType;
            }
            if( null != NodeType )
            {
                switch( EditMode )
                {
                    case NodeEditMode.AddInPopup:
                        CswNbtWebServiceQuotas wsQ = new CswNbtWebServiceQuotas( _CswNbtResources );
                        if( wsQ.CheckQuota( NodeTypeId ) )
                        {
                            Node = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
                            bool CanEdit = _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Edit, NodeType, false, NodeTypeTab, null, Node );
                            if( CanEdit )
                            {
                                RetNbtNodeKey = _saveProp( Node, PropsObj, View, EditMode, NodeTypeTab, true );
                                if( null != RetNbtNodeKey )
                                {
                                    AllSucceeded = true;
                                }
                            }
                        }
                        else
                        {
                            throw new CswDniException( ErrorType.Warning, "Quota Exceeded", "You have used all of your purchased quota, and must purchase additional quota space in order to add" );
                        }
                        break;
                    default:
                        for( Int32 i = 0; i < NodeIds.Count; i++ )
                        {
                            string NodeId = NodeIds[i];
                            string NodeKey = NodeKeys[i];
                            Node = wsTools.getNode( _CswNbtResources, NodeId, NodeKey, new CswDateTime( _CswNbtResources ) );
                            bool CanEdit = _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Edit, NodeType, false, NodeTypeTab, null, Node );
                            if( CanEdit )
                            {
								if( Node.PendingUpdate )
								{
									CswNbtActUpdatePropertyValue Act = new CswNbtActUpdatePropertyValue( _CswNbtResources );
									Act.UpdateNode( Node, false );
								}
                                RetNbtNodeKey = _saveProp( Node, PropsObj, View, EditMode, NodeTypeTab );
                                if( null != RetNbtNodeKey )
                                {
                                    Succeeded++;
                                }
                            }
                        }
                        AllSucceeded = NodeIds.Count == Succeeded;
                        break;
                } //switch( EditMode )
                if( AllSucceeded && null != RetNbtNodeKey )
                {
                    string RetNodeKey = wsTools.ToSafeJavaScriptParam( RetNbtNodeKey );
                    //string RetNodeId = RetNbtNodeKey.NodeId.PrimaryKey.ToString();
                    string RetNodeId = RetNbtNodeKey.NodeId.ToString();

                    ret = new JObject();
                    ret["result"] = "Succeeded";
                    ret["nodeid"] = RetNodeId;
                    ret["cswnbtnodekey"] = RetNodeKey;
                } //if( AllSucceeded && null != RetNbtNodeKey )
                else
                {
                    string ErrString;
                    if( EditMode == NodeEditMode.AddInPopup )
                    {
                        ErrString = "Attempt to Add failed.";
                    }
                    else
                    {
                        ErrString = Succeeded + " out of " + NodeIds.Count + " prop updates succeeded. Remaining prop updates failed";
                    }
                    ret = new JObject();
                    ret["result"] = ErrString;
                } //else
            } //if( null != NodeType && null != NodeTypeTab )
            return ret;
        } // saveProps()

        private CswNbtNodeKey _saveProp( CswNbtNode Node, JObject PropsObj, CswNbtView View, NodeEditMode EditMode, CswNbtMetaDataNodeTypeTab Tab, bool ForceUpdate = false )
        {
            CswNbtNodeKey Ret = null;
            if( Node != null )
            {
                foreach( JObject PropObj in
                    from PropJProp
                        in PropsObj.Properties()
                    where null != PropJProp.Value
                    select (JObject) PropJProp.Value
                        into PropObj
                        where PropObj.HasValues
                        select PropObj )
                {
                    _applyPropJson( Node, PropObj, EditMode, Tab );
                }

                // BZ 8517 - this sets sequences that have setvalonadd = 0
                _CswNbtResources.CswNbtNodeFactory.CswNbtNodeWriter.setSequenceValues( Node );

                Node.postChanges( ForceUpdate );

                ICswNbtTree Tree;
                if( View != null )
                {
                    // Get the nodekey of this node in the current view
                    Tree = _CswNbtResources.Trees.getTreeFromView( View, true, true, false, false );
                    Ret = Tree.getNodeKeyByNodeId( Node.NodeId );
                }
                if( Ret == null )
                {
                    // Make a nodekey from the default view
                    View = Node.NodeType.CreateDefaultView();
                    View.Root.ChildRelationships[0].NodeIdsToFilterIn.Add( Node.NodeId );
                    Tree = _CswNbtResources.Trees.getTreeFromView( View, true, true, false, false );
                    Ret = Tree.getNodeKeyByNodeId( Node.NodeId );
                }
            }
            return Ret;
        }

        public bool copyPropValues( string SourceNodeKeyStr, string[] CopyNodeIds, string[] PropIds )
        {
            bool ret = true;
            CswNbtNodeKey SourceNodeKey = new CswNbtNodeKey( _CswNbtResources, SourceNodeKeyStr );
            if( Int32.MinValue != SourceNodeKey.NodeId.PrimaryKey )
            {
                CswNbtNode SourceNode = _CswNbtResources.Nodes[SourceNodeKey];
                if( SourceNode != null )
                {
                    foreach( string NodeIdStr in CopyNodeIds )
                    {
                        CswPrimaryKey CopyToNodePk = new CswPrimaryKey();
                        CopyToNodePk.FromString( NodeIdStr );
                        if( Int32.MinValue != CopyToNodePk.PrimaryKey )
                        {
                            CswNbtNode CopyToNode = _CswNbtResources.Nodes[CopyToNodePk];
                            if( CopyToNode != null &&
                                _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Edit, CopyToNode.NodeType, false, null, null, CopyToNode, null ) )
                            {
                                foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in PropIds.Select( PropIdAttr => new CswPropIdAttr( PropIdAttr ) )
                                    .Select( PropId => _CswNbtResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId ) ) )
                                {
                                    CopyToNode.Properties[NodeTypeProp].copy( SourceNode.Properties[NodeTypeProp] );
                                }

                                CopyToNode.postChanges( false );
                            } // if( CopyToNode != null )
                        }
                        else
                        {
                            ret = false;
                        }
                    } // foreach( string NodeIdStr in CopyNodeIds )
                } // if(SourceNode != null)
            } // if( Int32.MinValue != SourceNodeKey.NodeId.PrimaryKey )
            return ret;
        } // copyPropValues()

        public JObject getPropertiesForLayoutAdd( string NodeId, string NodeKey, string NodeTypeId, string TabId, CswNbtMetaDataNodeTypeLayoutMgr.LayoutType LayoutType )
        {
            JObject ret = new JObject();

            CswNbtMetaDataNodeType NodeType = null;
            if( NodeTypeId != string.Empty )
            {
                NodeType = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( NodeTypeId ) );
            }
            else
            {
                CswNbtNode Node = wsTools.getNode( _CswNbtResources, NodeId, NodeKey, new CswDateTime( _CswNbtResources ) );
                NodeType = Node.NodeType;
            }

            if( NodeType != null )
            {
				//CswNbtMetaDataNodeTypeTab Tab = null;
				//if( TabId != string.Empty && LayoutType == CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit )
				//{
				//    Tab = NodeType.getNodeTypeTab( CswConvert.ToInt32( TabId ) );
				//}

				Collection<CswNbtMetaDataNodeTypeProp> Props = _CswNbtResources.MetaData.NodeTypeLayout.getPropsNotInLayout( NodeType, CswConvert.ToInt32( TabId ), LayoutType );
                foreach( CswNbtMetaDataNodeTypeProp Prop in Props )
                {
					// case 24179
					if( LayoutType != CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview || 
						Prop.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Grid )
					{
						ret["prop_" + Prop.PropId.ToString()] = new JObject();
						ret["prop_" + Prop.PropId.ToString()]["propid"] = Prop.PropId.ToString();
						ret["prop_" + Prop.PropId.ToString()]["propname"] = Prop.PropNameWithQuestionNo.ToString();
					}
                }
            } // if( NodeType != null )
            return ret;
        } // getPropertiesForLayoutAdd()


        public bool addPropertyToLayout( string PropId, string TabId, CswNbtMetaDataNodeTypeLayoutMgr.LayoutType LayoutType )
        {
            CswNbtMetaDataNodeTypeProp Prop = _CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( PropId ) );
            //CswNbtMetaDataNodeTypeTab Tab = _CswNbtResources.MetaData.getNodeTypeTab( CswConvert.ToInt32( TabId ) );
			_CswNbtResources.MetaData.NodeTypeLayout.updatePropLayout( LayoutType, Prop.NodeType.NodeTypeId, Prop.PropId, CswConvert.ToInt32( TabId ), Int32.MinValue, Int32.MinValue );
            return true;
        } // addPropertyToLayout()

        private void _applyPropJson( CswNbtNode Node, JObject PropObj, NodeEditMode EditMode, CswNbtMetaDataNodeTypeTab Tab )
        {
            CswPropIdAttr PropIdAttr = new CswPropIdAttr( CswConvert.ToString( PropObj["id"] ) );

            CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( PropIdAttr.NodeTypePropId );
            Node.Properties[MetaDataProp].ReadJSON( PropObj, null, null, EditMode, Tab );

            // Recurse on sub-props
            if( null != PropObj["subprops"] )
            {
                JObject SubPropsObj = (JObject) PropObj["subprops"];
                if( SubPropsObj.HasValues )
                {
                    foreach( JObject ChildPropObj in SubPropsObj.Properties()
                                .Where( ChildProp => null != ChildProp.Value && ChildProp.Value.HasValues )
                                .Select( ChildProp => (JObject) ChildProp.Value )
                                .Where( ChildPropObj => ChildPropObj.HasValues ) )
                    {
                        _applyPropJson( Node, ChildPropObj, EditMode, Tab );
                    }
                }
            }

        } // _applyPropJson

        public bool ClearPropValue( string PropIdAttr, bool IncludeBlob )
        {
            bool ret = false;
            CswPropIdAttr PropId = new CswPropIdAttr( PropIdAttr );
            CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId );
            if( Int32.MinValue != PropId.NodeId.PrimaryKey )
            {
                CswNbtNode Node = _CswNbtResources.Nodes[PropId.NodeId];
                CswNbtNodePropWrapper PropWrapper = Node.Properties[MetaDataProp];
                PropWrapper.ClearValue();
                if( IncludeBlob )
                {
                    PropWrapper.ClearBlob();
                }
                Node.postChanges( false );
                ret = true;
            }
            return ret;
        } // ClearPropValue()

        public bool saveMolProp( string moldata, string propIdAttr )
        {
            bool ret = false;

            CswPropIdAttr PropId = new CswPropIdAttr( propIdAttr );
            CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId );
            if( Int32.MinValue != PropId.NodeId.PrimaryKey )
            {
                CswNbtNode Node = _CswNbtResources.Nodes[PropId.NodeId];
                CswNbtNodePropWrapper PropWrapper = Node.Properties[MetaDataProp];

                // Do the update directly
                CswTableUpdate JctUpdate = _CswNbtResources.makeCswTableUpdate( "Clobber_save_update", "jct_nodes_props" );
                //JctUpdate.AllowBlobColumns = true;
                if( PropWrapper.JctNodePropId > 0 )
                {
                    DataTable JctTable = JctUpdate.getTable( "jctnodepropid", PropWrapper.JctNodePropId );
                    JctTable.Rows[0]["clobdata"] = moldata;
                    JctUpdate.update( JctTable );
                }
                else
                {
                    DataTable JctTable = JctUpdate.getEmptyTable();
                    DataRow JRow = JctTable.NewRow();
                    JRow["nodetypepropid"] = CswConvert.ToDbVal( PropId.NodeTypePropId );
                    JRow["nodeid"] = CswConvert.ToDbVal( Node.NodeId.PrimaryKey );
                    JRow["nodeidtablename"] = Node.NodeId.TableName;
                    JRow["clobdata"] = moldata;
                    JctTable.Rows.Add( JRow );
                    JctUpdate.update( JctTable );
                }
                ret = true;
            } // if( Int32.MinValue != NbtNodeKey.NodeId.PrimaryKey )
            return ret;
        }

        public bool SetPropBlobValue( byte[] Data, string FileName, string ContentType, string PropIdAttr, string Column )
        {
            bool ret = false;
            if( String.IsNullOrEmpty( Column ) ) Column = "blobdata";

            CswPropIdAttr PropId = new CswPropIdAttr( PropIdAttr );
            CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId );
            if( Int32.MinValue != PropId.NodeId.PrimaryKey )
            {
                CswNbtNode Node = _CswNbtResources.Nodes[PropId.NodeId];
                CswNbtNodePropWrapper PropWrapper = Node.Properties[MetaDataProp];

                // Do the update directly
                CswTableUpdate JctUpdate = _CswNbtResources.makeCswTableUpdate( "Blobber_save_update", "jct_nodes_props" );
                JctUpdate.AllowBlobColumns = true;
                if( PropWrapper.JctNodePropId > 0 )
                {
                    DataTable JctTable = JctUpdate.getTable( "jctnodepropid", PropWrapper.JctNodePropId );
                    if( JctTable.Columns[Column].DataType == typeof( string ) )
                    {
                        JctTable.Rows[0][Column] = CswTools.ByteArrayToString( Data );
                    }
                    else
                    {
                        JctTable.Rows[0][Column] = Data;
                    }
                    JctTable.Rows[0]["field1"] = FileName;
                    JctTable.Rows[0]["field2"] = ContentType;
                    JctUpdate.update( JctTable );
                }
                else
                {
                    DataTable JctTable = JctUpdate.getEmptyTable();
                    DataRow JRow = JctTable.NewRow();
                    JRow["nodetypepropid"] = CswConvert.ToDbVal( PropId.NodeTypePropId );
                    JRow["nodeid"] = CswConvert.ToDbVal( Node.NodeId.PrimaryKey );
                    JRow["nodeidtablename"] = Node.NodeId.TableName;
                    JRow[Column] = Data;
                    JRow["field1"] = FileName;
                    JRow["field2"] = ContentType;
                    JctTable.Rows.Add( JRow );
                    JctUpdate.update( JctTable );
                }
                ret = true;
            } // if( Int32.MinValue != NbtNodeKey.NodeId.PrimaryKey )
            return ret;
        } // SetPropBlobValue()

        private bool _canEditLayout()
        {
            return ( _CswNbtResources.Permit.can( CswNbtActionName.Design ) || _CswNbtResources.CurrentNbtUser.IsAdministrator() );
        }

    } // class CswNbtWebServiceTabsAndProps

} // namespace ChemSW.Nbt.WebServices
