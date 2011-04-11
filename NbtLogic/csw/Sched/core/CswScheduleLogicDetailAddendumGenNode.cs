using System;
using System.Data;
using System.Threading;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Config;
using ChemSW.DB;
using ChemSW.MtSched.Core;
using ChemSW.Nbt;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicDetailAddendumGenNode : CswScheduleLogicDetailAddendum
    {

        CswNbtResources _CswNbtResources = null;
        public CswScheduleLogicDetailAddendumGenNode( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }//ctor

        public override void read( CswScheduleLogicDetail CswScheduleLogicDetail )
        {
        }//read() 

        public override void write( CswScheduleLogicDetail CswScheduleLogicDetail )
        {
        }//write() 

    }//CswScheduleLogicDetailAddendumGenNode

}//namespace ChemSW.Nbt.Sched


