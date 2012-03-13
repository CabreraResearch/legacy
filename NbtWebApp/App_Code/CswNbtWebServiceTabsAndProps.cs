using System;
using System.Collections.Generic;
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
            TabOrderModifier = 0;

            CswNbtNode Node = wsTools.getNode( _CswNbtResources, NodeId, NodeKey, Date );
            if( filterToPropId != string.Empty )
            {
                CswPropIdAttr PropId = new CswPropIdAttr( filterToPropId );
                CswNbtMetaDataNodeTypeProp Prop = _CswNbtResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId );
                CswNbtMetaDataNodeTypeTab Tab = _CswNbtResources.MetaData.getNodeTypeTab( Prop.EditLayout.TabId );
                if( _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.View, Prop.getNodeType(), false, Tab, _CswNbtResources.CurrentNbtUser, Node, Prop ) )
                {
                    _makeTab( Ret, Tab.TabOrder, Tab.TabId.ToString(), Tab.TabName, false );
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
                    foreach( CswNbtMetaDataNodeTypeTab Tab in _CswNbtResources.MetaData.getNodeTypeTabs( Node.NodeTypeId )
                                                                .Cast<CswNbtMetaDataNodeTypeTab>()
                                                                .Where( Tab => _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.View, Node.getNodeType(), false, Tab ) ) )
                    {
                        _makeTab( Ret, Tab.TabOrder, Tab.TabId.ToString(), Tab.TabName, _canEditLayout() );
                    }

                    // History tab
                    if( false == CswConvert.ToBoolean( _IsMultiEdit ) &&
                        Date.IsNull &&
                        CswConvert.ToBoolean( _CswNbtResources.ConfigVbls.getConfigVariableValue( "auditing" ) ) )
                    {
                        if( _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.View, Node.getNodeType() ) )
                        {
                            _makeTab( Ret, Int32.MaxValue, HistoryTabPrefix + NodeId, "History", false );
                        }
                    }
                    Ret["nodename"] = Node.NodeName;

                } // if( Node != null )
                //        break;
                //} // switch(EditMode)

            } // if-else( filterToPropId != string.Empty )
            return Ret;
        } // getTabs()

        private Int32 TabOrderModifier = 0;
        public void _makeTab( JObject ParentObj, Int32 TabOrder, string Id, string Name, bool CanEditLayout )
        {
            // case 24250
            // This mechanism correctly orders all tabs even with redundant tab order values,
            // as long as the tabs are added in order
            while( ParentObj[( TabOrder + TabOrderModifier ).ToString()] != null )
            {
                TabOrderModifier++;
            }
            string RealTabOrder = ( TabOrder + TabOrderModifier ).ToString();

            ParentObj[RealTabOrder] = new JObject();
            ParentObj[RealTabOrder]["id"] = Id;
            ParentObj[RealTabOrder]["name"] = Name;
            ParentObj[RealTabOrder]["canEditLayout"] = CanEditLayout;
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
                if( _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.View, Node.getNodeType() ) )
                {
                    _getAuditHistoryGridProp( Ret, Node );
                }
            }
            else
            {
                CswNbtMetaDataNodeTypeLayoutMgr.LayoutType LayoutType = _CswNbtResources.MetaData.NodeTypeLayout.LayoutTypeForEditMode( EditMode );

                CswNbtNode Node = null;
                if( EditMode == NodeEditMode.Add && NodeTypeId != Int32.MinValue )
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
                    IEnumerable<CswNbtMetaDataNodeTypeProp> Props = _CswNbtResources.MetaData.NodeTypeLayout.getPropsInLayout( Node.NodeTypeId, CswConvert.ToInt32( TabId ), LayoutType );



                    foreach( CswNbtMetaDataNodeTypeProp Prop in Props )
                    {
                        if( _showProp( Prop, EditMode, FilterPropIdAttr, Node ) )
                        {
                            _addProp( Ret, EditMode, Node, Prop );
                        }
                    }
                } // if(Node != null)
            } // if-else( TabId.StartsWith( HistoryTabPrefix ) )
            return Ret;
        } // getProps()

        private bool _showProp( CswNbtMetaDataNodeTypeProp Prop, NodeEditMode EditMode, CswPropIdAttr FilterPropIdAttr, CswNbtNode Node )
        {
            bool RetShow = false;

            switch( EditMode )
            {
                case NodeEditMode.Add:
                    //Case 24023: Exclude buttons on Add
                    bool CanCreate = _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Create, Node.getNodeType() );
                    RetShow = ( CanCreate &&
                                Prop.EditProp( Node, _ThisUser, true ) &&
                                Prop.getFieldType().FieldType != CswNbtMetaDataFieldType.NbtFieldType.Button );
                    break;
                default:
                    RetShow = Prop.ShowProp( Node, _ThisUser );
                    break;
            }
            RetShow = RetShow && ( FilterPropIdAttr == null || Prop.PropId == FilterPropIdAttr.NodeTypePropId );
            return RetShow;
        }

        /// <summary>
        /// Returns XML for a single property and its conditional properties
        /// </summary>
        public JObject getSingleProp( NodeEditMode EditMode, string NodeId, string NodeKey, string PropIdFromJson, Int32 NodeTypeId, string NewPropJson )
        {
            JObject Ret = new JObject();
            JObject PropObj = new JObject();
            CswNbtNode Node = null;
            Node = EditMode == NodeEditMode.Add && NodeTypeId != Int32.MinValue ?
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
                CswNbtMetaDataNodeTypeProp Prop = _CswNbtResources.MetaData.getNodeTypeProp( PropIdAttr.NodeTypePropId );
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
            if( EditMode == NodeEditMode.Add )
            {
                ParentObj.Add( makePropJson( EditMode, Node.NodeId, Prop, Node.Properties[Prop], Prop.AddLayout.DisplayRow, Prop.AddLayout.DisplayColumn ) );
            }
            else
            {
                CswNbtMetaDataNodeTypeLayoutMgr.LayoutType LayoutType = _CswNbtResources.MetaData.NodeTypeLayout.LayoutTypeForEditMode( EditMode );
                CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout Layout = Prop.getLayout( LayoutType );

                JProperty JpProp = makePropJson( EditMode, Node.NodeId, Prop, Node.Properties[Prop], Layout.DisplayRow, Layout.DisplayColumn );
                ParentObj.Add( JpProp );
                JObject PropObj = (JObject) JpProp.Value;

                // Handle conditional properties
                JObject SubPropsObj = new JObject();
                JProperty SubPropsJProp = new JProperty( "subprops", SubPropsObj );
                PropObj.Add( SubPropsJProp );
                bool HasSubProps = false;
                foreach( CswNbtMetaDataNodeTypeProp FilterProp in _CswNbtResources.MetaData.NodeTypeLayout.getPropsInLayout( Prop.NodeTypeId, Layout.TabId, LayoutType ) )
                {
                    if( FilterProp.FilterNodeTypePropId == Prop.FirstPropVersionId )
                    {
                        HasSubProps = true;
                        CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout FilterPropLayout = _CswNbtResources.MetaData.NodeTypeLayout.getLayout( LayoutType, FilterProp );
                        JProperty JPFilterProp = makePropJson( EditMode, Node.NodeId, FilterProp, Node.Properties[FilterProp], FilterPropLayout.DisplayRow, FilterPropLayout.DisplayColumn );
                        SubPropsObj.Add( JPFilterProp );
                        JObject FilterPropXml = (JObject) JPFilterProp.Value;

                        // Hide those for whom the filter doesn't match
                        // (but we need the XML node to be there to store the value, for client-side changes)
                        FilterPropXml["display"] = FilterProp.CheckFilter( Node ).ToString().ToLower();

                    } // if( FilterProp.FilterNodeTypePropId == Prop.FirstPropVersionId )
                } // foreach( CswNbtMetaDataNodeTypeProp FilterProp in Tab.NodeTypePropsByDisplayOrder )
                PropObj["hassubprops"] = HasSubProps;

            } // if-else( EditMode == NodeEditMode.Add )
        } // addProp()


        public JProperty makePropJson( NodeEditMode EditMode, CswPrimaryKey NodeId, CswNbtMetaDataNodeTypeProp Prop, CswNbtNodePropWrapper PropWrapper, Int32 Row, Int32 Column )
        {
            CswPropIdAttr PropIdAttr = null;
            PropIdAttr = new CswPropIdAttr( NodeId, Prop.PropId );

            JObject PropObj = new JObject();
            //ParentObj["prop_" + PropIdAttr] = PropObj;
            JProperty ret = new JProperty( "prop_" + PropIdAttr, PropObj );
            CswNbtMetaDataFieldType.NbtFieldType FieldType = Prop.getFieldType().FieldType;
            PropObj["id"] = PropIdAttr.ToString();
            PropObj["name"] = Prop.PropNameWithQuestionNo;
            PropObj["helptext"] = Prop.HelpText;
            PropObj["fieldtype"] = FieldType.ToString();
            CswNbtMetaDataObjectClassProp OCP = Prop.getObjectClassProp();
            if( OCP != null )
            {
                PropObj["ocpname"] = OCP.PropName;
            }
            PropObj["displayrow"] = Row.ToString();
            PropObj["displaycol"] = Column.ToString();
            PropObj["required"] = Prop.IsRequired.ToString().ToLower();
            PropObj["copyable"] = Prop.IsCopyable().ToString().ToLower();

            bool ShowPropertyName = false == ( FieldType == CswNbtMetaDataFieldType.NbtFieldType.Image ||
                                       FieldType == CswNbtMetaDataFieldType.NbtFieldType.Button ||
                                       ( FieldType == CswNbtMetaDataFieldType.NbtFieldType.Grid && PropWrapper.AsGrid.GridMode == CswNbtNodePropGrid.GridPropMode.Full ) );

            PropObj["showpropertyname"] = ShowPropertyName;

            CswNbtMetaDataNodeTypeTab Tab = null;
            if( ( EditMode == NodeEditMode.Edit || EditMode == NodeEditMode.EditInPopup ) && Prop.EditLayout != null )
            {
                Tab = _CswNbtResources.MetaData.getNodeTypeTab( Prop.EditLayout.TabId );
            }

            if( PropWrapper != null )
            {
                PropObj["readonly"] = PropWrapper.IsReadOnly().ToString().ToLower();
                PropObj["gestalt"] = PropWrapper.Gestalt.Replace( "\"", "&quot;" );
                PropObj["highlight"] = PropWrapper.AuditChanged.ToString().ToLower();
                PropWrapper.ToJSON( PropObj, EditMode, Tab );
            }
            return ret;
        } // makePropJson()


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
                    if( EditMode == NodeEditMode.Add && Prop.IsRequired && false == Prop.HasDefaultValue() )
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

        public JObject saveProps( NodeEditMode EditMode, Collection<CswPrimaryKey> NodePks, Int32 TabId, string NewPropsJson, Int32 NodeTypeId, CswNbtView View )
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
                NodeType = NodeTypeTab.getNodeType();
            }
            if( null != NodeType )
            {
                switch( EditMode )
                {
                    case NodeEditMode.Add:
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
                        foreach( CswPrimaryKey NodePk in NodePks )
                        {
                            Node = _CswNbtResources.Nodes.GetNode( NodePk );
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
                                    Succeeded += 1;
                                }
                            }
                        }
                        AllSucceeded = ( NodePks.Count == Succeeded );
                        break;
                } //switch( EditMode )
                if( AllSucceeded && null != RetNbtNodeKey )
                {
                    string RetNodeKey = RetNbtNodeKey.ToString();
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
                    if( EditMode == NodeEditMode.Add )
                    {
                        ErrString = "Attempt to Add failed.";
                    }
                    else
                    {
                        ErrString = Succeeded + " out of " + NodePks.Count + " prop updates succeeded. Remaining prop updates failed";
                    }
                    ret = new JObject();
                    ret["result"] = ErrString;
                } //else
            } //if( null != NodeType && null != NodeTypeTab )

            // Good opportunity to force an update on the node
            if( Node != null )
            {
                CswNbtActUpdatePropertyValue ActUpdatePropVal = new CswNbtActUpdatePropertyValue( _CswNbtResources );
                ActUpdatePropVal.UpdateNode( Node, true );
                Node.postChanges( false );
            }

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
                    View = Node.getNodeType().CreateDefaultView();
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
                                _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Edit, CopyToNode.getNodeType(), false, null, null, CopyToNode, null ) )
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

        public JArray getPropertiesForLayoutAdd( string NodeId, string NodeKey, string NodeTypeId, string TabId, CswNbtMetaDataNodeTypeLayoutMgr.LayoutType LayoutType )
        {
            JArray ret = new JArray();

            CswNbtMetaDataNodeType NodeType = null;
            if( NodeTypeId != string.Empty )
            {
                NodeType = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( NodeTypeId ) );
            }
            else
            {
                CswNbtNode Node = wsTools.getNode( _CswNbtResources, NodeId, NodeKey, new CswDateTime( _CswNbtResources ) );
                NodeType = Node.getNodeType();
            }

            if( NodeType != null )
            {
                //CswNbtMetaDataNodeTypeTab Tab = null;
                //if( TabId != string.Empty && LayoutType == CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit )
                //{
                //    Tab = NodeType.getNodeTypeTab( CswConvert.ToInt32( TabId ) );
                //}

                IEnumerable<CswNbtMetaDataNodeTypeProp> Props = _CswNbtResources.MetaData.NodeTypeLayout.getPropsNotInLayout( NodeType, CswConvert.ToInt32( TabId ), LayoutType );
                foreach( CswNbtMetaDataNodeTypeProp Prop in from Prop in Props
                                                            orderby Prop.PropNameWithQuestionNo
                                                            select Prop )
                {
                    // case 24179
                    if( Prop.getFieldType().FieldType != CswNbtMetaDataFieldType.NbtFieldType.Grid ||
                        ( LayoutType != CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview &&
                          LayoutType != CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table ) )
                    {
                        JObject ThisPropObj = new JObject();
                        ThisPropObj["propid"] = Prop.PropId.ToString();
                        ThisPropObj["propname"] = Prop.PropNameWithQuestionNo.ToString();
                        ret.Add( ThisPropObj );
                    }
                }
            } // if( NodeType != null )
            return ret;
        } // getPropertiesForLayoutAdd()


        public bool addPropertyToLayout( string PropId, string TabId, CswNbtMetaDataNodeTypeLayoutMgr.LayoutType LayoutType )
        {
            CswNbtMetaDataNodeTypeProp Prop = _CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( PropId ) );
            //CswNbtMetaDataNodeTypeTab Tab = _CswNbtResources.MetaData.getNodeTypeTab( CswConvert.ToInt32( TabId ) );
            _CswNbtResources.MetaData.NodeTypeLayout.updatePropLayout( LayoutType, Prop.NodeTypeId, Prop.PropId, CswConvert.ToInt32( TabId ), Int32.MinValue, Int32.MinValue );
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


        /// <summary>
        /// Default content to display when no node is selected, or the tree is empty
        /// </summary>
        public JObject getDefaultContent( CswNbtView View )
        {
            JObject ret = new JObject();
            _getDefaultContentRecursive( ret, View.Root );
            return ret;
        }
        private void _getDefaultContentRecursive( JObject ParentObj, CswNbtViewNode ViewNode )
        {
            ParentObj["entries"] = new JObject();
            foreach( CswNbtViewNode.CswNbtViewAddNodeTypeEntry Entry in ViewNode.AllowedChildNodeTypes( true ) )
            {
                ParentObj["entries"][Entry.NodeType.NodeTypeName] = CswNbtWebServiceMainMenu.makeAddMenuItem( Entry, string.Empty, string.Empty );
            }

            JObject ChildObj = new JObject();
            ParentObj["children"] = ChildObj;

            // recurse
            foreach( CswNbtViewRelationship ChildViewRel in ViewNode.GetChildrenOfType( NbtViewNodeType.CswNbtViewRelationship ) )
            {
                _getDefaultContentRecursive( ChildObj, ChildViewRel );
            }
        } // _getDefaultContentRecursive()

    } // class CswNbtWebServiceTabsAndProps

} // namespace ChemSW.Nbt.WebServices
