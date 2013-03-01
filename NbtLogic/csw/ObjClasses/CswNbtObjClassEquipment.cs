using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassEquipment : CswNbtObjClass
    {
        public sealed class PropertyName
        {
            public const string Assembly = "Assembly";
            public const string Type = "Type";
            public const string Parts = "Parts";
            public const string Status = "Status";
        }

        public static string PartsXValueName { get { return "Uses"; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassEquipment( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.EquipmentClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassEquipment
        /// </summary>
        public static implicit operator CswNbtObjClassEquipment( CswNbtNode Node )
        {
            CswNbtObjClassEquipment ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.EquipmentClass ) )
            {
                ret = (CswNbtObjClassEquipment) Node.ObjClass;
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
            if( Type.RelatedNodeId != null )
            {
                CswNbtNode TypeNode = _CswNbtResources.Nodes[Type.RelatedNodeId];
                if( TypeNode != null )
                {
                    CswNbtObjClassEquipmentType TypeNodeAsType = (CswNbtObjClassEquipmentType) TypeNode;
                    CswDelimitedString PartsString = new CswDelimitedString( '\n' );
                    PartsString.FromString( TypeNodeAsType.Parts.Text.Replace( "\r", "" ) );
                    this.Parts.YValues = PartsString;
                }
            }
            SyncEquipmentToAssembly();
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            // BZ 10454
            // Filter out Retired Equipment by default
            CswNbtMetaDataObjectClassProp StatusOCP = this.ObjectClass.getObjectClassProp( PropertyName.Status );
            CswNbtViewProperty StatusViewProp = ParentRelationship.View.AddViewProperty( ParentRelationship, StatusOCP );
            CswNbtViewPropertyFilter StatusViewPropFilter = ParentRelationship.View.AddViewPropertyFilter( StatusViewProp,
                                                                                                           StatusOCP.getFieldTypeRule().SubFields.Default.Name,
                                                                                                           CswNbtPropFilterSql.PropertyFilterMode.NotEquals,
                                                                                                           "Retired", //StatusOptionToDisplayString( StatusOption.Retired ),
                                                                                                           false );

            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }

        public override CswNbtNode CopyNode()
        {
            CswNbtNode CopiedEquipmentNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
            CopiedEquipmentNode.copyPropertyValues( Node );
            CopiedEquipmentNode.postChanges( true, true );

            // Copy all Generators
            CswNbtMetaDataObjectClass GeneratorObjectClass = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.GeneratorClass );
            CswNbtView GeneratorView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship GeneratorRelationship = GeneratorView.AddViewRelationship( GeneratorObjectClass, false );
            CswNbtViewProperty OwnerProperty = GeneratorView.AddViewProperty( GeneratorRelationship, GeneratorObjectClass.getObjectClassProp( CswNbtObjClassGenerator.PropertyName.Owner ) );
            CswNbtViewPropertyFilter OwnerIsEquipmentFilter = GeneratorView.AddViewPropertyFilter( 
                OwnerProperty, 
                CswNbtSubField.SubFieldName.NodeID, 
                CswNbtPropFilterSql.PropertyFilterMode.Equals, 
                NodeId.PrimaryKey.ToString());

            ICswNbtTree GeneratorTree = _CswNbtResources.Trees.getTreeFromView( _CswNbtResources.CurrentNbtUser, GeneratorView, true, false, false );
            GeneratorTree.goToRoot();
            Int32 c = 0;
            while( c < GeneratorTree.getChildNodeCount() )
            {
                GeneratorTree.goToNthChild( c );
                CswNbtNode OriginalGeneratorNode = GeneratorTree.getNodeForCurrentPosition();
                CswNbtNode CopiedGeneratorNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( OriginalGeneratorNode.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
                CopiedGeneratorNode.copyPropertyValues( OriginalGeneratorNode );
                ( (CswNbtObjClassGenerator) CopiedGeneratorNode ).Owner.RelatedNodeId = CopiedEquipmentNode.NodeId;
                CopiedGeneratorNode.postChanges( true, true );
                GeneratorTree.goToParentNode();
                c++;
            }

            return CopiedEquipmentNode;
        }

        #endregion

        #region Object class specific properties


        public CswNbtNodePropRelationship Assembly { get { return ( _CswNbtNode.Properties[PropertyName.Assembly] ); } }
        public CswNbtNodePropRelationship Type { get { return ( _CswNbtNode.Properties[PropertyName.Type] ); } }
        public CswNbtNodePropLogicalSet Parts { get { return ( _CswNbtNode.Properties[PropertyName.Parts] ); } }
        public CswNbtNodePropList Status { get { return ( _CswNbtNode.Properties[PropertyName.Status] ); } }

        #endregion


        public void SyncEquipmentToAssembly()
        {
            // for all equipment properties that match properties on the assembly
            bool FoundAssemblyNode = false;
            if( this.Assembly.RelatedNodeId != null )
            {
                CswNbtNode AssemblyNode = _CswNbtResources.Nodes.GetNode( this.Assembly.RelatedNodeId );
                if( AssemblyNode != null )
                {
                    FoundAssemblyNode = true;
                    foreach( CswNbtNodePropWrapper EquipProp in this.Node.Properties )
                    {
                        bool FoundMatch = false;
                        foreach( CswNbtNodePropWrapper AssemblyProp in AssemblyNode.Properties )
                        {
                            if( EquipProp.getFieldTypeValue() != CswNbtMetaDataFieldType.NbtFieldType.Grid ) // case 27270
                            {
                                if( EquipProp.PropName.ToLower() == AssemblyProp.PropName.ToLower() && EquipProp.getFieldType() == AssemblyProp.getFieldType() )
                                {
                                    // Found a match -- copy the value and set readonly
                                    EquipProp.copy( AssemblyProp );
                                    EquipProp.setReadOnly( value: true, SaveToDb: true );
                                    FoundMatch = true;
                                    // case 21809
                                    EquipProp.HelpText = EquipProp.PropName + " is set on the Assembly, and must be modified there.";
                                }
                            }
                        }
                        if( !FoundMatch )
                        {
                            // if other things set these properties to readonly, this might be an issue.
                            // but it must be conditional - see BZ 7084
                            if( EquipProp.ReadOnly )
                            {
                                EquipProp.setReadOnly( value: false, SaveToDb: true );
                            }
                        }
                    }
                }
            } //  if( this.Assembly.RelatedNodeId != null )

            if( !FoundAssemblyNode )
            {
                foreach( CswNbtNodePropWrapper EquipProp in this.Node.Properties )
                {
                    // if other things set these properties to readonly, this might be an issue.
                    // but it must be conditional - see BZ 7084
                    if( EquipProp.ReadOnly )
                    {
                        EquipProp.setReadOnly( value: false, SaveToDb: true );
                    }
                }
            }
        } // SynchEquipmentToAssembly()
    }//CswNbtObjClassEquipment

}//namespace ChemSW.Nbt.ObjClasses
