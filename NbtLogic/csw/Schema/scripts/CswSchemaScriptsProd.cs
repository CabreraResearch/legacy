﻿using System.Collections.Generic;
using System.Linq;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswSchemaScriptsProd : ICswSchemaScripts
    {
        private Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> _UpdateDrivers = new Dictionary<CswSchemaVersion, CswSchemaUpdateDriver>();
        public Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> UpdateDrivers { get { return _UpdateDrivers; } }

        public CswSchemaScriptsProd()
        {
            // This is where you manually set to the last version of the previous release (the one currently in production)
            _MinimumVersion = new CswSchemaVersion(1, 'U', 18);

            // This is where you add new versions.
            #region WILLIAM

            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case26827()));              //01W-001
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case28283Part1()));         //01W-002
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case28283Part2()));         //01W-003
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case28281()));              //01W-004
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case28282()));              //01W-005
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case28339()));              //01W-006
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case28252()));              //01W-007
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case5083()));               //01W-008
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case28247()));              //01W-009
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case28393()));              //01W-010
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case28399()));              //01W-011
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case28399B()));             //01W-012
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case28331()));              //01W-013
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case28267()));              //01W-014
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case28374()));              //01W-015
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case28374B()));             //01W-016
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case28293()));              //01W-017
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case28408_Part1()));        //01W-018
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case28408_Part2()));        //01W-019
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case28295()));              //01W-020
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case27649()));              //01W-021
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case27436()));              //01W-022
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case27436B()));             //01W-023
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case27436C()));             //01W-024
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case27436D()));             //01W-025
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case28498()));              //01W-026
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case28570()));              //01W-027
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case28526()));              //01W-028
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case28122()));              //01W-029
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01V_Case28611()));              //01W-030
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01W_Case28363()));              //01W-031
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01W_Case24647()));              //01W-032
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01W_Case28246()));              //01W-033
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01W_Case28277()));              //01W-034
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01W_Case28053()));              //01W-035
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01W_TreeGrouping_Case27882())); //01W-036 
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01W_Case28393B()));             //01W-037
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01W_Case28345()));              //01W-038 
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01W_Case28507()));              //01W-039 
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01W_Case28508()));              //01W-040
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01W_Case28560()));              //01W-041
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01W_Case28409()));              //01W-042
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01W_Case28395()));              //01W-043
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01W_Case28258()));              //01W-044
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01W_Case28513()));              //01W-045
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01W_Recurring_Case28340()));    //01W-046
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01W_Case27901()));              //01W-047 
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01W_Case26840()));              //01W-048
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01W_Case28655()));              //01W-049
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01W_Case28534()));              //01W-050
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01W_Case28557()));              //01W-051
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01W_Case28713()));              //01W-052
            _addVersionedScript(new CswSchemaUpdateDriver(new CswUpdateSchema_01W_Case28395B()));             //01W-053

            #endregion WILLIAM

            #region YORICK

            // e.g. _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_01Y_CaseXXXXX() ) );  //01W-000   //01Y-000

            #endregion YORICK

            // This automatically detects the latest version
            _LatestVersion = _MinimumVersion;
            foreach (CswSchemaVersion Version in _UpdateDrivers.Keys.Where(Version => _LatestVersion == _MinimumVersion ||
                                                                                        (_LatestVersion.CycleIteration == Version.CycleIteration &&
                                                                                            _LatestVersion.ReleaseIdentifier == Version.ReleaseIdentifier &&
                                                                                            _LatestVersion.ReleaseIteration < Version.ReleaseIteration)))
            {
                _LatestVersion = Version;
            }

            #region Before Scripts

            _addRunBeforeScript(new CswSchemaUpdateDriver(new RunBeforeEveryExecutionOfUpdater_01()), RunBeforeEveryExecutionOfUpdater_01.Title);
            _addRunBeforeScript(new CswSchemaUpdateDriver(new RunBeforeEveryExecutionOfUpdater_01M()), RunBeforeEveryExecutionOfUpdater_01M.Title);
            _addRunBeforeScript(new CswSchemaUpdateDriver(new RunBeforeEveryExecutionOfUpdater_01OC()), RunBeforeEveryExecutionOfUpdater_01OC.Title);
            _addRunBeforeScript(new CswSchemaUpdateDriver(new RunBeforeEveryExecutionOfUpdater_02SQL()), RunBeforeEveryExecutionOfUpdater_02SQL.Title);
            _addRunBeforeScript(new CswSchemaUpdateDriver(new RunBeforeEveryExecutionOfUpdater_02()), RunBeforeEveryExecutionOfUpdater_02.Title);
            _addRunBeforeScript(new CswSchemaUpdateDriver(new RunBeforeEveryExecutionOfUpdater_03()), RunBeforeEveryExecutionOfUpdater_03.Title);

            #endregion Before Scripts

            #region After Scripts

            _addRunAfterScript(new CswSchemaUpdateDriver(new RunAfterEveryExecutionOfUpdater_01()), RunAfterEveryExecutionOfUpdater_01.Title);

            #endregion After Scripts

        }//ctor

        #region ICswSchemaScripts

        private CswSchemaVersion _LatestVersion = null;
        public CswSchemaVersion LatestVersion
        {
            get { return (_LatestVersion); }
        }

        private CswSchemaVersion _MinimumVersion = null;
        public CswSchemaVersion MinimumVersion
        {
            get { return (_MinimumVersion); }
        }

        public CswSchemaVersion CurrentVersion(CswNbtResources CswNbtResources)
        {
            return (new CswSchemaVersion(CswNbtResources.ConfigVbls.getConfigVariableValue("schemaversion")));
        }

        public CswSchemaVersion TargetVersion(CswNbtResources CswNbtResources)
        {
            CswSchemaVersion ret = null;
            CswSchemaVersion myCurrentVersion = CurrentVersion(CswNbtResources);
            if (myCurrentVersion == MinimumVersion)
                ret = new CswSchemaVersion(LatestVersion.CycleIteration, LatestVersion.ReleaseIdentifier, 1);
            else
                ret = new CswSchemaVersion(myCurrentVersion.CycleIteration, myCurrentVersion.ReleaseIdentifier, myCurrentVersion.ReleaseIteration + 1);
            return ret;
        }

        public CswSchemaUpdateDriver Next(CswNbtResources CswNbtResources)
        {
            CswSchemaUpdateDriver ReturnVal = null;

            CswSchemaVersion myCurrentVersion = CurrentVersion(CswNbtResources);
            if (myCurrentVersion == MinimumVersion ||
                (LatestVersion.CycleIteration == myCurrentVersion.CycleIteration &&
                    LatestVersion.ReleaseIdentifier == myCurrentVersion.ReleaseIdentifier &&
                    LatestVersion.ReleaseIteration > myCurrentVersion.ReleaseIteration))
            {
                ReturnVal = _UpdateDrivers[TargetVersion(CswNbtResources)];
            }
            return (ReturnVal);
        }

        public CswSchemaUpdateDriver this[CswSchemaVersion CswSchemaVersion]
        {
            get
            {
                CswSchemaUpdateDriver ReturnVal = null;

                if (_UpdateDrivers.ContainsKey(CswSchemaVersion))
                {
                    ReturnVal = _UpdateDrivers[CswSchemaVersion];
                }

                return (ReturnVal);
            }
        }

        public void stampSchemaVersion(CswNbtResources CswNbtResources, CswSchemaUpdateDriver CswSchemaUpdateDriver)
        {
            CswNbtResources.ConfigVbls.setConfigVariableValue("schemaversion", CswSchemaUpdateDriver.SchemaVersion.ToString()); ;
        }

        #endregion

        #region Versioned scripts

        CswSchemaVersion _makeNextSchemaVersion()
        {
            int SuperCycle = _MinimumVersion.CycleIteration;
            char ReleaseIdentifier = _MinimumVersion.ReleaseIdentifier;
            if ('Z' != ReleaseIdentifier)
            {
                char[] alpha = "ABCDEFGHIJKLMNOPQRSTUWXYZ".ToCharArray(); //No V for Viola
                List<char> Chars = new List<char>(alpha);
                int ReleaseIdInt = Chars.IndexOf(ReleaseIdentifier);
                ReleaseIdInt++;
                ReleaseIdentifier = Chars[ReleaseIdInt];
            }
            else
            {
                SuperCycle = _MinimumVersion.CycleIteration + 1;
                ReleaseIdentifier = 'A';
            }

            return (new CswSchemaVersion(SuperCycle, ReleaseIdentifier, _UpdateDrivers.Keys.Count + 1));
        }

        private void _addVersionedScript(CswSchemaUpdateDriver CswSchemaUpdateDriver)
        {
            CswSchemaUpdateDriver.SchemaVersion = _makeNextSchemaVersion();
            CswSchemaUpdateDriver.Description = CswSchemaUpdateDriver.SchemaVersion.ToString(); //we do this in prod scripts because test scripts have a different dispensation for description
            _UpdateDrivers.Add(CswSchemaUpdateDriver.SchemaVersion, CswSchemaUpdateDriver);
        }

        #endregion

        #region Run-always scripts

        private List<CswSchemaUpdateDriver> _RunBeforeScripts = new List<CswSchemaUpdateDriver>();
        public List<CswSchemaUpdateDriver> RunBeforeScripts
        {
            get
            {
                return (_RunBeforeScripts);
            }
        }

        private void _addRunBeforeScript(CswSchemaUpdateDriver CswSchemaUpdateDriver, string Description)
        {
            CswSchemaUpdateDriver.SchemaVersion = new CswSchemaVersion(0, '#', _RunBeforeScripts.Count);
            CswSchemaUpdateDriver.Description = Description;
            if (false == _RunBeforeScripts.Contains(CswSchemaUpdateDriver))
            {
                _RunBeforeScripts.Add(CswSchemaUpdateDriver);
            }
        }

        private List<CswSchemaUpdateDriver> _RunAfterScripts = new List<CswSchemaUpdateDriver>();
        public List<CswSchemaUpdateDriver> RunAfterScripts
        {
            get
            {
                return (_RunAfterScripts);
            }

        }
        private void _addRunAfterScript(CswSchemaUpdateDriver CswSchemaUpdateDriver, string Description)
        {
            CswSchemaUpdateDriver.SchemaVersion = new CswSchemaVersion(99, '#', _RunAfterScripts.Count);
            CswSchemaUpdateDriver.Description = Description;
            if (false == _RunAfterScripts.Contains(CswSchemaUpdateDriver))
            {
                _RunAfterScripts.Add(CswSchemaUpdateDriver);
            }

        }

        #endregion

    }//CswScriptCollections
}//ChemSW.Nbt.Schema
