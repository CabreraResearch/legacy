using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28513
    /// </summary>
    public class CswUpdateSchema_01W_Case28513 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 28513; }
        }

        public override void update()
        {
            // Add (demo) to Lab 1 Deficiencies Report
            CswNbtMetaDataObjectClass ReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ReportClass );
            foreach( CswNbtObjClassReport ReportNode in ReportOC.getNodes( false, false ) )
            {
                if( ReportNode.ReportName.Text == "Lab 1 Deficiencies" )
                {
                    ReportNode.ReportName.Text = "Lab 1 Deficiencies (demo)";
                    ReportNode.postChanges( false );
                }
            }

        } //Update()

    }//class CswUpdateSchema_01V_Case28513

}//namespace ChemSW.Nbt.Schema