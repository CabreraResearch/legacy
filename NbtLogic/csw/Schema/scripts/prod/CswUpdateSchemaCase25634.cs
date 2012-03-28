using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 25634
    /// </summary>
    public class CswUpdateSchemaCase25634 : CswUpdateSchemaTo
    {

        public override void update()
        {
            CswNbtMetaDataObjectClass ReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass );
            foreach( CswNbtNode ReportNode in ReportOC.getNodes( false, true ) )
            {
                CswNbtObjClassReport ReportNodeAsReport = CswNbtNodeCaster.AsReport( ReportNode );
                if( ReportNodeAsReport.ReportName.Text == "OOC Issues last 30 days" )
                {
                    ReportNodeAsReport.SQL.Text = @"select dept.P17 as Department,
                                                           targ.P_LOCATION as TargetLocation,
                                                           targ.nodeid as TargetNodeId,
                                                           targ.P_BARCODE as TargetBarcode,
                                                           targ.P_DESCRIPTION as TargetDesc,
                                                           targ.P_STATUS as TargetStatus,
                                                           des.P_NAME as InspectionDescription,
                                                           q.questionno,
                                                           q.question,
                                                           q.iscompliant,
                                                           q.answer,
                                                           q.correctiveaction,
                                                           q.comments
                                                      from ocinspectiondesignclass des
                                                      join ocinspectiontargetclass targ on targ.nodeid = des.P_TARGET_FK
                                                      join vwquestiondetail q on q.nodeid = des.nodeid
                                                      left outer join oclocationclass room on room.nodeid = targ.P_Location_fk
                                                      left outer join ntdepartment dept on dept.nodeid = room.P_DEPARTMENT_FK
                                                     where (q.iscompliant = '0' or q.correctiveaction is not null)
                                                     order by targ.P_LOCATION, des.nodeid, q.questionno";

                    ReportNodeAsReport.postChanges( false );
                }
            }



        }//Update()


    }//class CswUpdateSchemaCase25634
}//namespace ChemSW.Nbt.Schema