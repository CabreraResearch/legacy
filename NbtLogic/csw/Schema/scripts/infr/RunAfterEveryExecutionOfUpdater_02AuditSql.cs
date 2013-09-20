using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using ChemSW.Audit;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.RscAdo;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for PL/SQL object changes
    /// </summary>
    public class RunAfterEveryExecutionOfUpdater_02AuditSql : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author { get { return CswEnumDeveloper.SS; } }
        public override int CaseNo { get { return 30702; } }
        public override bool AlwaysRun { get { return true; } }
        public override string Title { get { return "Post-Script: 04: Auditing PL/SQL Objects"; } }
        public override string ScriptName { get { return "Audit Sql"; } }

        public override void update()
        {
            foreach( string Abbrev in CswNbtAuditTableAbbreviation.Abbreviations )
            {
                ICswDataDictionaryReader DataDictionary = _CswNbtSchemaModTrnsctn.CswDataDictionary;
                CswAuditMetaData CswAuditMetaData = new CswAuditMetaData();
                string AuditTable = CswNbtAuditTableAbbreviation.getAuditTableName( Abbrev );
                string RealTable = CswAuditMetaData.makeNameOfAuditedTable( AuditTable );
                string AuditTablePk = DataDictionary.getPrimeKeyColumn( AuditTable );
                string RealTablePk = DataDictionary.getPrimeKeyColumn( RealTable );

                string ObjectType = "CSW_" + Abbrev + @"_OBJ_TYPE";
                string TableType = "CSW_" + Abbrev + @"_TABLE_TYPE";
                string FuncName = CswNbtAuditTableAbbreviation.getAuditLookupFunctionName( AuditTable );

                CswCommaDelimitedString Columns = new CswCommaDelimitedString();
                CswCommaDelimitedString ObjectTypeColumns = new CswCommaDelimitedString();
                foreach( string ColumnName in _CswNbtSchemaModTrnsctn.CswDataDictionary.getColumnNames( RealTable ) )
                {
                    if( ColumnName != "auditlevel" )
                    {
                        DataDictionary.setCurrentColumn( RealTable, ColumnName );
                        string OracleType = CswDbVendorOpsOracle._getOracleDataTypeFromPortableDataType( DataDictionary.PortableDataType, DataDictionary.DataTypeSize );
                        Columns.Add( ColumnName );
                        ObjectTypeColumns.Add( ColumnName + " " + OracleType );
                    }
                }
                //Columns.Add( "recordcreated" );
                ObjectTypeColumns.Add( "recordcreated date" );

                string ObjectTypeSql = @"CREATE OR REPLACE TYPE " + ObjectType + " IS OBJECT (" + ObjectTypeColumns.ToString( false ) + ");";

                string TableTypeSql = @"CREATE OR REPLACE TYPE " + TableType + " AS TABLE OF " + ObjectType + ";";

                string FuncSql = @"create or replace function " + FuncName + @" (AsOfDate in Date) return " + TableType + @" is
                                      ResultTable " + TableType + @";
                                    begin

                                        with audit1 as (select " + AuditTablePk + @", " + RealTablePk + @", auditeventtype
                                              from " + AuditTable + @" a
                                             where a.recordcreated = (select max(recordcreated)
                                                                                   from " + AuditTable + @" a2
                                                                                  where a2.recordcreated <= AsOfDate
                                                                                    and a2." + RealTablePk + @" = a." + RealTablePk + @"))
                                        select " + ObjectType + @"(" + Columns.ToString( false ) + @",recordcreated) BULK COLLECT into ResultTable
                                        from (select " + Columns.ToString( false ) + @", recordcreated 
                                                from " + AuditTable + @"
                                               where " + AuditTablePk + @" in (select " + AuditTablePk + @" from audit1)
                                                 and auditeventtype <> 'PhysicalDelete'
                                             union all
                                              select " + Columns.ToString( false ) + @", sysdate as recordcreated 
                                                from " + RealTable + @" 
                                               where " + RealTablePk + @" not in (select " + RealTablePk + @" from audit1));

                                      RETURN ResultTable;

                                    EXCEPTION
                                      WHEN OTHERS THEN
                                        ResultTable.DELETE;
                                        RETURN ResultTable;
                                    end " + FuncName + @";";

                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql(
                        @"declare
                          object_not_exists EXCEPTION;
                          PRAGMA EXCEPTION_INIT(object_not_exists, -04043);
                        begin
                          execute immediate 'drop type " + TableType + @" force';
                        exception
                          when object_not_exists then null;
                        end;"
                    );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql(
                        @"declare
                          object_not_exists EXCEPTION;
                          PRAGMA EXCEPTION_INIT(object_not_exists, -04043);
                        begin
                          execute immediate 'drop type " + ObjectType + @" force';
                        exception
                          when object_not_exists then null;
                        end;"
                    );

                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( ObjectTypeSql );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( TableTypeSql );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( FuncSql );

            } // foreach
        } //update()
    }//class RunAfterEveryExecutionOfUpdater_02AuditSql
}//namespace ChemSW.Nbt.Schema


