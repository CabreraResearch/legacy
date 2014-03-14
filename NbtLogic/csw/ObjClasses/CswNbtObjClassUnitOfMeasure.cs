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

        public CswNbtObjClassUnitOfMeasure( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

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

        protected override void beforeWriteNodeLogic( bool Creating )
        {
            if( false == IsTemp )
            {
                _validateConversionFactor();
            }
        }//beforeWriteNode()

        protected override void beforeDeleteNodeLogic()
        {
            if( Name.Text == BaseUnit.Text )
            {
                throw new CswDniException( CswEnumErrorType.Warning, Name.Text + " cannot be deleted because it's a Base Unit for " + NodeType.NodeTypeName, "User attempted to delete " + NodeType.NodeTypeName + "'s Base Unit" );
            }
            if( _isUsedForRegulatoryReporting( Name.Text ) )
            {
                throw new CswDniException( CswEnumErrorType.Warning, Name.Text + " cannot be deleted because it's used for regulatory reporting.", "User attempted to delete " + Name.Text );
            }
        }//beforeDeleteNode()    

        protected override void afterPopulateProps()
        {
            ConversionFactor.SetOnPropChange( OnConversionFactorPropChange );
            Aliases.SetOnPropChange( onAliasesPropChange );
            Name.SetOnPropChange( OnNamePropChange );
        }//afterPopulateProps()

        #endregion

        private bool _isUsedForRegulatoryReporting( string UnitName )
        {
            return UnitName == "lb" || UnitName == "gal" || UnitName == "cu.ft." || UnitName == "kg" || UnitName == "Liters";
        }

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
                        CswCommaDelimitedString UoMNodeCommaDelimitedAliases = new CswCommaDelimitedString();
                        UoMNodeCommaDelimitedAliases.FromString( TreeNodeProp.Gestalt, false, true );

                        foreach( string Alias1 in AliasesAsDelimitedString )
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
                              CurrentUoMNode.Node.NodeLink + " Unit of Measurement.";
            }
            else if( Property.Equals( "Alias" ) )
            {
                ExotericMessage = "The " + PropertyName.Aliases +
                                     " property value must be unique: '" + AliasValue + "' already exists as an alias on the " +
                              CurrentUoMNode.Node.NodeLink + " Unit of Measurement.";
            }

            throw ( new CswDniException( CswEnumErrorType.Warning, ExotericMessage, EsotericMessage ) );
        }

        #region Object class specific properties

        public CswNbtNodePropText Name { get { return ( _CswNbtNode.Properties[PropertyName.Name] ); } }
        public CswNbtNodePropText BaseUnit { get { return ( _CswNbtNode.Properties[PropertyName.BaseUnit] ); } }
        private void OnNamePropChange( CswNbtNodeProp Prop, bool Creating )
        {
            string OrigName = Name.GetOriginalPropRowValue();
            if( OrigName != Name.Text )
            {
                if( OrigName == BaseUnit.Text )
                {
                    throw new CswDniException( CswEnumErrorType.Warning, Name.Text + " cannot be renamed because it's a Base Unit for " + NodeType.NodeTypeName, "User attempted to rename " + NodeType.NodeTypeName + "'s Base Unit" );
                }
                if( _isUsedForRegulatoryReporting( OrigName ) )
                {
                    throw new CswDniException( CswEnumErrorType.Warning, Name.Text + " cannot be renamed because it's used for regulatory reporting.", "User attempted to rename " + Name.Text );
                }
            }
        }
        public CswNbtNodePropScientific ConversionFactor { get { return ( _CswNbtNode.Properties[PropertyName.ConversionFactor] ); } }
        private void OnConversionFactorPropChange( CswNbtNodeProp Prop, bool Creating )
        {
            _validateConversionFactor();
        }
        public CswNbtNodePropLogical Fractional { get { return ( _CswNbtNode.Properties[PropertyName.Fractional] ); } }
        public CswNbtNodePropList UnitType { get { return ( _CswNbtNode.Properties[PropertyName.UnitType] ); } }
        public CswNbtNodePropStatic UnitConversion { get { return ( _CswNbtNode.Properties[PropertyName.UnitConversion] ); } }
        public CswNbtNodePropMemo Aliases { get { return ( _CswNbtNode.Properties[PropertyName.Aliases] ); } }
        private void onAliasesPropChange( CswNbtNodeProp Prop, bool Creating )
        {
            // Remove duplicates
            CswCommaDelimitedString AliasesWithoutDupes = new CswCommaDelimitedString();
            foreach( string Alias in AliasesAsDelimitedString.Where( Alias => false == AliasesWithoutDupes.Contains( Alias ) ) )
            {
                AliasesWithoutDupes.Add( Alias );
            }
            Aliases.Text = CswConvert.ToString( AliasesWithoutDupes );

            _validateAliasUniqueness();
        }

        public CswCommaDelimitedString AliasesAsDelimitedString
        {
            get
            {
                CswCommaDelimitedString ret = new CswCommaDelimitedString();
                ret.FromString( Aliases.Text, false, true );
                return ret;
            }
        }

        #endregion

    }//CswNbtObjClassUnitOfMeasure

}//namespace ChemSW.Nbt.ObjClasses
