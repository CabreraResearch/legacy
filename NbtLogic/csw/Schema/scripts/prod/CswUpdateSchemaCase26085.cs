using System.Collections.Generic;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26085
    /// </summary>
    public class CswUpdateSchemaCase26085 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // create "Unassigned Type" node
            CswNbtMetaDataNodeType eqtypeNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Equipment Type" );

            if( null != eqtypeNT )
            {
                CswNbtMetaDataNodeTypeProp nameProp = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( eqtypeNT.NodeTypeId, "Type Name" );
                CswNbtMetaDataNodeType equipNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Equipment" );
                CswNbtMetaDataNodeTypeProp equipstypeProp = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( equipNT.NodeTypeId, "Type" );
                CswNbtMetaDataNodeType assemblyNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Assembly" );
                CswNbtMetaDataNodeTypeProp atypeProp = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( assemblyNT.NodeTypeId, "Assembly Type" );

                CswNbtNode anode = null;
                if( null != nameProp )
                {
                    anode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( eqtypeNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                    anode.Properties[nameProp].AsText.Text = "Unassigned Type";
                    anode.postChanges( true );

                    //force all equipment with null type to be set to this node
                    CswNbtMetaDataObjectClass EquipmentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass );
                    IEnumerable<CswNbtNode> EquipNodes = EquipmentOC.getNodes( false, true );
                    foreach( CswNbtNode Node in EquipNodes )
                    {
                        CswNbtObjClassEquipment eqNode = CswNbtNodeCaster.AsEquipment( Node );
                        if( true == eqNode.Type.Empty )
                        {
                            eqNode.Type.RelatedNodeId = anode.NodeId;
                            eqNode.postChanges( true );
                        }
                    }

                    //force all assemblies with null type to be set to this node
                    CswNbtMetaDataObjectClass AssemblyOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentAssemblyClass );
                    IEnumerable<CswNbtNode> assemblyNodes = AssemblyOC.getNodes( false, true );
                    foreach( CswNbtNode Node in assemblyNodes )
                    {
                        CswNbtObjClassEquipmentAssembly eqNode = CswNbtNodeCaster.AsEquipmentAssembly( Node );
                        if( true == eqNode.Type.Empty )
                        {
                            eqNode.Type.RelatedNodeId = anode.NodeId;
                            eqNode.postChanges( true );
                        }
                    }

                    //change equipment and assembly so their type relationships are REQUIRED
                    equipstypeProp.IsRequired = true;
                    atypeProp.IsRequired = true;
                }

                //simplify the All Equipment view
                _CswNbtSchemaModTrnsctn.deleteView( "All Equipment", true );
                CswNbtView allEqView = _CswNbtSchemaModTrnsctn.makeView();
                allEqView.makeNew( "All Equipment", NbtViewVisibility.Global );
                allEqView.ViewMode = NbtViewRenderingMode.Tree;
                allEqView.Category = "Equipment";

                //CswNbtView allEqView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( "All Equipment" );
                if( null != allEqView )
                {
                    //allEqView.Root.ChildRelationships.Clear();

                    CswNbtViewRelationship typeRelationship = allEqView.AddViewRelationship( eqtypeNT, true );
                    //add equipment
                    CswNbtViewRelationship eqRelationship = allEqView.AddViewRelationship( typeRelationship, NbtViewPropOwnerType.Second, equipstypeProp, true );
                    CswNbtMetaDataNodeTypeProp eqstatTypeProp = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( assemblyNT.NodeTypeId, "Status" );
                    CswNbtViewProperty eqstatViewProp = allEqView.AddViewProperty( eqRelationship, eqstatTypeProp );
                    CswNbtViewPropertyFilter ArbitraryFilter1 = allEqView.AddViewPropertyFilter(
                                         eqstatViewProp,
                                         CswNbtSubField.SubFieldName.Unknown,
                                         CswNbtPropFilterSql.PropertyFilterMode.NotEquals,
                                         "Retired",
                                         false );

                    //add assembly
                    CswNbtViewRelationship assemblyRelationship = allEqView.AddViewRelationship( typeRelationship, NbtViewPropOwnerType.Second, atypeProp, true );
                    CswNbtMetaDataNodeTypeProp AstatTypeProp = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( assemblyNT.NodeTypeId, "Status" );
                    CswNbtViewProperty AstatViewProp = allEqView.AddViewProperty( assemblyRelationship, AstatTypeProp );
                    CswNbtViewPropertyFilter ArbitraryFilter2 = allEqView.AddViewPropertyFilter(
                                         AstatViewProp,
                                         CswNbtSubField.SubFieldName.Unknown,
                                         CswNbtPropFilterSql.PropertyFilterMode.NotEquals,
                                         "Retired",
                                         false );
                    allEqView.save();

                }
            }

        }//Update()

    }//class CswUpdateSchemaCase26085

}//namespace ChemSW.Nbt.Schema