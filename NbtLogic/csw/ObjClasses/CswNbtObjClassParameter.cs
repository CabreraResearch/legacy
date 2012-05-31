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

        #region Inherited Events
        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode(bool DeleteAllRelatedNodes = false)
        {
            _CswNbtObjClassDefault.beforeDeleteNode(DeleteAllRelatedNodes);

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

        public override bool onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp, out NbtButtonAction ButtonAction, out string ActionData, out string Message )
        {
            Message = string.Empty;
            ActionData = string.Empty;
            ButtonAction = NbtButtonAction.Unknown;
            if( null != NodeTypeProp ) { /*Do Something*/ }
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
