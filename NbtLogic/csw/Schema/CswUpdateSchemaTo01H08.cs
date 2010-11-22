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
            // case 20062
            // promote Location 'Name' and Mount Point 'Barcode' to object class props
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
            CswNbtMetaDataObjectClass MountPointOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MountPointClass );

            CswTableUpdate OCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01H08_ocp_update", "object_class_props" );
            DataTable OCPTable = OCPUpdate.getEmptyTable();
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( OCPTable, LocationOC.ObjectClassId, CswNbtObjClassLocation.NamePropertyName, CswNbtMetaDataFieldType.NbtFieldType.Text, Int32.MinValue, Int32.MinValue );
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow( OCPTable, MountPointOC.ObjectClassId, CswNbtObjClassMountPoint.BarcodePropertyName, CswNbtMetaDataFieldType.NbtFieldType.Barcode, Int32.MinValue, Int32.MinValue );
            OCPUpdate.update( OCPTable );

            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();

            // Case 20328
            CswNbtMetaDataNodeType WasteAreaInspectionNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Waste Area Inspection" );
            if( null != WasteAreaInspectionNT )
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeType( WasteAreaInspectionNT );

            CswNbtMetaDataNodeType WasteAreaRouteNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Waste Area Route" );
            if( null != WasteAreaRouteNT )
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeType( WasteAreaRouteNT );

            CswNbtMetaDataNodeType WasteInspectionScheduleNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Waste Inspection Schedule" );
            if( null != WasteInspectionScheduleNT )
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeType( WasteInspectionScheduleNT );

            CswNbtView WasteAreaInspectionsV = _CswNbtSchemaModTrnsctn.restoreView( "Waste Area Inspections" );
            if( null != WasteAreaInspectionsV )
                WasteAreaInspectionsV.Delete();
            
            //CswNbtMetaDataNodeType WasteAreaNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Waste Area" );
            //if( null != WasteAreaNT )
            //{
            //    _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeAllVersions( WasteAreaNT );
            //}

            String deleteWasteAreaJnps = @"delete from jct_nodes_props
											where nodetypepropid in (select nodetypepropid from nodetype_props where nodetypeid in 
											(select nodetypeid from nodetypes where lower(nodetypename) like 'waste area%'))";
            String deleteWasteAreaNtps = @"delete from nodetype_props
										   where nodetypeid in (select nodetypeid from nodetypes where lower(nodetypename) like 'waste area%')";
            String deleteWasteAreaJmn = @"delete from jct_modules_nodetypes
										  where nodetypeid in (select nodetypeid from nodetypes where lower(nodetypename) like 'waste area%')";
            String deleteWasteAreaNtts = @"delete from nodetype_tabset
										  where nodetypeid in (select nodetypeid from nodetypes where lower(nodetypename) like 'waste area%')";
            String deleteWasteAreaNodes = @"delete from nodes
										  where nodetypeid in (select nodetypeid from nodetypes where lower(nodetypename) like 'waste area%')";
            String deleteWasteAreaNts = @"delete from nodetypes
										  where lower(nodetypename) like 'waste area%'";

            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( deleteWasteAreaJnps );
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( deleteWasteAreaNtps );
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( deleteWasteAreaJmn );
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( deleteWasteAreaNtts );
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( deleteWasteAreaNodes );
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( deleteWasteAreaNts );

        } // update()

    }//class CswUpdateSchemaTo01H08

}//namespace ChemSW.Nbt.Schema


