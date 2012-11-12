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
            #region Report SQL Text
            string reportSqlText = @"select des.P4625 as InspectionDate, 
                                    des.P4615 as InspectionName,
                                    des.P4621 as Location,
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
                                    join ntlabsafetydemo targ on targ.nodeid=des.P4614_labsafetydemo_ntfk
                                    join vwquestiondetail q on q.nodeid = des.nodeid
                                      where (q.iscompliant = '0' or q.correctiveaction is not null)
                                        and des.P4621 like '%> Lab 1'
                                    order by des.P4621, des.P4625, q.questionno";
            #endregion

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

        }//Update()

    }//class CswUpdateSchemaCase27145

}//namespace ChemSW.Nbt.Schema