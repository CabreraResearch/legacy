using System;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
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
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void beforePropertySetWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            //case 2753 - naming logic
            if( false == IsTemp )
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
            _throwIfMaterialExists();
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
                        switch( ButtonData.SelectedText )
                        {
                            case FulfillMenu.Create:
                                CswNbtObjClassMaterial ExistingMaterial = _getExistingMaterial();
                                if( null != ExistingMaterial )
                                {
                                    ButtonData.Action = NbtButtonAction.nothing;
                                    ButtonData.Message = "The requested Material has already been created: " + ExistingMaterial.Node.NodeLink;
                                }
                                else
                                {
                                    ButtonData.Action = NbtButtonAction.nothing;
                                    ButtonData.Data["nodeid"] = NodeId.ToString();
                                    ButtonData.Data["title"] = "";
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

        private CswNbtObjClassMaterial _getExistingMaterial()
        {
            CswNbtObjClassMaterial Ret = null;
            Int32 SelectedNodeTypeId = NewMaterialType.SelectedNodeTypeIds.ToIntCollection().FirstOrDefault();
            CswPrimaryKey Supplier = NewMaterialSupplier.RelatedNodeId;
            string Tradename = NewMaterialTradename.Text;
            string PartNo = NewMaterialPartNo.Text;
            if( SelectedNodeTypeId > 0 //Default for Int32 is 0
                && CswTools.IsPrimaryKey( Supplier ) && false == string.IsNullOrEmpty( Tradename ) )
            {
                Ret = CswNbtObjClassMaterial.getExistingMaterial( _CswNbtResources, SelectedNodeTypeId, Supplier, Tradename, PartNo );
            }
            return Ret;
        }

        private void _throwIfMaterialExists()
        {
            CswNbtObjClassMaterial ExistingMaterial = _getExistingMaterial();
            if( null != ExistingMaterial )
            {
                throw new CswDniException( ErrorType.Warning, "The requested Material already exists: " + ExistingMaterial.Node.NodeLink, "The requested Material already exists: " + ExistingMaterial.Node.NodeLink );
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

        public CswNbtNodePropNodeTypeSelect NewMaterialType { get { return _CswNbtNode.Properties[PropertyName.NewMaterialType]; } }
        public CswNbtNodePropText NewMaterialTradename { get { return _CswNbtNode.Properties[PropertyName.NewMaterialTradename]; } }
        public CswNbtNodePropText NewMaterialPartNo { get { return _CswNbtNode.Properties[PropertyName.NewMaterialPartNo]; } }
        public CswNbtNodePropRelationship NewMaterialSupplier { get { return _CswNbtNode.Properties[PropertyName.NewMaterialSupplier]; } }

        #endregion
    }//CswNbtObjClassRequestMaterialCreate

}//namespace ChemSW.Nbt.ObjClasses
