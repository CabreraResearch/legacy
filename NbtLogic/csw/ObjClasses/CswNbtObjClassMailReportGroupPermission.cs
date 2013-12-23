using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassMailReportGroupPermission : CswNbtPropertySetPermission
    {
        #region Properties

        public new sealed class PropertyName : CswNbtPropertySetPermission.PropertyName
        {
            //Add ObjectClass-specific properties here
        }

        #endregion Properties

        #region Base

        public CswNbtObjClassMailReportGroupPermission( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.MailReportGroupPermissionClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassMailReportGroupPermission
        /// </summary>
        public static implicit operator CswNbtObjClassMailReportGroupPermission( CswNbtNode Node )
        {
            CswNbtObjClassMailReportGroupPermission ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.MailReportGroupPermissionClass ) )
            {
                ret = (CswNbtObjClassMailReportGroupPermission) Node.ObjClass;
            }
            return ret;
        }

        /// <summary>
        /// Cast a Permission PropertySet back to an Object Class
        /// </summary>
        public static CswNbtObjClassMailReportGroupPermission fromPropertySet( CswNbtPropertySetPermission PropertySet )
        {
            return PropertySet.Node;
        }

        /// <summary>
        /// Cast the Object Class as a PropertySet
        /// </summary>
        public static CswNbtPropertySetPermission toPropertySet( CswNbtObjClassMailReportGroupPermission ObjClass )
        {
            return ObjClass;
        }

        #endregion Base

        #region Inherited Events

        public override void beforePromoteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
        }//beforeCreateNode()

        public override void afterPromoteNode()
        {
        }//afterCreateNode()

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

        public override void setWildCardValues() { }

        #endregion Inherited Events

        #region Public Static Functions

        

        #endregion Public Static Functions

        #region Object class specific properties

        //Add ObjectClass-specific properties here

        #endregion Object class specific properties

    }//CswNbtObjClassMailReportGroupPermission

}//namespace ChemSW.Nbt.ObjClasses
