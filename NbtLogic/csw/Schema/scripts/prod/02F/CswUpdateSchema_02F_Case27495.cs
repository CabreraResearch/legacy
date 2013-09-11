using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case27495 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 27495; }
        }

        public override string ScriptName
        {
            get { return "02F_Case27495"; }
        }

        public override void update()
        {

            CswNbtMetaDataNodeType SiteNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Site" );
            if( null != SiteNT )
            {
                CswNbtMetaDataNodeTypeProp LocationNTP = SiteNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.PropertyName.Location );

                LocationNTP.ReadOnly = true;
                LocationNTP.removeFromAllLayouts();
                LocationNTP.Hidden = true;

                CswNbtView SitesView = _CswNbtSchemaModTrnsctn.makeNewView( "Sites Location Not Null", CswEnumNbtViewVisibility.Hidden );

                CswNbtViewRelationship parent = SitesView.AddViewRelationship( SiteNT, false );
                SitesView.AddViewPropertyAndFilter( parent, LocationNTP,
                                                    SubFieldName: CswEnumNbtSubFieldName.NodeID,
                                                    FilterMode: CswEnumNbtFilterMode.NotNull );

                ICswNbtTree tree = _CswNbtSchemaModTrnsctn.getTreeFromView( SitesView, true );
                for( int i = 0; i < tree.getChildNodeCount(); i++ )
                {
                    tree.goToNthChild( i );

                    CswNbtObjClassLocation SiteNode = tree.getNodeForCurrentPosition();
                    SiteNode.Location.SelectedNodeId = null;
                    SiteNode.Location.RefreshNodeName();
                    SiteNode.Location.SyncGestalt();
                    SiteNode.postChanges( false );

                    tree.goToParentNode();
                }

                SitesView.Delete();
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema