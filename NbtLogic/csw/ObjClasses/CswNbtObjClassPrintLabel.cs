using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;



namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassPrintLabel : CswNbtObjClass
    {
        public static string EplTextPropertyName { get { return "epltext"; } }
        public static string ParamsPropertyName { get { return "params"; } }
        public static string NodeTypesPropertyName { get { return "NodeTypes"; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassPrintLabel( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public CswNbtObjClassPrintLabel( CswNbtResources CswNbtResources )
            : base( CswNbtResources )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.PrintLabelClass ); }
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

        public override void beforeWriteNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode()
        {
            _CswNbtObjClassDefault.beforeDeleteNode();

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

        public override void onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            if( null != NodeTypeProp ) { /*Do Something*/ }
        }
        #endregion

        #region Object class specific properties



        public CswNbtNodePropMemo epltext
        {
            get
            {
                return ( _CswNbtNode.Properties[EplTextPropertyName].AsMemo );
            }
        }

        public CswNbtNodePropMemo Params
        {
            get
            {
                return ( _CswNbtNode.Properties[ParamsPropertyName].AsMemo );
            }
        }

        public CswNbtNodePropNodeTypeSelect NodeTypes
        {
            get
            {
                return ( _CswNbtNode.Properties[NodeTypesPropertyName].AsNodeTypeSelect );
            }
        }


        #endregion

    }//CswNbtObjClassPrintLabel

}//namespace ChemSW.Nbt.ObjClasses
