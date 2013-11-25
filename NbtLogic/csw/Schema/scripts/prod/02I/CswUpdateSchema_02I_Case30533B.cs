using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case30533B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30533; }
        }

        public override string AppendToScriptName()
        {
            return "F";
        }

        public override string Title
        {
            get { return "Organize Request Item NodeType Layout"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtMetaDataObjectClass RequestItemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestItemClass );
            foreach( CswNbtMetaDataNodeType RequestItemNT in RequestItemOC.getNodeTypes() )
            {
                //Set NewMaterialType default value
                CswNbtMetaDataNodeTypeProp NewMaterialTypeNTP = RequestItemNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.NewMaterialType );
                if( null != ChemicalOC.FirstNodeType && null != NewMaterialTypeNTP )
                {
                    NewMaterialTypeNTP.DefaultValue.AsNodeTypeSelect.SelectedNodeTypeIds.Add( ChemicalOC.FirstNodeType.NodeTypeId.ToString() );
                }
                CswNbtMetaDataNodeTypeProp DescriptionNTP = RequestItemNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Description );
                //Set RequestItem name template
                RequestItemNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( DescriptionNTP.PropName ) );
                //Hide Internal Flags and States
                CswNbtMetaDataNodeTypeProp TypeNTP = RequestItemNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Type );
                TypeNTP.Hidden = true;
                CswNbtMetaDataNodeTypeProp IsFavoriteNTP = RequestItemNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.IsFavorite );
                IsFavoriteNTP.Hidden = true;
                CswNbtMetaDataNodeTypeProp IsRecurringNTP = RequestItemNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.IsRecurring );
                IsRecurringNTP.Hidden = true;

                //TODO - fill in ReceiptLots Received grid definition (also, filter receipt lots by requested material ...and EP)

                //Remove all props from all layouts
                foreach( CswNbtMetaDataNodeTypeProp Prop in RequestItemNT.getNodeTypeProps() )
                {
                    if( Prop.PropName != CswNbtObjClass.PropertyName.Save )
                    {
                        Prop.removeFromAllLayouts();
                    }
                }
            }

            _updateAddLayout();
            _updateEditLayout();
            _updatePreviewLayout();
            _updateTableLayout();
        } // update()

        private void _updateAddLayout()
        {
            CswNbtSchemaUpdateLayoutMgr AddLayoutMgr = new CswNbtSchemaUpdateLayoutMgr( _CswNbtSchemaModTrnsctn, CswEnumNbtObjectClass.RequestItemClass, LayoutType: CswEnumNbtLayoutType.Add );

            AddLayoutMgr.First.moveProp( Row: 1, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.RequestedFor );
            AddLayoutMgr.First.moveProp( Row: 2, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.NewMaterialType );
            AddLayoutMgr.First.moveProp( Row: 3, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.NewMaterialTradename );
            AddLayoutMgr.First.moveProp( Row: 4, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.NewMaterialSupplier );
            AddLayoutMgr.First.moveProp( Row: 5, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.NewMaterialPartNo );
            AddLayoutMgr.First.moveProp( Row: 6, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Quantity );
            AddLayoutMgr.First.moveProp( Row: 7, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Size );
            AddLayoutMgr.First.moveProp( Row: 8, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.SizeCount );
            AddLayoutMgr.First.moveProp( Row: 9, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.ExternalOrderNumber );
            AddLayoutMgr.First.moveProp( Row: 10, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.NeededBy );
            AddLayoutMgr.First.moveProp( Row: 11, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.InventoryGroup );
            AddLayoutMgr.First.moveProp( Row: 12, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Location );
        }

        private void _updateEditLayout()
        {
            CswNbtSchemaUpdateLayoutMgr LayoutMgr = new CswNbtSchemaUpdateLayoutMgr( _CswNbtSchemaModTrnsctn, CswEnumNbtObjectClass.RequestItemClass, LayoutType: CswEnumNbtLayoutType.Edit );

            LayoutMgr.Identity.moveProp( Row: 1, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.RequestType );
            LayoutMgr.Identity.moveProp( Row: 2, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Description );
            LayoutMgr.Identity.moveProp( Row: 3, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Material );
            LayoutMgr.Identity.moveProp( Row: 4, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.EnterprisePart );
            LayoutMgr.Identity.moveProp( Row: 5, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Container );
            LayoutMgr.Identity.moveProp( Row: 6, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Fulfill );

            LayoutMgr.First.moveProp( Row: 1, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.ItemNumber );
            LayoutMgr.First.moveProp( Row: 2, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.IsBatch );
            LayoutMgr.First.moveProp( Row: 3, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Requestor );
            LayoutMgr.First.moveProp( Row: 4, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Status );
            LayoutMgr.First.moveProp( Row: 5, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.NewMaterialTradename );
            LayoutMgr.First.moveProp( Row: 6, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.NewMaterialSupplier );
            LayoutMgr.First.moveProp( Row: 7, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Quantity );
            LayoutMgr.First.moveProp( Row: 8, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Size );
            LayoutMgr.First.moveProp( Row: 9, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Location );
            LayoutMgr.First.moveProp( Row: 10, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.NeededBy );
            LayoutMgr.First.moveProp( Row: 11, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.TotalDispensed );
            LayoutMgr.First.moveProp( Row: 12, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.RecurringFrequency );
            LayoutMgr.First.moveProp( Row: 13, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Comments );

            LayoutMgr.First.moveProp( Row: 1, Column: 2, PropName: CswNbtObjClassRequestItem.PropertyName.ExternalOrderNumber );
            LayoutMgr.First.moveProp( Row: 2, Column: 2, PropName: CswNbtObjClassRequestItem.PropertyName.BatchNumber );
            LayoutMgr.First.moveProp( Row: 3, Column: 2, PropName: CswNbtObjClassRequestItem.PropertyName.RequestedFor );
            LayoutMgr.First.moveProp( Row: 4, Column: 2, PropName: CswNbtObjClassRequestItem.PropertyName.AssignedTo );
            LayoutMgr.First.moveProp( Row: 5, Column: 2, PropName: CswNbtObjClassRequestItem.PropertyName.NewMaterialPartNo );
            LayoutMgr.First.moveProp( Row: 6, Column: 2, PropName: CswNbtObjClassRequestItem.PropertyName.NewMaterialType );
            LayoutMgr.First.moveProp( Row: 7, Column: 2, PropName: CswNbtObjClassRequestItem.PropertyName.CertificationLevel );
            LayoutMgr.First.moveProp( Row: 8, Column: 2, PropName: CswNbtObjClassRequestItem.PropertyName.SizeCount );
            LayoutMgr.First.moveProp( Row: 9, Column: 2, PropName: CswNbtObjClassRequestItem.PropertyName.InventoryGroup );
            LayoutMgr.First.moveProp( Row: 10, Column: 2, PropName: CswNbtObjClassRequestItem.PropertyName.Priority );
            LayoutMgr.First.moveProp( Row: 11, Column: 2, PropName: CswNbtObjClassRequestItem.PropertyName.TotalMoved );
            LayoutMgr.First.moveProp( Row: 12, Column: 2, PropName: CswNbtObjClassRequestItem.PropertyName.NextReorderDate );
            LayoutMgr.First.moveProp( Row: 13, Column: 2, PropName: CswNbtObjClassRequestItem.PropertyName.FulfillmentHistory );

            //Receipt Lots tab
            LayoutMgr["Receipt Lots"].copyProp( Row: 1, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.GoodsReceived );
            LayoutMgr["Receipt Lots"].copyProp( Row: 2, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.ReceiptLotToDispense );
            LayoutMgr["Receipt Lots"].copyProp( Row: 3, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.ReceiptLotsReceived );
        }

        private void _updatePreviewLayout()
        {
            CswNbtSchemaUpdateLayoutMgr PreviewLayoutMgr = new CswNbtSchemaUpdateLayoutMgr( _CswNbtSchemaModTrnsctn, CswEnumNbtObjectClass.RequestItemClass, LayoutType: CswEnumNbtLayoutType.Preview );

            PreviewLayoutMgr.First.moveProp( Row: 1, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Description );
            PreviewLayoutMgr.First.moveProp( Row: 2, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.ExternalOrderNumber );
            PreviewLayoutMgr.First.moveProp( Row: 3, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.EnterprisePart );
            PreviewLayoutMgr.First.moveProp( Row: 4, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Material );
            PreviewLayoutMgr.First.moveProp( Row: 5, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Container );
            PreviewLayoutMgr.First.moveProp( Row: 6, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.NewMaterialTradename );
            PreviewLayoutMgr.First.moveProp( Row: 7, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.NewMaterialSupplier );
            PreviewLayoutMgr.First.moveProp( Row: 8, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.NewMaterialPartNo );
            PreviewLayoutMgr.First.moveProp( Row: 9, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Quantity );
            PreviewLayoutMgr.First.moveProp( Row: 10, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.SizeCount );
            PreviewLayoutMgr.First.moveProp( Row: 11, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Size );
            PreviewLayoutMgr.First.moveProp( Row: 12, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Location );
        }

        private void _updateTableLayout()
        {
            CswNbtSchemaUpdateLayoutMgr TableLayoutMgr = new CswNbtSchemaUpdateLayoutMgr( _CswNbtSchemaModTrnsctn, CswEnumNbtObjectClass.RequestItemClass, LayoutType: CswEnumNbtLayoutType.Table );

            TableLayoutMgr.First.moveProp( Row: 1, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.ItemNumber );
            TableLayoutMgr.First.moveProp( Row: 2, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.ExternalOrderNumber );
            TableLayoutMgr.First.moveProp( Row: 3, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.InventoryGroup );
            TableLayoutMgr.First.moveProp( Row: 4, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Fulfill );
        }

    }
}//namespace ChemSW.Nbt.Schema