using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
using ChemSW.DB;
using ChemSW.Core;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Test Case: 001, part 01
    /// </summary>
    public class CswTstCaseRsrc_012
    {

		private CswTestCaseRsrc _CswTestCaseRsrc;
		private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
		public CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn
		{
			set
			{
				_CswNbtSchemaModTrnsctn = value;
				_CswTestCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			}
		}

        public static string Purpose = "Rollback stale column value";

        public string FakeTestTableName { get { return ( _CswTestCaseRsrc.getFakeTestTableName( TestTableNamesFake.TestTable01 ) ); } }
        public string FakeValColumnName { get { return ( _CswTestCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ) ); } }
        public string FakePkColumnName { get { return ( FakeTestTableName + "id" ); } }

        public string LocalAribtiraryValue { get { return ( this.GetType().Name + "_testvalue" ); } }


        public string Val_Row_1 { get { return( "eenie"); } } 
        public string Val_Row_2 { get { return( "meeny"); } } 
        public string Val_Row_3 { get { return( "minie"); } }

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
