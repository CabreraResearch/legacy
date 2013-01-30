using System;
using System.Collections.Generic;
using System.Data;
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
            _FieldTypeRule = (CswNbtFieldTypeRuleRelationship) CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            _NameSubField = _FieldTypeRule.NameSubField;
            _NodeIDSubField = _FieldTypeRule.NodeIDSubField;

            // case 25956
            _SearchThreshold = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswNbtResources.ConfigurationVariables.relationshipoptionlimit.ToString() ) );
            if( _SearchThreshold <= 0 )
            {
                _SearchThreshold = 100;
            }
        }
        private CswNbtFieldTypeRuleRelationship _FieldTypeRule;
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


        override public string Gestalt
        {
            get
            {
                return _CswNbtNodePropData.Gestalt;
            }//

        }//Gestalt


        public CswNbtView View
        {
            get
            {
                CswNbtView Ret = _getView( _CswNbtResources, _CswNbtMetaDataNodeTypeProp );
                _setRootRelationship( Ret );
                return Ret;
            }
        }

        private static CswNbtView _getView( CswNbtResources NbtResources, CswNbtMetaDataNodeTypeProp RelationshipProp )
        {
            CswNbtView Ret = null;
            if( RelationshipProp.ViewId.isSet() )
            {
                Ret = NbtResources.ViewSelect.restoreView( RelationshipProp.ViewId );
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
                Int32 ThisNodeTypeId = this.NodeTypeProp.NodeTypeId;
                Int32 ThisObjectClassId = this.NodeTypeProp.getNodeType().ObjectClassId;

                Int32 TargetNodeTypeId = Int32.MinValue;
                Int32 TargetObjectClassId = Int32.MinValue;
                _getIds( _CswNbtResources, TargetType, TargetId, out TargetNodeTypeId, out TargetObjectClassId );

                if( ThisNodeTypeId != TargetNodeTypeId && ThisObjectClassId != TargetObjectClassId )
                {
                    foreach( CswNbtViewRelationship RootRel in View.Root.ChildRelationships )
                    {
                        Int32 RootNodeTypeId = Int32.MinValue;
                        Int32 RootObjectClassId = Int32.MinValue;
                        _getIds( _CswNbtResources, RootRel.SecondType, RootRel.SecondId, out RootNodeTypeId, out RootObjectClassId );

                        if( RootNodeTypeId == ThisNodeTypeId || RootObjectClassId == ThisObjectClassId )
                        {
                            RootRel.NodeIdsToFilterIn.Add( this.NodeId );
                        }

                    } // foreach( CswNbtViewRelationship RootRel in View.Root.ChildRelationships )
                } // if( ThisNodeTypeId != TargetNodeTypeId && ThisObjectClassId != TargetObjectClassId )
            } // if( this.NodeTypeProp != null )
        } // _setRootRelationship()

        private static void _getIds( CswNbtResources NbtResources, NbtViewRelatedIdType Type, Int32 Id, out Int32 NodeTypeId, out Int32 ObjectClassId )
        {
            NodeTypeId = Int32.MinValue;
            ObjectClassId = Int32.MinValue;
            if( Type == NbtViewRelatedIdType.NodeTypeId )
            {
                CswNbtMetaDataNodeType NodeType = NbtResources.MetaData.getNodeType( Id );
                if( NodeType != null )
                {
                    NodeTypeId = NodeType.NodeTypeId;
                    ObjectClassId = NodeType.ObjectClassId;
                }
            }
            else
            {
                ObjectClassId = Id;
            }
        }

        private string TargetTableName
        {
            get
            {
                string ret = "nodes";
                if( TargetId != Int32.MinValue )
                {
                    if( TargetType == NbtViewRelatedIdType.NodeTypeId )
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
                string StringVal = _CswNbtNodePropData.GetPropRowValue( _NodeIDSubField.Column );
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
                        throw new CswDniException( ErrorType.Error, "Invalid reference", "CswNbtNodePropRelationship.RelatedNodeId requires a primary key from tablename '" + TargetTableName + "' but got one from tablename '" + PotentialKey.TableName + "' instead." );
                    }
                    if( RelatedNodeId != PotentialKey )
                    {
                        _CswNbtNodePropData.SetPropRowValue( _NodeIDSubField.Column, PotentialKey.PrimaryKey );
                        CswNbtNode RelatedNode = _CswNbtResources.Nodes[value];
                        if( null != RelatedNode )
                        {
                            CachedNodeName = RelatedNode.NodeName;
                        }
                    }
                }
                else
                {
                    _CswNbtNodePropData.SetPropRowValue( _NodeIDSubField.Column, Int32.MinValue );
                }

                if( WasModified )
                {
                    PendingUpdate = true;
                }
            }
        }

        public string CachedNodeName
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _NameSubField.Column );
            }
            set
            {
                if( value != _CswNbtNodePropData.GetPropRowValue( _NameSubField.Column ) )
                {
                    _CswNbtNodePropData.SetPropRowValue( _NameSubField.Column, value, IsNonModifying: true );
                    _CswNbtNodePropData.Gestalt = value;
                }
            }
        }

        /// <summary>
        /// RelatedIdType of the TargetId
        /// </summary>
        public NbtViewRelatedIdType TargetType
        {
            get
            {
                NbtViewRelatedIdType ret = _targetType( _CswNbtResources, _CswNbtMetaDataNodeTypeProp );
                return ret;
            }
        }

        private static NbtViewRelatedIdType _targetType( CswNbtResources NbtResources, CswNbtMetaDataNodeTypeProp RelationshipProp )
        {
            NbtViewRelatedIdType ret = NbtViewRelatedIdType.Unknown;
            try
            {
                ret = (NbtViewRelatedIdType) RelationshipProp.FKType;
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

        public static Dictionary<CswPrimaryKey, string> getOptions( CswNbtResources NbtResources, CswNbtMetaDataNodeTypeProp RelationshipProp, CswPrimaryKey RelatedNodeId )
        {
            CswNbtView View = _getView( NbtResources, RelationshipProp );
            Dictionary<CswPrimaryKey, string> Options = new Dictionary<CswPrimaryKey, string>();
            if( View != null )
            {
                if( !RelationshipProp.IsRequired || RelatedNodeId == null )
                {
                    Options.Add( new CswPrimaryKey(), "" );
                }

                Int32 TargetNodeTypeId;
                Int32 TargetObjectClassId;
                _getIds( NbtResources, _targetType( NbtResources, RelationshipProp ), RelationshipProp.FKValue, out TargetNodeTypeId, out TargetObjectClassId );

                ICswNbtTree CswNbtTree = NbtResources.Trees.getTreeFromView(
                    View: View,
                    IncludeSystemNodes: false,
                    RequireViewPermissions: false,
                    IncludeHiddenNodes: false );
                _addOptionsRecurse( Options, CswNbtTree, TargetNodeTypeId, TargetObjectClassId );
            }
            return Options;
        }

        public static Dictionary<CswPrimaryKey, string> getOptions( CswNbtResources NbtResources, CswNbtViewId ViewId, Int32 TargetNodeTypeId, Int32 TargetObjectClassId )
        {
            CswNbtView View = NbtResources.ViewSelect.restoreView( ViewId );
            Dictionary<CswPrimaryKey, string> Options = new Dictionary<CswPrimaryKey, string>();
            if( View != null )
            {
                ICswNbtTree CswNbtTree = NbtResources.Trees.getTreeFromView(
                    View: View,
                    IncludeSystemNodes: false,
                    RequireViewPermissions: false,
                    IncludeHiddenNodes: false );
                _addOptionsRecurse( Options, CswNbtTree, TargetNodeTypeId, TargetObjectClassId );
            }
            return Options;
        }

        private static void _addOptionsRecurse( Dictionary<CswPrimaryKey, string> Options, ICswNbtTree CswNbtTree, Int32 TargetNodeTypeId, Int32 TargetObjectClassId )
        {
            for( Int32 c = 0; c < CswNbtTree.getChildNodeCount(); c++ )
            {
                CswNbtTree.goToNthChild( c );
                if( CswNbtTree.getNodeKeyForCurrentPosition().NodeTypeId == TargetNodeTypeId ||
                    CswNbtTree.getNodeKeyForCurrentPosition().ObjectClassId == TargetObjectClassId )
                {
                    Options.Add( CswNbtTree.getNodeIdForCurrentPosition(), CswNbtTree.getNodeNameForCurrentPosition() );
                }

                _addOptionsRecurse( Options, CswNbtTree, TargetNodeTypeId, TargetObjectClassId );

                CswNbtTree.goToParentNode();
            } // for( Int32 c = 0; c < CswNbtTree.getChildNodeCount(); c++ )
        } // _addOptionsRecurse()

        //public bool isNodeReference() { return true; }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_NodeIDSubField.ToXmlNodeName( true ).ToLower()] = default( string );
            if( RelatedNodeId != null && Int32.MinValue != RelatedNodeId.PrimaryKey )
            {
                ParentObject[_NodeIDSubField.ToXmlNodeName( true ).ToLower()] = RelatedNodeId.ToString();
            }
            ParentObject[_NameSubField.ToXmlNodeName( true ).ToLower()] = CachedNodeName;

            CswNbtMetaDataNodeType TargetNodeType = null;
            CswNbtMetaDataObjectClass TargetObjectClass = null;
            ParentObject["nodetypeid"] = default( string );
            ParentObject["objectclassid"] = default( string );
            if( TargetType == NbtViewRelatedIdType.NodeTypeId )
            {
                ParentObject["nodetypeid"] = TargetId.ToString();
                TargetNodeType = _CswNbtResources.MetaData.getNodeType( TargetId );
                if( null != TargetNodeType )
                {
                    TargetObjectClass = TargetNodeType.getObjectClass();
                }
            }
            else if( TargetType == NbtViewRelatedIdType.ObjectClassId )
            {
                ParentObject["objectclassid"] = TargetId.ToString();
                TargetObjectClass = _CswNbtResources.MetaData.getObjectClass( TargetId );
            }

            bool AllowAdd = ( TargetObjectClass != null && TargetObjectClass.CanAdd && // case 27189
                ( TargetNodeType == null || _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.Create, TargetNodeType ) ) );
            ParentObject["allowadd"] = AllowAdd;

            ParentObject["relatednodeid"] = default( string );
            CswNbtNode RelatedNode = _CswNbtResources.Nodes[RelatedNodeId];
            if( null != RelatedNode )
            {
                ParentObject["relatednodeid"] = RelatedNode.NodeId.ToString();
                ParentObject["relatednodelink"] = RelatedNode.NodeLink;
            }
            ParentObject["viewid"] = View.ViewId.ToString();

            bool AllowEdit = _CswNbtResources.Permit.isPropWritable( CswNbtPermit.NodeTypePermission.Create, NodeTypeProp, null );

            if( AllowEdit )
            {
                ParentObject["usesearch"] = false;
                CswPrimaryKey pk = null;
                if( null != RelatedNode )
                {
                    pk = RelatedNode.NodeId;
                }
                Dictionary<CswPrimaryKey, string> Options = getOptions( _CswNbtResources, _CswNbtMetaDataNodeTypeProp, pk );
                if( Options.Count > _SearchThreshold )
                {
                    ParentObject["usesearch"] = "true";
                }
                else
                {
                    JArray JOptions = new JArray();
                    ParentObject["options"] = JOptions;

                    foreach( CswPrimaryKey NodePk in Options.Keys ) //.Where( NodePk => NodePk != null && NodePk.PrimaryKey != Int32.MinValue ) )
                    {
                        JObject JOption = new JObject();
                        if( NodePk != null && NodePk.PrimaryKey != Int32.MinValue )
                        {
                            JOption["id"] = NodePk.ToString();
                            JOption["value"] = Options[NodePk];
                            JOption["link"] = CswNbtNode.getNodeLink( NodePk, Options[NodePk] );
                        }
                        else
                        {
                            JOption["id"] = "";
                            JOption["value"] = "";
                        }
                        JOptions.Add( JOption );
                    }
                }
            }
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
            _CswNbtNodePropData.Gestalt = CachedNodeName;
        }

    }//CswNbtNodePropRelationship

}//namespace ChemSW.Nbt.PropTypes

