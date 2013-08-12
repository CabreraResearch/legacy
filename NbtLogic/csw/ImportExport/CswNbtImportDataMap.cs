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
            CswTableUpdate ImportDataMapSelect = CswNbtResources.makeCswTableUpdate( "Importer_DataMap_Update", CswNbtImportTables.ImportDataMap.TableName );
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
        }
    }
}
