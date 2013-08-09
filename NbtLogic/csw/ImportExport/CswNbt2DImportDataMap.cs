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
    public class CswNbt2DImportDataMap
    {
        private CswNbtResources _CswNbtResources;
        private DataRow _row;

        public CswNbt2DImportDataMap( CswNbtResources CswNbtResources, string ImportDataTableName )
        {
            CswTableUpdate ImportDataMapSelect = CswNbtResources.makeCswTableUpdate( "Importer_DataMap_Update", CswNbt2DImportTables.ImportDataMap.TableName );
            DataTable ImportDataMapTable = ImportDataMapSelect.getTable( "where " + CswNbt2DImportTables.ImportDataMap.datatablename + " = '" + ImportDataTableName );
            if( ImportDataMapTable.Rows.Count > 0 )
            {
                _finishConstructor( CswNbtResources, ImportDataMapTable.Rows[0] );
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid Import Data Table Name: " + ImportDataTableName, "CswNbt2DImportDataMap could not find a match in " + CswNbt2DImportTables.ImportDataMap.TableName );
            }
        }

        public CswNbt2DImportDataMap( CswNbtResources CswNbtResources, DataRow DataMapRow )
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
            get { return CswConvert.ToInt32( _row[CswNbt2DImportTables.ImportDataMap.importdatamapid] ); }
        }
        public Int32 ImportDefinitionId
        {
            get { return CswConvert.ToInt32( _row[CswNbt2DImportTables.ImportDataMap.importdefinitionid] ); }
        }

        public string ImportDataTableName
        {
            get { return _row[CswNbt2DImportTables.ImportDataMap.datatablename].ToString(); }
        }
        public bool Overwrite
        {
            get { return CswConvert.ToBoolean( _row[CswNbt2DImportTables.ImportDataMap.overwrite] ); }
        }
    }
}
