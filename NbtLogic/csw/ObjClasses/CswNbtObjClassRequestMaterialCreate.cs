using System;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.UnitsOfMeasure;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Material Dispense Request Item
    /// </summary>
    public class CswNbtObjClassRequestMaterialCreate : CswNbtPropertySetRequestItem
    {
        #region Enums

        /// <summary>
        /// Property Names
        /// </summary>
        public new sealed class PropertyName : CswNbtPropertySetRequestItem.PropertyName
        {
            /// <summary>
            /// The type of the new material 
            /// </summary>
            public const string NewMaterialType = "New Material Type";

            /// <summary>
            /// The name of the new material 
            /// </summary>
            public const string NewMaterialTradename = "New Material Tradename";

            /// <summary>
            /// The supplier of the new material
            /// </summary>
            public const string NewMaterialSupplier = "New Material Supplier";

            /// <summary>
            /// The Part No of the new material
            /// </summary>
            public const string NewMaterialPartNo = "New Material Part No";

            /// <summary>
            /// The Approval level of this Create Material request
            /// </summary>
            public const string ApprovalLevel = "Approval Level";
            /// <summary>
            /// The Quantity(<see cref="CswNbtNodePropQuantity"/>) to request. 
            /// </summary>
            public const string Quantity = "Quantity";
        }

        /// <summary>
        /// Types: Bulk or Size
        /// </summary>
        public new sealed class Types : CswNbtPropertySetRequestItem.Types
        {
            public const string Create = "Request Material Create";
            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString { Create };
        }

        /// <summary>
        /// Statuses
        /// </summary>
        public new sealed class Statuses : CswNbtPropertySetRequestItem.Statuses
        {
            public const string Created = "Created";

            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString
                {
                    Pending, Submitted, Created, Completed, Cancelled
                };
        }

        /// <summary>
        /// Fulfill menu options
        /// </summary>
        public new sealed class FulfillMenu : CswNbtPropertySetRequestItem.FulfillMenu
        {
            public const string Create = "Create Material";

            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString
                {
                    Create, Cancel
                };
        }

        #endregion Enums

        #region Base

        public static implicit operator CswNbtObjClassRequestMaterialCreate( CswNbtNode Node )
        {
            CswNbtObjClassRequestMaterialCreate ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.RequestMaterialCreateClass ) )
            {
                ret = (CswNbtObjClassRequestMaterialCreate) Node.ObjClass;
            }
            return ret;
        }

        /// <summary>
        /// Cast a Request Item PropertySet back to an Object Class
        /// </summary>
        public static CswNbtObjClassRequestMaterialCreate fromPropertySet( CswNbtPropertySetRequestItem PropertySet )
        {
            return PropertySet.Node;
        }

        /// <summary>
        /// Cast a the Object Class as a PropertySet
        /// </summary>
        public static CswNbtPropertySetRequestItem toPropertySet( CswNbtObjClassRequestMaterialCreate ObjClass )
        {
            return ObjClass;
        }

        public CswNbtObjClassRequestMaterialCreate( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {

        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestMaterialCreateClass ); }
        }

        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
        }

        public override void afterCreateNode()
        {
        }

        #endregion Base

        #region Inherited Events

        /// <summary>
        /// 
        /// </summary>
        public override void toggleReadOnlyProps( bool IsReadOnly, CswNbtPropertySetRequestItem ItemInstance )
        {
            if( null != ItemInstance )
            {
                CswNbtObjClassRequestMaterialCreate ThisRequest = (CswNbtObjClassRequestMaterialCreate) ItemInstance;

                ThisRequest.Material.setReadOnly( value: IsReadOnly, SaveToDb: true );
                ThisRequest.NewMaterialType.setReadOnly( value: IsReadOnly, SaveToDb: true );
                ThisRequest.NewMaterialSupplier.setReadOnly( value: IsReadOnly, SaveToDb: true );
                ThisRequest.NewMaterialTradename.setReadOnly( value: IsReadOnly, SaveToDb: true );
                ThisRequest.NewMaterialPartNo.setReadOnly( value: IsReadOnly, SaveToDb: true );
                ThisRequest.Quantity.setReadOnly( value: IsReadOnly, SaveToDb: true );
            }
        }

        public override string setRequestDescription()
        {
            string Ret = "Create new " + NewMaterialType.SelectedNodeTypeNames() + ": " + NewMaterialTradename.Text + " " + NewMaterialSupplier.Gestalt;
            if( false == string.IsNullOrEmpty( NewMaterialPartNo.Text ) )
            {
                Ret += " " + NewMaterialPartNo.Text;
            }
            return Ret;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void beforePropertySetWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _throwIfMaterialExists();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void afterPropertySetWriteNode()
        {

        }

        public override void beforePropertySetDeleteNode()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public override void afterPropertySetPopulateProps()
        {
            CswNbtUnitViewBuilder Vb = new CswNbtUnitViewBuilder( _CswNbtResources );
            Vb.getQuantityUnitOfMeasureView( CswNbtPropertySetMaterial.CswEnumPhysicalState.Liquid, Quantity );
            Material.SetOnPropChange( onMaterialPropChange );
        }//afterPopulateProps()

        /// <summary>
        /// 
        /// </summary>
        public override bool onPropertySetButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData.NodeTypeProp )
            {
                //Remember: Save is an OCP too
                switch( ButtonData.NodeTypeProp.getObjectClassPropName() )
                {
                    case PropertyName.Fulfill:
                        //If we do this here, we'll override the Actions for other options managed by the Property Set
                        //ButtonData.Action = NbtButtonAction.nothing;

                        //Case 30722: The default selected text is "Fulfill": "Create" is the drop-down text.
                        // See also Case 30748 for future exploration.
                        //switch( ButtonData.SelectedText )     { case FulfillMenu.Create:
                        ButtonData.Action = CswEnumNbtButtonAction.nothing;
                        if( PotentialMaterial().existsInDb( ForceRecalc: true ) )
                        {
                            ButtonData.Message = "The requested Material has already been created: " + PotentialMaterial().existingMaterial().Node.NodeLink;
                        }
                        else
                        {
                            CswNbtPropertySetMaterial NewMaterial = PotentialMaterial().commit(); //See Case 28310. We do not want to upversion this node. The Create Material Wizard will do that for us.
                            bool Success = null != NewMaterial;
                            if( Success )
                            {
                                ButtonData.Action = CswEnumNbtButtonAction.creatematerial;
                                Material.RelatedNodeId = NewMaterial.NodeId;
                                Fulfill.State = FulfillMenu.Create;

                                ButtonData.Data["state"] = new JObject();
                                ButtonData.Data["state"]["materialType"] = new JObject();
                                ButtonData.Data["state"]["materialType"]["name"] = PotentialMaterial().NodeTypeName;
                                ButtonData.Data["state"]["materialType"]["val"] = PotentialMaterial().NodeTypeId;
                                ButtonData.Data["state"]["materialId"] = NewMaterial.NodeId.ToString();
                                ButtonData.Data["state"]["tradeName"] = PotentialMaterial().TradeName;
                                ButtonData.Data["state"]["supplier"] = new JObject();
                                ButtonData.Data["state"]["supplier"]["val"] = PotentialMaterial().SupplierId.ToString();
                                ButtonData.Data["state"]["supplier"]["name"] = PotentialMaterial().SupplierName;
                                ButtonData.Data["state"]["partNo"] = NewMaterialPartNo.Text;
                                ButtonData.Data["state"]["request"] = new JObject();
                                ButtonData.Data["state"]["request"]["requestitemid"] = NodeId.ToString();
                                ButtonData.Data["state"]["request"]["materialid"] = ( Material.RelatedNodeId ?? new CswPrimaryKey() ).ToString();
                            }
                            else
                            {
                                ButtonData.Message = "The requested Material cannot not be created.";
                            }
                            ButtonData.Data["success"] = Success;
                        }

                        //break; } //switch( ButtonData.SelectedText )
                        _getNextStatus( ButtonData.SelectedText );
                        break; //case PropertyName.Fulfill:
                }
            }
            return true;
        }

        private void _getNextStatus( string ButtonText )
        {
            switch( ButtonText )
            {
                case FulfillMenu.Cancel:
                    setNextStatus( Statuses.Cancelled );
                    break;
                case FulfillMenu.Complete:
                    setNextStatus( Statuses.Completed );
                    break;
                case FulfillMenu.Create:
                    //setNextStatus( Statuses.Created );
                    break;
            }
        }

        public void setNextStatus( string StatusVal )
        {
            switch( Status.Value )
            {
                case Statuses.Submitted:
                    if( StatusVal == Statuses.Created || StatusVal == Statuses.Cancelled || StatusVal == Statuses.Completed )
                    {
                        Status.Value = StatusVal;
                    }
                    break;

            }
        }

        private CswNbtActCreateMaterial.NewMaterial _PotentialMaterial = null;
        private CswNbtActCreateMaterial.NewMaterial PotentialMaterial()
        {
            Int32 SelectedNodeTypeId = NewMaterialType.SelectedNodeTypeIds.ToIntCollection().FirstOrDefault();
            _PotentialMaterial = _PotentialMaterial ?? new CswNbtActCreateMaterial.NewMaterial( _CswNbtResources, SelectedNodeTypeId, NewMaterialTradename.Text, NewMaterialSupplier.RelatedNodeId, false, NewMaterialPartNo.Text );
            _PotentialMaterial.TradeName = NewMaterialTradename.Text;
            _PotentialMaterial.SupplierId = NewMaterialSupplier.RelatedNodeId;
            _PotentialMaterial.PartNo = NewMaterialPartNo.Text;
            return _PotentialMaterial;
        }

        private void _throwIfMaterialExists()
        {
            if( Status.Value != Statuses.Cancelled &&
                false == CswTools.IsPrimaryKey( Material.RelatedNodeId ) &&
                NewMaterialType.SelectedNodeTypeIds.Count == 1 &&
                false == string.IsNullOrEmpty( NewMaterialTradename.Text ) &&
                CswTools.IsPrimaryKey( NewMaterialSupplier.RelatedNodeId ) )
            {
                CswNbtPropertySetMaterial ExistingMaterial = PotentialMaterial().existingMaterial( ForceRecalc: true );
                if( null != ExistingMaterial )
                {
                    throw new CswDniException( CswEnumErrorType.Warning, "The requested Material already exists: " + ExistingMaterial.Node.NodeLink, "The requested Material already exists: " + ExistingMaterial.Node.NodeLink );
                }
            }
        }

        #endregion

        #region CswNbtPropertySetRequestItem Members

        /// <summary>
        /// 
        /// </summary>
        public override void onStatusPropChange( CswNbtNodeProp Prop, bool Creating )
        {
            Type.setHidden( value: ( Status.Value == Statuses.Pending ), SaveToDb: true );

            switch( Status.Value )
            {
                case Statuses.Created:
                    Fulfill.State = FulfillMenu.Complete;
                    break;

            }
        }

        public override void onTypePropChange( CswNbtNodeProp Prop, bool Creating )
        {
            Type.setReadOnly( value: true, SaveToDb: true );

            Fulfill.MenuOptions = FulfillMenu.Options.ToString();
            Fulfill.State = FulfillMenu.Create;
        }

        public override void onRequestPropChange( CswNbtNodeProp Prop, bool Creating )
        {

        }

        public override void onPropertySetAddDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            //Nothing to do yet
        }

        #endregion

        #region Object class specific properties

        private void onMaterialPropChange( CswNbtNodeProp NodeProp, bool Creating )
        {
            if( CswTools.IsPrimaryKey( Material.RelatedNodeId ) )
            {
                NewMaterialType.setHidden( value: true, SaveToDb: true );
                NewMaterialTradename.setHidden( value: true, SaveToDb: true );
                NewMaterialSupplier.setHidden( value: true, SaveToDb: true );
                NewMaterialPartNo.setHidden( value: true, SaveToDb: true );
            }
        }
        public CswNbtNodePropNodeTypeSelect NewMaterialType { get { return _CswNbtNode.Properties[PropertyName.NewMaterialType]; } }
        public CswNbtNodePropText NewMaterialTradename { get { return _CswNbtNode.Properties[PropertyName.NewMaterialTradename]; } }
        public CswNbtNodePropText NewMaterialPartNo { get { return _CswNbtNode.Properties[PropertyName.NewMaterialPartNo]; } }
        public CswNbtNodePropRelationship NewMaterialSupplier { get { return _CswNbtNode.Properties[PropertyName.NewMaterialSupplier]; } }
        public CswNbtNodePropList ApprovalLevel { get { return _CswNbtNode.Properties[PropertyName.ApprovalLevel]; } }
        public CswNbtNodePropQuantity Quantity
        {
            get { return _CswNbtNode.Properties[PropertyName.Quantity]; }
        }

        #endregion
    }//CswNbtObjClassRequestMaterialCreate

}//namespace ChemSW.Nbt.ObjClasses
