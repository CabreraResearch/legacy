using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.Statistics;
using ChemSW.StructureSearch;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ServiceDrivers
{
    public class CswNbtSdTabsAndProps
    {

        private readonly CswNbtResources _CswNbtResources;
        private readonly ICswNbtUser _ThisUser;
        private readonly bool _IsMultiEdit;
        private readonly bool _ConfigMode;
        private string HistoryTabPrefix = "history_";
        private CswNbtStatisticsEvents _CswNbtStatisticsEvents;

        public CswNbtSdTabsAndProps( CswNbtResources CswNbtResources, CswNbtStatisticsEvents CswNbtStatisticsEvents = null, bool Multi = false, bool ConfigMode = false )
        {
            _CswNbtResources = CswNbtResources;
            _ThisUser = _CswNbtResources.CurrentNbtUser;
            _IsMultiEdit = Multi;
            _ConfigMode = ConfigMode;
            _CswNbtStatisticsEvents = CswNbtStatisticsEvents;
        }

        public JObject getTabs( string NodeId, string NodeKey, CswDateTime Date, string filterToPropId )
        {
            JObject Ret = new JObject();
            CswNbtNode Node = _CswNbtResources.Nodes.GetNode( NodeId, NodeKey, Date );
            if( null != Node )
            {
                TabOrderModifier = 0;
                CswNbtMetaDataNodeType NodeType = Node.getNodeType();
                Int32 NodeTypeId = Int32.MinValue;
                if( filterToPropId != string.Empty )
                {
                    CswPropIdAttr PropId = new CswPropIdAttr( filterToPropId );
                    CswNbtMetaDataNodeTypeProp Prop = _CswNbtResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId );
                    if( null != Prop )
                    {
                        NodeTypeId = Prop.NodeTypeId;
                        foreach( CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout EditLayout in Prop.getEditLayouts().Values )
                        {
                            //_CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.View, Prop.getNodeType(), false, Tab, _CswNbtResources.CurrentNbtUser, Node.NodeId, Prop )
                            CswNbtMetaDataNodeTypeTab Tab = _CswNbtResources.MetaData.getNodeTypeTab( EditLayout.TabId );
                            if(
                                    ( _ConfigMode || Tab.TabName != CswNbtMetaData.IdentityTabName ) && (
                                        _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.View, NodeType ) ||
                                        _CswNbtResources.Permit.canTab( CswNbtPermit.NodeTypePermission.View, NodeType, Tab ) ||
                                        (
                                         _CswNbtResources.Permit.isNodeWritable( CswNbtPermit.NodeTypePermission.View, NodeType, Node.NodeId ) &&
                                         _CswNbtResources.Permit.isPropWritable( CswNbtPermit.NodeTypePermission.View, Prop, Tab )
                                        )
                                    )
                               )
                            {
                                _makeTab( Ret, Tab, false );
                                break;
                            }
                        }
                    }
                }
                else
                {
                    NodeTypeId = Node.NodeTypeId;
                    foreach( CswNbtMetaDataNodeTypeTab Tab in from _Tab in _CswNbtResources.MetaData.getNodeTypeTabs( Node.NodeTypeId )
                                                              orderby _Tab.TabOrder, _Tab.TabName
                                                              where ( ( _ConfigMode || _Tab.TabName != CswNbtMetaData.IdentityTabName ) &&
                                                                    (
                                                                        _CswNbtResources.Permit.canTab( CswNbtPermit.NodeTypePermission.View, Node.getNodeType(), _Tab ) ||
                                                                        _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.View, Node.getNodeType() )
                                                                    ) )
                                                              select _Tab )
                    {
                        _makeTab( Ret, Tab, _canEditLayout() );
                    }

                    // History tab
                    if( false == _ConfigMode &&
                        false == _IsMultiEdit &&
                        Date.IsNull &&
                        NodeType.AuditLevel != Audit.AuditLevel.NoAudit &&
                        CswConvert.ToBoolean( _CswNbtResources.ConfigVbls.getConfigVariableValue( "auditing" ) ) )
                    {
                        if( _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.View, NodeType ) )
                        {
                            _makeTab( Ret, Int32.MaxValue, HistoryTabPrefix + NodeId, "History", false );
                        }
                    }
                    Ret["nodename"] = Node.NodeName;
                    CswNbtMetaDataNodeTypeLayoutMgr.LayoutType LayoutType = _CswNbtResources.MetaData.NodeTypeLayout.LayoutTypeForEditMode( _CswNbtResources.EditMode );
                    if( LayoutType == CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit )
                    {
                        CswNbtMetaDataNodeTypeTab IdentityTab = NodeType.getIdentityTab();
                        JObject Props = new JObject();
                        if( _CswNbtResources.Permit.canTab( CswNbtPermit.NodeTypePermission.View, NodeType, IdentityTab ) )
                        {
                            Props = getProps( Node, IdentityTab.TabId.ToString(), null, LayoutType );
                            Props.Remove( "nodeid" );
                        }
                        Ret["IdentityTab"] = Props;
                        Ret["IdentityTab"]["tabid"] = IdentityTab.TabId;
                    }
                } // if-else( filterToPropId != string.Empty )
                Ret["nodetypeid"] = NodeTypeId;
            }
            return Ret;
        } // getTabs()

        private Int32 TabOrderModifier = 0;

        public void _makeTab( JObject ParentObj, CswNbtMetaDataNodeTypeTab Tab, bool CanEditLayout )
        {
            if( null != Tab )
            {
                if( Tab.getNodeTypePropsByDisplayOrder().Any() )
                {
                    _makeTab( ParentObj, Tab.TabOrder, Tab.TabId.ToString(), Tab.TabName, CanEditLayout );
                }
            }
        }

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

        public CswNbtNode getAddNode( CswNbtMetaDataNodeType NodeType, CswNbtNodeCollection.MakeNodeOperation NodeOp = null )
        {
            CswNbtNode Ret = null;
            if( null != NodeType )
            {
                if( _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.Create, NodeType ) )
                {
                    NodeOp = NodeOp ?? CswNbtNodeCollection.MakeNodeOperation.MakeTemp;
                    Ret = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeType.NodeTypeId, NodeOp );
                }
                else
                {
                    throw new CswDniException(ErrorType.Warning, "Insufficient permission to create a new " + NodeType.NodeTypeName, "User " + _CswNbtResources.CurrentNbtUser.Username + " does not have Create permission for " + NodeType.NodeTypeName );
                }
            }
            return Ret;
        }

        public CswNbtNode getAddNode( Int32 NodeTypeId, string RelatedNodeId, string RelatedNodeTypeId, string RelatedObjectClassId, CswNbtNodeCollection.MakeNodeOperation NodeOp = null )
        {
            CswNbtNode Ret = null;
            CswNbtMetaDataNodeType NodeType = null;
            if( NodeTypeId != Int32.MinValue )
            {
                NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                if( null != NodeType )
                {
                    if( false == _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.Create, NodeType ) )
                    {
                        throw new CswDniException( ErrorType.Warning, "Insufficient permission to create a new " + NodeType.NodeTypeName, "User " + _CswNbtResources.CurrentNbtUser.Username + " does not have Create permission for " + NodeType.NodeTypeName );
                    }
                    else
                    {
                        NodeOp = NodeOp ?? CswNbtNodeCollection.MakeNodeOperation.MakeTemp;
                        Ret = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, NodeOp );
                        CswPrimaryKey RelatedNodePk = new CswPrimaryKey();
                        RelatedNodePk.FromString( RelatedNodeId );
                        if( Int32.MinValue != RelatedNodePk.PrimaryKey )
                        {
                            Int32 RelatedNodeTypePk = CswConvert.ToInt32( RelatedNodeTypeId );
                            Int32 RelatedObjectClassPk = CswConvert.ToInt32( RelatedObjectClassId );

                            if( Int32.MinValue != RelatedNodeTypePk && Int32.MinValue == RelatedObjectClassPk )
                            {
                                CswNbtMetaDataNodeType RelatedNodeType = _CswNbtResources.MetaData.getNodeType( RelatedNodeTypePk );
                                if( null != RelatedNodeType )
                                {
                                    RelatedObjectClassPk = RelatedNodeType.ObjectClassId;
                                }
                            }

                            if( Int32.MinValue != RelatedNodeTypePk && Int32.MinValue != RelatedObjectClassPk )
                            {
                                foreach( CswNbtNodePropRelationship Relationship in from _Prop
                                                                                        in Ret.Properties
                                                                                    where _Prop.getFieldType().FieldType == CswNbtMetaDataFieldType.NbtFieldType.Relationship &&
                                                                                      ( ( _Prop.AsRelationship.TargetType == NbtViewRelatedIdType.NodeTypeId &&
                                                                                          _Prop.AsRelationship.TargetId == RelatedNodeTypePk ) ||
                                                                                       ( _Prop.AsRelationship.TargetType == NbtViewRelatedIdType.ObjectClassId &&
                                                                                          _Prop.AsRelationship.TargetId == RelatedObjectClassPk ) )
                                                                                    select _Prop )
                                {
                                    Relationship.RelatedNodeId = RelatedNodePk;
                                    Ret.postChanges( ForceUpdate: false );
                                }
                            }
                        }
                    }
                }
            }
            return Ret;
        }

        /// <summary>
        /// Fetch or create a node, and return a JObject for all properties in a given tab
        /// </summary>
        public JObject getProps( string NodeId, string NodeKey, string TabId, Int32 NodeTypeId, CswDateTime Date, string filterToPropId, string RelatedNodeId, string RelatedNodeTypeId, string RelatedObjectClassId )
        {
            JObject Ret = new JObject();

            CswPropIdAttr FilterPropIdAttr = null;
            if( filterToPropId != string.Empty )
            {
                FilterPropIdAttr = new CswPropIdAttr( filterToPropId );
            }

            if( TabId.StartsWith( HistoryTabPrefix ) )
            {
                CswNbtNode Node = _CswNbtResources.getNode( NodeId, NodeKey, Date );
                if( _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.View, Node.getNodeType() ) )
                {
                    _getAuditHistoryGridProp( Ret, Node );
                }
            }
            else
            {
                CswNbtMetaDataNodeTypeLayoutMgr.LayoutType LayoutType = _CswNbtResources.MetaData.NodeTypeLayout.LayoutTypeForEditMode( _CswNbtResources.EditMode );

                CswNbtNode Node;
                if( _CswNbtResources.EditMode == NodeEditMode.Add )
                {
                    Node = getAddNode( NodeTypeId, RelatedNodeId, RelatedNodeTypeId, RelatedObjectClassId );
                }
                else
                {
                    Node = _CswNbtResources.getNode( NodeId, NodeKey, Date );
                }
                return getProps( Node, TabId, FilterPropIdAttr, LayoutType );

            } // if-else( TabId.StartsWith( HistoryTabPrefix ) )
            return Ret;
        } // getProps()

        /// <summary>
        /// Get props of a Node instance
        /// </summary>
        public JObject getProps( CswNbtNode Node, string TabId, CswPropIdAttr FilterPropIdAttr, CswNbtMetaDataNodeTypeLayoutMgr.LayoutType LayoutType )
        {
            JObject Ret = new JObject();
            if( Node != null )
            {
                if( CswTools.IsPrimaryKey( Node.NodeId ) )
                {
                    Ret["nodeid"] = Node.NodeId.ToString();
                    Ret["nodelink"] = Node.NodeLink;
                }
                CswNbtMetaDataNodeType NodeType = Node.getNodeType();

                if( TabId.StartsWith( HistoryTabPrefix ) )
                {
                    if( _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.View, Node.getNodeType() ) )
                    {
                        _getAuditHistoryGridProp( Ret, Node );
                    }
                }
                else
                {
                    IEnumerable<CswNbtMetaDataNodeTypeProp> Props = _CswNbtResources.MetaData.NodeTypeLayout.getPropsInLayout( Node.NodeTypeId, CswConvert.ToInt32( TabId ), LayoutType );

                    if( _CswNbtResources.EditMode != NodeEditMode.Add ||
                        _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.Create, NodeType ) )
                    {
                        var CswNbtNodePropColl = Node.Properties;
                        IEnumerable<CswNbtMetaDataNodeTypeProp> FilteredProps = ( from _Prop in Props
                                                                                  where CswNbtNodePropColl != null
                                                                                  let Pw = CswNbtNodePropColl[_Prop]
                                                                                  where _ConfigMode ||
                                                                                        _showProp( LayoutType, _Prop, FilterPropIdAttr, CswConvert.ToInt32( TabId ), Node )
                                                                                  select _Prop );

                        foreach( CswNbtMetaDataNodeTypeProp Prop in FilteredProps )
                        {
                            _addProp( Ret, Node, Prop, CswConvert.ToInt32( TabId ) );
                        }
                    }
                } // if(Node != null)
            }
            return Ret;
        }

        /// <summary>
        /// Get props of a Node instance
        /// </summary>
        public void addPropsToResponse( JObject Ret, IEnumerable<CswNbtMetaDataNodeTypeProp> Props, CswNbtNode Node )
        {
            foreach( CswNbtMetaDataNodeTypeProp Prop in Props )
            {
                _addProp( Ret, Node, Prop, Int32.MinValue );
            }
        }

        private bool _showProp( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType LayoutType, CswNbtMetaDataNodeTypeProp Prop, CswPropIdAttr FilterPropIdAttr, Int32 TabId, CswNbtNode Node )
        {
            bool RetShow = false;

            if( LayoutType == CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add )
            {
                //Case 24023: Exclude buttons on Add
                RetShow = ( Prop.EditProp( Node, _ThisUser, true ) &&
                            Prop.getFieldType().FieldType != CswNbtMetaDataFieldType.NbtFieldType.Button );
            }
            else
            {   
                RetShow = Prop.ShowProp( LayoutType, Node, _ThisUser, TabId );
            }
            RetShow = RetShow && ( FilterPropIdAttr == null || Prop.PropId == FilterPropIdAttr.NodeTypePropId );
            return RetShow;
        }

        /// <summary>
        /// Returns JObject for a single property and its conditional properties
        /// </summary>
        public JObject getSingleProp( string NodeId, string NodeKey, string PropIdFromJson, Int32 NodeTypeId, string NewPropJson )
        {
            JObject Ret = new JObject();
            JObject PropObj = new JObject();

            CswNbtNode Node = _CswNbtResources.getNode( NodeId, NodeKey, new CswDateTime( _CswNbtResources ) );
            if( null == Node &&
                ( _CswNbtResources.EditMode == NodeEditMode.Add || _CswNbtResources.EditMode == NodeEditMode.Temp ) &&
                NodeTypeId != Int32.MinValue )
            {
                Node = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
            }

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

                // for prop filters, update node prop value but don't save the change
                JObject PropJson = CswConvert.ToJObject( NewPropJson, true, "NewPropJson" );

                CswNbtSdNode NodeAction = new CswNbtSdNode( _CswNbtResources, _CswNbtStatisticsEvents );
                NodeAction.addSingleNodeProp( Node, PropJson, null );

                CswPropIdAttr PropIdAttr = new CswPropIdAttr( PropIdFromJson );
                CswNbtMetaDataNodeTypeProp Prop = _CswNbtResources.MetaData.getNodeTypeProp( PropIdAttr.NodeTypePropId );
                if( Prop.FirstEditLayout != null )
                {
                    _addProp( PropObj, Node, Prop, Prop.FirstEditLayout.TabId );
                }

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

        private void _addProp( JObject ParentObj, CswNbtNode Node, CswNbtMetaDataNodeTypeProp Prop, Int32 TabId )
        {
            CswNbtMetaDataNodeTypeLayoutMgr.LayoutType LayoutType = _CswNbtResources.MetaData.NodeTypeLayout.LayoutTypeForEditMode( _CswNbtResources.EditMode );
            CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout Layout = Prop.getLayout( LayoutType, TabId );
            if( false == Node.Properties[Prop].Hidden || _ConfigMode )
            {
                JProperty JpProp = makePropJson( Node.NodeId, TabId, Prop, Node.Properties[Prop], Layout.DisplayRow, Layout.DisplayColumn, Layout.TabGroup );
                ParentObj.Add( JpProp );
                JObject PropObj = (JObject) JpProp.Value;

                // Handle conditional properties
                JObject SubPropsObj = new JObject();
                JProperty SubPropsJProp = new JProperty( "subprops", SubPropsObj );
                PropObj.Add( SubPropsJProp );
                bool HasSubProps = false;
                foreach( CswNbtMetaDataNodeTypeProp FilterProp in
                        _CswNbtResources.MetaData.NodeTypeLayout.getPropsInLayout( Prop.NodeTypeId, Layout.TabId, LayoutType ) )
                {
                    if( FilterProp.FilterNodeTypePropId == Prop.FirstPropVersionId )
                    {
                        HasSubProps = true;
                        CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout FilterPropLayout =
                            _CswNbtResources.MetaData.NodeTypeLayout.getLayout( LayoutType, FilterProp.PropId, TabId );
                        JProperty JPFilterProp = makePropJson( Node.NodeId, TabId, FilterProp,
                                                               Node.Properties[FilterProp],
                                                               FilterPropLayout.DisplayRow,
                                                               FilterPropLayout.DisplayColumn,
                                                               FilterPropLayout.TabGroup );
                        SubPropsObj.Add( JPFilterProp );
                        JObject FilterPropXml = (JObject) JPFilterProp.Value;

                        // Hide those for whom the filter doesn't match
                        // (but we need the XML node to be there to store the value, for client-side changes)
                        FilterPropXml["display"] = FilterProp.CheckFilter( Node ).ToString().ToLower();

                    } // if( FilterProp.FilterNodeTypePropId == Prop.FirstPropVersionId )
                } // foreach( CswNbtMetaDataNodeTypeProp FilterProp in Tab.NodeTypePropsByDisplayOrder )
                PropObj["hassubprops"] = HasSubProps;
            } // if( false == Node.Properties[Prop].Hidden )
        } // addProp()

        private Dictionary<Int32, Collection<Int32>> _DisplayRowsAndCols = new Dictionary<Int32, Collection<Int32>>();

        private Int32 _getUniqueRow( Int32 ProposedRow, Int32 Column )
        {
            Int32 Ret = ProposedRow;
            if( ProposedRow < 1 )
            {
                ProposedRow = 1;
            }
            if( Column < 1 )
            {
                Column = 1;
            }
            if( false == _DisplayRowsAndCols.ContainsKey( Column ) )
            {
                _DisplayRowsAndCols.Add( Column, new Collection<Int32> { ProposedRow } );
            }
            else if( false == _DisplayRowsAndCols[Column].Contains( ProposedRow ) )
            {
                _DisplayRowsAndCols[Column].Add( ProposedRow );
            }
            else
            {
                Ret = _getUniqueRow( ProposedRow + 1, Column );
            }
            return Ret;
        }

        public JProperty makePropJson( CswPrimaryKey NodeId, Int32 TabId, CswNbtMetaDataNodeTypeProp Prop, CswNbtNodePropWrapper PropWrapper, Int32 Row, Int32 Column, string TabGroup )
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
            Int32 DisplayRow = _getUniqueRow( Row, Column );

            PropObj["displayrow"] = DisplayRow.ToString();
            PropObj["displaycol"] = Column.ToString();
            PropObj["tabgroup"] = TabGroup;
            PropObj["required"] = Prop.IsRequired;
            PropObj["copyable"] = Prop.IsCopyable();

            bool ShowPropertyName = false == ( FieldType == CswNbtMetaDataFieldType.NbtFieldType.Image ||
                                               FieldType == CswNbtMetaDataFieldType.NbtFieldType.Button ||
                                               ( FieldType == CswNbtMetaDataFieldType.NbtFieldType.Grid &&
                                                 PropWrapper.AsGrid.GridMode == CswNbtNodePropGrid.GridPropMode.Full ) );

            PropObj["showpropertyname"] = ShowPropertyName;

            CswNbtMetaDataNodeTypeTab Tab = null;
            if( _CswNbtResources.MetaData.NodeTypeLayout.LayoutTypeForEditMode( _CswNbtResources.EditMode ) == CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit &&
                TabId != Int32.MinValue )
            {
                Tab = _CswNbtResources.MetaData.getNodeTypeTab( TabId );
            }

            if( PropWrapper != null )
            {
                CswNbtMetaDataNodeType NodeType = Prop.getNodeType();
                if( _CswNbtResources.Permit.isPropWritable( CswNbtPermit.NodeTypePermission.Edit, Prop, Tab, PropWrapper ) &&
                    _CswNbtResources.Permit.isNodeWritable( CswNbtPermit.NodeTypePermission.Edit, NodeType, NodeId ) &&
                    ( _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.Edit, NodeType ) ||
                    _CswNbtResources.Permit.canTab( CswNbtPermit.NodeTypePermission.Edit, NodeType, Tab ) )
                    )
                {

                    PropObj["readonly"] = false;
                }
                else
                {
                    PropObj["readonly"] = true;
                }

                PropObj["gestalt"] = PropWrapper.Gestalt.Replace( "\"", "&quot;" );
                PropObj["highlight"] = PropWrapper.AuditChanged.ToString().ToLower();
                PropWrapper.ToJSON( PropObj );
            }
            return ret;
        } // makePropJson()


        public void _getAuditHistoryGridProp( JObject ParentObj, CswNbtNode Node )
        {
            if( Node != null )
            {
                JObject PropObj = new JObject();
                string FakePropIdAttr = Node.NodeId.ToString() + "_audit";
                ParentObj["prop_" + FakePropIdAttr] = PropObj;
                PropObj["name"] = "Audit History";
                PropObj["helptext"] = string.Empty;
                PropObj["fieldtype"] = "AuditHistoryGrid";
                PropObj["displayrow"] = 1;
                PropObj["displaycol"] = 1;
                PropObj["required"] = false;
                PropObj["readonly"] = true;
                PropObj["id"] = FakePropIdAttr;
                PropObj["showpropertyname"] = false;
            }
        } // _getAuditHistoryGridProp()

        public bool moveProp( string PropIdAttr, Int32 TabId, Int32 NewRow, Int32 NewColumn )
        {
            bool ret = false;
            if( _canEditLayout() )
            {
                CswPropIdAttr PropId = new CswPropIdAttr( PropIdAttr );
                Int32 NodeTypePropId = PropId.NodeTypePropId;
                if( NodeTypePropId != Int32.MinValue && NewRow > 0 && NewColumn > 0 )
                {
                    CswNbtMetaDataNodeTypeProp Prop = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );
                    Prop.updateLayout( _CswNbtResources.MetaData.NodeTypeLayout.LayoutTypeForEditMode( _CswNbtResources.EditMode ), false, TabId, NewRow, NewColumn );
                    ret = true;
                }
            } // _canEditLayout()
            else
            {
                throw new CswDniException( ErrorType.Warning, "You do not have permission to configure layout", _CswNbtResources.CurrentNbtUser.Username + " tried to change property layout without administrative or Design privileges" );
            }
            return ret;
        } // moveProp()

        public bool removeProp( string PropIdAttr, Int32 TabId )
        {
            bool ret = false;
            if( _canEditLayout() )
            {
                CswPropIdAttr PropId = new CswPropIdAttr( PropIdAttr );
                Int32 NodeTypePropId = PropId.NodeTypePropId;
                if( NodeTypePropId != Int32.MinValue )
                {
                    CswNbtMetaDataNodeTypeProp Prop = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );
                    if( _CswNbtResources.EditMode == NodeEditMode.Add && Prop.IsRequired && false == Prop.HasDefaultValue() )
                    {
                        throw new CswDniException( ErrorType.Warning, Prop.PropName + " may not be removed", Prop.PropName + " is required and has no unique value, and therefore cannot be removed from 'Add' layouts" );
                    }
                    Prop.removeFromLayout( _CswNbtResources.MetaData.NodeTypeLayout.LayoutTypeForEditMode( _CswNbtResources.EditMode ), TabId );
                    ret = true;
                }
            } // _canEditLayout()
            else
            {
                throw new CswDniException( ErrorType.Warning, "You do not have permission to configure layout", _CswNbtResources.CurrentNbtUser.Username + " tried to change property layout without administrative or Design privileges" );
            }
            return ret;
        } // removeProp()

        private CswNbtNode _addNode( CswNbtMetaDataNodeType NodeType, CswNbtNode Node, JObject PropsObj, out CswNbtNodeKey RetNbtNodeKey, CswNbtView View = null, CswNbtMetaDataNodeTypeTab NodeTypeTab = null )
        {
            CswNbtNode Ret = Node;
            RetNbtNodeKey = null;
            CswNbtActQuotas QuotaAction = new CswNbtActQuotas( _CswNbtResources );
            if( QuotaAction.CheckQuotaNT( NodeType ) )
            {
                if( null == Ret || false == CswTools.IsPrimaryKey( Ret.NodeId ) )
                {
                    Ret = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeType.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                }
                bool CanEdit = (
                                    _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.Edit, NodeType ) ||
                                    _CswNbtResources.Permit.canTab( CswNbtPermit.NodeTypePermission.Edit, NodeType, NodeTypeTab ) ||
                                    _CswNbtResources.Permit.isNodeWritable( CswNbtPermit.NodeTypePermission.Edit, NodeType, Ret.NodeId )
                               );
                if( CanEdit )
                {
                    RetNbtNodeKey = _saveProp( Ret, PropsObj, View, NodeTypeTab, true );
                }
            }
            else
            {
                throw new CswDniException( ErrorType.Warning, "Quota Exceeded", "You have used all of your purchased quota, and must purchase additional quota space in order to add" );
            }
            return Ret;
        }

        public CswNbtNode addNode( CswNbtMetaDataNodeType NodeType, JObject PropsObj, out CswNbtNodeKey RetNbtNodeKey, CswNbtView View = null, CswNbtMetaDataNodeTypeTab NodeTypeTab = null )
        {
            return _addNode( NodeType, null, PropsObj, out RetNbtNodeKey, View, NodeTypeTab );
        }

        public CswNbtNode addNode( CswNbtMetaDataNodeType NodeType, CswNbtNode Node, JObject PropsObj, out CswNbtNodeKey RetNbtNodeKey, CswNbtView View = null, CswNbtMetaDataNodeTypeTab NodeTypeTab = null )
        {
            return _addNode( NodeType, Node, PropsObj, out RetNbtNodeKey, View, NodeTypeTab );
        }

        public JObject saveProps( CswPrimaryKey NodePk, Int32 TabId, JObject PropsObj, Int32 NodeTypeId, CswNbtView View, bool IsIdentityTab )
        {
            JObject ret = new JObject();
            if( PropsObj.HasValues )
            {
                CswNbtNodeKey RetNbtNodeKey = null;
                bool AllSucceeded = false;
                CswNbtNode Node = null;
                CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                CswNbtMetaDataNodeTypeTab NodeTypeTab = null;
                if( false == IsIdentityTab )
                {
                    NodeTypeTab = _CswNbtResources.MetaData.getNodeTypeTab( TabId );
                }
                else if( null != NodeType )
                {
                    NodeTypeTab = NodeType.getIdentityTab();
                }

                if( null == NodeType && null != NodeTypeTab )
                {
                    NodeType = NodeTypeTab.getNodeType();
                }
                if( null != NodeType )
                {
                    Node = _CswNbtResources.Nodes.GetNode( NodePk );
                    switch( _CswNbtResources.EditMode )
                    {
                        case NodeEditMode.Temp:
                            if( null != Node )
                            {
                                addNode( NodeType, Node, PropsObj, out RetNbtNodeKey, View, NodeTypeTab );
                            }
                            else
                            {
                                Node = addNode( NodeType, null, PropsObj, out RetNbtNodeKey, View, NodeTypeTab );
                            }
                            break;
                        case NodeEditMode.Add:
                            if( null != Node )
                            {
                                Node.IsTemp = false;
                                addNode( NodeType, Node, PropsObj, out RetNbtNodeKey, View, NodeTypeTab );
                            }
                            else
                            {
                                Node = addNode( NodeType, null, PropsObj, out RetNbtNodeKey, View, NodeTypeTab );
                            }

                            AllSucceeded = ( null != Node );
                            break;
                        default:
                            if( null != Node )
                            {
                                bool CanEdit = (
                                                   _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.Edit, NodeType ) ||
                                                   _CswNbtResources.Permit.canTab( CswNbtPermit.NodeTypePermission.Edit, NodeType, NodeTypeTab ) ||
                                                   _CswNbtResources.Permit.canAnyTab( CswNbtPermit.NodeTypePermission.Edit, NodeType ) ||
                                                   _CswNbtResources.Permit.isNodeWritable( CswNbtPermit.NodeTypePermission.Edit, NodeType, Node.NodeId )


                                               );
                                if( CanEdit )
                                {
                                    if( Node.PendingUpdate )
                                    {
                                        CswNbtActUpdatePropertyValue Act = new CswNbtActUpdatePropertyValue( _CswNbtResources );
                                        Act.UpdateNode( Node, false );
                                    }
                                    RetNbtNodeKey = _saveProp( Node, PropsObj, View, NodeTypeTab );
                                    if( null != RetNbtNodeKey )
                                    {
                                        AllSucceeded = true;
                                    }
                                }
                            }
                            break;
                    } //switch( _CswNbtResources.EditMode )
                    if( AllSucceeded && null != RetNbtNodeKey )
                    {
                        string RetNodeKey = RetNbtNodeKey.ToString();
                        ret["nodekey"] = RetNodeKey;
                    } //if( AllSucceeded && null != RetNbtNodeKey )
                    else
                    {
                        string ErrString;
                        if( _CswNbtResources.EditMode == NodeEditMode.Add )
                        {
                            ErrString = "Attempt to Add failed.";
                        }
                        else
                        {
                            ErrString = "Prop updates failed";
                        }
                        ret["result"] = ErrString;
                    } //else
                } //if( null != NodeType && null != NodeTypeTab )

                // Good opportunity to force an update on the node
                if( Node != null )
                {
                    CswNbtActUpdatePropertyValue ActUpdatePropVal = new CswNbtActUpdatePropertyValue( _CswNbtResources );
                    ActUpdatePropVal.UpdateNode( Node, true );
                    Node.postChanges( false );
                    ret["result"] = "Succeeded";
                    //If we're Adding, NodeName won't be valid until now.
                    ret["nodename"] = Node.NodeName;
                    ret["nodelink"] = Node.NodeLink;
                    ret["nodeid"] = Node.NodeId.ToString();
                    ret["action"] = _determineAction( Node.ObjClass.ObjectClass.ObjectClass );
                }
            }
            return ret;
        } // saveProps()

        private string _determineAction( NbtObjectClass objectClass )
        {
            CswNbtObjClass.NbtButtonAction ret;
            switch( objectClass )
            {
                case NbtObjectClass.FeedbackClass:
                    ret = CswNbtObjClass.NbtButtonAction.loadView;
                    break;
                default:
                    ret = CswNbtObjClass.NbtButtonAction.refresh;
                    break;
            }
            return ret.ToString();
        }

        private CswNbtNodeKey _saveProp( CswNbtNode Node, JObject PropsObj, CswNbtView View, CswNbtMetaDataNodeTypeTab Tab, bool ForceUpdate = false )
        {
            CswNbtNodeKey Ret = null;
            if( Node != null )
            {
                CswNbtSdNode NodeAction = new CswNbtSdNode( _CswNbtResources, _CswNbtStatisticsEvents );
                NodeAction.addNodeProps( Node, PropsObj, Tab );

                /* Case 8517 - this sets sequences that have setvalonadd = 0 */
                _CswNbtResources.CswNbtNodeFactory.CswNbtNodeWriter.setSequenceValues( Node );

                ICswNbtTree Tree;
                if( View != null )
                {
                    // Get the nodekey of this node in the current view
                    Tree = _CswNbtResources.Trees.getTreeFromView( _CswNbtResources.CurrentNbtUser, View, true, false, false );
                    Ret = Tree.getNodeKeyByNodeId( Node.NodeId );
                }
                if( Ret == null )
                {
                    // Make a nodekey from the default view
                    View = Node.getNodeType().CreateDefaultView();
                    View.Root.ChildRelationships[0].NodeIdsToFilterIn.Add( Node.NodeId );
                    Tree = _CswNbtResources.Trees.getTreeFromView( _CswNbtResources.CurrentNbtUser, View, true, false, false );
                    Ret = Tree.getNodeKeyByNodeId( Node.NodeId );
                }
            }
            return Ret;
        }

        public JObject copyPropValues( string SourceNodeId, string CopyNodeIds, string PropIds )
        {
            JObject ret = new JObject();
            CswNbtNode SourceNode = _CswNbtResources.Nodes[SourceNodeId];
            if( null != SourceNode )
            {
                CswCommaDelimitedString CopyToNodeIds = new CswCommaDelimitedString();
                CopyToNodeIds.FromString( CopyNodeIds );
                if( CopyToNodeIds.Count > 0 )
                {
                    CswCommaDelimitedString CopyFromPropIds = new CswCommaDelimitedString();
                    CopyFromPropIds.FromString( PropIds );
                    if( CopyFromPropIds.Count > 0 )
                    {
                        if( CopyToNodeIds.Count == 0 )
                        {
                            ret["result"] = "false";
                        }
                        else if( CopyToNodeIds.Count < CswNbtBatchManager.getBatchThreshold( _CswNbtResources ) )
                        {
                            Collection<CswNbtNode> CopyToNodes = new Collection<CswNbtNode>();
                            foreach( string CopyToNodeId in CopyToNodeIds )
                            {
                                if( string.Compare( CopyToNodeId, SourceNodeId, StringComparison.OrdinalIgnoreCase ) != 0 )
                                {
                                    CswNbtNode Node = _CswNbtResources.Nodes[CopyToNodeId];
                                    if( null != Node &&
                                        Node.NodeTypeId == SourceNode.NodeTypeId &&
                                        _CswNbtResources.Permit.isNodeWritable( CswNbtPermit.NodeTypePermission.Edit, SourceNode.getNodeType(), Node.NodeId ) )
                                    {
                                        CopyToNodes.Add( Node );
                                    }
                                }
                            }

                            foreach( CswNbtNode CopyToNode in CopyToNodes )
                            {
                                foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in CopyFromPropIds.Select( PropIdAttr => new CswPropIdAttr( PropIdAttr ) )
                                    .Select( PropId => _CswNbtResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId ) ) )
                                {
                                    CopyToNode.Properties[NodeTypeProp].copy( SourceNode.Properties[NodeTypeProp] );
                                }

                                CopyToNode.postChanges( ForceUpdate: false );

                            } // foreach( string NodeIdStr in CopyNodeIds )
                            ret["result"] = "true";
                        } // else if( RealCopyNodeIds.Count < CswNbtBatchManager.getBatchThreshold( _CswNbtResources ) )
                        else
                        {
                            // Shelve this to a batch operation
                            Collection<Int32> NodeTypePropIds = new Collection<Int32>();
                            foreach( string PropIdAttrStr in CopyFromPropIds )
                            {
                                CswPropIdAttr PropIdAttr = new CswPropIdAttr( PropIdAttrStr );
                                NodeTypePropIds.Add( PropIdAttr.NodeTypePropId );
                            }
                            Collection<CswPrimaryKey> CopyNodePks = new Collection<CswPrimaryKey>();
                            foreach( string CopyToNodeId in CopyToNodeIds )
                            {
                                CopyNodePks.Add( CswConvert.ToPrimaryKey( CopyToNodeId ) );
                            }
                            CswNbtBatchOpMultiEdit op = new CswNbtBatchOpMultiEdit( _CswNbtResources );
                            CswNbtObjClassBatchOp BatchNode = op.makeBatchOp( SourceNode, CopyNodePks, NodeTypePropIds );
                            ret["batch"] = BatchNode.NodeId.ToString();
                        }
                    } // if(SourceNode != null)
                } // if( Int32.MinValue != SourceNodeKey.NodeId.PrimaryKey )
            }


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
                CswNbtNode CopyFromNode = _CswNbtResources.getNode( NodeId, NodeKey, new CswDateTime( _CswNbtResources ) );
                NodeType = CopyFromNode.getNodeType();
            }

            if( NodeType != null )
            {
                IEnumerable<CswNbtMetaDataNodeTypeProp> Props = _CswNbtResources.MetaData.NodeTypeLayout.getPropsNotInLayout( NodeType, CswConvert.ToInt32( TabId ), LayoutType );
                foreach( CswNbtMetaDataNodeTypeProp Prop in from Prop in Props
                                                            orderby Prop.PropNameWithQuestionNo
                                                            select Prop )
                {
                    // case 24179
                    if( Prop.getFieldType().IsLayoutCompatible( LayoutType ) )
                    {
                        JObject ThisPropObj = new JObject();
                        ThisPropObj["propid"] = Prop.PropId.ToString();
                        ThisPropObj["propname"] = Prop.PropNameWithQuestionNo.ToString();
                        ThisPropObj["hidden"] = ( LayoutType == CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit && Prop.FirstEditLayout == null ).ToString().ToLower();
                        ret.Add( ThisPropObj );
                    }
                }
            } // if( NodeType != null )
            return ret;
        } // getPropertiesForLayoutAdd()


        public bool addPropertyToLayout( string PropId, string TabId, CswNbtMetaDataNodeTypeLayoutMgr.LayoutType LayoutType )
        {
            CswNbtMetaDataNodeTypeProp Prop = _CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( PropId ) );
            _CswNbtResources.MetaData.NodeTypeLayout.updatePropLayout( LayoutType, Prop.NodeTypeId, Prop, false, CswConvert.ToInt32( TabId ), Int32.MinValue, Int32.MinValue );
            return true;
        } // addPropertyToLayout()

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
                Node.postChanges( false );
                if( IncludeBlob )
                {
                    PropWrapper.ClearBlob();
                }
                ret = true;
            }
            return ret;
        } // ClearPropValue()

        public JObject saveMolProp( string moldata, string propIdAttr )
        {
            JObject Ret = new JObject();
            bool Succeeded = false;
            CswPropIdAttr PropId = new CswPropIdAttr( propIdAttr );
            CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId );
            if( Int32.MinValue != PropId.NodeId.PrimaryKey )
            {
                CswNbtNode Node = _CswNbtResources.Nodes[PropId.NodeId];
                if( null != Node )
                {
                    CswNbtNodePropMol PropMol = Node.Properties[MetaDataProp];
                    if( null != PropMol )
                    {
                        // Do the update directly
                        CswTableUpdate JctUpdate = _CswNbtResources.makeCswTableUpdate( "Clobber_save_update", "jct_nodes_props" );
                        //JctUpdate.AllowBlobColumns = true;
                        if( PropMol.JctNodePropId > 0 )
                        {
                            DataTable JctTable = JctUpdate.getTable( "jctnodepropid", PropMol.JctNodePropId );
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
                        Succeeded = true;
                        Ret["mol"] = PropMol.Mol;
                        byte[] molImage = CswStructureSearch.GetImage( moldata );
                        string Href;
                        SetPropBlobValue( molImage, "mol.jpeg", "image/jpeg", propIdAttr, "blobdata", out Href );
                        Ret["href"] = Href;
                    }
                }
            } // if( Int32.MinValue != NbtNodeKey.NodeId.PrimaryKey )
            Ret["succeeded"] = Succeeded;
            return Ret;
        }

        public bool SetPropBlobValue( byte[] Data, string FileName, string ContentType, string PropIdAttr, string Column, out string Href )
        {
            bool ret = false;
            if( String.IsNullOrEmpty( Column ) ) Column = "blobdata";
            Href = "";
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
                    Href = CswNbtNodePropBlob.getLink( PropWrapper.JctNodePropId, PropId.NodeId, PropId.NodeTypePropId );
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
                if( Node.getObjectClass().ObjectClass == NbtObjectClass.ReportClass )
                {
                    CswNbtObjClassReport Report = Node;
                    CswFilePath FilePathTools = new CswFilePath( _CswNbtResources );
                    string ReportPath = FilePathTools.getFullReportFilePath( Report.RPTFile.JctNodePropId.ToString() );
                    _createReportFile( ReportPath, Report.RPTFile.JctNodePropId, Data );
                }
                Node.postChanges( ForceUpdate: false );
                ret = true;
            } // if( Int32.MinValue != NbtNodeKey.NodeId.PrimaryKey )
            return ret;
        } // SetPropBlobValue()

        private void _createReportFile( string ReportTempFileName, int NodePropId, byte[] BlobData )
        {
            ( new FileInfo( ReportTempFileName ) ).Directory.Create();
            FileMode fileMode = File.Exists( ReportTempFileName ) ? FileMode.Truncate : FileMode.CreateNew;
            FileStream fs = new FileStream( ReportTempFileName, fileMode );
            BinaryWriter BWriter = new BinaryWriter( fs, System.Text.Encoding.Default );
            BWriter.Write( BlobData );
        }

        private bool _canEditLayout()
        {
            return ( _CswNbtResources.Permit.can( CswNbtActionName.Design ) || _CswNbtResources.CurrentNbtUser.IsAdministrator() );
        }

        public JObject getObjectClassButtons( string ObjectClassId )
        {
            JObject Buttons = new JObject();

            CswNbtMetaDataObjectClass Oc = _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( ObjectClassId ) );
            foreach( CswNbtMetaDataObjectClassProp Prop in Oc.getObjectClassProps() )
            {
                CswNbtMetaDataFieldType Type = Prop.getFieldType();
                if( Type.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Button )
                {
                    string propName = "button_" + Prop.PropId;
                    Buttons[propName] = new JObject();
                    Buttons[propName]["id"] = Prop.PropId;
                    Buttons[propName]["name"] = Prop.PropName;
                }
            }

            return Buttons;
        }

        /// <summary>
        /// Returns the JSON needed to build the full location tree, no nodes attached
        /// </summary>
        /// <param name="SelectedNodeId">Location tree's selected NodeId - if null, uses the User's default location</param>
        /// <returns></returns>
        public JObject getLocationTree( string SelectedNodeId )
        {
            JObject LocationTreeJSON = new JObject();
            CswPrimaryKey LocationId = String.IsNullOrEmpty( SelectedNodeId )
                                           ? _CswNbtResources.CurrentNbtUser.DefaultLocationId
                                           : CswConvert.ToPrimaryKey( SelectedNodeId );
            CswNbtMetaDataNodeType LocationNT = _CswNbtResources.MetaData.getNodeType( "Site" );
            CswNbtObjClassLocation LocationNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( LocationNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
            LocationNode.Location.SelectedNodeId = LocationId;
            LocationNode.Location.RefreshNodeName();
            LocationNode.Location.ToJSON( LocationTreeJSON );
            return LocationTreeJSON;
        }

    } // class CswNbtSdTabsAndProps

} // namespace ChemSW.Nbt.WebServices
