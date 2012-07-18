using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Xml.Linq;
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
        
        public CswNbtNodePropRelationship( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
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
                //CswNbtView Ret = new CswNbtView(_CswNbtResources);
                CswNbtView Ret = null;
                if( _CswNbtMetaDataNodeTypeProp.ViewId.isSet() )
                    //Ret.LoadXml(_CswNbtMetaDataNodeTypeProp.ViewId);
                    Ret = _CswNbtResources.ViewSelect.restoreView( _CswNbtMetaDataNodeTypeProp.ViewId );
                return Ret;
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
                if( value != null ) //&& value.TableName == "nodes" )
                {
                    if( value.TableName != TargetTableName )
                    {
                        throw new CswDniException( ErrorType.Error, "Invalid reference", "CswNbtNodePropRelationship.RelatedNodeId requires a primary key from tablename '" + TargetTableName + "' but got one from tablename '" + value.TableName + "' instead." );
                    }
                    if( RelatedNodeId != value )
                    {
                        _CswNbtNodePropData.SetPropRowValue( _NodeIDSubField.Column, value.PrimaryKey );
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
                    _CswNbtNodePropData.SetPropRowValue( _NameSubField.Column, value );
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
                NbtViewRelatedIdType ret = NbtViewRelatedIdType.Unknown;
                try
                {
                    //ret = (NbtViewRelatedIdType) Enum.Parse( typeof( NbtViewRelatedIdType ), _CswNbtMetaDataNodeTypeProp.FKType, true );
                    ret = (NbtViewRelatedIdType) _CswNbtMetaDataNodeTypeProp.FKType;
                }
                catch( Exception ex )
                {
                    if( !( ex is System.ArgumentException ) )
                        throw ( ex );
                }
                return ret;
            }
            //set
            //{
            //    _CswNbtMetaDataNodeTypeProp.FKType = value.ToString();
            //}
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

        public Dictionary<CswPrimaryKey, string> getOptions()
        {
            Dictionary<CswPrimaryKey, string> Options = new Dictionary<CswPrimaryKey, string>();
            if( View != null )
            {
                if( !Required || this.RelatedNodeId == null )
                    Options.Add( new CswPrimaryKey(), "" );

                ICswNbtTree CswNbtTree = _CswNbtResources.Trees.getTreeFromView( View, false, true, false, false, false );
                for( Int32 c = 0; c < CswNbtTree.getChildNodeCount(); c++ )
                {
                    CswNbtTree.goToNthChild( c );
                    Options.Add( CswNbtTree.getNodeIdForCurrentPosition(), CswNbtTree.getNodeNameForCurrentPosition() );
                    CswNbtTree.goToParentNode();
                } // for( Int32 c = 0; c < CswNbtTree.getChildNodeCount(); c++ )

            } // if( View != null )
            return Options;
        } // getOptions()


        //public bool isNodeReference() { return true; }

        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode RelatedNodeIdNode = CswXmlDocument.AppendXmlNode( ParentNode, _NodeIDSubField.ToXmlNodeName() );
            if( RelatedNodeId != null )
                RelatedNodeIdNode.InnerText = RelatedNodeId.PrimaryKey.ToString();

            CswXmlDocument.AppendXmlNode( ParentNode, _NameSubField.ToXmlNodeName(), CachedNodeName );

            if( TargetType == NbtViewRelatedIdType.NodeTypeId )
            {
                CswXmlDocument.AppendXmlNode( ParentNode, "nodetypeid", TargetId.ToString() );
            }
            if( _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Create, _CswNbtResources.MetaData.getNodeType( TargetId ) ) )
            {
                CswXmlDocument.AppendXmlNode( ParentNode, "allowadd", "true" );
            }

            XmlNode OptionsNode = CswXmlDocument.AppendXmlNode( ParentNode, "options" );
            Dictionary<CswPrimaryKey, string> Options = getOptions();
            foreach( CswPrimaryKey NodePk in Options.Keys )
            {
                XmlNode OptionNode = CswXmlDocument.AppendXmlNode( OptionsNode, "option" );
                if( NodePk != null && NodePk.PrimaryKey != Int32.MinValue )
                {
                    CswXmlDocument.AppendXmlAttribute( OptionNode, "id", NodePk.PrimaryKey.ToString() );
                    CswXmlDocument.AppendXmlAttribute( OptionNode, "value", Options[NodePk] );
                }
                else
                {
                    CswXmlDocument.AppendXmlAttribute( OptionNode, "id", "" );
                    CswXmlDocument.AppendXmlAttribute( OptionNode, "value", "" );
                }
            }
        }

        public override void ToXElement( XElement ParentNode )
        {
            ParentNode.Add( new XElement( _NodeIDSubField.ToXmlNodeName( true ), ( RelatedNodeId != null ) ?
                RelatedNodeId.PrimaryKey.ToString() : string.Empty ),
                            new XElement( _NameSubField.ToXmlNodeName( true ), CachedNodeName ) );

            if( TargetType == NbtViewRelatedIdType.NodeTypeId )
            {
                ParentNode.Add( new XElement( "nodetypeid", TargetId.ToString() ) );
            }
            if( _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Create, _CswNbtResources.MetaData.getNodeType( TargetId ) ) )
            {
                ParentNode.Add( new XElement( "allowadd", "true" ) );
            }

            XElement OptionsNode = new XElement( "options" );
            ParentNode.Add( OptionsNode );

            Dictionary<CswPrimaryKey, string> Options = getOptions();
            foreach( CswPrimaryKey NodePk in Options.Keys )
            {
                OptionsNode.Add( new XElement( "option",
                                               new XAttribute( "id", ( NodePk != null && NodePk.PrimaryKey != Int32.MinValue ) ?
                                                   NodePk.PrimaryKey.ToString() : "" ),
                                               new XAttribute( "value", ( NodePk != null && NodePk.PrimaryKey != Int32.MinValue ) ?
                                                   Options[NodePk] : "" ) ) );
            }
        }

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
                TargetObjectClass = TargetNodeType.getObjectClass();
            }
            else if( TargetType == NbtViewRelatedIdType.ObjectClassId )
            {
                ParentObject["objectclassid"] = TargetId.ToString();
                TargetObjectClass = _CswNbtResources.MetaData.getObjectClass( TargetId );
            }

            ParentObject["allowadd"] = false;

            if( TargetObjectClass != null && TargetObjectClass.CanAdd && // case 27189
                ( TargetNodeType == null || _CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Create, TargetNodeType ) ) )
            {
                ParentObject["allowadd"] = "true";
            }

            ParentObject["relatednodeid"] = default( string );
            if( null != RelatedNodeId && Int32.MinValue != RelatedNodeId.PrimaryKey )
            {
                ParentObject["relatednodeid"] = RelatedNodeId.ToString();
            }

            ParentObject["usesearch"] = false;
            Dictionary<CswPrimaryKey, string> Options = getOptions();
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
                    }
                    else
                    {
                        JOption["id"] = "";
                        JOption["value"] = "";
                    }
                    JOptions.Add( JOption );
                }
            }
        } // ToJSON()

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            // Getting the value as a string is on purpose.
            //RelatedNodeId = new CswPrimaryKey( "nodes", _HandleReference( CswXmlDocument.ChildXmlNodeValueAsInteger( XmlNode, _NodeIDSubField.ToXmlNodeName() ), NodeMap ) );
            Int32 NodeId = CswXmlDocument.ChildXmlNodeValueAsInteger( XmlNode, _NodeIDSubField.ToXmlNodeName() );
            if( NodeMap != null && NodeMap.ContainsKey( NodeId ) )
            {
                NodeId = NodeMap[NodeId];
            }
            RelatedNodeId = new CswPrimaryKey( "nodes", NodeId );
            if( null != RelatedNodeId )
            {
                CswXmlDocument.AppendXmlAttribute( XmlNode, "destnodeid", RelatedNodeId.PrimaryKey.ToString() );
                PendingUpdate = true;
            }
        }



        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            if( null != XmlNode.Element( _NodeIDSubField.ToXmlNodeName( true ) ) )
            {
                string NodePkString = XmlNode.Element( _NodeIDSubField.ToXmlNodeName( true ) ).Value;
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
                    XmlNode.Add( new XElement( "destnodeid", RelatedNodeId.PrimaryKey.ToString() ) );
                    PendingUpdate = true;
                }
            }
        }

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

    }//CswNbtNodePropRelationship

}//namespace ChemSW.Nbt.PropTypes

