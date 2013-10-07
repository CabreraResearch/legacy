using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassInventoryGroupPermission : CswNbtPropertySetPermission
    {
        #region Properties

        public new sealed class PropertyName : CswNbtPropertySetPermission.PropertyName
        {
            /// <summary>
            /// Permission to dispense a Target Container
            /// </summary>
            public const string Dispense = "Dispense";
            /// <summary>
            /// Permission to dispose a Target Container
            /// </summary>
            public const string Dispose = "Dispose";
            /// <summary>
            /// Permission to undispose a Target Container
            /// </summary>
            public const string Undispose = "Undispose";
            /// <summary>
            /// Permission to request a Move or Dispense of a Target Container
            /// </summary>
            public const string Request = "Request";
        }

        #endregion Properties

        #region Base

        public CswNbtObjClassInventoryGroupPermission( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryGroupPermissionClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassInventoryGroupPermission
        /// </summary>
        public static implicit operator CswNbtObjClassInventoryGroupPermission( CswNbtNode Node )
        {
            CswNbtObjClassInventoryGroupPermission ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.InventoryGroupPermissionClass ) )
            {
                ret = (CswNbtObjClassInventoryGroupPermission) Node.ObjClass;
            }
            return ret;
        }

        /// <summary>
        /// Cast a Permission PropertySet back to an Object Class
        /// </summary>
        public static CswNbtObjClassInventoryGroupPermission fromPropertySet( CswNbtPropertySetPermission PropertySet )
        {
            return PropertySet.Node;
        }

        /// <summary>
        /// Cast the Object Class as a PropertySet
        /// </summary>
        public static CswNbtPropertySetPermission toPropertySet( CswNbtObjClassInventoryGroupPermission ObjClass )
        {
            return ObjClass;
        }

        #endregion Base

        #region Inherited Events

        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
        }
        public override void afterCreateNode()
        {
        }

        public override void beforePropertySetWriteNode( bool IsCopy, bool OverrideUniqueValidation ) { }

        public override void afterPropertySetWriteNode() { }

        public override void beforePropertySetDeleteNode( bool DeleteAllRequiredRelatedNodes = false ) { }

        public override void afterPropertySetDeleteNode() { }

        public override void afterPropertySetPopulateProps() { }

        public override void onPropertySetAddDefaultViewFilters( CswNbtViewRelationship ParentRelationship ) { }

        public override bool onPropertySetButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }

        public override bool canAction( CswNbtAction Action )
        {
            bool hasPermission = false;
            if( null != Action )
            {
                if( ( Action.Name == CswEnumNbtActionName.DispenseContainer && Dispense.Checked == CswEnumTristate.True ) ||
                    ( Action.Name == CswEnumNbtActionName.DisposeContainer && Dispose.Checked == CswEnumTristate.True ) ||
                    ( Action.Name == CswEnumNbtActionName.UndisposeContainer && Undispose.Checked == CswEnumTristate.True ) ||
                    ( Action.Name == CswEnumNbtActionName.Submit_Request && Request.Checked == CswEnumTristate.True ) )
                {
                    hasPermission = true;
                }
                else if( Action.Name == CswEnumNbtActionName.Receiving )
                {
                    CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
                    foreach( CswNbtMetaDataNodeType ContainerNt in ContainerOC.getLatestVersionNodeTypes() )
                    {
                        hasPermission = _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Create, ContainerNt );
                        if( hasPermission )
                        {
                            break;
                        }
                    }
                }
            }
            return hasPermission;
        }

        public override void setWildCardValues()
        {
            Dispense.Checked = CswEnumTristate.True;
            Dispose.Checked = CswEnumTristate.True;
            Request.Checked = CswEnumTristate.True;
            Undispose.Checked = CswEnumTristate.True;
            WorkUnit.RelatedNodeId = null;
            WorkUnit.RefreshNodeName();
            WorkUnit.SyncGestalt();
        }

        #endregion Inherited Events
        
        #region Object class specific properties

        public CswNbtNodePropLogical Dispense { get { return _CswNbtNode.Properties[PropertyName.Dispense]; } }
        public CswNbtNodePropLogical Dispose { get { return _CswNbtNode.Properties[PropertyName.Dispose]; } }
        public CswNbtNodePropLogical Undispose { get { return _CswNbtNode.Properties[PropertyName.Undispose]; } }
        public CswNbtNodePropLogical Request { get { return _CswNbtNode.Properties[PropertyName.Request]; } }

        #endregion Object class specific properties

    }//CswNbtObjClassInventoryGroupPermission

}//namespace ChemSW.Nbt.ObjClasses
