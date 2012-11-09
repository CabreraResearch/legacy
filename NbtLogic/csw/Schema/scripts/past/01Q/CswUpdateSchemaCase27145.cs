using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27145
    /// </summary>
    public class CswUpdateSchemaCase27145 : CswUpdateSchemaTo
    {
        public override void update()
        {
            string reportSqlText = _getLab1DeficienciesReportSQLText();

            if( false == String.IsNullOrEmpty( reportSqlText ) )
            {
                CswNbtMetaDataObjectClass ReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass );
                foreach( CswNbtNode ReportNode in ReportOC.getNodes( true, false ) )
                {
                    CswNbtObjClassReport NodeAsReport = ReportNode;
                    if( NodeAsReport.ReportName.Text == "Lab 1 Deficiencies" )
                    {
                        NodeAsReport.SQL.Text = reportSqlText;
                        NodeAsReport.postChanges( false );
                    }
                }
            }
        }//Update()

        private string _getLab1DeficienciesReportSQLText()
        {
            string reportSqlText = string.Empty;

            #region SQL Query Template
            string sqlText = @"select des.P{0} as InspectionDate, 
                                    des.P{1} as InspectionName,
                                    des.P{2} as Location,
                                    CASE nvl(q.correctiveaction,'NULL')
                                      WHEN 'NULL' then 'NO'
                                      ELSE 'yes'
                                    END as Resolved,
                                    q.questionno,
                                    q.question,
                                    q.answer,
                                    q.correctiveaction,
                                    q.comments
                                    from ntlabsafetychecklistdemo des
                                    join ntlabsafetydemo targ on targ.nodeid=des.P{3}_labsafetydemo_ntfk
                                    join vwquestiondetail q on q.nodeid = des.nodeid
                                      where (q.iscompliant = '0' or q.correctiveaction is not null)
                                        and des.P{2} like '%> Lab 1'
                                    order by des.P{2}, des.P{0}, q.questionno";
            #endregion

            CswNbtMetaDataNodeType LabSafetyChecklistDemoNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Lab Safety Checklist (demo)" );
            if( LabSafetyChecklistDemoNT != null )
            {
                CswNbtMetaDataNodeTypeProp InspectionDateNTP = LabSafetyChecklistDemoNT.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.InspectionDatePropertyName );
                CswNbtMetaDataNodeTypeProp InspectionNameNTP = LabSafetyChecklistDemoNT.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.NamePropertyName );
                CswNbtMetaDataNodeTypeProp LocationNTP = LabSafetyChecklistDemoNT.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.LocationPropertyName );
                CswNbtMetaDataNodeTypeProp TargetNTP = LabSafetyChecklistDemoNT.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.TargetPropertyName );

                reportSqlText = string.Format( sqlText,
                     InspectionDateNTP.PropId.ToString(),//P4625
                     InspectionNameNTP.PropId.ToString(),//P4615
                     LocationNTP.PropId.ToString(),//P4621
                     TargetNTP.PropId.ToString()//P4614_labsafetydemo_ntfk
                    );
            }

            return reportSqlText;
        }

    }//class CswUpdateSchemaCase27145

}//namespace ChemSW.Nbt.Schema