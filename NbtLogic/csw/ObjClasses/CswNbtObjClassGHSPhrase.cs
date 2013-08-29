using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassGHSPhrase : CswNbtObjClass
    {
        #region Enums
        /// <summary>
        /// Object Class property names
        /// </summary>
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Code = "Code";
            public const string Category = "Category";
            public const string English = "English";
            public const string Danish = "Danish";
            public const string Dutch = "Dutch";
            public const string Finnish = "Finnish";
            public const string French = "French";
            public const string German = "German";
            public const string Italian = "Italian";
            public const string Portuguese = "Portuguese";
            public const string Spanish = "Spanish";
            public const string Swedish = "Swedish";
            public const string Chinese = "Chinese";
        }

        #endregion Enums

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassGHSPhrase( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSPhraseClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassGHSPhrase
        /// </summary>
        public static implicit operator CswNbtObjClassGHSPhrase( CswNbtNode Node )
        {
            CswNbtObjClassGHSPhrase ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.GHSPhraseClass ) )
            {
                ret = (CswNbtObjClassGHSPhrase) Node.ObjClass;
            }
            return ret;
        }


        #region Inherited Events

        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( IsCopy, OverrideUniqueValidation );
        }//beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        }//afterCreateNode()


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

        public CswNbtNodePropText Code { get { return ( _CswNbtNode.Properties[PropertyName.Code] ); } }
        public CswNbtNodePropList Category { get { return ( _CswNbtNode.Properties[PropertyName.Category] ); } }
        public CswNbtNodePropText English { get { return ( _CswNbtNode.Properties[PropertyName.English] ); } }
        public CswNbtNodePropText Danish { get { return ( _CswNbtNode.Properties[PropertyName.Danish] ); } }
        public CswNbtNodePropText Dutch { get { return ( _CswNbtNode.Properties[PropertyName.Dutch] ); } }
        public CswNbtNodePropText Finnish { get { return ( _CswNbtNode.Properties[PropertyName.Finnish] ); } }
        public CswNbtNodePropText French { get { return ( _CswNbtNode.Properties[PropertyName.French] ); } }
        public CswNbtNodePropText German { get { return ( _CswNbtNode.Properties[PropertyName.German] ); } }
        public CswNbtNodePropText Italian { get { return ( _CswNbtNode.Properties[PropertyName.Italian] ); } }
        public CswNbtNodePropText Portuguese { get { return ( _CswNbtNode.Properties[PropertyName.Portuguese] ); } }
        public CswNbtNodePropText Spanish { get { return ( _CswNbtNode.Properties[PropertyName.Spanish] ); } }
        public CswNbtNodePropText Swedish { get { return ( _CswNbtNode.Properties[PropertyName.Swedish] ); } }
        public CswNbtNodePropText Chinese { get { return ( _CswNbtNode.Properties[PropertyName.Chinese] ); } }

        #endregion


    }//CswNbtObjClassGeneric

}//namespace ChemSW.Nbt.ObjClasses
