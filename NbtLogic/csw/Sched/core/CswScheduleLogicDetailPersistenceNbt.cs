using System;
using System.Data;
using System.Threading;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Config;
using ChemSW.DB;
using ChemSW.MtSched.Core;

namespace ChemSW.Cis.Sched
{
    public class CswScheduleLogicDetailPersistenceNbt : ICswScheduleLogicDetailPersistence
    {
        public CswScheduleLogicDetailPersistenceNbt()
        {
        }//ctor

        private ICswResources _CswResources = null;
        public void init( ICswResources CswResources )
        {
            _CswResources = CswResources;
        }//init() 

        public CswScheduleLogicDetail read( string RuleName )
        {

            CswScheduleLogicDetail ReturnVal = new CswScheduleLogicDetail();


            //foreach( DataRow CurrentRow in DataTableBgTasksInfo.Rows )
            //{
            //    ReturnVal.RunParams.Add( CurrentRow[_ColName_ParameterName].ToString(), CurrentRow[_ColName_ParameterValue] );
            //}

            return ( ReturnVal );

        }//read

        public void write( CswScheduleLogicDetail CswScheduleLogicDetail )
        {


        }//write() 

    }//CswScheduleLogicDetailPersistenceCis

}//namespace ChemSW.MtSched


