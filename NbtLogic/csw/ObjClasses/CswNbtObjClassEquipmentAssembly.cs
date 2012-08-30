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
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentAssemblyClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassEquipmentAssembly
        /// </summary>
        public static implicit operator CswNbtObjClassEquipmentAssembly( CswNbtNode Node )
        {
            CswNbtObjClassEquipmentAssembly ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentAssemblyClass ) )
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
            //CswNbtMetaDataObjectClass AssemblyObjectClass = _CswNbtResources.MetaData.getObjectClass(CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentAssemblyClass);
            //CswNbtMetaDataObjectClass EquipmentObjectClass = _CswNbtResources.MetaData.getObjectClass(CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass);

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

            //string EquipmentAssemblyObjectClassPropName = "Assembly";

            //// For each equipment related to this assembly, update matching properties

            //CswNbtMetaDataObjectClassProp EquipmentAssemblyObjectClassProp = EquipmentObjectClass.getObjectClassProp(EquipmentAssemblyObjectClassPropName);

            //// Make a view of equipment
            //CswNbtView View = new CswNbtView( _CswNbtResources );
            //View.ViewName = "_CswNbtNodeWriteEquipmentAssembly.handleAfterWriteNode()";

            //CswNbtViewRelationship AssemblyRelationship = View.MakeEmptyViewRelationship();
            //AssemblyRelationship.SecondType = RelatedIdType.ObjectClassId;
            //AssemblyRelationship.SecondId = AssemblyObjectClass.ObjectClassId;
            //AssemblyRelationship.SecondName = AssemblyObjectClass.ObjectClass.ToString();
            //View.Root.addChildRelationship( AssemblyRelationship );

            //CswNbtViewRelationship EquipmentRelationship = View.MakeEmptyViewRelationship();
            //EquipmentRelationship.PropId = EquipmentAssemblyObjectClassProp.PropId;
            //EquipmentRelationship.PropName = EquipmentAssemblyObjectClassPropName;
            //EquipmentRelationship.PropType = PropIdType.ObjectClassPropId;
            //EquipmentRelationship.PropOwner = PropOwnerType.Second;
            //EquipmentRelationship.FirstType = RelatedIdType.ObjectClassId;
            //EquipmentRelationship.FirstId = AssemblyObjectClass.ObjectClassId;
            //EquipmentRelationship.FirstName = AssemblyObjectClass.ObjectClass.ToString();
            //EquipmentRelationship.SecondType = RelatedIdType.ObjectClassId;
            //EquipmentRelationship.SecondId = EquipmentObjectClass.ObjectClassId;
            //EquipmentRelationship.SecondName = EquipmentObjectClass.ObjectClass.ToString();
            //AssemblyRelationship.addChildRelationship( EquipmentRelationship );

            //ICswNbtTree EquipTree = _CswNbtResources.Trees.getTreeFromView( View, _CswNbtNode.NodeId, false );

            //EquipTree.goToRoot();
            //if( EquipTree.getChildNodeCount() > 0 )  // should always be the case
            //{
            //    EquipTree.goToNthChild( 0 );
            //    if( EquipTree.getChildNodeCount() > 0 )   // might not always be the case
            //    {
            //        for( int i = 0; i < EquipTree.getChildNodeCount(); i++ )
            //        {
            //            EquipTree.goToNthChild( i );

            //            CswNbtNode EquipNode = EquipTree.getNodeForCurrentPosition();

            //            // Synchronize prop values
            //            foreach( CswNbtNodePropWrapper AssemblyProp in _CswNbtNode.Properties )
            //            {
            //                foreach( CswNbtNodePropWrapper EquipProp in EquipNode.Properties )
            //                {
            //                    if( AssemblyProp.PropName == EquipProp.PropName && AssemblyProp.FieldType == EquipProp.FieldType )
            //                    {
            //                        EquipProp.copy( AssemblyProp );
            //                        EquipProp.ReadOnly = true;
            //                    }
            //                }//iterate equip props

            //            }//iterate assemlby props

            //            _CswNbtResources.Nodes.save( EquipNode.NodeKey );

            //            EquipTree.goToParentNode();
            //        }//iterate equip nodes
            //    }
            //}

        }// _updateEquipment()

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
        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Type
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Type].AsRelationship );
            }
        }

        public CswNbtNodePropLogicalSet AssemblyParts
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.AssemblyParts].AsLogicalSet );
            }
        }

        #endregion
    }//CswNbtObjClassEquipmentAssembly

}//namespace ChemSW.Nbt.ObjClasses
