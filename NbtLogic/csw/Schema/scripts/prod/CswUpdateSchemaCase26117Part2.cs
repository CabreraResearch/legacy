using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26117Part2
    /// </summary>
    public class CswUpdateSchemaCase26117Part2 : CswUpdateSchemaTo
    {
        private void _addLocationRelationship( string LocationName, CswNbtView SiLocationsTreeView, CswNbtView SiLocationsTreeList, CswNbtViewRelationship ParentRelationship )
        {
            CswNbtMetaDataNodeType LocationNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( LocationName );
            if( null != LocationNt )
            {
                CswNbtMetaDataNodeType LocationLatestNt = LocationNt.getNodeTypeLatestVersion();
                CswNbtMetaDataNodeTypeProp LocationLocationNtp = LocationLatestNt.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.LocationPropertyName );

                CswNbtViewRelationship LocationTreeVr = SiLocationsTreeView.AddViewRelationship( ParentRelationship, NbtViewPropOwnerType.Second, LocationLocationNtp, true );
                CswNbtViewProperty LocationLocationTreeVp = SiLocationsTreeView.AddViewProperty( LocationTreeVr, LocationLocationNtp );
                LocationLocationTreeVp.SortBy = true;

                CswNbtViewRelationship LocationListVr = SiLocationsTreeList.AddViewRelationship( LocationLatestNt, true );
                CswNbtViewProperty LocationLocationListVp = SiLocationsTreeView.AddViewProperty( LocationListVr, LocationLocationNtp );
                LocationLocationListVp.SortBy = true;

                if( LocationName == "Building" )
                {
                    _addLocationRelationship( "Floor", SiLocationsTreeView, SiLocationsTreeList, LocationTreeVr );
                    _addLocationRelationship( "Room", SiLocationsTreeView, SiLocationsTreeList, LocationTreeVr );
                }
                else if( LocationName == "Floor" )
                {
                    _addLocationRelationship( "Room", SiLocationsTreeView, SiLocationsTreeList, LocationTreeVr );
                }
            }
        }

        public override void update()
        {
            CswNbtActSystemViews LocationTreeSystemView = new CswNbtActSystemViews( _CswNbtSchemaModTrnsctn.MetaData._CswNbtMetaDataResources.CswNbtResources,
                CswNbtActSystemViews.SystemViewName.SILocationsTree, null );
            CswNbtView SiLocationsTreeView = LocationTreeSystemView.SystemView;
            SiLocationsTreeView.Root.ChildRelationships.Clear();

            CswNbtActSystemViews LocationListSystemView = new CswNbtActSystemViews( _CswNbtSchemaModTrnsctn.MetaData._CswNbtMetaDataResources.CswNbtResources,
                CswNbtActSystemViews.SystemViewName.SILocationsList, null );
            CswNbtView SiLocationsTreeList = LocationListSystemView.SystemView;
            SiLocationsTreeList.Root.ChildRelationships.Clear();

            CswNbtMetaDataNodeType SiteNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Site" );
            if( null != SiteNt )
            {
                CswNbtMetaDataNodeType SiteLatestNt = SiteNt.getNodeTypeLatestVersion();
                CswNbtMetaDataNodeTypeProp SiteLocationNtp = SiteLatestNt.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.LocationPropertyName );

                CswNbtViewRelationship SiteTreeVr = SiLocationsTreeView.AddViewRelationship( SiteLatestNt, true );
                CswNbtViewProperty SiteLocationTreeVp = SiLocationsTreeView.AddViewProperty( SiteTreeVr, SiteLocationNtp );
                SiteLocationTreeVp.SortBy = true;

                CswNbtViewRelationship SiteListVr = SiLocationsTreeList.AddViewRelationship( SiteLatestNt, true );
                CswNbtViewProperty SiteLocationListVp = SiLocationsTreeView.AddViewProperty( SiteListVr, SiteLocationNtp );
                SiteLocationListVp.SortBy = true;
                _addLocationRelationship( "Building", SiLocationsTreeView, SiLocationsTreeList, SiteTreeVr );
            }

            SiLocationsTreeView.save();
            SiLocationsTreeList.save();

            new CswNbtActSystemViews( _CswNbtSchemaModTrnsctn.MetaData._CswNbtMetaDataResources.CswNbtResources,
                                      CswNbtActSystemViews.SystemViewName.SIInspectionsbyBarcode, null );
            new CswNbtActSystemViews( _CswNbtSchemaModTrnsctn.MetaData._CswNbtMetaDataResources.CswNbtResources,
                                      CswNbtActSystemViews.SystemViewName.SIInspectionsbyDate, null );
            new CswNbtActSystemViews( _CswNbtSchemaModTrnsctn.MetaData._CswNbtMetaDataResources.CswNbtResources,
                                      CswNbtActSystemViews.SystemViewName.SIInspectionsbyLocation, null );
            new CswNbtActSystemViews( _CswNbtSchemaModTrnsctn.MetaData._CswNbtMetaDataResources.CswNbtResources,
                                      CswNbtActSystemViews.SystemViewName.SIInspectionsbyUser, null );

        }//Update()

    }//class CswUpdateSchemaCase26117Part2

}//namespace ChemSW.Nbt.Schema