using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26117
    /// </summary>
    public class CswUpdateSchemaCase26117 : CswUpdateSchemaTo
    {
        private void _addLocationRelationship( string LocationName, CswNbtView SiLocationsTreeView, CswNbtView SiLocationsTreeList, CswNbtViewRelationship ParentRelationship )
        {
            CswNbtMetaDataNodeType LocationNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( LocationName );
            if( null != LocationNt )
            {
                CswNbtMetaDataNodeType LocationLatestNt = LocationNt.getNodeTypeLatestVersion();
                CswNbtMetaDataNodeTypeProp LocationNameNtp = LocationLatestNt.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.NamePropertyName );
                CswNbtMetaDataNodeTypeProp LocationLocationNtp = LocationLatestNt.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.LocationPropertyName );

                CswNbtViewRelationship LocationTreeVr = SiLocationsTreeView.AddViewRelationship( ParentRelationship, NbtViewPropOwnerType.Second, LocationLocationNtp, true );
                CswNbtViewProperty LocationNameTreeVp = SiLocationsTreeView.AddViewProperty( LocationTreeVr, LocationNameNtp );
                LocationNameTreeVp.SortBy = true;

                CswNbtViewRelationship LocationListVr = SiLocationsTreeList.AddViewRelationship( LocationLatestNt, true );
                CswNbtViewProperty LocationNameListVp = SiLocationsTreeView.AddViewProperty( LocationListVr, LocationNameNtp );
                LocationNameListVp.SortBy = true;

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
                CswNbtMetaDataNodeTypeProp SiteNameNtp = SiteLatestNt.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.NamePropertyName );

                CswNbtViewRelationship SiteTreeVr = SiLocationsTreeView.AddViewRelationship( SiteLatestNt, true );
                CswNbtViewProperty SiteNameTreeVp = SiLocationsTreeView.AddViewProperty( SiteTreeVr, SiteNameNtp );
                SiteNameTreeVp.SortBy = true;

                CswNbtViewRelationship SiteListVr = SiLocationsTreeList.AddViewRelationship( SiteLatestNt, true );
                CswNbtViewProperty SiteNameListVp = SiLocationsTreeView.AddViewProperty( SiteListVr, SiteNameNtp );
                SiteNameListVp.SortBy = true;
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

    }//class CswUpdateSchemaCase26117

}//namespace ChemSW.Nbt.Schema