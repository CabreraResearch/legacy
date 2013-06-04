using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassSDSDocument : CswNbtPropertySetDocument
    {
        #region Enums
        /// <summary>
        /// Object Class Property Names
        /// </summary>
        public new sealed class PropertyName : CswNbtPropertySetDocument.PropertyName
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
        /// Cast a Request Item PropertySet back to an Object Class
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

        public override void beforePropertySetWriteNode( bool IsCopy, bool OverrideUniqueValidation ) { }

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
                    ExistingDocsView.AddViewPropertyAndFilter( DocumentVr, Owner.NodeTypeProp, OwnerNode.NodeId.PrimaryKey.ToString(), CswEnumNbtSubFieldName.NodeID );
                    ExistingDocsView.AddViewPropertyAndFilter( DocumentVr, Archived.NodeTypeProp, CswEnumTristate.True.ToString(), FilterMode: CswEnumNbtFilterMode.NotEquals );
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
                                CswNbtObjClassDocument DocNodeAsDocument = DocNode;
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