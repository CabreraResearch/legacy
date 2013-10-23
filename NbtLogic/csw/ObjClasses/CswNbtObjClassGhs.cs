using System.Collections.Generic;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassGHS: CswNbtObjClass
    {
        #region Enums
        /// <summary>
        /// Object Class property names
        /// </summary>
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Jurisdiction = "Jurisdiction";
            public const string Material = "Material";
            public const string LabelCodes = "Label Codes";
            public const string ClassCodes = "Class Codes";
            public const string AddLabelCodes = "Add Label Codes";
            public const string AddClassCodes = "Add Class Codes";
            public const string LabelCodesGrid = "Label Codes Grid";
            public const string ClassCodesGrid = "Class Codes Grid";
            public const string SignalWord = "Signal Word";
            public const string Pictograms = "Pictograms";
            public const string Classification = "Classification";
        }

        public static readonly Dictionary<string, string> LanguageCodeMap = new Dictionary<string, string>
            {
                {"BG", "Bulgarian"},
                {"ES", "Spanish"},
                {"CS", "Czech"},
                {"DA", "Danish"},
                {"DE", "German"},
                {"ET", "Estonian"},
                {"EL", "Greek"},
                {"EN", "English"},
                {"FR", "French"},
                {"GA", "Irish"},
                {"IT", "Italian"},
                {"LV", "Latvian"},
                {"LT", "Lithuanian"},
                {"HU", "Hungarian"},
                {"MT", "Maltese"},
                {"NL", "Dutch"},
                {"PL", "Polish"},
                {"PT", "Portuguese"},
                {"RO", "Romanian"},
                {"SK", "Slovac"},
                {"SL", "Slovenian"},
                {"FI", "Finnish"},
                {"SV", "Swedish"}
            };

        #endregion Enums

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassGHS( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassGHS
        /// </summary>
        public static implicit operator CswNbtObjClassGHS( CswNbtNode Node )
        {
            CswNbtObjClassGHS ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.GHSClass ) )
            {
                ret = (CswNbtObjClassGHS) Node.ObjClass;
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
            LabelCodes.InitOptions = _initGhsPhraseOptions;
            ClassCodes.InitOptions = _initGhsPhraseOptions;

            AddLabelCodes.SetOnPropChange( OnAddLabelCodesPropChange );
            AddClassCodes.SetOnPropChange( OnAddClassCodesPropChange );

            _CswNbtNode.Properties[PropertyName.LabelCodesGrid].SetOnBeforeRender( delegate( CswNbtNodeProp Prop )
                {
                    CswNbtNodePropGrid PropAsGrid = (CswNbtNodePropGrid) Prop;
                    _setupPhraseView( PropAsGrid.View, LabelCodes.Value );
                } );

            _CswNbtNode.Properties[PropertyName.ClassCodesGrid].SetOnBeforeRender( delegate( CswNbtNodeProp Prop )
                {
                    CswNbtNodePropGrid PropAsGrid = (CswNbtNodePropGrid) Prop;
                    _setupPhraseView( PropAsGrid.View, ClassCodes.Value );
                } );

            _CswNbtNode.Properties[PropertyName.SignalWord].SetOnBeforeRender( delegate( CswNbtNodeProp Prop )
                {
                    CswNbtNodePropRelationship PropAsRelationship = (CswNbtNodePropRelationship) Prop;
                    Dictionary<CswPrimaryKey, string> TranslatedOpts = new Dictionary<CswPrimaryKey, string>();
                    ICswNbtTree tree = _CswNbtResources.Trees.getTreeFromView( PropAsRelationship.View, true, false, false );
                    int count = tree.getChildNodeCount();
                    for( int i = 0; i < count; i++ )
                    {
                        tree.goToNthChild( i );
                        CswNbtObjClassGHSSignalWord SignalWordNode = tree.getNodeForCurrentPosition();
                        string TranslatedText = SignalWordNode.Node.Properties[_getLanguageForTranslation()].AsText.Text;
                        TranslatedOpts.Add( SignalWordNode.NodeId, TranslatedText );
                        tree.goToParentNode();
                    }

                    PropAsRelationship.SetOptionsOverride( TranslatedOpts );
                    if( CswTools.IsPrimaryKey( PropAsRelationship.RelatedNodeId ) )
                    {
                        CswNbtObjClassGHSSignalWord Selected = _CswNbtResources.Nodes[PropAsRelationship.RelatedNodeId];
                        string TranslantedText = Selected.Node.Properties[_getLanguageForTranslation()].AsText.Text;
                        Selected.Node.NodeName = ( false == string.IsNullOrEmpty( TranslantedText ) ? TranslantedText : Selected.English.Text );
                    }
                } );

            _CswNbtObjClassDefault.triggerAfterPopulateProps();
        } //afterPopulateProps()

        /// <summary>
        /// Gets the name of the Language property to use for translations from the current user - defaults to English if not a supported language
        /// </summary>
        private string _getLanguageForTranslation()
        {
            string ret = "English";
            if( LanguageCodeMap.ContainsKey( _CswNbtResources.CurrentNbtUser.Language.ToUpper() ) )
            {
                ret = LanguageCodeMap[_CswNbtResources.CurrentNbtUser.Language.ToUpper()];
            }
            return ret;
        }

        private void _setupPhraseView( CswNbtView View, CswCommaDelimitedString SelectedPhraseIds )
        {
            CswNbtMetaDataObjectClass GhsPhraseOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSPhraseClass );
            CswNbtMetaDataNodeType GhsPhraseNT = GhsPhraseOC.FirstNodeType;

            View.SetVisibility( CswEnumNbtViewVisibility.Hidden, null, null );
            View.Root.ChildRelationships.Clear();
            if( SelectedPhraseIds.Count > 0 )
            {
                CswNbtViewRelationship PhraseVR = View.AddViewRelationship( GhsPhraseOC, false );
                foreach( string PhraseId in SelectedPhraseIds )
                {
                    CswPrimaryKey PhrasePk = new CswPrimaryKey();
                    PhrasePk.FromString( PhraseId );
                    PhraseVR.NodeIdsToFilterIn.Add( PhrasePk );
                }

                View.AddViewProperty( PhraseVR, GhsPhraseOC.getObjectClassProp( CswNbtObjClassGHSPhrase.PropertyName.Code ) );
                if( null != GhsPhraseNT )
                {
                    CswNbtMetaDataNodeTypeProp LanguageProp = GhsPhraseNT.getNodeTypePropByObjectClassProp( _getLanguageForTranslation() );
                    CswNbtViewProperty LanguageVP = View.AddViewProperty( PhraseVR, LanguageProp );
                    LanguageVP.Width = 100;
                }
            } // if( SelectedPhraseIds.Count > 0 )
            View.SaveToCache( IncludeInQuickLaunch : false, UpdateCache : true, KeepInQuickLaunch : false );
        } // _setupPhraseView()

        private Dictionary<string, string> _initGhsPhraseOptions()
        {
            CswNbtMetaDataObjectClass GhsPhraseOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSPhraseClass );
            Dictionary<CswPrimaryKey, string> Phrases = GhsPhraseOC.getNodeIdAndNames( false, false );
            return Phrases.Keys.ToDictionary( pk => pk.ToString(), pk => Phrases[pk] );
        } // _initGhsPhraseOptions()

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

        public CswNbtNodePropRelationship Jurisdiction { get { return ( _CswNbtNode.Properties[PropertyName.Jurisdiction] ); } }
        public CswNbtNodePropRelationship Material { get { return ( _CswNbtNode.Properties[PropertyName.Material] ); } }
        public CswNbtNodePropMultiList LabelCodes { get { return ( _CswNbtNode.Properties[PropertyName.LabelCodes] ); } }
        public CswNbtNodePropMultiList ClassCodes { get { return ( _CswNbtNode.Properties[PropertyName.ClassCodes] ); } }
        public CswNbtNodePropMemo AddLabelCodes { get { return ( _CswNbtNode.Properties[PropertyName.AddLabelCodes] ); } }
        private void OnAddLabelCodesPropChange( CswNbtNodeProp Prop, bool Creating )
        {
            CswCommaDelimitedString NewGHSLabelCodes = _getGHSCodesFromMemo( AddLabelCodes.Text );
            foreach( string LabelCode in NewGHSLabelCodes )
            {
                if( LabelCodes.Options.ContainsValue( LabelCode.Trim().ToUpper() ) )
                {
                    LabelCodes.AddValue( LabelCodes.Options.FirstOrDefault( x => x.Value == LabelCode.Trim().ToUpper() ).Key );
                }
            }
            AddLabelCodes.Text = string.Empty;
        }
        public CswNbtNodePropMemo AddClassCodes { get { return ( _CswNbtNode.Properties[PropertyName.AddClassCodes] ); } }
        private void OnAddClassCodesPropChange( CswNbtNodeProp Prop, bool Creating )
        {
            CswCommaDelimitedString NewGHSClassCodes = _getGHSCodesFromMemo( AddClassCodes.Text );
            foreach( string ClassCode in NewGHSClassCodes )
            {
                if( ClassCodes.Options.ContainsValue( ClassCode.Trim().ToUpper() ) )
                {
                    ClassCodes.AddValue( ClassCodes.Options.FirstOrDefault( x => x.Value == ClassCode.Trim().ToUpper() ).Key );
                }
            }
            AddClassCodes.Text = string.Empty;
        }
        public CswNbtNodePropGrid LabelCodesGrid { get { return ( _CswNbtNode.Properties[PropertyName.LabelCodesGrid] ); } }
        public CswNbtNodePropGrid ClassCodesGrid { get { return ( _CswNbtNode.Properties[PropertyName.ClassCodesGrid] ); } }
        public CswNbtNodePropRelationship SignalWord { get { return ( _CswNbtNode.Properties[PropertyName.SignalWord] ); } }
        public CswNbtNodePropImageList Pictograms { get { return ( _CswNbtNode.Properties[PropertyName.Pictograms] ); } }
        public CswNbtNodePropList Classification { get { return ( _CswNbtNode.Properties[PropertyName.Classification] ); } }

        #endregion

        private CswCommaDelimitedString _getGHSCodesFromMemo( string NewGHSCodes )
        {
            NewGHSCodes = NewGHSCodes.Replace( "\r\n", "," ); // Turn all delimiters into commas
            NewGHSCodes = NewGHSCodes.Replace( "\n", "," ); // Turn all delimiters into commas
            NewGHSCodes = NewGHSCodes.Replace( " ", "" ); // Trim whitespace
            CswCommaDelimitedString NewGHSCodesDelimited = new CswCommaDelimitedString();
            NewGHSCodesDelimited.FromString( NewGHSCodes, true );
            return NewGHSCodesDelimited;
        }

    }//CswNbtObjClassGHS

}//namespace ChemSW.Nbt.ObjClasses
