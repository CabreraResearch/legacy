using System;
using System.Linq;
using System.Collections.Generic;
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
        public sealed class PropertyName
        {
            public const string Jurisdiction = "Jurisdiction";
            public const string Material = "Material";
            public const string LabelCodes = "Label Codes";
            public const string ClassCodes = "Class Codes";
            public const string LabelCodesGrid = "Label Codes Grid";
            public const string ClassCodesGrid = "Class Codes Grid";
            public const string SignalWord = "Signal Word";
        }

        #endregion Enums

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassGHS( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.GHSClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassGHS
        /// </summary>
        public static implicit operator CswNbtObjClassGHS( CswNbtNode Node )
        {
            CswNbtObjClassGHS ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.GHSClass ) )
            {
                ret = (CswNbtObjClassGHS) Node.ObjClass;
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

        public override void afterPopulateProps()
        {
            LabelCodes.InitOptions = _initGhsPhraseOptions;
            ClassCodes.InitOptions = _initGhsPhraseOptions;

            _setupPhraseView( LabelCodesGrid.View, LabelCodes.Value );
            _setupPhraseView( ClassCodesGrid.View, ClassCodes.Value );

            _CswNbtObjClassDefault.afterPopulateProps();
        } //afterPopulateProps()

        private void _setupPhraseView( CswNbtView View, CswCommaDelimitedString SelectedPhraseIds )
        {
            CswNbtMetaDataObjectClass GhsPhraseOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.GHSPhraseClass );
            CswNbtMetaDataNodeType GhsPhraseNT = GhsPhraseOC.FirstNodeType;

            View.SetVisibility( NbtViewVisibility.Hidden, null, null );
            View.Root.ChildRelationships.Clear();
            CswNbtViewRelationship PhraseVR = View.AddViewRelationship( GhsPhraseOC, false );
            foreach( string PhraseId in SelectedPhraseIds )
            {
                CswPrimaryKey PhrasePk = new CswPrimaryKey();
                PhrasePk.FromString( PhraseId );
                PhraseVR.NodeIdsToFilterIn.Add( PhrasePk );
            }

            // Add language
            // TODO: Make this dependent on user's default language
            View.AddViewProperty( PhraseVR, GhsPhraseOC.getObjectClassProp( CswNbtObjClassGHSPhrase.PropertyName.Code ) );
            if( null != GhsPhraseNT )
            {
                CswNbtMetaDataNodeTypeProp EnglishNTP = GhsPhraseNT.getNodeTypeProp( "English" );
                if( null != EnglishNTP )
                {
                    CswNbtViewProperty EnglishVP = View.AddViewProperty( PhraseVR, EnglishNTP );
                    EnglishVP.Width = 100;
                }
            }

            View.save();
        }

        private Dictionary<string, string> _initGhsPhraseOptions()
        {
            CswNbtMetaDataObjectClass GhsPhraseOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.GHSPhraseClass );
            Dictionary<CswPrimaryKey, string> Phrases = GhsPhraseOC.getNodeIdAndNames( false, false );
            return Phrases.Keys.ToDictionary( pk => pk.ToString(), pk => Phrases[pk] );
        } // _initGhsPhraseOptions()

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

        public CswNbtNodePropRelationship Jurisdiction { get { return ( _CswNbtNode.Properties[PropertyName.Jurisdiction] ); } }
        public CswNbtNodePropRelationship Material { get { return ( _CswNbtNode.Properties[PropertyName.Material] ); } }
        public CswNbtNodePropMultiList LabelCodes { get { return ( _CswNbtNode.Properties[PropertyName.LabelCodes] ); } }
        public CswNbtNodePropMultiList ClassCodes { get { return ( _CswNbtNode.Properties[PropertyName.ClassCodes] ); } }
        public CswNbtNodePropGrid LabelCodesGrid { get { return ( _CswNbtNode.Properties[PropertyName.LabelCodesGrid] ); } }
        public CswNbtNodePropGrid ClassCodesGrid { get { return ( _CswNbtNode.Properties[PropertyName.ClassCodesGrid] ); } }
        public CswNbtNodePropList SignalWord { get { return ( _CswNbtNode.Properties[PropertyName.SignalWord] ); } }

        #endregion


    }//CswNbtObjClassGeneric

}//namespace ChemSW.Nbt.ObjClasses
