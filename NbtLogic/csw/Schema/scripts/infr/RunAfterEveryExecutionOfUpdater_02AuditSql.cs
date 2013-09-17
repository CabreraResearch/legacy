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
            Dictionary<string, string> AuditTables = new Dictionary<string, string>()
                {
                    {"bda", "blob_data_audit"},
                    {"jnpa", "jct_nodes_props_audit"},
                    {"laa", "license_accept_audit"},
                    {"na", "nodes_audit"},
                    {"nta", "nodetypes_audit"},
                    {"ntpa", "nodetype_props_audit"},
                    {"ntta", "nodetype_tabset_audit"},
                    {"nva", "node_views_audit"},
                    {"oca", "object_class_audit"},
                    {"ocpa", "object_class_props_audit"}
                };

            foreach( string Abbrev in AuditTables.Keys )
            {
                ICswDataDictionaryReader DataDictionary = _CswNbtSchemaModTrnsctn.CswDataDictionary;
                CswAuditMetaData CswAuditMetaData = new CswAuditMetaData();
                string AuditTable = AuditTables[Abbrev];
                string RealTable = CswAuditMetaData.makeNameOfAuditedTable( AuditTable );
                string RealTablePk = DataDictionary.getPrimeKeyColumn( RealTable );

                string ObjectType = "CSW_" + Abbrev + @"_OBJ_TYPE";
                string TableType = "CSW_" + Abbrev + @"_TABLE_TYPE";
                string FuncName = "AuditLookup_" + Abbrev;

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

                string FuncSql = @"create or replace function " + FuncName + @" (AsOfDate in Date, PrimaryKey in Number) return " + TableType + @" is
                                      ResultTable " + TableType + @";
                                      CURSOR audit1 RETURN " + ObjectType + @" is(
                                        select " + Columns.ToString( false ) + @", recordcreated
                                          from " + AuditTable + @"
                                         where " + RealTablePk + @" = PrimaryKey
                                           and recordcreated = (select max(recordcreated)
                                                                       from " + AuditTable + @"
                                                                      where recordcreated <= AsOfDate
                                                                        and " + RealTablePk + @" = PrimaryKey));
                                    begin

                                      open audit1;
                                      if audit1%notfound then
                                        SELECT " + ObjectType + @"(" + Columns.ToString( false ) + @",recordcreated) BULK COLLECT
                                          INTO ResultTable
                                          FROM (select " + Columns.ToString( false ) + @", sysdate as recordcreated 
                                                  from " + RealTable + @" where " + RealTablePk + @" = PrimaryKey);
                                      else
                                        SELECT " + ObjectType + @"(" + Columns.ToString( false ) + @",recordcreated) BULK COLLECT
                                          INTO ResultTable
                                          FROM (select " + Columns.ToString( false ) + @", recordcreated
                                                  from " + AuditTable + @"
                                                 where " + RealTablePk + @" = PrimaryKey
                                                   and recordcreated = (select max(recordcreated)
                                                                          from " + AuditTable + @"
                                                                         where recordcreated <= AsOfDate
                                                                           and " + RealTablePk + @" = PrimaryKey)) p;
                                      end if;

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


