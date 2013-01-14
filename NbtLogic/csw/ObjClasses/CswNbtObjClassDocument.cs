using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassDocument : CswNbtObjClass
    {
        #region Public Sealed Classes
        /// <summary>
        /// Object Class Property Names
        /// </summary>
        public sealed class PropertyName
        {
            /// <summary>
            /// Basis for the name of the Document
            /// </summary>
            public const string Title = "Title";
            /// <summary>
            /// Date document was created
            /// </summary>
            public const string AcquiredDate = "Acquired Date";
            /// <summary>
            /// Expiration Date, if any
            /// </summary>
            public const string ExpirationDate = "Expiration Date";
            /// <summary>
            /// If FileType == File, the File
            /// </summary>
            public const string File = "File";
            /// <summary>
            /// If FileType == Link, the Link
            /// </summary>
            public const string Link = "Link ";
            /// <summary>
            /// Class. Currently support SDS and CofA in Business Logic
            /// </summary>
            public const string DocumentClass = "Document Class";
            /// <summary>
            /// Type. Currently support File and List
            /// </summary>
            public const string FileType = "File Type";
            /// <summary>
            /// Document owner: Material, Equipment, etc
            /// </summary>
            public const string Owner = "Owner";
            /// <summary>
            /// Archive status
            /// </summary>
            public const string Archived = "Archived";
            /// <summary>
            /// Language of the document. Conditional on Document Class == SDS
            /// </summary>
            public const string Language = "Language";
            /// <summary>
            /// Format of the document. Conditional on Document Class == SDS
            /// </summary>
            public const string Format = "Format";
            /// <summary>
            /// Date document transitioned to Archive.
            /// </summary>
            public const string ArchiveDate = "Archive Date";
        }

        /// <summary>
        /// Potential File Types
        /// </summary>
        public sealed class FileTypes
        {
            /// <summary>
            /// Blob
            /// </summary>
            public const string File = "File";
            /// <summary>
            /// Hyperlink
            /// </summary>
            public const string Link = "Link";
            /// <summary>
            /// Options
            /// </summary>
            public static CswCommaDelimitedString Options = new CswCommaDelimitedString { File, Link };
        }

        /// <summary>
        /// Document Classes recognized by Business Logic
        /// </summary>
        public sealed class DocumentClasses
        {
            /// <summary>
            /// No associated class (Default)
            /// </summary>
            public const string None = "";
            /// <summary>
            /// (Material) Safety Data Sheet
            /// </summary>
            public const string SDS = "SDS";
            /// <summary>
            /// Certificate of Analysis
            /// </summary>
            public const string CofA = "CofA";
        }

        /// <summary>
        /// Formats recognized by Business Logic
        /// </summary>
        public sealed class Formats
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

        #endregion Public Sealed Classes

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassDocument( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.DocumentClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassDocument
        /// </summary>
        public static implicit operator CswNbtObjClassDocument( CswNbtNode Node )
        {
            CswNbtObjClassDocument ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.DocumentClass ) )
            {
                ret = (CswNbtObjClassDocument) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _updateOwnerViewSDSButtonOpts();

            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _updateOwnerViewSDSButtonOpts();

            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            Owner.SetOnPropChange( OnOwnerPropChange );
            File.SetOnPropChange( OnFilePropChange );
            Link.SetOnPropChange( OnLinkPropChange );
            AcquiredDate.SetOnPropChange( OnAcquiredDatePropChange );
            Archived.SetOnPropChange( OnArchivedPropChange );
            Language.SetOnPropChange( OnLanguagePropChange );
            Format.SetOnPropChange( OnFormatPropChange );
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        private void _archiveMatchingDocs()
        {
            //If the document is not already archived and ( has a Document Class which is not SDS or is SDS and has a Language and Format ), 
            //then archive existing Docs with the same property values
            if( Archived.Checked != Tristate.True &&
                false == string.IsNullOrEmpty( DocumentClass.Value ) &&
                ( DocumentClass.Value != DocumentClasses.SDS || (
                false == string.IsNullOrEmpty( Format.Value ) &&
                false == string.IsNullOrEmpty( Language.Value ) ) ) )
            {
                CswNbtNode OwnerNode = _CswNbtResources.Nodes.GetNode( Owner.RelatedNodeId );
                if( null != OwnerNode )
                {
                    CswNbtView ExistingDocsView = new CswNbtView( _CswNbtResources );
                    CswNbtViewRelationship DocumentVr = ExistingDocsView.AddViewRelationship( NodeType, false );
                    ExistingDocsView.AddViewPropertyAndFilter( DocumentVr, Owner.NodeTypeProp, OwnerNode.NodeId.PrimaryKey.ToString(), CswNbtSubField.SubFieldName.NodeID );
                    ExistingDocsView.AddViewPropertyAndFilter( DocumentVr, DocumentClass.NodeTypeProp, DocumentClass.Value );
                    ExistingDocsView.AddViewPropertyAndFilter( DocumentVr, Archived.NodeTypeProp, Tristate.True.ToString(), FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals );

                    if( DocumentClass.Value == DocumentClasses.SDS )
                    {
                        ExistingDocsView.AddViewPropertyAndFilter( DocumentVr, Format.NodeTypeProp, Format.Value );
                        ExistingDocsView.AddViewPropertyAndFilter( DocumentVr, Language.NodeTypeProp, Language.Value );
                    }

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
                                CswNbtObjClassDocument DocNodeAsDocument = DocNode;
                                DocNodeAsDocument.Archived.Checked = Tristate.True;
                                DocNode.postChanges( true );
                            }
                            Tree.goToParentNode();
                        }
                    }

                }
            }
        }

        public CswNbtNodePropText Title { get { return _CswNbtNode.Properties[PropertyName.Title]; } }
        public CswNbtNodePropDateTime AcquiredDate { get { return _CswNbtNode.Properties[PropertyName.AcquiredDate]; } }
        private void OnAcquiredDatePropChange( CswNbtNodeProp NodeProp )
        {
            ArchiveDate.setHidden( value: true, SaveToDb: true );
        }
        public CswNbtNodePropDateTime ExpirationDate { get { return _CswNbtNode.Properties[PropertyName.ExpirationDate]; } }
        public CswNbtNodePropBlob File { get { return _CswNbtNode.Properties[PropertyName.File]; } }
        private void OnFilePropChange( CswNbtNodeProp NodeProp )
        {
            if( AcquiredDate.DateTimeValue == DateTime.MinValue &&
                false == string.IsNullOrEmpty( File.FileName ) )
            {
                AcquiredDate.DateTimeValue = DateTime.Now;
            }
        }
        public CswNbtNodePropLink Link { get { return _CswNbtNode.Properties[PropertyName.Link]; } }
        private void OnLinkPropChange( CswNbtNodeProp NodeProp )
        {
            if( AcquiredDate.DateTimeValue == DateTime.MinValue &&
                false == string.IsNullOrEmpty( Link.Href ) )
            {
                AcquiredDate.DateTimeValue = DateTime.Now;
            }
        }
        public CswNbtNodePropList FileType { get { return _CswNbtNode.Properties[PropertyName.FileType]; } }
        public CswNbtNodePropList DocumentClass { get { return _CswNbtNode.Properties[PropertyName.DocumentClass]; } }
        public CswNbtNodePropRelationship Owner { get { return _CswNbtNode.Properties[PropertyName.Owner]; } }
        private void OnOwnerPropChange( CswNbtNodeProp NodeProp )
        {
            if( CswTools.IsPrimaryKey( Owner.RelatedNodeId ) )
            {
                _archiveMatchingDocs();
            }
        }
        public CswNbtNodePropLogical Archived { get { return _CswNbtNode.Properties[PropertyName.Archived]; } }
        private void OnArchivedPropChange( CswNbtNodeProp NodeProp )
        {
            ArchiveDate.setHidden( value: Archived.Checked != Tristate.True, SaveToDb: true );
            if( Archived.Checked == Tristate.True )
            {
                ArchiveDate.DateTimeValue = DateTime.Now;
                Title.Text += " (Archived)";
            }
        }

        public CswNbtNodePropList Language { get { return _CswNbtNode.Properties[PropertyName.Language]; } }
        private void OnLanguagePropChange( CswNbtNodeProp NodeProp )
        {
            _archiveMatchingDocs();
        }

        public CswNbtNodePropList Format { get { return _CswNbtNode.Properties[PropertyName.Format]; } }
        private void OnFormatPropChange( CswNbtNodeProp NodeProp )
        {
            _archiveMatchingDocs();
        }
        public CswNbtNodePropDateTime ArchiveDate { get { return _CswNbtNode.Properties[PropertyName.ArchiveDate]; } }

        #endregion

        #region Custom Logic

        private void _updateOwnerViewSDSButtonOpts()
        {
            if( false == IsTemp && DocumentClass.Value.Equals( DocumentClasses.SDS ) )
            {
                CswNbtObjClassMaterial material = _CswNbtResources.Nodes[Owner.RelatedNodeId];
                if( null != material )
                {
                    material.UpdateViewSDSButtonOpts();
                    material.postChanges( false );
                }
            }
        }

        #endregion

    }//CswNbtObjClassDocument

}//namespace ChemSW.Nbt.ObjClasses
