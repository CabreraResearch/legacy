using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Text;
using System.Data;
using ChemSW.Nbt.PropTypes;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;

using ChemSW.TblDn;
using ChemSW.Core;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassEquipmentAssembly : CswNbtObjClass
    {
        public static string TypePropertyName { get { return "Assembly Type"; } }
        public static string PartsPropertyName { get { return "Parts"; } }
        public static string PartsXValueName { get { return "Uses"; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassEquipmentAssembly( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public CswNbtObjClassEquipmentAssembly( CswNbtResources CswNbtResources )
            : base( CswNbtResources )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentAssemblyClass ); }
        }

        #region Inherited Events
        public override void beforeCreateNode()
        {
            _CswNbtObjClassDefault.beforeCreateNode();
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode()
        {
            _CswNbtObjClassDefault.beforeWriteNode();
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
                PropRefsSelect.S4Parameters.Add( "getassemblynodeid", _CswNbtNode.NodeId.PrimaryKey );
                DataTable PropRefsTable = PropRefsSelect.getTable();

                // Update the nodes.pendingupdate directly, to avoid having to fetch all the node info for every related node 
                string PkString = string.Empty;
                foreach( DataRow PropRefsRow in PropRefsTable.Rows )
                {
                    if( PkString != string.Empty ) PkString += ",";
                    PkString += PropRefsRow["nodeid"].ToString();
                }
                if( PkString != string.Empty )
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
            //AssemblyRelationship.SecondType = CswNbtViewRelationship.RelatedIdType.ObjectClassId;
            //AssemblyRelationship.SecondId = AssemblyObjectClass.ObjectClassId;
            //AssemblyRelationship.SecondName = AssemblyObjectClass.ObjectClass.ToString();
            //View.Root.addChildRelationship( AssemblyRelationship );

            //CswNbtViewRelationship EquipmentRelationship = View.MakeEmptyViewRelationship();
            //EquipmentRelationship.PropId = EquipmentAssemblyObjectClassProp.PropId;
            //EquipmentRelationship.PropName = EquipmentAssemblyObjectClassPropName;
            //EquipmentRelationship.PropType = CswNbtViewRelationship.PropIdType.ObjectClassPropId;
            //EquipmentRelationship.PropOwner = CswNbtViewRelationship.PropOwnerType.Second;
            //EquipmentRelationship.FirstType = CswNbtViewRelationship.RelatedIdType.ObjectClassId;
            //EquipmentRelationship.FirstId = AssemblyObjectClass.ObjectClassId;
            //EquipmentRelationship.FirstName = AssemblyObjectClass.ObjectClass.ToString();
            //EquipmentRelationship.SecondType = CswNbtViewRelationship.RelatedIdType.ObjectClassId;
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

        public override void beforeDeleteNode()
        {
            _CswNbtObjClassDefault.beforeDeleteNode();

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
                    CswNbtObjClassEquipmentType TypeNodeAsType = CswNbtNodeCaster.AsEquipmentType( TypeNode );                    
                    CswDelimitedString PartsString = new CswDelimitedString( '\n' );
					PartsString.FromString( TypeNodeAsType.Parts.Text.Replace( "\r", "" ) );
                    this.Parts.YValues = PartsString;
                }
            }
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Type
        {
            get
            {
                return ( _CswNbtNode.Properties[TypePropertyName].AsRelationship );
            }
        }

        public CswNbtNodePropLogicalSet Parts
        {
            get
            {
                return ( _CswNbtNode.Properties[PartsPropertyName].AsLogicalSet );
            }
        }

        #endregion



    }//CswNbtObjClassEquipmentAssembly

}//namespace ChemSW.Nbt.ObjClasses
