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
            public const string UnderInvestigation = "Under Investigation";
            public const string InvestigationNotes = "Investigation Notes";
            public const string Manufacturer = "Manufacturer";
            public const string ManufacturerLotNo = "Manufacturer Lot No";
            public const string ManufacturedDate = "Manufactured Date";
            public const string RequestItem = "Request Item";
            public const string AssignedCofA = "Assigned C of A";
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

        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
        }

        public override void afterCreateNode()
        {
        }

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            ViewCofA.State = PropertyName.ViewCofA;
            ViewCofA.MenuOptions = PropertyName.ViewCofA + ",View All";
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
            if( false == CswNbtObjClassCofADocument.receiptLotHasActiveCofA( _CswNbtResources, NodeId ) )
            {
                ViewCofA.setHidden( true, false );
            }
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
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.ManufacturerLotInfo ) )
            {
                if( ButtonData.SelectedText.Equals( PropertyName.ViewCofA ) )
                {
                    CswNbtObjClassCofADocument CofADoc = CswNbtObjClassCofADocument.getActiveCofADocument( _CswNbtResources, NodeId );
                    if( null != CofADoc )
                    {
                        string url = "";
                        switch( CofADoc.FileType.Value )
                        {
                            case CswNbtPropertySetDocument.CswEnumDocumentFileTypes.File:
                                url = CswNbtNodePropBlob.getLink( CofADoc.File.JctNodePropId, CofADoc.NodeId );
                                break;
                            case CswNbtPropertySetDocument.CswEnumDocumentFileTypes.Link:
                                url = CswNbtNodePropLink.GetFullURL( CofADoc.Link.Prefix, CofADoc.Link.Href, CofADoc.Link.Suffix );
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
                else
                {
                    CswNbtView AssignedCofADocsView = CswNbtObjClassCofADocument.getAssignedCofADocumentsView( _CswNbtResources, NodeId );
                    if( null != AssignedCofADocsView )
                    {
                        ButtonData.Data["viewid"] = AssignedCofADocsView.SessionViewId.ToString();
                        ButtonData.Data["title"] = AssignedCofADocsView.ViewName;
                        ButtonData.Data["nodeid"] = NodeId.ToString();
                        ButtonData.Data["nodetypeid"] = NodeTypeId.ToString();
                        ButtonData.Action = CswEnumNbtButtonAction.griddialog;
                    }
                    else
                    {
                        ButtonData.Message = "Could not find the Assigned C of A prop";
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
        public CswNbtNodePropText ManufacturerLotNo { get { return _CswNbtNode.Properties[PropertyName.ManufacturerLotNo]; } }
        public CswNbtNodePropDateTime ManufacturedDate { get { return _CswNbtNode.Properties[PropertyName.ManufacturedDate]; } }
        public CswNbtNodePropRelationship RequestItem { get { return _CswNbtNode.Properties[PropertyName.RequestItem]; } }
        public CswNbtNodePropGrid AssignedCofA { get { return _CswNbtNode.Properties[PropertyName.AssignedCofA]; } }
        public CswNbtNodePropButton ViewCofA { get { return _CswNbtNode.Properties[PropertyName.ViewCofA]; } }

        #endregion

    }//CswNbtObjClassReceiptLot

}//namespace ChemSW.Nbt.ObjClasses
