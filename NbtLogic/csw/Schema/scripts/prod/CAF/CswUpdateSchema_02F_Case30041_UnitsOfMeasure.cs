using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30041_UnitsOfMeasure : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 30041; }
        }

        public override string AppendToScriptName()
        {
            return "UnitsOfMeasure";
        }

        public override void update()
        {
                CswNbtSchemaUpdateImportMgr UnitMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "CAF" );

                UnitMgr.CAFimportOrder( "Unit_Each", "units_of_measure", "each_view", "unitofmeasureid" );

                UnitMgr.importBinding( "unitofmeasurename", CswNbtObjClassUnitOfMeasure.PropertyName.Name, "" );
                UnitMgr.importBinding( "conversionfactor", CswNbtObjClassUnitOfMeasure.PropertyName.ConversionFactor, CswEnumNbtSubFieldName.Base.ToString() );
                UnitMgr.importBinding( "conversionfactorexp", CswNbtObjClassUnitOfMeasure.PropertyName.ConversionFactor, CswEnumNbtSubFieldName.Exponent.ToString() );
                UnitMgr.importBinding( "unittype", CswNbtObjClassUnitOfMeasure.PropertyName.UnitType, "" );


                UnitMgr.CAFimportOrder( "Unit_Volume", "units_of_measure", "volume_view", "unitofmeasureid" );

                UnitMgr.importBinding( "unitofmeasurename", CswNbtObjClassUnitOfMeasure.PropertyName.Name, "" );
                UnitMgr.importBinding( "conversionfactor", CswNbtObjClassUnitOfMeasure.PropertyName.ConversionFactor, CswEnumNbtSubFieldName.Base.ToString() );
                UnitMgr.importBinding( "conversionfactorexp", CswNbtObjClassUnitOfMeasure.PropertyName.ConversionFactor, CswEnumNbtSubFieldName.Exponent.ToString() );
                UnitMgr.importBinding( "unittype", CswNbtObjClassUnitOfMeasure.PropertyName.UnitType, "" );


                UnitMgr.CAFimportOrder( "Unit_Weight", "units_of_measure", "weight_view", "unitofmeasureid" );

                UnitMgr.importBinding( "unitofmeasurename", CswNbtObjClassUnitOfMeasure.PropertyName.Name, "" );
                UnitMgr.importBinding( "conversionfactor", CswNbtObjClassUnitOfMeasure.PropertyName.ConversionFactor, CswEnumNbtSubFieldName.Base.ToString() );
                UnitMgr.importBinding( "conversionfactorexp", CswNbtObjClassUnitOfMeasure.PropertyName.ConversionFactor, CswEnumNbtSubFieldName.Exponent.ToString() );
                UnitMgr.importBinding( "unittype", CswNbtObjClassUnitOfMeasure.PropertyName.UnitType, "" );

                UnitMgr.finalize();

        } // update()

    } // class CswUpdateSchema_02F_Case30041_Vendors

}//namespace ChemSW.Nbt.Schema