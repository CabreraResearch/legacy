using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27432
    /// </summary>
    public class CswUpdateSchemaCase27432 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataNodeType TimeUnitNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Unit (Time)" );
            if( null != TimeUnitNodeType )
            {
                CswNbtObjClassUnitOfMeasure NodeAsUnitOfMeasure = null;
                foreach( CswNbtObjClassUnitOfMeasure Unit in TimeUnitNodeType.getNodes( forceReInit: true, includeSystemNodes: false ) )
                {
                    if( Unit.Name.Text == "Months" )
                    {
                        NodeAsUnitOfMeasure = Unit;
                        break;
                    }
                }
                if( null == NodeAsUnitOfMeasure )
                {
                    NodeAsUnitOfMeasure = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( TimeUnitNodeType.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                    NodeAsUnitOfMeasure.Name.Text = "Months";
                    NodeAsUnitOfMeasure.Fractional.Checked = Tristate.False;
                    NodeAsUnitOfMeasure.postChanges( true );
                }
                CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
                foreach( CswNbtMetaDataNodeType MaterialNodeType in MaterialOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp ExpInt = MaterialNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.ExpirationInterval );
                    CswNbtView TimeView = _CswNbtSchemaModTrnsctn.restoreView( ExpInt.ViewId );

                    CswNbtViewRelationship TimeUnitNodeTypeRelationship = TimeView.Root.ChildRelationships[0];
                    CswNbtMetaDataNodeTypeProp TimeNameNtp = TimeUnitNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassUnitOfMeasure.NamePropertyName );
                    CswNbtViewProperty TimeNameViewProp = TimeView.AddViewProperty( TimeUnitNodeTypeRelationship, TimeNameNtp );

                    TimeView.AddViewPropertyFilter(
                        ParentViewProperty: TimeNameViewProp,
                        Conjunction: CswNbtPropFilterSql.PropertyFilterConjunction.Or,
                        SubFieldName: CswNbtSubField.SubFieldName.Text,
                        FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals,
                        Value: "Months"
                        );
                    TimeView.AddViewPropertyFilter(
                        ParentViewProperty: TimeNameViewProp,
                        Conjunction: CswNbtPropFilterSql.PropertyFilterConjunction.Or,
                        SubFieldName: CswNbtSubField.SubFieldName.Text,
                        FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals,
                        Value: "Years"
                        );
                    TimeView.save();
                }

            }

        }//Update()

    }//class CswUpdateSchemaCase27432

}//namespace ChemSW.Nbt.Schema