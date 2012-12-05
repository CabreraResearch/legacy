//using System;
//using System.Collections;
//using System.Text;
//using System.Data;
//using System.Xml;
//using ChemSW.Exceptions;
//using ChemSW.Nbt.MetaData;
//using ChemSW.Nbt.ObjClasses;
//using ChemSW.Core;

//namespace ChemSW.Nbt.PropTypes
//{
//    public class CswNbtNodePropMultiRelationship : CswNbtNodeProp
//    {
//        private XmlDocument RelationshipXmlDoc;

//        public CswNbtNodePropMultiRelationship( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
//            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
//        {
//            RelationshipXmlDoc = new XmlDocument();
//            if( Empty )
//            {
//                XmlElement RelationshipRootNode = RelationshipXmlDoc.CreateElement( "Relationship" );
//                RelationshipXmlDoc.AppendChild( RelationshipRootNode );
//            }
//            else
//            {
//                RelationshipXmlDoc.LoadXml( Gestalt );
//            }

//        }

//        override public bool Empty
//        {
//            get
//            {
//                return ( 0 == Gestalt.Length );
//            }
//        }


//        override public string Gestalt
//        {
//            get
//            {
//                return _CswNbtNodePropData.Gestalt;
//            }

//        }//Gestalt

//        public void Save()
//        {
//            _CswNbtNodePropData.Gestalt = RelationshipXmlDoc.InnerXml.ToString();
//        }

//        public CswNbtView View
//        {
//            get
//            {
//                CswNbtView Ret = null;
//                if( _CswNbtMetaDataNodeTypeProp.ViewId != Int32.MinValue )
//                    Ret = (CswNbtView) CswNbtViewFactory.restoreView( _CswNbtResources, _CswNbtMetaDataNodeTypeProp.ViewId );
//                return Ret;
//            }
//        }

//        public void AddRelatedNode( Int32 NodeId, string NodeName )
//        {
//            XmlNode RelatedNode = RelationshipXmlDoc.ChildNodes[0].SelectSingleNode( "RelatedNode[@nodeid='" + NodeId.ToString() + "']" );
//            if( RelatedNode == null )
//            {
//                RelatedNode = RelationshipXmlDoc.CreateElement( "RelatedNode" );
//                XmlAttribute NodeIdAttribute = RelationshipXmlDoc.CreateAttribute( "nodeid" );
//                NodeIdAttribute.Value = NodeId.ToString();
//                RelatedNode.Attributes.Append( NodeIdAttribute );

//                XmlAttribute NodeNameAttribute = RelationshipXmlDoc.CreateAttribute( "nodename" );
//                NodeNameAttribute.Value = NodeName;
//                RelatedNode.Attributes.Append( NodeNameAttribute );

//                RelationshipXmlDoc.ChildNodes[0].AppendChild( RelatedNode );
//            }
//        }

//        public void RemoveRelatedNode( Int32 NodeId )
//        {
//            XmlNode RelatedNode = RelationshipXmlDoc.ChildNodes[0].SelectSingleNode( "RelatedNode[@nodeid='" + NodeId.ToString() + "']" );
//            if( RelatedNode != null )
//            {
//                RelatedNode.ParentNode.RemoveChild( RelatedNode );
//            }
//        }

//        public bool IsRelated( Int32 NodeId )
//        {
//            bool ret = false;
//            XmlNode RelatedNode = RelationshipXmlDoc.ChildNodes[0].SelectSingleNode( "RelatedNode[@nodeid='" + NodeId.ToString() + "']" );
//            if( RelatedNode != null )
//                ret = true;
//            return ret;
//        }

//        public string CommaSeparatedNodeNames
//        {
//            get
//            {
//                string ret = string.Empty;
//                foreach( XmlNode CurrentNode in RelationshipXmlDoc.ChildNodes[0].ChildNodes )
//                {
//                    if( ret != string.Empty )
//                        ret += ", ";
//                    ret += CurrentNode.Attributes["nodename"].Value;
//                }
//                return ret;
//            }
//        }


//        public RelatedIdType TargetType
//        {
//            get
//            {
//                RelatedIdType ret = RelatedIdType.Unknown;
//                try
//                {
//                    ret = (RelatedIdType) Enum.Parse( typeof( RelatedIdType ), _CswNbtMetaDataNodeTypeProp.FKType, true );
//                }
//                catch( Exception ex )
//                {
//                    if( !( ex is System.ArgumentException ) )
//                        throw ( ex );
//                }
//                return ret;
//            }
//            //set
//            //{
//            //    _CswNbtMetaDataNodeTypeProp.FKType = value.ToString();
//            //}
//        }
//        public Int32 TargetId
//        {
//            get
//            {
//                return _CswNbtMetaDataNodeTypeProp.FKValue;
//            }
//            //set
//            //{
//            //    _CswNbtMetaDataNodeTypeProp.FKValue = value;
//            //}
//        }

//        public void RefreshNodeNames()
//        {
//            foreach( XmlNode CurrentNode in RelationshipXmlDoc.ChildNodes[0].ChildNodes )
//            {
//                CurrentNode.Attributes["nodename"].Value = _CswNbtResources.Nodes.GetNode( CswConvert.ToInt32( CurrentNode.Attributes["nodeid"].Value ) ).NodeName;
//            }
//        }

//        private string _ElemName_Relatedxml = "Relatedxml";
//        private string _ElemName_CachedNodeNames = "CachedNodeNames";

//        public override void ToXml( XmlNode ParentNode )
//        {
//            XmlNode RelatedXmlNode = CswXmlDocument.AppendXmlNode( ParentNode, _ElemName_Relatedxml );
//            CswXmlDocument.SetInnerTextAsCData( RelatedXmlNode, RelationshipXmlDoc.InnerXml.ToString() );
//            XmlNode CachedNodeNamesNode = CswXmlDocument.AppendXmlNode( ParentNode, _ElemName_CachedNodeNames, CommaSeparatedNodeNames );
//        }
//        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
//        {
//            _HandleReferences( CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _ElemName_Relatedxml ), NodeMap );
//        }
//        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
//        {
//            _HandleReferences( PropRow[_ElemName_Relatedxml].ToString(), NodeMap );
//        }


//        private void _HandleReferences( string RelatedXml, Dictionary<Int32, Int32> NodeMap )
//        {
//            if( RelatedXml != string.Empty )
//            {
//                XmlDocument TempRelationshipXmlDoc = new XmlDocument();
//                TempRelationshipXmlDoc.LoadXml( RelatedXml );

//                if( NodeMap != null )
//                {
//                    RelationshipXmlDoc.RemoveAll();
//                    foreach( XmlNode RelatedNode in TempRelationshipXmlDoc.ChildNodes[0].ChildNodes )
//                    {
//                        string thisNodeId = RelatedNode.Attributes["nodeid"].Value;
//                        if( NodeMap.ContainsKey( thisNodeId ) )
//                            AddRelatedNode( CswConvert.ToInt32( NodeMap[thisNodeId].ToString() ), "" );
//                        else if( CswTools.IsInteger( thisNodeId ) )
//                            AddRelatedNode( CswConvert.ToInt32( thisNodeId ), _CswNbtResources.Nodes.GetNode( CswConvert.ToInt32( thisNodeId ) ).NodeName );
//                    }
//                }
//                else
//                {
//                    RelationshipXmlDoc = TempRelationshipXmlDoc;
//                }
//            }
//            PendingUpdate = true;
//        }



//    }//CswNbtNodePropMultiRelationship

//}//namespace ChemSW.Nbt.PropTypes
