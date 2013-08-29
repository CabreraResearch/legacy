
using System.IO;
using System.Windows.Forms;
using ChemSW.Nbt.Sched;

namespace ChemSW.Nbt.csw.Schema
{
    partial class CswNbtSchemaUpdateImportMgr
    {
        private string ImportTable { get { return "nbtimportqueue@" + _CAFDbLink; } }
        private string ImportSequence { get { return "seq_nbtimportqueueid.nextval@" + _CAFDbLink; } }
        private string SourceTable { get { return _SourceTableName + "@" + _CAFDbLink; } }

        private void _populateImportQueueTable( string WhereClause )
        {
            string State = CswScheduleLogicNbtCAFImport.State.N.ToString();

            //Optional extension to where clause. Logical deletes already excluded.
            WhereClause = WhereClause ?? string.Empty;
            if( false == string.IsNullOrEmpty( WhereClause ) && false == WhereClause.Trim().StartsWith( "and" ) )
            {
                WhereClause = " and " + WhereClause;
            }

            //Populate the import queue
            string SqlText = "insert into " + ImportTable + " ( nbtimportqueueid, state, itempk, tablename, priority, errorlog, viewname ) " +
                             @" select " + ImportSequence + ", '" + State + "', " + SourceTablePkColumnName + ", '" + _SourceTableName + "',0, '', '" + _ViewName + "' from " + SourceTable + " where deleted='0' " + WhereClause;
            SchemaModTrnsctn.execArbitraryPlatformNeutralSql( SqlText );
        }

        private string TriggerName
        {
            get
            {
                string Ret = "TRG_IMPRT_" + _SourceTableName;
                if( Ret.Length > 30 )
                {
                    Ret = Ret.Substring( 0, 30 );
                }
                return Ret;
            }
        }

        private void _createTriggerOnImportTable()
        {

#if DEBUG

            _SourceColumns.Add( "deleted", IsUnique: true );
            string ColumnNames = _SourceColumns.ToString();
            string Trigger = @"CREATE OR REPLACE TRIGGER " + TriggerName + " " +
                             @"AFTER INSERT OR DELETE OR UPDATE OF " + ColumnNames + " ON " + SourceTable + " " +
                             @"FOR EACH ROW 
                                BEGIN
  
                                IF INSERTING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'I', :new." + SourceTablePkColumnName + ", '" + SourceTable + "', '', '');" + " " +
    
                             @" ELSIF DELETING THEN
                                    INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old." + SourceTablePkColumnName + ", '" + SourceTable + "', '', '');" + " " +
  
                             @" ELSE
                                    IF :old.deleted = '0' THEN
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'D', :old." + SourceTablePkColumnName + ", '" + SourceTable + "', '', '');" + " " +
                             @"     ELSE
                                        INSERT INTO nbtimportqueue VALUES (seq_nbtimportqueueid.NEXTVAL, 'U', :old." + SourceTablePkColumnName + ", '" + SourceTable + "', '', '');" + " " +
                             @"     END IF
    
                                END IF;
  
                                END;";

            //string Ddl = "execDdl@" + _CAFDbLink + "(" + Trigger + ")";
            //SchemaModTrnsctn.execStoredProc( "execDdl@" + _CAFDbLink, new List<CswStoredProcParam>() {new CswStoredProcParam( "ddlOp", Trigger, CswEnumDataDictionaryPortableDataType.String )} );

            using( StreamWriter TriggerStream = new StreamWriter( Application.StartupPath + "..\\..\\..\\..\\Scripts\\cafsql\\" + TriggerName + ".sql" ) )
            {
                TriggerStream.Write( Trigger );
            }

#endif

        }



    }
}