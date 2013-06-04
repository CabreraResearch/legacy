using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassCofADocument : CswNbtPropertySetDocument
    {
        #region Enums
        /// <summary>
        /// Object Class Property Names
        /// </summary>
        public new sealed class PropertyName : CswNbtPropertySetDocument.PropertyName
        {
            /// <summary>
            /// Revision Date of the document.
            /// </summary>
            public const string RevisionDate = "Revision Date";
        }

        #endregion Enums

        #region Base

        public CswNbtObjClassCofADocument( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.CofADocumentClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassCofADocument
        /// </summary>
        public static implicit operator CswNbtObjClassCofADocument( CswNbtNode Node )
        {
            CswNbtObjClassCofADocument ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.CofADocumentClass ) )
            {
                ret = (CswNbtObjClassCofADocument) Node.ObjClass;
            }
            return ret;
        }

        /// <summary>
        /// Cast a Document PropertySet back to an Object Class
        /// </summary>
        public static CswNbtObjClassCofADocument fromPropertySet( CswNbtPropertySetDocument PropertySet )
        {
            return PropertySet.Node;
        }

        /// <summary>
        /// Cast a the Object Class as a PropertySet
        /// </summary>
        public static CswNbtPropertySetDocument toPropertySet( CswNbtObjClassCofADocument ObjClass )
        {
            return ObjClass;
        }

        #endregion Base

        #region Inherited Events

        public override void beforePropertySetWriteNode( bool IsCopy, bool OverrideUniqueValidation ) { }

        public override void afterPropertySetWriteNode() { }

        public override void beforePropertySetDeleteNode( bool DeleteAllRequiredRelatedNodes = false ) { }

        public override void afterPropertySetDeleteNode() { }

        public override void afterPropertySetPopulateProps() { }

        public override void onPropertySetAddDefaultViewFilters( CswNbtViewRelationship ParentRelationship ) { }

        public override bool onPropertySetButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }

        public override void archiveMatchingDocs()
        {
            //Archives existing Documents related to the same Owner.
            if( Archived.Checked != CswEnumTristate.True )
            {
                CswNbtNode OwnerNode = _CswNbtResources.Nodes.GetNode( Owner.RelatedNodeId );
                if( null != OwnerNode )
                {
                    CswNbtView ExistingDocsView = new CswNbtView( _CswNbtResources );
                    CswNbtViewRelationship DocumentVr = ExistingDocsView.AddViewRelationship( NodeType, false );
                    ExistingDocsView.AddViewPropertyAndFilter( DocumentVr, Owner.NodeTypeProp, OwnerNode.NodeId.PrimaryKey.ToString(), CswEnumNbtSubFieldName.NodeID );
                    ExistingDocsView.AddViewPropertyAndFilter( DocumentVr, Archived.NodeTypeProp, CswEnumTristate.True.ToString(), FilterMode: CswEnumNbtFilterMode.NotEquals );

                    ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( ExistingDocsView, true, false, false );
                    Int32 DocCount = Tree.getChildNodeCount();
                    if( DocCount > 0 )
                    {
                        for( Int32 I = 0; I < DocCount; I += 1 )
                        {
                            Tree.goToNthChild( I );
                            CswNbtNode DocNode = Tree.getNodeForCurrentPosition();
                            if( DocNode.NodeId != NodeId )
                            {
                                CswNbtObjClassCofADocument DocNodeAsDocument = DocNode;
                                DocNodeAsDocument.Archived.Checked = CswEnumTristate.True;
                                DocNode.postChanges( true );
                            }
                            Tree.goToParentNode();
                        }
                    }

                }
            }
        }

        #endregion Inherited Events

        #region Custom Logic

        public static CswNbtObjClassCofADocument getActiveCofADocument( CswNbtResources _CswNbtResources, CswPrimaryKey ReceiptLotId )
        {
            CswNbtObjClassCofADocument CofADoc = null;
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.CofA ) )
            {
                CswNbtMetaDataObjectClass CofADocOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.CofADocumentClass );
                CswNbtMetaDataNodeType CofADocumentNT = CofADocOC.FirstNodeType;
                if( null != CofADocumentNT )
                {
                    CswNbtView AssignedCofAView = getAssignedCofADocumentsView( _CswNbtResources, ReceiptLotId );
                    ICswNbtTree CofATree = _CswNbtResources.Trees.getTreeFromView( AssignedCofAView, false, false, false );
                    if( CofATree.getChildNodeCount() > 0 )
                    {
                        CofATree.goToNthChild( 0 );
                        if( CofATree.getChildNodeCount() > 0 )
                        {
                            CofATree.goToNthChild( 0 );
                            CofADoc = CofATree.getNodeForCurrentPosition();
                        }
                    }
                }
            }
            return CofADoc;
        }

        public static CswNbtView getAssignedCofADocumentsView( CswNbtResources _CswNbtResources, CswPrimaryKey ReceiptLotId, bool IncludeArchivedDocs = false )
        {
            CswNbtView AssignedCofAView = null;
            CswNbtNode ReceiptLot = _CswNbtResources.Nodes[ReceiptLotId];
            CswNbtMetaDataObjectClass CofADocOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.CofADocumentClass );
            CswNbtMetaDataNodeType CofADocumentNT = CofADocOC.FirstNodeType;
            if( null != CofADocumentNT )
            {
                CswNbtMetaDataNodeTypeProp OwnerNTP = CofADocumentNT.getNodeTypePropByObjectClassProp( PropertyName.Owner );
                CswNbtMetaDataNodeTypeProp RevisionDateNTP = CofADocumentNT.getNodeTypePropByObjectClassProp( PropertyName.RevisionDate );
                CswNbtMetaDataNodeTypeProp ArchivedNTP = CofADocumentNT.getNodeTypePropByObjectClassProp( PropertyName.Archived );
                CswNbtMetaDataNodeTypeProp FileNTP = CofADocumentNT.getNodeTypePropByObjectClassProp( PropertyName.File );
                CswNbtMetaDataNodeTypeProp LinkNTP = CofADocumentNT.getNodeTypePropByObjectClassProp( PropertyName.Link );

                AssignedCofAView = new CswNbtView( _CswNbtResources )
                {
                    ViewName = "All Assigned C of A Documents",
                    ViewMode = CswEnumNbtViewRenderingMode.Grid,
                    ViewVisibility = CswEnumNbtViewVisibility.Property.ToString()
                };
                CswNbtViewRelationship RootRel = AssignedCofAView.AddViewRelationship( ReceiptLot.getNodeType(), false );
                RootRel.NodeIdsToFilterIn.Add( ReceiptLotId );
                CswNbtViewRelationship DocRel = AssignedCofAView.AddViewRelationship( RootRel, CswEnumNbtViewPropOwnerType.Second, OwnerNTP, true );
                if( false == IncludeArchivedDocs )
                {
                    AssignedCofAView.AddViewPropertyAndFilter( DocRel, ArchivedNTP, CswEnumTristate.False.ToString(),
                                                               FilterMode: CswEnumNbtFilterMode.Equals,
                                                               ShowAtRuntime: true,
                                                               ShowInGrid: false );
                }
                if( null != RevisionDateNTP )
                {
                    AssignedCofAView.AddViewProperty( DocRel, RevisionDateNTP, 1 );
                }
                AssignedCofAView.AddViewProperty( DocRel, FileNTP, 2 );
                AssignedCofAView.AddViewProperty( DocRel, LinkNTP, 3 );
                AssignedCofAView.SaveToCache( false );
            }
            return AssignedCofAView;
        }

        #endregion Custom Logic

        #region Object class specific properties

        public CswNbtNodePropDateTime RevisionDate { get { return _CswNbtNode.Properties[PropertyName.RevisionDate]; } }

        #endregion Object class specific properties

    }//CswNbtObjClassCofADocument

}//namespace ChemSW.Nbt.ObjClasses