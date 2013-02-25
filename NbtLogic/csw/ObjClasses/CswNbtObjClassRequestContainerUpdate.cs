using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Container Update (Move or Dispose) Request Item
    /// </summary>
    public class CswNbtObjClassRequestContainerUpdate : CswNbtPropertySetRequestItem
    {
        #region Enums
        /// <summary>
        /// Property Names
        /// </summary>
        public new sealed class PropertyName : CswNbtPropertySetRequestItem.PropertyName
        {
            /// <summary>
            /// Relationship(<see cref="CswNbtNodePropRelationship"/> ) to the Container (<see cref="CswNbtObjClassContainer"/>) from which the Request Item will be Fulfilled.
            /// </summary>
            public const string Container = "Container";
        }

        public new sealed class Types : CswNbtPropertySetRequestItem.Types
        {
            public const string Dispose = "Request Container Dispose";
            public const string Move = "Request Container Move";
            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString { Dispose, Move };
        }

        public new sealed class Statuses : CswNbtPropertySetRequestItem.Statuses
        {
            public const string Disposed = "Disposed";
            public const string Moved = "Moved";

            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString
                {
                    Pending, Submitted, Completed, Cancelled
                };
        }

        public new sealed class FulfillMenu : CswNbtPropertySetRequestItem.FulfillMenu
        {
            public const string Move = "Move this Container";
            public const string Dispose = "Dispose this Container";

            public static readonly CswCommaDelimitedString DisposeOptions = new CswCommaDelimitedString
                {
                    Dispose, Cancel
                };
            public static readonly CswCommaDelimitedString MoveOptions = new CswCommaDelimitedString
                {
                    Move, Cancel
                };
        }

        #endregion Enums

        #region Base

        /// <summary>
        /// Implicit cast of Node to Object Class
        /// </summary>
        public static implicit operator CswNbtObjClassRequestContainerUpdate( CswNbtNode Node )
        {
            CswNbtObjClassRequestContainerUpdate ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.RequestContainerUpdateClass ) )
            {
                ret = (CswNbtObjClassRequestContainerUpdate) Node.ObjClass;
            }
            return ret;
        }

        /// <summary>
        /// Cast a Request Item PropertySet back to an Object Class
        /// </summary>
        public static CswNbtObjClassRequestContainerUpdate fromPropertySet( CswNbtPropertySetRequestItem PropertySet )
        {
            return PropertySet.Node;
        }

        /// <summary>
        /// Cast a the Object Class as a PropertySet
        /// </summary>
        public static CswNbtPropertySetRequestItem toPropertySet( CswNbtObjClassRequestContainerUpdate ObjClass )
        {
            return ObjClass;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public CswNbtObjClassRequestContainerUpdate( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {

        }//ctor()

        /// <summary>
        /// Object Class
        /// </summary>
        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.RequestContainerUpdateClass ); }
        }

        #endregion Base

        #region Inherited Events

        public override void toggleReadOnlyProps( bool IsReadOnly, CswNbtPropertySetRequestItem ItemInstance )
        {
            if( null != ItemInstance )
            {
                CswNbtObjClassRequestContainerUpdate ThisRequest = (CswNbtObjClassRequestContainerUpdate) ItemInstance;
                ThisRequest.Type.setReadOnly( value: IsReadOnly, SaveToDb: true );
                ThisRequest.Container.setReadOnly( value: IsReadOnly, SaveToDb: true );
            }

        }

        public override string setRequestDescription()
        {
            string Ret = "";

            switch( Type.Value )
            {
                case Types.Dispose:
                    Ret += "Dispose " + Container.Gestalt + " of " + Material.Gestalt;
                    break;

                case Types.Move:
                    Ret += "Move " + Container.Gestalt + " of " + Material.Gestalt + " to " + Location.Gestalt;
                    break;
            }

            return Ret;
        }

        public override void beforePropertySetWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            /* Container-specific logic */
            if( false == InventoryGroup.Hidden )
            {
                InventoryGroup.setHidden( value: true, SaveToDb: true );
            }
        }

        public override void afterPropertySetWriteNode()
        {

        }

        public override void afterPropertySetPopulateProps()
        {
            Container.SetOnPropChange( onContainerPropChange );
        }

        /// <summary>
        /// Abstract override to be called on onButtonClick
        /// </summary>
        public override bool onPropertySetButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData.NodeTypeProp )
            {
                switch( ButtonData.NodeTypeProp.getObjectClassPropName() )
                {
                    case PropertyName.Fulfill:
                        CswNbtObjClassContainer NodeAsContainer = null;
                        switch( ButtonData.SelectedText )
                        {
                            case FulfillMenu.Dispose:
                                NodeAsContainer = _CswNbtResources.Nodes.GetNode( Container.RelatedNodeId );
                                if( null != NodeAsContainer )
                                {
                                    NodeAsContainer.DisposeContainer();
                                    NodeAsContainer.postChanges( true );
                                    Status.Value = Statuses.Completed;
                                }
                                ButtonData.Action = NbtButtonAction.refresh;
                                break;
                            case FulfillMenu.Move:
                                if( null != Container.RelatedNodeId )
                                {
                                    NodeAsContainer = _CswNbtResources.Nodes.GetNode( Container.RelatedNodeId );
                                    if( null != NodeAsContainer )
                                    {
                                        NodeAsContainer.Location.SelectedNodeId = Location.SelectedNodeId;
                                        NodeAsContainer.Location.CachedNodeName = Location.CachedNodeName;
                                        NodeAsContainer.Location.CachedPath = Location.CachedPath;
                                        NodeAsContainer.postChanges( false );
                                        Status.Value = Statuses.Completed;
                                        ButtonData.Action = NbtButtonAction.refresh;
                                    }
                                }
                                break;
                        } //switch( ButtonData.SelectedText )

                        _getNextStatus( ButtonData.SelectedText );

                        ButtonData.Data["requestitem"] = ButtonData.Data["requestitem"] ?? new JObject();
                        ButtonData.Data["requestitem"]["requestitemid"] = NodeId.ToString();
                        ButtonData.Data["requestitem"]["containerid"] = ( Container.RelatedNodeId ?? new CswPrimaryKey() ).ToString();
                        ButtonData.Data["requestitem"]["locationid"] = ( Location.SelectedNodeId ?? new CswPrimaryKey() ).ToString();
                        break;
                }//case PropertyName.Fulfill:
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
                case FulfillMenu.Dispose:
                    setNextStatus( Statuses.Completed );
                    break;
                case FulfillMenu.Move:
                    setNextStatus( Statuses.Completed );
                    break;
            }
        }

        public void setNextStatus( string StatusVal )
        {
            switch( Status.Value )
            {
                case Statuses.Submitted:
                    if( StatusVal == Statuses.Disposed || StatusVal == Statuses.Moved || StatusVal == Statuses.Cancelled || StatusVal == Statuses.Completed )
                    {
                        Status.Value = StatusVal;
                    }
                    break;
            }
        }

        #endregion

        #region CswNbtPropertySetRequestItem Members

        /// <summary>
        /// Additional, Request-specific Status change event logic to be called
        /// </summary>
        public override void onStatusPropChange( CswNbtNodeProp Prop )
        {
            Type.setHidden( value: ( Status.Value == Statuses.Pending ), SaveToDb: true );

            switch( Status.Value )
            {
                case Statuses.Moved: //This fallthrough is intentional
                case Statuses.Disposed:
                    Fulfill.State = FulfillMenu.Complete;
                    break;
            }
        }

        #endregion

        #region Object class specific properties

        public override void onTypePropChange( CswNbtNodeProp Prop )
        {
            /* Spec W1010: Location applies to all but Dispose */
            Location.setHidden( value: ( Types.Dispose == Type.Value ), SaveToDb: true );
            Location.setReadOnly( value: ( Status.Value != Statuses.Pending || Types.Dispose == Type.Value ), SaveToDb: true );

            switch( Type.Value )
            {
                case Types.Dispose:
                    Fulfill.MenuOptions = FulfillMenu.DisposeOptions.ToString();
                    Fulfill.State = FulfillMenu.Dispose;
                    break;
                case Types.Move:
                    Fulfill.MenuOptions = FulfillMenu.MoveOptions.ToString();
                    Fulfill.State = FulfillMenu.Move;
                    break;
            }
        }

        public override void onRequestPropChange( CswNbtNodeProp Prop )
        {
        
        }

        public override void onPropertySetAddDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            //Nothing to do yet
        }

        public CswNbtNodePropRelationship Container
        {
            get { return _CswNbtNode.Properties[PropertyName.Container]; }
        }
        private void onContainerPropChange( CswNbtNodeProp Prop )
        {
            if( null != Container.RelatedNodeId )
            {
                Container.setReadOnly( value: true, SaveToDb: true );
                if( false == CswTools.IsPrimaryKey( Material.RelatedNodeId ) )
                {
                    CswNbtObjClassContainer ThisContainer = _CswNbtResources.Nodes[Container.RelatedNodeId];
                    if( null != ThisContainer )
                    {
                        Material.RelatedNodeId = ThisContainer.Material.RelatedNodeId;
                    }
                }
            }
        }

        #endregion
    }//CswNbtObjClassRequestContainerUpdate

}//namespace ChemSW.Nbt.ObjClasses
