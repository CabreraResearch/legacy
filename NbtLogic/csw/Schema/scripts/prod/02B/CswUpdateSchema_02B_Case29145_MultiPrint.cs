using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_02B_Case29145_MultiPrint : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {
            CswNbtObjClassPrintLabel.updateLabels( _CswNbtSchemaModTrnsctn.MetaData._CswNbtMetaDataResources.CswNbtResources );
        }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 29145; }
        }

    }//class CswUpdateSchema_02B_CaseXXXXX

}//namespace ChemSW.Nbt.Schema