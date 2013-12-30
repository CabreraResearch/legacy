using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
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
            /// <summary>
            /// If FileType == ChemWatch, the Link
            /// </summary>
            public const string ChemWatch = "ChemWatch";
            /// <summary>
            /// Country of the document.
            /// </summary>
            public const string Country = "Country";
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

        /// <summary>
        /// Potential File Type options (Includes menu inherited from base class <see cref="CswNbtPropertySetDocument"/>)
        /// </summary>
        public new sealed class CswEnumDocumentFileTypes: CswNbtPropertySetDocument.CswEnumDocumentFileTypes
        {
            /// <summary>
            /// Hyperlink
            /// </summary>
            public const string ChemWatch = "ChemWatch";

            public new static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString
                {
                    File, Link, ChemWatch
                };
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

        public override void beforePropertySetWriteNode()
        {
            this.MakeFilePropReadonly();
        }

        public override void afterPropertySetPopulateProps()
        {
            Language.SetOnPropChange( OnLanguagePropChange );
            Format.SetOnPropChange( OnFormatPropChange );
        }//afterPopulateProps()

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
                    ExistingDocsView.AddViewPropertyAndFilter( DocumentVr, Owner.NodeTypeProp, OwnerNode.NodeId.PrimaryKey.ToString(), CswEnumNbtSubFieldName.NodeID );
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
                if( docsTree.getChildNodeCount() > 0 ) //if the given Material Id is of a temp node, we won't see it in the view and can't go to it's Nth child
                {
                    docsTree.goToNthChild( 0 ); //The docView is a property view
                    HasActiveSDS = docsTree.getChildNodeCount() > 0;
                }
            }
            return HasActiveSDS;
        }

        public static string getAssignedSDSDocumentUrl( CswNbtResources _CswNbtResources, CswPrimaryKey MaterialId )
        {
            string url = "";
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.SDS ) )
            {
                CswNbtMetaDataObjectClass SDSDocOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.SDSDocumentClass );
                CswNbtMetaDataNodeType SDSDocumentNT = SDSDocOC.FirstNodeType;

                CswNbtView docView = getAssignedSDSDocumentsView( _CswNbtResources, MaterialId );

                // We need to re-add these properties because they were removed from the 'Assigned SDS' view in Case 31223
                CswNbtMetaDataNodeTypeProp LinkNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.Link );
                docView.AddViewProperty( docView.Root.ChildRelationships[0].ChildRelationships[0], LinkNTP );
                CswNbtMetaDataNodeTypeProp FileNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.File );
                docView.AddViewProperty( docView.Root.ChildRelationships[0].ChildRelationships[0], FileNTP );
                CswNbtMetaDataNodeTypeProp FileTypeNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetDocument.PropertyName.FileType );
                docView.AddViewProperty( docView.Root.ChildRelationships[0].ChildRelationships[0], FileTypeNTP );

                // If ChemWatch is enabled, we need to check for this type of file too
                if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.ChemWatch ) )
                {
                    CswNbtMetaDataNodeTypeProp ChemWatchNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( PropertyName.ChemWatch );
                    docView.AddViewProperty( docView.Root.ChildRelationships[0].ChildRelationships[0], ChemWatchNTP );
                }

                CswNbtObjClassUser currentUserNode = _CswNbtResources.Nodes[_CswNbtResources.CurrentNbtUser.UserId];
                CswNbtObjClassJurisdiction userJurisdictionNode = _CswNbtResources.Nodes[currentUserNode.JurisdictionProperty.RelatedNodeId];

                ICswNbtTree docsTree = _CswNbtResources.Trees.getTreeFromView( docView, false, false, false );
                docsTree.goToNthChild( 0 ); //This is a property view, so the data is on the 2nd level
                int childCount = docsTree.getChildNodeCount();
                int lvlMatched = Int32.MinValue;
                string matchedFileType = "";
                CswNbtTreeNodeProp matchedFileProp = null;
                CswNbtTreeNodeProp matchedLinkProp = null;
                CswNbtTreeNodeProp matchedChemWatchProp = null;
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
                        CswNbtTreeNodeProp chemWatchProp = null;
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
                                case PropertyName.ChemWatch:
                                    chemWatchProp = prop;
                                    break;
                            }
                        }

                        if( lvlMatched < 0 )
                        {
                            matchedFileType = fileType;
                            matchedFileProp = fileProp;
                            matchedLinkProp = linkProp;
                            matchedChemWatchProp = chemWatchProp;
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
                                matchedChemWatchProp = chemWatchProp;
                                matchedNodeId = nodeId;
                                lvlMatched = 1;
                            }
                            if( lvlMatched < 2 && language.Equals( currentUserNode.Language ) )
                            {
                                matchedFileType = fileType;
                                matchedFileProp = fileProp;
                                matchedLinkProp = linkProp;
                                matchedChemWatchProp = chemWatchProp;
                                matchedNodeId = nodeId;
                                lvlMatched = 2;
                            }
                            if( lvlMatched < 3 && format.Equals( userJurisdictionNode.Format.Value ) && language.Equals( currentUserNode.Language ) )
                            {
                                matchedFileType = fileType;
                                matchedFileProp = fileProp;
                                matchedLinkProp = linkProp;
                                matchedChemWatchProp = chemWatchProp;
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
                            CswNbtMetaDataNodeTypeProp linkNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( PropertyName.Link );
                            url = CswNbtNodePropLink.GetFullURL( linkNTP.Attribute1, matchedLinkProp.Field1_Big, linkNTP.Attribute2 );
                            break;
                        case CswEnumDocumentFileTypes.ChemWatch:
                            url = "Services/ChemWatch/GetSDSDocument?filename=" + matchedChemWatchProp.Field1;
                            break;
                    }
                }
            }
            return url;
        }

        public static CswNbtView getAssignedSDSDocumentsView( CswNbtResources _CswNbtResources, CswPrimaryKey NodeId )
        {
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtMetaDataObjectClassProp AssignedSDS_OCP = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.AssignedSDS );
            CswNbtView docView = _CswNbtResources.ViewSelect.restoreView( AssignedSDS_OCP.ViewXml );
            docView = docView.PrepGridView( NodeId );
            return docView;
        }

        #endregion Custom Logic

        #region Object class specific properties

        public CswNbtNodePropList Language { get { return _CswNbtNode.Properties[PropertyName.Language]; } }
        private void OnLanguagePropChange( CswNbtNodeProp NodeProp, bool Creating )
        {
            archiveMatchingDocs();
        }
        public CswNbtNodePropList Format { get { return _CswNbtNode.Properties[PropertyName.Format]; } }
        private void OnFormatPropChange( CswNbtNodeProp NodeProp, bool Creating )
        {
            archiveMatchingDocs();
        }
        public CswNbtNodePropDateTime RevisionDate { get { return _CswNbtNode.Properties[PropertyName.RevisionDate]; } }
        public CswNbtNodePropText ChemWatch { get { return _CswNbtNode.Properties[PropertyName.ChemWatch]; } }
        public CswNbtNodePropText Country { get { return _CswNbtNode.Properties[PropertyName.Country]; } }

        #endregion Object class specific properties

    }//CswNbtObjClassSDSDocument

}//namespace ChemSW.Nbt.ObjClasses