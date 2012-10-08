using ChemSW.Nbt.csw.Dev;
using ChemSW.DB;
using System.Data;
using ChemSW.Nbt.Sched;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case CswUpdateSchema_01T_Case27880
    /// </summary>
    public class CswUpdateSchema_01T_Case27880 : CswUpdateSchemaTo
    {
        public override void update()
        {

            //add the expired containers scheduled rule to the scheduledrules table
            CswTableUpdate tu = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "expiredContainerSchedRule_27880", "scheduledrules" );
            DataTable scheduledrules = tu.getTable();
            DataRow expiredContainersRow = scheduledrules.NewRow();
            expiredContainersRow["rulename"] = "ExpiredContainers";
            expiredContainersRow["recurrence"] = "Daily";
            expiredContainersRow["interval"] = "1";
            expiredContainersRow["disabled"] = CswConvert.ToDbVal( false );
            expiredContainersRow["reprobatethreshold"] = "3";
            expiredContainersRow["maxruntimems"] = "300000";
            scheduledrules.Rows.Add( expiredContainersRow );
            tu.update( scheduledrules );

        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 27880; }
        }

        //Update()

    }//class CswUpdateSchema_01T_Case27880

}//namespace ChemSW.Nbt.Schema