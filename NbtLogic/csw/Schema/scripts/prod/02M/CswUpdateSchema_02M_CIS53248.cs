using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_CIS53248 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 53248; }
        }

        public override string Title
        {
            get { return "Script for " + CaseNo + ": MLM2: CertDef configuration"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass CertDefOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.CertificateDefinitionClass );

        }
    }
}