using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    public class CswUpdateMetaData_02K_Case31843: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 31843; }
        }

        public override string Title
        {
            get { return "Attach GHS Object Classes to CISPro Module"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass SignalWordOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSSignalWordClass );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswEnumNbtModuleName.CISPro, SignalWordOC.ObjectClassId );
            CswNbtMetaDataObjectClass ClassificationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSClassificationClass );
            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswEnumNbtModuleName.CISPro, ClassificationOC.ObjectClassId );
        } // update()

    }//class CswUpdateMetaData_02K_Case31843

}//namespace ChemSW.Nbt.Schema