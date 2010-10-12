using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.RscAdo;
using ChemSW.TblDn;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswSchemaUpdater
    {
        private CswNbtResources _CswNbtResources;
        private CswTableCaddy _UpdateHistoryTableCaddy;
        private DataTable _UpdateHistoryTable;
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn = null;

        private List<CswSchemaUpdateDriver> _UpdaterDrivers = new List<CswSchemaUpdateDriver>();

        /// <summary>
        /// Constructor
        /// </summary>
        public CswSchemaUpdater( CswNbtResources Resources )
        {
            _CswNbtResources = Resources;
            _CswNbtSchemaModTrnsctn = new CswNbtSchemaModTrnsctn( _CswNbtResources );

            _UpdaterDrivers.Add( new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo68( _CswNbtSchemaModTrnsctn ) ) );
            _UpdaterDrivers.Add( new CswSchemaUpdateDriver( _CswNbtSchemaModTrnsctn, new CswUpdateSchemaTo69( _CswNbtSchemaModTrnsctn ) ) );
        }

        /// <summary>
        /// The latest schema version
        /// </summary>
//        public Int32 LatestVersion = 69;


        private Int32 CurrentVersion
        {
            get { return Convert.ToInt32( _CswNbtResources.getConfigVariableValue( "schemaversion" ) ); }
        }

        /// <summary>
        /// Update the schema to the latest version
        /// </summary>
        public void Update()
        {
            //while ( CurrentVersion < LatestVersion )
            //{
            //    Int32 PriorCurrentVersion = CurrentVersion;
            //    Int32 TargetVersion = CurrentVersion + 1;
            //    _UpdaterDrivers.Add( _CswUpdateSchemaFactory.makeUpdater( TargetVersion ) );
            //    if ( PriorCurrentVersion == CurrentVersion )  // Prevents infinite loops
            //        throw new CswDniException( "Schema Update Error", "After update to " + TargetVersion.ToString() + ", schema version number did not change from: " + PriorCurrentVersion );
            //}

            bool UpdateSuccessful = true;
            for ( Int32 idx = 0; ( idx < _UpdaterDrivers.Count ) && UpdateSuccessful; idx++ )
            {
                CswSchemaUpdateDriver CurrentUpdateDriver = _UpdaterDrivers[ idx ];

                if ( CurrentUpdateDriver.SchemaVersion > CurrentVersion )
                {
                    CurrentUpdateDriver.update();
                    if ( UpdateSuccessful = CurrentUpdateDriver.UpdateSucceded )
                    {
                        _CswNbtResources.setConfigVariableValue( "schemaversion", CurrentUpdateDriver.SchemaVersion.ToString() );

                    }

                    if ( _UpdateHistoryTableCaddy == null )
                    {
                        // We have to do this after the update to 68, because this table doesn't exist in 67
                        _UpdateHistoryTableCaddy = _CswNbtResources.makeCswTableCaddy( "update_history" );
                        _UpdateHistoryTable = _UpdateHistoryTableCaddy.Table;
                    }

                    DataRow NewUpdateHistoryRow = _UpdateHistoryTable.NewRow();
                    NewUpdateHistoryRow[ "updatedate" ] = DateTime.Now.ToString();
                    NewUpdateHistoryRow[ "version" ] = CurrentUpdateDriver.SchemaVersion.ToString();

                    if ( UpdateSuccessful = CurrentUpdateDriver.UpdateSucceded )
                    {
                        NewUpdateHistoryRow[ "log" ] = CurrentUpdateDriver.Message;
                    }
                    else if ( CurrentUpdateDriver.RollbackSucceded )
                    {
                        NewUpdateHistoryRow[ "log" ] = "Schema rolled back to previous version due to failure: " + CurrentUpdateDriver.Message;
                    }
                    else
                    {
                        NewUpdateHistoryRow[ "log" ] = "Schema rollback failed; current schema state undefined: " + CurrentUpdateDriver.Message;
                    }

                    _UpdateHistoryTable.Rows.Add( NewUpdateHistoryRow );

                }//if current updater is higher than current version

            }//iterate updaters

            _UpdateHistoryTableCaddy.updateAndCommit( _UpdateHistoryTable );
            _CswNbtResources.finalize();

        }//Update()

    }//CswSchemaUpdater

}//ChemSW.Nbt.Schema
