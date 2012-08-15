using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassDocument : CswNbtObjClass
    {
        public sealed class PropertyName
        {
            public const string Title = "Title";
            public const string AcquiredDate = "Acquired Date";
            public const string ExpirationDate = "Expiration Date";
            public const string File = "File";
            public const string Link = "Link ";
            public const string DocumentClass = "Document Class";
            public const string FileType = "File Type";
            public const string Owner = "Owner";
            public const string Archived = "Archived";
        }

        public static CswCommaDelimitedString AllowedFileTypes = new CswCommaDelimitedString { "File", "Link" };

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassDocument( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.DocumentClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassDocument
        /// </summary>
        public static implicit operator CswNbtObjClassDocument( CswNbtNode Node )
        {
            CswNbtObjClassDocument ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.DocumentClass ) )
            {
                ret = (CswNbtObjClassDocument) Node.ObjClass;
            }
            return ret;
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

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            Owner.SetOnPropChange( OnOwnerPropChange );
            File.SetOnPropChange( OnFilePropChange );
            Link.SetOnPropChange( OnLinkPropChange );
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

        public CswNbtNodePropText Title { get { return _CswNbtNode.Properties[PropertyName.Title]; } }
        public CswNbtNodePropDateTime AcquiredDate { get { return _CswNbtNode.Properties[PropertyName.AcquiredDate]; } }
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
            if( Archived.Checked != Tristate.True &&
                Owner.RelatedNodeId != null &&
                false == string.IsNullOrEmpty( DocumentClass.Value ) )
            {
                CswNbtNode OwnerNode = _CswNbtResources.Nodes.GetNode( Owner.RelatedNodeId );
                if( null != OwnerNode )
                {
                    CswNbtView ExistingDocsView = new CswNbtView( _CswNbtResources );
                    CswNbtViewRelationship DocumentVr = ExistingDocsView.AddViewRelationship( NodeType, false );
                    ExistingDocsView.AddViewPropertyAndFilter( DocumentVr, Owner.NodeTypeProp, OwnerNode.NodeId.PrimaryKey.ToString(), CswNbtSubField.SubFieldName.NodeID );
                    ExistingDocsView.AddViewPropertyAndFilter( DocumentVr, DocumentClass.NodeTypeProp, DocumentClass.Value );
                    ExistingDocsView.AddViewPropertyAndFilter( DocumentVr, Archived.NodeTypeProp, Tristate.True.ToString(), FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals );

                    ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( ExistingDocsView, true, false );
                    Int32 DocCount = Tree.getChildNodeCount();
                    if( DocCount > 0 )
                    {
                        for( Int32 I = 0; I < DocCount; I += 1 )
                        {
                            Tree.goToNthChild( I );
                            CswNbtNode DocNode = Tree.getNodeForCurrentPosition();
                            if( DocNode.NodeId != NodeId )
                            {
                                CswNbtObjClassDocument DocNodeAsDocument = (CswNbtObjClassDocument) DocNode;
                                DocNodeAsDocument.Archived.Checked = Tristate.True;
                                DocNode.postChanges( true );
                            }
                            Tree.goToParentNode();
                        }
                    }

                }
            }
        }

        public CswNbtNodePropLogical Archived { get { return _CswNbtNode.Properties[PropertyName.Archived]; } }

        #endregion
    }//CswNbtObjClassDocument

}//namespace ChemSW.Nbt.ObjClasses
