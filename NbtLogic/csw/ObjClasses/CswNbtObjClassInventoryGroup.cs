using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassInventoryGroup : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Name = "Name";
            public const string Central = "Central";
            public const string AutomaticCertificateApproval = "Automatic Certificate Approval";
            public const string AssignLocation = "Assign Location";
        }


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
            AutomaticCertificateApproval.setHidden( Central.Checked != Tristate.True, false );
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

                if( PropertyName.AssignLocation == ButtonData.NodeTypeProp.getObjectClassPropName() )
                {
                    ButtonData.Action = NbtButtonAction.assignivglocation;


                    JObject Ret = new JObject();

                    JObject ActionOptioinsJObj = new JObject();
                    ActionOptioinsJObj["ivgnodeid"] = NodeId.ToString();

                    ButtonData.Data["ActionOptions"] = ActionOptioinsJObj;
                    //ButtonData.Data["ivgnodeid"] = NodeId.ToString();
                    //ButtonData.Data["viewmode"] = containerFamilyView.ViewMode.ToString();
                    //ButtonData.Data["type"] = "view";

                }//if clicked button is assign location


            }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropText Name { get { return _CswNbtNode.Properties[PropertyName.Name]; } }
        public CswNbtNodePropLogical Central { get { return _CswNbtNode.Properties[PropertyName.Central]; } }
        public CswNbtNodePropLogical AutomaticCertificateApproval { get { return _CswNbtNode.Properties[PropertyName.AutomaticCertificateApproval]; } }
        public CswNbtNodePropButton AssignLocation { get { return ( _CswNbtNode.Properties[PropertyName.AssignLocation] ); } }


        #endregion

    }//CswNbtObjClassInventoryGroup

}//namespace ChemSW.Nbt.ObjClasses
