using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 7608
    /// </summary>
    public class CswUpdateSchemaCase7608 : CswUpdateSchemaTo
    {
        public override void update()
        {
            #region Update UnitOfMeasure ObjClassProps - delete UnitType, add BaseUnit and Fractional

            CswNbtMetaDataObjectClass UnitOfMeasureOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UnitOfMeasureClass );

            CswNbtMetaDataObjectClassProp UnitTypeOCP = UnitOfMeasureOC.getObjectClassProp( "Unit Type" );
            _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( UnitTypeOCP, true );

            _CswNbtSchemaModTrnsctn.createObjectClassProp(
                CswNbtMetaDataObjectClass.NbtObjectClass.UnitOfMeasureClass,
                CswNbtObjClassUnitOfMeasure.BaseUnitPropertyName,
                CswNbtMetaDataFieldType.NbtFieldType.Text,
                ServerManaged: true );

            _CswNbtSchemaModTrnsctn.createObjectClassProp(
                CswNbtMetaDataObjectClass.NbtObjectClass.UnitOfMeasureClass,
                CswNbtObjClassUnitOfMeasure.FractionalPropertyName,
                CswNbtMetaDataFieldType.NbtFieldType.Logical );

            #endregion

            #region Delete Unit Of Measure ViewSelect View

            _CswNbtSchemaModTrnsctn.deleteView( "Units of Measure (IMCS)", true );

            #endregion

            #region Delete UnitOfMeasure NodeType (and Nodes)

            CswNbtMetaDataNodeType UnitOfMeasureNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Unit of Measure" );
            if( null != UnitOfMeasureNodeType )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeType( UnitOfMeasureNodeType );
            }

            #endregion

            #region Delete UnitType ObjectClass (and NodeTypes/Nodes)

            CswNbtMetaDataObjectClass UnitTypeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( "Unit Type" );
            if( null != UnitTypeOC )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClass( UnitTypeOC );
            }

            #endregion

            #region Add the new NodeTypes and Nodes

            CswNbtMetaDataNodeType WeightUnitNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( "UnitOfMeasureClass", "Weight", "Units" );
            WeightUnitNodeType.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassUnitOfMeasure.NamePropertyName ) );
            CswNbtMetaDataNodeTypeProp WeightBaseUnitProp = WeightUnitNodeType.getNodeTypeProp( CswNbtObjClassUnitOfMeasure.BaseUnitPropertyName );
            WeightBaseUnitProp.DefaultValue.AsText.Text = "kg";

            createUnitOfMeasureNode( WeightUnitNodeType.NodeTypeId, "kg", 1.0, 0, Tristate.True );
            createUnitOfMeasureNode( WeightUnitNodeType.NodeTypeId, "g", 1.0, -3, Tristate.True );
            createUnitOfMeasureNode( WeightUnitNodeType.NodeTypeId, "mg", 1.0, -6, Tristate.True );
            createUnitOfMeasureNode( WeightUnitNodeType.NodeTypeId, "lb", 4.5359237, -1, Tristate.True );
            createUnitOfMeasureNode( WeightUnitNodeType.NodeTypeId, "ounces", 2.83495231, -2, Tristate.True );

            CswNbtMetaDataNodeType VolumeUnitNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( "UnitOfMeasureClass", "Volume", "Units" );
            VolumeUnitNodeType.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassUnitOfMeasure.NamePropertyName ) );
            CswNbtMetaDataNodeTypeProp VolumeBaseUnitProp = VolumeUnitNodeType.getNodeTypeProp( CswNbtObjClassUnitOfMeasure.BaseUnitPropertyName );
            VolumeBaseUnitProp.DefaultValue.AsText.Text = "Liters";

            createUnitOfMeasureNode( VolumeUnitNodeType.NodeTypeId, "Liters", 1.0, 0, Tristate.True );
            createUnitOfMeasureNode( VolumeUnitNodeType.NodeTypeId, "mL", 1.0, -3, Tristate.True );
            createUnitOfMeasureNode( VolumeUnitNodeType.NodeTypeId, "µL", 1.0, -6, Tristate.True );
            createUnitOfMeasureNode( VolumeUnitNodeType.NodeTypeId, "gal", 3.78541178, 0, Tristate.True );
            createUnitOfMeasureNode( VolumeUnitNodeType.NodeTypeId, "fluid ounces", 2.95735296, -2, Tristate.True );

            CswNbtMetaDataNodeType EachUnitNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( "UnitOfMeasureClass", "Each", "Units" );
            EachUnitNodeType.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassUnitOfMeasure.NamePropertyName ) );
            CswNbtMetaDataNodeTypeProp EachBaseUnitProp = EachUnitNodeType.getNodeTypeProp( CswNbtObjClassUnitOfMeasure.BaseUnitPropertyName );
            EachBaseUnitProp.DefaultValue.AsText.Text = "Each";

            createUnitOfMeasureNode( EachUnitNodeType.NodeTypeId, "Each", 1.0, 0, Tristate.False );
            createUnitOfMeasureNode( EachUnitNodeType.NodeTypeId, "Boxes", Int32.MinValue, Int32.MinValue, Tristate.False );
            createUnitOfMeasureNode( EachUnitNodeType.NodeTypeId, "Cases", Int32.MinValue, Int32.MinValue, Tristate.False );
            createUnitOfMeasureNode( EachUnitNodeType.NodeTypeId, "Cylinders", Int32.MinValue, Int32.MinValue, Tristate.False );

            CswNbtMetaDataNodeType TimeUnitNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( "UnitOfMeasureClass", "Time", "Units" );
            TimeUnitNodeType.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassUnitOfMeasure.NamePropertyName ) );
            CswNbtMetaDataNodeTypeProp TimeBaseUnitProp = TimeUnitNodeType.getNodeTypeProp( CswNbtObjClassUnitOfMeasure.BaseUnitPropertyName );
            TimeBaseUnitProp.DefaultValue.AsText.Text = "Days";

            createUnitOfMeasureNode( TimeUnitNodeType.NodeTypeId, "Days", 1.0, 0, Tristate.True );
            createUnitOfMeasureNode( TimeUnitNodeType.NodeTypeId, "Weeks", 7.0, 0, Tristate.True );
            createUnitOfMeasureNode( TimeUnitNodeType.NodeTypeId, "Years", 3.65, 2, Tristate.True );
            createUnitOfMeasureNode( TimeUnitNodeType.NodeTypeId, "Hours", 4.1666667, -2, Tristate.True );
            createUnitOfMeasureNode( TimeUnitNodeType.NodeTypeId, "Minutes", 6.94444444, -4, Tristate.True );
            createUnitOfMeasureNode( TimeUnitNodeType.NodeTypeId, "Seconds", 1.15740741, -5, Tristate.True );

            #endregion

            #region Create new UnitOfMeasure ViewSelect View

            CswNbtView UnitsView = _CswNbtSchemaModTrnsctn.makeView();
            UnitsView.makeNew( "Units of Measurement", NbtViewVisibility.Global );
            UnitsView.Category = "System";
            UnitsView.ViewMode = NbtViewRenderingMode.Tree;

            CswNbtViewRelationship UnitRelationship = UnitsView.AddViewRelationship( UnitOfMeasureOC, true );

            UnitsView.save();

            #endregion
        }//Update()

        #region Private Helper Functions

        private void createUnitOfMeasureNode( int NodeTypeId, string Name, double ConversionFactorBase, int ConversionFactorExponent, Tristate Fractional )
        {
            CswNbtNode UnitOfMeasureNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            CswNbtObjClassUnitOfMeasure NodeAsUnitOfMeasure = _CswNbtSchemaModTrnsctn.Nodes[UnitOfMeasureNode.NodeId];
            NodeAsUnitOfMeasure.Name.Text = Name;
            if( ConversionFactorBase != Int32.MinValue )
                NodeAsUnitOfMeasure.ConversionFactor.Base = ConversionFactorBase;
            if( ConversionFactorExponent != Int32.MinValue )
                NodeAsUnitOfMeasure.ConversionFactor.Exponent = ConversionFactorExponent;
            NodeAsUnitOfMeasure.Fractional.Checked = Fractional;
            NodeAsUnitOfMeasure.postChanges( true );
        }

        #endregion

    }//class CswUpdateSchemaCase7608

}//namespace ChemSW.Nbt.Schema