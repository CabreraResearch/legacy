using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_CIS52993 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 52993; }
        }

        public override string Title
        {
            get { return "fix workunit props"; }
        }

        public override string AppendToScriptName()
        {
            return "";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass WorkUnitOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.WorkUnitClass );
            
            CswNbtMetaDataObjectClassProp WorkUnitAuditingEnabledOCP = WorkUnitOC.getObjectClassProp( CswNbtObjClassWorkUnit.PropertyName.AuditingEnabled );
            CswNbtMetaDataObjectClassProp WorkUnitSignatureRequiredOCP = WorkUnitOC.getObjectClassProp( CswNbtObjClassWorkUnit.PropertyName.SignatureRequired );
            
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( WorkUnitAuditingEnabledOCP, CswEnumNbtObjectClassPropAttributes.multi, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( WorkUnitSignatureRequiredOCP, CswEnumNbtObjectClassPropAttributes.multi, true );
        }
    } // class CswUpdateSchema_02M_CIS52993
}