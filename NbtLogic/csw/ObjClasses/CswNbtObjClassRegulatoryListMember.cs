using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

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
            public const string ByUser = "Required By User";
        }

        public CswNbtObjClassRegulatoryListMember( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

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

        protected override void afterDeleteNodeLogic()
        {
            // case 28303 - add list to Chemical's Suppressed list
            if( false == SetByChemical && CswTools.IsPrimaryKey( Chemical.RelatedNodeId ) )
            {
                CswNbtObjClassChemical ChemicalNode = _CswNbtResources.Nodes[Chemical.RelatedNodeId];
                if( null != ChemicalNode )
                {
                    ChemicalNode.addSuppressedRegulatoryList( RegulatoryList.RelatedNodeId );
                    ChemicalNode.postChanges( false );
                }
            }
        }//afterDeleteNode()        

        protected override void afterPopulateProps()
        {
            RegulatoryList.SetOnPropChange( _RegulatoryList_OnChange );
        }//afterPopulateProps()

        #endregion

        public bool SetByChemical = false;

        #region Object class specific properties


        public CswNbtNodePropRelationship RegulatoryList { get { return _CswNbtNode.Properties[PropertyName.RegulatoryList]; } }
        public void _RegulatoryList_OnChange( CswNbtNodeProp Prop, bool Creating )
        {
            if( null != RegulatoryList.RelatedNodeId &&
                RegulatoryList.RelatedNodeId.PrimaryKey != CswConvert.ToInt32( RegulatoryList.GetOriginalPropRowValue( CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID ) ) )
            {
                // case 28303 - set ByUser to current user when regulatory list is modified
                if( false == SetByChemical && false == _CswNbtResources.CurrentNbtUser is CswNbtSystemUser )
                {
                    ByUser.RelatedNodeId = _CswNbtResources.CurrentNbtUser.UserId;

                    // case 28303 - remove list from Chemical's Suppressed list
                    if( CswTools.IsPrimaryKey( Chemical.RelatedNodeId ) )
                    {
                        CswNbtObjClassChemical ChemicalNode = _CswNbtResources.Nodes[Chemical.RelatedNodeId];
                        if( null != ChemicalNode )
                        {
                            if( ChemicalNode.isRegulatoryListSuppressed( RegulatoryList.RelatedNodeId ) ) // important to prevent an infinite loop
                            {
                                ChemicalNode.removeSuppressedRegulatoryList( RegulatoryList.RelatedNodeId );
                                ChemicalNode.postChanges( false );
                            }
                        }
                    }
                }
            }
        } // _RegulatoryList_OnChange()
        public CswNbtNodePropRelationship Chemical { get { return _CswNbtNode.Properties[PropertyName.Chemical]; } }
        //public CswNbtNodePropCASNo CASNo { get { return _CswNbtNode.Properties[PropertyName.CASNo]; } }
        //public CswNbtNodePropLogical Exclusive { get { return _CswNbtNode.Properties[PropertyName.Exclusive]; } }
        //public CswNbtNodePropLogical Show { get { return _CswNbtNode.Properties[PropertyName.Show]; } }
        public CswNbtNodePropRelationship ByUser { get { return _CswNbtNode.Properties[PropertyName.ByUser]; } }

        #endregion

    }//CswNbtObjClassRegulatoryListMember

}//namespace ChemSW.Nbt.ObjClasses
