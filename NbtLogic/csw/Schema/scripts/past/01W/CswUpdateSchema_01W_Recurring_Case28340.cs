using ChemSW.MtSched.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Sched;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_01W_Recurring_Case28340: CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 27882; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass RequestMaterialDispenseOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RequestMaterialDispenseClass );
            CswNbtMetaDataObjectClassProp IsRecurringOcp = RequestMaterialDispenseOc.getObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.IsRecurring );
            foreach( CswNbtMetaDataNodeTypeProp RecurringNtp in IsRecurringOcp.getNodeTypeProps() )
            {
                RecurringNtp.removeFromAllLayouts(); 
            }

            CswNbtMetaDataObjectClass RequestOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RequestClass );
            CswNbtMetaDataObjectClassProp IsRecurringReqOcp = RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.IsRecurring );
            foreach( CswNbtMetaDataNodeTypeProp RecurringNtp in IsRecurringReqOcp.getNodeTypeProps() )
            {
                RecurringNtp.removeFromAllLayouts();
            }

            _CswNbtSchemaModTrnsctn.createScheduledRule( NbtScheduleRuleNames.GenRequest, Recurrence.NSeconds, 5 );

        } //Update()

    }//class CswUpdateSchema_01V_CaseXXXXX

}//namespace ChemSW.Nbt.Schema