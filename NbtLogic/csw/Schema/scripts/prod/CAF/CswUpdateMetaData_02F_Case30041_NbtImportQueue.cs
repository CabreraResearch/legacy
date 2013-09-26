using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.Sched;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for OC changes
    /// </summary>
    public class CswUpdateMetaData_02F_Case30041_NbtImportQueue : CswUpdateSchemaTo
    {
        public override string Title { get { return "Pre-Script: Case 30041: DDL for NbtImportQueue"; } }

        public override string ScriptName
        {
            get { return ""; }
        }

        #region Blame Logic

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 30041; }
        }

        #endregion Blame Logic

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            string ExceptionText = string.Empty;
            if( _CswNbtSchemaModTrnsctn.IsDbLinkConnectionHealthy( CswScheduleLogicNbtCAFImport.CAFDbLink, ref ExceptionText ) )
            {

                CswArbitrarySelect ArbTableSelect = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "nbtimportqueue_table_check", "select table_name from user_tables@" + CswScheduleLogicNbtCAFImport.CAFDbLink + " where lower(table_name)='nbtimportqueue'" );
                DataTable CafTable = ArbTableSelect.getTable();
                if( CafTable.Rows.Count == 0 )
                {
                    throw new CswDniException( "The nbtimportqueue table is missing." );
                }
                else
                {
                    CswArbitrarySelect ArbColumnSelect = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "nbtimportqueue_column_check", "select lower(column_name) col from user_tab_columns@" + CswScheduleLogicNbtCAFImport.CAFDbLink + " where lower(table_name)='nbtimportqueue'" );
                    DataTable CafColumns = ArbColumnSelect.getTable();

                    CswCommaDelimitedString ActualColumns = new CswCommaDelimitedString();
                    foreach( DataRow Row in CafColumns.Rows )
                    {
                        string ColumnName = CswConvert.ToString( Row["col"] );
                        ActualColumns.Add( ColumnName );
                    }

                    CswCommaDelimitedString FailedOnColumns = new CswCommaDelimitedString();
                    CswCommaDelimitedString ExpectedColumns = new CswCommaDelimitedString() { "nbtimportqueueid", "state", "itempk", "tablename", "priority", "errorlog", "viewname" };
                    foreach( string ExpectedColumn in ExpectedColumns.Where( ExpectedColumn => false == ActualColumns.Contains( ExpectedColumn ) ) )
                    {
                        FailedOnColumns.Add( ExpectedColumn );
                    }

                    if( FailedOnColumns.Count > 0 )
                    {
                        throw new CswDniException( "The nbtimportqueue is expected to contain these columns {" + ExpectedColumns.ToString() + "}, but the following columns were missing {" + FailedOnColumns.ToString() + "}." );
                    }

                }
            }
        }

    }//class RunBeforeEveryExecutionOfUpdater_02F_Case30281
}//namespace ChemSW.Nbt.Schema


