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
using ChemSW.Nbt.Actions;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-08
    /// </summary>
    public class CswUpdateSchemaTo01H08 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 08 ); } }
        public CswUpdateSchemaTo01H08( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
        }

        public void update()
        {

            // case 20033
            // Inspection Design and Inspection Route should be in FE as well as SI

            CswTableSelect ModulesTableSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "modules_select", "modules" );
            DataTable FETable = ModulesTableSelect.getTable( "where name = 'FE'" );
            Int32 FEModuleId = CswConvert.ToInt32( FETable.Rows[0]["moduleid"] );

            CswNbtMetaDataObjectClass InspectionDesignOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
            CswNbtMetaDataObjectClass InspectionRouteOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionRouteClass );

            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( FEModuleId, InspectionDesignOC.ObjectClassId );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( FEModuleId, InspectionRouteOC.ObjectClassId );

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

                          where (((o.objectclass = 'GeneratorClass'
                              or o.objectclass = 'MailReportClass')
                          and e.enabled = '1'
                          and (sysdate >= (ddi.initialduedate - wd.warningdays)
                           and sysdate >= (ndd.nextduedate - wd.warningdays))
                           and (fdd.finalduedate is null or sysdate <= fdd.finalduedate)))

                        union

                        select n.nodeid
                          from nodes n
                          join nodetypes t on n.nodetypeid = t.nodetypeid
                          join object_class o on t.objectclassid = o.objectclassid

                          join (select op.objectclassid, p.nodetypeid, j.nodeid, p.propname, j.field1_date duedate, j.field1_numeric warningdays
                                  from object_class_props op 
                                  join nodetype_props p on op.objectclasspropid = p.objectclasspropid
                                  join jct_nodes_props j on j.nodetypepropid = p.nodetypepropid
                                 where op.propname like 'Due Date' ) dd on (dd.objectclassid = o.objectclassid 
                                                                            and dd.nodeid = n.nodeid 
                                                                            and dd.nodetypeid = t.nodetypeid)

                           join (select op.objectclassid, p.nodetypeid, j.nodeid, p.propname, j.field1 status
                                  from object_class_props op 
                                  join nodetype_props p on op.objectclasspropid = p.objectclasspropid
                                  join jct_nodes_props j on j.nodetypepropid = p.nodetypepropid
                                 where op.propname like 'Status' ) s on (s.objectclassid = o.objectclassid 
                                                                            and s.nodeid = n.nodeid 
                                                                            and s.nodetypeid = t.nodetypeid)                                                                            

                           where o.objectclass = 'InspectionDesignClass' 
                           and sysdate >= ( dd.duedate )
                           and s.status = 'Pending' ";

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
            DataTable IMCSTable = ModulesTableSelect.getTable( "where name = 'IMCS'" );
            Int32 IMCSModuleId = CswConvert.ToInt32( IMCSTable.Rows[0]["moduleid"] );

            CswNbtMetaDataNodeType CabinetNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Cabinet" );
            CswNbtMetaDataNodeType ShelfNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Shelf" );
            CswNbtMetaDataNodeType BoxNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Box" );

            if( null != CabinetNT )
                _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( IMCSModuleId, CabinetNT.NodeTypeId );

            if( null != ShelfNT )
                _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( IMCSModuleId, ShelfNT.NodeTypeId );

            if( null != BoxNT )
                _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( IMCSModuleId, BoxNT.NodeTypeId );

            //Create Floor
            CswNbtMetaDataNodeType BuildingNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Building" );
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
            CswNbtMetaDataNodeType FloorNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( LocationOC.ObjectClassId, CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Floor ), "Locations" );
            CswNbtMetaDataNodeTypeProp FloorNameNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( FloorNT, CswNbtMetaDataFieldType.NbtFieldType.Text, "Name", Int32.MinValue );
            FloorNameNTP.SetValueOnAdd = true;
            FloorNT.NameTemplateText = CswNbtMetaData.MakeTemplateEntry( "Name" );
            FloorNT.IconFileName = "ball_blueS.gif";
            CswNbtMetaDataNodeTypeProp FloorParent = FloorNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.LocationPropertyName );
            if( null != BuildingNT )
                FloorParent.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), BuildingNT.NodeTypeId, string.Empty, Int32.MinValue );

            _CswNbtSchemaModTrnsctn.MetaData.refreshAll();

            CswNbtNode RoleNode = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "Administrator" );
            if( RoleNode != null )
            {
                CswNbtNodeTypePermissions Permissions = _CswNbtSchemaModTrnsctn.getNodeTypePermissions( (CswNbtObjClassRole) CswNbtNodeCaster.AsRole( RoleNode ), FloorNT );
                Permissions.Create = true;
                Permissions.Edit = true;
                Permissions.View = true;
                Permissions.Delete = true;
                Permissions.Save();
                RoleNode.postChanges( true );
            }

            //Modify Locations View

            CswNbtMetaDataNodeType RoomNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Room" );
            CswNbtMetaDataNodeType MountPointNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Mount_Point ) );
            CswNbtMetaDataNodeType FireExtinguisherNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.Fire_Extinguisher ) );
            CswNbtMetaDataNodeTypeProp RoomParent = null;

            //FE Locations View
            if( null != BuildingNT && null != FloorNT && null != RoomNT && null != MountPointNT && null != FireExtinguisherNT )
            {
                RoomParent = RoomNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.LocationPropertyName );
                CswNbtMetaDataNodeTypeProp MountPointParent = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.LocationPropertyName );
                CswNbtMetaDataNodeTypeProp FireExtinguisherParent = FireExtinguisherNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassFireExtinguisher.InspectionTargetPropertyName );
                CswNbtMetaDataNodeTypeProp BuildingNameNTP = BuildingNT.getNodeTypeProp( "Name" );
                CswNbtMetaDataNodeTypeProp BuildingBarcodeNTP = BuildingNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.BarcodePropertyName );
                CswNbtMetaDataNodeTypeProp FloorBarcodeNTP = FloorNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.BarcodePropertyName );
                CswNbtMetaDataNodeTypeProp RoomNameNTP = RoomNT.getNodeTypeProp( "Name" );
                CswNbtMetaDataNodeTypeProp RoomBarcodeNTP = RoomNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.BarcodePropertyName );
                
                CswNbtView FELocationsView = _CswNbtSchemaModTrnsctn.makeView();
                FELocationsView.makeNew( "Mount Points", NbtViewVisibility.Global, null, null, null );
                FELocationsView.Category = string.Empty;
                CswNbtViewRelationship BuildingRelationship = FELocationsView.AddViewRelationship( BuildingNT, false );
                if( null != BuildingNameNTP )
                {
                    CswNbtViewProperty BuildingNameVP = FELocationsView.AddViewProperty( BuildingRelationship, BuildingNameNTP );
                    FELocationsView.AddViewPropertyFilter( BuildingNameVP, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );
                }
                CswNbtViewProperty BuildingBarcodeVP = FELocationsView.AddViewProperty( BuildingRelationship, BuildingBarcodeNTP );
                FELocationsView.AddViewPropertyFilter( BuildingBarcodeVP, CswNbtSubField.SubFieldName.Barcode, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );
                CswNbtViewRelationship MP1Relationship = FELocationsView.AddViewRelationship( BuildingRelationship, CswNbtViewRelationship.PropOwnerType.Second, MountPointParent, false );
                CswNbtViewRelationship FE1Relationship = FELocationsView.AddViewRelationship( MP1Relationship, CswNbtViewRelationship.PropOwnerType.Second, FireExtinguisherParent, false );

                CswNbtViewRelationship FloorRelationship = FELocationsView.AddViewRelationship( BuildingRelationship, CswNbtViewRelationship.PropOwnerType.Second, FloorParent, false );
                CswNbtViewProperty FloorNameVP = FELocationsView.AddViewProperty( FloorRelationship, FloorNameNTP );
                CswNbtViewProperty FloorBarcodeVP = FELocationsView.AddViewProperty( FloorRelationship, FloorBarcodeNTP );
                FELocationsView.AddViewPropertyFilter( FloorNameVP, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );
                FELocationsView.AddViewPropertyFilter( FloorBarcodeVP, CswNbtSubField.SubFieldName.Barcode, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );
                CswNbtViewRelationship MP2Relationship = FELocationsView.AddViewRelationship( FloorRelationship, CswNbtViewRelationship.PropOwnerType.Second, MountPointParent, false );
                CswNbtViewRelationship FE2Relationship = FELocationsView.AddViewRelationship( MP2Relationship, CswNbtViewRelationship.PropOwnerType.Second, FireExtinguisherParent, false );

                CswNbtViewRelationship RoomRelationship = FELocationsView.AddViewRelationship( FloorRelationship, CswNbtViewRelationship.PropOwnerType.Second, RoomParent, false );
                if( null != RoomNameNTP )
                {
                    CswNbtViewProperty RoomNameVP = FELocationsView.AddViewProperty( RoomRelationship, RoomNameNTP );
                    FELocationsView.AddViewPropertyFilter( RoomNameVP, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );
                }
                CswNbtViewProperty RoomBarcodeVP = FELocationsView.AddViewProperty( RoomRelationship, RoomBarcodeNTP );
                FELocationsView.AddViewPropertyFilter( RoomBarcodeVP, CswNbtSubField.SubFieldName.Barcode, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );
                CswNbtViewRelationship MP3Relationship = FELocationsView.AddViewRelationship( RoomRelationship, CswNbtViewRelationship.PropOwnerType.Second, MountPointParent, false );
                CswNbtViewRelationship FE3Relationship = FELocationsView.AddViewRelationship( MP3Relationship, CswNbtViewRelationship.PropOwnerType.Second, FireExtinguisherParent, false );

                CswNbtViewRelationship Room2Relationship = FELocationsView.AddViewRelationship( BuildingRelationship, CswNbtViewRelationship.PropOwnerType.Second, RoomParent, false );
                CswNbtViewRelationship MP4Relationship = FELocationsView.AddViewRelationship( Room2Relationship, CswNbtViewRelationship.PropOwnerType.Second, MountPointParent, false );
                CswNbtViewRelationship FE4Relationship = FELocationsView.AddViewRelationship( MP4Relationship, CswNbtViewRelationship.PropOwnerType.Second, FireExtinguisherParent, false );

                FELocationsView.save();
            }

            //IMCS Locations View
            if( null != BuildingNT && null != FloorNT && null != RoomNT && null != CabinetNT && null != ShelfNT && null != BoxNT )
            {
                _CswNbtSchemaModTrnsctn.deleteView( "Locations", true );

                CswNbtView IMCSLocationsView = _CswNbtSchemaModTrnsctn.makeView();
                IMCSLocationsView.makeNew( "Locations", NbtViewVisibility.Global, null, null, null );
                IMCSLocationsView.Category = string.Empty;

                RoomParent = RoomNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.LocationPropertyName );
                CswNbtMetaDataNodeTypeProp CabinetParent = CabinetNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.LocationPropertyName );
                CswNbtMetaDataNodeTypeProp ShelfParent = ShelfNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.LocationPropertyName );
                CswNbtMetaDataNodeTypeProp BoxParent = BoxNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.LocationPropertyName );

                CswNbtViewRelationship IMCSBuildingRelationship = IMCSLocationsView.AddViewRelationship( BuildingNT, false );
                CswNbtViewRelationship IMCSFloorRelationship = IMCSLocationsView.AddViewRelationship( IMCSBuildingRelationship, CswNbtViewRelationship.PropOwnerType.Second, FloorParent, false );
                CswNbtViewRelationship IMCSRoomRelationship = IMCSLocationsView.AddViewRelationship( IMCSFloorRelationship, CswNbtViewRelationship.PropOwnerType.Second, RoomParent, false );
                CswNbtViewRelationship IMCSCabinetRelationship = IMCSLocationsView.AddViewRelationship( IMCSRoomRelationship, CswNbtViewRelationship.PropOwnerType.Second, CabinetParent, false );
                CswNbtViewRelationship IMCSShelfRelationship = IMCSLocationsView.AddViewRelationship( IMCSCabinetRelationship, CswNbtViewRelationship.PropOwnerType.Second, ShelfParent, false );
                CswNbtViewRelationship IMCSBoxRelationship = IMCSLocationsView.AddViewRelationship( IMCSShelfRelationship, CswNbtViewRelationship.PropOwnerType.Second, BoxParent, false );

                CswNbtViewRelationship IMCSRoom2Relationship = IMCSLocationsView.AddViewRelationship( IMCSBuildingRelationship, CswNbtViewRelationship.PropOwnerType.Second, RoomParent, false );
                CswNbtViewRelationship IMCSCabinet2Relationship = IMCSLocationsView.AddViewRelationship( IMCSRoom2Relationship, CswNbtViewRelationship.PropOwnerType.Second, CabinetParent, false );
                CswNbtViewRelationship IMCSShelf2Relationship = IMCSLocationsView.AddViewRelationship( IMCSCabinet2Relationship, CswNbtViewRelationship.PropOwnerType.Second, ShelfParent, false );
                CswNbtViewRelationship IMCSBox2Relationship = IMCSLocationsView.AddViewRelationship( IMCSShelf2Relationship, CswNbtViewRelationship.PropOwnerType.Second, BoxParent, false );
                IMCSLocationsView.IsSearchable();
                IMCSLocationsView.save();
            }

        } // update()

    }//class CswUpdateSchemaTo01H08

}//namespace ChemSW.Nbt.Schema


