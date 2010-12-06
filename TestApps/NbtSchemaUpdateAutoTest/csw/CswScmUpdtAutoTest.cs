using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Data;
using System.Text;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Config;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
//using ChemSW.TblDn;
using ChemSW.Nbt.Schema;
//using ChemSW.Nbt.TableEvents;
using ChemSW.Nbt.TreeEvents;
using ChemSW.Log;
using ChemSW.Config;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.SchemaUpdaterAutoTest
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswScmUpdtAutoTest
    {
        private CswNbtResources _CswNbtResources = null;
        private CswDbCfgInfoNbt _CswDbCfgInfoNbt = null;
        private CswSetupVblsNbt _CswSetupVblsNbt = null;
        private CswNbtObjClassFactory _CswNbtObjClassFactory = null;
        private CswNbtMetaDataEvents _CswNbtMetaDataEvents = null;
        private ICswLogger _CswLogger = null;
        CswScmUpdt_CollectionOfTestCases _CswSchemaUpdaterTestCaseCollection = null;

        public delegate void UpdateTestWriteMessage( string Message );
        public event UpdateTestWriteMessage WriteMessage = null;

        private List<CswSchemaUpdateDriver> _UpdaterDrivers = new List<CswSchemaUpdateDriver>();

        /// <summary>
        /// Constructor
        /// </summary>
        public CswScmUpdtAutoTest()
        {
            string ConfigurationPath = Application.StartupPath + "\\..\\etc";
            _CswDbCfgInfoNbt = new CswDbCfgInfoNbt( SetupMode.Executable );
            _CswSetupVblsNbt = new CswSetupVblsNbt( SetupMode.Executable );


            _CswNbtResources = CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, _CswSetupVblsNbt, _CswDbCfgInfoNbt, CswTools.getConfigurationFilePath( SetupMode.Executable ) );

            //_CswNbtObjClassFactory = new CswNbtObjClassFactory();

            //_CswNbtResources = new CswNbtResources( AppType.Nbt, _CswSetupVblsNbt, _CswDbCfgInfoNbt, false, false );
            //_CswNbtResources.SetDbResources( new CswNbtTreeFactory( ConfigurationPath ) );

            //_CswNbtMetaDataEvents = new CswNbtMetaDataEvents( _CswNbtResources );
            //_CswNbtResources.OnMakeNewNodeType += new CswNbtResources.NewNodeTypeEventHandler( _CswNbtMetaDataEvents.OnMakeNewNodeType );
            //_CswNbtResources.OnCopyNodeType += new CswNbtResources.CopyNodeTypeEventHandler( _CswNbtMetaDataEvents.OnCopyNodeType );
            //_CswNbtResources.OnMakeNewNodeTypeProp += new CswNbtResources.NewNodeTypePropEventHandler( _CswNbtMetaDataEvents.OnMakeNewNodeTypeProp );
            //_CswNbtResources.OnEditNodeTypePropName += new CswNbtResources.EditPropNameEventHandler( _CswNbtMetaDataEvents.OnEditNodeTypePropName );
            //_CswNbtResources.OnDeleteNodeTypeProp += new CswNbtResources.DeletePropEventHandler( _CswNbtMetaDataEvents.OnDeleteNodeTypeProp );
            //_CswNbtResources.OnEditNodeTypeName += new CswNbtResources.EditNodeTypeNameEventHandler( _CswNbtMetaDataEvents.OnEditNodeTypeName );

            _CswLogger = _CswNbtResources.CswLogger;

            _CswNbtResources.CurrentUser = new CswNbtSystemUser( _CswNbtResources, "_ScmUpdtAutoTestUser" );

            _CswSchemaUpdaterTestCaseCollection = new CswScmUpdt_CollectionOfTestCases();


            //_CswNbtResources.CswTblFactory = new CswNbtTblFactory( _CswNbtResources );
            //_CswNbtResources.CswTableCaddyFactory = new CswTableCaddyFactoryNbt( _CswNbtResources );

        }//ctor

        public List<string> Names
        {
            get
            {
                return ( _CswSchemaUpdaterTestCaseCollection.Names );
            }
        }

        public void runTests( List<string> NamesOfTestsToRun )
        {

            ArrayList AccessIds = new ArrayList( _CswDbCfgInfoNbt.AccessIds );
            foreach( string CurrentAccessId in AccessIds )
            {
                _CswNbtResources.AccessId = CurrentAccessId;
                _CswNbtResources.refreshDataDictionary();

                CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn = new CswNbtSchemaModTrnsctn( _CswNbtResources );
                Int32 TestCaseCount = 0;
                Int32 SuccededTests = 0;
                Int32 FailedTests = 0;

                List<CswScmUpdt_TstCse> TestCasesToRun = new List<CswScmUpdt_TstCse>();
                foreach( CswScmUpdt_TstCse CurrentTest in _CswSchemaUpdaterTestCaseCollection )
                {
                    if( NamesOfTestsToRun.Contains( CurrentTest.Name ) )
                    {
                        TestCaseCount++;
                        TestCasesToRun.Add( CurrentTest );
                    }
                }

                WriteMessage( "***Initiating execution of " + TestCaseCount.ToString() + " test cases on AccessId " + CurrentAccessId.ToString() );
                Int32 Counter = 0;
                double TotalSeconds = 0;
                foreach( CswScmUpdt_TstCse CurrentTest in TestCasesToRun )
                {
                    try
                    {
                        CurrentTest.CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
                        DateTime StartTime = DateTime.Now;
                        CurrentTest.runTest();
                        DateTime EndTime = DateTime.Now;
                        TimeSpan RunTime = EndTime - StartTime;
                        TotalSeconds += RunTime.TotalSeconds;
                        WriteMessage( "Test Case " + ( ++Counter ).ToString() + " Succeeded -- Test " + CurrentTest.Name + " (" + RunTime.TotalSeconds.ToString() + ") seconds." );
                        //as of the fix for bz # 9018, we need to clear caddies
                        //between test runs because otherwise we end up having to do 
                        //CswTableCaddy's rollback semantics on a ton of caddies that 
                        //get left sitting around. We would not ordinarily need to do this
                        //in the case of a production situation because there the request
                        //cycle cleans out the caddies
                        CswNbtSchemaModTrnsctn.clearUpdates();

                        SuccededTests++;
                    }

                    catch( CswScmUpdt_Exception CswSchemaUpdaterTestException )
                    {
                        WriteMessage( "Test Case Failed -- Test " + CurrentTest.Name + " threw " + CswSchemaUpdaterTestException.Message );
                        FailedTests++;
                    }//

                    catch( Exception Exception )
                    {
                        WriteMessage( "Test Process Failed: " + CurrentTest.Name + " -- " + Exception.Message );
                        FailedTests++;
                    }

                }//iterate test cases

                WriteMessage( "***Completed execution of " + TestCaseCount.ToString() + " test cases on AccessId " + CurrentAccessId.ToString() );
                WriteMessage( "***Total Succeded: " + SuccededTests.ToString() );
                WriteMessage( "***Total Failed: " + FailedTests.ToString() );
                WriteMessage( "***Execution Time:  " + TotalSeconds + " seconds" );

            }//iterate accesssids

        }//runTests()


        private void _testOps()
        {
        }//_testOps()

        /// <summary>
        /// The latest schema version
        /// </summary>
        //        public Int32 LatestVersion = 69;


        //private Int32 CurrentVersion
        //{
        //    get { return Convert.ToInt32( _CswNbtResources.getConfigVariableValue( "schemaversion" ) ); }
        //}

        ///// <summary>
        ///// Update the schema to the latest version
        ///// </summary>
        //public void Update()
        //{
        //    bool UpdateSuccessful = true;
        //    for ( Int32 idx = 0; ( idx < _UpdaterDrivers.Count ) && UpdateSuccessful; idx++ )
        //    {
        //        CswSchemaUpdateDriver CurrentUpdateDriver = _UpdaterDrivers[ idx ];

        //        if ( CurrentUpdateDriver.SchemaVersion > CurrentVersion )
        //        {
        //            CurrentUpdateDriver.update();
        //            if ( UpdateSuccessful = CurrentUpdateDriver.UpdateSucceeded )
        //            {
        //                _CswNbtResources.setConfigVariableValue( "schemaversion", CurrentUpdateDriver.SchemaVersion.ToString() );

        //            }

        //            if ( _UpdateHistoryTableCaddy == null )
        //            {
        //                // We have to do this after the update to 68, because this table doesn't exist in 67
        //                _UpdateHistoryTableCaddy = _CswNbtResources.makeCswTableCaddy( "update_history" );
        //                _UpdateHistoryTable = _UpdateHistoryTableCaddy.Table;
        //            }

        //            DataRow NewUpdateHistoryRow = _UpdateHistoryTable.NewRow();
        //            NewUpdateHistoryRow[ "updatedate" ] = DateTime.Now.ToString();
        //            NewUpdateHistoryRow[ "version" ] = CurrentUpdateDriver.SchemaVersion.ToString();

        //            if ( UpdateSuccessful = CurrentUpdateDriver.UpdateSucceeded )
        //            {
        //                NewUpdateHistoryRow[ "log" ] = CurrentUpdateDriver.Message;
        //            }
        //            else if ( CurrentUpdateDriver.RollbackSucceeded )
        //            {
        //                NewUpdateHistoryRow[ "log" ] = "Schema rolled back to previous version due to failure: " + CurrentUpdateDriver.Message;
        //            }
        //            else
        //            {
        //                NewUpdateHistoryRow[ "log" ] = "Schema rollback failed; current schema state undefined: " + CurrentUpdateDriver.Message;
        //            }

        //            _UpdateHistoryTable.Rows.Add( NewUpdateHistoryRow );

        //        }//if current updater is higher than current version

        //    }//iterate updaters

        //    _UpdateHistoryTableCaddy.updateAndCommit( _UpdateHistoryTable );
        //    _CswNbtResources.finalize();

        //}//Update()

    }//CswSchemaUpdaterAutoTest

}//ChemSW.Nbt.Schema
