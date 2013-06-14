using System;
using System.Text.RegularExpressions;
using ChemSW.Core;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassRegulatoryListMember : CswNbtObjClass
    {
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string RegulatoryList = "Regulatory List";
            public const string Chemical = "Chemical";
            //public const string CASNo = "CAS No";
            //public const string Exclusive = "Exclusive";
            //public const string Show = "Show";
            public const string ByUser = "ByUser";
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassRegulatoryListMember( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );

        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListMemberClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassRegulatoryListMember
        /// </summary>
        public static implicit operator CswNbtObjClassRegulatoryListMember( CswNbtNode Node )
        {
            CswNbtObjClassRegulatoryListMember ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.RegulatoryListMemberClass ) )
            {
                ret = (CswNbtObjClassRegulatoryListMember) Node.ObjClass;
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
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship RegulatoryList { get { return _CswNbtNode.Properties[PropertyName.RegulatoryList]; } }
        public CswNbtNodePropRelationship Chemical { get { return _CswNbtNode.Properties[PropertyName.Chemical]; } }
        //public CswNbtNodePropCASNo CASNo { get { return _CswNbtNode.Properties[PropertyName.CASNo]; } }
        //public CswNbtNodePropLogical Exclusive { get { return _CswNbtNode.Properties[PropertyName.Exclusive]; } }
        //public CswNbtNodePropLogical Show { get { return _CswNbtNode.Properties[PropertyName.Show]; } }
        public CswNbtNodePropRelationship ByUser { get { return _CswNbtNode.Properties[PropertyName.ByUser]; } }

        #endregion

    }//CswNbtObjClassRegulatoryListMember

}//namespace ChemSW.Nbt.ObjClasses
