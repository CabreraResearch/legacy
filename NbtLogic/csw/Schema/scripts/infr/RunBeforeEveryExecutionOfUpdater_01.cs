using System;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_01 : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: DDL";

        #region Blame Logic

        private CswDeveloper _Author = CswDeveloper.NBT;

        public override CswDeveloper Author
        {
            get { return _Author; }
        }

        private Int32 _CaseNo;

        public override int CaseNo
        {
            get { return _CaseNo; }
        }

        private void _acceptBlame( CswDeveloper BlameMe, Int32 BlameCaseNo )
        {
            _Author = BlameMe;
            _CaseNo = BlameCaseNo;
        }

        private void _resetBlame()
        {
            _Author = CswDeveloper.NBT;
            _CaseNo = 0;
        }

        #endregion Blame Logic

        public override void update()
        {
            // This script is for changes to schema structure,
            // or other changes that must take place before any other schema script.

            // NOTE: This script will be run many times, so make sure your changes are safe!

            #region YORICK

            _createExcludeInQuotaBarColumns( CswDeveloper.MB, 28752 );
            _createPrereqColumn( CswDeveloper.MB, 29089 );

            #endregion YORICK

            #region ASPEN

            _createNodeCountColumns( CswDeveloper.MB, 28355 );
            _createLoginDataTable( CswDeveloper.BV, 27906 );

            #endregion ASPEN

        }//Update()

        #region Yorick

        private void _createExcludeInQuotaBarColumns( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );

            string ExcludeInQuotaBarColName = "excludeinquotabar";

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "object_class", ExcludeInQuotaBarColName ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "object_class", ExcludeInQuotaBarColName, "Whether this ObjectClass is excluded when determining the quota bar size", false, true );
            }

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetypes", ExcludeInQuotaBarColName ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "nodetypes", ExcludeInQuotaBarColName, "Whether this NodeType is excluded when determining the quota bar size", false, true );
            }

            _resetBlame();
        }

        private void _createPrereqColumn( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );

            string PrereqColName = "prereq";

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "modules", PrereqColName ) )
            {
                _CswNbtSchemaModTrnsctn.addLongColumn( "modules", PrereqColName, "Prerequisite for this module to be activated", false, false );
            }

            _resetBlame();
        }

        #endregion Yorick

        #region ASPEN

        private void _createNodeCountColumns( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );

            string NodeCountColName = "nodecount";
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetypes", NodeCountColName ) )
            {
                _CswNbtSchemaModTrnsctn.addLongColumn( "nodetypes", NodeCountColName, "The number of nodes", false, false );
            }

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "object_class", NodeCountColName ) )
            {
                _CswNbtSchemaModTrnsctn.addLongColumn( "object_class", NodeCountColName, "The number of nodes", false, false );
            }

            _resetBlame();
        }

        private void _createLoginDataTable( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );

            const string LoginDataTableName = "login_data";

            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( LoginDataTableName ) )
            {
                _CswNbtSchemaModTrnsctn.addTable( LoginDataTableName, "loginid" );
                _CswNbtSchemaModTrnsctn.getNewPrimeKey( LoginDataTableName );
            }
            if( _CswNbtSchemaModTrnsctn.isTableDefined( LoginDataTableName ) )
            {
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( LoginDataTableName, "username" ) )
                {
                    _CswNbtSchemaModTrnsctn.addStringColumn( LoginDataTableName, "username", "User's Username", false, false, 50 );
                }
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( LoginDataTableName, "ipaddress" ) )
                {
                    _CswNbtSchemaModTrnsctn.addStringColumn( LoginDataTableName, "ipaddress", "User's IP Address", false, false, 30 );
                }
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( LoginDataTableName, "logindate" ) )
                {
                    _CswNbtSchemaModTrnsctn.addDateColumn( LoginDataTableName, "logindate", "Date of Login Attempt", false, false );
                }
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( LoginDataTableName, "loginstatus" ) )
                {
                    _CswNbtSchemaModTrnsctn.addStringColumn( LoginDataTableName, "loginstatus", "Status of Login Attempt", false, false, 50 );
                }
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( LoginDataTableName, "failurereason" ) )
                {
                    _CswNbtSchemaModTrnsctn.addStringColumn( LoginDataTableName, "failurereason", "Reason for Login Failure", false, false, 100 );
                }
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( LoginDataTableName, "failedlogincount" ) )
                {
                    _CswNbtSchemaModTrnsctn.addLongColumn( LoginDataTableName, "failedlogincount", "Number of times user login has failed this session", false, false );
                }
            }

            _resetBlame();
        }

        #endregion ASPEN

    }//class RunBeforeEveryExecutionOfUpdater_01
}//namespace ChemSW.Nbt.Schema


