using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30041_UnitsOfMeasure : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 30041; }
        }

        public override string ScriptName
        {
            get { return "02G_Case30041_UnitsOfMeasure"; }
        }

        public override void update()
        {
            {
                CswNbtSchemaUpdateImportMgr UnitMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "units_of_measure", ViewName: "each_view", DestNodeTypeName: "Unit_Each");

                UnitMgr.importBinding( "unitofmeasurename", CswNbtObjClassUnitOfMeasure.PropertyName.Name, "" );
                UnitMgr.importBinding( "conversionfactor", CswNbtObjClassUnitOfMeasure.PropertyName.ConversionFactor, "" );
                UnitMgr.importBinding( "unittype", CswNbtObjClassUnitOfMeasure.PropertyName.UnitType, "" );

                UnitMgr.finalize( UseView: true );

            }
            {
                CswNbtSchemaUpdateImportMgr UnitMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "units_of_measure", ViewName : "volume_view", DestNodeTypeName : "Unit_Volume");

                UnitMgr.importBinding( "unitofmeasurename", CswNbtObjClassUnitOfMeasure.PropertyName.Name, "" );
                UnitMgr.importBinding( "conversionfactor", CswNbtObjClassUnitOfMeasure.PropertyName.ConversionFactor, "" );
                UnitMgr.importBinding( "unittype", CswNbtObjClassUnitOfMeasure.PropertyName.UnitType, "" );

                UnitMgr.finalize( UseView : true );

            }
            {
                CswNbtSchemaUpdateImportMgr UnitMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "units_of_measure", ViewName : "weight_view", DestNodeTypeName : "Unit_Weight");

                UnitMgr.importBinding( "unitofmeasurename", CswNbtObjClassUnitOfMeasure.PropertyName.Name, "" );
                UnitMgr.importBinding( "conversionfactor", CswNbtObjClassUnitOfMeasure.PropertyName.ConversionFactor, "" );
                UnitMgr.importBinding( "unittype", CswNbtObjClassUnitOfMeasure.PropertyName.UnitType, "" );

                UnitMgr.finalize( UseView : true );

            }

        } // update()

    } // class CswUpdateSchema_02G_Case30041_UnitsOfMeasure

}//namespace ChemSW.Nbt.Schema