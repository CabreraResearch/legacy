using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using System;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28282
    /// </summary>
    public class CswUpdateSchema_01V_Case28282 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28282; }
        }

        public override void update()
        {
            CswNbtMetaDataNodeType ControlZoneNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Control Zone" );
            if( null != ControlZoneNt )
            {
                CswNbtMetaDataNodeTypeTab LocationsTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( ControlZoneNt, "Locations", 2 );

                CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.LocationClass );
                CswNbtMetaDataObjectClassProp ControlZoneOCP =
                    _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProp( LocationOC.ObjectClassId, CswNbtObjClassLocation.PropertyName.ControlZone );

                CswNbtMetaDataNodeTypeProp LocationsNTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( 
                    new CswNbtWcfMetaDataModel.NodeTypeProp( ControlZoneNt, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Grid ), "Locations" )
                {
                    TabId = LocationsTab.TabId
                } );

                CswNbtView LocationsView = _CswNbtSchemaModTrnsctn.makeNewView( "Control_Zone_Locations", NbtViewVisibility.Property );
                LocationsNTP.ViewId = LocationsView.ViewId;
                LocationsView.Root.ChildRelationships.Clear();
                LocationsView.SetViewMode( NbtViewRenderingMode.Grid );
                LocationsView.Visibility = NbtViewVisibility.Property;

                CswNbtViewRelationship RootRel = LocationsView.AddViewRelationship( ControlZoneNt, true );
                CswNbtViewRelationship LocationRel = LocationsView.AddViewRelationship( RootRel, NbtViewPropOwnerType.Second, ControlZoneOCP, false );
                LocationsView.AddViewProperty( LocationRel, LocationOC.getObjectClassProp( CswNbtObjClassLocation.PropertyName.Barcode ) );
                LocationsView.AddViewProperty( LocationRel, LocationOC.getObjectClassProp( CswNbtObjClassLocation.PropertyName.Name ) );
                LocationsView.AddViewProperty( LocationRel, LocationOC.getObjectClassProp( CswNbtObjClassLocation.PropertyName.Location ) );
                
                LocationsView.save();      
            }
        }

        //Update()

    }//class CswUpdateSchemaCase_01V_28282

}//namespace ChemSW.Nbt.Schema