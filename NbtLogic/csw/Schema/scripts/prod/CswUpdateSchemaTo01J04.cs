using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01J-04
    /// </summary>
    public class CswUpdateSchemaTo01J04 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'J', 04 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            //Case 23809
            foreach( CswNbtView View in _CswNbtSchemaModTrnsctn.restoreViews( "Roles and Users" ) )
            {
                View.Category = "System";
                View.save();
            }

            //Case 23782
            CswNbtMetaDataNodeType RouteNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Physical Inspection Route" );
            CswNbtMetaDataNodeType PiNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "FE Inspection Point" );
            CswNbtMetaDataNodeTypeProp MpGridNtp = null;
            CswNbtMetaDataNodeTypeProp RouteNtp = null;
            CswNbtView MpgView = null;
            if( null != RouteNt && null != PiNt )
            {
                MpGridNtp = RouteNt.getNodeTypeProp( "FE Inspection Points Grid" );
                if( null != MpGridNtp && MpGridNtp.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Grid )
                {
                    RouteNtp = PiNt.getNodeTypeProp( "Route" );
                    if( null != MpGridNtp.ViewId )
                    {
                        MpgView = _CswNbtSchemaModTrnsctn.restoreView( MpGridNtp.ViewId );
                        MpgView.Root.ChildRelationships.Clear();
                    }
                    else
                    {
                        MpgView = new CswNbtView( _CswNbtSchemaModTrnsctn.MetaData._CswNbtMetaDataResources.CswNbtResources );
                        MpgView.ViewName = "Inspection Points Grid";
                        MpgView.Visibility = NbtViewVisibility.Property;
                        MpgView.ViewMode = NbtViewRenderingMode.Grid;
                        MpGridNtp.ViewId.set( MpgView.ViewId.get() );
                    }
                }
            }
            if( null != RouteNtp && null != MpgView )
            {
                CswNbtViewRelationship RouteRel = MpgView.AddViewRelationship( RouteNt, false );
                CswNbtViewRelationship InspectPointRel = MpgView.AddViewRelationship( RouteRel, CswNbtViewRelationship.PropOwnerType.Second, RouteNtp, false );

                CswNbtMetaDataNodeTypeProp BarcodeNtp = PiNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.BarcodePropertyName );
                MpgView.AddViewProperty( InspectPointRel, BarcodeNtp );

                CswNbtMetaDataNodeTypeProp DescNtp = PiNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.DescriptionPropertyName );
                MpgView.AddViewProperty( InspectPointRel, DescNtp );

                CswNbtMetaDataNodeTypeProp LocationNtp = PiNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.LocationPropertyName );
                MpgView.AddViewProperty( InspectPointRel, LocationNtp );

                CswNbtMetaDataNodeTypeProp LastIDateNtp = PiNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.LastInspectionDatePropertyName );
                MpgView.AddViewProperty( InspectPointRel, LastIDateNtp );

                CswNbtMetaDataNodeTypeProp StatusNtp = PiNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.StatusPropertyName );
                MpgView.AddViewProperty( InspectPointRel, StatusNtp );

                MpgView.save();
            }

            //Case 23775
            CswNbtMetaDataObjectClass TargetGroupClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetGroupClass );
            foreach( CswNbtMetaDataNodeType GroupNT in TargetGroupClass.NodeTypes )
            {
                CswNbtMetaDataNodeTypeTab LocationsTab = GroupNT.getNodeTypeTab( "Mount Point Locations" );
                if( null != LocationsTab )
                {
                    LocationsTab.TabName = "Inspection Point Locations";
                }
            }

            //Case 23814
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "delete from sessionlist" );

            //Case 23868
            CswNbtMetaDataNodeType GroupNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Inspection Group" );
            CswNbtMetaDataNodeTypeProp PointGridNtp = null;
            CswNbtMetaDataNodeTypeProp PointGroupNtp = null;
            CswNbtView LocationView = null;
            if( null != GroupNt && null != PiNt )
            {
                PointGridNtp = GroupNt.getNodeTypeProp( "FE Inspection Point Locations" );
                if( null != PointGridNtp && PointGridNtp.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Grid )
                {
                    PointGroupNtp = PiNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.InspectionTargetGroupPropertyName );
                    if( null != PointGridNtp.ViewId )
                    {
                        LocationView = _CswNbtSchemaModTrnsctn.restoreView( PointGridNtp.ViewId );
                        LocationView.Root.ChildRelationships.Clear();
                    }
                    else
                    {
                        LocationView = new CswNbtView( _CswNbtSchemaModTrnsctn.MetaData._CswNbtMetaDataResources.CswNbtResources );
                        LocationView.ViewName = "Inspection Points Grid";
                        LocationView.Visibility = NbtViewVisibility.Property;
                        LocationView.ViewMode = NbtViewRenderingMode.Grid;
                        PointGridNtp.ViewId.set( LocationView.ViewId.get() );
                    }
                }
            }
            if( null != PointGridNtp && null != LocationView )
            {
                CswNbtViewRelationship GroupRel = LocationView.AddViewRelationship( GroupNt, false );
                CswNbtViewRelationship InspectPointRel = LocationView.AddViewRelationship( GroupRel, CswNbtViewRelationship.PropOwnerType.Second, PointGroupNtp, false );

                CswNbtMetaDataNodeTypeProp BarcodeNtp = PiNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.BarcodePropertyName );
                CswNbtViewProperty Bvp = LocationView.AddViewProperty( InspectPointRel, BarcodeNtp );
                Bvp.Order = 0;

                CswNbtMetaDataNodeTypeProp LocationNtp = PiNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.LocationPropertyName );
                CswNbtViewProperty Lvp = LocationView.AddViewProperty( InspectPointRel, LocationNtp );
                Lvp.Order = 1;
                Lvp.SortBy = true;

                LocationView.save();
            }

        }//Update()

    }//class CswUpdateSchemaTo01J04

}//namespace ChemSW.Nbt.Schema


