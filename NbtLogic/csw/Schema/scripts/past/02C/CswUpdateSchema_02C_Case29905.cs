using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29905
    /// </summary>
    public class CswUpdateSchema_02C_Case29905 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 29905; }
        }

        public override void update()
        {
            CswNbtMetaDataNodeType ContainerDocumentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Container Document" );
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            foreach( CswNbtMetaDataNodeType ContainerNT in ContainerOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp DocumentsNTP = ContainerNT.getNodeTypeProp( "Documents" );
                if( null != DocumentsNTP && null != ContainerDocumentNT )
                {
                    CswNbtView ContainerDocsView = _CswNbtSchemaModTrnsctn.restoreView( DocumentsNTP.ViewId );
                    if( null == ContainerDocsView )
                    {
                        ContainerDocsView = _CswNbtSchemaModTrnsctn.makeSafeView( "Documents", CswEnumNbtViewVisibility.Property );
                        ContainerDocsView.ViewMode = CswEnumNbtViewRenderingMode.Grid;
                    }
                    ContainerDocsView.Root.ChildRelationships.Clear();

                    CswNbtMetaDataNodeTypeProp OwnerNTP = ContainerDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Owner );
                    CswNbtMetaDataNodeTypeProp ArchivedNTP = ContainerDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Archived );
                    CswNbtMetaDataNodeTypeProp FileNTP = ContainerDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.File );
                    CswNbtMetaDataNodeTypeProp LinkNTP = ContainerDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Link );
                    CswNbtMetaDataNodeTypeProp TitleNTP = ContainerDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Title );
                    CswNbtMetaDataNodeTypeProp AcquiredDateNTP = ContainerDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.AcquiredDate );
                    CswNbtMetaDataNodeTypeProp ExpirationDateNTP = ContainerDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.ExpirationDate );

                    CswNbtViewRelationship RootRel = ContainerDocsView.AddViewRelationship( ContainerNT, false );
                    CswNbtViewRelationship DocRel = ContainerDocsView.AddViewRelationship( RootRel, CswEnumNbtViewPropOwnerType.Second, OwnerNTP, true );
                    ContainerDocsView.AddViewPropertyAndFilter( DocRel, ArchivedNTP, CswEnumTristate.False.ToString(),
                                                        FilterMode: CswEnumNbtFilterMode.Equals,
                                                        ShowAtRuntime: true,
                                                        ShowInGrid: false );
                    ContainerDocsView.AddViewProperty( DocRel, FileNTP, 1 );
                    ContainerDocsView.AddViewProperty( DocRel, LinkNTP, 2 );
                    ContainerDocsView.AddViewProperty( DocRel, TitleNTP, 3 );
                    ContainerDocsView.AddViewProperty( DocRel, AcquiredDateNTP, 4 );
                    ContainerDocsView.AddViewProperty( DocRel, ExpirationDateNTP, 5 );
                    ContainerDocsView.save();
                    DocumentsNTP.ViewId = ContainerDocsView.ViewId;
                }
            }
        } // update()

    }//class CswUpdateSchema_02B_Case29905

}//namespace ChemSW.Nbt.Schema