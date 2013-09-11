using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30557 : CswUpdateSchemaTo
    {
        public override string Title { get { return "Fill in Quantity val_kg and val_Liters"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30557; }
        }

        public override string ScriptName
        {
            get { return "Case30557"; }
        }

        public override void update()
        {
            CswNbtMetaDataFieldType QuantityFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswEnumNbtFieldType.Quantity );
            CswTableUpdate QtyUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "QuantityVal_kgLiters_Update", "jct_nodes_props" );
            DataTable QtyPropsTable = QtyUpdate.getTable( "where nodetypepropid in (select nodetypepropid from nodetype_props where fieldtypeid = " + QuantityFT.FieldTypeId + ") " );
            foreach( DataRow Row in QtyPropsTable.Rows )
            {
                CswPrimaryKey UnitId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( Row["field1_fk"].ToString() ) );
                CswNbtObjClassUnitOfMeasure CurrentUnit = _CswNbtSchemaModTrnsctn.Nodes[UnitId];
                if( null != CurrentUnit &&
                    ( CurrentUnit.UnitType.Value == CswEnumNbtUnitTypes.Weight.ToString() ||
                      CurrentUnit.UnitType.Value == CswEnumNbtUnitTypes.Volume.ToString() ) )
                {
                    Double Quantity = CswConvert.ToDouble( Row["field1_numeric"].ToString() );
                    if( CswTools.IsDouble( Quantity ) )
                    {
                        CswNbtObjClassUnitOfMeasure kgUnit = getUnit( "kg", "Unit_Weight" );
                        CswNbtObjClassUnitOfMeasure LitersUnit = getUnit( "Liters", "Unit_Volume" );
                        if( null != kgUnit && CurrentUnit.UnitType.Value == kgUnit.UnitType.Value )
                        {
                            Double Val_kg = Quantity*CurrentUnit.ConversionFactor.RealValue/kgUnit.ConversionFactor.RealValue;
                            Row["field2_numeric"] = Val_kg.ToString();
                        }
                        if( null != LitersUnit && CurrentUnit.UnitType.Value == LitersUnit.UnitType.Value )
                        {
                            Double Val_Liters = Quantity*CurrentUnit.ConversionFactor.RealValue/LitersUnit.ConversionFactor.RealValue;
                            Row["field3_numeric"] = Val_Liters.ToString();
                        }
                    }
                }
            }
            QtyUpdate.update( QtyPropsTable );
        }

        private CswNbtObjClassUnitOfMeasure getUnit( String UnitName, String NodeTypeName )
        {
            CswNbtObjClassUnitOfMeasure Unit = null;
            CswNbtMetaDataNodeType UnitNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( NodeTypeName );
            if( null != UnitNT )
            {
                foreach( CswNbtObjClassUnitOfMeasure UnitNode in UnitNT.getNodes( false, false ) )
                {
                    if( UnitName == UnitNode.Name.Text )
                    {
                        Unit = UnitNode;
                        break;
                    }
                }
            }
            return Unit;
        }
    }
}