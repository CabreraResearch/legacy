using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassReportGroupPermission : CswNbtPropertySetPermission
    {
        #region Properties

        public new sealed class PropertyName : CswNbtPropertySetPermission.PropertyName
        {
            /// <summary>
            /// The Group with which to apply permissions
            /// </summary>
            public const string ReportGroup = "Report Group";
        }

        /// <summary>
        /// Returns the Group ObjectClass that relates to the Target
        /// </summary>
        public override CswEnumNbtObjectClass GroupClass
        {
            get { return CswEnumNbtObjectClass.ReportGroupClass; }
        }

        /// <summary>
        /// Returns the Group ObjectClass that relates to the Target
        /// </summary>
        public override CswEnumNbtObjectClass TargetClass
        {
            get { return CswEnumNbtObjectClass.ReportClass; }
        }

        #endregion Properties

        #region Base

        public CswNbtObjClassReportGroupPermission( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportGroupPermissionClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassReportGroupPermission
        /// </summary>
        public static implicit operator CswNbtObjClassReportGroupPermission( CswNbtNode Node )
        {
            CswNbtObjClassReportGroupPermission ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.ReportGroupPermissionClass ) )
            {
                ret = (CswNbtObjClassReportGroupPermission) Node.ObjClass;
            }
            return ret;
        }

        /// <summary>
        /// Cast a Permission PropertySet back to an Object Class
        /// </summary>
        public static CswNbtObjClassReportGroupPermission fromPropertySet( CswNbtPropertySetPermission PropertySet )
        {
            return PropertySet.Node;
        }

        /// <summary>
        /// Cast the Object Class as a PropertySet
        /// </summary>
        public static CswNbtPropertySetPermission toPropertySet( CswNbtObjClassReportGroupPermission ObjClass )
        {
            return ObjClass;
        }

        #endregion Base

        #region Inherited Events

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

        public override bool canAction( CswNbtAction Action ) { return true; }

        #endregion Inherited Events

        #region Public Static Functions

        

        #endregion Public Static Functions

        #region Object class specific properties

        public override CswNbtNodePropRelationship Group { get { return _CswNbtNode.Properties[PropertyName.ReportGroup]; } }

        #endregion Object class specific properties

    }//CswNbtObjClassReportGroupPermission

}//namespace ChemSW.Nbt.ObjClasses
