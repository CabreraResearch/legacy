//using System;
//using System.Data;
//using System.Collections;
//using System.Text.RegularExpressions;
////using ChemSW.TblDn;
////using ChemSW.RscAdo;
//using ChemSW.DB;
//using ChemSW.Exceptions;
//using ChemSW.Nbt;

//namespace ChemSW.Nbt.TableEvents
//{

//    public class CswNbtTableEventBeforeUpdateSequences
//    {

//        public CswNbtTableEventBeforeUpdateSequences()
//        {
//        }//ctor()

//        //private ICswTableCaddyFactory _CswTableCaddyFactory = null;
//        public void BeforeUpdateHandler(Object sender, CswTableModEventArgs CswTableModEventArgs)
//        {

//        }//DataTableRowChangedEventHandler

//        private void _validateRow( DataRow SequencesRow )
//        {
//            if( SequencesRow.IsNull( "sequencename" ) )
//                throw ( new CswDniException( "A data error occurred", "The SEQUENCES record with SEQUENCEID " + SequencesRow["sequenceid"].ToString() + " is missing a value for column SEQUENCE" ) );
//        }//_validateRow

//        private bool _isSequenceInUse( string SequenceName )
//        {
//            bool ReturnVal = false;

//            return( ReturnVal );

//        }//_isSequenceInUseElsewhere()

//    }//class CswNbtTableEventBeforeUpdateSequences

//}//namespace ChemSW.Nbt.TableEvents


