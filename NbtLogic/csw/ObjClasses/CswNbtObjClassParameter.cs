using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassParameter : CswNbtObjClass
    {
        public static string TestPropertyName { get { return "Test"; } }
        public static string ResultTypePropertyName { get { return "Result Type"; } }
        //public static string NumberOfResultsPropertyName { get { return "Number of Results"; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassParameter( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ParameterClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassParameter
        /// </summary>
        public static implicit operator CswNbtObjClassParameter( CswNbtNode Node )
        {
            CswNbtObjClassParameter ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.ParameterClass ) )
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

        public override bool onButtonClick( NbtButtonData ButtonData )
        {
            
            
            
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Test
        {
            get { return _CswNbtNode.Properties[TestPropertyName].AsRelationship; }
        }
        public CswNbtNodePropNodeTypeSelect ResultType
        {
            get { return _CswNbtNode.Properties[ResultTypePropertyName].AsNodeTypeSelect; }
        }
        //public CswNbtNodePropNumber NumberOfResults
        //{
        //    get { return _CswNbtNode.Properties[NumberOfResultsPropertyName].AsNumber; }
        //}


        #endregion

    }//CswNbtObjClassResultParam

}//namespace ChemSW.Nbt.ObjClasses
