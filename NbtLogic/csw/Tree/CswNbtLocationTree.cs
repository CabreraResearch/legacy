using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt
{
    public class CswNbtLocationTree
    {
        public const string TopLevelName = "Top";

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
                
        #endregion Enums

        private CswNbtResources _CswNbtResources;

        private void _init( CswNbtNodePropLocation StartLocation, Int32 DepthToInit, string NodeIdsToFilterOut )
        {
            CswTimer Timer = new CswTimer();
            string StartingNodeIdString = string.Empty;
            string StartingNodeName = string.Empty;
            if( StartLocation != null )
            {
                StartingNodeIdString = StartLocation.NodeId.ToString();
                StartingNodeName = StartLocation.CachedNodeName;
                populateLocations( StartLocation.NodeId, StartLocation.CachedNodeName, NodeIdsToFilterOut, DepthToInit );
            }
            else
            {
                populateLocations( null, TopLevelName, NodeIdsToFilterOut, DepthToInit );
            }

            _CswNbtResources.logTimerResult( "Built location tree with starting node " + StartingNodeName + " (" + StartingNodeIdString + ")", Timer.ElapsedDurationInSecondsAsString );
        }

        public CswNbtLocationTree( CswNbtResources Rsc, CswNbtNodePropLocation StartLocation, Int32 DepthToInit, string NodeIdsToFilterOut = "" )
        {
            _CswNbtResources = Rsc;
            _init( StartLocation, DepthToInit, NodeIdsToFilterOut );
        }

        public Dictionary<CswPrimaryKey, string> Locations = new Dictionary<CswPrimaryKey, string>();
        public CswCommaDelimitedString LocationIds = new CswCommaDelimitedString();
        /// <summary>
        /// Deprecated
        /// </summary>
        private void populateLocations( CswPrimaryKey NodeId, string NodeName, string NodeIdsToFilterOut, Int32 Depth )
        {
            CswTimer Timer = new CswTimer();
            if( Depth > 0 )
            {
                // Get some location-specific info about this node
                LocationType ChildType = CswNbtResources.UnknownEnum;
                Int32 Rows = Int32.MinValue;
                Int32 Columns = Int32.MinValue;

                if( NodeId == null )
                {
                    ChildType = LocationType.Horizontal;
                    Rows = Int32.MinValue;
                    Columns = Int32.MinValue;
                }
                else
                {
                    if( false == Locations.ContainsKey( NodeId ) )
                    {
                        Locations.Add( NodeId, NodeName );
                        LocationIds.Add( NodeId.PrimaryKey.ToString() );
                    }
                    string PropSql = @"select op.propname, j.field1
                                    from nodes n
                                    join nodetypes t on n.nodetypeid = t.nodetypeid
                                    join nodetype_props p on t.nodetypeid = p.nodetypeid
                                    join object_class_props op on p.objectclasspropid = op.objectclasspropid
                                    left outer join jct_nodes_props j on n.nodeid = j.nodeid and p.nodetypepropid = j.nodetypepropid
                                    where n.nodeid = " + NodeId.PrimaryKey.ToString();

                    CswArbitrarySelect PropSelect = _CswNbtResources.makeCswArbitrarySelect( "populateLocations_select", PropSql );
                    DataTable PropTable = null;
                    try
                    {
                        PropTable = PropSelect.getTable();
                    }
                    catch( Exception ex )
                    {
                        throw new CswDniException( ErrorType.Error, "Invalid View", "populateLocations() attempted to run invalid SQL: " + PropSql, ex );
                    }

                    foreach( DataRow PropRow in PropTable.Rows )
                    {
                        string PropName = CswConvert.ToString( PropRow["propname"] );
                        switch( PropName )
                        {
                            case "Child Location Type":
                                if( false == string.IsNullOrEmpty( CswConvert.ToString( PropRow["field1"] ) ) )
                                {
                                    ChildType = CswConvert.ToString( PropRow["field1"] );
                                }
                                break;
                            case "Rows":
                                Rows = CswConvert.ToInt32( PropRow["field1"] );
                                break;
                            case "Columns":
                                Columns = CswConvert.ToInt32( PropRow["field1"] );
                                break;

                        }
                    }
                }

                string LocationNodeVal = "None";
                if( NodeId != null )
                    LocationNodeVal = NodeId.ToString();

                _CswNbtResources.logTimerResult( "Fetched Location node: " + LocationNodeVal, Timer.ElapsedDurationInSecondsAsString );

                if( ChildType != LocationType.Grid )
                {
                    addLocationChildren( NodeId, Int32.MinValue, Int32.MinValue, NodeIdsToFilterOut, Depth );
                }
                else
                {
                    if( Rows < 1 ) Rows = 1;
                    if( Columns < 1 ) Columns = 1;

                    for( Int32 r = 0; r < Rows; r += 1 )
                    {
                        for( Int32 c = 0; c < Columns; c += 1 )
                        {
                            addLocationChildren( NodeId, r, c, NodeIdsToFilterOut, Depth );
                        }
                    }
                }

            } // if(Depth > 0)
        } // populateLocations()

        private void addLocationChildren( CswPrimaryKey ParentNodeId, Int32 Row, Int32 Column, string NodeIdsToFilterOut, Int32 Depth )
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
            {
                ChildLocationsSql += "   and j.field1_fk = " + ParentNodeId.PrimaryKey.ToString();
            }
            else
            {
                ChildLocationsSql += "   and j.field1_fk is null";
            }

            if( Row > Int32.MinValue )
            {
                ChildLocationsSql += "  and j.field2 = " + Row.ToString() + " ";
            }
            if( Column > Int32.MinValue )
            {
                ChildLocationsSql += "  and j.field3 = " + Column.ToString() + " ";
            }

            if( NodeIdsToFilterOut != string.Empty )
            {
                ChildLocationsSql += "  and n.nodeid not in (" + NodeIdsToFilterOut + ") ";
            }
            ChildLocationsSql += " order by n.nodeid";

            CswArbitrarySelect ChildLocationsSelect = _CswNbtResources.makeCswArbitrarySelect( "LocationTree_childlocation_select", ChildLocationsSql );
            DataTable ChildLocationsTable = null;
            try
            {
                ChildLocationsTable = ChildLocationsSelect.getTable();
            }
            catch( Exception ex )
            {
                throw new CswDniException( ErrorType.Error, "Invalid View", "addLocationChildren() attempted to run invalid SQL: " + ChildLocationsSql, ex );
            }

            if( ChildLocationsTable.Rows.Count > 0 )
            {
                foreach( DataRow ChildRow in ChildLocationsTable.Rows )
                {
                    if( CswConvert.ToString( ChildRow["objectclass"] ) == "LocationClass" )
                    {
                        populateLocations( new CswPrimaryKey( "nodes", CswConvert.ToInt32( ChildRow["nodeid"] ) ), CswConvert.ToString( ChildRow["nodename"] ), NodeIdsToFilterOut, Depth - 1 );
                    }
                }
            }
        } // addLocationChildren()

    }
}
