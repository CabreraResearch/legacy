
namespace ChemSW.Nbt.Sched
{

    public class CswNbtMailReportStatus
    {
        public bool ReportReadyForQuery
        {
            get
            {
                return ( string.Empty != Link );

            }//get
        }//ReportReadyForQuery

        public bool ReportDataExist = false;

        public bool EmailWasSent = false;
        public string Link = string.Empty;
        public string ReportReason = string.Empty;
        public string ReportNotMadeReason = string.Empty;
        public string ReportFailureReason = string.Empty;
        public string EmailSentReason = string.Empty;
        public string EmailNotSentReason = string.Empty;
        public string EmailFailureReason = string.Empty;
        private string _LineEnd = "; ";
        public string Message
        {
            get
            {
                string ReturnVal = string.Empty;

                if ( string.Empty != ReportReason )
                {
                    ReturnVal = ReportReason + _LineEnd;

                    if ( string.Empty != EmailSentReason )
                    {
                        //ReturnVal += EmailSentReason + _LineEnd;
                        ReturnVal += EmailSentReason;
                    }
                    else if ( string.Empty != EmailNotSentReason )
                    {
                        //ReturnVal += EmailNotSentReason + _LineEnd;
                        ReturnVal += EmailNotSentReason;
                    }

                    if ( string.Empty != EmailFailureReason )
                    {
                        ReturnVal += EmailFailureReason;
                        //ReturnVal += EmailFailureReason + _LineEnd;
                    }//
                        
                }
                else if ( string.Empty != ReportNotMadeReason ) 
                {
                    ReturnVal = "No report was created because: " + ReportNotMadeReason + _LineEnd;
                }
                else if ( string.Empty != ReportFailureReason )
                {
                    ReturnVal = "The report failed because: " + ReportFailureReason + _LineEnd;
                }//if-else there was a report

                return ( ReturnVal );
            }//
        }//Message

    }//CswNbtMailReportStatus 

}//namespace ChemSW.Nbt.Sched
