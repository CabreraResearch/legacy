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
            public const string ListName = "List Name";
            public const string ListCode = "List Code";
        }

        public CswNbtObjClassRegulatoryListListCode( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

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

        protected override void beforeWriteNodeLogic( bool Creating, bool OverrideUniqueValidation )
        {
            // Set the value of the ListCode property
            if( ListCode.Empty && false == string.IsNullOrEmpty( ListName.Value ) )
            {
                ListCode.Text = ListName.Value;
                ListCode.SyncGestalt();
            }
        }

        protected override void afterPromoteNodeLogic()
        {
            _setChemicalsPendingUpdate();
        }

        protected override void afterDeleteNodeLogic()
        {
            _setChemicalsPendingUpdate();
        }

        protected override void afterPopulateProps()
        {
            ListName.OnBeforeFilterOptions = _searchRegulationDb;
        }

        #endregion

        private void _searchRegulationDb( string SearchTerm, Int32 SearchThreshold )
        {
            CswC3SearchParams CswC3SearchParams = new CswC3SearchParams();
            CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( _CswNbtResources, CswC3SearchParams );
            SearchClient C3SearchClient = CswNbtC3ClientManager.initializeC3Client();
            if( null != C3SearchClient )
            {
                CswNbtObjClassRegulatoryList RegListNode = _CswNbtResources.Nodes.GetNode( RegulatoryList.RelatedNodeId );
                if( null != RegListNode )
                {
                    CswC3SearchParams.RegulationDatabase = CswNbtC3ClientManager.RegulationDatabase;

                    string Regions = "";
                    if( string.IsNullOrEmpty( CswConvert.ToString( RegListNode.Regions.Value ) ) )
                    {
                        if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.ArielSync ) )
                        {
                            Regions = _CswNbtResources.ConfigVbls.getConfigVariableValue( CswConvert.ToString( CswEnumNbtConfigurationVariables.arielmodules ) );
                        }
                    }
                    else
                    {
                        Regions = CswConvert.ToString( RegListNode.Regions.Value );
                    }
                    CswC3SearchParams.Regions = Regions;
                }
                CswC3SearchParams.Query = SearchTerm;

                // Perform the search
                CswRetObjSearchResults SearchResults = C3SearchClient.getListCodesByName( CswC3SearchParams );
                if( null != SearchResults.RegulationDbDataResults )
                {
                    if( SearchResults.RegulationDbDataResults.Length > 0 && SearchResults.RegulationDbDataResults.Length < SearchThreshold )
                    {
                        Collection<CswNbtNodeTypePropListOption> MatchingRegLists = new Collection<CswNbtNodeTypePropListOption>();

                        foreach( CswC3RegulationDbData RegDbRecord in SearchResults.RegulationDbDataResults )
                        {
                            MatchingRegLists.Add( new CswNbtNodeTypePropListOption( RegDbRecord.ListName, CswConvert.ToString( RegDbRecord.ListId ) ) );
                        }

                        // Set the list options
                        ListName.Options.Options = MatchingRegLists;
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
        public CswNbtNodePropList ListName { get { return _CswNbtNode.Properties[PropertyName.ListName]; } }
        public CswNbtNodePropText ListCode { get { return _CswNbtNode.Properties[PropertyName.ListCode]; } }

        #endregion

    }//CswNbtObjClassRegulatoryListListCode

}//namespace ChemSW.Nbt.ObjClasses
