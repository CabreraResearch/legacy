using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29563
    /// </summary>
    public class CswUpdateSchema_02C_Case29563B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 29563; }
        }

        public override void update()
        {
            CswNbtMetaDataNodeType CofADocumentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "C of A Document" );
            if( null != CofADocumentNT )
            {
                CswNbtMetaDataObjectClass ReceiptLotOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReceiptLotClass );
                foreach( CswNbtMetaDataNodeType ReceiptLotNT in ReceiptLotOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp AssignedCofANTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp(
                        ReceiptLotNT,
                        CswEnumNbtFieldType.Grid,
                        "Assigned C of A",
                        ReceiptLotNT.getFirstNodeTypeTab().TabId
                    );
                    AssignedCofANTP.Extended = "Link";
                    CswNbtMetaDataNodeTypeProp OwnerOCP = CofADocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassCofADocument.PropertyName.Owner );
                    CswNbtMetaDataNodeTypeProp RevisionDateNTP = CofADocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassCofADocument.PropertyName.RevisionDate );
                    CswNbtMetaDataNodeTypeProp ArchivedNTP = CofADocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassCofADocument.PropertyName.Archived );
                    CswNbtMetaDataNodeTypeProp FileNTP = CofADocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassCofADocument.PropertyName.File );
                    CswNbtMetaDataNodeTypeProp LinkNTP = CofADocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassCofADocument.PropertyName.Link );

                    CswNbtView AssignedCofAView = _CswNbtSchemaModTrnsctn.makeSafeView( "Assigned C of A", CswEnumNbtViewVisibility.Property );
                    AssignedCofAView.ViewMode = CswEnumNbtViewRenderingMode.Grid;
                    CswNbtViewRelationship RootRel = AssignedCofAView.AddViewRelationship( ReceiptLotNT, false );
                    CswNbtViewRelationship DocRel = AssignedCofAView.AddViewRelationship( RootRel, CswEnumNbtViewPropOwnerType.Second, OwnerOCP, true );
                    AssignedCofAView.AddViewPropertyAndFilter( DocRel, ArchivedNTP, CswEnumTristate.False.ToString(),
                                                        FilterMode: CswEnumNbtFilterMode.Equals,
                                                        ShowAtRuntime: true,
                                                        ShowInGrid: false );
                    if( null != RevisionDateNTP )
                    {
                        AssignedCofAView.AddViewProperty( DocRel, RevisionDateNTP, 1 );
                    }
                    AssignedCofAView.AddViewProperty( DocRel, FileNTP, 2 );
                    AssignedCofAView.AddViewProperty( DocRel, LinkNTP, 3 );
                    AssignedCofAView.save();
                    AssignedCofANTP.ViewId = AssignedCofAView.ViewId;
                }
            }
        } // update()
    }//class CswUpdateSchema_02B_Case29563B
}//namespace ChemSW.Nbt.Schema