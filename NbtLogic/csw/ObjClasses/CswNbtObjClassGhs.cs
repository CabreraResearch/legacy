using System.Collections.Generic;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassGHS : CswNbtObjClass
    {
        #region Enums
        /// <summary>
        /// Object Class property names
        /// </summary>
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string AddLabelCodes = "Add Label Codes";
            public const string Classifications = "Classifications";
            public const string ClassificationsGrid = "Classifications Grid";
            public const string Jurisdiction = "Jurisdiction";
            public const string LabelCodes = "Label Codes";
            public const string LabelCodesGrid = "Label Codes Grid";
            public const string Material = "Material";
            public const string Pictograms = "Pictograms";
            public const string SignalWord = "Signal Word";
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

        public CswNbtObjClassGHS( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

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

        protected override void afterPopulateProps()
        {
            LabelCodes.InitOptions = _initGhsPhraseOptions;
            Classifications.InitOptions = _initGhsClassificationOptions;

            AddLabelCodes.SetOnPropChange( OnAddLabelCodesPropChange );
            Pictograms.SetOnPropChange( OnPictorgramsChange );

            LabelCodesGrid.SetOnBeforeRender( delegate( CswNbtNodeProp Prop )
                {
                    CswNbtNodePropGrid PropAsGrid = (CswNbtNodePropGrid) Prop;
                    CswNbtView UpdatedPhraseView = setupPhraseView( PropAsGrid.View, LabelCodes.Value );
                    UpdatedPhraseView.SaveToCache( IncludeInQuickLaunch: false, UpdateCache: true, KeepInQuickLaunch: false );
                } );

            ClassificationsGrid.SetOnBeforeRender( delegate( CswNbtNodeProp Prop )
                {
                    CswNbtNodePropGrid PropAsGrid = (CswNbtNodePropGrid) Prop;
                    _setupClassificationView( PropAsGrid.View, Classifications.Value );
                } );

            SignalWord.SetOnBeforeRender( delegate( CswNbtNodeProp Prop )
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

        public CswNbtView setupPhraseView( CswNbtView View, CswCommaDelimitedString SelectedPhraseIds )
        {
            CswNbtMetaDataObjectClass GhsPhraseOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSPhraseClass );
            CswNbtMetaDataNodeType GhsPhraseNT = GhsPhraseOC.FirstNodeType;

            View.SetVisibility( CswEnumNbtViewVisibility.Hidden, null, null );
            View.ViewMode = CswEnumNbtViewRenderingMode.Grid;
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

            return View;

        } // setupPhraseView()


        private void _setupClassificationView( CswNbtView View, CswCommaDelimitedString SelectedClassIds )
        {
            CswNbtMetaDataObjectClass GhsClassOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSClassificationClass );
            CswNbtMetaDataNodeType GhsClassNT = GhsClassOC.FirstNodeType;

            View.SetVisibility( CswEnumNbtViewVisibility.Hidden, null, null );
            View.ViewMode = CswEnumNbtViewRenderingMode.Grid;
            View.Root.ChildRelationships.Clear();
            if( SelectedClassIds.Count > 0 )
            {
                CswNbtViewRelationship ClassVR = View.AddViewRelationship( GhsClassOC, false );
                foreach( string ClassId in SelectedClassIds )
                {
                    CswPrimaryKey ClassPk = new CswPrimaryKey();
                    ClassPk.FromString( ClassId );
                    ClassVR.NodeIdsToFilterIn.Add( ClassPk );
                }

                View.AddViewProperty( ClassVR, GhsClassOC.getObjectClassProp( CswNbtObjClassGHSClassification.PropertyName.Category ) );
                if( null != GhsClassNT )
                {
                    CswNbtMetaDataNodeTypeProp LanguageProp = GhsClassNT.getNodeTypePropByObjectClassProp( _getLanguageForTranslation() );
                    CswNbtViewProperty LanguageVP = View.AddViewProperty( ClassVR, LanguageProp );
                    LanguageVP.Width = 100;
                }
            } // if( SelectedClassIds.Count > 0 )
            View.SaveToCache( IncludeInQuickLaunch: false, UpdateCache: true, KeepInQuickLaunch: false );
        } // _setupClassificationView()


        private Dictionary<string, string> _initGhsPhraseOptions()
        {
            CswNbtMetaDataObjectClass GhsPhraseOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSPhraseClass );
            Dictionary<CswPrimaryKey, string> Phrases = GhsPhraseOC.getNodeIdAndNames( false, false );
            return Phrases.Keys.ToDictionary( pk => pk.ToString(), pk => Phrases[pk] );
        } // _initGhsPhraseOptions()

        private Dictionary<string, string> _initGhsClassificationOptions()
        {
            CswNbtMetaDataObjectClass GhsClassOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSClassificationClass );
            Dictionary<CswPrimaryKey, string> Classes = GhsClassOC.getNodeIdAndNames( false, false );
            return Classes.Keys.ToDictionary( pk => pk.ToString(), pk => Classes[pk] );
        } // _initGhsClassificationOptions()

        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Jurisdiction { get { return ( _CswNbtNode.Properties[PropertyName.Jurisdiction] ); } }
        public CswNbtNodePropRelationship Material { get { return ( _CswNbtNode.Properties[PropertyName.Material] ); } }
        public CswNbtNodePropMultiList LabelCodes { get { return ( _CswNbtNode.Properties[PropertyName.LabelCodes] ); } }
        public CswNbtNodePropMultiList Classifications { get { return ( _CswNbtNode.Properties[PropertyName.Classifications] ); } }
        public CswNbtNodePropMemo AddLabelCodes { get { return ( _CswNbtNode.Properties[PropertyName.AddLabelCodes] ); } }
        private void OnAddLabelCodesPropChange( CswNbtNodeProp Prop, bool Creating )
        {
            CswCommaDelimitedString NewGHSLabelCodes = _getGHSCodesFromMemo( AddLabelCodes.Text );
            foreach( string LabelCode in NewGHSLabelCodes )
            {
                if( LabelCodes.Options.ContainsValue( LabelCode.Trim().ToUpper() ) )
                {
                    KeyValuePair<string, string> first = new KeyValuePair<string, string>();
                    foreach( KeyValuePair<string, string> x in LabelCodes.Options.Where( x => x.Value == LabelCode.Trim().ToUpper() ) )
                    {
                        first = x;
                        break;
                    }
                    if( false == LabelCodes.Value.Contains( first.Key ) )
                    {
                        LabelCodes.AddValue( first.Key );
                    }
                }
            }
            AddLabelCodes.Text = string.Empty;
        }
        public CswNbtNodePropGrid LabelCodesGrid { get { return ( _CswNbtNode.Properties[PropertyName.LabelCodesGrid] ); } }
        public CswNbtNodePropGrid ClassificationsGrid { get { return ( _CswNbtNode.Properties[PropertyName.ClassificationsGrid] ); } }
        public CswNbtNodePropRelationship SignalWord { get { return ( _CswNbtNode.Properties[PropertyName.SignalWord] ); } }
        public CswNbtNodePropImageList Pictograms { get { return ( _CswNbtNode.Properties[PropertyName.Pictograms] ); } }
        public void OnPictorgramsChange( CswNbtNodeProp Prop, bool Creating )
        {
            // Save the pictorgrams in the same order they are presented on the client
            CswDelimitedString CurrentPictograms = Pictograms.Value;
            CswDelimitedString OrderedPictograms = new CswDelimitedString( CswNbtNodePropImageList.Delimiter );
            foreach( KeyValuePair<string, string> keyValuePair in Pictograms.Options )
            {
                if( CurrentPictograms.Contains( keyValuePair.Key ) )
                {
                    OrderedPictograms.Add( keyValuePair.Key );
                }
            }
            Pictograms.Value = OrderedPictograms;
        }

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
