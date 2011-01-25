using System;
using ChemSW.DB;
using System.Data;
using ChemSW.Core;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.Schema
{
    public class CswDemoDataManager
    {

        private CswNbtResources _CswNbtResources;
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn = null;
        
        public CswDemoDataManager( CswNbtResources CswNbtResources)
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtSchemaModTrnsctn = new CswNbtSchemaModTrnsctn( _CswNbtResources );
        }

        /// <summary>
        /// Removes all rows flagged isdemo=1
        /// </summary>
        public void RemoveDemoData()
        {
            if( "1" == _CswNbtResources.getConfigVariableValue( "is_demo" ) )
            {
                String AllDemoTablesSQL = " select distinct tablename from data_dictionary where columnname='isdemo' and tablename <> 'nodes' and tablename <> 'statistics' order by tablename ";
                CswArbitrarySelect AllDemoTables = _CswNbtResources.makeCswArbitrarySelect( "Fetch Tables With Demo Data", AllDemoTablesSQL );
                DataTable DemosDataTable = AllDemoTables.getTable();
                CswCommaDelimitedString TablesToPrune = new CswCommaDelimitedString();
                
                for( Int32 i = 0; i < DemosDataTable.Rows.Count; i++ )
                {
                    TablesToPrune.Add( DemosDataTable.Rows[i]["tablename"].ToString() );
                }
                TablesToPrune.Sort();

                //As of 01H-17, executing this in alphabetical order (minus nodes/statistics) will work
                foreach( String TableName in TablesToPrune )
                {
                    String NukeDemoDataSQL = "delete from " + TableName + " where isdemo = '" + CswConvert.ToDbVal( true ) + "' or isdemo is null ";
                    try
                    {
                        _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( NukeDemoDataSQL );
                        _CswNbtSchemaModTrnsctn.commitTransaction();
                    }
                    catch( Exception ex )
                    {
                        throw new CswDniException( "Before records from the " + TableName + " table can be deleted, child records must be deleted first.", "Oracle threw an " + ex + " exception." );
                    }
                }
                
                // We just happen to know that these are the only 2 tables which have constraints and need to be dealt with separately
                try
                {
                    _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "delete from nodes where isdemo= '" + CswConvert.ToDbVal( true ) + "' or isdemo is null " );
                    _CswNbtSchemaModTrnsctn.commitTransaction();
                    _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "delete from statistics where isdemo= '" + CswConvert.ToDbVal( true ) + "' or isdemo is null " );
                    _CswNbtSchemaModTrnsctn.commitTransaction();
                }
                catch( Exception ex )
                {
                    throw new CswDniException( "Before records from the nodes/statistics table(s) can be deleted, child records must be deleted first.", "Oracle threw an " + ex + " exception." );
                }

                _CswNbtResources.setConfigVariableValue( "is_demo", "0" );
            }//if( "1" == _CswNbtResources.getConfigVariableValue( "is_demo" ) )
        }
    }
}
