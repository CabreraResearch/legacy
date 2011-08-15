﻿using System;
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
        public enum NodeEditMode { Edit, AddInPopup, EditInPopup, Demo, PrintReport, DefaultValue, AuditHistoryInPopup };

        private readonly CswNbtResources _CswNbtResources;
        private readonly ICswNbtUser _ThisUser;

        private string HistoryTabPrefix = "history_";

        public CswNbtWebServiceTabsAndProps( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _ThisUser = _CswNbtResources.CurrentNbtUser;
        }

        private CswNbtNode _getNode( string NodeId, string NodeKey, DateTime Date )
        {
            CswNbtNode Node = null;
            if( !string.IsNullOrEmpty( NodeKey ) )
            {
                CswNbtNodeKey RealNodeKey = new CswNbtNodeKey( _CswNbtResources, NodeKey );
                Node = _CswNbtResources.getNode( RealNodeKey, Date );
            }
            else if( !string.IsNullOrEmpty( NodeId ) )
            {
                CswPrimaryKey RealNodeId = new CswPrimaryKey();
                RealNodeId.FromString( NodeId );
                Node = _CswNbtResources.getNode( RealNodeId, Date );
            }
            return Node;
        } // _getNode()

        public JObject getTabs( NodeEditMode EditMode, string NodeId, string NodeKey, Int32 NodeTypeId, DateTime Date, string filterToPropId )
        {
            JObject Ret = new JObject();

            if( EditMode == NodeEditMode.AddInPopup && NodeTypeId != Int32.MinValue )
            {
                CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                if( _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Create, NodeType ) )
                {
                    Ret.Add( new JProperty( "newtab",
                                    new JObject(
                                            new JProperty( "id", "newtab" ),
                                            new JProperty( "name", "Add New " + NodeType.NodeTypeName ),
                                            new JProperty( "canEditLayout", "false" ) ) ) );
                }
            }
            else if( filterToPropId != string.Empty )
            {
                CswPropIdAttr PropId = new CswPropIdAttr( filterToPropId );
                CswNbtMetaDataNodeTypeProp Prop = _CswNbtResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId );
                if( _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.View, Prop.NodeType, false, Prop.NodeTypeTab ) )
                {
                    Ret.Add( new JProperty( Prop.NodeTypeTab.TabId.ToString(),
                                      new JObject(
                                            new JProperty( "id", Prop.NodeTypeTab.TabId.ToString() ),
                                            new JProperty( "name", Prop.NodeTypeTab.TabName ),
                                            new JProperty( "canEditLayout", "false" ) ) ) );
                }
            }
            else
            {
                CswNbtNode Node = _getNode( NodeId, NodeKey, Date );
                if( Node != null )
                {
                    foreach( CswNbtMetaDataNodeTypeTab Tab in Node.NodeType.NodeTypeTabs
                        .Cast<CswNbtMetaDataNodeTypeTab>()
                        .Where( Tab => _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.View, Node.NodeType, false, Tab ) ) )
                    {
                        Ret.Add( new JProperty( Tab.TabId.ToString(),
                                       new JObject(
                                               new JProperty( "id", Tab.TabId.ToString() ),
                                               new JProperty( "name", Tab.TabName ),
                                               new JProperty( "canEditLayout", _canEditLayout().ToString().ToLower() ) ) ) );
                    }

                    // History tab
                    if( Date == DateTime.MinValue &&
                        CswConvert.ToBoolean( _CswNbtResources.getConfigVariableValue( "auditing" ) ) )
                    {
                        if( _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.View, Node.NodeType ) )
                        {
                            Ret.Add( new JProperty( HistoryTabPrefix + NodeId,
                                         new JObject(
                                                new JProperty( "id", HistoryTabPrefix + NodeId ),
                                                new JProperty( "name", "History" ),
                                                new JProperty( "canEditLayout", "false" ) ) ) );
                        }
                    }
                } // if( Node != null )
            } // if-else( EditMode == NodeEditMode.AddInPopup )
            return Ret;
        } // getTabs()


        /// <summary>
        /// Returns XML for all properties in a given tab
        /// </summary>
        public JObject getProps( NodeEditMode EditMode, string NodeId, string NodeKey, string TabId, Int32 NodeTypeId, DateTime Date )
        {
            JObject Ret = new JObject();

            if( TabId.StartsWith( HistoryTabPrefix ) )
            {
                CswNbtNode Node = _getNode( NodeId, NodeKey, Date );
                if( _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.View, Node.NodeType ) )
                {
                    _getAuditHistoryGridProp( Ret, Node );
                }
            }
            else
            {
                if( EditMode == NodeEditMode.AddInPopup )
                {
                    CswNbtNode Node = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );

                    if( _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Create, Node.NodeType ) )
                    {
                        foreach( CswNbtMetaDataNodeTypeProp Prop in Node.NodeType.NodeTypeProps
                                                                             .Cast<CswNbtMetaDataNodeTypeProp>()
                                                                             .Where( Prop => Prop.EditProp( Node, _ThisUser, true ) ) )
                        {
                            _addProp( Ret, EditMode, Node, Prop );
                        }
                    }
                }
                else
                {

                    CswNbtNode Node = _getNode( NodeId, NodeKey, Date );
                    if( Node != null )
                    {
                        CswNbtMetaDataNodeTypeTab Tab = Node.NodeType.getNodeTypeTab( CswConvert.ToInt32( TabId ) );
                        foreach( CswNbtMetaDataNodeTypeProp Prop in Tab.NodeTypePropsByDisplayOrder
                                                                       .Cast<CswNbtMetaDataNodeTypeProp>()
                                                                       .Where( Prop => Prop.ShowProp( Node, _ThisUser ) ) )
                        {
                            _addProp( Ret, EditMode, Node, Prop );
                        }
                        //}
                    }
                    //}
                } // if-else( EditMode == NodeEditMode.AddInPopup )
            } // if( TabId.StartsWith( HistoryTabPrefix ) )
            return Ret;
        } // getProps()


        /// <summary>
        /// Returns XML for a single property and its conditional properties
        /// </summary>
        public JObject getSingleProp( NodeEditMode EditMode, string NodeId, string NodeKey, string PropIdFromJson, Int32 NodeTypeId, string NewPropJson )
        {
            JObject PropObj = new JObject();
            CswNbtNode Node = null;
            Node = EditMode == NodeEditMode.AddInPopup ?
                                    _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing ) :
                                    _getNode( NodeId, NodeKey, DateTime.MinValue );

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

                if( !string.IsNullOrEmpty( NewPropJson ) )
                {
                    // for prop filters, update node prop value but don't save the change
                    JObject PropJson = JObject.Parse( NewPropJson );
                    _applyPropJson( Node, PropJson );
                }

                CswPropIdAttr PropIdAttr = new CswPropIdAttr( PropIdFromJson );
                CswNbtMetaDataNodeTypeProp Prop = Node.NodeType.getNodeTypeProp( PropIdAttr.NodeTypePropId );
                _addProp( PropObj, EditMode, Node, Prop );

                if( NewPropJson != string.Empty )
                {
                    //Node.Rollback();
                }
            }

            return PropObj;
        } // getProp()

        private void _addProp( JObject ParentObj, NodeEditMode EditMode, CswNbtNode Node, CswNbtMetaDataNodeTypeProp Prop )
        {
            if( EditMode == NodeEditMode.AddInPopup )
            {
                _makePropJson( ParentObj, Node, Prop, Prop.DisplayRowAdd, Prop.DisplayColAdd );
            }
            else
            {
                JObject PropObj = _makePropJson( ParentObj, Node, Prop, Prop.DisplayRow, Prop.DisplayColumn );

                // Handle conditional properties
                JObject SubPropsObj = new JObject();
                JProperty SubPropsXmlNode = new JProperty( "subprops", SubPropsObj );
                foreach( CswNbtMetaDataNodeTypeProp FilterProp in Prop.NodeTypeTab.NodeTypePropsByDisplayOrder )
                {
                    if( FilterProp.FilterNodeTypePropId == Prop.FirstPropVersionId )
                    {
                        PropObj["hassubprops"] = "true";

                        JObject FilterPropXml = _makePropJson( SubPropsObj, Node, FilterProp, FilterProp.DisplayRow, FilterProp.DisplayColumn );

                        // Hide those for whom the filter doesn't match
                        // (but we need the XML node to be there to store the value, for client-side changes)
                        FilterPropXml["display"] = FilterProp.CheckFilter( Node ).ToString().ToLower();

                    } // if( FilterProp.FilterNodeTypePropId == Prop.FirstPropVersionId )
                } // foreach( CswNbtMetaDataNodeTypeProp FilterProp in Tab.NodeTypePropsByDisplayOrder )
            } // if-else( EditMode == NodeEditMode.AddInPopup )
        } // addProp()


        private JObject _makePropJson( JObject ParentObj, CswNbtNode Node, CswNbtMetaDataNodeTypeProp Prop, Int32 Row, Int32 Column )
        {
            CswNbtNodePropWrapper PropWrapper = Node.Properties[Prop];

            CswPropIdAttr PropIdAttr = null;
            if( Node.NodeId != null )
            {
                PropIdAttr = new CswPropIdAttr( Node, Prop );
            }
            else
            {
                PropIdAttr = new CswPropIdAttr( null, Prop );
            }

            JObject PropObj = new JObject();
            ParentObj.Add( "prop_" + PropIdAttr.ToString(), PropObj );

            PropObj.Add( new JProperty( "id", PropIdAttr.ToString() ) );
            PropObj.Add( new JProperty( "name", Prop.PropNameWithQuestionNo ) );
            PropObj.Add( new JProperty( "helptext", PropWrapper.HelpText ) );
            PropObj.Add( new JProperty( "fieldtype", Prop.FieldType.FieldType.ToString() ) );
            if( Prop.ObjectClassProp != null )
            {
                PropObj.Add( new JProperty( "ocpname", Prop.ObjectClassProp.PropName ) );
            }
            PropObj.Add( new JProperty( "displayrow", Row.ToString() ) );
            PropObj.Add( new JProperty( "displaycol", Column.ToString() ) );
            PropObj.Add( new JProperty( "required", Prop.IsRequired.ToString().ToLower() ) );
            bool IsReadOnly = ( Prop.ReadOnly ||                  // nodetype_props.readonly
                                PropWrapper.ReadOnly ||           // jct_nodes_props.readonly
                                Node.ReadOnly ||                  // nodes.readonly
                                !_CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Edit, Prop.NodeType, false, Prop.NodeTypeTab, null, Node, Prop ) );

            PropObj.Add( new JProperty( "readonly", IsReadOnly.ToString().ToLower() ) );
            PropObj.Add( new JProperty( "gestalt", PropWrapper.Gestalt.Replace( "\"", "&quot;" ) ) );
            PropObj.Add( new JProperty( "copyable", Prop.IsCopyable().ToString().ToLower() ) );
            PropObj.Add( new JProperty( "highlight", PropWrapper.AuditChanged.ToString().ToLower() ) );

            PropWrapper.ToJSON( PropObj );

            return PropObj;
        } // _makePropJson()


        public void _getAuditHistoryGridProp( JObject ParentObj, CswNbtNode Node )
        {
            Random Num = new Random();
            JObject PropObj = new JObject();
            ParentObj.Add( new JProperty( "prop" + Num.Next(), PropObj ) );
            PropObj.Add( new JProperty( "name", "Audit History" ) );
            PropObj.Add( new JProperty( "helptext", string.Empty ) );
            PropObj.Add( new JProperty( "fieldtype", "AuditHistoryGrid" ) );
            PropObj.Add( new JProperty( "displayrow", "1" ) );
            PropObj.Add( new JProperty( "displaycol", "1" ) );
            PropObj.Add( new JProperty( "required", "false" ) );
            PropObj.Add( new JProperty( "readonly", "true" ) );

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
                    if( EditMode == NodeEditMode.AddInPopup )
                    {
                        Prop.DisplayColAdd = NewColumn;
                        Prop.DisplayRowAdd = NewRow;
                    }
                    else
                    {
                        Prop.DisplayColumn = NewColumn;
                        Prop.DisplayRow = NewRow;
                    }
                    ret = true;
                }
            } // if( _CswNbtResources.Permit.can( CswNbtActionName.Design ) || _CswNbtResources.CurrentNbtUser.IsAdministrator() )
            else
            {
                throw new CswDniException( ErrorType.Warning, "You do not have permission to configure layout", _CswNbtResources.CurrentNbtUser.Username + " tried to change property layout without administrative or Design privileges" );
            }
            return ret;
        } // moveProp()

        public JObject saveProps( NodeEditMode EditMode, string NodeId, string NodeKey, Int32 TabId, string NewPropsJson, Int32 NodeTypeId, CswNbtView View )
        {
            JObject ret = null;
            JObject PropsObj = JObject.Parse( NewPropsJson );

            CswNbtNode Node = null;
            CswNbtNodeKey NbtNodeKey = null;
            Node = EditMode == NodeEditMode.AddInPopup ?
                                    _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode ) :
                                    _getNode( NodeId, NodeKey, DateTime.MinValue );

            if( Node != null &&
                ( EditMode == NodeEditMode.AddInPopup &&
                  _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Create, Node.NodeType ) ) ||
                _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Edit, Node.NodeType, false, Node.NodeType.getNodeTypeTab( TabId ), null, Node, null ) )
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
                    _applyPropJson( Node, PropObj );
                }

                // BZ 8517 - this sets sequences that have setvalonadd = 0
                _CswNbtResources.CswNbtNodeFactory.CswNbtNodeWriter.setSequenceValues( Node );

                Node.postChanges( false );

                if( NbtNodeKey == null && View != null )
                {
                    // Get the nodekey of this node in the current view
                    ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, true, true, false, false );
                    NbtNodeKey = Tree.getNodeKeyByNodeId( Node.NodeId );
                    if( NbtNodeKey == null )
                    {
                        // Make a nodekey from the default view
                        View = Node.NodeType.CreateDefaultView();
                        View.Root.ChildRelationships[0].NodeIdsToFilterIn.Add( Node.NodeId );
                        Tree = _CswNbtResources.Trees.getTreeFromView( View, true, true, false, false );
                        NbtNodeKey = Tree.getNodeKeyByNodeId( Node.NodeId );
                    }
                }
                string NodeKeyString = string.Empty;
                if( NbtNodeKey != null )
                    NodeKeyString = wsTools.ToSafeJavaScriptParam( NbtNodeKey.ToString() );

                ret = new JObject( new JProperty( "result", "Succeeded" ),
                                    new JProperty( "nodeid", Node.NodeId.ToString() ),
                                    new JProperty( "cswnbtnodekey", NodeKeyString ) );
            }
            else
            {
                ret = new JObject( new JProperty( "result", "Failed" ) );
            }
            return ret;
        } // saveProps()

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
                        else
                        {
                            ret = false;
                        }
                    } // foreach( string NodeIdStr in CopyNodeIds )
                } // if(SourceNode != null)
            } // if( Int32.MinValue != SourceNodeKey.NodeId.PrimaryKey )
            return ret;
        } // copyPropValues()

        private void _applyPropJson( CswNbtNode Node, JObject PropObj )
        {
            CswPropIdAttr PropIdAttr = new CswPropIdAttr( CswConvert.ToString( PropObj["id"] ) );

            CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( PropIdAttr.NodeTypePropId );
            Node.Properties[MetaDataProp].ReadJSON( PropObj, null, null );

            // Recurse on sub-props
            if( null != PropObj["subprops"] )
            {
                JObject SubPropsObj = (JObject) PropObj["subprops"];
                if( SubPropsObj.HasValues )
                {
                    foreach( JObject ChildPropObj in SubPropsObj.Properties()
                                .Where( ChildProp => null != ChildProp.Value )
                                .Select( ChildProp => (JObject) ChildProp.Value )
                                .Where( ChildPropObj => ChildPropObj.HasValues ) )
                    {
                        _applyPropJson( Node, ChildPropObj );
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

        public bool SetPropBlobValue( byte[] Data, string FileName, string ContentType, string PropIdAttr )
        {
            bool ret = false;
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
                    JctTable.Rows[0]["blobdata"] = Data;
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
                    JRow["blobdata"] = Data;
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
