using ChemSW.Audit;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02D_Case30318 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30318; }
        }

        public override void update()
        {
            // This is a placeholder script that does nothing.
            CswNbtMetaDataObjectClass GeneratorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.GeneratorClass );
            foreach( CswNbtMetaDataNodeType GeneratorNT in GeneratorOC.getNodeTypes() )
            {
                GeneratorNT.AuditLevel = CswEnumAuditLevel.PlainAudit;
                CswNbtMetaDataNodeTypeProp NextDueDateNTP = GeneratorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.NextDueDate );
                NextDueDateNTP.AuditLevel = CswEnumAuditLevel.PlainAudit;
                CswNbtMetaDataNodeTypeProp EnabledNTP = GeneratorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.Enabled );
                EnabledNTP.AuditLevel = CswEnumAuditLevel.PlainAudit;
                CswNbtMetaDataNodeTypeProp WarningDaysNTP = GeneratorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.WarningDays );
                WarningDaysNTP.AuditLevel = CswEnumAuditLevel.PlainAudit;
                CswNbtMetaDataNodeTypeProp DueDateIntervalNTP = GeneratorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.DueDateInterval );
                DueDateIntervalNTP.AuditLevel = CswEnumAuditLevel.PlainAudit;
                CswNbtMetaDataNodeTypeProp FinalDueDateNTP = GeneratorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.FinalDueDate );
                FinalDueDateNTP.AuditLevel = CswEnumAuditLevel.PlainAudit;
                CswNbtMetaDataNodeTypeProp RunStatusNTP = GeneratorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.RunStatus );
                RunStatusNTP.AuditLevel = CswEnumAuditLevel.PlainAudit;
            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema