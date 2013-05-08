using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassUnitOfMeasure : CswNbtObjClass
    {
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string Name = "Name";
            public const string BaseUnit = "Base Unit";
            public const string ConversionFactor = "Conversion Factor";
            public const string Fractional = "Fractional";
            public const string UnitType = "Unit Type";
            public const string UnitConversion = "Unit Conversion";
            public const string Aliases = "Aliases";
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassUnitOfMeasure( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.UnitOfMeasureClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassUnitOfMeasure
        /// </summary>
        public static implicit operator CswNbtObjClassUnitOfMeasure( CswNbtNode Node )
        {
            CswNbtObjClassUnitOfMeasure ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.UnitOfMeasureClass ) )
            {
                ret = (CswNbtObjClassUnitOfMeasure) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            if( false == IsTemp )
            {
                _validateConversionFactor();
            }
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
            ConversionFactor.SetOnPropChange( OnConversionFactorPropChange );
            Aliases.SetOnPropChange( onAliasesPropChange );
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

        private void _validateConversionFactor()
        {
            if( UnitType.Value != CswEnumNbtUnitTypes.Each.ToString() && false == CswTools.IsDouble( ConversionFactor.RealValue ) )
            {
                throw new CswDniException
                (
                    CswEnumErrorType.Warning,
                    "Units of type " + UnitType.Value + " must have a Conversion Factor.",
                    "Unit of Measure cannot be used for unit conversion."
                );
            }
        }

        private void _validateAliasUniqueness()
        {
            CswCommaDelimitedString CommaDelimitedAliases = new CswCommaDelimitedString();
            string AliasesWithoutSpaces = Aliases.Text.Replace( " ", "" );
            CommaDelimitedAliases.FromString( AliasesWithoutSpaces );

            // Create a view of all UoM nodes and their Aliases property
            CswNbtView UoMView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship ParentRelationship = UoMView.AddViewRelationship( ObjectClass, true );

            CswNbtMetaDataObjectClassProp AliasesOCP = ObjectClass.getObjectClassProp( PropertyName.Aliases );
            UoMView.AddViewProperty( ParentRelationship, AliasesOCP );

            ICswNbtTree UoMNodesTree = _CswNbtResources.Trees.getTreeFromView( UoMView, false, false, false );
            for( int i = 0; i < UoMNodesTree.getChildNodeCount(); i++ )
            {
                UoMNodesTree.goToNthChild( i );
                string CurrentNodeName = UoMNodesTree.getNodeNameForCurrentPosition();
                CswPrimaryKey CurrentNodeId = UoMNodesTree.getNodeIdForCurrentPosition();
                if( CurrentNodeId != NodeId )
                {
                    foreach( CswNbtTreeNodeProp TreeNodeProp in UoMNodesTree.getChildNodePropsOfNode() )
                    {
                        CswNbtMetaDataNodeTypeProp UoMNTP = _CswNbtResources.MetaData.getNodeTypeProp( TreeNodeProp.NodeTypePropId );

                        CswCommaDelimitedString UoMNodeCommaDelimitedAliases = new CswCommaDelimitedString();
                        string UoMNodeAliasesWithoutSpaces = TreeNodeProp.Gestalt.Replace( " ", "" );
                        UoMNodeCommaDelimitedAliases.FromString( UoMNodeAliasesWithoutSpaces );

                        foreach( string Alias1 in CommaDelimitedAliases )
                        {
                            // First check to see whether Alias1 matches the CurrentNodeName
                            if( Alias1.Equals( CurrentNodeName ) )
                            {
                                _aliasUniquenessViolated( UoMNodesTree, "Name", Alias1 );
                            }

                            if( UoMNodeCommaDelimitedAliases.Any( Alias2 => Alias1.Equals( Alias2 ) ) )
                            {
                                _aliasUniquenessViolated( UoMNodesTree, "Alias", Alias1 );
                            }
                        }
                    }
                }
                UoMNodesTree.goToParentNode();
            }
        }

        private void _aliasUniquenessViolated( ICswNbtTree UoMNodesTree, string Property, string AliasValue )
        {
            CswNbtObjClassUnitOfMeasure CurrentUoMNode = UoMNodesTree.getNodeForCurrentPosition();

            string EsotericMessage = "Unique constraint violation: The proposed value '" + AliasValue + "' ";
            EsotericMessage += "of property '" + PropertyName.Aliases + "' ";
            EsotericMessage += "for nodeid (" + this.NodeId + ") ";
            EsotericMessage += "of nodetype '" + this.NodeType + "' ";
            EsotericMessage += "is invalid because the same value is already set as the " + Property + " of node '" +
                               CurrentUoMNode.NodeName + "' (" + CurrentUoMNode.NodeId + ").";
            string ExotericMessage = string.Empty;

            if( Property.Equals( "Name" ) )
            {
                ExotericMessage = "The " + PropertyName.Aliases +
                                     " property value must be unique: '" + AliasValue + "' is set as the Name on the " +
                              CurrentUoMNode.Node.NodeLink + " node.";
            }
            else if( Property.Equals( "Alias" ) )
            {
                ExotericMessage = "The " + PropertyName.Aliases +
                                     " property value must be unique: '" + AliasValue + "' already exists as an alias on the " +
                              CurrentUoMNode.Node.NodeLink + " node.";
            }

            throw ( new CswDniException( CswEnumErrorType.Warning, ExotericMessage, EsotericMessage ) );
        }

        #region Object class specific properties

        public CswNbtNodePropText Name { get { return ( _CswNbtNode.Properties[PropertyName.Name] ); } }
        public CswNbtNodePropText BaseUnit { get { return ( _CswNbtNode.Properties[PropertyName.BaseUnit] ); } }
        public CswNbtNodePropScientific ConversionFactor { get { return ( _CswNbtNode.Properties[PropertyName.ConversionFactor] ); } }
        private void OnConversionFactorPropChange( CswNbtNodeProp Prop )
        {
            _validateConversionFactor();
        }
        public CswNbtNodePropLogical Fractional { get { return ( _CswNbtNode.Properties[PropertyName.Fractional] ); } }
        public CswNbtNodePropList UnitType { get { return ( _CswNbtNode.Properties[PropertyName.UnitType] ); } }
        public CswNbtNodePropStatic UnitConversion { get { return ( _CswNbtNode.Properties[PropertyName.UnitConversion] ); } }
        public CswNbtNodePropMemo Aliases { get { return ( _CswNbtNode.Properties[PropertyName.Aliases] ); } }
        private void onAliasesPropChange( CswNbtNodeProp Prop )
        {
            _validateAliasUniqueness();
        }

        #endregion

    }//CswNbtObjClassUnitOfMeasure

}//namespace ChemSW.Nbt.ObjClasses
