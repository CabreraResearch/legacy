using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Mail;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.csw.Conversion;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ServiceDrivers
{
    public class CswNbtSdLocations
    {
        private CswNbtResources _CswNbtResources;


        public CswNbtSdLocations( CswNbtResources Resources )
        {
            _CswNbtResources = Resources;
        }
        
        public CswNbtViewRelationship getLocationRelationship( string LocationSql, CswNbtView LocationsView, CswPrimaryKey StartLocationId )
        {
            CswNbtViewRelationship LocationRel = null;
            CswArbitrarySelect LocationSelect = _CswNbtResources.makeCswArbitrarySelect( "populateLocations_select", LocationSql );
            DataTable LocationTable = null;
            try
            {
                LocationTable = LocationSelect.getTable();
                Collection<CswPrimaryKey> LocationPks = new Collection<CswPrimaryKey>();
                LocationPks.Add( StartLocationId );
                CswNbtMetaDataObjectClass LocationOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.LocationClass );
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
                throw new CswDniException( ErrorType.Error, "Invalid Query", "_getContainerRelationship() attempted to run invalid SQL: " + LocationSql, ex );
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
                                            and n.nodeid != " + StartLocationId.PrimaryKey + " " +
                                     " start with jnp.nodeid = " + StartLocationId.PrimaryKey + " " +
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
                                      start with n.nodeid = " + StartLocationId.PrimaryKey + " " +
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
        }

        public Collection<Location> getLocationList()
        {
            Collection<Location> Locations = new Collection<Location>();

            CswNbtActSystemViews LocationSystemView = new CswNbtActSystemViews( _CswNbtResources, SystemViewName.SILocationsList, null );
            CswNbtView LocationsListView = LocationSystemView.SystemView;
            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( LocationsListView, true, false, false );
            Int32 LocationCount = Tree.getChildNodeCount();
            
            if( LocationCount > 0 )
            {
                CswNbtMetaDataObjectClass LocationsOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.LocationClass );
                
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
                            if( Prop.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Location )
                            {
                                LocationNode.Name = Prop.Gestalt + CswNbtNodePropLocation.PathDelimiter + Tree.getNodeNameForCurrentPosition();
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
                
            }
            return Locations;
        } 

       

        

    } // public class CswNbtSdLocations

} // namespace ChemSW.Nbt.WebServices
