using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Document Property Set
    /// </summary>
    public abstract class CswNbtPropertySetPhrase: CswNbtObjClass
    {
        #region Enums

        /// <summary>
        /// Object Class property names
        /// </summary>
        public new class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Code = "Code";
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

        #region Base

        /// <summary>
        /// Default Object Class for consumption by derived classes
        /// </summary>
        public CswNbtObjClassDefault CswNbtObjClassDefault = null;

        /// <summary>
        /// Property Set ctor
        /// </summary>
        protected CswNbtPropertySetPhrase( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public CswNbtMetaDataPropertySet PropertySet
        {
            get { return _CswNbtResources.MetaData.getPropertySet( CswEnumNbtPropertySetName.PhraseSet ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtPropertySetDocument
        /// </summary>
        public static implicit operator CswNbtPropertySetPhrase( CswNbtNode Node )
        {
            CswNbtPropertySetPhrase ret = null;
            if( null != Node && Members().Contains( Node.ObjClass.ObjectClass.ObjectClass ) )
            {
                ret = (CswNbtPropertySetPhrase) Node.ObjClass;
            }
            return ret;
        }

        public static Collection<CswEnumNbtObjectClass> Members()
        {
            Collection<CswEnumNbtObjectClass> Ret = new Collection<CswEnumNbtObjectClass>
            {
                CswEnumNbtObjectClass.GHSPhraseClass
            };
            return Ret;
        }

        #endregion Base

        #region Abstract Methods

        /// <summary>
        /// Before write node event for derived classes to implement
        /// </summary>
        public abstract void beforePropertySetWriteNode( bool IsCopy, bool OverrideUniqueValidation );

        /// <summary>
        /// After write node event for derived classes to implement
        /// </summary>
        public abstract void afterPropertySetWriteNode();

        /// <summary>
        /// Before delete node event for derived classes to implement
        /// </summary>
        public abstract void beforePropertySetDeleteNode( bool DeleteAllRequiredRelatedNodes = false );

        /// <summary>
        /// After delete node event for derived classes to implement
        /// </summary>
        public abstract void afterPropertySetDeleteNode();

        /// <summary>
        /// Populate props event for derived classes to implement
        /// </summary>
        public abstract void afterPropertySetPopulateProps();

        /// <summary>
        /// Button click event for derived classes to implement
        /// </summary>
        public abstract bool onPropertySetButtonClick( NbtButtonData ButtonData );

        /// <summary>
        /// Mechanism to add default filters in derived classes
        /// </summary>
        public abstract void onPropertySetAddDefaultViewFilters( CswNbtViewRelationship ParentRelationship );

        #endregion Abstract Methods

        #region Inherited Events

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation, bool Creating )
        {
            beforePropertySetWriteNode( IsCopy, OverrideUniqueValidation );


            CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation, Creating );
        }

        public override void afterWriteNode( bool Creating )
        {
            afterPropertySetWriteNode();
            CswNbtObjClassDefault.afterWriteNode( Creating );
        }

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            beforePropertySetDeleteNode( DeleteAllRequiredRelatedNodes );
            CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );
        }

        public override void afterDeleteNode()
        {
            afterPropertySetDeleteNode();
            CswNbtObjClassDefault.afterDeleteNode();
        }

        protected override void afterPopulateProps()
        {
            CswNbtObjClassDefault.triggerAfterPopulateProps();
        }

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            onPropertySetAddDefaultViewFilters( ParentRelationship );
            CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }

        #endregion Inherited Events

        #region Property Set specific properties

        public CswNbtNodePropText Code { get { return ( _CswNbtNode.Properties[PropertyName.Code] ); } }
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

    }//CswNbtPropertySetDocument

}//namespace ChemSW.Nbt.ObjClasses