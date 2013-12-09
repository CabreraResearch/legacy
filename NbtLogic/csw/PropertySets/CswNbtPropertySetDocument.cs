using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;

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
            /// Type. Currently support File, Link, and ChemWatch (in SDS Document)
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
            /// <summary>
            /// Date the document was last modified
            /// </summary>
            public const string LastModifiedOn = "Last Modified On";
            /// <summary>
            /// User who last modified this document
            /// </summary>
            public const string LastModifiedBy = "Last Modified By";
            /// <summary>
            /// Opens file based on selected FileType
            /// </summary>
            public const string OpenFile = "Open File";
        }

        /// <summary>
        /// Potential File Types
        /// </summary>
        public class CswEnumDocumentFileTypes
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

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation, bool Creating )
        {
            beforePropertySetWriteNode( IsCopy, OverrideUniqueValidation );

            if( _CswNbtNode.Properties.Any( Prop => Prop.wasAnySubFieldModified() ) && false == IsTemp && false == _CswNbtResources.CurrentNbtUser is CswNbtSystemUser )
            {
                LastModifiedBy.RelatedNodeId = _CswNbtResources.CurrentNbtUser.UserId;
                LastModifiedBy.SyncGestalt();
                LastModifiedOn.DateTimeValue = DateTime.Now;
            }

            CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation, Creating );
        }

        public override void afterWriteNode( bool Creating )
        {
            afterPropertySetWriteNode();
            CswNbtObjClassDefault.afterWriteNode( Creating );
        }

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false, bool ValidateRequiredRelationships = true )
        {
            beforePropertySetDeleteNode( DeleteAllRequiredRelatedNodes );
            CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes, ValidateRequiredRelationships );
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
            bool HasPermission = true;
            if( null != ButtonData.NodeTypeProp )
            {
                HasPermission = false;
                string OCPPropName = ButtonData.NodeTypeProp.getObjectClassPropName();
                switch( OCPPropName )
                {
                    case PropertyName.OpenFile:
                        HasPermission = true;
                        string url = "";
                        if( FileType.Value.Equals( CswEnumDocumentFileTypes.File ) )
                        {
                            url = File.Href;
                        }
                        else if( FileType.Value.Equals( CswEnumDocumentFileTypes.Link ) )
                        {
                            url = Link.GetFullURL();
                        }
                        else if( FileType.Value.Equals( CswNbtObjClassSDSDocument.CswEnumDocumentFileTypes.ChemWatch ) )
                        {
                            //todo: check if this is actually an sdsdocument node
                            CswNbtObjClassSDSDocument SDSDocNode = Node;
                            if( null != SDSDocNode.ChemWatch && false == string.IsNullOrEmpty( SDSDocNode.ChemWatch.Text ) )
                            {
                                url = "Services/ChemWatch/GetSDSDocument?filename=" + SDSDocNode.ChemWatch.Text;
                            }
                        }

                        ButtonData.Data["url"] = url;
                        ButtonData.Action = CswEnumNbtButtonAction.popup;
                        break;
                }
            }
            return HasPermission;
        }

        public override CswNbtNode CopyNode( bool IsNodeTemp = false )
        {
            CswNbtNode NewNode = base.CopyNodeImpl( IsNodeTemp );   

            //Copy the existing file if there is any
            CswNbtSdBlobData.CopyBlobData( _CswNbtResources, this.File, ( NewNode.Properties[PropertyName.File].JctNodePropId ) );

            return NewNode;
        }

        #endregion Inherited Events

        #region Property Set specific properties

        public CswNbtNodePropText Title { get { return _CswNbtNode.Properties[PropertyName.Title]; } }
        public CswNbtNodePropDateTime AcquiredDate { get { return _CswNbtNode.Properties[PropertyName.AcquiredDate]; } }
        private void OnAcquiredDatePropChange( CswNbtNodeProp NodeProp, bool Creating )
        {
            ArchiveDate.setHidden( value : true, SaveToDb : true );
        }
        public CswNbtNodePropBlob File { get { return _CswNbtNode.Properties[PropertyName.File]; } }
        private void OnFilePropChange( CswNbtNodeProp NodeProp, bool Creating )
        {
            if( AcquiredDate.DateTimeValue == DateTime.MinValue &&
                false == string.IsNullOrEmpty( File.FileName ) )
            {
                AcquiredDate.DateTimeValue = DateTime.Now;
            }
        }
        public CswNbtNodePropLink Link { get { return _CswNbtNode.Properties[PropertyName.Link]; } }
        private void OnLinkPropChange( CswNbtNodeProp NodeProp, bool Creating )
        {
            if( AcquiredDate.DateTimeValue == DateTime.MinValue &&
                false == string.IsNullOrEmpty( Link.Href ) )
            {
                AcquiredDate.DateTimeValue = DateTime.Now;
            }
        }
        public CswNbtNodePropList FileType { get { return _CswNbtNode.Properties[PropertyName.FileType]; } }
        private void OnFileTypePropChange( CswNbtNodeProp NodeProp, bool Creating )
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
        private void OnOwnerPropChange( CswNbtNodeProp NodeProp, bool Creating )
        {
            if( CswTools.IsPrimaryKey( Owner.RelatedNodeId ) )
            {
                archiveMatchingDocs();
            }
        }
        public CswNbtNodePropLogical Archived { get { return _CswNbtNode.Properties[PropertyName.Archived]; } }
        private void OnArchivedPropChange( CswNbtNodeProp NodeProp, bool Creating )
        {
            ArchiveDate.setHidden( value : Archived.Checked != CswEnumTristate.True, SaveToDb : true );
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
        public CswNbtNodePropDateTime LastModifiedOn { get { return _CswNbtNode.Properties[PropertyName.LastModifiedOn]; } }
        public CswNbtNodePropRelationship LastModifiedBy { get { return _CswNbtNode.Properties[PropertyName.LastModifiedBy]; } }
        public CswNbtNodePropButton OpenFile { get { return _CswNbtNode.Properties[PropertyName.OpenFile]; } }
        #endregion

        #region Custom Logic

        public void MakeFilePropReadonly()
        {
            if( false == File.ReadOnly && false == IsTemp &&
                false == _CswNbtResources.CurrentNbtUser is CswNbtSystemUser )
            {
                File.setReadOnly( true, true );
            }
        }

        #endregion

    }//CswNbtPropertySetDocument

}//namespace ChemSW.Nbt.ObjClasses