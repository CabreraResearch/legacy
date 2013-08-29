using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.PropertySets;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassInventoryGroup : CswNbtObjClass, ICswNbtPermissionGroup
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
        }

        public CswEnumNbtObjectClass PermissionClass { get { return CswEnumNbtObjectClass.InventoryGroupPermissionClass; } }
        public CswEnumNbtObjectClass TargetClass { get { return CswEnumNbtObjectClass.ContainerClass; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassInventoryGroup( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

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


        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( IsCopy, OverrideUniqueValidation );
        }//beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        }//afterCreateNode()


        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            string OldName = Name.GetOriginalPropRowValue();
            if( string.IsNullOrEmpty( OldName ) && false == IsTemp )
            {
                CswNbtPropertySetPermission.createDefaultWildcardPermission( _CswNbtResources, PermissionClass, NodeId );
            }
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
            AutomaticCertificateApproval.setHidden( Central.Checked != CswEnumTristate.True, false );
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
                if( PropertyName.ManageLocations == ButtonData.NodeTypeProp.getObjectClassPropName() )
                {
                    ButtonData.Action = CswEnumNbtButtonAction.managelocations;

                    JObject ActionOptioinsJObj = new JObject();
                    ActionOptioinsJObj["ivgnodeid"] = NodeId.ToString();

                    ButtonData.Data["ActionOptions"] = ActionOptioinsJObj;
                    //ButtonData.Data["ivgnodeid"] = NodeId.ToString();
                    //ButtonData.Data["viewmode"] = containerFamilyView.ViewMode.ToString();
                    //ButtonData.Data["type"] = "view";

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

        #endregion

    }//CswNbtObjClassInventoryGroup

}//namespace ChemSW.Nbt.ObjClasses
