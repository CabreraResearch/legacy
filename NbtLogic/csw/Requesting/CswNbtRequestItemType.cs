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
    }
}
