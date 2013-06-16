using System;
using System.Collections.ObjectModel;
using System.Linq;
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
                        //case 29610 - Foreach Cas Number in the CAS Number Input property, create a new RegulatoryListCASNo node. 

                        string NewCasNos = AddCASNumbers.Text;
                        NewCasNos = NewCasNos.Replace( "\r\n", "," ); // Turn all delimiters into commas
                        NewCasNos = NewCasNos.Replace( "\n", "," ); // Turn all delimiters into commas
                        NewCasNos = NewCasNos.Replace( " ", "" ); // Trim whitespace
                        CswCommaDelimitedString NewCasNosDelimited = new CswCommaDelimitedString();
                        NewCasNosDelimited.FromString( NewCasNos );

                        // But don't create dupes
                        Collection<string> existingCasnos = getCASNumbers();

                        foreach( string CAS in NewCasNosDelimited.Where( c => false == existingCasnos.Contains( c ) ) )
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

                                CswNbtTreeNodeProp exclusiveTreeProp = Tree.getChildNodePropsOfNode().FirstOrDefault( p => p.ObjectClassPropName == RegListCasNoCasNoOCP.PropName );
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

    }//CswNbtObjClassRegulatoryList

}//namespace ChemSW.Nbt.ObjClasses
