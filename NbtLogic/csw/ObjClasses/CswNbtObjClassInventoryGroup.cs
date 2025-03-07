using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropertySets;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassInventoryGroup: CswNbtObjClass, ICswNbtPermissionGroup
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Name = "Name";
            public const string Central = "Central";
            public const string AutomaticCertificateApproval = "Automatic Certificate Approval";
            public const string ManageLocations = "Manage Locations";
            public const string Description = "Description";
            public const string Locations = "Locations";
            public const string Permissions = "Permissions";
            public const string LimitRequestDeliveryLocation = "Limit Request Delivery Location";
        }

        public CswEnumNbtObjectClass PermissionClass { get { return CswEnumNbtObjectClass.InventoryGroupPermissionClass; } }
        public CswEnumNbtObjectClass TargetClass { get { return CswEnumNbtObjectClass.ContainerClass; } }

        public CswNbtObjClassInventoryGroup( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryGroupClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassInventoryGroup
        /// </summary>
        public static implicit operator CswNbtObjClassInventoryGroup( CswNbtNode Node )
        {
            CswNbtObjClassInventoryGroup ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.InventoryGroupClass ) )
            {
                ret = (CswNbtObjClassInventoryGroup) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        protected override void afterPromoteNodeLogic()
        {
            CswNbtPropertySetPermission.createDefaultWildcardPermission( _CswNbtResources, PermissionClass, NodeId );
        }  

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp )
            {
                if( PropertyName.ManageLocations == ButtonData.NodeTypeProp.getObjectClassPropName() )
                {
                    ButtonData.Action = CswEnumNbtButtonAction.managelocations;

                    JObject ActionOptioinsJObj = new JObject();
                    ActionOptioinsJObj["ivgnodeid"] = NodeId.ToString();
                    ButtonData.Data["ActionOptions"] = ActionOptioinsJObj;
                }//if clicked button is manage locations
            }
            return true;
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropText Name { get { return _CswNbtNode.Properties[PropertyName.Name]; } }
        public CswNbtNodePropLogical Central { get { return _CswNbtNode.Properties[PropertyName.Central]; } }
        public CswNbtNodePropLogical AutomaticCertificateApproval { get { return _CswNbtNode.Properties[PropertyName.AutomaticCertificateApproval]; } }
        public CswNbtNodePropButton AssignLocation { get { return ( _CswNbtNode.Properties[PropertyName.ManageLocations] ); } }
        public CswNbtNodePropMemo Description { get { return ( _CswNbtNode.Properties[PropertyName.Description] ); } }
        public CswNbtNodePropGrid Locations { get { return ( _CswNbtNode.Properties[PropertyName.Locations] ); } }
        public CswNbtNodePropGrid Permissions { get { return ( _CswNbtNode.Properties[PropertyName.Permissions] ); } }
        public CswNbtNodePropLogical LimitRequestDeliveryLocation { get { return ( _CswNbtNode.Properties[PropertyName.LimitRequestDeliveryLocation] ); } }

        #endregion

    }//CswNbtObjClassInventoryGroup

}//namespace ChemSW.Nbt.ObjClasses
