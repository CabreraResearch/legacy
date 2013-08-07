using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for making the missing NodeTypeProps
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_MakeMissingNodeTypeProps: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 0; }
        }

        public override void update()
        {

            //This script must always come after the RunBefore Milestone scripts
            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();

        } // update()


    }//class CswUpdateSchema_02C_MakeMissingNodeTypeProps

}//namespace ChemSW.Nbt.Schema