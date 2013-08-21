using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.ImportExport
{
    public class CswNbtImportDataMap
    {
        private CswNbtResources _CswNbtResources;
        private DataRow _row;

        public CswNbtImportDataMap( CswNbtResources CswNbtResources, string ImportDataTableName )
        {
            CswTableSelect ImportDataMapSelect = CswNbtResources.makeCswTableSelect( "Importer_DataMap_Select", CswNbtImportTables.ImportDataMap.TableName );
            DataTable ImportDataMapTable = ImportDataMapSelect.getTable( "where " + CswNbtImportTables.ImportDataMap.datatablename + " = '" + ImportDataTableName + "'" );
            if( ImportDataMapTable.Rows.Count > 0 )
            {
                _finishConstructor( CswNbtResources, ImportDataMapTable.Rows[0] );
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid Import Data Table Name: " + ImportDataTableName, "CswNbt2DImportDataMap could not find a match in " + CswNbtImportTables.ImportDataMap.TableName );
            }
        }

        public CswNbtImportDataMap( CswNbtResources CswNbtResources, DataRow DataMapRow )
        {
            _finishConstructor( CswNbtResources, DataMapRow );
        }

        private void _finishConstructor( CswNbtResources CswNbtResources, DataRow DataMapRow )
        {
            _CswNbtResources = CswNbtResources;
            _row = DataMapRow;
        }

        public Int32 ImportDataMapId
        {
            get { return CswConvert.ToInt32( _row[CswNbtImportTables.ImportDataMap.importdatamapid] ); }
        }
        public Int32 ImportDataJobId
        {
            get { return CswConvert.ToInt32( _row[CswNbtImportTables.ImportDataMap.importdatajobid] ); }
        }
        public Int32 ImportDefinitionId
        {
            get { return CswConvert.ToInt32( _row[CswNbtImportTables.ImportDataMap.importdefid] ); }
        }
        public string ImportDataTableName
        {
            get { return _row[CswNbtImportTables.ImportDataMap.datatablename].ToString(); }
        }
        public bool Overwrite
        {
            get { return CswConvert.ToBoolean( _row[CswNbtImportTables.ImportDataMap.overwrite] ); }
        }
        public bool Completed
        {
            get { return CswConvert.ToBoolean( _row[CswNbtImportTables.ImportDataMap.completed] ); }
            set
            {
                CswTableUpdate ImportDataMapUpdate = _CswNbtResources.makeCswTableUpdate( "Importer_DataMap_Update", CswNbtImportTables.ImportDataMap.TableName );
                DataTable ImportDataMapTable = ImportDataMapUpdate.getTable( CswNbtImportTables.ImportDataMap.PkColumnName, this.ImportDataMapId );
                if( ImportDataMapTable.Rows.Count > 0 )
                {
                    ImportDataMapTable.Rows[0][CswNbtImportTables.ImportDataMap.completed] = CswConvert.ToDbVal( value );
                    ImportDataMapUpdate.update( ImportDataMapTable );
                }
            }
        }

        public void getStatus( out Int32 RowsDone,
                               out Int32 RowsTotal,
                               out Int32 RowsError,
                               out Int32 ItemsDone,
                               out Int32 ItemsTotal )
        {
            RowsDone = 0;
            RowsTotal = 0;
            RowsError = 0;
            ItemsDone = 0;
            ItemsTotal = 0;

            if( false == string.IsNullOrEmpty( ImportDataTableName ) && _CswNbtResources.isTableDefinedInDataBase( ImportDataTableName ) )
            {
                CswTableSelect ImportDataSelect = _CswNbtResources.makeCswTableSelect( "getStatus_data_select", ImportDataTableName );
                CswNbtImportDef BindingDef = new CswNbtImportDef( _CswNbtResources, ImportDefinitionId );
                if( null != BindingDef && BindingDef.ImportOrder.Count > 0 )
                {
                    // RowsDone
                    string RowsPendingWhereClause = string.Empty;
                    foreach( CswNbtImportDefOrder Order in BindingDef.ImportOrder.Values )
                    {
                        if( string.Empty != RowsPendingWhereClause )
                        {
                            RowsPendingWhereClause += " and ";
                        }
                        RowsPendingWhereClause += Order.PkColName + " is not null";
                    }
                    RowsDone = ImportDataSelect.getRecordCount( "where " + CswNbtImportTables.ImportDataN.error + " = '" + CswConvert.ToDbVal( false ) + "' and (" + RowsPendingWhereClause + ") " );

                    // ItemsPending
                    foreach( CswNbtImportDefOrder Order in BindingDef.ImportOrder.Values )
                    {
                        ItemsDone += ImportDataSelect.getRecordCount( "where " + CswNbtImportTables.ImportDataN.error + " = '" + CswConvert.ToDbVal( false ) + "' and " + Order.PkColName + " is not null " );
                    }

                    // And the rest
                    RowsTotal = ImportDataSelect.getRecordCount();
                    RowsError = ImportDataSelect.getRecordCount( "where error = '" + CswConvert.ToDbVal( true ) + "'" );
                    ItemsTotal = RowsTotal * BindingDef.ImportOrder.Values.Count;

                } // if( null != BindingDef && BindingDef.ImportOrder.Count > 0 )
            } // if( false == string.IsNullOrEmpty( ImportDataTableName ) && _CswNbtResources.isTableDefinedInDataBase( ImportDataTableName ) )
        } // getStatus()
    } // class
} // namespace
