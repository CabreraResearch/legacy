using System;
using System.Collections;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;
using ChemSW.MtSched.Core;

namespace ChemSW.Nbt.Sched
{

    public class CswScheduleLogicNbtGenEmailRpt : ICswScheduleLogic
    {

        public string RuleName
        {
            get { throw new NotImplementedException(); }
        }

        public bool doesItemRunNow()
        {
            throw new NotImplementedException();
        }

        public LogicRunStatus LogicRunStatus
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string CompletionMessage
        {
            get { throw new NotImplementedException(); }
        }

        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { throw new NotImplementedException(); }
        }

        public void init( ICswResources RuleResources, CswScheduleLogicDetail CswScheduleLogicDetail )
        {
            throw new NotImplementedException();
        }

        public void threadCallBack()
        {
            throw new NotImplementedException();
        }

        public void stop()
        {
            throw new NotImplementedException();
        }

        public void reset()
        {
            throw new NotImplementedException();
        }

        public void releaseResources()
        {
            throw new NotImplementedException();
        }
    }//CswScheduleLogicNbtGenEmailRpt

}//namespace ChemSW.Nbt.Sched
