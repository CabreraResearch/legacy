using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

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
            CswNbtMetaDataObjectClass CofADocumentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.CofADocumentClass );
            CswNbtMetaDataObjectClass ReceiptLotOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReceiptLotClass );
            foreach( CswNbtMetaDataNodeType ReceiptLotNT in ReceiptLotOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp AssignedCofANTP = _CswNbtSchemaModTrnsctn.MetaData.makeNewPropDeprecated(
                    ReceiptLotNT,
                    CswEnumNbtFieldType.Grid,
                    "Assigned C of A",
                    ReceiptLotNT.getFirstNodeTypeTab().TabId
                );
                AssignedCofANTP.Extended = "Link";

                CswNbtMetaDataObjectClassProp OwnerOCP = CofADocumentOC.getObjectClassProp( CswNbtObjClassCofADocument.PropertyName.Owner );
                CswNbtMetaDataObjectClassProp RevisionDateOCP = CofADocumentOC.getObjectClassProp( CswNbtObjClassCofADocument.PropertyName.RevisionDate );
                CswNbtMetaDataObjectClassProp ArchivedOCP = CofADocumentOC.getObjectClassProp( CswNbtObjClassCofADocument.PropertyName.Archived );
                CswNbtMetaDataObjectClassProp FileOCP = CofADocumentOC.getObjectClassProp( CswNbtObjClassCofADocument.PropertyName.File );
                CswNbtMetaDataObjectClassProp LinkOCP = CofADocumentOC.getObjectClassProp( CswNbtObjClassCofADocument.PropertyName.Link );

                CswNbtView AssignedCofAView = _CswNbtSchemaModTrnsctn.makeSafeView( "Assigned C of A", CswEnumNbtViewVisibility.Property );
                AssignedCofAView.ViewMode = CswEnumNbtViewRenderingMode.Grid;
                CswNbtViewRelationship RootRel = AssignedCofAView.AddViewRelationship( ReceiptLotNT, false );
                CswNbtViewRelationship DocRel = AssignedCofAView.AddViewRelationship( RootRel, CswEnumNbtViewPropOwnerType.Second, OwnerOCP, true );
                AssignedCofAView.AddViewPropertyAndFilter( DocRel, ArchivedOCP, CswEnumTristate.False.ToString(),
                                                    FilterMode: CswEnumNbtFilterMode.Equals,
                                                    ShowAtRuntime: true,
                                                    ShowInGrid: false );

                AssignedCofAView.AddViewProperty( DocRel, RevisionDateOCP, 1 );
                AssignedCofAView.AddViewProperty( DocRel, FileOCP, 2 );
                AssignedCofAView.AddViewProperty( DocRel, LinkOCP, 3 );
                AssignedCofAView.save();
                AssignedCofANTP.ViewId = AssignedCofAView.ViewId;
            }
        } // update()
    }//class CswUpdateSchema_02B_Case29563B
}//namespace ChemSW.Nbt.Schema