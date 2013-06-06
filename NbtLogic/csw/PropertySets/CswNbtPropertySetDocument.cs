using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Document Property Set
    /// </summary>
    public abstract class CswNbtPropertySetDocument: CswNbtObjClass
    {
        #region Enums

        /// <summary>
        /// Object Class property names
        /// </summary>
        public new class PropertyName: CswNbtObjClass.PropertyName
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
            /// If FileType == File, the File
            /// </summary>
            public const string File = "File";
            /// <summary>
            /// If FileType == Link, the Link
            /// </summary>
            public const string Link = "Link ";
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
            /// Date document transitioned to Archive.
            /// </summary>
            public const string ArchiveDate = "Archive Date";
        }

        /// <summary>
        /// Potential File Types
        /// </summary>
        public sealed class CswEnumDocumentFileTypes
        {
            /// <summary>
            /// Blob
            /// </summary>
            public const string File = "File";
            /// <summary>
            /// Hyperlink
            /// </summary>
            public const string Link = "Link";
            public static CswCommaDelimitedString Options = new CswCommaDelimitedString { File, Link };
        }

        #endregion Enums

        #region Base

        /// <summary>
        /// Default Object Class for consumption by derived classes
        /// </summary>
        public CswNbtObjClassDefault CswNbtObjClassDefault = null;

        /// <summary>
        /// Property Set ctor
        /// </summary>
        protected CswNbtPropertySetDocument( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GenericClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtPropertySetDocument
        /// </summary>
        public static implicit operator CswNbtPropertySetDocument( CswNbtNode Node )
        {
            CswNbtPropertySetDocument ret = null;
            if( null != Node && Members().Contains( Node.ObjClass.ObjectClass.ObjectClass ) )
            {
                ret = (CswNbtPropertySetDocument) Node.ObjClass;
            }
            return ret;
        }

        public static Collection<CswEnumNbtObjectClass> Members()
        {
            Collection<CswEnumNbtObjectClass> Ret = new Collection<CswEnumNbtObjectClass>
            {
                CswEnumNbtObjectClass.DocumentClass,
                CswEnumNbtObjectClass.SDSDocumentClass,
                CswEnumNbtObjectClass.CofADocumentClass
            };
            return Ret;
        }

        #endregion Base

        #region Abstract Methods

        /// <summary>
        /// Before write node event for derived classes to implement
        /// </summary>
        public abstract void beforePropertySetWriteNode( bool IsCopy, bool OverrideUniqueValidation );

        /// <summary>
        /// After write node event for derived classes to implement
        /// </summary>
        public abstract void afterPropertySetWriteNode();

        /// <summary>
        /// Before delete node event for derived classes to implement
        /// </summary>
        public abstract void beforePropertySetDeleteNode( bool DeleteAllRequiredRelatedNodes = false );

        /// <summary>
        /// After delete node event for derived classes to implement
        /// </summary>
        public abstract void afterPropertySetDeleteNode();

        /// <summary>
        /// Populate props event for derived classes to implement
        /// </summary>
        public abstract void afterPropertySetPopulateProps();

        /// <summary>
        /// Button click event for derived classes to implement
        /// </summary>
        public abstract bool onPropertySetButtonClick( NbtButtonData ButtonData );

        /// <summary>
        /// Mechanism to add default filters in derived classes
        /// </summary>
        public abstract void onPropertySetAddDefaultViewFilters( CswNbtViewRelationship ParentRelationship );

        /// <summary>
        /// ObjectClass-specific logic for archiving existing matching Documents
        /// </summary>
        public abstract void archiveMatchingDocs();

        #endregion Abstract Methods

        #region Inherited Events

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            beforePropertySetWriteNode( IsCopy, OverrideUniqueValidation );
            CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }

        public override void afterWriteNode()
        {
            afterPropertySetWriteNode();
            CswNbtObjClassDefault.afterWriteNode();
        }

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            beforePropertySetDeleteNode( DeleteAllRequiredRelatedNodes );
            CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );
        }

        public override void afterDeleteNode()
        {
            afterPropertySetDeleteNode();
            CswNbtObjClassDefault.afterDeleteNode();
        }

        protected override void afterPopulateProps()
        {
            afterPropertySetPopulateProps();
            Owner.SetOnPropChange( OnOwnerPropChange );
            File.SetOnPropChange( OnFilePropChange );
            Link.SetOnPropChange( OnLinkPropChange );
            AcquiredDate.SetOnPropChange( OnAcquiredDatePropChange );
            Archived.SetOnPropChange( OnArchivedPropChange );
            FileType.SetOnPropChange( OnFileTypePropChange );
            CswNbtObjClassDefault.triggerAfterPopulateProps();
        }

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            onPropertySetAddDefaultViewFilters( ParentRelationship );
            CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }

        #endregion Inherited Events

        #region Property Set specific properties

        public CswNbtNodePropText Title { get { return _CswNbtNode.Properties[PropertyName.Title]; } }
        public CswNbtNodePropDateTime AcquiredDate { get { return _CswNbtNode.Properties[PropertyName.AcquiredDate]; } }
        private void OnAcquiredDatePropChange( CswNbtNodeProp NodeProp )
        {
            ArchiveDate.setHidden( value: true, SaveToDb: true );
        }
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
        private void OnFileTypePropChange( CswNbtNodeProp NodeProp )
        {
            //case 28755 - clear the File/Link prop, depending on what the FileType was changed to
            CswNbtNodePropWrapper wrapper;
            if( FileType.Value.Equals( CswEnumDocumentFileTypes.File ) )
            {
                wrapper = Node.Properties[PropertyName.Link];
            }
            else
            {
                wrapper = Node.Properties[PropertyName.File];
                wrapper.ClearBlob();
            }
            wrapper.ClearValue();
        }
        public CswNbtNodePropRelationship Owner { get { return _CswNbtNode.Properties[PropertyName.Owner]; } }
        private void OnOwnerPropChange( CswNbtNodeProp NodeProp )
        {
            if( CswTools.IsPrimaryKey( Owner.RelatedNodeId ) )
            {
                archiveMatchingDocs();
            }
        }
        public CswNbtNodePropLogical Archived { get { return _CswNbtNode.Properties[PropertyName.Archived]; } }
        private void OnArchivedPropChange( CswNbtNodeProp NodeProp )
        {
            ArchiveDate.setHidden( value: Archived.Checked != CswEnumTristate.True, SaveToDb: true );
            string ArchivedTitleSuffix = " (Archived)";
            if( Archived.Checked == CswEnumTristate.True )
            {
                ArchiveDate.DateTimeValue = DateTime.Now;
                Title.Text += ArchivedTitleSuffix;
            }
            else
            {
                ArchiveDate.DateTimeValue = DateTime.MinValue;
                if( Title.Text.EndsWith( ArchivedTitleSuffix ) )
                {
                    Title.Text = Title.Text.Substring( 0, Title.Text.Length - ArchivedTitleSuffix.Length );
                }
            }
        } // OnArchivedPropChange()
        public CswNbtNodePropDateTime ArchiveDate { get { return _CswNbtNode.Properties[PropertyName.ArchiveDate]; } }

        #endregion

    }//CswNbtPropertySetDocument

}//namespace ChemSW.Nbt.ObjClasses