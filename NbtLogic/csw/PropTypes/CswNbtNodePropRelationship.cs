using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{
    public class CswNbtNodePropRelationship : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropRelationship( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsRelationship;
        }

        public CswNbtNodePropRelationship( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _NameSubField = ( (CswNbtFieldTypeRuleRelationship) _FieldTypeRule ).NameSubField;
            _NodeIDSubField = ( (CswNbtFieldTypeRuleRelationship) _FieldTypeRule ).NodeIDSubField;

            // case 25956
            _SearchThreshold = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumNbtConfigurationVariables.relationshipoptionlimit.ToString() ) );
            if( _SearchThreshold <= 0 )
            {
                _SearchThreshold = 100;
            }

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _NameSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => CachedNodeName, x => CachedNodeName = CswConvert.ToString( x ) ) );
            _SubFieldMethods.Add( _NodeIDSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => RelatedNodeId, x => RelatedNodeId = CswConvert.ToPrimaryKey( x ) ) );
        }

        private CswNbtSubField _NameSubField;
        private CswNbtSubField _NodeIDSubField;

        private Int32 _SearchThreshold;

        override public bool Empty
        {
            get
            {
                return ( RelatedNodeId == null || Int32.MinValue == RelatedNodeId.PrimaryKey );
            }//
        }

        private CswNbtView _View = null;
        public CswNbtView View
        {
            get
            {
                if( null == _View )
                {
                    //_View = _getView( _CswNbtResources, _CswNbtMetaDataNodeTypeProp );
                    CswNbtViewId ViewId = _CswNbtMetaDataNodeTypeProp.ViewId;
                    if( null != ViewId && ViewId.isSet() )
                    {
                        _View = _getView( _CswNbtResources, ViewId );
                        _setRootRelationship( _View );
                    }
                }
                return _View;
            }
        }

        /// <summary>
        /// Override the view for determining options (usually from business logic)
        /// </summary>
        public void OverrideView( CswNbtView NewView )
        {
            _View = NewView;
            _setRootRelationship( _View );
        }

        private static CswNbtView _getView( CswNbtResources NbtResources, CswNbtViewId ViewId )
        {
            CswNbtView Ret = null;
            if( ViewId.isSet() )
            {
                Ret = NbtResources.ViewSelect.restoreView( ViewId );
            }
            return Ret;
        }

        private void _setRootRelationship( CswNbtView View )
        {
            // case 27488
            // If the root nodetype or object class of the Relationship options view 
            // matches the current nodetype or object class, 
            // and does not match the target of the relationship, 
            // limit the root to the current node.

            if( null != this.NodeId && Int32.MinValue != this.NodeId.PrimaryKey && null != this.NodeTypeProp )
            {
                //Int32 ThisNodeTypeId = this.NodeTypeProp.NodeTypeId;
                //Int32 ThisObjectClassId = this.NodeTypeProp.getNodeType().ObjectClassId;

                //Int32 TargetNodeTypeId = Int32.MinValue;
                //Int32 TargetObjectClassId = Int32.MinValue;
                //_getIds( _CswNbtResources, TargetType, TargetId, out TargetNodeTypeId, out TargetObjectClassId );

                //if( ThisNodeTypeId != TargetNodeTypeId && ThisObjectClassId != TargetObjectClassId )
                //{
                if( false == CswNbtViewRelationship.Matches( _CswNbtResources, TargetType, TargetId, this.NodeTypeProp.getNodeType() ) )
                {
                    foreach( CswNbtViewRelationship RootRel in View.Root.ChildRelationships )
                    {
                        //Int32 RootNodeTypeId = Int32.MinValue;
                        //Int32 RootObjectClassId = Int32.MinValue;
                        //_getIds( _CswNbtResources, RootRel.SecondType, RootRel.SecondId, out RootNodeTypeId, out RootObjectClassId );
                        //if( RootNodeTypeId == ThisNodeTypeId || RootObjectClassId == ThisObjectClassId )
                        //{

                        if( CswNbtViewRelationship.Matches( _CswNbtResources, RootRel.SecondType, RootRel.SecondId, this.NodeTypeProp.getNodeType() ) )
                        {
                            RootRel.NodeIdsToFilterIn.Add( this.NodeId );
                        }

                    } // foreach( CswNbtViewRelationship RootRel in View.Root.ChildRelationships )
                }
                //} // if( ThisNodeTypeId != TargetNodeTypeId && ThisObjectClassId != TargetObjectClassId )
            } // if( this.NodeTypeProp != null )
        } // _setRootRelationship()

        //private static void _getIds( CswNbtResources NbtResources, NbtViewRelatedIdType Type, Int32 Id, out Int32 NodeTypeId, out Int32 ObjectClassId )
        //{
        //    NodeTypeId = Int32.MinValue;
        //    ObjectClassId = Int32.MinValue;
        //    if( Type == NbtViewRelatedIdType.NodeTypeId )
        //    {
        //        CswNbtMetaDataNodeType NodeType = NbtResources.MetaData.getNodeType( Id );
        //        if( NodeType != null )
        //        {
        //            NodeTypeId = NodeType.NodeTypeId;
        //            ObjectClassId = NodeType.ObjectClassId;
        //        }
        //    }
        //    else if( Type == NbtViewRelatedIdType.ObjectClassId )
        //    {
        //        ObjectClassId = Id;
        //    }
        //}

        private string TargetTableName
        {
            get
            {
                string ret = "nodes";
                if( TargetId != Int32.MinValue )
                {
                    if( TargetType == CswEnumNbtViewRelatedIdType.NodeTypeId )
                    {
                        CswNbtMetaDataNodeType TargetNodeType = _CswNbtResources.MetaData.getNodeType( TargetId );
                        if( TargetNodeType != null )
                            ret = TargetNodeType.TableName;
                    }
                    //else if( TargetType == RelatedIdType.ObjectClassId )
                    //    ret = _CswNbtResources.MetaData.getObjectClass( TargetId ).TableName;
                }
                return ret;
            }
        }

        /// <summary>
        /// Empty the subfield data on this Prop
        /// </summary>
        public void clearRelationship()
        {
            RelatedNodeId = null;
            CachedNodeName = "";
        }

        /// <summary>
        /// Primary key of related node
        /// </summary>
        /// <remarks>
        /// When we store the primary key, we're losing the TableName.
        /// But since this is a relationship, the relationship stores the target object class, 
        /// and that stores the tablename.  We'll validate to be sure the number comes from the table we expect.
        /// </remarks>
        public CswPrimaryKey RelatedNodeId
        {
            get
            {
                CswPrimaryKey ret = null;
                string StringVal = GetPropRowValue( _NodeIDSubField );
                if( CswTools.IsInteger( StringVal ) )
                    ret = new CswPrimaryKey( TargetTableName, CswConvert.ToInt32( StringVal ) );
                return ret;
            }
            set
            {
                CswPrimaryKey PotentialKey = value;

                if( CswTools.IsPrimaryKey( PotentialKey ) &&
                    false == string.IsNullOrEmpty( PotentialKey.TableName ) &&
                    Int32.MinValue != PotentialKey.PrimaryKey ) //&& value.TableName == "nodes" )
                {
                    if( value.TableName != TargetTableName )
                    {
                        throw new CswDniException( CswEnumErrorType.Error, "Invalid reference", "CswNbtNodePropRelationship.RelatedNodeId requires a primary key from tablename '" + TargetTableName + "' but got one from tablename '" + PotentialKey.TableName + "' instead." );
                    }
                    if( RelatedNodeId != PotentialKey )
                    {
                        SetPropRowValue( _NodeIDSubField, PotentialKey.PrimaryKey );
                        CswNbtNode RelatedNode = _CswNbtResources.Nodes[value];
                        if( null != RelatedNode )
                        {
                            CachedNodeName = RelatedNode.NodeName;
                        }
                    }
                }
                else
                {
                    SetPropRowValue( _NodeIDSubField, Int32.MinValue );
                }
            }
        }

        public string CachedNodeName
        {
            get
            {
                return GetPropRowValue( _NameSubField );
            }
            set
            {
                if( value != GetPropRowValue( _NameSubField ) )
                {
                    SetPropRowValue( _NameSubField, value, IsNonModifying: true );
                    Gestalt = value;
                }
            }
        }

        /// <summary>
        /// RelatedIdType of the TargetId
        /// </summary>
        public CswEnumNbtViewRelatedIdType TargetType
        {
            get
            {
                CswEnumNbtViewRelatedIdType ret = _targetType( _CswNbtResources, _CswNbtMetaDataNodeTypeProp );
                return ret;
            }
        }

        private static CswEnumNbtViewRelatedIdType _targetType( CswNbtResources NbtResources, CswNbtMetaDataNodeTypeProp RelationshipProp )
        {
            CswEnumNbtViewRelatedIdType ret = CswEnumNbtViewRelatedIdType.Unknown;
            try
            {
                ret = (CswEnumNbtViewRelatedIdType) RelationshipProp.FKType;
            }
            catch( Exception ex )
            {
                if( !( ex is System.ArgumentException ) )
                    throw ( ex );
            }
            return ret;
        }

        /// <summary>
        /// Relationship's Target NodeTypeId
        /// </summary>
        public Int32 TargetId
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.FKValue;
            }
            //set
            //{
            //    _CswNbtMetaDataNodeTypeProp.FKValue = value;
            //}
        }

        public bool TargetMatches( ICswNbtMetaDataDefinitionObject Compare, bool IgnoreVersions = false )
        {
            return CswNbtViewRelationship.Matches( _CswNbtResources, TargetType, TargetId, Compare, IgnoreVersions );
        }

        public void RefreshNodeName()
        {
            if( RelatedNodeId != null && RelatedNodeId.PrimaryKey != Int32.MinValue )
            {
                CswNbtNode Node = _CswNbtResources.Nodes.GetNode( RelatedNodeId );
                if( Node != null )
                    CachedNodeName = Node.NodeName;
                else
                    CachedNodeName = string.Empty;
            }
            else
            {
                CachedNodeName = string.Empty;
            }
            this.PendingUpdate = false;
        }

        public Int32 Rows
        {
            get
            {
                if( _CswNbtMetaDataNodeTypeProp.TextAreaRows == Int32.MinValue )
                    return 4;
                else
                    return _CswNbtMetaDataNodeTypeProp.TextAreaRows;
            }
        }

        private Dictionary<CswPrimaryKey, string> _getOptions( CswNbtResources NbtResources, bool IsRequired, Int32 FkValue, CswPrimaryKey RelatedNodeId, CswNbtView OptionsView )
        {
            Dictionary<CswPrimaryKey, string> Options = new Dictionary<CswPrimaryKey, string>();
            if( OptionsView != null )
            {
                //if( !RelationshipProp.IsRequired || RelatedNodeId == null )
                if( false == IsRequired || RelatedNodeId == null )
                {
                    Options.Add( new CswPrimaryKey(), "" );
                }

                //Int32 TargetNodeTypeId;
                //Int32 TargetObjectClassId;
                //_getIds( NbtResources, _targetType( NbtResources, RelationshipProp ), RelationshipProp.FKValue, out TargetNodeTypeId, out TargetObjectClassId );

                ICswNbtTree CswNbtTree = NbtResources.Trees.getTreeFromView( View: OptionsView,
                                                                             IncludeSystemNodes: false,
                                                                             RequireViewPermissions: false,
                                                                             IncludeHiddenNodes: false );
                CswEnumNbtViewRelatedIdType targetType = _targetType( NbtResources, _CswNbtMetaDataNodeTypeProp );
                //_addOptionsRecurse( NbtResources, Options, CswNbtTree, _targetType( NbtResources, RelationshipProp ), RelationshipProp.FKValue ); //, TargetNodeTypeId, TargetObjectClassId );
                _addOptionsRecurse( NbtResources, Options, CswNbtTree, targetType, FkValue );
                //if( RelationshipProp.IsRequired && Options.Count == 2 )
                if( IsRequired && Options.Count == 2 )
                {
                    Options.Remove( new CswPrimaryKey() );
                }
            }
            return Options;
        }

        //public static Dictionary<CswPrimaryKey, string> getOptions( CswNbtResources NbtResources, CswNbtViewId ViewId, Int32 TargetNodeTypeId, Int32 TargetObjectClassId )
        //{
        //    CswNbtView View = NbtResources.ViewSelect.restoreView( ViewId );
        //    Dictionary<CswPrimaryKey, string> Options = new Dictionary<CswPrimaryKey, string>();
        //    if( View != null )
        //    {
        //        ICswNbtTree CswNbtTree = NbtResources.Trees.getTreeFromView(
        //            View: View,
        //            IncludeSystemNodes: false,
        //            RequireViewPermissions: false,
        //            IncludeHiddenNodes: false );
        //        _addOptionsRecurse( Options, CswNbtTree, TargetNodeTypeId, TargetObjectClassId );
        //    }
        //    return Options;
        //}

        private static void _addOptionsRecurse( CswNbtResources NbtResources, Dictionary<CswPrimaryKey, string> Options, ICswNbtTree CswNbtTree, CswEnumNbtViewRelatedIdType TargetType, Int32 TargetId ) //, Int32 TargetNodeTypeId, Int32 TargetObjectClassId )
        {
            for( Int32 c = 0; c < CswNbtTree.getChildNodeCount(); c++ )
            {
                CswNbtTree.goToNthChild( c );
                CswNbtMetaDataNodeType NodeType = NbtResources.MetaData.getNodeType( CswNbtTree.getNodeKeyForCurrentPosition().NodeTypeId );
                if( CswNbtViewRelationship.Matches( NbtResources, TargetType, TargetId, NodeType ) )
                {
                    //if( CswNbtTree.getNodeKeyForCurrentPosition().NodeTypeId == TargetNodeTypeId ||
                    //    CswNbtTree.getNodeKeyForCurrentPosition().ObjectClassId == TargetObjectClassId )
                    //{
                    Options.Add( CswNbtTree.getNodeIdForCurrentPosition(), CswNbtTree.getNodeNameForCurrentPosition() );
                }

                _addOptionsRecurse( NbtResources, Options, CswNbtTree, TargetType, TargetId ); //TargetNodeTypeId, TargetObjectClassId );

                CswNbtTree.goToParentNode();
            } // for( Int32 c = 0; c < CswNbtTree.getChildNodeCount(); c++ )
        } // _addOptionsRecurse()

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }

        public override void ToJSON( JObject ParentObject )
        {
            base.ToJSON( ParentObject );  // FIRST

            
            JArray JOptions = new JArray();
            ParentObject["options"] = JOptions;

            CswNbtNode RelatedNode = null;
            if( CswTools.IsPrimaryKey( RelatedNodeId ) )
            {
                ParentObject[_NodeIDSubField.ToXmlNodeName( true ).ToLower()] = RelatedNodeId.ToString();
                RelatedNode = _CswNbtResources.Nodes[RelatedNodeId];
            }

            bool AllowEdit = _CswNbtResources.Permit.isPropWritable( CswEnumNbtNodeTypePermission.Create, NodeTypeProp, null );
            if( AllowEdit )
            {
                CswPrimaryKey pk = null;
                if( null != RelatedNode )
                {
                    pk = RelatedNode.NodeId;
                }
                Dictionary<CswPrimaryKey, string> Options = _getOptions( _CswNbtResources, _CswNbtMetaDataNodeTypeProp.IsRequired, _CswNbtMetaDataNodeTypeProp.FKValue, pk, View );
                if( Options.Count > _SearchThreshold )
                {
                    ParentObject["usesearch"] = true;
                }
                else
                {
                    Int32 OptionCount = 0;
                    CswPrimaryKey FirstPk = null;
                    foreach( CswPrimaryKey NodePk in Options.Keys ) //.Where( NodePk => NodePk != null && NodePk.PrimaryKey != Int32.MinValue ) )
                    {
                        if( CswTools.IsPrimaryKey( NodePk ) )
                        {
                            OptionCount += 1;
                            JObject JOption = new JObject();
                            FirstPk = FirstPk ?? NodePk;
                            JOption["id"] = NodePk.ToString();
                            JOption["value"] = Options[NodePk];
                            JOption["link"] = CswNbtNode.getNodeLink( NodePk, Options[NodePk] );
                            JOptions.Add( JOption );
                        }
                    }

                    //If Required, show empty when no value is selected and if there is more than one option to select from
                    bool ShowEmptyOption = Required && false == CswTools.IsPrimaryKey( RelatedNodeId ) && 1 < OptionCount;
                    //If not Required, always show the empty value
                    ShowEmptyOption = ShowEmptyOption || false == Required;

                    if( ShowEmptyOption )
                    {
                        JObject JOption = new JObject();
                        JOption[ "id" ] = "";
                        JOption[ "value" ] = "";
                        JOptions.AddFirst( JOption );
                    }
                    else
                    {
                        //Case 30030
                        if( null == RelatedNode && 
                            Required && 
                            Options.Count == 1 &&
                            CswTools.IsPrimaryKey(FirstPk) )
                        {
                            RelatedNode = _CswNbtResources.Nodes[ FirstPk ];
                        }
                    }
                }
            }

            ParentObject[_NodeIDSubField.ToXmlNodeName( true ).ToLower()] = string.Empty;
            ParentObject[_NameSubField.ToXmlNodeName( true ).ToLower()] = CachedNodeName;

            ParentObject["nodetypeid"] = 0;
            ParentObject["objectclassid"] = 0;
            ParentObject["propertysetid"] = 0;
            bool AllowAdd = false;
            if( TargetType == CswEnumNbtViewRelatedIdType.NodeTypeId )
            {
                ParentObject["nodetypeid"] = TargetId;
                CswNbtMetaDataNodeType TargetNodeType = _CswNbtResources.MetaData.getNodeType( TargetId );
                AllowAdd = ( null != TargetNodeType && _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Create, TargetNodeType ) );
            }
            else if( TargetType == CswEnumNbtViewRelatedIdType.ObjectClassId )
            {
                ParentObject["objectclassid"] = TargetId;
                CswNbtMetaDataObjectClass TargetObjectClass = _CswNbtResources.MetaData.getObjectClass( TargetId );
                AllowAdd = ( null != TargetObjectClass &&
                             TargetObjectClass.CanAdd &&
                             TargetObjectClass.getNodeTypes().Any( nt => _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Create, nt ) ) );
            }
            else if( TargetType == CswEnumNbtViewRelatedIdType.PropertySetId )
            {
                ParentObject["propertysetid"] = TargetId;
                CswNbtMetaDataPropertySet TargetPropSet = _CswNbtResources.MetaData.getPropertySet( TargetId );
                AllowAdd = TargetPropSet.getObjectClasses().Any( oc => null != oc &&
                                                                       oc.CanAdd &&
                                                                       oc.getNodeTypes().Any( nt => _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Create, nt ) ) );
            }
            ParentObject["allowadd"] = AllowAdd;

            ParentObject["relatednodeid"] = string.Empty;
            ParentObject["relatednodelink"] = string.Empty;
            if( null != RelatedNode )
            {
                ParentObject["relatednodeid"] = RelatedNode.NodeId.ToString();
                ParentObject["relatednodelink"] = RelatedNode.NodeLink;
            }
            ParentObject["viewid"] = View.ViewId.ToString();
            
            ParentObject["usesearch"] = false;
        } // ToJSON()

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            // Getting the value as a string is on purpose.
            //RelatedNodeId = new CswPrimaryKey( "nodes", _HandleReference( CswConvert.ToInt32( PropRow[_NodeIDSubField.ToXmlNodeName()] ), NodeMap ) );

            string NodeId = CswTools.XmlRealAttributeName( PropRow[_NodeIDSubField.ToXmlNodeName()].ToString() );
            if( NodeMap != null && NodeMap.ContainsKey( NodeId.ToLower() ) )
                RelatedNodeId = new CswPrimaryKey( "nodes", NodeMap[NodeId.ToLower()] );
            else if( CswTools.IsInteger( NodeId ) )
                RelatedNodeId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( NodeId ) );
            else
                RelatedNodeId = null;

            if( null != RelatedNodeId )
            {
                PendingUpdate = true;
            }
            /* As per steve, the intention of this side effect was that the input table from which the PropRow parameter
               comes would be written back to the original input xml. As Steve says, "it's a bit kookie." Since 
              the exeprimental algorithm keeps track of all this data in the temporary database tables, we don't need
              to maintain this. And since it brakes with the column structure of the temp tables, I'm commenting it 
              out at least for now. Bye bye . . . */
            /*
            if( RelatedNodeId != null )
            {
                PropRow["destnodeid"] = RelatedNodeId.PrimaryKey;
                PendingUpdate = true;
            }
             */
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_NodeIDSubField.ToXmlNodeName( true )] )
            {
                string NodePkString = JObject[_NodeIDSubField.ToXmlNodeName( true )].ToString();
                CswPrimaryKey thisRelatedNodeId = new CswPrimaryKey();
                bool validPk = thisRelatedNodeId.FromString( NodePkString );
                if( false == validPk )
                {
                    thisRelatedNodeId.TableName = "nodes";
                    thisRelatedNodeId.PrimaryKey = CswConvert.ToInt32( NodePkString );
                }
                if( thisRelatedNodeId.PrimaryKey != Int32.MinValue )
                {
                    if( NodeMap != null && NodeMap.ContainsKey( thisRelatedNodeId.PrimaryKey ) )
                    {
                        thisRelatedNodeId.PrimaryKey = NodeMap[thisRelatedNodeId.PrimaryKey];
                    }
                    RelatedNodeId = thisRelatedNodeId;
                    JObject["destnodeid"] = RelatedNodeId.PrimaryKey.ToString();
                    //PendingUpdate = true;
                }
                else
                {
                    RelatedNodeId = null;
                }
            }
        }
        //private Int32 _HandleReference( Int32 NodeId, Dictionary<string, Int32> NodeMap )
        //{
        //    Int32 ret = Int32.MinValue;
        //    if( NodeMap != null && NodeMap.ContainsKey( NodeId.ToString() ) )
        //        ret = NodeMap[NodeId.ToString()];
        //    else if( CswTools.IsInteger( NodeId ) )
        //        ret = CswConvert.ToInt32( NodeId );
        //    return ret;
        //}


        public bool IsUserRelationship()
        {
            return _CswNbtMetaDataNodeTypeProp.IsUserRelationship();
        }

        public override void SyncGestalt()
        {
            SetPropRowValue( CswEnumNbtSubFieldName.Gestalt, CswEnumNbtPropColumn.Gestalt, CachedNodeName );
        }

    }//CswNbtNodePropRelationship

}//namespace ChemSW.Nbt.PropTypes

