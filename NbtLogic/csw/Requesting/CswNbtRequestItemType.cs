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
        /// Determines whether the given Prop should be hidden or readonly
        /// </summary>
        /// <param name="Prop">RequestItem Prop</param>
        public abstract void setPropUIVisibility( CswNbtNodeProp Prop );

        /// <summary>
        /// Sets the Request Item's description
        /// </summary>
        public abstract void setDescription();

        /// <summary>
        /// Sets the Request Item's Fulfill button menu options
        /// </summary>
        public abstract void setFulfillOptions();

        protected void setRecurringPropVisibility( CswNbtNodeProp Prop )
        {
            switch( Prop.PropName )
            {
                case CswNbtObjClassRequestItem.PropertyName.NeededBy:
                case CswNbtObjClassRequestItem.PropertyName.Priority:
                case CswNbtObjClassRequestItem.PropertyName.TotalDispensed:
                case CswNbtObjClassRequestItem.PropertyName.TotalMoved:
                case CswNbtObjClassRequestItem.PropertyName.Comments:
                case CswNbtObjClassRequestItem.PropertyName.FulfillmentHistory:
                    Prop.setHidden( true, SaveToDb: false );
                    break;
            }
        }
    }
}
