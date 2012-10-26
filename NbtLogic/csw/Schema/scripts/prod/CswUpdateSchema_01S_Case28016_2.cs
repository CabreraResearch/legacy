﻿using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28016 part 2
    /// </summary>
    public class CswUpdateSchemaCase28016_2 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // Set runtime hidden for existing Hourly reports and Schedules
            
            CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );
            foreach( CswNbtObjClassMailReport MailReportNode in MailReportOC.getNodes( false, true ) )
            {
                MailReportNode.RunTime.setHidden( 
                    value: ( MailReportNode.DueDateInterval.RateInterval.RateType == CswRateInterval.RateIntervalType.Hourly ), 
                    SaveToDb: true 
                );
            } // foreach( CswNbtObjClassMailReport MailReportNode in MailReportOC.getNodes( false, true ) )

            CswNbtMetaDataObjectClass GeneratorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
            foreach( CswNbtObjClassGenerator GeneratorNode in GeneratorOC.getNodes( false, true ) )
            {
                GeneratorNode.RunTime.setHidden(
                    value: ( GeneratorNode.DueDateInterval.RateInterval.RateType == CswRateInterval.RateIntervalType.Hourly ),
                    SaveToDb: true
                );
            } // foreach( CswNbtObjClassGenerator GeneratorNode in GeneratorOC.getNodes( false, true ) )
        } //Update()

    }//class CswUpdateSchemaCase28016_2

}//namespace ChemSW.Nbt.Schema