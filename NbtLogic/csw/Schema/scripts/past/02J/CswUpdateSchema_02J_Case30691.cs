using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02J_Case30691 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30691; }
        }

        public override string Title
        {
            get { return "Add aliases to units of measure"; }
        }

        public override void update()
        {
            // Set aliases for Units of Measurement nodes
            CswNbtMetaDataObjectClass UoMOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UnitOfMeasureClass );
            foreach( CswNbtObjClassUnitOfMeasure UoMNode in UoMOC.getNodes( false, false ) )
            {
                switch( UoMNode.Name.Text )
                {
                    case "Each":
                        _updateAliasesValue( UoMOC, UoMNode, "each,ea,EA" );
                        break;
                    case "fluid ounces":
                        _updateAliasesValue( UoMOC, UoMNode, "fl oz,fl.oz." );
                        break;
                    case "g":
                        _updateAliasesValue( UoMOC, UoMNode, "G,gm,GM" );
                        break;
                    case "gal":
                        _updateAliasesValue( UoMOC, UoMNode, "Gal,GL,GA" );
                        break;
                    case "kg":
                        _updateAliasesValue( UoMOC, UoMNode, "KG" );
                        break;
                    case "Liters":
                        _updateAliasesValue( UoMOC, UoMNode, "L,LT" );
                        break;
                    case "mg":
                        _updateAliasesValue( UoMOC, UoMNode, "MG" );
                        break;
                    case "mL":
                        _updateAliasesValue( UoMOC, UoMNode, "ml,ML" );
                        break;
                    case "ounces":
                        _updateAliasesValue( UoMOC, UoMNode, "oz,OZ" );
                        break;
                    case "µL":
                        _updateAliasesValue( UoMOC, UoMNode, "microliter,microL,UL,uL" );
                        break;
                }
            }

        } // update()

        private void _updateAliasesValue( CswNbtMetaDataObjectClass UoMOC, CswNbtObjClassUnitOfMeasure UoMNode, string NewAliases )
        {
            // The new aliases we want to add
            CswCommaDelimitedString NewAliasesCommaDelimited = new CswCommaDelimitedString( NewAliases );
            CswCommaDelimitedString UpdatedAliases = UoMNode.AliasesAsDelimitedString;

            // Create a view of all UoM nodes and their Aliases property
            CswNbtView UoMView = _CswNbtSchemaModTrnsctn.makeView();
            CswNbtViewRelationship ParentRelationship = UoMView.AddViewRelationship( UoMOC, false );

            CswNbtMetaDataObjectClassProp AliasesOCP = UoMOC.getObjectClassProp( CswNbtObjClassUnitOfMeasure.PropertyName.Aliases );
            UoMView.AddViewProperty( ParentRelationship, AliasesOCP );

            CswCommaDelimitedString AliasesToRemove = new CswCommaDelimitedString();
            ICswNbtTree UoMNodesTree = _CswNbtSchemaModTrnsctn.getTreeFromView( UoMView, false );
            for( int i = 0; i < UoMNodesTree.getChildNodeCount(); i++ )
            {
                UoMNodesTree.goToNthChild( i );
                string CurrentNodeName = UoMNodesTree.getNodeNameForCurrentPosition();
                CswPrimaryKey CurrentNodeId = UoMNodesTree.getNodeIdForCurrentPosition();
                if( CurrentNodeId != UoMNode.NodeId )
                {
                    foreach( CswNbtTreeNodeProp TreeNodeProp in UoMNodesTree.getChildNodePropsOfNode() )
                    {
                        CswCommaDelimitedString CurrentUoMNodeAliases = new CswCommaDelimitedString();
                        CurrentUoMNodeAliases.FromString( TreeNodeProp.Gestalt, false, true );

                        foreach( string alias1 in NewAliasesCommaDelimited )
                        {
                            // If alias1 matches the NodeName or any of the Aliases on the Node, we don't want it
                            if( alias1.Equals( CurrentNodeName ) || CurrentUoMNodeAliases.Any( alias2 => alias1.Equals( alias2 ) ) )
                            {
                                AliasesToRemove.Add( alias1 );
                            }
                        }
                    }
                }
                UoMNodesTree.goToParentNode();
            }

            // Make the updated aliases for the node
            foreach( string alias1 in NewAliasesCommaDelimited )
            {
                if( false == AliasesToRemove.Contains( alias1 ) && false == UoMNode.AliasesAsDelimitedString.Contains( alias1 ) )
                {
                    UpdatedAliases.Add( alias1 );
                }
            }

            // Update the property value
            UoMNode.Aliases.Text = CswConvert.ToString( UpdatedAliases );
            UoMNode.postChanges( false );

        }//_updateAliasesValue()
    }

}//namespace ChemSW.Nbt.Schema