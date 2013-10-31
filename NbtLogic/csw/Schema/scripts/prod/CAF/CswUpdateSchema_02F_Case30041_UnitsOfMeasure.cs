﻿using ChemSW.Nbt.csw.Dev;
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

        public override string AppendToScriptName()
        {
            return "UnitsOfMeasure";
        }

        public override void update()
        {
            {
                CswNbtSchemaUpdateImportMgr UnitMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "units_of_measure", ViewName: "each_view", SourceColumn: "unitofmeasureid", DestNodeTypeName: "Unit_Each" );

                UnitMgr.importBinding( "unitofmeasurename", CswNbtObjClassUnitOfMeasure.PropertyName.Name, "" );
                UnitMgr.importBinding( "conversionfactor", CswNbtObjClassUnitOfMeasure.PropertyName.ConversionFactor, "" );
                UnitMgr.importBinding( "unittype", CswNbtObjClassUnitOfMeasure.PropertyName.UnitType, "" );

                UnitMgr.finalize();

            }
            {
                CswNbtSchemaUpdateImportMgr UnitMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "units_of_measure", ViewName: "volume_view", SourceColumn: "unitofmeasureid", DestNodeTypeName: "Unit_Volume" );

                UnitMgr.importBinding( "unitofmeasurename", CswNbtObjClassUnitOfMeasure.PropertyName.Name, "" );
                UnitMgr.importBinding( "conversionfactor", CswNbtObjClassUnitOfMeasure.PropertyName.ConversionFactor, "" );
                UnitMgr.importBinding( "unittype", CswNbtObjClassUnitOfMeasure.PropertyName.UnitType, "" );

                UnitMgr.finalize();
            }
            {
                CswNbtSchemaUpdateImportMgr UnitMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "units_of_measure", ViewName: "weight_view", SourceColumn: "unitofmeasureid", DestNodeTypeName: "Unit_Weight" );

                UnitMgr.importBinding( "unitofmeasurename", CswNbtObjClassUnitOfMeasure.PropertyName.Name, "" );
                UnitMgr.importBinding( "conversionfactor", CswNbtObjClassUnitOfMeasure.PropertyName.ConversionFactor, "" );
                UnitMgr.importBinding( "unittype", CswNbtObjClassUnitOfMeasure.PropertyName.UnitType, "" );

                UnitMgr.finalize();
            }

        } // update()

    } // class CswUpdateSchema_02F_Case30041_Vendors

}//namespace ChemSW.Nbt.Schema