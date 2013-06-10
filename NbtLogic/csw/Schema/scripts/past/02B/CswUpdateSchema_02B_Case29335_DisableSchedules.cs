using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropertySets;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_02B_Case29335_DisableSchedules: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 29335; }
        }

        public override void update()
        {
            if( _CswNbtSchemaModTrnsctn.isMaster() )
            {
                CswNbtMetaDataPropertySet SchedulerSet = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.SchedulerSet );
                foreach( CswNbtMetaDataObjectClass ScheduleOc in SchedulerSet.getObjectClasses() )
                {
                    foreach( CswNbtNode Schedule in ScheduleOc.getNodes( forceReInit : false, includeSystemNodes : false ) )
                    {
                        ICswNbtPropertySetScheduler ISchedule = CswNbtPropSetCaster.AsPropertySetScheduler( Schedule );
                        ISchedule.Enabled.Checked = CswEnumTristate.False;
                        Schedule.postChanges( ForceUpdate: false );
                    }
                }

            }
        } // update()

    }//class CswUpdateSchema_02B_CaseXXXXX

}//namespace ChemSW.Nbt.Schema