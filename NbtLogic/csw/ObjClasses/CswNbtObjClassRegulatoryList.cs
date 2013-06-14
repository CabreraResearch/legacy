using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using ChemSW.Core;
using ChemSW.Nbt.Batch;
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

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            //if( CASNumbers.WasModified || Name.WasModified )
            //{
            //    //Case 28838 - remove newline char from CASNos
            //    CASNumbers.Text = CASNumbers.Text.Replace( "\n", "" ).Replace( "\r", "" );
            //    CASNumbers.Text = Regex.Replace( CASNumbers.Text, @"\s+", "" );

            //    //remove this list from all material nodes
            //    _removeListFromMaterials();

            //    //start the batch operation to update
            //    CswNbtBatchOpUpdateRegulatoryLists BatchOp = new CswNbtBatchOpUpdateRegulatoryLists( _CswNbtResources );
            //    CswCommaDelimitedString CASNosAsCommaString = new CswCommaDelimitedString();
            //    CASNosAsCommaString.FromString( CASNumbers.Text );
            //    BatchOp.makeBatchOp( Name.Text, CASNosAsCommaString );
            //}


            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

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
                        //case 29610
                        // Foreach Cas Number in the CAS Number Input property, create a new RegulatoryListCASNo node. 

                        string NewCasNos = AddCASNumbers.Text;
                        NewCasNos = NewCasNos.Replace( "\r\n", "," ); // Turn all delimiters into commas
                        NewCasNos = NewCasNos.Replace( "\n", "," ); // Turn all delimiters into commas
                        NewCasNos = NewCasNos.Replace( " ", "" ); // Trim whitespace
                        CswCommaDelimitedString NewCasNosDelimited = new CswCommaDelimitedString();
                        NewCasNosDelimited.FromString( NewCasNos );
                        foreach( string CAS in NewCasNosDelimited )
                        {
                            if( false == string.IsNullOrEmpty( CAS ) )
                            {
                                //string errormsg;
                                //CswNbtNodePropCASNo.Validate( CAS, out errormsg );

                                CswNbtObjClassRegulatoryListCasNo newCasNoNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( RegListCasNoNT.NodeTypeId, CswEnumNbtMakeNodeOperation.WriteNode );
                                newCasNoNode.CASNo.Text = CAS;
                                //newCasNoNode.ErrorMessage.Text = errormsg;
                                newCasNoNode.RegulatoryList.RelatedNodeId = this.NodeId;
                                newCasNoNode.postChanges( false );
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

        #endregion

        #region private helper functions
        //private void _removeListFromMaterials()
        //{
        //    string OriginalName = Name.GetOriginalPropRowValue();
        //    if( false == String.IsNullOrEmpty( OriginalName ) ) //if the original name is blank, it's a new node no materials have this on their reg lists prop
        //    {
        //        CswNbtView materialsWithThisList = new CswNbtView( _CswNbtResources );
        //        CswNbtMetaDataObjectClass chemicalOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
        //        CswNbtMetaDataObjectClassProp regListsOCP = chemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.RegulatoryLists );
        //        CswNbtViewRelationship parent = materialsWithThisList.AddViewRelationship( chemicalOC, false );
        //        materialsWithThisList.AddViewPropertyAndFilter( parent, regListsOCP, Value : OriginalName, FilterMode : CswEnumNbtFilterMode.Contains );

        //        ICswNbtTree materialsWithListTree = _CswNbtResources.Trees.getTreeFromView( materialsWithThisList, false, false, false );
        //        int nodeCount = materialsWithListTree.getChildNodeCount();
        //        for( int i = 0; i < nodeCount; i++ )
        //        {
        //            materialsWithListTree.goToNthChild( i );
        //            CswNbtObjClassChemical nodeAsMaterial = (CswNbtObjClassChemical) materialsWithListTree.getNodeForCurrentPosition();
        //            CswCommaDelimitedString regLists = new CswCommaDelimitedString();
        //            regLists.FromString( nodeAsMaterial.RegulatoryLists.StaticText );
        //            regLists.Remove( OriginalName );
        //            nodeAsMaterial.RegulatoryLists.StaticText = regLists.ToString();
        //            nodeAsMaterial.postChanges( false );
        //            materialsWithListTree.goToParentNode();
        //        }
        //    }
        //}
        #endregion

        /// <summary>
        /// Returns a collection of matching Regulatory List primary keys (and the CAS number it matched on), based on the provided cas numbers
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
                    Collection<CswPrimaryKey> ExclusiveMatches = new Collection<CswPrimaryKey>();

                    // find matches
                    if( CasNos.Count > 0 )
                    {
                        CswNbtView View = new CswNbtView( CswNbtResources );
                        CswNbtViewRelationship casnoRel = View.AddViewRelationship( RegListCasNoOC, false );
                        CswNbtViewProperty casnoVP = View.AddViewProperty( casnoRel, RegListCasNoOC.getObjectClassProp( CswNbtObjClassRegulatoryListCasNo.PropertyName.CASNo ) );
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

                                CswNbtTreeNodeProp exclusiveTreeProp = Tree.getChildNodePropsOfNode()[0];
                                CswEnumTristate thisExclusive = CswConvert.ToTristate( exclusiveTreeProp[( (CswNbtFieldTypeRuleLogical) RegListExclusiveOCP.getFieldTypeRule() ).CheckedSubField.Column] );
                                if( CswEnumTristate.True == thisExclusive )
                                {
                                    ExclusiveMatches.Add( thisRegListId );
                                }
                                else
                                {
                                    ret.Add( thisRegListId );
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

    }//CswNbtObjClassRegulatoryList

}//namespace ChemSW.Nbt.ObjClasses
