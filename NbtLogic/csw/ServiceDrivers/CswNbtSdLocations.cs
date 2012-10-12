using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Mail;
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
        
    } // public class CswNbtSdLocations

} // namespace ChemSW.Nbt.WebServices
