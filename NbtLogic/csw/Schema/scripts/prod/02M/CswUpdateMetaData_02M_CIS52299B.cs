using System;
using ChemSW.Audit;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.Sched;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Schema;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02M_CIS52299B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 52299; }
        }

        public override string Title
        {
            get { return "switch Component to use NumericRange fieldtype"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public const string oldLowPercentageValue = "Low % Value";
        public const string oldTargetPercentageValue = "Target % Value";
        public const string oldHighPercentageValue = "High % Value";
        public const string oldPercentageValue = "Percentage";

        public override void update()
        {
            CswNbtMetaDataObjectClass ComponentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialComponentClass );

            // Make new NumericRange property on MaterialComponent
            _CswNbtSchemaModTrnsctn.createObjectClassProp( ComponentOC, new CswNbtWcfMetaDataModel.ObjectClassProp()
                {
                    PropName = CswNbtObjClassMaterialComponent.PropertyName.PercentageRange,
                    FieldType = CswEnumNbtFieldType.NumericRange,
                    NumberMinValue = 0,
                    NumberMaxValue = 100,
                    NumberPrecision = 3,
                    IsRequired = true,
                    SetValOnAdd = true,
                    AuditLevel = CswEnumAuditLevel.PlainAudit
                } );
            

            // Fix CAF bindings
            CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "CAF" );

            ImpMgr.removeImportBinding( CswScheduleLogicNbtCAFImport.DefinitionName, "quantity", "Material Component", oldTargetPercentageValue, "Value" );
            ImpMgr.removeImportBinding( CswScheduleLogicNbtCAFImport.DefinitionName, "quantity", "Material Component", oldHighPercentageValue, "Value" );

            ImpMgr.importBinding( "quantity", CswNbtObjClassMaterialComponent.PropertyName.PercentageRange, CswNbtFieldTypeRuleNumericRange.SubFieldName.Target.ToString() );
            ImpMgr.importBinding( "quantity", CswNbtObjClassMaterialComponent.PropertyName.PercentageRange, CswNbtFieldTypeRuleNumericRange.SubFieldName.Upper.ToString() );

            ImpMgr.finalize();

        }
    }
}