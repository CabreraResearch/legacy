using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Data;
//using ChemSW.RscAdo;
//using ChemSW.TblDn;
using ChemSW.DB;
using ChemSW.Core;
using ChemSW.Exceptions;


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswSchemaScriptsTest : ICswSchemaScripts
    {
        private CswNbtResources _CswNbtResources;
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn = null;
        private List<CswSchemaUpdateDriver> _UpdateDriverList = new List<CswSchemaUpdateDriver>();
		private Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> _UpdateDrivers = new Dictionary<CswSchemaVersion, CswSchemaUpdateDriver>();
		public Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> UpdateDrivers { get { return _UpdateDrivers; } }


        private string _getCaseNumberFromTestCaseTypeName( string TypeName ) { return ( TypeName.Substring( 12, 3 ) ); }
        //        private string _getCaseNumberFromResourceTypeName( string TypeName ) { return ( TypeName.Substring( 12, 3 ) ); }

        public CswSchemaScriptsTest( CswNbtResources CswNbtResources, int StartAtTestCase, int EndAtTestCase, List<string> IgnoreCases )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtSchemaModTrnsctn = new CswNbtSchemaModTrnsctn( _CswNbtResources );

            List<string> TestCaseTypeNames = new List<string>();
            Dictionary<string, Type> TestCaseTypesByName = new Dictionary<string, Type>();
            //Dictionary<string, Type> TestResourceTypesByName = new Dictionary<string, Type>();
            //Dictionary<string, Object> TestResourceInstancesByName = new Dictionary<string, Object>();
            Type[] Types = Assembly.GetExecutingAssembly().GetTypes();
            foreach( Type CurrentType in Types )
            {
                if( CurrentType.Namespace == "ChemSW.Nbt.Schema" && CurrentType.IsClass )
                {
                    if( typeof( CswUpdateSchemaTo ).IsAssignableFrom( CurrentType ) && CurrentType.Name.Contains( "CswTestCase_" ) )
                    {
                        string TestCaseNumberSegment = _getCaseNumberFromTestCaseTypeName( CurrentType.Name );
                        if( false == IgnoreCases.Contains( TestCaseNumberSegment ) )
                        {
                            TestCaseTypeNames.Add( CurrentType.Name );
                            TestCaseTypesByName.Add( CurrentType.Name, CurrentType );
                        }
                    }//if it's a test case

					//if( CurrentType.Name.Contains( "CswTstCaseRsrc_" ) )
					//{
					//    string TestCaseGroupId = CurrentType.Name.Substring( 15, 3 );
					//    if( false == TestResourceTypesByName.ContainsKey( CurrentType.Name ) )
					//    {
					//        TestResourceTypesByName.Add( TestCaseGroupId, CurrentType );
					//        TestResourceInstancesByName.Add( TestCaseGroupId, null );
					//    }
					//}//if its a test case resource

                }//if it's in our schema and it's a class

            }//iterate types




            List<Type> TestCaseTypes = new List<Type>();
            TestCaseTypeNames.Sort();
            if( 0 == EndAtTestCase )
            {
                EndAtTestCase = TestCaseTypeNames.Count;
            }
            foreach( string CurrentTypeName in TestCaseTypeNames )
            {
                string TestCaseNumberSegment = _getCaseNumberFromTestCaseTypeName( CurrentTypeName );
                Int32 TestCaseNumber = CswConvert.ToInt32( TestCaseNumberSegment );
                if( TestCaseNumber >= StartAtTestCase && TestCaseNumber <= EndAtTestCase )
                {
                    TestCaseTypes.Add( TestCaseTypesByName[CurrentTypeName] );
                }
            }


            for( Int32 idx = 0; idx < TestCaseTypes.Count; idx++ )
            {
                Type CurrentTestCaseType = TestCaseTypes[idx];



				//Object[] ResourceCtorArgs = new Object[1];
				//ResourceCtorArgs[0] = _CswNbtSchemaModTrnsctn;

				//string TestCaseGroupId = _getCaseNumberFromTestCaseTypeName( CurrentTestCaseType.Name );
				//if( null == TestResourceInstancesByName[TestCaseGroupId] )
				//{
				//    TestResourceInstancesByName[TestCaseGroupId] = Activator.CreateInstance( TestResourceTypesByName[TestCaseGroupId], ResourceCtorArgs );
				//}


                CswSchemaVersion CurrentVersion = new CswSchemaVersion( 1, 'T', idx + 1 );

                Object[] TestCaseCtorArgs = new Object[1];
                //TestCaseCtorArgs[0] = _CswNbtSchemaModTrnsctn;
                TestCaseCtorArgs[0] = CurrentVersion;
                //TestCaseCtorArgs[2] = TestResourceInstancesByName[TestCaseGroupId];

                CswUpdateSchemaTo CurrentTestCaseInstance = (CswUpdateSchemaTo) Activator.CreateInstance( CurrentTestCaseType, TestCaseCtorArgs );
                _UpdateDrivers.Add( CurrentTestCaseInstance.SchemaVersion, new CswSchemaUpdateDriver( CurrentTestCaseInstance ) );

            }

            foreach( CswSchemaUpdateDriver CurrentDriver in _UpdateDrivers.Values )
            {
                _UpdateDriverList.Add( CurrentDriver );
            }

        }//ctor


        public CswSchemaVersion LatestVersion
        {
            get { return ( _UpdateDriverList[_UpdateDriverList.Count - 1].SchemaVersion ); }
        }

        public CswSchemaVersion MinimumVersion
        {
            get { return ( new CswSchemaVersion( 1, 'T', 00 ) ); }
            //get { return ( _UpdateDriverList[0].SchemaVersion ); }
        }

        public CswSchemaVersion CurrentVersion
        {
            get
            {
                CswSchemaVersion ReturnVal = MinimumVersion;
                if( _CurrentIdx >= 0 )
                {
                    ReturnVal = _UpdateDriverList[_CurrentIdx].SchemaVersion;
                }

                return ( ReturnVal );
            }//get

        }//CurrentVersion

        public CswSchemaVersion TargetVersion
        {
            get
            {
                CswSchemaVersion ret = null;
                if( CurrentVersion == MinimumVersion )
                    ret = new CswSchemaVersion( LatestVersion.CycleIteration, LatestVersion.ReleaseIdentifier, 1 );
                else
                    ret = new CswSchemaVersion( CurrentVersion.CycleIteration, CurrentVersion.ReleaseIdentifier, CurrentVersion.ReleaseIteration );
                return ret;
            }
        }//TargetVersion

        private Int32 _CurrentIdx = Int32.MinValue;
        public CswSchemaUpdateDriver Next
        {
            get
            {
                CswSchemaUpdateDriver ReturnVal = null;


                if( _UpdateDriverList.Count > ( _CurrentIdx + 1 ) )
                {
                    if( Int32.MinValue == _CurrentIdx )
                    {
                        _CurrentIdx = 0;
                    }
                    else
                    {
                        _CurrentIdx++;
                    }

                    ReturnVal = _UpdateDriverList[_CurrentIdx];
                }

                return ( ReturnVal );

            }//get

        }//Next

        public CswSchemaUpdateDriver this[CswSchemaVersion CswSchemaVersion]
        {
            get
            {
                CswSchemaUpdateDriver ReturnVal = null;

                if( _UpdateDrivers.ContainsKey( CswSchemaVersion ) )
                {
                    ReturnVal = _UpdateDrivers[CswSchemaVersion];
                }

                return ( ReturnVal );

            }//get

        }//indexer

        public void stampSchemaVersion( CswSchemaUpdateDriver CswSchemaUpdateDriver )
        {
            _CswNbtResources.CswLogger.reportAppState( "Succesfully ran schema updater test " + CswSchemaUpdateDriver.Description );
        }//stampSchemaVersion()


        //#region IEnumerable
        //public IEnumerator<CswSchemaUpdateDriver> GetEnumerator()
        //{
        //    return ( new CswSchemaScriptsTestEnumerator( this ) );
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return ( new CswSchemaScriptsTestEnumerator( this ) );
        //}


        //private class CswSchemaScriptsTestEnumerator : IEnumerator<CswSchemaUpdateDriver>
        //{
        //    private CswSchemaScriptsTest _CswSchemaScriptsTest = null;
        //    public CswSchemaScriptsTestEnumerator( CswSchemaScriptsTest CswSchemaScriptsTest )
        //    {
        //        _CswSchemaScriptsTest = CswSchemaScriptsTest;
        //    }

        //    public CswSchemaUpdateDriver Current
        //    {
        //        get { throw new NotImplementedException(); }
        //    }

        //    public void Dispose()
        //    {
        //        throw new NotImplementedException();
        //    }

        //    object IEnumerator.Current
        //    {
        //        get { throw new NotImplementedException(); }
        //    }

        //    public bool MoveNext()
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public void Reset()
        //    {
        //        throw new NotImplementedException();
        //    }
        //}//CswSchemaScriptsTestEnumerator

        //#endregion
    }//CswScriptCollections

}//ChemSW.Nbt.Schema
