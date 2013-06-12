using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29428
    /// </summary>
    public class CswUpdateSchema_02C_Case29428 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 29428; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            foreach( CswNbtMetaDataNodeType LocationNT in LocationOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeTab LocationSettingsNTT = LocationNT.getNodeTypeTab( "Location Settings" );
                if( null != LocationSettingsNTT )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeTab( LocationSettingsNTT );
                }
            }

        } // update()

    }//class CswUpdateSchema_02C_Case29428

}//namespace ChemSW.Nbt.Schema