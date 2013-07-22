using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 30090
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02D_Case30126 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30126; }
        }

        public override void update()
        {
            // Remove the C3SyncDate Property from the Material Property Set
            CswNbtMetaDataPropertySet MaterialPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
            foreach( CswNbtMetaDataObjectClass CurrentObjectClass in MaterialPS.getObjectClasses() )
            {
                CswNbtMetaDataObjectClassProp C3SyncDateOCP = CurrentObjectClass.getObjectClassProp( "C3SyncDate" );
                if( null != C3SyncDateOCP )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.DeleteJctPropertySetOcPropRow( C3SyncDateOCP );
                    _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassPropDeprecated( C3SyncDateOCP, true );
                }
            }

        } // update()

    }//class RunBeforeEveryExecutionOfUpdater_02D_Case30126

}//namespace ChemSW.Nbt.Schema