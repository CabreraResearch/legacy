using System;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
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
            // Set the value of the LOLIListCode property
            if( LOLIListCode.Empty && false == string.IsNullOrEmpty( LOLIListName.Value ) )
            {
                LOLIListCode.Value = CswConvert.ToDouble( LOLIListName.Value );
                LOLIListCode.SyncGestalt();
            }

            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation, Creating );
        }//beforeWriteNode()

        public override void afterWriteNode( bool Creating )
        {
            _setChemicalsPendingUpdate(); // TODO: Move to afterCreateNode() when Design Mode is done
            _CswNbtObjClassDefault.afterWriteNode( Creating );
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );
        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _setChemicalsPendingUpdate();
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        protected override void afterPopulateProps()
        {
            LOLIListName.OnBeforeFilterOptions = _searchLOLI;
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

        private void _searchLOLI( string SearchTerm, Int32 SearchThreshold )
        {
            // Instance a ChemCatCentral SearchClient
            CswC3SearchParams CswC3SearchParams = new CswC3SearchParams();
            CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( _CswNbtResources, CswC3SearchParams );
            SearchClient C3SearchClient = CswNbtC3ClientManager.initializeC3Client();
            if( null != C3SearchClient )
            {
                CswC3SearchParams.Query = SearchTerm;

                // Perform the search
                CswRetObjSearchResults SearchResults = C3SearchClient.getListCodesByName( CswC3SearchParams );
                if( null != SearchResults.LoliDataResults )
                {
                    if( SearchResults.LoliDataResults.Length > 0 && SearchResults.LoliDataResults.Length < SearchThreshold )
                    {
                        Collection<CswNbtNodeTypePropListOption> MatchingRegLists = new Collection<CswNbtNodeTypePropListOption>();

                        foreach( CswC3LoliData LoliRecord in SearchResults.LoliDataResults )
                        {
                            MatchingRegLists.Add( new CswNbtNodeTypePropListOption( LoliRecord.ListName, CswConvert.ToString( LoliRecord.ListId ) ) );
                        }

                        // Set the list options
                        LOLIListName.Options.Options = MatchingRegLists;
                    }
                }
            }
        }

        private void _setChemicalsPendingUpdate()
        {
            // Not ideal, but... set all chemicals to refresh their reg lists
            // We do this directly, not using a view, for performance
            CswTableUpdate ChemicalNodesUpdate = _CswNbtResources.makeCswTableUpdate( "CswNbtObjClassRegulatoryListListCode_lolilistcode_update", "nodes" );
            DataTable NodesTable = ChemicalNodesUpdate.getTable( @"where pendingupdate = '0' 
                                                                     and nodetypeid in (select nodetypeid from nodetypes 
                                                                                         where objectclassid = (select objectclassid from object_class 
                                                                                                                 where objectclass = '" + CswEnumNbtObjectClass.ChemicalClass + "'))" );
            foreach( DataRow NodesRow in NodesTable.Rows )
            {
                NodesRow["pendingupdate"] = "1";
            }
            ChemicalNodesUpdate.update( NodesTable );
        } // _setChemicalsPendingUpdate()

        #region Object class specific properties

        public CswNbtNodePropRelationship RegulatoryList { get { return _CswNbtNode.Properties[PropertyName.RegulatoryList]; } }
        public CswNbtNodePropList LOLIListName { get { return _CswNbtNode.Properties[PropertyName.LOLIListName]; } }
        public CswNbtNodePropNumber LOLIListCode { get { return _CswNbtNode.Properties[PropertyName.LOLIListCode]; } }

        #endregion

    }//CswNbtObjClassRegulatoryListListCode

}//namespace ChemSW.Nbt.ObjClasses
