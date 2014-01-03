using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.PropertySets;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassMailReportGroup : CswNbtObjClass, ICswNbtPermissionGroup
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            /// <summary>
            /// The name of the Mail Report Group
            /// </summary>
            public const string Name = "Name";
            /// <summary>
            /// Description
            /// </summary>
            public const string Description = "Description";
            /// <summary>
            /// A grid of Reports that are assigned to this Group
            /// </summary>
            public const string MailReports = "Mail Reports";
            /// <summary>
            /// A grid of Permissions that are assigned to this Group
            /// </summary>
            public const string Permissions = "Permissions";
        }

        public CswEnumNbtObjectClass PermissionClass { get { return CswEnumNbtObjectClass.MailReportGroupPermissionClass; } }
        public CswEnumNbtObjectClass TargetClass { get { return CswEnumNbtObjectClass.MailReportClass; } }

        #region Base

        public CswNbtObjClassMailReportGroup( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.MailReportGroupClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassMailReportGroup
        /// </summary>
        public static implicit operator CswNbtObjClassMailReportGroup( CswNbtNode Node )
        {
            CswNbtObjClassMailReportGroup ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.MailReportGroupClass ) )
            {
                ret = (CswNbtObjClassMailReportGroup) Node.ObjClass;
            }
            return ret;
        }

        #endregion Base

        #region Inherited Events
        
        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( IsCopy, OverrideUniqueValidation );
        }

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        }

        public override void afterPromoteNode()
        {
            CswNbtPropertySetPermission.createDefaultWildcardPermission( _CswNbtResources, PermissionClass, NodeId );
        }

        #endregion Inherited Events

        #region Object class specific properties

        public CswNbtNodePropText Name { get { return _CswNbtNode.Properties[PropertyName.Name]; } }
        public CswNbtNodePropMemo Description { get { return _CswNbtNode.Properties[PropertyName.Description]; } }
        public CswNbtNodePropGrid Targets { get { return _CswNbtNode.Properties[PropertyName.MailReports]; } }
        public CswNbtNodePropGrid Permissions { get { return _CswNbtNode.Properties[PropertyName.Permissions]; } }

        #endregion Object class specific properties

    }//CswNbtObjClassMailReportGroup

}//namespace ChemSW.Nbt.ObjClasses