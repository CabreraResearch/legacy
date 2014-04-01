using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_CIS52751 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 52751; }
        }

        public override string Title
        {
            get { return "Remove Orphaned SDS Docs"; }
        }

        public override string AppendToScriptName()
        {
            return "";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass SDSOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.SDSDocumentClass );
            CswNbtMetaDataObjectClassProp MaterialOCP = SDSOC.getObjectClassProp( CswNbtObjClassSDSDocument.PropertyName.Owner );
            CswNbtView OrphanedSDSDocsView = _CswNbtSchemaModTrnsctn.makeView();
            CswNbtViewRelationship RootVR = OrphanedSDSDocsView.AddViewRelationship( SDSOC, false );
            OrphanedSDSDocsView.AddViewPropertyAndFilter( RootVR, MaterialOCP, null, CswEnumNbtSubFieldName.NodeID, false, CswEnumNbtFilterMode.Null );
            ICswNbtTree OrphanedSDSDocsTree = _CswNbtSchemaModTrnsctn.getTreeFromView( OrphanedSDSDocsView, false );
            for( int i = 0; i < OrphanedSDSDocsTree.getChildNodeCount(); i++ )
            {
                OrphanedSDSDocsTree.goToNthChild( i );
                CswNbtNode OrphanedSDSDoc = OrphanedSDSDocsTree.getNodeForCurrentPosition();
                OrphanedSDSDoc.delete( false, true, false );
                OrphanedSDSDocsTree.goToParentNode();
            }
        }
    }
}