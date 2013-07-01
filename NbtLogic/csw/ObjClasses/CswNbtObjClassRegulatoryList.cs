using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.ChemCatCentral;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassRegulatoryList : CswNbtObjClass
    {
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string AddCASNumbers = "Add CAS Numbers";
            public const string CASNosGrid = "CAS Numbers";
            public const string Name = "Name";
            public const string Exclusive = "Exclusive";
            public const string ListMode = "List Mode";
            public const string LOLIListCodes = "LOLI List Codes";
        }

        /// <summary>
        /// Potential List Modes
        /// </summary>
        public sealed class CswEnumRegulatoryListListModes
        {
            /// <summary>
            /// Manually Managed - User provides Cas Numbers
            /// </summary>
            public const string ManuallyManaged = "Manually Managed";
            /// <summary>
            /// LOLI Managed - List Codes are synced with LOLI
            /// </summary>
            public const string LOLIManaged = "LOLI Managed";

            public static CswCommaDelimitedString Options = new CswCommaDelimitedString { ManuallyManaged, LOLIManaged };
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassRegulatoryList( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );

        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassRegulatoryList
        /// </summary>
        public static implicit operator CswNbtObjClassRegulatoryList( CswNbtNode Node )
        {
            CswNbtObjClassRegulatoryList ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.RegulatoryListClass ) )
            {
                ret = (CswNbtObjClassRegulatoryList) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
        }

        public override void afterCreateNode()
        {
        }

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            // Set which properties are displayed
            switch( ListMode.Value )
            {
                case CswEnumRegulatoryListListModes.LOLIManaged:
                    CASNosGrid.setHidden( true, true );
                    AddCASNumbers.setHidden( true, true );
                    Exclusive.setHidden( true, true );
                    break;
                case CswEnumRegulatoryListListModes.ManuallyManaged:
                    LOLIListCodes.setHidden( true, true );
                    break;
            }

            // We don't want users to be able to edit this property after the node is created
            ListMode.setReadOnly( true, true );

            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }

        //beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            //_removeListFromMaterials();
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );
        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        protected override void afterPopulateProps()
        {
            AddCASNumbers.SetOnPropChange( _AddCASNumbers_OnChange );

            // If the LOLI Sync module is disabled, then we don't want to the user to see the 'LOLI Managed' option.
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.LOLISync ) )
            {
                CswCommaDelimitedString NewOptions = new CswCommaDelimitedString
                    {
                        CswEnumRegulatoryListListModes.ManuallyManaged
                    };
                ListMode.Options.Override( NewOptions );
            }

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

        public CswNbtNodePropMemo AddCASNumbers { get { return _CswNbtNode.Properties[PropertyName.AddCASNumbers]; } }
        private void _AddCASNumbers_OnChange( CswNbtNodeProp Prop )
        {
            if( false == string.IsNullOrEmpty( AddCASNumbers.Text ) )
            {
                CswNbtMetaDataObjectClass RegListCasNoOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListCasNoClass );
                if( null != RegListCasNoOC )
                {
                    CswNbtMetaDataNodeType RegListCasNoNT = RegListCasNoOC.FirstNodeType;
                    if( null != RegListCasNoNT )
                    {
                        //case 29610 - Foreach Cas Number in the CAS Number Input property, create a new RegulatoryListCASNo node. 

                        string NewCasNos = AddCASNumbers.Text;
                        NewCasNos = NewCasNos.Replace( "\r\n", "," ); // Turn all delimiters into commas
                        NewCasNos = NewCasNos.Replace( "\n", "," ); // Turn all delimiters into commas
                        NewCasNos = NewCasNos.Replace( " ", "" ); // Trim whitespace
                        CswCommaDelimitedString NewCasNosDelimited = new CswCommaDelimitedString();
                        NewCasNosDelimited.FromString( NewCasNos, true );

                        // But don't create dupes
                        Collection<string> existingCasnos = getCASNumbers();

                        foreach( string CAS in NewCasNosDelimited.Where( c => false == existingCasnos.Contains( c ) ) )
                        {
                            if( false == string.IsNullOrEmpty( CAS ) )
                            {
                                //string errormsg;
                                //CswNbtNodePropCASNo.Validate( CAS, out errormsg );

                                _CswNbtResources.Nodes.makeNodeFromNodeTypeId( RegListCasNoNT.NodeTypeId, delegate( CswNbtNode NewNode )
                                    {
                                        CswNbtObjClassRegulatoryListCasNo newCasNoNode = NewNode;
                                        newCasNoNode.CASNo.Text = CAS;
                                        //newCasNoNode.ErrorMessage.Text = errormsg;
                                        newCasNoNode.RegulatoryList.RelatedNodeId = this.NodeId;
                                    } );
                            }
                        }
                        AddCASNumbers.Text = string.Empty;
                    }
                }
            }
        } // _AddCASNumbers_OnChange()
        public CswNbtNodePropText Name { get { return _CswNbtNode.Properties[PropertyName.Name]; } }
        public CswNbtNodePropGrid CASNosGrid { get { return _CswNbtNode.Properties[PropertyName.CASNosGrid]; } }
        public CswNbtNodePropLogical Exclusive { get { return _CswNbtNode.Properties[PropertyName.Exclusive]; } }
        public CswNbtNodePropList ListMode { get { return _CswNbtNode.Properties[PropertyName.ListMode]; } }
        public CswNbtNodePropGrid LOLIListCodes { get { return _CswNbtNode.Properties[PropertyName.LOLIListCodes]; } }

        #endregion

        public Collection<string> getCASNumbers()
        {
            Collection<string> ret = new Collection<string>();
            CswNbtMetaDataObjectClass RegListCasNoOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListCasNoClass );
            CswNbtMetaDataObjectClassProp RegListCasNoCasNoOCP = RegListCasNoOC.getObjectClassProp( CswNbtObjClassRegulatoryListCasNo.PropertyName.CASNo );
            CswNbtMetaDataObjectClassProp RegListCasNoRegListOCP = RegListCasNoOC.getObjectClassProp( CswNbtObjClassRegulatoryListCasNo.PropertyName.RegulatoryList );

            CswNbtView View = new CswNbtView( _CswNbtResources );
            View.ViewName = "Reglist_getCASNumbers";
            CswNbtViewRelationship casnoRel = View.AddViewRelationship( RegListCasNoOC, false );
            View.AddViewProperty( casnoRel, RegListCasNoCasNoOCP );
            View.AddViewPropertyAndFilter( casnoRel,
                                           RegListCasNoRegListOCP,
                                           SubFieldName: CswEnumNbtSubFieldName.NodeID,
                                           FilterMode: CswEnumNbtFilterMode.Equals,
                                           Value: this.NodeId.PrimaryKey.ToString() );

            ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, RequireViewPermissions: false, IncludeSystemNodes: true, IncludeHiddenNodes: true );
            for( Int32 i = 0; i < Tree.getChildNodeCount(); i++ )
            {
                Tree.goToNthChild( i );

                CswNbtTreeNodeProp casnoTreeProp = Tree.getChildNodePropsOfNode().FirstOrDefault( p => p.ObjectClassPropName == RegListCasNoCasNoOCP.PropName );
                if( null != casnoTreeProp )
                {
                    ret.Add( casnoTreeProp[( (CswNbtFieldTypeRuleCASNo) RegListCasNoCasNoOCP.getFieldTypeRule() ).TextSubField.Column] );
                }

                Tree.goToParentNode();
            }
            return ret;
        } // getCASNumbers()


        /// <summary>
        /// Returns a collection of matching Regulatory List primary keys, based on the provided cas numbers
        /// </summary>
        public static Collection<CswPrimaryKey> findMatches( CswNbtResources CswNbtResources, Collection<string> CasNos )
        {
            Collection<CswPrimaryKey> ret = new Collection<CswPrimaryKey>();
            if( CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.RegulatoryLists ) )
            {
                CswNbtMetaDataObjectClass RegulatoryListOC = CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListClass );
                CswNbtMetaDataObjectClass RegListCasNoOC = CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListCasNoClass );
                if( null != RegulatoryListOC && null != RegListCasNoOC )
                {
                    CswNbtMetaDataObjectClassProp RegListExclusiveOCP = RegulatoryListOC.getObjectClassProp( CswNbtObjClassRegulatoryList.PropertyName.Exclusive );
                    CswNbtMetaDataObjectClassProp RegListCasNoCasNoOCP = RegListCasNoOC.getObjectClassProp( CswNbtObjClassRegulatoryListCasNo.PropertyName.CASNo );
                    Collection<CswPrimaryKey> ExclusiveMatches = new Collection<CswPrimaryKey>();

                    // find matches
                    if( CasNos.Count > 0 )
                    {
                        CswNbtView View = new CswNbtView( CswNbtResources );
                        View.ViewName = "Reglist_findMatches";
                        CswNbtViewRelationship casnoRel = View.AddViewRelationship( RegListCasNoOC, false );
                        CswNbtViewProperty casnoVP = View.AddViewProperty( casnoRel, RegListCasNoCasNoOCP );
                        foreach( string cas in CasNos )
                        {
                            View.AddViewPropertyFilter( casnoVP, Conjunction: CswEnumNbtFilterConjunction.Or, FilterMode: CswEnumNbtFilterMode.Equals, Value: cas );
                        }
                        CswNbtViewRelationship regListRel = View.AddViewRelationship( casnoRel, CswEnumNbtViewPropOwnerType.First, RegListCasNoOC.getObjectClassProp( CswNbtObjClassRegulatoryListCasNo.PropertyName.RegulatoryList ), false );
                        View.AddViewProperty( regListRel, RegListExclusiveOCP );

                        ICswNbtTree Tree = CswNbtResources.Trees.getTreeFromView( View, RequireViewPermissions: false, IncludeSystemNodes: true, IncludeHiddenNodes: true );
                        for( Int32 i = 0; i < Tree.getChildNodeCount(); i++ ) // RegListCasNo
                        {
                            Tree.goToNthChild( i );
                            for( Int32 j = 0; j < Tree.getChildNodeCount(); j++ ) // RegList
                            {
                                Tree.goToNthChild( j );

                                CswPrimaryKey thisRegListId = Tree.getNodeIdForCurrentPosition();

                                CswNbtTreeNodeProp exclusiveTreeProp = Tree.getChildNodePropsOfNode().FirstOrDefault( p => p.ObjectClassPropName == RegListExclusiveOCP.PropName );
                                if( null != exclusiveTreeProp )
                                {
                                    CswEnumTristate thisExclusive = CswConvert.ToTristate( exclusiveTreeProp[( (CswNbtFieldTypeRuleLogical) RegListExclusiveOCP.getFieldTypeRule() ).CheckedSubField.Column] );
                                    if( CswEnumTristate.True == thisExclusive )
                                    {
                                        ExclusiveMatches.Add( thisRegListId );
                                    }
                                    else
                                    {
                                        ret.Add( thisRegListId );
                                    }
                                }
                                Tree.goToParentNode();
                            } // for( Int32 j = 0; j < Tree.getChildNodeCount(); j++ ) // RegList
                            Tree.goToParentNode();
                        } // for( Int32 i = 0; i < Tree.getChildNodeCount(); i++ ) // RegListCasNo
                    } // if( CasNos.Count > 0 )

                    // find exclusive lists that didn't match
                    {
                        CswNbtView exclusiveView = new CswNbtView( CswNbtResources );
                        CswNbtViewRelationship regListRel = exclusiveView.AddViewRelationship( RegulatoryListOC, false );
                        regListRel.NodeIdsToFilterOut = ExclusiveMatches;
                        exclusiveView.AddViewPropertyAndFilter( regListRel, RegListExclusiveOCP, Value: CswEnumTristate.True.ToString() );

                        ICswNbtTree Tree = CswNbtResources.Trees.getTreeFromView( exclusiveView, RequireViewPermissions: false, IncludeSystemNodes: true, IncludeHiddenNodes: true );
                        for( Int32 i = 0; i < Tree.getChildNodeCount(); i++ )
                        {
                            Tree.goToNthChild( i );
                            ret.Add( Tree.getNodeIdForCurrentPosition() );
                            Tree.goToParentNode();
                        }
                    } // exclusive

                } // if( null != RegulatoryListOC && null != RegListCasNoOC )
            } // if( CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.RegulatoryLists ) )
            return ret;
        } // findMatches()

        /// <summary>
        /// LOLI SYNC
        /// </summary>
        public static Collection<CswPrimaryKey> findMatches_LOLI( CswNbtResources CswNbtResources, Collection<string> CasNos )
        {
            Collection<CswPrimaryKey> ret = new Collection<CswPrimaryKey>();

            // If RegLists and LOLISync enabled
            if( CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.RegulatoryLists ) && CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.LOLISync ) )
            {
                CswNbtMetaDataObjectClass RegulatoryListOC = CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListClass );
                CswNbtMetaDataObjectClass RegListListCodeOC = CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListListCodeClass );
                if( null != RegulatoryListOC && null != RegListListCodeOC )
                {
                    CswNbtMetaDataObjectClassProp RegListListCodeLOLIListCodeOCP = RegListListCodeOC.getObjectClassProp( CswNbtObjClassRegulatoryListListCode.PropertyName.LOLIListCode );
                    CswNbtMetaDataObjectClassProp RegListListCodeRegulatoryListOCP = RegListListCodeOC.getObjectClassProp( CswNbtObjClassRegulatoryListListCode.PropertyName.RegulatoryList );
                    CswNbtMetaDataObjectClassProp RegListListModeOCP = RegulatoryListOC.getObjectClassProp( PropertyName.ListMode );

                    // find matches by searching LOLI
                    if( CasNos.Count > 0 )
                    {
                        // get all loli managed regulatory lists

                        CswNbtView View1 = new CswNbtView( CswNbtResources );
                        View1.ViewName = "RegLists_LoliManaged";
                        CswNbtViewRelationship ParentRelationship = View1.AddViewRelationship( RegulatoryListOC, false );
                        View1.AddViewPropertyAndFilter( ParentViewRelationship: ParentRelationship,
                                                       MetaDataProp: RegListListModeOCP,
                                                       Value: CswEnumRegulatoryListListModes.LOLIManaged,
                                                       SubFieldName: CswEnumNbtSubFieldName.Value,
                                                       FilterMode: CswEnumNbtFilterMode.Equals );
                        CswNbtViewRelationship SecondaryRelationship = View1.AddViewRelationship( ParentRelationship, CswEnumNbtViewPropOwnerType.Second, RegListListCodeRegulatoryListOCP, false );
                        View1.AddViewProperty( SecondaryRelationship, RegListListCodeLOLIListCodeOCP );

                        // Dictionary that stores the List Codes for each Regulatory List
                        Dictionary<CswPrimaryKey, List<string>> RegListListCodes = new Dictionary<CswPrimaryKey, List<string>>();

                        // Get and iterate the Tree
                        ICswNbtTree Tree1 = CswNbtResources.Trees.getTreeFromView( View1, false, true, true );
                        for( Int32 i = 0; i < Tree1.getChildNodeCount(); i++ ) // Regulatory List Nodes
                        {
                            Tree1.goToNthChild( i );

                            List<string> CurrentListCodes = new List<string>();
                            CswPrimaryKey CurrentRegListPk = Tree1.getNodeIdForCurrentPosition();

                            for( int j = 0; j < Tree1.getChildNodeCount(); j++ ) // Regulatory List List Code Nodes
                            {
                                Tree1.goToNthChild( j );
                                CswNbtTreeNodeProp ListCodeTreeProp = null;
                                foreach( CswNbtTreeNodeProp currentTnp in Tree1.getChildNodePropsOfNode() )
                                {
                                    if( currentTnp.ObjectClassPropName == RegListListCodeLOLIListCodeOCP.PropName )
                                    {
                                        ListCodeTreeProp = currentTnp;
                                        break;
                                    }
                                }
                                if( null != ListCodeTreeProp )
                                {
                                    CurrentListCodes.Add( CswConvert.ToString( ListCodeTreeProp.Field1_Numeric ) );
                                }
                                Tree1.goToParentNode();
                            }

                            // Add to the dictionary
                            RegListListCodes.Add( CurrentRegListPk, CurrentListCodes );

                            Tree1.goToParentNode();
                        }

                        // Search the loli database

                        foreach( string CurrentCasNo in CasNos )
                        {
                            foreach( KeyValuePair<CswPrimaryKey, List<string>> Pair in RegListListCodes )
                            {
                                CswC3SearchParams CswC3SearchParams = new CswC3SearchParams();
                                CswNbtC3ClientManager CswNbtC3ClientManager = new CswNbtC3ClientManager( CswNbtResources, CswC3SearchParams );
                                ChemCatCentral.SearchClient C3SearchClient = CswNbtC3ClientManager.initializeC3Client();

                                string ListCodes = string.Join( ",", Pair.Value.ToArray() );

                                // Set LOLI Sync specific properties
                                CswC3SearchParams.Query = CurrentCasNo; // Query takes the Cas Number
                                CswC3SearchParams.ListCodes = ListCodes; // ListCodes should be a comma delimited string of all list codes

                                CswRetObjSearchResults SearchResults = C3SearchClient.getListCodesByCasNo( CswC3SearchParams );
                                if( null != SearchResults.LoliDataResults )
                                {
                                    if( SearchResults.LoliDataResults.Length > 0 ) // Does this mean it returned no list codes???
                                    {
                                        // If at least one list code was returned, add this regulatory list id to the list of matching reg lists
                                        ret.Add( Pair.Key );
                                    }
                                }

                            }//foreach( KeyValuePair<CswPrimaryKey, List<string>> Pair in RegListListCodes )

                        }//foreach( string CurrentCasNo in CasNos )

                    }//if( CasNos.Count > 0 )

                } // if( null != RegulatoryListOC && null != RegListCasNoOC )
            } // if( CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.RegulatoryLists ) )
            return ret;
        } // findMatches()

    }//CswNbtObjClassRegulatoryList

}//namespace ChemSW.Nbt.ObjClasses
