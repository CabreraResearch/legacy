using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassInventoryLevel : CswNbtObjClass
    {

        public sealed class PropertyName
        {
            public const string Material = "Material";
            public const string Type = "Type";
            public const string Level = "Level";
            public const string Subscribe = "Subscribe";
            public const string Location = "Location";
            public const string LastNotified = "Last Notified";
            public const string Status = "Status";
        }

        public sealed class Statuses
        {
            public const string Above = "Above Inventory Level";
            public const string Below = "Below Inventory Level";
            public const string Ok = "Ok";
            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString { Above, Below, Ok };
        }

        public sealed class Types
        {
            public const string Minimum = "Minimum";
            public const string Maximum = "Maximum";
            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString { Minimum, Maximum };
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public static implicit operator CswNbtObjClassInventoryLevel( CswNbtNode Node )
        {
            CswNbtObjClassInventoryLevel ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass ) )
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
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass ); }
        }

        #region Inherited Events
        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
        } // beforeCreateNode()


        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()


        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
            if( LastNotified.DateTimeValue == DateTime.MinValue )
            {
                Status.StaticText = Statuses.Ok;
            }
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
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            //CswNbtMetaDataObjectClassProp StatusOcp = ObjectClass.getObjectClassProp( PropertyName.Status.ToString() );
            //ParentRelationship.View.AddViewPropertyAndFilter( ParentRelationship, StatusOcp, Statuses.Pending.ToString() );

            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp, out NbtButtonAction ButtonAction, out string ActionData, out string Message )
        {
            Message = string.Empty;
            ActionData = string.Empty;
            ButtonAction = NbtButtonAction.Unknown;
            if( null != NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropStatic Type { get { return _CswNbtNode.Properties[PropertyName.Type]; } }
        public CswNbtNodePropQuantity Level { get { return _CswNbtNode.Properties[PropertyName.Level]; } }
        public CswNbtNodePropRelationship Material { get { return _CswNbtNode.Properties[PropertyName.Material]; } }
        public CswNbtNodePropLocation Location { get { return _CswNbtNode.Properties[PropertyName.Location]; } }
        public CswNbtNodePropDateTime LastNotified { get { return _CswNbtNode.Properties[PropertyName.LastNotified]; } }
        public CswNbtNodePropUserSelect Subscribe { get { return _CswNbtNode.Properties[PropertyName.Subscribe]; } }
        public CswNbtNodePropStatic Status { get { return _CswNbtNode.Properties[PropertyName.Status]; } }
        #endregion
    }//CswNbtObjClassInventoryLevel

}//namespace ChemSW.Nbt.ObjClasses
