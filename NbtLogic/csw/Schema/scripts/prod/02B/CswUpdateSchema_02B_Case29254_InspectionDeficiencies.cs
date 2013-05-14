using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_02B_Case29254_InspectionDeficiencies : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.PG; }
        }

        public override int CaseNo
        {
            get { return 29254; }
        }

        public override void update()
        {

            CswNbtMetaDataNodeType ReportsNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Report" );
            if( null != ReportsNodeType )
            {

                foreach( CswNbtObjClassReport CurrentReport in ReportsNodeType.getNodes( false, true ) )
                {
                    if( "Deficient Inspections" == CurrentReport.NodeName )
                    {
                        CurrentReport.ReportName.Text = "Deficient Inspections (Demo)";
                        CurrentReport.IsDemo = true;
                        CurrentReport.postChanges( true );
                        break;
                    }
                }
            }


            //CswNbtObjClassReport CswNbtObjClassReport = _CswNbtSchemaModTrnsctn.obj
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update node_views set isdemo='0'" );


        } // update()

    }//class CswUpdateSchema_02B_Case29254_InspectionDeficiencies

}//namespace ChemSW.Nbt.Schema