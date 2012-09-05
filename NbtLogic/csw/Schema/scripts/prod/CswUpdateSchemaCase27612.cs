
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchemaCase27612 : CswUpdateSchemaTo
    {
        public override void update()
        {
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( @"insert into scheduledruleparams
                                                                          (scheduledruleparamid, scheduledruleid, paramname, paramval)
                                                                       values (
                                                                      (select max(scheduledruleparamid) from scheduledruleparams) + 1,
                                                                      (select scheduledruleid
                                                                         from scheduledrules
                                                                        where rulename = 'UpdtPropVals'), 'NodesPerCycle', '25'
                                                                        )" );
        }//Update()

    }//class CswUpdateSchemaCase27612

}//namespace ChemSW.Nbt.Schema