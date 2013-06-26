using System;
using ChemSW.Core;
using ChemSW.Nbt.ChemCatCentral;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassRegulatoryListListCode : CswNbtObjClass
    {
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string RegulatoryList = "Regulatory List";
            public const string LOLIListName = "LOLI List Name";
            public const string LOLIListCode = "LOLI List Code";
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassRegulatoryListListCode( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );

        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListCasNoClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassRegulatoryListListCode
        /// </summary>
        public static implicit operator CswNbtObjClassRegulatoryListListCode( CswNbtNode Node )
        {
            CswNbtObjClassRegulatoryListListCode ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.RegulatoryListCasNoClass ) )
            {
                ret = (CswNbtObjClassRegulatoryListListCode) Node.ObjClass;
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

            LOLIListName.OnBeforeFilterOptions = searchLOLI;
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

        private void searchLOLI( string SearchTerm, Int32 SearchThreshold )
        {
            // Instance a ChemCatCentral SearchClient
            CswC3SearchParams CswC3SearchParams = new CswC3SearchParams();
            CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( _CswNbtResources, CswC3SearchParams );
            ChemCatCentral.SearchClient C3SearchClient = CswNbtC3ClientManager.initializeC3Client();

            CswC3SearchParams.Query = SearchTerm;

            // Perform the search
            CswRetObjSearchResults SearchResults = C3SearchClient.getListCodesByName( CswC3SearchParams );
            if( null != SearchResults.LoliDataResults )
            {
                if( SearchResults.LoliDataResults.Length > 0 && SearchResults.LoliDataResults.Length < SearchThreshold )
                {
                    CswCommaDelimitedString MatchingRegLists = new CswCommaDelimitedString();

                    foreach( CswC3LoliData LoliRecord in SearchResults.LoliDataResults )
                    {
                        MatchingRegLists.Add( LoliRecord.ListName );
                    }

                    // Set the list options
                    LOLIListName.Options.Override( MatchingRegLists );
                }
            }
        }

        #region Object class specific properties

        public CswNbtNodePropRelationship RegulatoryList { get { return _CswNbtNode.Properties[PropertyName.RegulatoryList]; } }
        public CswNbtNodePropList LOLIListName { get { return _CswNbtNode.Properties[PropertyName.LOLIListName]; } }
        public CswNbtNodePropNumber LOLIListCode { get { return _CswNbtNode.Properties[PropertyName.LOLIListCode]; } }

        #endregion

    }//CswNbtObjClassRegulatoryListListCode

}//namespace ChemSW.Nbt.ObjClasses
