using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-07
    /// </summary>
    public class CswUpdateSchemaTo01H07 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 07 ); } }
        public CswUpdateSchemaTo01H07( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
        }

        public void update()
        {
            // Case 20025
            String newSQL = @"select n.nodeid 
                          from nodes n
                          join nodetypes t on n.nodetypeid = t.nodetypeid
                          join object_class o on t.objectclassid = o.objectclassid

                          join (select op.objectclassid, p.nodetypeid, j.nodeid, p.propname, j.field1 enabled
                                  from object_class_props op 
                                  join nodetype_props p on op.objectclasspropid = p.objectclasspropid
                                  join jct_nodes_props j on j.nodetypepropid = p.nodetypepropid
                                 where op.propname = 'Enabled' ) e on (e.objectclassid = o.objectclassid 
                                                                   and e.nodeid = n.nodeid 
                                                                   and e.nodetypeid = t.nodetypeid)

                          left outer join (select op.objectclassid, p.nodetypeid, j.nodeid, p.propname, j.field1_date finalduedate
                                  from object_class_props op 
                                  join nodetype_props p on op.objectclasspropid = p.objectclasspropid
                                  join jct_nodes_props j on j.nodetypepropid = p.nodetypepropid
                                 where op.propname = 'Final Due Date' ) fdd on (fdd.objectclassid = o.objectclassid 
                                                                            and fdd.nodeid = n.nodeid 
                                                                            and fdd.nodetypeid = t.nodetypeid)

                          join (select op.objectclassid, p.nodetypeid, j.nodeid, p.propname, j.field1_date nextduedate
                                  from object_class_props op 
                                  join nodetype_props p on op.objectclasspropid = p.objectclasspropid
                                  join jct_nodes_props j on j.nodetypepropid = p.nodetypepropid
                                 where op.propname = 'Next Due Date' ) ndd on (ndd.objectclassid = o.objectclassid 
                                                                           and ndd.nodeid = n.nodeid 
                                                                           and ndd.nodetypeid = t.nodetypeid)

                          join (select op.objectclassid, p.nodetypeid, j.nodeid, p.propname, j.field1_numeric warningdays
                                  from object_class_props op 
                                  join nodetype_props p on op.objectclasspropid = p.objectclasspropid
                                  join jct_nodes_props j on j.nodetypepropid = p.nodetypepropid
                                 where op.propname = 'Warning Days' ) wd on (wd.objectclassid = o.objectclassid 
                                                                         and wd.nodeid = n.nodeid
                                                                         and wd.nodetypeid = t.nodetypeid)

                          join (select op.objectclassid, p.nodetypeid, j.nodeid, p.propname, j.field1_date initialduedate
                          ,j.field1, j.field2, j.field3, j.field4, j.field5, j.field1_date
                                from object_class_props op 
                                  join nodetype_props p on op.objectclasspropid = p.objectclasspropid
                                  join jct_nodes_props j on j.nodetypepropid = p.nodetypepropid
                                 where op.propname = 'Due Date Interval' ) ddi on (ddi.objectclassid = o.objectclassid   
                                                                               and ddi.nodeid = n.nodeid 
                                                                               and ddi.nodetypeid = t.nodetypeid)

                          left outer join (select op.objectclassid, p.nodetypeid, j.nodeid, p.propname, j.field1_date duedate
                                  from object_class_props op 
                                  join nodetype_props p on op.objectclasspropid = p.objectclasspropid
                                  join jct_nodes_props j on j.nodetypepropid = p.nodetypepropid
                                 where op.propname = 'Due Date' ) dd on (dd.objectclassid = o.objectclassid 
                                                                            and dd.nodeid = n.nodeid 
                                                                            and dd.nodetypeid = t.nodetypeid)

                          where (((o.objectclass = 'GeneratorClass'
                              or o.objectclass = 'MailReportClass')
                          and e.enabled = '1'
                          and (sysdate >= (ddi.initialduedate - wd.warningdays)
                           and sysdate >= (ndd.nextduedate - wd.warningdays))
                           and (fdd.finalduedate is null or sysdate <= fdd.finalduedate))
                           or (o.objectclass = 'InspectionDesignClass' 
                           and sysdate >= dd.duedate)) ";

            CswTableUpdate ScheduleItemsS4 = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "ScheduleItemsS4", "static_sql_selects" );
            DataTable S4Table = ScheduleItemsS4.getTable( " where lower(queryid)='generatorsdue'" );
            if( S4Table.Rows.Count == 1 )
            {
                S4Table.Rows[0]["querytext"] = newSQL;
                S4Table.Rows[0]["queryid"] = "ScheduleItemsDue";
                ScheduleItemsS4.update( S4Table );
            }

            // Case 20029
            //Associate Cabinet, Shelf and Box with IMCS
            CswTableSelect ModulesTableSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "modules_select", "modules" );
            DataTable ModulesTable = ModulesTableSelect.getTable( "where name = 'IMCS'" );
            Int32 IMCSModuleId = CswConvert.ToInt32( ModulesTable.Rows[0]["moduleid"] );

            CswNbtMetaDataNodeType CabinetNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Cabinet" );
            CswNbtMetaDataNodeType ShelfNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Shelf" );
            CswNbtMetaDataNodeType BoxNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Box" );

            _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( IMCSModuleId, CabinetNT.NodeTypeId );
            _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( IMCSModuleId, ShelfNT.NodeTypeId );
            _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( IMCSModuleId, BoxNT.NodeTypeId );

            //Create Floor
            CswNbtMetaDataNodeType BuildingNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Building" );
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
            CswNbtMetaDataNodeType FloorNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( LocationOC.ObjectClassId, "Floor", "Locations" );
            CswNbtMetaDataNodeTypeProp FloorNameNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( FloorNT, CswNbtMetaDataFieldType.NbtFieldType.Text, "Name", Int32.MinValue );
            FloorNameNTP.SetValueOnAdd = true;
            FloorNT.NameTemplateText = CswNbtMetaData.MakeTemplateEntry( "Name" );
            FloorNT.IconFileName = "ball_blueS.gif";
            CswNbtMetaDataNodeTypeProp FloorParent = FloorNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.LocationPropertyName );
            FloorParent.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), BuildingNT.NodeTypeId, string.Empty, Int32.MinValue );
            
            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();

            CswNbtNode RoleNode = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "Administrator" );
            if( RoleNode != null )
            {
                CswNbtNodePropLogicalSet Permissions = ( (CswNbtObjClassRole) CswNbtNodeCaster.AsRole( RoleNode ) ).NodeTypePermissions;

                Permissions.SetValue( NodeTypePermission.Create.ToString(), FloorNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.Delete.ToString(), FloorNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.Edit.ToString(), FloorNT.NodeTypeId.ToString(), true );
                Permissions.SetValue( NodeTypePermission.View.ToString(), FloorNT.NodeTypeId.ToString(), true );

                Permissions.Save();
                RoleNode.postChanges( true );
            }

            //Modify Locations View

            CswNbtMetaDataNodeType RoomNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Room" );
            CswNbtMetaDataNodeTypeProp RoomParent = RoomNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.LocationPropertyName );
            CswNbtMetaDataNodeTypeProp CabinetParent = CabinetNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.LocationPropertyName );
            CswNbtMetaDataNodeTypeProp ShelfParent = ShelfNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.LocationPropertyName );
            CswNbtMetaDataNodeTypeProp BoxParent = BoxNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.LocationPropertyName );

            _CswNbtSchemaModTrnsctn.deleteView( "Locations", true );
            CswNbtView LocationsView = _CswNbtSchemaModTrnsctn.makeView();
            LocationsView.makeNew( "Locations", NbtViewVisibility.Global, null, null, null );
            LocationsView.Category = string.Empty;
            CswNbtViewRelationship BuildingRelationship = LocationsView.AddViewRelationship( BuildingNT, false );

            //Branch #1
            CswNbtViewRelationship FloorRelationship = LocationsView.AddViewRelationship( BuildingRelationship, CswNbtViewRelationship.PropOwnerType.Second, FloorParent, false );
            CswNbtViewRelationship RoomRelationship = LocationsView.AddViewRelationship( FloorRelationship, CswNbtViewRelationship.PropOwnerType.Second, RoomParent, false );
            CswNbtViewRelationship CabinetRelationship = LocationsView.AddViewRelationship( RoomRelationship, CswNbtViewRelationship.PropOwnerType.Second, CabinetParent, false );
            CswNbtViewRelationship ShelfRelationship = LocationsView.AddViewRelationship( CabinetRelationship, CswNbtViewRelationship.PropOwnerType.Second, ShelfParent, false );
            CswNbtViewRelationship BoxRelationship = LocationsView.AddViewRelationship( ShelfRelationship, CswNbtViewRelationship.PropOwnerType.Second, BoxParent, false );

            //Branch #2
            CswNbtViewRelationship Room2Relationship = LocationsView.AddViewRelationship( BuildingRelationship, CswNbtViewRelationship.PropOwnerType.Second, RoomParent, false );
            CswNbtViewRelationship Cabinet2Relationship = LocationsView.AddViewRelationship( Room2Relationship, CswNbtViewRelationship.PropOwnerType.Second, CabinetParent, false );
            CswNbtViewRelationship Shelf2Relationship = LocationsView.AddViewRelationship( Cabinet2Relationship, CswNbtViewRelationship.PropOwnerType.Second, ShelfParent, false );
            CswNbtViewRelationship Box2Relationship = LocationsView.AddViewRelationship( Shelf2Relationship, CswNbtViewRelationship.PropOwnerType.Second, BoxParent, false );

            LocationsView.save();

        }

    }//class CswUpdateSchemaTo01H07

}//namespace ChemSW.Nbt.Schema


