using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassSDSDocument: CswNbtPropertySetDocument
    {
        #region Enums
        /// <summary>
        /// Object Class Property Names
        /// </summary>
        public new sealed class PropertyName: CswNbtPropertySetDocument.PropertyName
        {
            /// <summary>
            /// Language of the document.
            /// </summary>
            public const string Language = "Language";
            /// <summary>
            /// Format of the document.
            /// </summary>
            public const string Format = "Format";
            /// <summary>
            /// Revision Date of the document.
            /// </summary>
            public const string RevisionDate = "Revision Date";
        }


        /// <summary>
        /// Formats recognized by Business Logic
        /// </summary>
        public sealed class CswEnumSDSDocumentFormats
        {
            /// <summary>
            /// No associated format (Default)
            /// </summary>
            public const string None = "";
            /// <summary>
            /// Occupational Safety and Health Administration
            /// </summary>
            public const string OSHA = "OSHA";
            /// <summary>
            /// Globally Harmonized System
            /// </summary>
            public const string GHS = "GHS";
            public static CswCommaDelimitedString Options = new CswCommaDelimitedString { OSHA, GHS };
        }

        #endregion Enums

        #region Base

        public CswNbtObjClassSDSDocument( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.SDSDocumentClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassSDSDocument
        /// </summary>
        public static implicit operator CswNbtObjClassSDSDocument( CswNbtNode Node )
        {
            CswNbtObjClassSDSDocument ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.SDSDocumentClass ) )
            {
                ret = (CswNbtObjClassSDSDocument) Node.ObjClass;
            }
            return ret;
        }

        /// <summary>
        /// Cast a Document PropertySet back to an Object Class
        /// </summary>
        public static CswNbtObjClassSDSDocument fromPropertySet( CswNbtPropertySetDocument PropertySet )
        {
            return PropertySet.Node;
        }

        /// <summary>
        /// Cast a the Object Class as a PropertySet
        /// </summary>
        public static CswNbtPropertySetDocument toPropertySet( CswNbtObjClassSDSDocument ObjClass )
        {
            return ObjClass;
        }

        #endregion Base

        #region Inherited Events
        
        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation ) { }

        public override void afterCreateNode() { }

        public override void beforePropertySetWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            this.MakeFilePropReadonly();
        }

        public override void afterPropertySetWriteNode() { }

        public override void beforePropertySetDeleteNode( bool DeleteAllRequiredRelatedNodes = false ) { }

        public override void afterPropertySetDeleteNode() { }

        public override void afterPropertySetPopulateProps()
        {
            Language.SetOnPropChange( OnLanguagePropChange );
            Format.SetOnPropChange( OnFormatPropChange );
        }//afterPopulateProps()

        public override void onPropertySetAddDefaultViewFilters( CswNbtViewRelationship ParentRelationship ) { }

        public override bool onPropertySetButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }

        public override void archiveMatchingDocs()
        {
            //Archives existing Documents related to the same Owner.
            //Existing SDS Documents are only archived if both Language and Format matches.
            if( Archived.Checked != CswEnumTristate.True &&
                false == String.IsNullOrEmpty( Language.Value ) &&
                false == String.IsNullOrEmpty( Format.Value ) )
            {
                CswNbtNode OwnerNode = _CswNbtResources.Nodes.GetNode( Owner.RelatedNodeId );
                if( null != OwnerNode )
                {
                    CswNbtView ExistingDocsView = new CswNbtView( _CswNbtResources );
                    CswNbtViewRelationship DocumentVr = ExistingDocsView.AddViewRelationship( NodeType, false );
                    ExistingDocsView.AddViewPropertyAndFilter( DocumentVr, Owner.NodeTypeProp, OwnerNode.NodeId.PrimaryKey.ToString(), CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID );
                    ExistingDocsView.AddViewPropertyAndFilter( DocumentVr, Archived.NodeTypeProp, CswEnumTristate.True.ToString(), FilterMode : CswEnumNbtFilterMode.NotEquals );
                    ExistingDocsView.AddViewPropertyAndFilter( DocumentVr, Format.NodeTypeProp, Format.Value );
                    ExistingDocsView.AddViewPropertyAndFilter( DocumentVr, Language.NodeTypeProp, Language.Value );

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
                                CswNbtObjClassSDSDocument DocNodeAsDocument = DocNode;
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

        public static bool materialHasActiveSDS( CswNbtResources _CswNbtResources, CswPrimaryKey MaterialId )
        {
            bool HasActiveSDS = false;
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.SDS ) && null != MaterialId )
            {
                CswNbtView docView = getAssignedSDSDocumentsView( _CswNbtResources, MaterialId );
                ICswNbtTree docsTree = _CswNbtResources.Trees.getTreeFromView( docView, false, false, false );
                HasActiveSDS = docsTree.getChildNodeCount() > 0;
            }
            return HasActiveSDS;
        }

        public static string getAssignedSDSDocumentUrl( CswNbtResources _CswNbtResources, CswPrimaryKey MaterialId )
        {
            string url = "";
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.SDS ) )
            {
                CswNbtView docView = getAssignedSDSDocumentsView( _CswNbtResources, MaterialId );
                CswNbtObjClassUser currentUserNode = _CswNbtResources.Nodes[_CswNbtResources.CurrentNbtUser.UserId];
                CswNbtObjClassJurisdiction userJurisdictionNode = _CswNbtResources.Nodes[currentUserNode.JurisdictionProperty.RelatedNodeId];

                ICswNbtTree docsTree = _CswNbtResources.Trees.getTreeFromView( docView, false, false, false );
                int childCount = docsTree.getChildNodeCount();
                int lvlMatched = Int32.MinValue;
                string matchedFileType = "";
                CswNbtTreeNodeProp matchedFileProp = null;
                CswNbtTreeNodeProp matchedLinkProp = null;
                CswPrimaryKey matchedNodeId = null;

                if( childCount > 0 )
                {
                    for( int i = 0; i < childCount; i++ )
                    {
                        docsTree.goToNthChild( i );

                        string format = "";
                        string language = "";
                        string fileType = "";
                        CswNbtTreeNodeProp fileProp = null;
                        CswNbtTreeNodeProp linkProp = null;
                        CswPrimaryKey nodeId = docsTree.getNodeIdForCurrentPosition();

                        foreach( CswNbtTreeNodeProp prop in docsTree.getChildNodePropsOfNode() )
                        {
                            CswNbtMetaDataNodeTypeProp docNTP = _CswNbtResources.MetaData.getNodeTypeProp( prop.NodeTypePropId );
                            switch( docNTP.getObjectClassPropName() )
                            {
                                case PropertyName.Format:
                                    format = prop.Field1;
                                    break;
                                case PropertyName.Language:
                                    language = prop.Field1;
                                    break;
                                case PropertyName.FileType:
                                    fileType = prop.Field1;
                                    break;
                                case PropertyName.File:
                                    fileProp = prop;
                                    break;
                                case PropertyName.Link:
                                    linkProp = prop;
                                    break;
                            }
                        }

                        if( lvlMatched < 0 )
                        {
                            matchedFileType = fileType;
                            matchedFileProp = fileProp;
                            matchedLinkProp = linkProp;
                            matchedNodeId = nodeId;
                            lvlMatched = 0;
                        }
                        if( null != userJurisdictionNode )
                        {
                            if( lvlMatched < 1 && format.Equals( userJurisdictionNode.Format.Value ) )
                            {
                                matchedFileType = fileType;
                                matchedFileProp = fileProp;
                                matchedLinkProp = linkProp;
                                matchedNodeId = nodeId;
                                lvlMatched = 1;
                            }
                            if( lvlMatched < 2 && language.Equals( currentUserNode.Language ) )
                            {
                                matchedFileType = fileType;
                                matchedFileProp = fileProp;
                                matchedLinkProp = linkProp;
                                matchedNodeId = nodeId;
                                lvlMatched = 2;
                            }
                            if( lvlMatched < 3 && format.Equals( userJurisdictionNode.Format.Value ) && language.Equals( currentUserNode.Language ) )
                            {
                                matchedFileType = fileType;
                                matchedFileProp = fileProp;
                                matchedLinkProp = linkProp;
                                matchedNodeId = nodeId;
                                lvlMatched = 3;
                            }
                        }
                        docsTree.goToParentNode();
                    }
                    switch( matchedFileType )
                    {
                        case CswEnumDocumentFileTypes.File:
                            int jctnodepropid = CswConvert.ToInt32( matchedFileProp.JctNodePropId );
                            url = CswNbtNodePropBlob.getLink( jctnodepropid, matchedNodeId );
                            break;
                        case CswEnumDocumentFileTypes.Link:
                            CswNbtMetaDataObjectClass SDSDocOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.SDSDocumentClass );
                            CswNbtMetaDataNodeType SDSDocumentNT = SDSDocOC.FirstNodeType;
                            CswNbtMetaDataNodeTypeProp linkNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( PropertyName.Link );
                            url = CswNbtNodePropLink.GetFullURL( linkNTP.DesignNode.getAttributeValueByColumn( CswEnumNbtPropertyAttributeColumn.Attribute1 ),
                                                                 matchedLinkProp.Field2,
                                                                 linkNTP.DesignNode.getAttributeValueByColumn( CswEnumNbtPropertyAttributeColumn.Attribute2 ) );
                            break;
                    }
                }
            }
            return url;
        }

        public static CswNbtView getAssignedSDSDocumentsView( CswNbtResources _CswNbtResources, CswPrimaryKey MaterialId, bool IncludeArchivedDocs = false )
        {
            CswNbtMetaDataObjectClass SDSDocOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.SDSDocumentClass );
            CswNbtMetaDataObjectClassProp archivedOCP = SDSDocOC.getObjectClassProp( PropertyName.Archived );
            CswNbtMetaDataObjectClassProp formatOCP = SDSDocOC.getObjectClassProp( PropertyName.Format );
            CswNbtMetaDataObjectClassProp languageOCP = SDSDocOC.getObjectClassProp( PropertyName.Language );
            CswNbtMetaDataObjectClassProp fileTypeOCP = SDSDocOC.getObjectClassProp( PropertyName.FileType );
            CswNbtMetaDataObjectClassProp fileOCP = SDSDocOC.getObjectClassProp( PropertyName.File );
            CswNbtMetaDataObjectClassProp linkOCP = SDSDocOC.getObjectClassProp( PropertyName.Link );
            CswNbtMetaDataObjectClassProp ownerOCP = SDSDocOC.getObjectClassProp( PropertyName.Owner );
            CswNbtMetaDataObjectClassProp revisionDateOCP = SDSDocOC.getObjectClassProp( PropertyName.RevisionDate );

            CswNbtView docView = new CswNbtView( _CswNbtResources )
                                        {
                                            ViewName = "All Assigned SDS",
                                            ViewMode = CswEnumNbtViewRenderingMode.Grid
                                        };
            CswNbtViewRelationship parent = docView.AddViewRelationship( SDSDocOC, true );

            if( false == IncludeArchivedDocs )
            {
                docView.AddViewPropertyAndFilter( parent,
                                                    MetaDataProp : archivedOCP,
                                                    SubFieldName : CswNbtFieldTypeRuleLogical.SubFieldName.Checked,
                                                    Value : false.ToString(),
                                                    FilterMode : CswEnumNbtFilterMode.Equals,
                                                    ShowInGrid : false );
            }

            docView.AddViewPropertyAndFilter( parent,
                                                MetaDataProp : ownerOCP,
                                                SubFieldName: CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID,
                                                Value : MaterialId.PrimaryKey.ToString(),
                                                FilterMode : CswEnumNbtFilterMode.Equals,
                                                ShowInGrid : false );

            docView.AddViewProperty( parent, revisionDateOCP, 1 );
            docView.AddViewProperty( parent, formatOCP, 5 );
            docView.AddViewProperty( parent, languageOCP, 4 );
            docView.AddViewProperty( parent, fileOCP, 2 );
            docView.AddViewProperty( parent, linkOCP, 3 );
            CswNbtViewProperty FTVP = docView.AddViewProperty( parent, fileTypeOCP );
            FTVP.ShowInGrid = false;
            docView.SaveToCache( false );

            return docView;
        }

        #endregion Custom Logic

        #region Object class specific properties

        public CswNbtNodePropList Language { get { return _CswNbtNode.Properties[PropertyName.Language]; } }
        private void OnLanguagePropChange( CswNbtNodeProp NodeProp )
        {
            archiveMatchingDocs();
        }
        public CswNbtNodePropList Format { get { return _CswNbtNode.Properties[PropertyName.Format]; } }
        private void OnFormatPropChange( CswNbtNodeProp NodeProp )
        {
            archiveMatchingDocs();
        }
        public CswNbtNodePropDateTime RevisionDate { get { return _CswNbtNode.Properties[PropertyName.RevisionDate]; } }

        #endregion Object class specific properties

    }//CswNbtObjClassSDSDocument

}//namespace ChemSW.Nbt.ObjClasses