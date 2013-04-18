using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29401
    /// </summary>
    public class CswUpdateSchema_02B_Case29401 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 29401; }
        }

        public override void update()
        {
            CswNbtMetaDataNodeType SDSDocumentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "SDS Document" );
            if( null != SDSDocumentNT )
            {
                CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialClass );
                foreach( CswNbtMetaDataNodeType MaterialNT in MaterialOC.getNodeTypes() )
                {
                    if( MaterialNT.NodeTypeName == "Chemical" )
                    {
                        CswNbtMetaDataNodeTypeProp AssignedSDSNTP = MaterialNT.getNodeTypeProp( "Assigned SDS" );
                        if( null != AssignedSDSNTP )
                        {
                            CswNbtView AssignedSDSView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( AssignedSDSNTP.ViewId );
                            AssignedSDSView.Root.ChildRelationships.Clear();
                            CswNbtViewRelationship RootRel = AssignedSDSView.AddViewRelationship( MaterialNT, false );
                            CswNbtMetaDataNodeTypeProp OwnerOCP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Owner );
                            CswNbtViewRelationship DocRel = AssignedSDSView.AddViewRelationship( RootRel, CswEnumNbtViewPropOwnerType.Second, OwnerOCP, true );
                            CswNbtMetaDataNodeTypeProp ArchivedNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Archived );
                            AssignedSDSView.AddViewPropertyAndFilter( DocRel, ArchivedNTP, CswEnumTristate.False.ToString(),
                                                             FilterMode: CswEnumNbtFilterMode.Equals,
                                                             ShowAtRuntime: true,
                                                             ShowInGrid: false );
                            CswNbtMetaDataNodeTypeProp RevisionDateNTP = SDSDocumentNT.getNodeTypeProp( "Revision Date" );
                            if( null != RevisionDateNTP )
                            {
                                CswNbtViewProperty RevisionDateVP = AssignedSDSView.AddViewProperty( DocRel, RevisionDateNTP );
                                RevisionDateVP.Order = 1;
                            }
                            CswNbtMetaDataNodeTypeProp FileNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.File );
                            CswNbtViewProperty FileVP = AssignedSDSView.AddViewProperty( DocRel, FileNTP );
                            FileVP.Order = 2;
                            CswNbtMetaDataNodeTypeProp LinkNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Link );
                            CswNbtViewProperty LinkVP = AssignedSDSView.AddViewProperty( DocRel, LinkNTP );
                            LinkVP.Order = 3;
                            CswNbtMetaDataNodeTypeProp LanguageNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Language );
                            CswNbtViewProperty LanguageVP = AssignedSDSView.AddViewProperty( DocRel, LanguageNTP );
                            LanguageVP.Order = 4;
                            CswNbtMetaDataNodeTypeProp FormatNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Format );
                            CswNbtViewProperty FormatVP = AssignedSDSView.AddViewProperty( DocRel, FormatNTP );
                            FormatVP.Order = 5;
                            AssignedSDSView.save();
                        }
                    }
                }
            }
        } // update()

    }//class CswUpdateSchema_02B_Case29401

}//namespace ChemSW.Nbt.Schema