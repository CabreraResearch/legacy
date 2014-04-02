using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ServiceDrivers
{
    public class CswNbtSdLocations
    {
        private CswNbtResources _CswNbtResources;
        private Collection<Location> _LocationCollection = new Collection<Location>();
        private Int32 _SearchThreshold;
        public CswNbtSdLocations( CswNbtResources Resources )
        {
            _CswNbtResources = Resources;
            _SearchThreshold = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumNbtConfigurationVariables.relationshipoptionlimit.ToString() ) );
        }

        public CswNbtViewRelationship getLocationRelationship( string LocationSql, CswNbtView LocationsView, CswPrimaryKey StartLocationId )
        {
            CswNbtViewRelationship LocationRel = null;
            CswArbitrarySelect LocationSelect = _CswNbtResources.makeCswArbitrarySelect( "populateLocations_select", LocationSql );
            LocationSelect.addParameter( "startlocationid", StartLocationId.PrimaryKey.ToString() );

            DataTable LocationTable = null;
            try
            {
                LocationTable = LocationSelect.getTable();
                Collection<CswPrimaryKey> LocationPks = new Collection<CswPrimaryKey>();
                LocationPks.Add( StartLocationId );
                CswNbtMetaDataObjectClass LocationOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
                LocationRel = LocationsView.AddViewRelationship( LocationOc, false );

                if( LocationTable.Rows.Count > 0 )
                {
                    foreach( DataRow Row in LocationTable.Rows )
                    {
                        Int32 LocationNodeId = CswConvert.ToInt32( Row["nodeid"] );
                        CswPrimaryKey LocationPk = new CswPrimaryKey( "nodes", LocationNodeId );
                        LocationPks.Add( LocationPk );

                    }
                }
                LocationRel.NodeIdsToFilterIn = LocationPks;
            }
            catch( Exception ex )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid Query", "_getContainerRelationship() attempted to run invalid SQL: " + LocationSql, ex );
            }
            return LocationRel;
        }

        public CswNbtViewRelationship getAllParentsLocationRelationship( CswNbtView LocationsView, CswPrimaryKey StartLocationId )
        {
            CswNbtViewRelationship LocationRel = null;
            if( null != StartLocationId )
            {
                string LocationSql = @"select distinct nodeid from (select n.nodeid, jnp.field1_fk
                                      from nodes n 
                                      join nodetypes nt on n.nodetypeid=nt.nodetypeid
                                      join object_class oc on nt.objectclassid=oc.objectclassid
                                      join jct_nodes_props jnp on n.nodeid=jnp.nodeid
                                      join nodetype_props ntp on jnp.nodetypepropid=ntp.nodetypepropid
                                      join field_types ft on ntp.fieldtypeid=ft.fieldtypeid
                                      where oc.objectclass='LocationClass' 
                                            and ft.fieldtype='Location'  
                                            and n.nodeid != :startlocationid " +
                                     " start with jnp.nodeid = :startlocationid " +
                                     " connect by n.nodeid = prior jnp.field1_fk )";
                LocationRel = getLocationRelationship( LocationSql, LocationsView, StartLocationId );
            }
            return LocationRel;
        }

        public CswNbtViewRelationship getAllChildrenLocationRelationship( CswNbtView LocationsView, CswPrimaryKey StartLocationId )
        {
            CswNbtViewRelationship LocationRel = null;
            if( null != StartLocationId )
            {
                string LocationSql = @"select distinct nodeid from (select n.nodeid, jnp.field1_fk
                                      from nodes n 
                                      join nodetypes nt on n.nodetypeid=nt.nodetypeid
                                      join object_class oc on nt.objectclassid=oc.objectclassid
                                      join jct_nodes_props jnp on n.nodeid=jnp.nodeid
                                      join nodetype_props ntp on jnp.nodetypepropid=ntp.nodetypepropid
                                      join field_types ft on ntp.fieldtypeid=ft.fieldtypeid
                                      where oc.objectclass='LocationClass' 
                                            and ft.fieldtype='Location'  
                                      start with n.nodeid = :startlocationid " +
                                     " connect by jnp.field1_fk = prior n.nodeid )";
                LocationRel = getLocationRelationship( LocationSql, LocationsView, StartLocationId );
            }
            return LocationRel;
        }

        [DataContract]
        public class Location
        {
            [DataMember]
            public string Name { get; set; }

            [DataMember]
            public string LocationId { get; set; }

            [DataMember]
            public string Path { get; set; }

            [DataMember]
            public bool Selected { get; set; }
        }

        public Collection<Location> getLocationListMobile()
        {
            Collection<Location> Locations = new Collection<Location>();

            CswNbtActSystemViews LocationSystemView = new CswNbtActSystemViews( _CswNbtResources, CswEnumNbtSystemViewName.SILocationsList, null );
            CswNbtView LocationsListView = LocationSystemView.SystemView;
            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( LocationsListView, true, false, false );
            Int32 LocationCount = Tree.getChildNodeCount();

            if( LocationCount > 0 )
            {
                CswNbtMetaDataObjectClass LocationsOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );

                for( Int32 N = 0; N < LocationCount; N += 1 )
                {
                    Tree.goToNthChild( N );
                    CswNbtNodeKey NodeKey = Tree.getNodeKeyForCurrentPosition();

                    if( NodeKey.ObjectClassId == LocationsOc.ObjectClassId )
                    {
                        Location LocationNode = new Location();
                        Collection<CswNbtTreeNodeProp> Props = Tree.getChildNodePropsOfNode();

                        // LocationNode.Name = Tree.getNodeNameForCurrentPosition();
                        LocationNode.LocationId = Tree.getNodeIdForCurrentPosition().ToString();
                        foreach( CswNbtTreeNodeProp Prop in Props )
                        {
                            if( Prop.FieldType == CswEnumNbtFieldType.Location )
                            {
                                // CIS-52811: If Location != Site
                                if( Int32.MinValue != Prop.Field1_Fk )
                                {
                                    LocationNode.Name = Prop.Gestalt + CswNbtNodePropLocation.PathDelimiter;
                                }
                                LocationNode.Name += Tree.getNodeNameForCurrentPosition();
                            }
                        }
                        Locations.Add( LocationNode );
                    }
                    Tree.goToParentNode();
                }
                foreach( var Location in from Location _Location
                                                          in Locations
                                         orderby _Location.Name
                                         select _Location )
                {
                    Locations.Add( Location );
                }

                //IEnumerable < Location > OrderedLocations = Locations.OrderBy( location => location.Name );
                //Locations = OrderedLocations.ToList();

            }
            return Locations;
        }//getLocationListMobile()

        public Collection<Location> GetLocationsList( string ViewId, string SelectedNodeId = "" )
        {
            // Only return options if the total number of locations is < the relationshipoptionlimit configuration variable
            if( CswNbtNodePropLocation.getNumberOfLocationNodes( _CswNbtResources ) < _SearchThreshold )
            {
                CswPrimaryKey SelectedLocationId = String.IsNullOrEmpty( SelectedNodeId ) ? _CswNbtResources.CurrentNbtUser.DefaultLocationId : CswConvert.ToPrimaryKey( SelectedNodeId );

                CswNbtView LocationView = _getLocationsView( ViewId );
                ICswNbtTree tree = _CswNbtResources.Trees.getTreeFromView( LocationView, false, false, false );

                if( tree.getChildNodeCount() > 0 )
                {
                    _iterateTree( tree, SelectedLocationId );
                }
            }

            return _LocationCollection;
        }//GetLocationsList()

        public Collection<Location> searchLocations( string Query, string ViewId )
        {
            Collection<Location> Ret = new Collection<Location>();

            CswNbtView LocationView = _getLocationsView( ViewId, Query );
            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( LocationView, false, false, false );

            int count = Tree.getChildNodeCount();
            // Only return options if the total number of locations is < the relationshipoptionlimit configuration variable
            if( count < _SearchThreshold )
            {
                if( Tree.getChildNodeCount() > 0 )
                {
                    _iterateTree( Tree, null );
                }

                foreach( Location LocationObj in _LocationCollection )
                {
                    Location location = new Location();
                    location.Name = LocationObj.Name;
                    location.LocationId = LocationObj.LocationId;
                    location.Path = LocationObj.Path;
                    Ret.Add( location );
                }
            }

            return Ret;
        }//searchLocations()

        private CswNbtView _getLocationsView( string ViewId, string NameFilter = "" )
        {
            CswNbtView Ret = new CswNbtView();

            if( string.IsNullOrEmpty( ViewId ) )
            {
                Ret = CswNbtNodePropLocation.LocationPropertyView( _CswNbtResources, null, ResultMode: CswEnumNbtFilterResultMode.Hide, NameFilter: NameFilter );
                Ret.SaveToCache( false );
                ViewId = Ret.SessionViewId.ToString();
            }

            CswNbtSessionDataId SessionViewId = new CswNbtSessionDataId( ViewId );
            if( SessionViewId.isSet() )
            {
                Ret = _CswNbtResources.ViewSelect.getSessionView( SessionViewId );
                CswNbtMetaDataObjectClass LocationOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
                CswNbtMetaDataObjectClassProp LocationNameOCP = LocationOC.getObjectClassProp( CswNbtObjClassLocation.PropertyName.Name );
                if( false == string.IsNullOrEmpty( NameFilter ) )
                {
                    CswNbtViewRoot.forEachProperty forEachPropDelegate = delegate( CswNbtViewProperty ViewProp )
                        {
                            if( ViewProp.ObjectClassPropId == LocationNameOCP.PropId )
                            {
                                Ret.AddViewPropertyFilter( ViewProp,
                                                          Conjunction: CswEnumNbtFilterConjunction.And,
                                                          FilterMode: CswEnumNbtFilterMode.Contains,
                                                          Value: NameFilter );

                            }
                        };
                    Ret.Root.eachRelationship( null, forEachPropDelegate );
                }
            }

            return Ret;
        }

        private void _iterateTree( ICswNbtTree Tree, CswPrimaryKey SelectedNodeId )
        {
            CswNbtMetaDataObjectClass LocationOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            CswNbtMetaDataObjectClassProp LocationNameOCP = LocationOC.getObjectClassProp( CswNbtObjClassLocation.PropertyName.Name );
            CswNbtMetaDataObjectClassProp LocationLocationOCP = LocationOC.getObjectClassProp( CswNbtObjClassLocation.PropertyName.Location );

            for( Int32 c = 0; c < Tree.getChildNodeCount(); c += 1 )
            {
                Tree.goToNthChild( c );

                Location LocationObj = new Location();

                CswNbtTreeNodeProp nameTreeProp = Tree.getChildNodePropsOfNode().FirstOrDefault( p => p.ObjectClassPropName == LocationNameOCP.PropName );
                CswNbtTreeNodeProp locationTreeProp = Tree.getChildNodePropsOfNode().FirstOrDefault( p => p.ObjectClassPropName == LocationLocationOCP.PropName );

                if( locationTreeProp.Field1_Fk > 0 )
                {
                    LocationObj.Path = locationTreeProp.Gestalt + CswNbtNodePropLocation.PathDelimiter + nameTreeProp.Field1;
                }
                else
                {
                    LocationObj.Path = nameTreeProp.Field1;
                }
                LocationObj.Name = nameTreeProp.Field1;
                LocationObj.LocationId = Tree.getNodeIdForCurrentPosition().ToString();
                LocationObj.Selected = ( Tree.getNodeIdForCurrentPosition() == SelectedNodeId );
                _LocationCollection.Add( LocationObj );

                if( Tree.getChildNodeCount() > 0 )
                {
                    _iterateTree( Tree, SelectedNodeId );
                }
                Tree.goToParentNode();
            }
        }//_iterateTree()

    } // public class CswNbtSdLocations

} // namespace ChemSW.Nbt.WebServices
