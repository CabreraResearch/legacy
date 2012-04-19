using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassDocument : CswNbtObjClass
    {
        public static string TitlePropertyName { get { return "Title"; } }
        public static string AcquiredDatePropertyName { get { return "Acquired Date"; } }
        public static string ExpirationDatePropertyName { get { return "Expiration Date"; } }
        public static string FilePropertyName { get { return "File"; } }
        public static string LinkPropertyName { get { return "Link "; } }
        public static string DocumentClassPropertyName { get { return "Document Class"; } }
        public static string FileTypePropertyName { get { return "File Type"; } }
        public static string OpenPropertyName { get { return "Open"; } }
        public static string OwnerPropertyName { get { return "Owner"; } }
        public static string ArchivedPropertyName { get { return "Archived"; } }

        public static CswCommaDelimitedString AllowedFileTypes = new CswCommaDelimitedString { "File", "Link" };

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassDocument( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass ); }
        }

        #region Inherited Events
        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode()
        {
            _CswNbtObjClassDefault.beforeDeleteNode();

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp, out NbtButtonAction ButtonAction, out string ActionData, out string Message )
        {
            Message = string.Empty;
            ActionData = string.Empty;
            ButtonAction = NbtButtonAction.Unknown;
            if( null != NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropText Title
        {
            get { return _CswNbtNode.Properties[TitlePropertyName].AsText; }
        }

        public CswNbtNodePropDateTime AcquiredDate
        {
            get { return _CswNbtNode.Properties[AcquiredDatePropertyName].AsDateTime; }
        }

        public CswNbtNodePropDateTime ExpirationDate
        {
            get { return _CswNbtNode.Properties[ExpirationDatePropertyName].AsDateTime; }
        }

        public CswNbtNodePropBlob File
        {
            get { return _CswNbtNode.Properties[FilePropertyName].AsBlob; }
        }

        public CswNbtNodePropLink Link
        {
            get { return _CswNbtNode.Properties[LinkPropertyName].AsLink; }
        }

        public CswNbtNodePropList FileType
        {
            get { return _CswNbtNode.Properties[FileTypePropertyName].AsList; }
        }

        public CswNbtNodePropList DocumentClass
        {
            get { return _CswNbtNode.Properties[DocumentClassPropertyName].AsList; }
        }

        public CswNbtNodePropButton Open
        {
            get { return _CswNbtNode.Properties[OpenPropertyName].AsButton; }
        }

        public CswNbtNodePropRelationship Owner
        {
            get { return _CswNbtNode.Properties[OwnerPropertyName].AsRelationship; }
        }

        public CswNbtNodePropLogical Archived
        {
            get { return _CswNbtNode.Properties[ArchivedPropertyName].AsLogical; }
        }

        #endregion
    }//CswNbtObjClassDocument

}//namespace ChemSW.Nbt.ObjClasses
