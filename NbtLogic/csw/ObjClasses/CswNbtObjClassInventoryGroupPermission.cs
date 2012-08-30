using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassInventoryGroupPermission : CswNbtObjClass
    {
        public sealed class PropertyName
        {
            public const string InventoryGroup = "Inventory Group";
            public const string WorkUnit = "WorkUnit";
            public const string Role = "Role";
            public const string View = "View";
            public const string Edit = "Edit";
            public const string Dispense = "Dispense";
            public const string Dispose = "Dispose";
            public const string Undispose = "Undispose";
            public const string Request = "Request";
        }


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassInventoryGroupPermission( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupPermissionClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassInventoryGroupPermission
        /// </summary>
        public static implicit operator CswNbtObjClassInventoryGroupPermission( CswNbtNode Node )
        {
            CswNbtObjClassInventoryGroupPermission ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupPermissionClass ) )
            {
                ret = (CswNbtObjClassInventoryGroupPermission) Node.ObjClass;
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

        public override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
        {



            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship InventoryGroup { get { return _CswNbtNode.Properties[PropertyName.InventoryGroup].AsRelationship; } }
        public CswNbtNodePropRelationship WorkUnit { get { return _CswNbtNode.Properties[PropertyName.WorkUnit].AsRelationship; } }
        public CswNbtNodePropRelationship Role { get { return _CswNbtNode.Properties[PropertyName.Role].AsRelationship; } }
        public CswNbtNodePropLogical View { get { return _CswNbtNode.Properties[PropertyName.View].AsLogical; } }
        public CswNbtNodePropLogical Edit { get { return _CswNbtNode.Properties[PropertyName.Edit].AsLogical; } }
        public CswNbtNodePropLogical Dispense { get { return _CswNbtNode.Properties[PropertyName.Dispense].AsLogical; } }
        public CswNbtNodePropLogical Dispose { get { return _CswNbtNode.Properties[PropertyName.Dispose].AsLogical; } }
        public CswNbtNodePropLogical Undispose { get { return _CswNbtNode.Properties[PropertyName.Undispose].AsLogical; } }
        public CswNbtNodePropLogical Request { get { return _CswNbtNode.Properties[PropertyName.Request].AsLogical; } }

        #endregion

    }//CswNbtObjClassInventoryGroupPermission

}//namespace ChemSW.Nbt.ObjClasses
