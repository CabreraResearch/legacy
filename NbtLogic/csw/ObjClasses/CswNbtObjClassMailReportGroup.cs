using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassMailReportGroup : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            /// <summary>
            /// The name of the Mail Report Group
            /// </summary>
            public const string Name = "Name";
            /// <summary>
            /// A grid of Reports that are assigned to this Group
            /// </summary>
            public const string Reports = "Reports";
            /// <summary>
            /// A grid of Permissions that are assigned to this Group
            /// </summary>
            public const string Permissions = "Permissions";
        }

        #region Base

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassMailReportGroup( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

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

        protected override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.triggerAfterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something */ }
            return true;
        }

        #endregion Inherited Events

        #region Object class specific properties

        public CswNbtNodePropText Name { get { return _CswNbtNode.Properties[PropertyName.Name]; } }
        public CswNbtNodePropGrid Targets { get { return _CswNbtNode.Properties[PropertyName.Reports]; } }
        public CswNbtNodePropGrid Permissions { get { return _CswNbtNode.Properties[PropertyName.Permissions]; } }

        #endregion Object class specific properties

    }//CswNbtObjClassMailReportGroup

}//namespace ChemSW.Nbt.ObjClasses
