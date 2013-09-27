using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
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

        public override string ScriptName
        {
            get { return "02F_Case30041_UnitsOfMeasure"; }
        }

        public override void update()
        {
            {
                CswNbtSchemaUpdateImportMgr UnitMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "units_of_measure", "Unit_Each" );

                UnitMgr.importBinding( "unitofmeasurename", CswNbtObjClassUnitOfMeasure.PropertyName.Name, "" );
                UnitMgr.importBinding( "convertfromeaches_base", CswNbtObjClassUnitOfMeasure.PropertyName.ConversionFactor, "" );
                UnitMgr.importBinding( "unittype", CswNbtObjClassUnitOfMeasure.PropertyName.UnitType, "" );

                UnitMgr.finalize();

            }
            {
                CswNbtSchemaUpdateImportMgr UnitMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "units_of_measure", "Unit_Volume" );

                UnitMgr.importBinding( "unitofmeasurename", CswNbtObjClassUnitOfMeasure.PropertyName.Name, "" );
                UnitMgr.importBinding( "convertfromliters_base", CswNbtObjClassUnitOfMeasure.PropertyName.ConversionFactor, "" );
                UnitMgr.importBinding( "unittype", CswNbtObjClassUnitOfMeasure.PropertyName.UnitType, "" );

                UnitMgr.finalize();
            }
            {
                CswNbtSchemaUpdateImportMgr UnitMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "units_of_measure", "Unit_Weight" );

                UnitMgr.importBinding( "unitofmeasurename", CswNbtObjClassUnitOfMeasure.PropertyName.Name, "" );
                UnitMgr.importBinding( "convertfromkg_base", CswNbtObjClassUnitOfMeasure.PropertyName.ConversionFactor, "" );
                UnitMgr.importBinding( "unittype", CswNbtObjClassUnitOfMeasure.PropertyName.UnitType, "" );

                UnitMgr.finalize();
            }

        } // update()

    } // class CswUpdateSchema_02F_Case30041_Vendors

}//namespace ChemSW.Nbt.Schema