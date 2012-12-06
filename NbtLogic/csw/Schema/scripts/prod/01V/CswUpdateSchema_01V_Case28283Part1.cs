using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28283
    /// </summary>
    public class CswUpdateSchema_01V_Case28283Part1 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28283; }
        }

        public override void update()
        {
            CswNbtMetaDataNodeType VolumeUnitNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Volume Unit" );
            if( null != VolumeUnitNodeType )
            {
                createUnitOfMeasureNode( VolumeUnitNodeType.NodeTypeId, "cu.ft.", 3.53147, -2, Tristate.True );
            }

            CswNbtMetaDataNodeType RadiationUnitNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( "UnitOfMeasureClass", "Radiation Unit", "Units" );
            RadiationUnitNodeType.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassUnitOfMeasure.PropertyName.Name ) );
            CswNbtMetaDataNodeTypeProp RadiationBaseUnitProp = RadiationUnitNodeType.getNodeTypeProp( CswNbtObjClassUnitOfMeasure.PropertyName.BaseUnit );
            RadiationBaseUnitProp.DefaultValue.AsText.Text = "Ci";
            CswNbtMetaDataNodeTypeProp RadiationUnitTypeProp = RadiationUnitNodeType.getNodeTypeProp( CswNbtObjClassUnitOfMeasure.PropertyName.UnitType );
            RadiationUnitTypeProp.DefaultValue.AsList.Value = CswNbtObjClassUnitOfMeasure.UnitTypes.Radiation.ToString();

            createUnitOfMeasureNode( RadiationUnitNodeType.NodeTypeId, "Ci", 1.0, 0, Tristate.True );
            createUnitOfMeasureNode( RadiationUnitNodeType.NodeTypeId, "mCi", 1.0, 3, Tristate.True );
            createUnitOfMeasureNode( RadiationUnitNodeType.NodeTypeId, "Bq", 2.7027027, -11, Tristate.True );
        }

        private void createUnitOfMeasureNode( int NodeTypeId, string Name, double ConversionFactorBase, int ConversionFactorExponent, Tristate Fractional )
        {
            CswNbtNode UnitOfMeasureNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            CswNbtObjClassUnitOfMeasure NodeAsUnitOfMeasure = _CswNbtSchemaModTrnsctn.Nodes[UnitOfMeasureNode.NodeId];
            NodeAsUnitOfMeasure.Name.Text = Name;
            NodeAsUnitOfMeasure.ConversionFactor.Base = ConversionFactorBase;
            NodeAsUnitOfMeasure.ConversionFactor.Exponent = ConversionFactorExponent;
            NodeAsUnitOfMeasure.Fractional.Checked = Fractional;
            NodeAsUnitOfMeasure.postChanges( true );
        }

    }//class CswUpdateSchemaCase_01V_28283

}//namespace ChemSW.Nbt.Schema