using System;
using System.Collections.Generic;
using System.Data;
using ChemSW;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace NbtWebApp.Actions.Explorer
{
    public class CswNbtWebServiceNodeExplorer
    {
        private static int MAX_DEPTH = 1;
        private static HashSet<int> SEEN = new HashSet<int>();
        private static CswNbtNode StartingNode;

        public static void Initialize( ICswResources CswResources, CswNbtExplorerReturn Return, CswNbtExplorerRequest Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            CswPrimaryKey NodeId = CswConvert.ToPrimaryKey( Request.NodeId );

            MAX_DEPTH = Request.Depth;

            StartingNode = NbtResources.Nodes[NodeId];
            //Add the initial node to the graph
            _addToGraph( Return, StartingNode.NodeName, string.Empty, NodeId.ToString(), StartingNode.IconFileName, 0, "Instance" );

            _recurseForRelatingNodes( NbtResources, Return, StartingNode, 1 );

            //Get all NTs that have a relationship prop that relates to this node
            string relatingNTsToNodeSQL = _makeGetRelatedToNodeSQL( NodeId.PrimaryKey );
            _getRelating( NbtResources, Return, NodeId, relatingNTsToNodeSQL, 1 );
        }

        private static void _recurseForRelatingNodes( CswNbtResources NbtResources, CswNbtExplorerReturn Return, CswNbtNode Node, int Level )
        {
            CswNbtMetaDataNodeType TargetNodeType = Node.getNodeType();
            foreach( CswNbtMetaDataNodeTypeProp RelNTP in TargetNodeType.getNodeTypeProps( CswEnumNbtFieldType.Relationship ) ) //TODO: Locations are just like relationships, we should be able to handle them
            {
                CswNbtNodePropRelationship RelProp = Node.Properties[RelNTP];
                string Icon = _getIconFromRelationshipProp( NbtResources, RelNTP );
                if( CswTools.IsPrimaryKey( RelProp.RelatedNodeId ) )
                {
                    _addToGraph( Return, RelProp.PropName + ": " + RelProp.CachedNodeName, Node.NodeId.ToString(), RelProp.RelatedNodeId.ToString(), Icon, Level, "Instance" );

                    string relatingNTsToRelatedNodeSQL = _makeGetRelatedToNodeSQL( RelProp.RelatedNodeId.PrimaryKey );
                    if( Level + 1 <= MAX_DEPTH )
                    {
                        _getRelating( NbtResources, Return, RelProp.RelatedNodeId, relatingNTsToRelatedNodeSQL, Level + 1 );
                    }

                    if( Level + 1 <= MAX_DEPTH )
                    {
                        CswNbtNode TargetNode = NbtResources.Nodes[RelProp.RelatedNodeId];
                        _recurseForRelatingNodes( NbtResources, Return, TargetNode, Level + 1 );
                    }
                }
            }
        }

        private static string _getIconFromRelationshipProp( CswNbtResources NbtResources, CswNbtMetaDataNodeTypeProp RelationshipNTP )
        {
            string ret = string.Empty;
            if( RelationshipNTP.FKType == "ObjectClassId" )
            {
                CswNbtMetaDataObjectClass ObjClass = NbtResources.MetaData.getObjectClass( RelationshipNTP.FKValue );
                ret = ObjClass.IconFileName;
            }
            else
            {
                CswNbtMetaDataNodeType NodeType = NbtResources.MetaData.getNodeType( RelationshipNTP.FKValue );
                ret = NodeType.IconFileName;
            }
            return ret;
        }

        private static void _getRelating( CswNbtResources NbtResources, CswNbtExplorerReturn Return, CswPrimaryKey NodeId, string sql, int level )
        {
            CswArbitrarySelect relatingToNTsArbSel = NbtResources.makeCswArbitrarySelect( "getRelatingNTsToNode", sql );
            DataTable relatingToNTTbl = relatingToNTsArbSel.getTable();
            foreach( DataRow Row in relatingToNTTbl.Rows )
            {
                int RelatingNodeTypeId = CswConvert.ToInt32( Row["id"] );
                string IconFileName = CswConvert.ToString( Row["iconfilename"] );
                string DisplayName = CswConvert.ToString( Row["display"] ) + "s"; ;
                _addToGraph( Return, DisplayName, NodeId.ToString(), "NT_" + RelatingNodeTypeId, IconFileName, level, "Category" );

                _recurseForRelatedNTs( NbtResources, Return, RelatingNodeTypeId, level + 1 );
            }
        }

        private static void _recurseForRelatedNTs( CswNbtResources NbtResources, CswNbtExplorerReturn Return, int NodeTypeId, int level )
        {
            if( false == SEEN.Contains( NodeTypeId ) && level <= MAX_DEPTH )
            {
                //SEEN.Add( NodeTypeId );
                string relatingNTsToNTSQL = _makeGetRelatedNodeTypesNTSQL( NodeTypeId );
                CswArbitrarySelect relatingNTsToNTArbSel = NbtResources.makeCswArbitrarySelect( "getNTsRelatingToNT", relatingNTsToNTSQL );
                DataTable relatingNTstoNTTbl = relatingNTsToNTArbSel.getTable();
                foreach( DataRow Row in relatingNTstoNTTbl.Rows )
                {
                    int RelatingNodeTypeToNodeTypeId = CswConvert.ToInt32( Row["nodetypeid"] );
                    string IconFileName = CswConvert.ToString( Row["iconfilename"] );
                    string DisplayName = CswConvert.ToString( Row["nodetypename"] ) + "s"; ;

                    if( RelatingNodeTypeToNodeTypeId != StartingNode.NodeTypeId )
                    {
                        _addToGraph( Return, DisplayName, "NT_" + NodeTypeId, "NT_" + RelatingNodeTypeToNodeTypeId, IconFileName, level, "Category" );
                    }

                    if( level + 1 <= MAX_DEPTH )
                    {
                        _recurseForRelatedNTs( NbtResources, Return, RelatingNodeTypeToNodeTypeId, level + 1 );
                    }
                }

                string relatingOCsToNTSQL = _makeGetRelatedObjClassesOCSQL( NodeTypeId );
                CswArbitrarySelect relatingOCsToNTArbSel = NbtResources.makeCswArbitrarySelect( "getOCsRelatingToNT", relatingOCsToNTSQL );
                DataTable relatingOCstoNTTbl = relatingOCsToNTArbSel.getTable();
                foreach( DataRow Row in relatingOCstoNTTbl.Rows )
                {
                    int RelatingObjClassToNodeTypeId = CswConvert.ToInt32( Row["objectclassid"] );
                    string IconFileName = CswConvert.ToString( Row["iconfilename"] );
                    string DisplayName = CswConvert.ToString( Row["objectclass"] ).Replace( "Class", "" ) + "s";

                    if( RelatingObjClassToNodeTypeId != StartingNode.getObjectClassId() )
                    {
                        _addToGraph( Return, DisplayName, "NT_" + NodeTypeId, "OC_" + RelatingObjClassToNodeTypeId, IconFileName, level, "Category" );
                    }

                    //TODO: find OCs that relate to the current OC. After depth 3, most MetaData objects are related on the obj class level
                }
            }
        }

        /// <summary>
        /// Helper method for adding data to the return object
        /// </summary>
        private static void _addToGraph( CswNbtExplorerReturn Return, string Label, string OwnerId, string TargetId, string Icon, int level, string Type )
        {
            Return.Data.Nodes.Add( new CswNbtArborNode()
                {
                    NodeIdStr = TargetId,
                    Data = new CswNbtArborNode.CswNbtArborNodeData()
                        {
                            Icon = "Images/newicons/100/" + Icon,
                            Label = Label,
                            NodeId = TargetId,
                            Level = level,
                            Type = Type
                        }
                } );

            if( false == String.IsNullOrEmpty( OwnerId ) && false == String.IsNullOrEmpty( TargetId ) )
            {
                Return.Data.Edges.Add( new CswNbtArborEdge()
                    {
                        OwnerNodeIdStr = OwnerId,
                        TargetNodeIdStr = TargetId,
                        Data = new CswNbtArborEdge.CswNbtArborEdgeData()
                            {
                                Length = level * 10
                            }
                    } );
            }
        }

        /// <summary>
        /// Generates SQL to get all NTs that have a relationship prop that relates to this node
        /// </summary>
        private static string _makeGetRelatedToNodeSQL( int NodeId )
        {
            return @"select distinct nt.nodetypeid id, nt.nodetypename display, nt.iconfilename from jct_nodes_props jnp
                                                              join nodes n on n.nodeid = jnp.nodeid
                                                              join nodetypes nt on n.nodetypeid = nt.nodetypeid
                                                       where jnp.field1_fk = " + NodeId + @" and nodetypepropid in 
                                                              (select nodetypepropid from nodetype_props ntp where ntp.fieldtypeid = 
                                                                      (select fieldtypeid from field_types ft where ft.fieldtype = 'Relationship'))";
        }

        private static string _makeGetRelatedNodeTypesNTSQL( int NodeTypeId )
        {
            return @"select ntp.propname, ntp.fieldtypeid, nt.nodetypename, nt.iconfilename, nt.nodetypeid from nodetype_props ntp
                            join nodetypes nt on ntp.nodetypeid = nt.nodetypeid
                            join field_types ft on ntp.fieldtypeid = ft.fieldtypeid
                     where ntp.fkvalue = " + NodeTypeId + " and ft.fieldtype = 'Relationship' and ntp.fktype = 'NodeTypeId'";
        }

        private static string _makeGetRelatedObjClassesOCSQL( int NodeTypeId )
        {
            return @"select oc.objectclass, oc.objectclassid, oc.iconfilename from object_class oc
                            join nodetypes nt on nt.objectclassid = nt.objectclassid and nt.nodetypeid = " + NodeTypeId +
                            @" join nodetype_props ntp on ntp.nodetypeid = nt.nodetypeid
                            join field_types ft on ft.fieldtypeid = ntp.fieldtypeid
                     where ntp.fkvalue = oc.objectclassid and ntp.fktype = 'ObjectClassId' and ft.fieldtype = 'Relationship'";
        }


    }
}