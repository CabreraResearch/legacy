using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassInventoryGroupPermission : CswNbtObjClass
    {
        public static string InventoryGroupPropertyName { get { return "Inventory Group"; } }
        public static string WorkUnitPropertyName { get { return "WorkUnit"; } }
        public static string RolePropertyName { get { return "Role"; } }
        public static string ViewPropertyName { get { return "View"; } }
        public static string EditPropertyName { get { return "Edit"; } }
        public static string DispensePropertyName { get { return "Dispense"; } }
        public static string DisposePropertyName { get { return "Dispose"; } }
        public static string UndisposePropertyName { get { return "Undispose"; } }
        public static string RequestPropertyName { get { return "Request"; } }

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

        public CswNbtNodePropRelationship InventoryGroup { get { return _CswNbtNode.Properties[InventoryGroupPropertyName].AsRelationship; } }
        public CswNbtNodePropRelationship WorkUnit { get { return _CswNbtNode.Properties[WorkUnitPropertyName].AsRelationship; } }
        public CswNbtNodePropRelationship Role { get { return _CswNbtNode.Properties[RolePropertyName].AsRelationship; } }
        public CswNbtNodePropLogical View { get { return _CswNbtNode.Properties[ViewPropertyName].AsLogical; } }
        public CswNbtNodePropLogical Edit { get { return _CswNbtNode.Properties[EditPropertyName].AsLogical; } }
        public CswNbtNodePropLogical Dispense { get { return _CswNbtNode.Properties[DispensePropertyName].AsLogical; } }
        public CswNbtNodePropLogical Dispose { get { return _CswNbtNode.Properties[DisposePropertyName].AsLogical; } }
        public CswNbtNodePropLogical Undispose { get { return _CswNbtNode.Properties[UndisposePropertyName].AsLogical; } }
        public CswNbtNodePropLogical Request { get { return _CswNbtNode.Properties[RequestPropertyName].AsLogical; } }

        #endregion

    }//CswNbtObjClassInventoryGroupPermission

}//namespace ChemSW.Nbt.ObjClasses
