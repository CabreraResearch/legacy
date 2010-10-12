//using System;
//using System.Data;
//using System.Collections;
//using System.Text.RegularExpressions;
////using ChemSW.TblDn;
////using ChemSW.RscAdo;
//using ChemSW.Exceptions;
//using ChemSW.Nbt;
//using ChemSW.Core;
//using ChemSW.Audit;
//using ChemSW.DB;

//namespace ChemSW.Nbt.TableEvents
//{

//    public class CswNbtTableEventAfterUpdateAuditing
//    {

//        private ICswAuditRecorder _CswAuditor = null;
//        public CswNbtTableEventAfterUpdateAuditing( ICswAuditRecorder CswAuditor )
//        {
//            _CswAuditor = CswAuditor;
//        }//ctor()
        
//        //private ICswTableCaddyFactory _CswTableCaddyFactory = null;
//        public void AfterUpdateHandler(Object sender, CswTableModEventArgs CswTableModEventArgs)
//        {
        
//            _CswAuditor.InsertRecords = CswTableModEventArgs.InsertedRows;
//            _CswAuditor.UpdateRecords = CswTableModEventArgs.UpdatedRows;
//            // BZ 4899
//            //_CswAuditor.DeleteRecords = CswTableModEventArgs.DeletedRows;

//        }//DataTableRowChangedEventHandler

//    }//class CswNbtTableEventAfterUpdateAuditing

//}//namespace ChemSW.Nbt.TableEvents


