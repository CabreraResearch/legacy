using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassReceiptLot : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            //public const string ReceiptLotNo = "Receipt Lot No"; //waiting on 27877
            public const string Material = "Material";
            //public const string MaterialID = "Material ID"; //waiting on 27864
            public const string ExpirationDate = "Expiration Date";
            //public const string Certificates = "Certificates"; //waiting for Certificate ObjClass to be implemented (allegedly in William)
            public const string UnderInvestigation = "Under Investigation";
            public const string InvestigationNotes = "Investigation Notes";
            public const string Manufacturer = "Manufacturer";
            public const string RequestItem = "Request Item";
            public const string ViewCofA = "View C of A";
        }


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassReceiptLot( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );

        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ReceiptLotClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassReceiptLot
        /// </summary>
        public static implicit operator CswNbtObjClassReceiptLot( CswNbtNode Node )
        {
            CswNbtObjClassReceiptLot ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.ReceiptLotClass ) )
            {
                ret = (CswNbtObjClassReceiptLot) Node.ObjClass;
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

        protected override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.triggerAfterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) 
            { 
                string OCPPropName = ButtonData.NodeTypeProp.getObjectClassPropName();
                switch( OCPPropName )
                {
                    case PropertyName.ViewCofA:
                        getCofA( ButtonData );
                        break;
                }
            }
            return true;
        }
        #endregion

        #region Custom Logic

        /// <summary>
        /// Gets the url for the active C of A Document attached to this ReceiptLot node.
        /// </summary>
        /// <param name="ButtonData">Data required for the client to open the file</param>
        public void getCofA( NbtButtonData ButtonData )
        {
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.CofA ) )
            {
                CswNbtMetaDataNodeType CofADocumentNT = _CswNbtResources.MetaData.getNodeType( "C of A Document" );
                if( null != CofADocumentNT )
                {
                    CswNbtMetaDataNodeTypeProp archivedNTP = CofADocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Archived );
                    CswNbtMetaDataNodeTypeProp fileTypeNTP = CofADocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.FileType );
                    CswNbtMetaDataNodeTypeProp fileNTP = CofADocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.File );
                    CswNbtMetaDataNodeTypeProp linkNTP = CofADocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Link );
                    CswNbtMetaDataNodeTypeProp ownerNTP = CofADocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Owner );

                    CswNbtView CofAView = new CswNbtView( _CswNbtResources );
                    CswNbtViewRelationship parent = CofAView.AddViewRelationship( CofADocumentNT, true );

                    CofAView.AddViewPropertyAndFilter( parent,
                                                      MetaDataProp: archivedNTP,
                                                      SubFieldName: CswEnumNbtSubFieldName.Checked,
                                                      Value: false.ToString(),
                                                      FilterMode: CswEnumNbtFilterMode.Equals );

                    CofAView.AddViewPropertyAndFilter( parent,
                                                      MetaDataProp: ownerNTP,
                                                      SubFieldName: CswEnumNbtSubFieldName.NodeID,
                                                      Value: NodeId.PrimaryKey.ToString(),
                                                      FilterMode: CswEnumNbtFilterMode.Equals );
                    CofAView.AddViewProperty( parent, fileNTP );
                    CofAView.AddViewProperty( parent, linkNTP );
                    CofAView.AddViewProperty( parent, fileTypeNTP );

                    ICswNbtTree CofATree = _CswNbtResources.Trees.getTreeFromView( CofAView, false, false, false );
                    if( CofATree.getChildNodeCount() > 0 )
                    {
                        CofATree.goToNthChild( 0 );
                        CswNbtObjClassDocument CofADoc = CofATree.getNodeForCurrentPosition();
                        string url = "";
                        switch( CofADoc.FileType.Value )
                        {
                            case CswNbtPropertySetDocument.CswEnumDocumentFileTypes.File:
                                url = CswNbtNodePropBlob.getLink( CofADoc.File.JctNodePropId, CofADoc.NodeId );
                                break;
                            case CswNbtPropertySetDocument.CswEnumDocumentFileTypes.Link:
                                url = CswNbtNodePropLink.GetFullURL( linkNTP.Attribute1, CofADoc.Link.Href, linkNTP.Attribute2 );
                                break;
                        }
                        ButtonData.Data["url"] = url;
                        ButtonData.Action = CswEnumNbtButtonAction.popup;
                    }
                    else
                    {
                        ButtonData.Message = "There are no active C of A assigned to this " + NodeType.NodeTypeName;
                        ButtonData.Action = CswEnumNbtButtonAction.nothing;
                    }
                }
            }
        }

        #endregion Custom Logic

        #region Object class specific properties

        //public CswNbtNodePropPropRefSequence ReceiptLotNo { get { return _CswNbtNode.Properties[PropertyName.ReceiptLotNo]; } } //waiting on 27877
        public CswNbtNodePropRelationship Material { get { return _CswNbtNode.Properties[PropertyName.Material]; } }
        //public CswNbtNodePropPropertyReference MaterialID { get { return _CswNbtNode.Properties[PropertyName.MaterialID]; } } //waiting on 27864
        public CswNbtNodePropDateTime ExpirationDate { get { return _CswNbtNode.Properties[PropertyName.ExpirationDate]; } }
        public CswNbtNodePropLogical UnderInvestigation { get { return _CswNbtNode.Properties[PropertyName.UnderInvestigation]; } }
        public CswNbtNodePropComments InvestigationNotes { get { return _CswNbtNode.Properties[PropertyName.InvestigationNotes]; } }
        public CswNbtNodePropRelationship Manufacturer { get { return _CswNbtNode.Properties[PropertyName.Manufacturer]; } }
        public CswNbtNodePropRelationship RequestItem { get { return _CswNbtNode.Properties[PropertyName.RequestItem]; } }
        public CswNbtNodePropButton ViewCofA { get { return _CswNbtNode.Properties[PropertyName.ViewCofA]; } }

        #endregion

    }//CswNbtObjClassReceiptLot

}//namespace ChemSW.Nbt.ObjClasses
