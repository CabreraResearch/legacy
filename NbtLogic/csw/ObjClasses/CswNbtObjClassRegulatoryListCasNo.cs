using System;
using System.Text.RegularExpressions;
using ChemSW.Core;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassRegulatoryListCasNo : CswNbtObjClass
    {
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string RegulatoryList = "Regulatory List";
            public const string CASNo = "CAS No";
            public const string TPQ = "TPQ";
            //public const string IsValid = "Is Valid";
            public const string ErrorMessage = "Error Message";
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassRegulatoryListCasNo( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );

        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListMemberClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassRegulatoryListCasNo
        /// </summary>
        public static implicit operator CswNbtObjClassRegulatoryListCasNo( CswNbtNode Node )
        {
            CswNbtObjClassRegulatoryListCasNo ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.RegulatoryListMemberClass ) )
            {
                ret = (CswNbtObjClassRegulatoryListCasNo) Node.ObjClass;
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

            CASNo.SetOnPropChange( _CasNo_OnChange );
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
        
        public CswNbtNodePropCASNo CASNo { get { return _CswNbtNode.Properties[PropertyName.CASNo]; } }
        public void _CasNo_OnChange( CswNbtNodeProp Prop )
        {
            string error;
            if( false == CASNo.Validate( out error ) )
            {
                //IsValid.Checked = CswEnumTristate.False;
                ErrorMessage.Text = error;
            }
        }

        public CswNbtNodePropNumber TPQ { get { return _CswNbtNode.Properties[PropertyName.TPQ]; } }
        //public CswNbtNodePropLogical IsValid { get { return _CswNbtNode.Properties[PropertyName.IsValid]; } }
        public CswNbtNodePropMemo ErrorMessage { get { return _CswNbtNode.Properties[PropertyName.ErrorMessage]; } }
        
        #endregion

    }//CswNbtObjClassRegulatoryListCasNo

}//namespace ChemSW.Nbt.ObjClasses
