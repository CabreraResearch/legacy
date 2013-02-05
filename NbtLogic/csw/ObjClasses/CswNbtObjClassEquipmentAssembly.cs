using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassEquipmentAssembly : CswNbtObjClass
    {
        public sealed class PropertyName
        {
            public const string Type = "Assembly Type";
            public const string AssemblyParts = "Assembly Parts";
        }

        public static string PartsXValueName = "Uses";

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassEquipmentAssembly( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.EquipmentAssemblyClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassEquipmentAssembly
        /// </summary>
        public static implicit operator CswNbtObjClassEquipmentAssembly( CswNbtNode Node )
        {
            CswNbtObjClassEquipmentAssembly ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.EquipmentAssemblyClass ) )
            {
                ret = (CswNbtObjClassEquipmentAssembly) Node.ObjClass;
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
            _updateEquipment();
        }//afterWriteNode()

        private void _updateEquipment()
        {
            // For each equipment related to this assembly, mark matching properties as pending update
            if( NodeModificationState.Modified == _CswNbtNode.ModificationState )
            {
                CswStaticSelect PropRefsSelect = _CswNbtResources.makeCswStaticSelect( "afterWriteNode_select", "getMatchingEquipPropsForAssembly" );
                CswStaticParam StaticParam = new CswStaticParam( "getassemblynodeid", _CswNbtNode.NodeId.PrimaryKey );
                PropRefsSelect.S4Parameters.Add( "getassemblynodeid", StaticParam );
                DataTable PropRefsTable = PropRefsSelect.getTable();

                // Update the nodes.pendingupdate directly, to avoid having to fetch all the node info for every related node 
                string PkString = String.Empty;
                foreach( DataRow PropRefsRow in PropRefsTable.Rows )
                {
                    if( PkString != String.Empty ) PkString += ",";
                    PkString += PropRefsRow["nodeid"].ToString();
                }
                if( PkString != String.Empty )
                {
                    CswTableUpdate NodesUpdate = _CswNbtResources.makeCswTableUpdate( "afterWriteNode_update", "nodes" );
                    DataTable NodesTable = NodesUpdate.getTable( "where nodeid in (" + PkString + ")" );
                    foreach( DataRow NodesRow in NodesTable.Rows )
                    {
                        NodesRow["pendingupdate"] = "1";
                    }
                    NodesUpdate.update( NodesTable );
                }
            }
        }

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
                    AssemblyParts.YValues = PartsString;
                }
            }
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }

        public override CswNbtNode CopyNode()
        {
            // Copy this Assembly
            CswNbtNode CopiedAssemblyNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
            CopiedAssemblyNode.copyPropertyValues( Node );
            CopiedAssemblyNode.postChanges( true, true );

            // Copy all Equipment
            CswNbtMetaDataObjectClass EquipmentObjectClass = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.EquipmentClass );
            CswNbtView EquipmentView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship EquipmentRelationship = EquipmentView.AddViewRelationship( EquipmentObjectClass, false );
            CswNbtViewProperty AssemblyProperty = EquipmentView.AddViewProperty( EquipmentRelationship, EquipmentObjectClass.getObjectClassProp( CswNbtObjClassEquipment.PropertyName.Assembly ) );
            CswNbtViewPropertyFilter AssemblyIsOriginalFilter = EquipmentView.AddViewPropertyFilter( 
                AssemblyProperty, 
                CswNbtSubField.SubFieldName.NodeID, 
                CswNbtPropFilterSql.PropertyFilterMode.Equals, 
                NodeId.PrimaryKey.ToString());

            ICswNbtTree EquipmentTree = _CswNbtResources.Trees.getTreeFromView( _CswNbtResources.CurrentNbtUser, EquipmentView, true, false, false );
            EquipmentTree.goToRoot();
            Int32 c = 0;
            while( c < EquipmentTree.getChildNodeCount() )
            {
                EquipmentTree.goToNthChild( c );
                CswNbtObjClassEquipment OriginalEquipmentNode = EquipmentTree.getNodeForCurrentPosition();
                CswNbtObjClassEquipment CopiedEquipmentNode = OriginalEquipmentNode.CopyNode();
                CopiedEquipmentNode.Assembly.RelatedNodeId = CopiedAssemblyNode.NodeId;
                CopiedEquipmentNode.postChanges( true );
                EquipmentTree.goToParentNode();
                c++;
            }

            return CopiedAssemblyNode;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Type
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Type] );
            }
        }

        public CswNbtNodePropLogicalSet AssemblyParts
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.AssemblyParts] );
            }
        }

        #endregion
    }//CswNbtObjClassEquipmentAssembly

}//namespace ChemSW.Nbt.ObjClasses
