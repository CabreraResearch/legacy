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
                CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
                foreach( CswNbtMetaDataNodeType MaterialNT in MaterialOC.getNodeTypes() )
                {
                    if( MaterialNT.NodeTypeName == "Chemical" )
                    {
                        CswNbtMetaDataNodeTypeProp AssignedSDSNTP = MaterialNT.getNodeTypeProp( "Assigned SDS" );
                        if( null != AssignedSDSNTP )
                        {
                            CswNbtMetaDataNodeTypeProp OwnerOCP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Owner );
                            CswNbtMetaDataNodeTypeProp RevisionDateNTP = SDSDocumentNT.getNodeTypeProp( "Revision Date" );
                            CswNbtMetaDataNodeTypeProp ArchivedNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Archived );
                            CswNbtMetaDataNodeTypeProp FileNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.File );
                            CswNbtMetaDataNodeTypeProp LinkNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Link );
                            CswNbtMetaDataNodeTypeProp LanguageNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Language );
                            CswNbtMetaDataNodeTypeProp FormatNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Format );

                            CswNbtView AssignedSDSView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( AssignedSDSNTP.ViewId );
                            AssignedSDSView.Root.ChildRelationships.Clear();
                            CswNbtViewRelationship RootRel = AssignedSDSView.AddViewRelationship( MaterialNT, false );
                            CswNbtViewRelationship DocRel = AssignedSDSView.AddViewRelationship( RootRel, CswEnumNbtViewPropOwnerType.Second, OwnerOCP, true );
                            AssignedSDSView.AddViewPropertyAndFilter( DocRel, ArchivedNTP, CswEnumTristate.False.ToString(),
                                                             FilterMode: CswEnumNbtFilterMode.Equals,
                                                             ShowAtRuntime: true,
                                                             ShowInGrid: false );
                            if( null != RevisionDateNTP )
                            {
                                AssignedSDSView.AddViewProperty( DocRel, RevisionDateNTP, 1 );
                            }
                            AssignedSDSView.AddViewProperty( DocRel, FileNTP, 2 );
                            AssignedSDSView.AddViewProperty( DocRel, LinkNTP, 3 );
                            AssignedSDSView.AddViewProperty( DocRel, LanguageNTP, 4 );
                            AssignedSDSView.AddViewProperty( DocRel, FormatNTP, 5 );
                            AssignedSDSView.save();
                        }
                    }
                }
            }
        } // update()

    }//class CswUpdateSchema_02B_Case29401

}//namespace ChemSW.Nbt.Schema