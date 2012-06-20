using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt
{
    public class CswNbtLocationTree
    {
        public const string TopLevelName = "Top";

        public const string XmlNodeName_ChildSet = "Nodes";   // Denotes the set of children under a node
        public const string XmlNodeName_Child = "Node";       // Denotes a particular non-Location child
        public const string XmlNodeName_Key = "Key";         // Key node for a given child
        public const string XmlNodeName_Display = "Name";     // Display node for a given child
        public const string XmlNodeName_ObjectClass = "ObjectClass";  // Denotes the child's object class
        public const string XmlNodeName_IconFileName = "IconFileName";  // Denotes the icon used for a node
        public const string XmlNodeName_Order = "Order";      // Denotes the order value for a given child
        public const string XmlAttrName_LocationTemplate = "LocationTemplate";  // Attribute which holds the template for a given child
        public const string XmlAttrName_LocationType = "LocationType";  // Attribute which holds the type for the children
        public const string XmlNodeName_Row = "Row";          // Denotes the set of children in a grid row
        public const string XmlNodeName_Cell = "Cell";        // Denotes the set of children in a grid cell of a grid row

        #region Enums

        public sealed class LocationType
        {
            private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
                                                                   {
                                                                       { Horizontal, Horizontal },
                                                                       { Vertical, Vertical },
                                                                       { Grid, Grid }
                                                                   };

            public readonly string Value;

            private static string _Parse( string Val )
            {
                string ret = CswNbtResources.UnknownEnum;
                if( _Enums.ContainsKey( Val ) )
                {
                    ret = _Enums[Val];
                }
                return ret;
            }
            public LocationType( string ItemName = CswNbtResources.UnknownEnum )
            {
                Value = _Parse( ItemName );
            }

            public static implicit operator LocationType( string Val )
            {
                return new LocationType( Val );
            }
            public static implicit operator string( LocationType item )
            {
                return item.Value;
            }

            public override string ToString()
            {
                return Value;
            }

            public const string Horizontal = "Horizontal";
            public const string Vertical = "Vertical";
            public const string Grid = "Grid";
        };

        public sealed class LocationTemplate
        {
            private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
                                                                   {
                                                                       { Building, Building },
                                                                       { ShortBox, ShortBox },
                                                                       { MediumBox, MediumBox },
                                                                       { TallBox, TallBox },
                                                                       { Door, Door },
                                                                       { Fridge, Fridge },
                                                                       { Cabinet, Cabinet },
                                                                       { SafetyCabinet, SafetyCabinet },
                                                                       { Shelf, Shelf },
                                                                       { Grid, Grid },
                                                                       { Slot, Slot },
                                                                       { Empty, Empty }
                                                                   };

            public readonly string Value;

            private static string _Parse( string Val )
            {
                string ret = CswResources.UnknownEnum;
                if( _Enums.ContainsKey( Val ) )
                {
                    ret = _Enums[Val];
                }
                return ret;
            }
            public LocationTemplate( string ItemName = CswResources.UnknownEnum )
            {
                Value = _Parse( ItemName );
            }

            public static implicit operator LocationTemplate( string Val )
            {
                return new LocationTemplate( Val );
            }
            public static implicit operator string( LocationTemplate item )
            {
                return item.Value;
            }

            public override string ToString()
            {
                return Value;
            }

            public const string Building = "Building";
            public const string ShortBox = "ShortBox";
            public const string MediumBox = "MediumBox";
            public const string TallBox = "TallBox";
            public const string Door = "Door";
            public const string Fridge = "Fridge";
            public const string Cabinet = "Cabinet";
            public const string SafetyCabinet = "SafetyCabinet";
            public const string Shelf = "Shelf";
            public const string Grid = "Grid";
            public const string Slot = "Slot";
            public const string Empty = "Empty";
        };

        public sealed class HorizontalLocationTemplate
        {
            private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
                                                                   {
                                                                       { Building, Building },
                                                                       { ShortBox, ShortBox },
                                                                       { MediumBox, MediumBox },
                                                                       { TallBox, TallBox },
                                                                       { Door, Door },
                                                                       { Fridge, Fridge },
                                                                       { Cabinet, Cabinet },
                                                                       { SafetyCabinet, SafetyCabinet },
                                                                       { Slot, Slot },
                                                                       { Empty, Empty }
                                                                   };

            public readonly string Value;

            private static string _Parse( string Val )
            {
                string ret = CswResources.UnknownEnum;
                if( _Enums.ContainsKey( Val ) )
                {
                    ret = _Enums[Val];
                }
                return ret;
            }
            public HorizontalLocationTemplate( string ItemName = CswResources.UnknownEnum )
            {
                Value = _Parse( ItemName );
            }

            public static implicit operator HorizontalLocationTemplate( string Val )
            {
                return new HorizontalLocationTemplate( Val );
            }
            public static implicit operator string( HorizontalLocationTemplate item )
            {
                return item.Value;
            }

            public override string ToString()
            {
                return Value;
            }

            public const string Building = LocationTemplate.Building;
            public const string ShortBox = LocationTemplate.ShortBox;
            public const string MediumBox = LocationTemplate.MediumBox;
            public const string TallBox = LocationTemplate.TallBox;
            public const string Door = LocationTemplate.Door;
            public const string Fridge = LocationTemplate.Fridge;
            public const string Cabinet = LocationTemplate.Cabinet;
            public const string SafetyCabinet = LocationTemplate.SafetyCabinet;
            public const string Slot = LocationTemplate.Slot;
            public const string Empty = LocationTemplate.Empty;
        };

        public sealed class VerticalLocationTemplate
        {
            private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
                                                                   {
                                                                       { Grid, Grid },
                                                                       { Shelf, Shelf },
                                                                       { Slot, Slot },
                                                                       { Empty, Empty }
                                                                   };

            public readonly string Value;

            private static string _Parse( string Val )
            {
                string ret = CswResources.UnknownEnum;
                if( _Enums.ContainsKey( Val ) )
                {
                    ret = _Enums[Val];
                }
                return ret;
            }
            public VerticalLocationTemplate( string ItemName = CswResources.UnknownEnum )
            {
                Value = _Parse( ItemName );
            }

            public static implicit operator VerticalLocationTemplate( string Val )
            {
                return new VerticalLocationTemplate( Val );
            }
            public static implicit operator string( VerticalLocationTemplate item )
            {
                return item.Value;
            }

            public override string ToString()
            {
                return Value;
            }

            public const string Grid = LocationTemplate.Grid;
            public const string Shelf = LocationTemplate.Shelf;
            public const string Slot = LocationTemplate.Slot;
            public const string Empty = LocationTemplate.Empty;
        };
        
        public sealed class GridLocationTemplate
        {
            private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
                                                                   {
                                                                       { Grid, Grid },
                                                                       { Empty, Empty }
                                                                   };

            public readonly string Value;

            private static string _Parse( string Val )
            {
                string ret = CswResources.UnknownEnum;
                if( _Enums.ContainsKey( Val ) )
                {
                    ret = _Enums[Val];
                }
                return ret;
            }
            public GridLocationTemplate( string ItemName = CswResources.UnknownEnum )
            {
                Value = _Parse( ItemName );
            }

            public static implicit operator GridLocationTemplate( string Val )
            {
                return new GridLocationTemplate( Val );
            }
            public static implicit operator string( GridLocationTemplate item )
            {
                return item.Value;
            }

            public override string ToString()
            {
                return Value;
            }

            public const string Grid = LocationTemplate.Grid;
            public const string Empty = LocationTemplate.Empty;
        };
        
        #endregion Enums

        /// <summary>
        /// Deprecated
        /// </summary>
        public XmlDocument LocationTreeXml;
        private CswNbtResources _CswNbtResources;

        private void _init( CswPrimaryKey StartingNodeId, string StartingNodeName, Int32 DepthToInit, string NodeIdsToFilterOut )
        {
            CswTimer Timer = new CswTimer();
            LocationTreeXml = new XmlDocument();
            if( StartingNodeId != null )
            {
                addLocationXmlNode( null, "LocationClass", StartingNodeId, StartingNodeName, "", NodeIdsToFilterOut, DepthToInit );
            }
            else
            {
                addLocationXmlNode( null, "LocationClass", null, TopLevelName, "", NodeIdsToFilterOut, DepthToInit );
            }

            string StartingNodeIdString = String.Empty;
            if( StartingNodeId != null )
            {
                StartingNodeIdString = StartingNodeId.ToString();
            }

            _CswNbtResources.logTimerResult( "Built location tree with starting node " + StartingNodeName + " (" + StartingNodeIdString + ")", Timer.ElapsedDurationInSecondsAsString );
        }

        public CswNbtLocationTree( CswNbtResources Rsc, CswPrimaryKey StartingNodeId, string StartingNodeName, Int32 DepthToInit, string NodeIdsToFilterOut )
        {
            _CswNbtResources = Rsc;
            _init( StartingNodeId, StartingNodeName, DepthToInit, NodeIdsToFilterOut );
        }

        public CswNbtLocationTree( CswNbtResources Rsc, CswNbtNodePropLocation StartLocation, Int32 DepthToInit, string NodeIdsToFilterOut = "" )
        {
            _CswNbtResources = Rsc;
            if( null == StartLocation )
            {
                throw new CswDniException( ErrorType.Error, "Location Tree requires a starting Location", "Cannot instance CswNbtLocationTree with a null StartLocation." );
            }
            _init( StartLocation.NodeId, StartLocation.CachedNodeName, DepthToInit, NodeIdsToFilterOut );
        }

        /// <summary>
        /// Deprecated
        /// </summary>
        private XmlNode makeGenericXmlNode( string ObjectClass, CswPrimaryKey NodeId, string NodeName, string IconFileName )
        {
            XmlNode ThisNode = LocationTreeXml.CreateElement( XmlNodeName_Child );

            XmlNode NameNode = LocationTreeXml.CreateElement( XmlNodeName_Display );
            ThisNode.AppendChild( NameNode );
            NameNode.InnerText = NodeName;

            XmlNode KeyNode = LocationTreeXml.CreateElement( XmlNodeName_Key );
            ThisNode.AppendChild( KeyNode );
            if( NodeId != null )
                KeyNode.InnerText = NodeId.PrimaryKey.ToString();
            else
                KeyNode.InnerText = "";

            XmlNode IconNode = LocationTreeXml.CreateElement( XmlNodeName_IconFileName );
            ThisNode.AppendChild( IconNode );
            IconNode.InnerText = IconFileName;

            XmlNode ObjectClassNode = LocationTreeXml.CreateElement( XmlNodeName_ObjectClass );
            ThisNode.AppendChild( ObjectClassNode );
            ObjectClassNode.InnerText = ObjectClass;

            return ThisNode;
        }

        /// <summary>
        /// Deprecated
        /// </summary>
        private void addLocationXmlNode( XmlNode ParentNode, string ObjectClass, CswPrimaryKey NodeId, string NodeName, string IconFileName, string NodeIdsToFilterOut, Int32 Depth )
        {
            CswTimer Timer = new CswTimer();
            if( Depth > 0 )
            {
                // Get some location-specific info about this node
                LocationTemplate Template = LocationTemplate.Unknown;
                LocationType ChildType = LocationType.Unknown;
                Int32 Order = 1;
                Int32 Rows = Int32.MinValue;
                Int32 Columns = Int32.MinValue;

                if( NodeId == null )
                {
                    // Top node
                    Template = LocationTemplate.MediumBox;
                    ChildType = LocationType.Horizontal;
                    Order = 1;
                    Rows = Int32.MinValue;
                    Columns = Int32.MinValue;
                }
                else
                {
                    string PropSql = @"select op.propname, j.field1
    from nodes n
    join nodetypes t on n.nodetypeid = t.nodetypeid
    join nodetype_props p on t.nodetypeid = p.nodetypeid
    join object_class_props op on p.objectclasspropid = op.objectclasspropid
    left outer join jct_nodes_props j on n.nodeid = j.nodeid and p.nodetypepropid = j.nodetypepropid
    where n.nodeid = " + NodeId.PrimaryKey.ToString();

                    CswArbitrarySelect PropSelect = _CswNbtResources.makeCswArbitrarySelect( "addLocationXmlNode_select", PropSql );
                    DataTable PropTable = null;
                    try
                    {
                        PropTable = PropSelect.getTable();
                    }
                    catch( Exception ex )
                    {
                        throw new CswDniException( ErrorType.Error, "Invalid View", "getLocationXml() attempted to run invalid SQL: " + PropSql, ex );
                    }

                    foreach( DataRow PropRow in PropTable.Rows )
                    {
                        if( PropRow["propname"].ToString() == "Location Template" )
                        {
                            if( PropRow["field1"] != null && PropRow["field1"].ToString() != String.Empty )
                                Template = (LocationTemplate) Enum.Parse( typeof( LocationTemplate ), PropRow["field1"].ToString(), true );
                        }
                        if( PropRow["propname"].ToString() == "Child Location Type" )
                        {
                            if( PropRow["field1"] != null && PropRow["field1"].ToString() != String.Empty )
                                ChildType = (LocationType) Enum.Parse( typeof( LocationType ), PropRow["field1"].ToString(), true );
                        }
                        if( PropRow["propname"].ToString() == "Order" )
                        {
                            if( PropRow["field1"] != null && PropRow["field1"].ToString() != String.Empty )
                                Order = CswConvert.ToInt32( PropRow["field1"].ToString() );
                        }
                        if( PropRow["propname"].ToString() == "Rows" )
                        {
                            if( PropRow["field1"] != null && PropRow["field1"].ToString() != String.Empty )
                                Rows = CswConvert.ToInt32( PropRow["field1"].ToString() );
                        }
                        if( PropRow["propname"].ToString() == "Columns" )
                        {
                            if( PropRow["field1"] != null && PropRow["field1"].ToString() != String.Empty )
                                Columns = CswConvert.ToInt32( PropRow["field1"].ToString() );
                        }
                    }
                }

                //XmlNode ThisNode = LocationTreeXml.CreateElement(XmlNodeName_Child);
                XmlNode ThisNode = makeGenericXmlNode( "LocationClass", NodeId, NodeName, IconFileName );
                bool bDidInsert = false;
                if( ParentNode != null )
                {
                    // Insertion sort
                    foreach( XmlNode Sibling in ParentNode.ChildNodes )
                    {
                        if( Sibling.SelectSingleNode( XmlNodeName_Order ) != null &&
                            Sibling.SelectSingleNode( XmlNodeName_Order ).InnerText != String.Empty &&
                            Order < CswConvert.ToInt32( Sibling.SelectSingleNode( XmlNodeName_Order ).InnerText ) )
                        {
                            ParentNode.InsertBefore( ThisNode, Sibling );
                            bDidInsert = true;
                            break;
                        }
                    }
                    if( !bDidInsert )
                        ParentNode.AppendChild( ThisNode );
                }
                else
                {
                    // Insertion sort
                    foreach( XmlNode Sibling in LocationTreeXml.ChildNodes )
                    {
                        if( Sibling.SelectSingleNode( XmlNodeName_Order ) != null &&
                            Sibling.SelectSingleNode( XmlNodeName_Order ).InnerText != String.Empty &&
                            Order < CswConvert.ToInt32( Sibling.SelectSingleNode( XmlNodeName_Order ).InnerText ) )
                        {
                            LocationTreeXml.InsertBefore( ThisNode, Sibling );
                            bDidInsert = true;
                            break;
                        }
                    }
                    if( !bDidInsert )
                        LocationTreeXml.AppendChild( ThisNode );
                }

                XmlAttribute TemplateAttribute = LocationTreeXml.CreateAttribute( XmlAttrName_LocationTemplate );
                ThisNode.Attributes.Append( TemplateAttribute );
                TemplateAttribute.Value = Template.ToString();

                //XmlNode NameNode = LocationTreeXml.CreateElement(XmlNodeName_Display);
                //ThisNode.AppendChild(NameNode);
                //NameNode.InnerText = NodeName;

                //XmlNode KeyNode = LocationTreeXml.CreateElement(XmlNodeName_Key);
                //ThisNode.AppendChild(KeyNode);
                //KeyNode.InnerText = NodeId.PrimaryKey.ToString();

                XmlNode OrderNode = LocationTreeXml.CreateElement( XmlNodeName_Order );
                ThisNode.AppendChild( OrderNode );
                OrderNode.InnerText = Order.ToString();

                XmlNode ChildNodes = LocationTreeXml.CreateElement( XmlNodeName_ChildSet );
                ThisNode.AppendChild( ChildNodes );

                XmlAttribute ChildTypeAttribute = LocationTreeXml.CreateAttribute( XmlAttrName_LocationType );
                ChildNodes.Attributes.Append( ChildTypeAttribute );
                ChildTypeAttribute.Value = ChildType.ToString();

                string LocationNodeVal = "None";
                if( NodeId != null )
                    LocationNodeVal = NodeId.ToString();

                _CswNbtResources.logTimerResult( "Fetched Location node: " + LocationNodeVal, Timer.ElapsedDurationInSecondsAsString );

                if( ChildType != LocationType.Grid )
                {
                    addLocationChildren( ChildNodes, NodeId, Int32.MinValue, Int32.MinValue, ChildType, NodeIdsToFilterOut, Depth );
                }
                else
                {
                    if( Rows < 1 ) Rows = 1;
                    if( Columns < 1 ) Columns = 1;

                    for( Int32 r = 0; r < Rows; r++ )
                    {
                        XmlNode RowNode = LocationTreeXml.CreateElement( XmlNodeName_Row );
                        ChildNodes.AppendChild( RowNode );
                        for( Int32 c = 0; c < Columns; c++ )
                        {
                            XmlNode ColumnNode = LocationTreeXml.CreateElement( XmlNodeName_Cell );
                            RowNode.AppendChild( ColumnNode );

                            XmlAttribute TemplateXmlAttribute = LocationTreeXml.CreateAttribute( XmlAttrName_LocationTemplate );
                            ColumnNode.Attributes.Append( TemplateXmlAttribute );
                            TemplateXmlAttribute.Value = LocationTemplate.Grid.ToString();

                            addLocationChildren( ColumnNode, NodeId, r, c, ChildType, NodeIdsToFilterOut, Depth );
                        }
                    }
                }

            } // if(Depth > 0)
        } // addLocationXmlNode()

        private void addLocationChildren( XmlNode ChildNodeSet, CswPrimaryKey ParentNodeId, Int32 Row, Int32 Column, LocationType ChildType, string NodeIdsToFilterOut, Int32 Depth )
        {
            // Find children
            string ChildLocationsSql = @"
select n.nodeid, n.nodename, o.objectclass, t.iconfilename
  from nodes n
  join nodetypes t on n.nodetypeid = t.nodetypeid
  join object_class o on t.objectclassid = o.objectclassid
  join nodetype_props p on t.nodetypeid = p.nodetypeid
  join field_types f on (p.fieldtypeid = f.fieldtypeid)
  left outer join jct_nodes_props j on n.nodeid = j.nodeid and p.nodetypepropid = j.nodetypepropid
 where f.fieldtype = 'Location'";
            if( ParentNodeId != null && ParentNodeId.TableName == "nodes" )
                ChildLocationsSql += "   and j.field1_fk = " + ParentNodeId.PrimaryKey.ToString();
            else
                ChildLocationsSql += "   and j.field1_fk is null";

            if( Row > Int32.MinValue )
                ChildLocationsSql += "  and j.field2 = " + Row.ToString() + " ";
            if( Column > Int32.MinValue )
                ChildLocationsSql += "  and j.field3 = " + Column.ToString() + " ";

            if( NodeIdsToFilterOut != string.Empty )
                ChildLocationsSql += "  and n.nodeid not in (" + NodeIdsToFilterOut + ") ";
            ChildLocationsSql += " order by n.nodeid";

            CswArbitrarySelect ChildLocationsSelect = _CswNbtResources.makeCswArbitrarySelect( "LocationTree_childlocation_select", ChildLocationsSql );
            DataTable ChildLocationsTable = null;
            try
            {
                ChildLocationsTable = ChildLocationsSelect.getTable();
            }
            catch( Exception ex )
            {
                throw new CswDniException( ErrorType.Error, "Invalid View", "getLocationXmlRecursive() attempted to run invalid SQL: " + ChildLocationsSql, ex );
            }

            if( ChildLocationsTable.Rows.Count > 0 )
            {
                foreach( DataRow ChildRow in ChildLocationsTable.Rows )
                {
                    if( ChildRow["objectclass"].ToString() == "LocationClass" )
                    {
                        addLocationXmlNode( ChildNodeSet, ChildRow["objectclass"].ToString(), new CswPrimaryKey( "nodes", CswConvert.ToInt32( ChildRow["nodeid"] ) ), ChildRow["nodename"].ToString(), ChildRow["iconfilename"].ToString(), NodeIdsToFilterOut, Depth - 1 );
                    }
                    else
                    {
                        XmlNode Node = makeGenericXmlNode( ChildRow["objectclass"].ToString(), new CswPrimaryKey( "nodes", CswConvert.ToInt32( ChildRow["nodeid"] ) ), ChildRow["nodename"].ToString(), ChildRow["iconfilename"].ToString() );
                        ChildNodeSet.AppendChild( Node );
                    }
                }
            }
        } // addLocationChildren()

    }
}
