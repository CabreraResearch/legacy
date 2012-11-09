using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

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

            _CswNbtSchemaModTrnsctn.createObjectClassProp(
                CswNbtMetaDataObjectClass.NbtObjectClass.UnitOfMeasureClass,
                CswNbtObjClassUnitOfMeasure.UnitTypePropertyName,
                CswNbtMetaDataFieldType.NbtFieldType.List,
                ServerManaged: true,
                ListOptions: String.Join( ",", CswNbtObjClassUnitOfMeasure.UnitTypes._All )
                );
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

            #region Delete UnitType ObjectClass, NodeTypes and Nodes

            CswNbtMetaDataObjectClass UnitTypeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( "UnitTypeClass" );
            if( null != UnitTypeOC )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClass( UnitTypeOC );
            }

            #endregion

            #region Add the new NodeTypes and Nodes

            CswNbtMetaDataNodeType WeightUnitNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( "UnitOfMeasureClass", "Weight Unit", "Units" );
            WeightUnitNodeType.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassUnitOfMeasure.NamePropertyName ) );
            CswNbtMetaDataNodeTypeProp WeightBaseUnitProp = WeightUnitNodeType.getNodeTypeProp( CswNbtObjClassUnitOfMeasure.BaseUnitPropertyName );
            WeightBaseUnitProp.DefaultValue.AsText.Text = "kg";
            CswNbtMetaDataNodeTypeProp WeightUnitTypeProp = WeightUnitNodeType.getNodeTypeProp( CswNbtObjClassUnitOfMeasure.UnitTypePropertyName );
            WeightUnitTypeProp.DefaultValue.AsList.Value = CswNbtObjClassUnitOfMeasure.UnitTypes.Weight.ToString();

            createUnitOfMeasureNode( WeightUnitNodeType.NodeTypeId, "kg", 1.0, 0, Tristate.True );
            createUnitOfMeasureNode( WeightUnitNodeType.NodeTypeId, "g", 1.0, 3, Tristate.True );
            createUnitOfMeasureNode( WeightUnitNodeType.NodeTypeId, "mg", 1.0, 6, Tristate.True );
            createUnitOfMeasureNode( WeightUnitNodeType.NodeTypeId, "lb", 2.20462262, 0, Tristate.True );
            createUnitOfMeasureNode( WeightUnitNodeType.NodeTypeId, "ounces", 3.52739619, 1, Tristate.True );

            CswNbtMetaDataNodeType VolumeUnitNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( "UnitOfMeasureClass", "Volume Unit", "Units" );
            VolumeUnitNodeType.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassUnitOfMeasure.NamePropertyName ) );
            CswNbtMetaDataNodeTypeProp VolumeBaseUnitProp = VolumeUnitNodeType.getNodeTypeProp( CswNbtObjClassUnitOfMeasure.BaseUnitPropertyName );
            VolumeBaseUnitProp.DefaultValue.AsText.Text = "Liters";
            CswNbtMetaDataNodeTypeProp VolumeUnitTypeProp = VolumeUnitNodeType.getNodeTypeProp( CswNbtObjClassUnitOfMeasure.UnitTypePropertyName );
            VolumeUnitTypeProp.DefaultValue.AsList.Value = CswNbtObjClassUnitOfMeasure.UnitTypes.Volume.ToString();

            createUnitOfMeasureNode( VolumeUnitNodeType.NodeTypeId, "Liters", 1.0, 0, Tristate.True );
            createUnitOfMeasureNode( VolumeUnitNodeType.NodeTypeId, "mL", 1.0, 3, Tristate.True );
            createUnitOfMeasureNode( VolumeUnitNodeType.NodeTypeId, "µL", 1.0, 6, Tristate.True );
            createUnitOfMeasureNode( VolumeUnitNodeType.NodeTypeId, "gal", 2.64172052, -1, Tristate.True );
            createUnitOfMeasureNode( VolumeUnitNodeType.NodeTypeId, "fluid ounces", 3.38140227, 1, Tristate.True );

            CswNbtMetaDataNodeType EachUnitNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( "UnitOfMeasureClass", "Each Unit", "Units" );
            EachUnitNodeType.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassUnitOfMeasure.NamePropertyName ) );
            CswNbtMetaDataNodeTypeProp EachBaseUnitProp = EachUnitNodeType.getNodeTypeProp( CswNbtObjClassUnitOfMeasure.BaseUnitPropertyName );
            EachBaseUnitProp.DefaultValue.AsText.Text = "Each";
            CswNbtMetaDataNodeTypeProp EachUnitTypeProp = EachUnitNodeType.getNodeTypeProp( CswNbtObjClassUnitOfMeasure.UnitTypePropertyName );
            EachUnitTypeProp.DefaultValue.AsList.Value = CswNbtObjClassUnitOfMeasure.UnitTypes.Each.ToString();

            createUnitOfMeasureNode( EachUnitNodeType.NodeTypeId, "Each", 1.0, 0, Tristate.False );
            createUnitOfMeasureNode( EachUnitNodeType.NodeTypeId, "Boxes", Int32.MinValue, Int32.MinValue, Tristate.False );
            createUnitOfMeasureNode( EachUnitNodeType.NodeTypeId, "Cases", Int32.MinValue, Int32.MinValue, Tristate.False );
            createUnitOfMeasureNode( EachUnitNodeType.NodeTypeId, "Cylinders", Int32.MinValue, Int32.MinValue, Tristate.False );

            CswNbtMetaDataNodeType TimeUnitNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( "UnitOfMeasureClass", "Time Unit", "Units" );
            TimeUnitNodeType.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassUnitOfMeasure.NamePropertyName ) );
            CswNbtMetaDataNodeTypeProp TimeBaseUnitProp = TimeUnitNodeType.getNodeTypeProp( CswNbtObjClassUnitOfMeasure.BaseUnitPropertyName );
            TimeBaseUnitProp.DefaultValue.AsText.Text = "Days";
            CswNbtMetaDataNodeTypeProp TimeUnitTypeProp = TimeUnitNodeType.getNodeTypeProp( CswNbtObjClassUnitOfMeasure.UnitTypePropertyName );
            TimeUnitTypeProp.DefaultValue.AsList.Value = CswNbtObjClassUnitOfMeasure.UnitTypes.Time.ToString();

            createUnitOfMeasureNode( TimeUnitNodeType.NodeTypeId, "Days", 1.0, 0, Tristate.True );
            createUnitOfMeasureNode( TimeUnitNodeType.NodeTypeId, "Weeks", 1.42857143, -1, Tristate.True );
            createUnitOfMeasureNode( TimeUnitNodeType.NodeTypeId, "Years", 2.73790926, -3, Tristate.True );
            createUnitOfMeasureNode( TimeUnitNodeType.NodeTypeId, "Hours", 2.4, 1, Tristate.True );
            createUnitOfMeasureNode( TimeUnitNodeType.NodeTypeId, "Minutes", 1.44, 3, Tristate.True );
            createUnitOfMeasureNode( TimeUnitNodeType.NodeTypeId, "Seconds", 8.64, 4, Tristate.True );

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