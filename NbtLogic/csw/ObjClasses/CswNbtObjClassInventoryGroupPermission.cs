using ChemSW.Nbt.MetaData;
using Newtonsoft.Json.Linq;
using ChemSW.Nbt.PropTypes;

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
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode(bool DeleteAllRequiredRelatedNodes = false)
        {
            _CswNbtObjClassDefault.beforeDeleteNode(DeleteAllRequiredRelatedNodes);

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

        public CswNbtNodePropRelationship InventoryGroup { get { return _CswNbtNode.Properties[InventoryGroupPropertyName].AsRelationship; } }
        public CswNbtNodePropRelationship WorkUnit { get { return _CswNbtNode.Properties[WorkUnitPropertyName].AsRelationship; } }
        public CswNbtNodePropRelationship Role { get { return _CswNbtNode.Properties[RolePropertyName].AsRelationship; } }
        public CswNbtNodePropLogical View { get { return _CswNbtNode.Properties[ViewPropertyName].AsLogical; } }
        public CswNbtNodePropLogical Edit { get { return _CswNbtNode.Properties[EditPropertyName].AsLogical; } }
        public CswNbtNodePropLogical Dispense { get { return _CswNbtNode.Properties[DispensePropertyName].AsLogical; } }
        public CswNbtNodePropLogical Request { get { return _CswNbtNode.Properties[RequestPropertyName].AsLogical; } }

        #endregion

    }//CswNbtObjClassInventoryGroupPermission

}//namespace ChemSW.Nbt.ObjClasses
