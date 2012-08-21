using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.UnitsOfMeasure;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassInventoryLevel : CswNbtObjClass
    {
        /// <summary>
        /// Property Nammes
        /// </summary>
        public sealed class PropertyName
        {
            public const string Material = "Material";
            public const string Type = "Type";
            public const string Level = "Level";
            public const string Subscribe = "Subscribe";
            public const string Location = "Location";
            public const string LastNotified = "Last Notified";
            public const string Status = "Status";
            public const string CurrentQuantity = "Current Quantity";
            public const string CurrentQuantityLog = "Current Quantity Log";
        }

        /// <summary>
        /// Possible statuses
        /// </summary>
        public sealed class Statuses
        {
            public const string Above = "Above Inventory Level";
            public const string Below = "Below Inventory Level";
            public const string Ok = "Ok";
            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString { Above, Below, Ok };
        }

        /// <summary>
        /// Possible Types
        /// </summary>
        public sealed class Types
        {
            public const string Minimum = "Minimum";
            public const string Maximum = "Maximum";
            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString { Minimum, Maximum };
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;
        private CswNbtSdInventoryLevelMgr _LevelMgr = null;

        public static implicit operator CswNbtObjClassInventoryLevel( CswNbtNode Node )
        {
            CswNbtObjClassInventoryLevel ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.InventoryLevelClass ) )
            {
                ret = (CswNbtObjClassInventoryLevel) Node.ObjClass;
            }
            return ret;
        }

        public CswNbtObjClassInventoryLevel copyNode()
        {
            CswNbtNode CopyNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
            CopyNode.copyPropertyValues( Node );
            CswNbtObjClassInventoryLevel RetCopy = CopyNode;
            RetCopy.postChanges( true );
            return RetCopy;
        }

        public CswNbtObjClassInventoryLevel( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
            _LevelMgr = new CswNbtSdInventoryLevelMgr( _CswNbtResources );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InventoryLevelClass ); }
        }


        #region Inherited Events
        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {

        } // beforeCreateNode()


        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()


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

        public override void afterPopulateProps()
        {
            Level.SetOnPropChange( OnLevelPropChange );
            Material.SetOnPropChange( OnMaterialPropChange );
            CurrentQuantity.SetOnPropChange( OnCurrrentQuantityPropChange );
            Location.SetOnPropChange( OnLocationPropChange );
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            //CswNbtMetaDataObjectClassProp StatusOcp = ObjectClass.getObjectClassProp( PropertyName.Status.ToString() );
            //ParentRelationship.View.AddViewPropertyAndFilter( ParentRelationship, StatusOcp, Statuses.Pending.ToString() );

            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropList Type { get { return _CswNbtNode.Properties[PropertyName.Type]; } }
        public CswNbtNodePropQuantity Level { get { return _CswNbtNode.Properties[PropertyName.Level]; } }
        private void OnLevelPropChange( CswNbtNodeProp Prop )
        {
            if( Level.UnitId != CurrentQuantity.UnitId )
            {
                CurrentQuantity.UnitId = Level.UnitId;
                CurrentQuantity.Quantity = _LevelMgr.getCurrentInventoryLevel( this );
                CurrentQuantityLog.AddComment( "Set initial Inventory Level Quantity: " + CurrentQuantity.Gestalt );
            }
        }
        public CswNbtNodePropRelationship Material { get { return _CswNbtNode.Properties[PropertyName.Material]; } }
        private void OnMaterialPropChange( CswNbtNodeProp Prop )
        {
            if( CswTools.IsPrimaryKey( Material.RelatedNodeId ) )
            {
                CswNbtNode MaterialNode = _CswNbtResources.Nodes[Material.RelatedNodeId];
                if( null != MaterialNode )
                {
                    CswNbtUnitViewBuilder Vb = new CswNbtUnitViewBuilder( _CswNbtResources );
                    Vb.setQuantityUnitOfMeasureView( MaterialNode, CurrentQuantity );
                    Vb.setQuantityUnitOfMeasureView( MaterialNode, Level );
                }
            }
        }

        public CswNbtNodePropLocation Location { get { return _CswNbtNode.Properties[PropertyName.Location]; } }
        private void OnLocationPropChange( CswNbtNodeProp Prop )
        {
            CurrentQuantity.Quantity = _LevelMgr.getCurrentInventoryLevel( this );
        }

        public CswNbtNodePropDateTime LastNotified { get { return _CswNbtNode.Properties[PropertyName.LastNotified]; } }
        public CswNbtNodePropUserSelect Subscribe { get { return _CswNbtNode.Properties[PropertyName.Subscribe]; } }
        public CswNbtNodePropList Status { get { return _CswNbtNode.Properties[PropertyName.Status]; } }
        public CswNbtNodePropQuantity CurrentQuantity { get { return _CswNbtNode.Properties[PropertyName.CurrentQuantity]; } }
        private void OnCurrrentQuantityPropChange( CswNbtNodeProp Prop )
        {
            if( _LevelMgr.doSendEmail( this ) )
            {
                LastNotified.DateTimeValue = _LevelMgr.sendPastThreshholdEmail( this );
            }
            if( _LevelMgr.isLevelPastThreshhold( this ) )
            {
                if( CurrentQuantity.Quantity > Level.Quantity )
                {
                    Status.Value = Statuses.Above;
                }
                else
                {
                    Status.Value = Statuses.Below;
                }
            }
            else
            {
                Status.Value = Statuses.Ok;
            }
        }

        public CswNbtNodePropComments CurrentQuantityLog { get { return _CswNbtNode.Properties[PropertyName.CurrentQuantityLog]; } }

        #endregion
    }//CswNbtObjClassInventoryLevel

}//namespace ChemSW.Nbt.ObjClasses
