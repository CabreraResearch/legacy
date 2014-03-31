using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02M_CIS53175 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 53175; }
        }

        public override string Title
        {
            get { return "Delete old LegacyId NTPs and OCPs"; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {
            foreach( CswNbtMetaDataObjectClass OC in _CswNbtSchemaModTrnsctn.MetaData.getObjectClasses() )
            {
                CswNbtMetaDataObjectClassProp LegacyIdOCP = OC.getObjectClassProp( "Legacy Id" );
                if( null != LegacyIdOCP )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( LegacyIdOCP, true );
                }
            }
        }
    }
}