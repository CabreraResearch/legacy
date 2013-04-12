using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassParameter : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Test = "Test";
            public const string ResultType = "Result Type";
            //public static string NumberOfResultsPropertyName { get { return "Number of Results"; } }    
        }


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassParameter( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ParameterClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassParameter
        /// </summary>
        public static implicit operator CswNbtObjClassParameter( CswNbtNode Node )
        {
            CswNbtObjClassParameter ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.ParameterClass ) )
            {
                ret = (CswNbtObjClassParameter) Node.ObjClass;
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

        public CswNbtNodePropRelationship Test { get { return _CswNbtNode.Properties[PropertyName.Test]; } }
        public CswNbtNodePropNodeTypeSelect ResultType { get { return _CswNbtNode.Properties[PropertyName.ResultType]; } }

        //public CswNbtNodePropNumber NumberOfResults
        //{
        //    get { return _CswNbtNode.Properties[NumberOfResultsPropertyName].AsNumber; }
        //}


        #endregion

    }//CswNbtObjClassResultParam

}//namespace ChemSW.Nbt.ObjClasses
