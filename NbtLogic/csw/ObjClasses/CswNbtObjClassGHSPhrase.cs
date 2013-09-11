using ChemSW.Core;
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
            public const string Bulgarian = "Bulgarian";
            public const string Chinese = "Chinese";
            public const string Czech = "Czech";
            public const string Danish = "Danish";
            public const string Dutch = "Dutch";
            public const string Estonian = "Estonian";
            public const string Finnish = "Finnish";
            public const string French = "French";
            public const string German = "German";
            public const string Greek = "Greek";
            public const string Hungarian = "Hungarian";
            public const string Irish = "Irish";
            public const string Italian = "Italian";
            public const string Latvian = "Latvian";
            public const string Lithuanian = "Lithuanian";
            public const string Maltese = "Maltese";
            public const string Polish = "Polish";
            public const string Portuguese = "Portuguese";
            public const string Romanian = "Romanian";
            public const string Slovac = "Slovac";
            public const string Slovenian = "Slovenian";
            public const string Spanish = "Spanish";
            public const string Swedish = "Swedish";
        }

        public sealed class SupportedLanguages
        {
            public static CswCommaDelimitedString All = new CswCommaDelimitedString
                {
                    PropertyName.Bulgarian,
                    PropertyName.Spanish,
                    PropertyName.Chinese,
                    PropertyName.Czech,
                    PropertyName.Danish,
                    PropertyName.German,
                    PropertyName.Estonian,
                    PropertyName.Greek,
                    PropertyName.English,
                    PropertyName.French,
                    PropertyName.Irish,
                    PropertyName.Italian,
                    PropertyName.Latvian,
                    PropertyName.Lithuanian,
                    PropertyName.Hungarian,
                    PropertyName.Maltese,
                    PropertyName.Dutch,
                    PropertyName.Polish,
                    PropertyName.Portuguese,
                    PropertyName.Romanian,
                    PropertyName.Slovac,
                    PropertyName.Slovenian,
                    PropertyName.Finnish,
                    PropertyName.Swedish
                };
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


        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation, bool Creating )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation, Creating );
        }//beforeWriteNode()

        public override void afterWriteNode( bool Creating )
        {
            _CswNbtObjClassDefault.afterWriteNode( Creating );
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
        public CswNbtNodePropText Bulgarian { get { return ( _CswNbtNode.Properties[PropertyName.Bulgarian] ); } }
        public CswNbtNodePropText Chinese { get { return ( _CswNbtNode.Properties[PropertyName.Chinese] ); } }
        public CswNbtNodePropText Czech { get { return ( _CswNbtNode.Properties[PropertyName.Czech] ); } }
        public CswNbtNodePropText Danish { get { return ( _CswNbtNode.Properties[PropertyName.Danish] ); } }
        public CswNbtNodePropText Dutch { get { return ( _CswNbtNode.Properties[PropertyName.Dutch] ); } }
        public CswNbtNodePropText Estonian { get { return ( _CswNbtNode.Properties[PropertyName.Estonian] ); } }
        public CswNbtNodePropText Finnish { get { return ( _CswNbtNode.Properties[PropertyName.Finnish] ); } }
        public CswNbtNodePropText French { get { return ( _CswNbtNode.Properties[PropertyName.French] ); } }
        public CswNbtNodePropText German { get { return ( _CswNbtNode.Properties[PropertyName.German] ); } }
        public CswNbtNodePropText Greek { get { return ( _CswNbtNode.Properties[PropertyName.Greek] ); } }
        public CswNbtNodePropText Hungarian { get { return ( _CswNbtNode.Properties[PropertyName.Hungarian] ); } }
        public CswNbtNodePropText Irish { get { return ( _CswNbtNode.Properties[PropertyName.Irish] ); } }
        public CswNbtNodePropText Italian { get { return ( _CswNbtNode.Properties[PropertyName.Italian] ); } }
        public CswNbtNodePropText Latvian { get { return ( _CswNbtNode.Properties[PropertyName.Latvian] ); } }
        public CswNbtNodePropText Lithuanian { get { return ( _CswNbtNode.Properties[PropertyName.Lithuanian] ); } }
        public CswNbtNodePropText Maltese { get { return ( _CswNbtNode.Properties[PropertyName.Maltese] ); } }
        public CswNbtNodePropText Polish { get { return ( _CswNbtNode.Properties[PropertyName.Polish] ); } }
        public CswNbtNodePropText Portuguese { get { return ( _CswNbtNode.Properties[PropertyName.Portuguese] ); } }
        public CswNbtNodePropText Romanian { get { return ( _CswNbtNode.Properties[PropertyName.Romanian] ); } }
        public CswNbtNodePropText Slovac { get { return ( _CswNbtNode.Properties[PropertyName.Slovac] ); } }
        public CswNbtNodePropText Slovenian { get { return ( _CswNbtNode.Properties[PropertyName.Slovenian] ); } }
        public CswNbtNodePropText Spanish { get { return ( _CswNbtNode.Properties[PropertyName.Spanish] ); } }
        public CswNbtNodePropText Swedish { get { return ( _CswNbtNode.Properties[PropertyName.Swedish] ); } }
        
        #endregion


    }//CswNbtObjClassGeneric

}//namespace ChemSW.Nbt.ObjClasses
