using System;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
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
            /// Relationship(<see cref="CswNbtNodePropRelationship"/> ) to the Material (<see cref="CswNbtObjClassMaterial"/>) from which the Request Item will be Fulfilled.
            /// </summary>
            public const string Material = "Material";

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
                    Create, Complete, Cancel
                };
        }

        #endregion Enums

        #region Base

        public static implicit operator CswNbtObjClassRequestMaterialCreate( CswNbtNode Node )
        {
            CswNbtObjClassRequestMaterialCreate ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.RequestMaterialCreateClass ) )
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
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RequestMaterialCreateClass ); }
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
            }
        }

        public override string setRequestDescription()
        {
            string Ret = "Create new " + NewMaterialType.SelectedNodeTypeNames() + " name " + NewMaterialTradename.Text + " from supplier " + NewMaterialSupplier.Gestalt;
            if( false == string.IsNullOrEmpty( NewMaterialPartNo.Text ) )
            {
                Ret += " on partno " + NewMaterialPartNo.Text;
            }
            return Ret;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void beforePropertySetWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _throwIfMaterialExists();
            if( false == Location.Hidden )
            {
                Location.setHidden( value: true, SaveToDb: true );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void afterPropertySetWriteNode()
        {

        }


        /// <summary>
        /// 
        /// </summary>
        public override void afterPropertySetPopulateProps()
        {
            Material.SetOnPropChange( onMaterialPropChange );
        }//afterPopulateProps()

        /// <summary>
        /// 
        /// </summary>
        public override bool onPropertySetButtonClick( CswNbtMetaDataObjectClassProp OCP, NbtButtonData ButtonData )
        {
            if( null != ButtonData.NodeTypeProp && null != OCP )
            {
                switch( OCP.PropName )
                {
                    case PropertyName.Fulfill:
                        //If we do this here, we'll override the Actions for other options managed by the Property Set
                        //ButtonData.Action = NbtButtonAction.nothing;
                        switch( ButtonData.SelectedText )
                        {
                            case FulfillMenu.Create:
                                ButtonData.Action = NbtButtonAction.nothing;
                                if( PotentialMaterial().existsInDb( ForceRecalc: true ) )
                                {
                                    ButtonData.Message = "The requested Material has already been created: " + PotentialMaterial().existingMaterial().Node.NodeLink;
                                }
                                else
                                {
                                    CswNbtObjClassMaterial NewMaterial = PotentialMaterial().commit( UpversionTemp: true );
                                    bool Success = null != NewMaterial;
                                    if( Success )
                                    {
                                        ButtonData.Action = NbtButtonAction.landingpage;
                                        Material.RelatedNodeId = NewMaterial.NodeId;
                                        Fulfill.MenuOptions = FulfillMenu.Complete + "," + FulfillMenu.Cancel;
                                        Fulfill.State = FulfillMenu.Complete;
                                        ButtonData.Data["landingpage"] = CswNbtActCreateMaterial.getLandingPageData( _CswNbtResources, NewMaterial.Node );
                                    }
                                    else
                                    {
                                        ButtonData.Message = "The requested Material could not be created.";
                                    }
                                    ButtonData.Data["success"] = Success;
                                }
                                break;

                        } //switch( ButtonData.SelectedText )

                        _getNextStatus( ButtonData.SelectedText );
                        ButtonData.Data["requestitem"] = ButtonData.Data["requestitem"] ?? new JObject();
                        ButtonData.Data["requestitem"]["requestitemid"] = NodeId.ToString();
                        ButtonData.Data["requestitem"]["materialid"] = ( Material.RelatedNodeId ?? new CswPrimaryKey() ).ToString();
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
                    setNextStatus( Statuses.Created );
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
            _PotentialMaterial = _PotentialMaterial ?? new CswNbtActCreateMaterial.NewMaterial( _CswNbtResources, SelectedNodeTypeId, NewMaterialTradename.Text, NewMaterialSupplier.RelatedNodeId, NewMaterialPartNo.Text );
            _PotentialMaterial.TradeName = NewMaterialTradename.Text;
            _PotentialMaterial.SupplierId = NewMaterialSupplier.RelatedNodeId;
            _PotentialMaterial.PartNo = NewMaterialPartNo.Text;
            return _PotentialMaterial;
        }

        private void _throwIfMaterialExists()
        {
            if( false == CswTools.IsPrimaryKey( Material.RelatedNodeId ) &&
                NewMaterialType.SelectedNodeTypeIds.Count == 1 &&
                false == string.IsNullOrEmpty( NewMaterialTradename.Text ) &&
                CswTools.IsPrimaryKey( NewMaterialSupplier.RelatedNodeId ) )
            {
                CswNbtObjClassMaterial ExistingMaterial = PotentialMaterial().existingMaterial( ForceRecalc: true );
                if( null != ExistingMaterial )
                {
                    throw new CswDniException( ErrorType.Warning, "The requested Material already exists: " + ExistingMaterial.Node.NodeLink, "The requested Material already exists: " + ExistingMaterial.Node.NodeLink );
                }
            }
        }

        #endregion

        #region CswNbtPropertySetRequestItem Members

        /// <summary>
        /// 
        /// </summary>
        public override void onStatusPropChange( CswNbtNodeProp Prop )
        {
            Type.setHidden( value: ( Status.Value == Statuses.Pending ), SaveToDb: true );

            switch( Status.Value )
            {
                case Statuses.Created:
                    Fulfill.State = FulfillMenu.Complete;
                    break;

            }
        }

        public override void onTypePropChange( CswNbtNodeProp Prop )
        {
            Type.setReadOnly( value: true, SaveToDb: true );

            Fulfill.MenuOptions = FulfillMenu.Options.ToString();
            Fulfill.State = FulfillMenu.Create;
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Material { get { return _CswNbtNode.Properties[PropertyName.Material]; } }
        private void onMaterialPropChange( CswNbtNodeProp NodeProp )
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

        #endregion
    }//CswNbtObjClassRequestMaterialCreate

}//namespace ChemSW.Nbt.ObjClasses
