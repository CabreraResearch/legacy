//using ChemSW.RscAdo;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Test Case: 001, part 01
    /// </summary>
    public class CswTstCaseRsrc_011
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


        public static string Purpose = "Update same table after rollback of insert";

        public string FakeTestTableName { get { return ( _CswTestCaseRsrc.getFakeTestTableName( TestTableNamesFake.TestTable01 ) ); } }
        public string FakeTestColumnName { get { return ( _CswTestCaseRsrc.getFakeTestColumnName( TestColumnNamesFake.TestColumn01 ) ); } }

        public string LocalAribtiraryValue { get { return ( this.GetType().Name + "_testvalue" ); } }


    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
