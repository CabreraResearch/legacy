using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02M_CIS52432 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 52432; }
        }

        public override string Title
        {
            get { return "Rename GHSSignalWord to GHSSignalWordClass"; }
        }

        public override string AppendToScriptName()
        {
            return "";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass GHSSignalWordOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( "GHSSignalWord" );
            if( null != GHSSignalWordOC )
            {
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update object_class set objectclass = '" + CswEnumNbtObjectClass.GHSSignalWordClass + "' where objectclassid = " + GHSSignalWordOC.ObjectClassId );
            }
        }
    }
}