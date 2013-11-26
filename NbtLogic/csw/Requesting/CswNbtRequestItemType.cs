using System;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Requesting
{
    public abstract class CswNbtRequestItemType
    {
        public String RequestItemType;
        protected CswNbtResources _CswNbtResources;
        protected CswNbtObjClassRequestItem _RequestItem;

        protected CswNbtRequestItemType( CswNbtResources CswNbtResources, CswNbtObjClassRequestItem RequestItem )
        {
            _CswNbtResources = CswNbtResources;
            _RequestItem = RequestItem;
            //Set RequestItemType explicitly in the constructor in case the referenced RequestItem object's Type value changes
            RequestItemType = _RequestItem.Type.Value;
        }      

        /// <summary>
        /// Returns the Target property for the RequestItem.
        /// </summary>
        public abstract CswNbtNodePropRelationship Target { get; }

        /// <summary>
        /// Sets the Request Item's description
        /// </summary>
        public abstract void setDescription();

        /// <summary>
        /// Sets the Request Item's Fulfill button menu options
        /// </summary>
        public abstract void setFulfillOptions();

        #region UI Visibility Logic

        /// <summary>
        /// Determines whether the given Prop should be hidden or readonly
        /// </summary>
        /// <param name="Prop">RequestItem Prop</param>
        public abstract void setPropUIVisibility( CswNbtNodeProp Prop );

        /// <summary>
        /// Trigger logic to hide/show/edit properties based on Type, Status, and other attributes
        /// </summary>
        public void setUIVisibility( CswNbtNodeProp Prop )
        {
            setPropVisibilityBasedOnPendingStatus( Prop );
            setPropUIVisibility( Prop );
        }

        /// <summary>
        /// Hide props that should not be present on Request Items that are marked as Recurring
        /// </summary>
        protected void setRecurringPropVisibility( CswNbtNodeProp Prop )
        {
            switch( Prop.PropName )
            {
                case CswNbtObjClassRequestItem.PropertyName.ItemNumber:
                case CswNbtObjClassRequestItem.PropertyName.AssignedTo:
                case CswNbtObjClassRequestItem.PropertyName.NeededBy:
                case CswNbtObjClassRequestItem.PropertyName.Priority:
                case CswNbtObjClassRequestItem.PropertyName.TotalDispensed:
                case CswNbtObjClassRequestItem.PropertyName.TotalMoved:
                case CswNbtObjClassRequestItem.PropertyName.Comments:
                case CswNbtObjClassRequestItem.PropertyName.FulfillmentHistory:
                case CswNbtObjClassRequestItem.PropertyName.ReceiptLotToDispense:
                case CswNbtObjClassRequestItem.PropertyName.ReceiptLotsReceived:
                case CswNbtObjClassRequestItem.PropertyName.GoodsReceived:
                    Prop.setHidden( true, SaveToDb: false );
                    break;
            }
        }

        /// <summary>
        /// Hide props that should not be present on Pending Request Items; 
        /// Make props that should not be edited once Request Item has been submitted readonly 
        /// </summary>
        protected void setPropVisibilityBasedOnPendingStatus( CswNbtNodeProp Prop )
        {
            if( _RequestItem.Status.Value == CswNbtObjClassRequestItem.Statuses.Pending )
            {
                switch( Prop.PropName )
                {
                    case CswNbtObjClassRequestItem.PropertyName.Requestor:
                    case CswNbtObjClassRequestItem.PropertyName.Priority:
                    case CswNbtObjClassRequestItem.PropertyName.TotalDispensed:
                    case CswNbtObjClassRequestItem.PropertyName.TotalMoved:
                    case CswNbtObjClassRequestItem.PropertyName.FulfillmentHistory:
                    case CswNbtObjClassRequestItem.PropertyName.ReceiptLotToDispense:
                    case CswNbtObjClassRequestItem.PropertyName.ReceiptLotsReceived:
                    case CswNbtObjClassRequestItem.PropertyName.GoodsReceived:
                        Prop.setHidden( true, SaveToDb: false );
                        break;
                }
            }
            else
            {
                switch( Prop.PropName )
                {
                    case CswNbtObjClassRequestItem.PropertyName.InventoryGroup:
                    case CswNbtObjClassRequestItem.PropertyName.Location:
                    case CswNbtObjClassRequestItem.PropertyName.Requestor:
                    case CswNbtObjClassRequestItem.PropertyName.Quantity:
                    case CswNbtObjClassRequestItem.PropertyName.Size:
                    case CswNbtObjClassRequestItem.PropertyName.SizeCount:
                    case CswNbtObjClassRequestItem.PropertyName.NewMaterialType:
                    case CswNbtObjClassRequestItem.PropertyName.NewMaterialTradename:
                    case CswNbtObjClassRequestItem.PropertyName.NewMaterialSupplier:
                    case CswNbtObjClassRequestItem.PropertyName.NewMaterialPartNo:
                        Prop.setReadOnly( true, SaveToDb: false );
                        break;
                }
            }
        }

        #endregion UI Visibility Logic
    }
}
