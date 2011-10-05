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
    public class CswTstCaseRsrc_013
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


        public static string Purpose = "Reject stale column value";

        public string FakeTestTableName { get { return ( _CswTestCaseRsrc.getFakeTestTableName( TestTableNamesFake.TestTable01 ) ); } }
        public string FakeValColumnName01 { get { return ( _CswTestCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ) ); } }
        public string FakeValColumnName02 { get { return ( _CswTestCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn02 ) ); } }
        public string FakePkColumnName { get { return ( FakeTestTableName + "id" ); } }

        public string LocalAribtiraryValue01 { get { return ( this.GetType().Name + "1" ); } }
        public string LocalAribtiraryValue01Delta { get { return ( this.GetType().Name + "2" ); } }
        public string LocalAribtiraryValue02 { get { return ( this.GetType().Name + "X" ); } }
        public string LocalAribtiraryValue02Delta { get { return ( this.GetType().Name + "Y" ); } }


    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
